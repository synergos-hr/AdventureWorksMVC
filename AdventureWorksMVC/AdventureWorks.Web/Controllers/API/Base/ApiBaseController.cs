using System.Web.Http;
using NLog;

namespace AdventureWorks.Web.Controllers.API.Base
{
    public abstract class ApiBaseController : ApiController
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}