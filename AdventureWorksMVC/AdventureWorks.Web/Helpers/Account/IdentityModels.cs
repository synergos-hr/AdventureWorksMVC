using Microsoft.AspNet.Identity.EntityFramework;

// http://johnatten.com/2014/07/13/asp-net-identity-2-0-extending-identity-models-and-using-integer-keys-instead-of-strings/
// https://www.codeproject.com/Articles/777733/ASP-NET-Identity-Change-Primary-Key
namespace AdventureWorks.Web.Helpers.Account
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
    }

    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
    }

    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
    }
}