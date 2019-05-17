using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Customer
    {
        public Customer()
        {
            CertificateLink = new HashSet<CertificateLink>();
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        [Display(Name = "Customer Name")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
