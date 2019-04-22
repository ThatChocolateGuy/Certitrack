using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Staff
    {
        public Staff()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime Created { get; set; }

        public StaffLink StaffLink { get; set; }
        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
