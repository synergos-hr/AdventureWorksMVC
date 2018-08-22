using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AdventureWorks.Web;
using AdventureWorks.Web.Helpers.Exceptions;

namespace AdventureWorksMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configuration.Filters.Add(new DefaultApiExceptionHandler());

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MetadataConfig.SetMetadataProvider();

            MapperConfig.RegisterMaps();

            JsonSerializerConfig.CustomizeConfig(GlobalConfiguration.Configuration);
            //XmlSerializerConfig.CustomizeConfig(GlobalConfiguration.Configuration);
        }
    }
}
