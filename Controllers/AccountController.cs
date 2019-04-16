using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Certitrack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Certitrack.Crypto;

namespace Certitrack
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        CertitrackContext db = new CertitrackContext();

        [HttpPost]
        public ActionResult Validate(Staff staff)
        {
            var _staff = db.Staff
                .Where(s => s.Email == staff.Email);

            if (_staff.Any())
            {
                var staffDbPw = _staff.Single().Password;
                
                if ( SecurePasswordHasherHelper.Verify(staff.Password, staffDbPw) )
                {

                    return Json(new { status = true, message = "Login Successfull!" });
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Password!" });
                }
            }
            else
            {
                return Json(new { status = false, message = "Invalid Email!" });
            }
        }

        [HttpPost]
        public ActionResult CreateUser(Staff staff)
        {
            string hashed_pw = SecurePasswordHasherHelper.Hash(staff.Password);
            staff.Password = hashed_pw;
            db.AddRange(staff.Name, staff.Email, staff.Password);

            return null;
        }
    }
}