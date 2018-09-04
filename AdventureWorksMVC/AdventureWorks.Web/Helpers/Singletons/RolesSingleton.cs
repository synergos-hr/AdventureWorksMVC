using System.Collections.Concurrent;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Repository;

namespace AdventureWorks.Web.Helpers.Singletons
{
    public sealed class RolesSingleton
    {
        private static RolesSingleton _instance;

        private static readonly object Padlock = new object();

        public readonly ConcurrentBag<AspNetRole> Roles;

        public static RolesSingleton Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new RolesSingleton());
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

        RolesSingleton()
        {
            RepositoryAspNetRoles repo = new RepositoryAspNetRoles();

            Roles = new ConcurrentBag<AspNetRole>(repo.GetAll());
        }
    }
}