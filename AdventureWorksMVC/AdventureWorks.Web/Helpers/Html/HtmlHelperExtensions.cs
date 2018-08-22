using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace AdventureWorks.Web.Helpers.Html
{
    public static class HtmlHelperExtensions
    {
        private static readonly JsonSerializerSettings jsonSettings;

        static HtmlHelperExtensions()
        {
            jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>Serializes the object to a JSON string.</summary>
        /// <returns>A JSON string representation of the object.</returns>
        public static MvcHtmlString ToJson(this HtmlHelper html, object value)
        {
            return MvcHtmlString.Create(JsonConvert.SerializeObject(value, Formatting.None, jsonSettings));
        }

        public static MvcHtmlString LteActionLink(this HtmlHelper html, string text, string action, string controller, string id = null, string faIcon = null)
        {
            var context = html.ViewContext;

            if (context.Controller.ControllerContext.IsChildAction)
                context = html.ViewContext.ParentActionViewContext;

            var routeValues = context.RouteData.Values;
            var currentAction = routeValues["action"].ToString();
            var currentController = routeValues["controller"].ToString();

            bool isActive = currentAction.Equals(action, StringComparison.InvariantCulture) && currentController.Equals(controller, StringComparison.InvariantCulture);

            string linkText = faIcon != null 
                ? string.Format("<i class=\"fa {0}\"></i> {1}", faIcon, text)
                : string.Format("{0}", text);

            string tagId = id != null
                ? string.Format("id=\"{0}\"", id)
                : "";

            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

            string link = string.Format("<a href=\"{0}\">{1}</a>",
                urlHelper.Action(action, controller),
                linkText);

            string str = string.Format("<li {0}{1}>{2}</li>",
                tagId, 
                isActive ? " class=\"active\"" : string.Empty,
                link
            );

            return new MvcHtmlString(str);
        }

        //public static MvcHtmlString LteMenuTreeLink(this HtmlHelper html, string text, string action, string controller, string id, string faIcon)
        //{
        //    var context = html.ViewContext;

        //    if (context.Controller.ControllerContext.IsChildAction)
        //        context = html.ViewContext.ParentActionViewContext;

        //    var routeValues = context.RouteData.Values;
        //    var currentAction = routeValues["action"].ToString();
        //    var currentController = routeValues["controller"].ToString();

        //    bool isActive = currentAction.Equals(action, StringComparison.InvariantCulture) && currentController.Equals(controller, StringComparison.InvariantCulture);

        //    string linkText = string.Format("<i class=\"fa {0}\"></i> {1}",
        //        faIcon,
        //        text);

        //    var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

        //    string link = string.Format("<a href=\"{0}\">{1}</a>",
        //        urlHelper.Action(action, controller),
        //        linkText);

        //    var str = string.Format("<a href=\"#\">" +
        //        "<i class=\"fa fa-gear\"></i>" +
        //        "<i id=\"admin_arrow\" class=\"fa fa-angle-left pull-right\"></i>" +
        //        "</a>",
        //        id,
        //        isActive ? " class=\"active\"" : string.Empty,
        //        link
        //    );

        //    return new MvcHtmlString(str);
        //}

        // http://stackoverflow.com/questions/19931698/how-to-display-a-default-image-in-case-the-source-does-not-exists
        public static string ImageOrDefault(this HtmlHelper helper, string imagePath)
        {
            var imageSrc = File.Exists(HttpContext.Current.Server.MapPath(imagePath))
                               ? imagePath : imageMissingPath;

            string appUrl = HttpRuntime.AppDomainAppVirtualPath;

            imageSrc = imageSrc.Replace("~", appUrl == "/" ? "" : appUrl);

            return imageSrc;
        }

        private const string imageMissingPath = "~/images/missing.png";
    }
}
