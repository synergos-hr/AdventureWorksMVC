using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.Account.Authorization
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
