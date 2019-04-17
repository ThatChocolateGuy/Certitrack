using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Certitrack;

namespace Certitrack
{
    public class StaffController : Controller
    {
        CertitrackContext db = new CertitrackContext();

        // DISPLAY STAFF LIST
        public IActionResult Index()
        {
            var staffList = db.Staff.ToList(); // get params from staff_type also
            return View(staffList);
        }

        // DISPLAY STAFF DETAILS
        public IActionResult Details()
        {
            return View();
        }

        // DISPLAY OPEN FIELDS TO EDIT
        //public IActionResult Edit()
        //{
        //    var editFields = db.Staff.Where().ToList();

        //    return View(editFields);
        //}

        //// UPDATE ENTITY MODEL & DB
        //[HttpPost]
        //public IActionResult Edit(Staff staff)
        //{
        //    var editFields = db.Staff.Where().ToList();

        //    return View(editFields);
        //}

        //// CREATE NEW STAFF (IF ADMIN)
        //public IActionResult CreateStaff()
        //{
        //    return View(db.Staff.ToList(),);
        
    }
}