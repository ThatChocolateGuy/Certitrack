using System;
using System.Collections.Generic;

namespace Certitrack
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CertificateId { get; set; }

        public Certificate Certificate { get; set; }
        public Order Order { get; set; }
    }
}
