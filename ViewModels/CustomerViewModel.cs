using Certitrack.Models;
using System.Collections.Generic;

namespace Certitrack.ViewModels
{
    public class CustomerViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
