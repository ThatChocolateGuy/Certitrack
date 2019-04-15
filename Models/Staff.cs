using System;
using System.Collections.Generic;

namespace Certitrack
{
    public partial class Staff
    {
        public Staff()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }

        public StaffLink StaffLink { get; set; }
        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
