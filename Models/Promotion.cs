using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Promotion
    {
        public Promotion()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        [Display(Name = "Promo")]
        [DataType(DataType.Currency)]
        public int Discount { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
