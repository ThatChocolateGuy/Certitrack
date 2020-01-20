using Certitrack.Data;
using Certitrack.Extensions.Alerts;
using Certitrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly CertitrackContext _context;

        public PromotionsController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Promotions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Promotion.ToListAsync());
        }

        // GET: Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Promotion promotion = await _context.Promotion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // GET: Promotions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Promotions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Discount")] Promotion promotion)
        {
            if (DiscountExists(promotion.Discount))
            {
                return View(promotion)
                    .WithWarning("Promo Exists", "An equivalent promotion exists. Try a different value.");
            }

            if (ModelState.IsValid)
            {
                if (promotion.Discount > 0)
                {
                    _context.Add(promotion);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return View(promotion)
                        .WithWarning("Invalid Promo", "Promo must be a value greater than zero.");
                }

                return RedirectToAction(nameof(Index))
                    .WithSuccess("Success", "$" + promotion.Discount + " promotion successfully created."); ;
            }
            return View(promotion)
                .WithWarning("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Promotion promotion = await _context.Promotion.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Discount")] Promotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }
            if (DiscountExists(promotion.Discount))
            {
                return View(promotion)
                    .WithWarning("Promo Exists", "An equivalent promotion exists. Try a different value.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion.Discount > 0)
                    {
                        _context.Update(promotion);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return View(promotion)
                            .WithWarning("Invalid Promo", "Promo must be a value greater than zero.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", "Promotion successfully changed to $" + promotion.Discount);
            }
            return View(promotion)
                .WithWarning("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Promotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Promotion promotion = await _context.Promotion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            try
            {
                Promotion promotion = await _context.Promotion.FindAsync(id);
                _context.Promotion.Remove(promotion);
                await _context.SaveChangesAsync();
                return "The $" + promotion.Discount + " promotion was deleted successfully";
            }
            catch (Exception e)
            {
                if (!PromotionExists(id))
                {
                    return "Promotion Id #" + id + " doesn't exist";
                }
                else
                {
                    return e.ToString();
                    throw;
                }
            }
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotion.Any(e => e.Id == id);
        }

        private bool DiscountExists(int discount)
        {
            return _context.Promotion.Any(e => e.Discount == discount);
        }
    }
}
