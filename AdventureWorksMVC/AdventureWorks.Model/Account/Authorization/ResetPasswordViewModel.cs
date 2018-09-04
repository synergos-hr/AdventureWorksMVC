using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.Account.Authorization
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "StringFieldMinLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "PasswordMissmatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}