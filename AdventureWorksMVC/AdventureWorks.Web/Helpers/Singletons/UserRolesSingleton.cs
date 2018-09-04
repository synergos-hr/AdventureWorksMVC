using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.ViewModels;

namespace AdventureWorks.Web.Helpers.Singletons
{
    public sealed class UserRolesSingleton
    {
        private static UserRolesSingleton _instance;

        private static readonly object Padlock = new object();

        public static UserRolesSingleton Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new UserRolesSingleton());
                }
            }
        }

        public static void ClearInstance()
        {
            lock (Padlock)
            {
                _instance = null;
            }
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<int, List<AspNetRole>> _usersRoles = new ConcurrentDictionary<int, List<AspNetRole>>();

        private UserRolesSingleton()
        {
        }

        public static async Task<List<AspNetRole>> ListRolesAsync(int id)
        {
            if (Instance._usersRoles.ContainsKey(id))
            {
                KeyValuePair<int, List<AspNetRole>> userRoles = Instance._usersRoles.Single(x => x.Key == id);

                return userRoles.Value;
            }
            else
            {
                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                IQueryable<AspNetRole> rolesAsync = await Task.Run(() => repo.ListRoles(id));

                List<AspNetRole> roles = rolesAsync.ToList();

                if (!Instance._usersRoles.TryAdd(id, roles))
                    Instance._log.Trace("Failed to add user roles!");

                return roles;
            }
        }

        public static List<AspNetRole> ListRoles(int id)
        {
            if (Instance._usersRoles.ContainsKey(id))
            {
                KeyValuePair<int, List<AspNetRole>> userRoles = Instance._usersRoles.Single(x => x.Key == id);

                return userRoles.Value;
            }
            else
            {
                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                IQueryable<AspNetRole> rolesAsync = repo.ListRoles(id);

                List<AspNetRole> roles = rolesAsync.ToList();

                if (!Instance._usersRoles.TryAdd(id, roles))
                    Instance._log.Trace("Failed to add user roles!");

                return roles;
            }
        }

        public static void RemoveById(int id)
        {
            if (!Instance._usersRoles.ContainsKey(id))
                return;

            if (!Instance._usersRoles.TryRemove(id, out _))
                Instance._log.Trace("Failed to remove user roles!");
        }

        public static bool IsInRole(int id, string roleName)
        {
            List<AspNetRole> roles = ListRoles(id);

            return roles.Any(x => x.Name == roleName);
        }

        public static async Task<bool> IsInRoleAsync(int id, string roleName)
        {
            List<AspNetRole> roles = await ListRolesAsync(id);

            return roles.Any(x => x.Name == roleName);
        }

        public static async Task<bool> IsInRolesAsync(int id, List<string> roles)
        {
            List<AspNetRole> userRoles = await ListRolesAsync(id);

            foreach (string roleName in roles)
            {
                if (userRoles.Any(x => x.Name == roleName))
                    return true;
            }

            return false;
        }

        private readonly ConcurrentDictionary<int, string> _usersRolesTxt = new ConcurrentDictionary<int, string>();

        public static string RolesTxt(int id)
        {
            if (id == 0)
                return "-";

            if (Instance._usersRolesTxt.ContainsKey(id))
            {
                KeyValuePair<int, string> userRolesTxt = Instance._usersRolesTxt.Single(x => x.Key == id);

                return userRolesTxt.Value;
            }

            RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

            UserViewModel user = repo.GetModelById(id);

            if (!Instance._usersRolesTxt.TryAdd(id, user.RolesTxt))
                Instance._log.Trace("Failed to add user roles!");

            return user.RolesTxt;
        }
    }
}