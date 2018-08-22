using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Model.Models;

namespace AdventureWorks.Data.Contracts.Repository
{
    public interface IRepositoryEmployeeDepartment : IRepository<vEmployeeDepartment, EmployeeDepartmentModel>
    {
    }
}
