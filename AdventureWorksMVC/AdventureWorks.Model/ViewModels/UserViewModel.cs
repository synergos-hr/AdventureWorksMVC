using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Fields))]
        public string UserName { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Fields))]
        public string Email { get; set; }

        public string DomainUserName { get; set; }

        public string UserCode { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Fields))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Fields))]
        public string LastName { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Fields))]
        public string FullName { get; set; }

        public string DisplayName { get; set; }

        public int? RegionId { get; set; }

        public string RegionName { get; set; }

        public int? DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int? WorkPositionId { get; set; }

        public string WorkPositionName { get; set; }

        [Display(Name = "Locked", ResourceType = typeof(Fields))]
        public bool Locked { get; set; }

        public string RolesTxt { get; set; }

        public virtual List<RoleViewModel> AspNetRoles { get; set; }

        //public string RolesDisplayTxt { get { return AspNetRoles != null ? string.Join(",", AspNetRoles.Select(x => x.DisplayName)) : "-"; } }
    }
}
