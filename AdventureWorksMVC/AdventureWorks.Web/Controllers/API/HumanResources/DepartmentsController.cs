using AdventureWorks.Data;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Model.Models;
using AdventureWorks.Web.Controllers.API.Base;

namespace AdventureWorks.Web.Controllers.API.HumanResources
{
    public class DepartmentsController : ApiBaseModelController<Department, DepartmentModel, Repository<Department, DepartmentModel>>
    {
        public DepartmentsController()
           : base("DepartmentID", "Name")
        {
        }
    }
}
