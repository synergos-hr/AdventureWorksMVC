using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdventureWorks.Model.ViewModels;
using AdventureWorks.Resources;

namespace AdventureWorks.Web.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Display(Name = "Email", ResourceType = typeof(Fields))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "UserName", ResourceType = typeof(Fields))]
        public string UserName { get; set; }

        public string UserCode { get; set; }

        public string DomainUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "StringFieldMinMaxLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Fields))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Fields))]
        [Compare("Password", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "PasswordMissmatch")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Fields))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Fields))]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        public bool SendEmailToUser { get; set; }

        public IList<UserRolesViewModel> RolesList { get; set; }

        //public IEnumerable<System.Web.Mvc.SelectListItem> RolesList { get; set; }

        public string FullName => FirstName + " " + LastName;
    }
}
