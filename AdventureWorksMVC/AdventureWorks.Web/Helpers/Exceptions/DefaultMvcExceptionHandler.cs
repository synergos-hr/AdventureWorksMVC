using System;
using System.Web;
using System.Web.Mvc;
using NLog;
using AdventureWorks.Model.API;

namespace AdventureWorks.Web.Helpers.Exceptions
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DefaultMvcExceptionHandler : FilterAttribute, IExceptionFilter
    {
        private readonly Logger _logger;

        public DefaultMvcExceptionHandler()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void OnException(ExceptionContext filterContext)
        {
            _logger.Error(filterContext.Exception, filterContext.Exception.Message);

            if (filterContext.ExceptionHandled)
                return;

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
                return;

            var response = filterContext.RequestContext.HttpContext.Response;

            response.Write(new ErrorResult { Status = "error", Message = filterContext.Exception.Message });

            //if (filterContext.Exception is System.Security.Authentication.AuthenticationException)
            //{
            //    filterContext.ExceptionHandled = true;
            //    filterContext.HttpContext.Response.Clear();
            //    filterContext.HttpContext.Response.Redirect(UrlConfig.GetBaseUrl() + "Account/Relog");

            //    return;
            //}

            //if (filterContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    var response = filterContext.RequestContext.HttpContext.Response;
            //    response.Write(JsonConvert.SerializeObject(new { Errors = filterContext.Exception.Message }));
            //    response.ContentType = "application/json";
            //}
            //else
            //{
            //    var controllerName = (string)filterContext.RouteData.Values["controller"];
            //    var actionName = (string)filterContext.RouteData.Values["action"];
            //    var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            //    filterContext.Result = new ViewResult
            //    {
            //        ViewName = "InternalServerError",
            //        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            //        TempData = filterContext.Controller.TempData
            //    };

            //    filterContext.HttpContext.Response.Clear();
            //}

            filterContext.ExceptionHandled = true;

            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
