using System.Collections.Generic;
using System.Security.Principal;
using AdventureWorks.Web.Helpers.User;

namespace AdventureWorks.Web.Models.Menu
{
    public class MenuViewModel
    {
        #region Properties

        //private static bool _usingCache = false;  // TODO: MM: cache

        public List<MenuItem> MenuItems { get; set; }

        #endregion

        #region Methods

        public void ProcessPermissions(IPrincipal principal)
        {
            ProcessPermissions(MenuItems, principal);
        }

        private void ProcessPermissions(List<MenuItem> menuItems, IPrincipal principal)
        {
            foreach (MenuItem item in menuItems)
            {
                bool allowed = true;

                if (!string.IsNullOrEmpty(item.AllowedRoles) && item.AllowedRoles != "*")
                    allowed = IsMenuItemAllowed(item, principal);

                item.Visible = allowed;
                item.Enabled = allowed;

                if (item.MenuItems != null && item.MenuItems.Count > 0)
                    ProcessPermissions(item.MenuItems, principal);
            }
        }

        private bool IsMenuItemAllowed(MenuItem item, IPrincipal principal)
        {
            bool allowed = false;

            if (principal.IsInRole(UserRoleHelper.Role.SuperAdmin))
            {
                allowed = true;
            }
            else
            {
                foreach (var role in item.AllowedRoles.Split(','))
                {
                    allowed = principal.IsInRole(role);

                    if (allowed)
                        break;
                }
            }

            return allowed;
        }

        #endregion
    }
}