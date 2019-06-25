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
            StaffController staffController = new StaffController(UserManager, SignInManager, _certitrackContext);
            
            try
            {
                Staff staff = await (UserManager.FindByEmailAsync("admin@certitrack.com"));
                if (staff == null)
                {
                    var staffLink = new StaffLink
                    {
                        Role = await _certitrackContext.Role.FirstOrDefaultAsync(),
                        StaffType = await _certitrackContext.StaffType.FirstOrDefaultAsync()
                    };
                    staff = new Staff
                    {
                        UserName = "Admin",
                        Email = "admin@certitrack.com",
                        Name = "Admin",
                        Password = "admin123",
                        StaffLink = staffLink
                    };

                    await staffController.Create(staff);
                    ViewBag.Message = "DB Seed Successful - Staff: '" + staff.Name + "' was created";
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(staffController.GetStaffCreateViewModel());
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Validate(Staff staff)
        {
            var _staff = await UserManager.FindByEmailAsync(staff.Email); // from db

            if (_staff != null)
            {
                if (SecurePasswordHasherHelper.Verify(staff.Password, _staff.Password))
                {
                    try
                    {
                        await SignInManager.PasswordSignInAsync(_staff, _staff.Password, false, false);
                        return Json(new { status = true, message = "Login Successfull!" });
                    }
                    catch (Exception e)
                    {
                        return Json(new { status = false, message = "Error: " + e.Message });
                        throw;
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Password!" });
                }
            }
            else
            {
                return Json(new { status = false, message = "Invalid Email!\n" });
            }
        }
    }
}