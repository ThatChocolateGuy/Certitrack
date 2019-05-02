using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;

namespace Certitrack.ViewModels
{
    public class CertificateEditViewModel
    {
        public Certificate Certificate { get; set; }
        public Channel Channel { get; set; }
        public Customer Customer { get; set; }
        public Promotion Promotion { get; set; }
        public Staff Staff { get; set; }
        public Order Order { get; set; }


    }
}
