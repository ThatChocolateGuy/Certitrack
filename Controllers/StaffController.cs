using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Certitrack.Data;
using Certitrack.Models;

namespace Certitrack.Controllers
{
    public class StaffController : Controller
    {
        CertitrackContext db = new CertitrackContext();

        // DISPLAY STAFF LIST
        public IActionResult Index()
        {
            List<Staff> staffList = new List<Staff>(); // Staff list to pass to view
            var staffs = db.Staff; // Get all staff from db

            foreach (Staff staff in staffs)
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


        // DISPLAY OPEN FIELDS TO EDIT
        public IActionResult Edit()
        {
            //var editFields = db.Staff.Where().ToList();

            return View();
        }
        // UPDATE ENTITY MODEL & DB
        [HttpPost]
        public IActionResult Edit(string name, string email, string password, string staff_type, string role_title)
        {
            //var editFields = db.Staff.Where().ToList();

            return View();
        }

        // CREATE NEW STAFF (IF ADMIN)
        public IActionResult CreateStaff()
        {
            return View();

        }
        // CREATE NEW STAFF (IF ADMIN)
        public IActionResult CreateStaff(string name, string email, string password, string staff_type, string role_title)
        {
            return View();

        }
    }
}