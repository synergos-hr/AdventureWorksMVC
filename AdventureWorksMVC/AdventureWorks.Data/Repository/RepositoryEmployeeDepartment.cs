using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Model.Models;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryEmployeeDepartment : Repository<vEmployeeDepartment, EmployeeDepartmentModel>, IRepositoryEmployeeDepartment
    {
    }
}
