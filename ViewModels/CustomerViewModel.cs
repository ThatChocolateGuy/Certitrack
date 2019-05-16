using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Certitrack.ViewModels
{
    public class CustomerViewModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }

        public IEnumerable<Customer> CustomerList { get; set; }
        public IEnumerable<Order> OrderList { get; set; }
    }
}
