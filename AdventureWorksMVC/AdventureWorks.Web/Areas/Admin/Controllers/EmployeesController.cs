using System.Web.Mvc;
using AdventureWorks.Model.Models;
using AdventureWorks.Web.Models.ViewModels;
using AdventureWorks.Web.Areas.Admin.Controllers.Base;

namespace AdventureWorks.Web.Areas.Admin.Controllers.HumanResources
{
    public class EmployeesController : BaseController
    {
        // GET: Admin/Departments
        public ActionResult Index()
        {
            GridViewModel<EmployeeModel> vm = new GridViewModel<EmployeeModel>() {};

            return View(vm);
        }
    }
}