using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdventureWorks.Web.Helpers.Account
{
    public class AccountDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public AccountDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<AccountDbContext>(null);
        }

        //public static ApplicationUserManager ApplicationUserManager;
        //public static ApplicationRoleManager ApplicationRoleManager;

        static AccountDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer(ApplicationUserManager, ApplicationRoleManager));

            //Database.SetInitializer<AccountDbContext>(new ApplicationDbInitializer());

            //Database.SetInitializer<AccountDbContext>(null);
        }

        public static AccountDbContext Create()
        {
            return new AccountDbContext();
        }
    }
}