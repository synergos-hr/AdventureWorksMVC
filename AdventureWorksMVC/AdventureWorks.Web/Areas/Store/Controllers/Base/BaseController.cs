using System.Web.Mvc;
using NLog;

namespace AdventureWorks.Web.Areas.Store.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}