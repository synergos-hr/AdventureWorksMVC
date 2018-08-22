using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Model.Models;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryEmployees : Repository<Employee, EmployeeModel>, IRepositoryEmployees
    {
    }
}
