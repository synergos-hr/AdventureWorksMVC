using System.Collections.Generic;
using System.Web;
using AdventureWorks.Web.Helpers.Singletons;

namespace AdventureWorks.Web.Helpers.User
{
    public static class UserRoleHelper
    {
        public enum Role { SuperAdmin, Admin, Manager, Reporter, Auditor, User }

        public static readonly Dictionary<Role, string> Roles = new Dictionary<Role, string>
        {
            { Role.SuperAdmin, "SuperAdmin" },
            { Role.Admin, "Admin" },
            { Role.Manager, "Manager" },
            { Role.Reporter, "Reporter" },
            { Role.Auditor, "Auditor" },
            { Role.User, "User" }
        };

        public static string GetAllRolesAboveRole(Role role)
        {
            string roles = Roles[Role.SuperAdmin];

            if (role == Role.SuperAdmin)
                return roles;

            roles += Roles[Role.Admin];

            if (role == Role.Admin)
                return roles;

            roles += Roles[Role.Manager];

            if (role == Role.Manager)
                return roles;

            roles += Roles[Role.Reporter];
            roles += Roles[Role.Auditor];

            if (role == Role.Reporter || role == Role.Auditor)
                return roles;

            roles += Roles[Role.User];

            return roles;
        }

        public static string GetRolesTxt()
        {
            return UserRolesSingleton.RolesTxt(UserProfileHelper.GetUserId());
        }

        public static bool IsSuperAdmin()
        {
            return HttpContext.Current.User.IsInRole(Role.SuperAdmin);
        }

        public static bool IsAdmin()
        {
            return IsSuperAdmin() || HttpContext.Current.User.IsInRole(Role.Admin);
        }

        public static bool IsManager()
        {
            return IsAdmin() || HttpContext.Current.User.IsInRole(Role.Manager);
        }
    }
}