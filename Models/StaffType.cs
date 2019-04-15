using System;
using System.Collections.Generic;

namespace Certitrack
{
    public partial class StaffType
    {
        public StaffType()
        {
            StaffLink = new HashSet<StaffLink>();
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public ICollection<StaffLink> StaffLink { get; set; }
    }
}
