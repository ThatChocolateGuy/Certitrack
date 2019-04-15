using System;
using System.Collections.Generic;

namespace Certitrack
{
    public partial class Certificate
    {
        public Certificate()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public decimal CertificateNo { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime? DateRedeemed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }

        public CertificateLink CertificateLink { get; set; }
        public ICollection<OrderItem> OrderItem { get; set; }
    }
}
