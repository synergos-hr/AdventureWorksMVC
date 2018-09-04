using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.ViewModels;

namespace AdventureWorks.Web.Helpers.Singletons
{
    public sealed class UserProfileSingleton
    {
        private static UserProfileSingleton _instance;

        private static readonly object Padlock = new object();

        public static UserProfileSingleton Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new UserProfileSingleton());
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

        private readonly ConcurrentDictionary<int, UserProfileViewModel> _profiles = new ConcurrentDictionary<int, UserProfileViewModel>();

        private UserProfileSingleton()
        {
        }

        public static UserProfileViewModel GetProfileById(int id)
        {
            if (Instance._profiles.ContainsKey(id))
            {
                KeyValuePair<int, UserProfileViewModel> userProfileCached = Instance._profiles.SingleOrDefault(x => x.Key == id);

                if (userProfileCached.Value != null)
                    return userProfileCached.Value;
            }

            RepositoryUserProfiles repo = new RepositoryUserProfiles();

            UserProfileViewModel userProfile = repo.GetModelById(id);

            if (userProfile != null)
            {
                if (!Instance._profiles.TryAdd(id, userProfile))
                    Instance._log.Trace("Failed to add user profile!");
            }

            return userProfile;
        }

        public static void RemoveProfileById(int id)
        {
            if (!Instance._profiles.ContainsKey(id))
                return;

            if (!Instance._profiles.TryRemove(id, out _))
                Instance._log.Trace("Failed to remove user profile!");
        }
    }
}
