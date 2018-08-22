using System.Web.Mvc;

namespace AdventureWorks.Web.Areas.Site.Controllers
{
    public class TestController : Controller
    {
        // GET: Site/Test
        public ActionResult Index()
        {
            return View();
        }
    }
}