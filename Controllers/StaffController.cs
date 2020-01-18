using Certitrack.Crypto;
using Certitrack.Data;
using Certitrack.Extensions.Alerts;
using Certitrack.Models;
using Certitrack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly CertitrackContext _context;

        private UserManager<Staff> UserManager { get; set; }
        private SignInManager<Staff> SignInManager { get; set; }

        public StaffController(UserManager<Staff> userManager,
            SignInManager<Staff> signInManager, CertitrackContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _context = context;
        }

        // DISPLAY STAFF LIST
        public async Task<IActionResult> Index()
        {
            try
            {
                // Staff list to pass to view
                List<Staff> staffList = new List<Staff>();

                // Populate staffList
                await PopulateStaffList(staffList);

                return View(staffList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DISPLAY OPEN FIELDS TO EDIT (IF ADMIN)
        public IActionResult Edit(int? id)
        {
            Staff staff = _context.Staff.Find(id);
            StaffLink staffLink = _context.StaffLink.Find(id);

            StaffCreateViewModel model = GetStaffCreateViewModel();

            model.Staff = staff;
            model.Staff.StaffLink = staffLink;

            return View(model);
        }
        // UPDATE ENTITY MODEL & DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Staff staff)
        {
            try // save staff to db
            {
                // entities to update
                Staff staffToUpdate = _context.Staff.Find(id);
                StaffLink staffLinkToUpdate = _context.StaffLink.Find(id);

                // assign staff name & email to entities marked for update
                staffToUpdate.Name = staff.Name;
                staffToUpdate.Email = staff.Email;

                /*
				 * I've intentionally used both Fluent and standard LINQ
				 * syntax below to show equivalent db querying techniques
				 * (options are good, right?)
				 */

                // assign StaffLink RoleId
                staffLinkToUpdate.RoleId = _context.Role
                    .Where(r => r.Title == staff.StaffLink.Role.Title)
                    .Select(rId => rId.Id)
                    .Single();

                // assign StaffLink StaffTypeId
                staffLinkToUpdate.StaffTypeId = (
                    from sType in _context.StaffType
                    where sType.Type == staff.StaffLink.StaffType.Type
                    select sType.Id).FirstOrDefault();

                // update db with changes
                _context.UpdateRange(staffToUpdate, staffLinkToUpdate);
                _context.SaveChanges();

                return RedirectToAction("Index")
                    .WithSuccess("Successful Update", staff.Name + " has been updated");
            }
            catch (Exception)
            {
                return RedirectToAction("Index")
                    .WithDanger("Update Not Successful", "Something went wrong. Try again.");
                throw;
            }
        }

        // STAFF REGISTRATION (IF ADMIN)
        public IActionResult Create()
        {
            // sets action for create view form submission
            ViewData["FormAction"] = "Create";

            StaffCreateViewModel model = GetStaffCreateViewModel();

            return View(model);
        }

        // SUBMIT NEW STAFF TO DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            StaffCreateViewModel model = GetStaffCreateViewModel();
            model.Staff = staff;

            if (!ModelState.IsValid)
            {
                return View(model)
                    .WithWarning("Something's Not Right", "Check the form");
            }

            // Create hashed pw from user input
            string hashed_pw = SecurePasswordHasherHelper.Hash(staff.Password);

            // prepare output parameters for SQL query
            CreateOutputParams(
                staff,
                hashed_pw,
                out SqlParameter messageParam,
                out SqlParameter staffCreatedParam,
                out Staff _staff);

            // attempt registration of new user with UserManager
            IdentityResult result = await UserManager.CreateAsync(_staff, hashed_pw);
            if (result.Succeeded)
            {
                // link role and staffType to newly created user
                await createStaffLink(staff, _staff);
                staffCreatedParam.Value = 1; // set staff creation success flag
            }
            else
            {
                // executes stpAssignStaff Stored Procedure as fallback linking method
                ExecStpAssignStaff(staff, hashed_pw, messageParam, staffCreatedParam);
            }

            // determine redirect action
            return await ReturnRedirectCreate(staff, messageParam, staffCreatedParam);
        }

        // DELETE STAFF FROM DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Delete(int id)
        {
            try
            {
                Staff staff = _context.Staff.Where(s => s.Id == id).Single();
                StaffLink staffLink = _context.StaffLink.Where(sl => sl.StaffId == id).Single();

                // deletes selected staff & associated staffLink record
                _context.RemoveRange(staff, staffLink);
                _context.SaveChanges();

                return staff.Name + " has successfully been removed from the team";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a StaffCreateViewModel
        /// </summary>
        /// <returns>StaffCreateViewModel</returns>
        public StaffCreateViewModel GetStaffCreateViewModel()
        {
            IEnumerable<SelectListItem> roleTitles =
                from role in _context.Role.ToList()
                select new SelectListItem
                {
                    Text = role.Title,
                    Value = role.Title
                };
            IEnumerable<SelectListItem> staffTypes =
                from sType in _context.StaffType.ToList()
                select new SelectListItem
                {
                    Text = sType.Type,
                    Value = sType.Type
                };

            // Create new StaffCreateViewModel with set list props
            return new StaffCreateViewModel(
                roleTitleList: roleTitles, staffTypeList: staffTypes);
        }

        /// <summary>
        /// Creates staffLink for new staff from user selected values
        /// </summary>
        /// <param name="staff_userInput"></param>
        /// <param name="_newStaff"></param>
        /// <returns></returns>
        private async Task createStaffLink(Staff staff_userInput, Staff _newStaff)
        {
            Staff newStaff = await UserManager.FindByEmailAsync(_newStaff.Email);
            Role role = await _context.Role
                .FirstOrDefaultAsync(r => r.Title == staff_userInput.StaffLink.Role.Title);
            StaffType staffType = await _context.StaffType
                .FirstOrDefaultAsync(st => st.Type == staff_userInput.StaffLink.StaffType.Type);
            newStaff.StaffLink = new StaffLink
            {
                StaffId = newStaff.Id,
                RoleId = role.Id,
                StaffTypeId = staffType.Id
            };

            _context.StaffLink.Add(newStaff.StaffLink);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executes stpAssignStaff Stored Procedure
        /// </summary>
        /// <param name="staff_userInput"></param>
        /// <param name="hashed_pw"></param>
        /// <param name="messageParam"></param>
        /// <param name="staffCreatedParam"></param>
        private void ExecStpAssignStaff(Staff staff_userInput, string hashed_pw, SqlParameter messageParam, SqlParameter staffCreatedParam)
        {
            _ = _context.Database.ExecuteSqlCommand(
                    @"EXEC [dbo].[stpAssignStaff]
						 @staff_name = @name
						,@staff_email = @email
						,@staff_pw = @pw
						,@role_title = @rt
						,@staff_type = @st
						,@message_out = @messageOut OUTPUT
						,@staff_created = @staffCreatedOut OUTPUT"
                        , new SqlParameter("@name", staff_userInput.Name)
                        , new SqlParameter("@email", staff_userInput.Email)
                        , new SqlParameter("@pw", hashed_pw)
                        , new SqlParameter("@rt", staff_userInput.StaffLink.Role.Title)
                        , new SqlParameter("@st", staff_userInput.StaffLink.StaffType.Type)
                        , messageParam //debugging param
                        , staffCreatedParam //debugging param
                );
        }

        /// <summary>
        /// Returns redirect action for create method
        /// </summary>
        /// <param name="staff"></param>
        /// <param name="messageParam"></param>
        /// <param name="staffCreatedParam"></param>
        /// <returns>Task<IActionResult></returns>
        private async Task<IActionResult> ReturnRedirectCreate(Staff staff, SqlParameter messageParam, SqlParameter staffCreatedParam)
        {
            // Redirects to [controller] Index w/ Success Alert
            if ((int)staffCreatedParam.Value == 1)
            {
                // Staff Index (Admin)
                Staff createdStaff = await UserManager.FindByEmailAsync(staff.Email);
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index")
                        .WithSuccess("Staff Added", staff.Name + " has been added to the team!");
                }
                else // Certificates Index (Non-Admin)
                {
                    await SignInManager.SignInAsync(createdStaff, false);
                    return RedirectToAction("Certificates", "Index")
                        .WithSuccess("Staff Added", "Welcome to the team, " + staff.Name + "!");
                }
            }
            // staff exists, etc. - Redirect to Staff Index w/ Error Alert
            else if ((string)TempData["OriginController"] != "Account")
            {
                return RedirectToAction("Index")
                    .WithDanger("Staff Not Added", messageParam.Value.ToString());
            }
            else //staff exists, etc. - Redirect to Account Registration w/ Error Alert
            {
                TempData["OriginController"] = null;
                return RedirectToAction("Register", "Account")
                    .WithDanger("Staff Not Added", messageParam.Value.ToString());
            }
        }

        /// <summary>
        /// Populates StaffList to pass to view
        /// </summary>
        /// <param name="staffList"></param>
        /// <returns></returns>
        private async Task PopulateStaffList(List<Staff> staffList)
        {
            foreach (Staff staff in UserManager.Users)
            {
                Staff _staff = await UserManager.FindByIdAsync(staff.Id.ToString());
                if (_staff.StaffLink == null)
                {
                    _staff.StaffLink = await _context.StaffLink.FindAsync(staff.Id);
                    _staff.StaffLink.Role = await _context.Role.FindAsync(_staff.StaffLink.RoleId);
                    _staff.StaffLink.StaffType = await _context.StaffType.FindAsync(_staff.StaffLink.StaffTypeId);
                }
                // Add Current Staff to List for View Render
                staffList.Add(_staff);
            }
        }

        /// <summary>
        /// Prepare output parameters for later use with SQL query
        /// </summary>
        /// <param name="staff"></param>
        /// <param name="hashed_pw"></param>
        /// <param name="messageParam"></param>
        /// <param name="staffCreatedParam"></param>
        /// <param name="_staff"></param>
        private static void CreateOutputParams(Staff staff, string hashed_pw, out SqlParameter messageParam, out SqlParameter staffCreatedParam, out Staff _staff)
        {
            messageParam = new SqlParameter()
            {
                ParameterName = "@messageOut",
                Direction = ParameterDirection.Output,
                Value = DBNull.Value,
                SqlDbType = SqlDbType.VarChar,
                Size = 50
            };
            staffCreatedParam = new SqlParameter()
            {
                ParameterName = "@staffCreatedOut",
                Direction = ParameterDirection.Output,
                Value = DBNull.Value,
                SqlDbType = SqlDbType.Int
            };
            _staff = new Staff()
            {
                UserName = staff.UserName ?? staff.Email,
                Email = staff.Email,
                Password = hashed_pw,
                Name = staff.Name
            };
        }
    }
}