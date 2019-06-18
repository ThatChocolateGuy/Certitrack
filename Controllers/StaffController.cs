using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Certitrack.Data;
using Certitrack.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Certitrack.ViewModels;
using Microsoft.EntityFrameworkCore;
using Certitrack.Crypto;
using Certitrack.Extensions.Alerts;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace Certitrack.Controllers
{
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
        public IActionResult Index()
        {
            try
            {
                // Staff list to pass to view
                List<Staff> staffList = new List<Staff>();
                
                // Populate staffList
                foreach (Staff staff in _context.Staff)
                {
                    // Get StaffLink record for current staff
                    staff.StaffLink = _context.StaffLink.Find(staff.Id);
                    // Get Role from StaffLink
                    staff.StaffLink.Role = _context.Role.Find(staff.StaffLink.RoleId);
                    // Get StaffType from StaffLink
                    staff.StaffLink.StaffType = _context.StaffType.Find(staff.StaffLink.StaffTypeId);

                    // Add Current Staff to List for View Render
                    staffList.Add(staff);
                }

                return View(staffList);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        // DISPLAY OPEN FIELDS TO EDIT (IF ADMIN)
        public IActionResult Edit(int? id)
        {
            var staff = _context.Staff.Find(id);
            var staffLink = _context.StaffLink.Find(id);

            var model = GetStaffCreateViewModel();
            
            model.Staff = staff;
            model.Staff.StaffLink = staffLink;

            return View(model);
        }
        // UPDATE ENTITY MODEL & DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Staff staff)
        {
            try //save staff to db
            {
                //entities to update
                var staffToUpdate = _context.Staff.Find(id);
                var staffLinkToUpdate = _context.StaffLink.Find(id);
                /*
                 * assign staff name & email
                 * to entity marked for update
                 */
                staffToUpdate.Name = staff.Name;
                staffToUpdate.Email = staff.Email;

                /*
                 * I've intentionally used both Fluent and standard LINQ
                 * syntax below to show equivalent methods of db querying
                 * (options are good, right?)
                 */

                //assign StaffLink RoleId
                staffLinkToUpdate.RoleId = _context.Role
                    .Where(r => r.Title == staff.StaffLink.Role.Title)
                    .Select(rId => rId.Id)
                    .Single();
                //assign StaffLink StaffTypeId
                staffLinkToUpdate.StaffTypeId = (
                    from sType in _context.StaffType
                    where sType.Type == staff.StaffLink.StaffType.Type
                    select sType.Id
                ).FirstOrDefault();
                
                //update db with changes
                _context.UpdateRange(staffToUpdate, staffLinkToUpdate);
                _context.SaveChanges();

                return RedirectToAction("Index").WithSuccess("Successful Update", staff.Name + " has been updated");
            }
            catch (Exception)
            {
                return RedirectToAction("Index").WithDanger("Update Not Successful", "Something went wrong. Try again.");
                throw;
            }
        }
        
        // STAFF REGISTRATION (IF ADMIN)
        public IActionResult Create()
        {
            //sets action for create view form submission
            ViewData["FormAction"] = "Create";

            var model = GetStaffCreateViewModel();

            return View(model);
        }
        // SUBMIT NEW STAFF TO DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            ViewData["FormAction"] = "Create";
            var model = GetStaffCreateViewModel();
            model.Staff = staff;

            if (!ModelState.IsValid)
                return View(model)
                    .WithWarning("Something's Not Right", "Check the form");

            try
            {
                // Create hashed pw from user input
                string hashed_pw = SecurePasswordHasherHelper.Hash(staff.Password);

                // Output Parameters for SQL Query
                var messageParam = new SqlParameter()
                {
                    ParameterName = "@messageOut",
                    Direction = ParameterDirection.Output,
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50
                };
                var staffCreatedParam = new SqlParameter()
                {
                    ParameterName = "@staffCreatedOut",
                    Direction = ParameterDirection.Output,
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.Int
                };

                var _staff = new Staff()
                {
                    UserName = staff.Email,
                    Password = staff.Password,
                    Email = staff.Email,
                    Name = staff.Name
                };
                
                IdentityResult result = await UserManager.CreateAsync(_staff, _staff.Password);
                if (result.Succeeded)
                {
                    // Executes stpAssignStaff Stored Procedure
                    _context.Database.ExecuteSqlCommand(
                        @"EXEC [dbo].[stpAssignStaff]
                         @staff_name = @name
                        ,@staff_email = @email
                        ,@staff_pw = @pw
                        ,@role_title = @rt
                        ,@staff_type = @st
                        ,@message_out = @messageOut OUTPUT
                        ,@staff_created = @staffCreatedOut OUTPUT"
                            , new SqlParameter("@name", staff.Name)
                            , new SqlParameter("@email", staff.Email)
                            , new SqlParameter("@pw", hashed_pw)
                            , new SqlParameter("@rt", staff.StaffLink.Role.Title)
                            , new SqlParameter("@st", staff.StaffLink.StaffType.Type)
                            , messageParam //debugging param
                            , staffCreatedParam //debugging param
                    );
                }
                else
                {
                    var e = result.Errors.Aggregate("User Creation Failed - Identity Exception. Errors were: \n\r\n\r",
                        (current, error) => current + (" - " + error + "\n\r"));
                    throw new Exception(e);
                }

                // Redirects to Staff Index w/ Success Alert
                if (staffCreatedParam.Value != null)
                    return RedirectToAction("Index")
                        .WithSuccess("Staff Added", "Welcome to the team, " + staff.Name + "!");
                // staff exists, etc. - Redirect to Staff Index w/ Error Alert
                else if ((string)TempData["OriginController"] != "Account")
                    return RedirectToAction("Index")
                        .WithDanger("Staff Not Added", messageParam.Value.ToString());
                else //staff exists, etc. - Redirect to Account Registration w/ Error Alert
                {
                    TempData["OriginController"] = null;
                    return RedirectToAction("Register", "Account")
                        .WithDanger("Staff Not Added", messageParam.Value.ToString());
                }
            }
            catch (Exception) { throw; }
        }
        
        // DELETE STAFF FROM DB (IF ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Delete(int id)
        {
            try
            {
                var staff = _context.Staff.Where(s => s.Id == id).Single();
                var staffLink = _context.StaffLink.Where(sl => sl.StaffId == id).Single();

                //deletes selected staff & associated staffLink record
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
            var roleTitles =
                from role in _context.Role.ToList()
                select new SelectListItem
                {
                    Text = role.Title,
                    Value = role.Title
                };
            var staffTypes =
                from sType in _context.StaffType.ToList()
                select new SelectListItem
                {
                    Text = sType.Type,
                    Value = sType.Type
                };

            // Create new StaffCreateViewModel with set list props
            var model = new StaffCreateViewModel(
                roleTitleList: roleTitles, staffTypeList: staffTypes);
            return model;
        }
    }
}