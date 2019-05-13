using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.ViewModels
{
    public class CertificateCreateViewModel
    {
        public CertificateCreateViewModel(
            IEnumerable<SelectListItem> staffList,
            IEnumerable<SelectListItem> channelList,
            IEnumerable<SelectListItem> promoList,
            IEnumerable<SelectListItem> customerNameList)
        {
            StaffList = staffList;
            ChannelList = channelList;
            PromoList = promoList;
            CustomerNameList = customerNameList;
        }

        public CertificateCreateViewModel() { }

        //certificate fields
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ExpiryDate { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Quantity")]
        public int CertQty { get; set; }

        //customer fields
        [Required]
        [Display(Name = "Customer Name")]
        [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
        public string CustomerName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }
        [Phone]
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }

        //dropdown input fields
        [Required]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }
        [Required]
        [Display(Name = "Channel")]
        public string ChannelName { get; set; }
        [Required]
        [Display(Name = "Promo")]
        public int PromoAmt { get; set; }

        //dropdown lists
        public IEnumerable<SelectListItem> StaffList { get; set; }
        public IEnumerable<SelectListItem> ChannelList { get; set; }
        public IEnumerable<SelectListItem> PromoList { get; set; }
        public IEnumerable<SelectListItem> CustomerNameList { get; set; }
    }
}
