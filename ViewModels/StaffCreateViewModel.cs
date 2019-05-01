using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;
using Certitrack.Crypto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Certitrack.ViewModels
{
    public partial class StaffCreateViewModel
    {
        public StaffCreateViewModel(
            IEnumerable<SelectListItem> roleTitleList,
            IEnumerable<SelectListItem> staffTypeList )
        {
            RoleTitleList = roleTitleList;
            StaffTypeList = staffTypeList;
        }

        public StaffCreateViewModel(Staff staff)
        {
            Staff = staff;
        }

        [Required]
        public Staff Staff { get; set; }

        public IEnumerable<SelectListItem> RoleTitleList { get; set; }
        public IEnumerable<SelectListItem> StaffTypeList { get; set; }
    }
}
