using AdventureWorks.Data;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Model.Models;
using AdventureWorks.Web.Controllers.API.Base;

namespace AdventureWorks.Web.Controllers.API.HumanResources
{
    public class EmployeesController : ApiBaseModelController<vEmployeeDepartment, EmployeeDepartmentModel, Repository<vEmployeeDepartment, EmployeeDepartmentModel>>
    {
        public EmployeesController()
           : base("BusinessEntityID", "LastName")
        {
        }
    }
}
