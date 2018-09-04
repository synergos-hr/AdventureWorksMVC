using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using AdventureWorks.Web.Controllers.Mvc.Base;
using AdventureWorks.Web.Helpers.Account;
using AdventureWorks.Web.Helpers.Attributes;

namespace AdventureWorks.Web.Areas.Admin.Controllers
{
    [CustomMvcAuthorize(Roles = "SuperAdmin,Admin")]
    public class RolesAdminController : BaseController
    {
        #region Propreties

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            set => _userManager = value;
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }

        #endregion

        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        //
        // GET: Admin/Roles/
        public ActionResult Index()
        {
            return View();
        }
    }

}