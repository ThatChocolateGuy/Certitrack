using Certitrack.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.ViewModels
{
    public partial class StaffCreateViewModel
    {
        public StaffCreateViewModel(
            IEnumerable<SelectListItem> roleTitleList,
            IEnumerable<SelectListItem> staffTypeList)
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
