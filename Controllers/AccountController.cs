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
using Microsoft.AspNetCore.Identity;

namespace Certitrack.Controllers
{
    public class AccountController : Controller
    {
        private readonly CertitrackContext _certitrackContext;

        private UserManager<Staff> UserManager { get; set; }
        private SignInManager<Staff> SignInManager { get; set; }

        public AccountController(UserManager<Staff> userManager,
            SignInManager<Staff> signInManager, CertitrackContext certitrackContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _certitrackContext = certitrackContext;
        }

        public async Task<IActionResult> Register()
        {
            try
            {
                ViewBag.Message = "User already registered";

                Staff staff = await (UserManager.FindByEmailAsync("admin@certitrack.com"));
                if (staff == null)
                {
                    staff = new Staff
                    {
                        UserName = "Admin",
                        Email = "admin@certitrack.com",
                        Name = "Admin"
                    };

                    IdentityResult result = await UserManager.CreateAsync(staff, "admin123");
                    ViewBag.Message = "Staff '" + staff.Name + "' was created";
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            StaffController staffController = new StaffController(UserManager, SignInManager ,_certitrackContext);
            return View(staffController.GetStaffCreateViewModel());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Validate(Staff staff)
        {
            var _staff = _certitrackContext.Staff.Where(s => s.Email == staff.Email).FirstOrDefaultAsync().Result;
            var verify = SecurePasswordHasherHelper.Verify(staff.Password, _staff.Password);
            //why does this fail?
            var result = await SignInManager.PasswordSignInAsync(staff, staff.Password, false, false);

            if (_staff != null)
            {
                if (SecurePasswordHasherHelper.Verify(staff.Password, _staff.Password))
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
                return Json(new { status = false, message = "Invalid Email!\n" + result.ToString() });
            }
        }
    }
}