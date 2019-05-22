using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Certificate
    {
        public Certificate()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Certificate No")]
        public decimal CertificateNo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Issued")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateIssued { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Redeemed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateRedeemed { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Expiry")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ExpiryDate { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public CertificateLink CertificateLink { get; set; }
        public ICollection<OrderItem> OrderItem { get; set; }
    }
}
