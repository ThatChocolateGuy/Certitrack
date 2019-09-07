using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Certitrack.Data;
using Certitrack.Models;
using Certitrack.ViewModels;
using Certitrack.Extensions.Alerts;
using Microsoft.AspNetCore.Authorization;

namespace certitrack_certificate_manager.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly CertitrackContext _context;

        public CustomersController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers =
                from customer in await _context.Customer.ToListAsync()
                select new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Orders = _context.Order
                        .Where(o => o.CustomerId == customer.Id).ToList()
                };

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            var orders = await
                (from order in _context.Order
                where order.CustomerId == customer.Id
                select new Order
                {
                    CustomerId = customer.Id,
                    Id = order.Id,
                    OrderItems = _context.OrderItem
                        .Where(oi => oi.OrderId == order.Id).ToList(),
                }).ToListAsync();

            foreach (var order in orders)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Certificate =
                        await _context.Certificate.FindAsync(orderItem.CertificateId);
                    orderItem.Certificate.CertificateLink =
                        await _context.CertificateLink.FindAsync(orderItem.CertificateId);

                    orderItem.Certificate.CertificateLink.Staff =
                        await _context.Staff.FindAsync(orderItem.Certificate.CertificateLink.StaffId);
                    orderItem.Certificate.CertificateLink.Promotion =
                        await _context.Promotion.FindAsync(orderItem.Certificate.CertificateLink.PromotionId);
                    orderItem.Certificate.CertificateLink.Channel =
                        await _context.Channel.FindAsync(orderItem.Certificate.CertificateLink.ChannelId);
                }
            }

            var model = new CustomerViewModel
            {
                Customer = customer,
                Orders = orders
            };

            return View(model);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Registration Successful", customer.Name + " was successfully registered as a new customer.");
            }
            return View(customer)
                .WithWarning("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (!CustomerExists(id))
                return RedirectToAction(nameof(Edit))
                    .WithDanger("Update Not Successful", "Customer Id: " + id + " doesn't exist");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", customer.Name + " updated successfully");
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            if (!CustomerExists(id))
                return "Customer Id: " + id + " doesn't exist";
            try
            {
                var customer = await _context.Customer.FindAsync(id);
                var orders = await _context.Order
                    .Where(o => o.CustomerId == id).ToListAsync();
                List<OrderItem> orderItems = null;
                List<Certificate> certificates = new List<Certificate>();
                List<CertificateLink> certificateLinks = new List<CertificateLink>();

                foreach (var order in orders)
                {
                    orderItems = await _context.OrderItem
                        .Where(oi => oi.OrderId == order.Id).ToListAsync();
                    foreach (var orderItem in orderItems)
                    {
                        var certificate = await _context.Certificate
                            .Where(c => c.Id == orderItem.CertificateId).FirstOrDefaultAsync();
                        var certificateLink = await _context.CertificateLink.FindAsync(certificate.Id);
                        certificates.Add(certificate);
                        certificateLinks.Add(certificateLink);
                    }
                }

                _context.CertificateLink.RemoveRange(certificateLinks);
                _context.Certificate.RemoveRange(certificates);
                _context.OrderItem.RemoveRange(orderItems);
                _context.Order.RemoveRange(orders);
                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();

                return customer.Name + " deleted";
            }
            catch (Exception e)
            {
                return e.ToString();
                throw;
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }
    }
}
