using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AdventureWorks.Helpers.Filters;

namespace AdventureWorks.Web
{
    public static class CustomConfig
    {
        public static void CustomizeConfig(HttpConfiguration config)
        {
            // Remove Xml formatters. This means when we visit an endpoint from a browser,
            // Instead of returning Xml, it will return Json.
            // More information from Dave Ward: http://jpapa.me/P4vdx6
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            var json = config.Formatters.JsonFormatter;

            // Configure json camelCasing per the following post: http://jpapa.me/NqC2HH
            // Here we configure it to write JSON property names with camel casing
            // without changing our server-side data model:
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //json.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;

            // reference looping in entity framework objects: child points to parent, parent points to child
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // Add model validation, globally
            config.Filters.Add(new ValidationActionFilter());
        }
    }
}