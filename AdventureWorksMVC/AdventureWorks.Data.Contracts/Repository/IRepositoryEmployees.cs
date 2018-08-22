using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.Models;

namespace AdventureWorks.Data.Contracts.Repository
{
    public interface IRepositoryEmployees : IRepository<Employee, EmployeeModel>
    {
    }
}
