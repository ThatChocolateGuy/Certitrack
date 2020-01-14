using System;
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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

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
                var certificateDetails =
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