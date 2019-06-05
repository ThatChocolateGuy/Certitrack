using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Certitrack.Data;
using Certitrack.Models;
using Certitrack.Extensions.Alerts;

namespace certitrack_certificate_manager.Controllers
{
    public class RolesController : Controller
    {
        private readonly CertitrackContext _context;

        public RolesController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Role.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] Role role)
        {
            if (RoleExists(role.Title))
                return View(role)
                    .WithWarning("Role Exists", "A role with the same title exists. Try a different title.");
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", role.Title + " role successfully updated.");
            }
            return View(role)
                .WithWarning("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }
            if (RoleExists(role.Title) && RoleDescriptionExists(role.Description))
                return View(role)
                    .WithWarning("Role Exists", "A role with the same title & description exists. Try a different title.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", role.Title + " role successfully updated.");
            }
            return View(role)
                .WithWarning("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            try
            {
                var role = await _context.Role.FindAsync(id);
                _context.Role.Remove(role);
                await _context.SaveChangesAsync();
                return "The " + role.Title + " role was deleted successfully";
            }
            catch (Exception e)
            {
                if (!RoleExists(id))
                {
                    return "Role Id #" + id + " doesn't exist";
                }
                else
                {
                    return e.ToString();
                    throw;
                }
            }
        }

        private bool RoleExists(int id)
        {
            return _context.Role.Any(e => e.Id == id);
        }

        private bool RoleExists(string title)
        {
            return _context.Role.Any(e => e.Title == title);
        }

        private bool RoleDescriptionExists(string description)
        {
            return _context.Role.Any(e => e.Description == description);
        }
    }
}
