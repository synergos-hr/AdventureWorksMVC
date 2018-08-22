using System.Web.Mvc;
using AdventureWorks.Model.Models;
using AdventureWorks.Web.Models.ViewModels;
using AdventureWorks.Web.Areas.Admin.Controllers.Base;

namespace AdventureWorks.Web.Areas.Admin.Controllers.HumanResources
{
    public class DepartmentsController : BaseController
    {
        // GET: Admin/Departments
        public ActionResult Index()
        {
            GridViewModel<DepartmentModel> vm = new GridViewModel<DepartmentModel>() { Test = 1 };

            return View(vm);
        }
    }
}