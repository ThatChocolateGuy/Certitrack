using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Certitrack.ViewModels
{
    public class CertificateEditViewModel
    {
        public Certificate Certificate { get; set; }
        public Channel Channel { get; set; }
        public Customer Customer { get; set; }
        public Promotion Promotion { get; set; }
        public Staff Staff { get; set; }
        public Order Order { get; set; }

        //dropdown lists
        public IEnumerable<SelectListItem> StaffList { get; set; }
        public IEnumerable<SelectListItem> PromoList { get; set; }
        public IEnumerable<SelectListItem> ChannelList { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
    }
}
