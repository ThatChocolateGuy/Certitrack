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
            Staff staff = new Staff();
            // Lists to pass into StaffCreateViewModel
            List<Role> roleTitleList = new List<Role>();
            List<StaffType> staffTypeList = new List<StaffType>();
            
            // Get Role Titles
            foreach (Role r in db.Role)
            {
                Role role = new Role { Title = r.Title };
                StaffLink staffLink = new StaffLink { Role = role };
                staff.StaffLink = staffLink;
                roleTitleList.Add(staff.StaffLink.Role);
            }
            // Get Staff Types
            foreach (StaffType st in db.StaffType)
            {
                StaffType staffType = new StaffType { Type = st.Type };
                StaffLink staffLink = new StaffLink { StaffType = staffType };
                staff.StaffLink = staffLink;
                staffTypeList.Add(staff.StaffLink.StaffType);
            }

            // Create SelectList for Role Titles
            IEnumerable<SelectListItem> roleTitleSelectList =
                from role in roleTitleList
                select new SelectListItem
                {
                    Text = role.Title,
                    Value = role.Title
                };
            // Create SelectList for Staff Types
            IEnumerable<SelectListItem> sTypeSelectList =
                from sType in staffTypeList
                select new SelectListItem
                {
                    Text = sType.Type,
                    Value = sType.Type
                };

            // Create new StaffCreateViewModel with set list props
            var model = new StaffCreateViewModel(roleTitleSelectList, sTypeSelectList);

            return View(model);
        }
        // CREATE NEW STAFF (IF ADMIN)
        [HttpPost]
        public IActionResult Create(Staff staff)
        { // USE STP FOR STAFF & LINK POPULATION

            //var staffJson = JObject.Parse(Json(staff).Value.ToString());
            //var staffConvert = JObject.FromObject(staff);

            //var staffToCreate = new Staff
            //{
            //    Name = staffJson.Property("name").Value.ToString(),
            //    Email = staffJson.Property("email").Value.ToString(),
            //    Password = staffJson.Property("password").Value.ToString(),

            //    StaffLink = new StaffLink
            //    {
            //        Role = new Role
            //        {
            //            Title = staffJson["staffLink"][0]["Role"][0]["Title"].ToString()
            //        },

            //        StaffType = new StaffType
            //        {
            //            Type = staffJson["staffLink"][0]["StaffType"][0]["Type"].ToString()
            //        }
            //    }
            //};

            return Json(staff);
            //return RedirectToAction("Index");
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