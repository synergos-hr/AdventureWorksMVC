using System;
using AdventureWorks.Model.Models.Base;

namespace AdventureWorks.Model.Models
{
    public class EmployeeDepartmentModel : BaseModel
    {
        public short BusinessEntityID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
