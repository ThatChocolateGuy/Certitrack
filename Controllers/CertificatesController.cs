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
using System.Data.SqlClient;
using Certitrack.Extensions.Alerts;

namespace Certitrack.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly CertitrackContext _context;

        public CertificatesController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Certificates
        public async Task<IActionResult> Index()
        {
            //var certificates = await _context.Certificate.ToListAsync();
            //var channels = await _context.Channel.ToListAsync();
            //var customers = await _context.Customer.ToListAsync();
            //var promotions = await _context.Promotion.ToListAsync();
            //var staffs = await _context.Staff.ToListAsync();

            var certificateDetails =
                from link in await _context.CertificateLink.ToListAsync()
                select new CertificateLink
                {
                    Certificate = _context.Certificate.Find(link.CertificateId),
                    Channel = _context.Channel.Find(link.ChannelId),
                    Customer = _context.Customer.Find(link.CustomerId),
                    Promotion = _context.Promotion.Find(link.PromotionId),
                    Staff = _context.Staff.Find(link.Staff)
                };

            return View(certificateDetails);
        }

        // GET: Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // GET: Certificates/Create
        public IActionResult Create()
        {
            var staffNames =
                from staff in _context.Staff.ToList()
                select new SelectListItem
                {
                    Text = staff.Name,
                    Value = staff.Name
                };
            var channels =
                from channel in _context.Channel.ToList()
                select new SelectListItem
                {
                    Text = channel.ChannelName,
                    Value = channel.ChannelName
                };
            var promos =
                from promo in _context.Promotion.ToList()
                select new SelectListItem
                {
                    Text = promo.Discount.ToString(),
                    Value = promo.Discount.ToString()
                };

            var model = new CertificateCreateViewModel(staffList: staffNames, channelList: channels, promoList: promos);

            return View(model);
        }

        // POST: Certificates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ExpiryDate,Price,CertQty,CustomerName,CustomerEmail,CustomerPhone,StaffName,ChannelName,PromoAmt")] CertificateCreateViewModel certificateCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.ExecuteSqlCommand(@"
                    EXEC stpAssignCertificate
                        @customer_name = @customerName
                        ,@customer_email = @customerEmail
                        ,@customer_phone = @customerPhone
                        ,@staff_name = @staffName
                        ,@promo_discount = @promo
                        ,@channel = @channelName
                        ,@cert_price = @price
                        ,@cert_qty = @qty"
                            , new SqlParameter("@customerName", certificateCreateViewModel.CustomerName)
                            , new SqlParameter("@customerEmail", certificateCreateViewModel.CustomerEmail)
                            , new SqlParameter("@customerPhone", certificateCreateViewModel.CustomerPhone)
                            , new SqlParameter("@staffName", certificateCreateViewModel.StaffName)
                            , new SqlParameter("@promo", certificateCreateViewModel.PromoAmt)
                            , new SqlParameter("@channelName", certificateCreateViewModel.ChannelName)
                            , new SqlParameter("@price", certificateCreateViewModel.Price)
                            , new SqlParameter("@qty", certificateCreateViewModel.CertQty)
                    );
                }
                catch (Exception) { throw; }

                return RedirectToAction(nameof(Index)).WithSuccess("Success!", "Certificate created for " + certificateCreateViewModel.CustomerName);
            }
            return View(certificateCreateViewModel).WithDanger("Certificate Not Created", "Something went wrong. Try again.");
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificate.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }
            return View(certificate);
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CertificateNo,DateIssued,DateRedeemed,ExpiryDate,Price")] Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(certificate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(certificate);
        }

        // GET: Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _context.Certificate.FindAsync(id);
            _context.Certificate.Remove(certificate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificate.Any(e => e.Id == id);
        }
    }
}
