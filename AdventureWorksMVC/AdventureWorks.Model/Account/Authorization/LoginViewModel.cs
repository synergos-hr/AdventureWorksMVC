using System.ComponentModel.DataAnnotations;
using AdventureWorks.Resources;

namespace AdventureWorks.Model.Account.Authorization
{
    public class LoginViewModel
    {
        //[Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        //[Display(Name = "Email", ResourceType = typeof(Fields))]
        //[EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "InvalidEmailAddress")]
        //public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "UserName", ResourceType = typeof(Fields))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Fields))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Fields))]
        public bool RememberMe { get; set; }
    }
}
