using Microsoft.Owin;
using Owin;
using AdventureWorks.Web;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace AdventureWorks.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
