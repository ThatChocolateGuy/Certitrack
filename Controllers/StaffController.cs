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
        public CertitrackContext Context { get; }

        private UserManager<Staff> UserManager { get; set; }
        private SignInManager<Staff> SignInManager { get; set; }

        public StaffController(UserManager<Staff> userManager,
            SignInManager<Staff> signInManager, CertitrackContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Context = context;
        }

        // DISPLAY STAFF LIST
        public async Task<IActionResult> Index()
        {
            // Staff list to pass to view
            List<Staff> staffList = new List<Staff>();

            try
            {
                // Populate staffList
                await PopulateStaffList(staffList);
            }
            catch (Exception)
            {
                throw;
            }

            return View(staffList);
        }

        // DISPLAY OPEN FIELDS TO EDIT (IF ADMIN)
        public IActionResult Edit(int? id)
        {
            StaffCreateViewModel model = GetStaffCreateViewModel();

            model.Staff = Context.Staff.Find(id);
            model.Staff.StaffLink = Context.StaffLink.Find(id);

            return View(model);
        }
        // UPDATE ENTITY MODEL & DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Staff staff)
        {
            try //save staff changes to db
            {
                //entities to update
                Staff staffToUpdate = Context.Staff.Find(id);
                StaffLink staffLinkToUpdate = Context.StaffLink.Find(id);
                
				//assign staff name & email to entities marked for update
                staffToUpdate.Name = staff.Name;
                staffToUpdate.Email = staff.Email;

                /*
				 * I've intentionally used both Fluent and standard LINQ
				 * syntax below to show equivalent methods of db querying
				 * (options are good, right?)
				 */

                //assign StaffLink RoleId
                staffLinkToUpdate.RoleId = Context.Role
                    .Where(r => r.Title == staff.StaffLink.Role.Title)
                    .Select(rId => rId.Id)
                    .Single();
                //assign StaffLink StaffTypeId
                staffLinkToUpdate.StaffTypeId = (
                    from sType in Context.StaffType
                    where sType.Type == staff.StaffLink.StaffType.Type
                    select sType.Id
                ).FirstOrDefault();

                //update db with changes
                Context.UpdateRange(staffToUpdate, staffLinkToUpdate);
                Context.SaveChanges();

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
            //sets action for form submission
            ViewData["FormAction"] = "Create";
            return View(GetStaffCreateViewModel());
        }
        // SUBMIT NEW STAFF TO DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name", "Email", "Password", "Stafflink")] Staff staff_userInput)
        {
            ViewData["FormAction"] = "Create";
            if (staff_userInput is null)
            {
                throw new ArgumentNullException(nameof(staff_userInput));
            }

            StaffCreateViewModel model = GetStaffCreateViewModel();
            model.Staff = staff_userInput;

            if (!ModelState.IsValid)
            {
                return View(model)
                    .WithWarning("Something's Not Right", "Check the form");
            }

            // Create hashed pw from user input
            string hashed_pw = SecurePasswordHasherHelper.Hash(staff_userInput.Password);

            // Output parameters for db queries
            CreateSqlOutputParams(
                staff_userInput,
                hashed_pw,
                out SqlParameter messageParam,
                out SqlParameter staffCreatedParam,
                out Staff _staff);

            // attempt registration of new user with UserManager
            IdentityResult result = await UserManager.CreateAsync(_staff, hashed_pw);
            if (result.Succeeded)
            {
                // link role and staffType to newly created user
                try
                {
                    await CreateStaffLink(staff_userInput, _staff);
                }
                catch (Exception)
                {
                    throw;
                }

                staffCreatedParam.Value = 1; // set staff creation success flag
            }
            else
            {
                // Executes stpAssignStaff Stored Procedure as fallback linking method
                ExecStpAssignStaff(staff_userInput, hashed_pw, messageParam, staffCreatedParam);
            }

            // Redirects to [controller] Index w/ Success Alert
            if ((int)staffCreatedParam.Value == 1)
            {
                // Staff Index (if Admin)
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index")
                        .WithSuccess("Staff Added", staff_userInput.Name + " has been added to the team!");
                else // Certificates Index (if Non-Admin)
                {
                    try
                    {
                        await SignInManager.SignInAsync(await UserManager.FindByEmailAsync(staff_userInput.Email), false);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    return RedirectToAction("Certificates", "Index")
                        .WithSuccess("Staff Added", "Welcome to the team, " + staff_userInput.Name + "!");
                }
            }
            // staff exists, etc. - Redirect to Staff Index w/ Error Alert
            else if ((string)TempData["OriginController"] != "Account")
            {
                return RedirectToAction("Index")
                    .WithDanger("Staff Not Added", messageParam.Value.ToString());
            }
            else // staff exists, etc. - Redirect to Account Registration w/ Error Alert
            {
                TempData["OriginController"] = null;
                return RedirectToAction("Register", "Account")
                    .WithDanger("Staff Not Added", messageParam.Value.ToString());
            }
        }

        // DELETE STAFF FROM DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Delete(int id)
        {
            try
            {
                Staff staff = Context.Staff.Where(s => s.Id == id).Single();
                StaffLink staffLink = Context.StaffLink.Where(sl => sl.StaffId == id).Single();

                // deletes selected staff & associated staffLink record
                Context.RemoveRange(staff, staffLink);
                Context.SaveChanges();

                return staff.Name + " has been removed from the team";
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
            var roleTitles =
                from role in Context.Role.ToList()
                select new SelectListItem
                {
                    Text = role.Title,
                    Value = role.Title
                };
            var staffTypes =
                from sType in Context.StaffType.ToList()
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
        private async Task CreateStaffLink(Staff staff_userInput, Staff _newStaff)
        {
            Staff newStaff = await UserManager.FindByEmailAsync(_newStaff.Email);
            var role = await Context.Role
                .FirstOrDefaultAsync(r => r.Title == staff_userInput.StaffLink.Role.Title);
            var staffType = await Context.StaffType
                .FirstOrDefaultAsync(st => st.Type == staff_userInput.StaffLink.StaffType.Type);
            newStaff.StaffLink = new StaffLink
            {
                StaffId = newStaff.Id,
                RoleId = role.Id,
                StaffTypeId = staffType.Id
            };

            Context.StaffLink.Add(newStaff.StaffLink);
            await Context.SaveChangesAsync();
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
            _ = Context.Database.ExecuteSqlCommand(
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
        /// Generate Output Parameters for use with SQL Query
        /// </summary>
        /// <param name="staff_userInput"></param>
        /// <param name="hashed_pw"></param>
        /// <param name="messageParam"></param>
        /// <param name="staffCreatedParam"></param>
        /// <param name="_staff"></param>
        private static void CreateSqlOutputParams(Staff staff_userInput, string hashed_pw, out SqlParameter messageParam, out SqlParameter staffCreatedParam, out Staff _staff)
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
                UserName = staff_userInput.UserName ?? staff_userInput.Email,
                Email = staff_userInput.Email,
                Password = hashed_pw,
                Name = staff_userInput.Name
            };
        }

        /// <summary>
        /// Populate staffList with staffLink data to be pushed to view
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
                    _staff.StaffLink = await Context.StaffLink.FindAsync(staff.Id);
                    _staff.StaffLink.Role = await Context.Role.FindAsync(_staff.StaffLink.RoleId);
                    _staff.StaffLink.StaffType = await Context.StaffType.FindAsync(_staff.StaffLink.StaffTypeId);
                }
                // Add Current Staff to List for View Render
                staffList.Add(_staff);
            }
        }
    }
}