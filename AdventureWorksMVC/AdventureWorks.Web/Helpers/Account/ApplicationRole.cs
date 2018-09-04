using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdventureWorks.Web.Helpers.Account
{
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>, IRole<int>
    {
        public int Code { get; set; }

        public string Description { get; set; }

        public ApplicationRole() { }

        public ApplicationRole(string name)
            : this()
        {
            Name = name;
        }

        public ApplicationRole(string name, string description)
            : this(name)
        {
            Description = description;
        }
    }
}