using Certitrack.Data;
using Certitrack.Extensions.Alerts;
using Certitrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly CertitrackContext _context;

        public OrdersController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, Customer> certitrackContext = _context.Order
                .Include(o => o.OrderItems)
                .Include(o => o.Customer);
            return View(await certitrackContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await _context.Order
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
            order.OrderItems = await _context.OrderItem
                .Include(oi => oi.Certificate).ThenInclude(cl => cl.CertificateLink)
                .Where(oi => oi.OrderId == order.Id).ToListAsync();
            foreach (OrderItem orderItem in order.OrderItems)
            {
                orderItem.Certificate.CertificateLink = await _context.CertificateLink
                    .Include(cl => cl.Staff)
                    .Include(cl => cl.Promotion)
                    .Include(cl => cl.Channel)
                    .FirstOrDefaultAsync(cl => cl.CertificateId == orderItem.CertificateId);
            }

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Email", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await _context.Order
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Name", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", "Order #" + order.Id + " updated successfully");
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Name", order.CustomerId);
            return View(order)
                .WithWarning("Uh-Oh!", "Something went wrong, try again.");
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await _context.Order
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            try
            {
                Order order = await _context.Order
                    .Include(o => o.OrderItems)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == id);

                System.Collections.Generic.List<Certificate> certificates = await _context.OrderItem
                    .Include(oi => oi.Certificate)
                    .Where(oi => oi.OrderId == id)
                    .Select(oi => oi.Certificate)
                    .ToListAsync();
                System.Collections.Generic.List<CertificateLink> cLinks = await _context.OrderItem
                    .Where(oi => oi.OrderId == id)
                    .Include(oi => oi.Certificate)
                    .Select(oi => oi.Certificate)
                    .Include(oi => oi.CertificateLink)
                    .Select(oi => oi.CertificateLink)
                    .ToListAsync();

                // must delete cert. links along with certs
                _context.CertificateLink.RemoveRange(cLinks);
                _context.Certificate.RemoveRange(certificates);
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                return "Order #" + order.Id + " for " + order.Customer.Name + " deleted";
            }
            catch (Exception e)
            {
                if (!OrderExists(id))
                {
                    return "Order Id #" + id + " doesn't exist";
                }
                else
                {
                    return e.ToString();
                    throw;
                }
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
