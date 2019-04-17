using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Certitrack;

namespace certitrack_certificate_manager.Controllers
{
    public class StaffController : Controller
    {
        CertitrackContext db = new CertitrackContext();

        public IActionResult Index()
        {
            return View();
        }
    }
}