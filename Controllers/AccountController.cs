using Certitrack.Crypto;
using Certitrack.Data;
using Certitrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    public class AccountController : Controller
    {
        private readonly CertitrackContext _certitrackContext;

        private UserManager<Staff> UserManager { get; set; }
        private SignInManager<Staff> SignInManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }

        public AccountController(UserManager<Staff> userManager,
            SignInManager<Staff> signInManager, RoleManager<Role> roleManager, CertitrackContext certitrackContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _certitrackContext = certitrackContext;
        }

        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Total");
            }

            using (StaffController staffController = new StaffController(UserManager, SignInManager, _certitrackContext))
            {
                try
                {
                    Staff staff = await UserManager.FindByEmailAsync("admin@certitrack.com").ConfigureAwait(false);
                    if (staff == null)
                    {
                        Role role = await RoleManager.FindByNameAsync("Admin").ConfigureAwait(false);
                        StaffType staffType = await _certitrackContext.StaffType.FirstOrDefaultAsync().ConfigureAwait(false);

                        if (role == null)
                        {
                            role = new Role
                            {
                                Title = "Admin",
                                Description = "Can create, edit, delete anyone and anything. Basically, God mode.",
                                Name = "Admin"
                            };
                            await RoleManager.CreateAsync(role).ConfigureAwait(false);
                        }
                        if (staffType == null)
                        {
                            staffType = new StaffType
                            {
                                Type = "Head Therapist"
                            };
                            await _certitrackContext.StaffType.AddAsync(staffType).ConfigureAwait(false);
                            await _certitrackContext.SaveChangesAsync().ConfigureAwait(false);
                        }

                        StaffLink staffLink = new StaffLink
                        {
                            Role = role,
                            StaffType = staffType
                        };
                        staff = new Staff
                        {
                            UserName = "Admin",
                            Email = "admin@certitrack.com",
                            Name = "Admin",
                            Password = "admin123",
                            StaffLink = staffLink
                        };

                        await staffController.Create(staff).ConfigureAwait(false);
                        IdentityResult result = await UserManager.AddToRoleAsync(staff, "Admin").ConfigureAwait(false);
                        // output result to console - debugging
                        Console.WriteLine("UserManager.AddToRoleAsync - Result: " + result);
                        ViewBag.Message = "DB Seed Successful - Staff: '" + staff.Name + "' was created";
                    }
                }
                catch (DbUpdateConcurrencyException e)
                {
                    ViewBag.Message = e.Message;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Validate(Staff staff)
        {
            if (staff == null)
            {
                return NotFound();
            }

            Staff _staff = await UserManager.FindByEmailAsync(staff.Email).ConfigureAwait(false);

            if (_staff != null)
            {
                if (SecurePasswordHasherHelper.Verify(staff.Password, _staff.Password))
                {
                    try
                    {
                        await SignInManager.PasswordSignInAsync(_staff, _staff.Password, false, false).ConfigureAwait(false);
                        return Json(new
                        {
                            status = true,
                            message = "Login Successful!",
                            isAdmin = await UserManager.IsInRoleAsync(_staff, "Admin").ConfigureAwait(false)
                        });
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

        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Login", "Account");
        }
    }
}