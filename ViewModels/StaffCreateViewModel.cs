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
            IEnumerable<SelectListItem> roleTitleList,
            IEnumerable<SelectListItem> staffTypeList )
        {
            this.RoleTitleList = roleTitleList;
            this.StaffTypeList = staffTypeList;
        }

        public StaffCreateViewModel(Staff staff)
        {
            Staff = staff;
        }

        [Required]
        public IEnumerable<SelectListItem> RoleTitleList { get; set; }
        [Required]
        public IEnumerable<SelectListItem> StaffTypeList { get; set; }

        [Required]
        public Staff Staff { get; set; }

        //TODO: hash pw before passing to controller if possible
    }
}
