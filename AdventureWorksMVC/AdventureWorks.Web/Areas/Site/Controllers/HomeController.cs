using System.Web.Mvc;

namespace AdventureWorks.Web.Areas.Site.Controllers
{
    public class HomeController : Controller
    {
        // GET: Site/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}