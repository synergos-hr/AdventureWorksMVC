using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace AdventureWorks.Web.Helpers.User
{
    public static class PrincipalExtensions
    {
        public static bool IsInAllRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.All(principal.IsInRole);
        }

        public static bool IsInAnyRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(principal.IsInRole);
        }

        public static bool IsInRole(this IPrincipal principal, UserRoleHelper.Role role)
        {
            return principal.IsInRole(UserRoleHelper.Roles[role]);
        }

        public static bool IsInRoleOrAbove(this IPrincipal principal, UserRoleHelper.Role role)
        {
            foreach (KeyValuePair<UserRoleHelper.Role, string> dictRole in UserRoleHelper.Roles)
            {
                if (principal.IsInRole(dictRole.Value))
                    return true;

                if (dictRole.Key == role)
                    break;
            }

            return false;
        }
    }
}