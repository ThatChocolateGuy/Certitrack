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

namespace Certitrack.Controllers
{
    public class StaffController : Controller
    {
        CertitrackContext db = new CertitrackContext();

        // DISPLAY STAFF LIST
        public IActionResult Index()
        {
            try
            {
                List<Staff> staffList = new List<Staff>(); // Staff list to pass to view
                var allStaff = db.Staff; // Get all staff from db

                foreach (Staff staff in allStaff)
                {
                    // Get StaffLink record for current staff
                    staff.StaffLink = db.StaffLink
                        .Where(sl => sl.StaffId == staff.Id).Single();
                    // Get Role from StaffLink
                    staff.StaffLink.Role = db.Role
                        .Where(r => r.Id == staff.StaffLink.RoleId).Single();
                    // Get StaffType from StaffLink
                    staff.StaffLink.StaffType = db.StaffType
                        .Where(st => st.Id == staff.StaffLink.StaffTypeId).Single();

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

        // DISPLAY OPEN FIELDS TO EDIT
        public IActionResult Edit()
        {
            return View();
        }
        // UPDATE ENTITY MODEL & DB
        [HttpPost]
        public IActionResult Edit(Staff staff)
        {
            var staffToUpdate = new Staff();
            return View();
        }

        // CREATE NEW STAFF (IF ADMIN)
        public IActionResult Create()
        {
            var model = GetStaffCreateViewModel();
            return View(model);
        }

        // StaffCreateViewModel Methods
        /// <summary>
        /// Gets a StaffCreateViewModel
        /// </summary>
        /// <returns>StaffCreateViewModel</returns>
        private StaffCreateViewModel GetStaffCreateViewModel()
        {
            Staff staff = new Staff();

            // Lists to pass into StaffCreateViewModel
            List<Role> roleTitleList = new List<Role>();
            List<StaffType> staffTypeList = new List<StaffType>();

            // Get Role Titles
            GetRoleTitleList(staff, roleTitleList);
            // Get Staff Types
            GetStaffTypeList(staff, staffTypeList);

            // Create SelectList for Role Titles
            IEnumerable<SelectListItem> roleTitleSelectList = CreateRoleTitleSelectList(roleTitleList);
            // Create SelectList for Staff Types
            IEnumerable<SelectListItem> sTypeSelectList = CreateStaffTypeSelectList(staffTypeList);

            // Create new StaffCreateViewModel with set list props
            var model = new StaffCreateViewModel(roleTitleSelectList, sTypeSelectList);
            return model;
        }
        /// <summary>
        /// Creates a Select List for Staff Type
        /// </summary>
        /// <param name="staffTypeList">Staff Type List to Fill</param>
        /// <returns>IEnumerable<SelectListItem></returns>
        private static IEnumerable<SelectListItem> CreateStaffTypeSelectList(List<StaffType> staffTypeList)
        {
            return from sType in staffTypeList
                   select new SelectListItem
                   {
                       Text = sType.Type,
                       Value = sType.Type
                   };
        }
        /// <summary>
        /// Creates a Select List for Role Title
        /// </summary>
        /// <param name="roleTitleList">Role Title List to Fill</param>
        /// <returns>IEnumerable<roleTitleList></returns>
        private static IEnumerable<SelectListItem> CreateRoleTitleSelectList(List<Role> roleTitleList)
        {
            return from role in roleTitleList
                   select new SelectListItem
                   {
                       Text = role.Title,
                       Value = role.Title
                   };
        }
        /// <summary>
        /// Gets List of Staff Types
        /// </summary>
        /// <param name="staff">Staff model instance to reference</param>
        /// <param name="staffTypeList">Staff Type list to populate</param>
        private void GetStaffTypeList(Staff staff, List<StaffType> staffTypeList)
        {
            foreach (StaffType st in db.StaffType)
            {
                StaffType staffType = new StaffType { Type = st.Type };
                StaffLink staffLink = new StaffLink { StaffType = staffType };
                staff.StaffLink = staffLink;
                staffTypeList.Add(staff.StaffLink.StaffType);
            }
        }
        /// <summary>
        /// Gets List of Role Titles
        /// </summary>
        /// <param name="staff">Staff model instance to reference</param>
        /// <param name="roleTitleList">Role Title list to populate</param>
        private void GetRoleTitleList(Staff staff, List<Role> roleTitleList)
        {
            foreach (Role r in db.Role)
            {
                Role role = new Role { Title = r.Title };
                StaffLink staffLink = new StaffLink { Role = role };
                staff.StaffLink = staffLink;
                roleTitleList.Add(staff.StaffLink.Role);
            }
        }
        //

        // CREATE NEW STAFF (IF ADMIN)
        [HttpPost]
        public IActionResult Create(Staff staff)
        {
            if (!ModelState.IsValid)
            {
                var model = GetStaffCreateViewModel();
                return View(model).WithWarning("Something's Not Right", "Check the form");
            }

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

            // Executes stpAssignStaff Stored Procedure
            db.Database.ExecuteSqlCommand(
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
                    , messageParam
                    , staffCreatedParam
                );

            // Redirects to Staff Index w/ Alert TempData
            if (staffCreatedParam.Value.Equals(1))
                return RedirectToAction("Index").WithSuccess("Staff Added", "Welcome to the team, " + staff.Name + "!");
            else
                return RedirectToAction("Index").WithDanger("Staff Not Added", messageParam.Value.ToString());
        }

        public IActionResult Delete()
        {
            return View();
        }
        // DELETE STAFF FROM DB (IF ADMIN)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }
    }
}