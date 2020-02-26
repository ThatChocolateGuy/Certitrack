﻿using Certitrack.Data;
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

                ViewBag.Certs = certificateDetails.Select(m => m.Certificate).Distinct().Count();
                ViewBag.Channel = certificateDetails.Select(m => m.Channel).Distinct().Count();
                ViewBag.Customer = certificateDetails.Select(m => m.Customer).Distinct().Count();
                ViewBag.Promo = certificateDetails.Select(m => m.Promotion).Distinct().Count();

                ViewBag.Staff = _context.Staff.Distinct().Count();
                ViewBag.Orders = _context.Order.Distinct().Count();
                ViewBag.Promos = _context.Promotion.Distinct().Count() - 1; // account for default $0 promo

                ViewBag.Certs = _context.Certificate.Distinct().Count();
                ViewBag.CertsProgress = decimal.Divide(certificateDetails.Select(m => m.Certificate).Where(c => c.DateRedeemed.HasValue).Count(), certificateDetails.Count()) * 100;
                ViewBag.CertsRemainder = certificateDetails.Count() - certificateDetails.Select(m => m.Certificate).Where(c => c.DateRedeemed.HasValue).Count();

                // Certificates by Promotion data
                IEnumerable<Promotion> promos = certificateDetails.Select(m => m.Promotion).Distinct();
                IEnumerable<int> certsByPromoLabels = certificateDetails.Select(m => m.Promotion.Discount).Distinct();
                List<int> certsByPromoArray = new List<int>();
                certsByPromoArray.AddRange(
                    promos.Select(
                        promo => certificateDetails.Select(
                            m => m.Certificate).Where(
                            c => c.CertificateLink.Promotion.Discount == promo.Discount).Count()));
                ViewBag.CertsByPromoArray = string.Join(",", certsByPromoArray);
                ViewBag.CertsByPromoLabels = string.Concat(certsByPromoLabels.Select(i => string.Format("`${0} discount`,", i)));

                // Certificates by Channel data
                IEnumerable<Channel> channels = certificateDetails.Select(m => m.Channel).Distinct();
                IEnumerable<string> certsByChannelLabels = certificateDetails.Select(m => m.Channel.ChannelName).Distinct();
                List<int> certsByChannelArray = new List<int>();
                certsByChannelArray.AddRange(
                    channels.Select(
                        channel => certificateDetails.Select(
                            m => m.Certificate).Where(
                            c => c.CertificateLink.Channel.ChannelName == channel.ChannelName).Count()));
                ViewBag.CertsByChannelArray = string.Join(",", certsByChannelArray);
                ViewBag.CertsByChannelLabels = string.Concat(certsByChannelLabels.Select(i => string.Format("`{0}`,", i)));

                return View(certificateDetails);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}