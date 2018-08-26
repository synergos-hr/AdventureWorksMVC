using System.Web.Mvc;
using AdventureWorks.Web.Controllers.Mvc.Base;
using AdventureWorks.Web.Models.Error;

namespace AdventureWorks.Web.Controllers.Mvc
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        // GET: AccessDenied
        public ActionResult AccessDenied(string url)
        {
            AccessDeniedViewModel model = new AccessDeniedViewModel
            {
                Url = url
            };

            return View(model);
        }

        public ActionResult NotFound(string url)
        {
            NotFoundViewModel model = new NotFoundViewModel
            {
                Url = url
            };

            return View(model);
        }
    }
}