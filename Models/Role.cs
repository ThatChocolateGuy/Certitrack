using System;
using System.Collections.Generic;

namespace Certitrack.Models
{
    public partial class Role
    {
        public Role()
        {
            StaffLink = new HashSet<StaffLink>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<StaffLink> StaffLink { get; set; }
    }
}
