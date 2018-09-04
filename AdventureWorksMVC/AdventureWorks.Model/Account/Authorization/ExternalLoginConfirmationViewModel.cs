using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Model.Account.Authorization
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}