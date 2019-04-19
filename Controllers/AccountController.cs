using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Certitrack.Crypto;
using Certitrack.Data;

namespace Certitrack.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        CertitrackContext db = new CertitrackContext();

        [HttpPost]
        public IActionResult Validate(Staff staff)
        {
            var _staff = db.Staff.Where(s => s.Email == staff.Email);

            if (_staff.Any())
            {
                if ( SecurePasswordHasherHelper.Verify(staff.Password, _staff.Single().Password) )
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
    }
}