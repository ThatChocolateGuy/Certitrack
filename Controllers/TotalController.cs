using Certitrack.Data;
using Certitrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Controllers
{
    [Authorize]
    public class TotalController : Controller
    {
        private readonly CertitrackContext _context;

        public TotalController(CertitrackContext context)
        {
            _context = context;
        }
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
    }
}