using System.Web;

namespace AdventureWorks.Web.Helpers.User
{
    public static class UserProfileHelper
    {
        private static bool _usingDisplayUser = false;

        private static bool _usingCache = false;

        //public static int GetUserId()
        //{
        //    if (_usingDisplayUser)
        //    {
        //        int? displayUserId = GetDisplayUserId();

        //        return displayUserId != null
        //            ? Convert.ToInt32(displayUserId)
        //            : HttpContext.Current.User.Identity.GetUserId<int>();
        //    }

        //    return HttpContext.Current.User.Identity.GetUserId<int>();
        //}

        //public static int? GetDisplayUserId()
        //{
        //    int userId = HttpContext.Current.User.Identity.GetUserId<int>();

        //    ObjectCache cache = MemoryCache.Default;

        //    var cacheKey = "displayUserIdOf" + userId;

        //    var displayUserId = cache.Get(cacheKey);

        //    return (int?)displayUserId;
        //}

        //public static bool IsDisplayUserOn()
        //{
        //    return GetDisplayUserId() != null;
        //}

        //public static AspNetUser GetUser()
        //{
        //    //if (_usingCache)
        //    //{
        //    //}

        //    IRepositoryAspNetUsers repo = new RepositoryAspNetUsers();

        //    return repo.GetById(GetUserId());
        //}

        //public static UserViewModel GetUser(int id)
        //{
        //    //if (_usingCache)
        //    //{
        //    //}

        //    IRepositoryAspNetUsers repo = new RepositoryAspNetUsers();

        //    return repo.GetModelById(id);
        //}

        public static string GetUserName()
        {
            //UserProfileViewModel profile = GetProfile();

            //if (profile == null || profile.FullName.IsEmpty())
            //    return GetUserEmail();

            //return profile.FullName;

            return "-";
        }

        public static string GetUserEmail()
        {
            return "-";
            //return HttpContext.Current.User.Identity.GetUserName();
        }

        //public static UserProfileViewModel GetProfile()
        //{
        //    return GetProfile(GetUserId());
        //}

        //public static UserProfileViewModel GetProfile(int userId)
        //{
        //    if (_usingCache)
        //        return UserProfileSingleton.GetProfileById(userId);

        //    IRepositoryUserProfiles repo = new RepositoryUserProfiles();

        //    return repo.GetModelById(userId);
        //}

        //public static string GetProfileFullName()
        //{
        //    UserProfileViewModel profile = GetProfile();

        //    if (profile == null)
        //        return "-";

        //    return profile.FullName;
        //}

        //public static string GetProfileImage(UserProfile userProfile)
        //{
        //    //if (userProfile == null)
        //    return "~/Content/css/adminLTE/img/generic.jpg";

        //    //return userProfile.Gender == "M" ? "~/Content/img/avatar5.png" : "~/Content/img/avatar3.png";
        //}

        public static string GetProfileImage()
        {
            //if (userProfile == null)
            return "~/Content/css/adminLTE/img/generic.jpg";

            //return userProfile.Gender == "M" ? "~/Content/img/avatar5.png" : "~/Content/img/avatar3.png";
        }

        //public static DepartmentViewModel GetProfileDepartment()
        //{
        //    RepositoryUserProfiles repo = new RepositoryUserProfiles();

        //    UserProfileViewModel model = repo.GetModelById(GetUserId());

        //    return model.Department;
        //}

        //public static DepartmentViewModel GetProfileDepartmentAuthority()
        //{
        //    if (!IsDepartmentAuthority())
        //        return null;

        //    return GetProfileDepartment();
        //}

        //public static bool IsDepartmentAuthority()
        //{
        //    return UserProfileSingleton.IsDeparmentAuthority(GetUserId());
        //}

        //public static bool ShowAdminLink()
        //{
        //    if (UserRoleHelper.IsSuperAdmin() || UserRoleHelper.IsAdmin())
        //        return true;

        //    if (UserProfileSingleton.IsDeparmentAuthority(GetUserId()))
        //        return true;

        //    return false;
        //}

        //public static NotificationsViewModel GetNotifications()
        //{
        //    NotificationsViewModel model = new NotificationsViewModel();

        //    UserProfile profile = GetProfile();

        //    List<string> notifications = new List<string>();

        //    if (profile != null)
        //    {
        //        if (profile.FirstName.IsNullOrWhiteSpace() || profile.LastName.IsNullOrWhiteSpace())
        //            notifications.Add("Unesite profilno ime i prezime.");
        //    }

        //    model.Notifications = notifications;

        //    return model;
        //}
    }
}
