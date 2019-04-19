using System;
using System.Collections.Generic;

namespace Certitrack.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItem { get; set; }
    }
}
