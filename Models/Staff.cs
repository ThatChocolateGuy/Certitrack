using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Staff : IdentityUser<int>
    {
        public Staff()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public override int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
        public string Name { get; set; }
        //[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public override string Email { get; set; }
        //[Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
        public string Password { get; set; }
        public DateTime Created { get; set; }

        public StaffLink StaffLink { get; set; }
        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
