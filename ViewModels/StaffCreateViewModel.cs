using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Certitrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Certitrack.ViewModels
{
    public partial class StaffCreateViewModel
    {
        public StaffCreateViewModel(
            IEnumerable<SelectListItem> roleTitleList, IEnumerable<SelectListItem> staffTypeList)
        {
            this.RoleTitleList = roleTitleList;
            this.StaffTypeList = staffTypeList;
        }

        [Required]
        public IEnumerable<SelectListItem> RoleTitleList { get; set; }
        [Required]
        public IEnumerable<SelectListItem> StaffTypeList { get; set; }

        public Staff Staff { get; set; }

        public string Role { get; set; }
        public string StaffType { get; set; }
    }
}
