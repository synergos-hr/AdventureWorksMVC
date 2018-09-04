using System;
using System.Runtime.Caching;
using System.Web;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.ViewModels;
using AdventureWorks.Web.Helpers.Singletons;

namespace AdventureWorks.Web.Helpers.User
{
    public static class UserProfileHelper
    {
        private static bool _usingDisplayUser = false;

        private static bool _usingCache = false;

        public static int GetUserId()
        {
            if (_usingDisplayUser)
            {
                int? displayUserId = GetDisplayUserId();

                return displayUserId != null
                    ? Convert.ToInt32(displayUserId)
                    : HttpContext.Current.User.Identity.GetUserId<int>();
            }

            return HttpContext.Current.User.Identity.GetUserId<int>();
        }

        public static int? GetDisplayUserId()
        {
            int userId = HttpContext.Current.User.Identity.GetUserId<int>();

            ObjectCache cache = MemoryCache.Default;

            var cacheKey = "displayUserIdOf" + userId;

            var displayUserId = cache.Get(cacheKey);

            return (int?)displayUserId;
        }

        public static bool IsDisplayUserOn()
        {
            return GetDisplayUserId() != null;
        }

        public static UserViewModel GetUser(int id)
        {
            //if (_usingCache)
            //{
            //}

            RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

            return repo.GetModelById(id);
        }

        public static string GetUserName()
        {
            UserProfileViewModel profile = GetProfile();

            if (profile == null || profile.FullName.IsEmpty())
                return HttpContext.Current.User.Identity.Name;

            return profile.FullName;
        }

        public static string GetUserEmail()
        {
            return "-";
            //return HttpContext.Current.User.Identity.;
        }

        public static UserProfileViewModel GetProfile()
        {
            return GetProfile(GetUserId());
        }

        public static UserProfileViewModel GetProfile(int userId)
        {
            if (_usingCache)
                return UserProfileSingleton.GetProfileById(userId);

            RepositoryUserProfiles repo = new RepositoryUserProfiles();

            return repo.GetModelById(userId);
        }

        public static string GetProfileFullName()
        {
            UserProfileViewModel profile = GetProfile();

            if (profile == null)
                return "-";

            return profile.FullName;
        }

        public static string GetProfileImage(UserProfile userProfile)
        {
            //if (userProfile == null)
            return "~/Content/css/adminLTE/img/generic.jpg";

            //return userProfile.Gender == "M" ? "~/Content/img/avatar5.png" : "~/Content/img/avatar3.png";
        }

        public static string GetProfileImage()
        {
            //if (userProfile == null)
            return "~/Content/css/adminLTE/img/generic.jpg";

            //return userProfile.Gender == "M" ? "~/Content/img/avatar5.png" : "~/Content/img/avatar3.png";
        }

        public static bool ShowAdminLink()
        {
            if (UserRoleHelper.IsSuperAdmin() || UserRoleHelper.IsAdmin())
                return true;

            return false;
        }
    }
}
