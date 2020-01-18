using Certitrack.Data;
using Certitrack.Extensions.Alerts;
using Certitrack.Models;
using Certitrack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    [Authorize]
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
            try
            {
                IEnumerable<CertificateLink> certificateDetails =
                        from link in await _context.CertificateLink.ToListAsync()
                        select new CertificateLink
                        {
                            Certificate = _context.Certificate.Find(link.CertificateId),
                            Channel = _context.Channel.Find(link.ChannelId),
                            Customer = _context.Customer.Find(link.CustomerId),
                            Promotion = _context.Promotion.Find(link.PromotionId),
                            Staff = _context.Staff.Find(link.StaffId)
                        };

                return View(certificateDetails);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Certificate certificate = await _context.Certificate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // POST: Certificates/GetCustomerEmail/5
        [HttpPost]
        public async Task<List<string>> GetCustomerEmail(string customerName)
        {
            if (customerName == null)
            {
                return null;
            }

            Customer customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.Name == customerName);

            List<string> customerDetails = new List<string>
            {
                customer.Email,
                customer.Phone
            };

            return customerDetails;
        }

        // GET: Certificates/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<SelectListItem> staffNames =
                from staff in await _context.Staff.ToListAsync()
                select new SelectListItem
                {
                    Text = staff.Name,
                    Value = staff.Name
                };
            IEnumerable<SelectListItem> channels =
                from channel in await _context.Channel.ToListAsync()
                select new SelectListItem
                {
                    Text = channel.ChannelName,
                    Value = channel.ChannelName
                };
            IEnumerable<SelectListItem> promos =
                from promo in await _context.Promotion.ToListAsync()
                select new SelectListItem
                {
                    Text = promo.Discount.ToString(),
                    Value = promo.Discount.ToString()
                };
            IEnumerable<SelectListItem> customerNames =
                from customer in await _context.Customer.ToListAsync()
                select new SelectListItem
                {
                    Text = customer.Name.ToString(),
                    Value = customer.Name.ToString()
                };

            CertificateCreateViewModel model = new CertificateCreateViewModel(
                staffList: staffNames,
                channelList: channels,
                promoList: promos,
                customerNameList: customerNames);

            return View(model);
        }

        // POST: Certificates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Price,CertQty,CustomerName,CustomerEmail,CustomerPhone,StaffName,ChannelName,PromoAmt"
            )] CertificateCreateViewModel certificateCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int result = _context.Database.ExecuteSqlCommand(@"
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

                return RedirectToAction(nameof(Index)).WithSuccess("Success!", "Certificate(s) issued for " + certificateCreateViewModel.CustomerName);
            }
            return View(certificateCreateViewModel).WithDanger("Uh-Oh!", "Something went wrong. Try again.");
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                CertificateLink certificateLink = await _context.CertificateLink.FindAsync(id);
                int orderId = _context.OrderItem
                    .Where(oi => oi.CertificateId == id).Single().OrderId;
                Order order = await _context.Order.FindAsync(orderId);

                IEnumerable<SelectListItem> staffList =
                    from staff in await _context.Staff.ToListAsync()
                    select new SelectListItem
                    {
                        Text = staff.Name,
                        Value = staff.Name
                    };
                IEnumerable<SelectListItem> promoList =
                    from discount in await _context.Promotion.ToListAsync()
                    select new SelectListItem
                    {
                        Text = discount.Discount.ToString(),
                        Value = discount.Discount.ToString()
                    };
                IEnumerable<SelectListItem> channelList =
                    from channel in await _context.Channel.ToListAsync()
                    select new SelectListItem
                    {
                        Text = channel.ChannelName,
                        Value = channel.ChannelName
                    };
                IEnumerable<SelectListItem> customerList =
                    from customer in await _context.Customer.ToListAsync()
                    select new SelectListItem
                    {
                        Text = customer.Name,
                        Value = customer.Name
                    };

                CertificateEditViewModel model = new CertificateEditViewModel()
                {
                    Certificate = _context.Certificate.Find(certificateLink.CertificateId),
                    Channel = _context.Channel.Find(certificateLink.ChannelId),
                    Customer = _context.Customer.Find(certificateLink.CustomerId),
                    Promotion = _context.Promotion.Find(certificateLink.PromotionId),
                    Order = order,
                    StaffList = staffList,
                    PromoList = promoList,
                    ChannelList = channelList,
                    CustomerList = customerList
                };

                if (model == null)
                {
                    return NotFound();
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Customer,Channel,Certificate,Price,Promotion,Staff")] CertificateEditViewModel certificateEditViewModel)
        {
            if (certificateEditViewModel == null)
            {
                throw new ArgumentNullException(nameof(certificateEditViewModel));
            }

            if (!CertificateExists(id))
            {
                return RedirectToAction(nameof(Edit))
                    .WithDanger("Update Not Successful", "Certificate Id: " + id + " doesn't exist");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Certificate certificate = await _context.Certificate.FindAsync(id);
                    CertificateLink certificateLink = await _context.CertificateLink.FindAsync(id);

                    Channel channel = await _context.Channel
                        .Where(ch => ch.Id == certificateLink.ChannelId)
                        .FirstOrDefaultAsync();
                    Customer customer = await _context.Customer
                        .Where(c => c.Id == certificateLink.CustomerId)
                        .FirstOrDefaultAsync();
                    Promotion promotion = await _context.Promotion
                        .Where(p => p.Id == certificateLink.PromotionId)
                        .FirstOrDefaultAsync();
                    Staff staff = await _context.Staff
                        .Where(s => s.Id == certificateLink.StaffId)
                        .FirstOrDefaultAsync();

                    certificate.ExpiryDate = certificateEditViewModel.Certificate.ExpiryDate;
                    certificate.DateRedeemed = certificateEditViewModel.Certificate.DateRedeemed;
                    certificate.Price = certificateEditViewModel.Certificate.Price;

                    certificateLink.ChannelId = await _context.Channel
                        .Where(ch => ch.ChannelName == certificateEditViewModel.Channel.ChannelName)
                        .Select(ch => ch.Id)
                        .FirstOrDefaultAsync();
                    certificateLink.CustomerId = await _context.Customer
                        .Where(c => c.Name == certificateEditViewModel.Customer.Name)
                        .Select(c => c.Id)
                        .FirstOrDefaultAsync();
                    certificateLink.PromotionId = await _context.Promotion
                        .Where(p => p.Discount == certificateEditViewModel.Promotion.Discount)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    certificateLink.StaffId = await _context.Staff
                        .Where(s => s.Name == certificateEditViewModel.Staff.Name)
                        .Select(s => s.Id)
                        .FirstOrDefaultAsync();

                    _context.UpdateRange(certificate, certificateLink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                //redirects back to customer details if origin view
                if (TempData["_ReturnRoute.Update.Controller"] != null)
                {
                    string message = null;
                    object _ReturnRouteController = TempData["_ReturnRoute.Update.Controller"];
                    TempData["_ReturnRoute.Update.Controller"] = null;

                    if ((string)_ReturnRouteController == "Certificates" ||
                        (string)_ReturnRouteController == "Orders")
                    {
                        message = "Certificate #" +
                            _context.Certificate.FindAsync(id).Result.CertificateNo + " updated successfully";
                    }

                    return Redirect("/" + _ReturnRouteController +
                            "/" + TempData["_ReturnRoute.Update.Action"] + "/" + TempData["_ReturnRoute.Update.Id"])
                                .WithSuccess("Update Successful", message);
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful",
                        "Certificate #" +
                        _context.Certificate.FindAsync(id).Result.CertificateNo +
                        " updated successfully");
            }
            return RedirectToAction(nameof(Index))
                .WithDanger("Update Not Successful", "Something went wrong. Try again.");
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            try
            {
                if (!CertificateExists(id))
                {
                    return "Certificate Id: " + id + " doesn't exist";
                }

                Certificate certificate = await _context.Certificate.FindAsync(id);
                CertificateLink certificateLink = await _context.CertificateLink.FindAsync(certificate.Id);
                Customer customer = await _context.Customer.FindAsync(certificateLink.CustomerId);

                _context.CertificateLink.Remove(certificateLink);
                _context.Certificate.Remove(certificate);
                await _context.SaveChangesAsync();

                return "Certificate #" + certificate.CertificateNo + " deleted for " + customer.Name;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: Certificates/Redeem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Redeem(int id)
        {
            try
            {
                if (!CertificateExists(id))
                {
                    return "Certificate Id: " + id + " doesn't exist";
                }

                Certificate certificate = await _context.Certificate.FindAsync(id);
                CertificateLink certificateLink = await _context.CertificateLink.FindAsync(certificate.Id);
                Customer customer = await _context.Customer.FindAsync(certificateLink.CustomerId);

                certificate.DateRedeemed = DateTime.Today;
                _context.Certificate.Update(certificate);
                await _context.SaveChangesAsync();

                return "Certificate #" + certificate.CertificateNo + " redeemed for " + customer.Name;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificate.Any(e => e.Id == id);
        }
    }
}
