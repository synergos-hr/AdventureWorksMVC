using System.Web.Mvc;
using Newtonsoft.Json;
using AdventureWorks.Web.Areas.Admin.Controllers.Base;
using AdventureWorks.Web.Models.Menu;

namespace AdventureWorks.Web.Areas.Admin.Controllers
{
    public class MenuController : BaseController
    {
        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            MenuViewModel model = JsonConvert.DeserializeObject<MenuViewModel>(System.IO.File.ReadAllText(Server.MapPath("~/Content/menu/menu.en-EN.json")));

            model.ProcessPermissions(User);

            return PartialView("_Partial_SidebarMenu", model);
        }
    }
}