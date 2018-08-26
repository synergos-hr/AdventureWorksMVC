using System;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;

namespace AdventureWorks.Web.Helpers.Attributes
{
    // http://stackoverflow.com/questions/24376800/the-back-button-and-the-anti-forgery-token
    // https://stackoverflow.com/questions/12967917/a-way-of-properly-handling-httpantiforgeryexception-in-mvc-4-application
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        public HandleAntiforgeryTokenErrorAttribute()
        {
            ExceptionType = typeof (HttpAntiForgeryException);
        }

        public override void OnException(ExceptionContext filterContext)
        {
            Type exceptionType = filterContext.Exception.GetType();

            if (exceptionType != ExceptionType)
                return;

            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Error("Anti forgery token is redirecting to home page!");

            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { action = "Index", controller = "Home" }));
        }
    }
}
