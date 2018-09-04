using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.Account.Manage
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "StringFieldMinMaxLength")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Fields))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Fields))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "PasswordMissmatch")]
        public string ConfirmPassword { get; set; }
    }
}