using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.Account.Manage
{
    public class SetEmailViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        public int UserId { get; set; }

        [Display(Name = "OldEmail", ResourceType = typeof(Fields))]
        public string OldEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Display(Name = "NewEmail", ResourceType = typeof(Fields))]
        public string NewEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Display(Name = "ConfirmEmail", ResourceType = typeof(Fields))]
        public string ConfirmEmail { get; set; }
    }
}
