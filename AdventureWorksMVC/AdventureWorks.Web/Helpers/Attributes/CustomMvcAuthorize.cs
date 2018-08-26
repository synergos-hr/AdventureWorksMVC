using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using NLog;

namespace AdventureWorks.Web.Helpers.Attributes
{
    public class CustomMvcAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                Logger logger = LogManager.GetCurrentClassLogger();

                logger.Error("Access denied! Url: {0}.", JsonConvert.SerializeObject(filterContext.HttpContext.Request.RawUrl));

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", area = "" , url = filterContext.RequestContext.HttpContext.Request.Url }));
            }
        }
    }
}
