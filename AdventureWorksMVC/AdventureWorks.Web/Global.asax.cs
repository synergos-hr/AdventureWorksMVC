using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AdventureWorks.Web.Controllers.Mvc;
using AdventureWorks.Web.Helpers.Exceptions;

namespace AdventureWorks.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configuration.Filters.Add(new DefaultApiExceptionHandler());

            //IocConfig.RegisterIoc();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MetadataConfig.SetMetadataProvider();

            MapperConfig.RegisterMaps();

            JsonSerializerConfig.CustomizeConfig(GlobalConfiguration.Configuration);
            //XmlSerializerConfig.CustomizeConfig(GlobalConfiguration.Configuration);
        }

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                RouteData rd = new RouteData();

                rd.Values["controller"] = "Error";
                rd.Values["action"] = "NotFound";
                rd.Values["url"] = Context.Request.Url;

                IController c = new ErrorController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
        }
    }
}
