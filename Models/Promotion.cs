using System;
using System.Collections.Generic;

namespace Certitrack.Models
{
    public partial class Promotion
    {
        public Promotion()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        public int Discount { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
