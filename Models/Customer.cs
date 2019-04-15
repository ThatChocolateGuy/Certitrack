using System;
using System.Collections.Generic;

namespace Certitrack
{
    public partial class Customer
    {
        public Customer()
        {
            CertificateLink = new HashSet<CertificateLink>();
            Order = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
        public ICollection<Order> Order { get; set; }
    }
}
