using AdventureWorks.Model.Models.Base;

namespace AdventureWorks.Model.Models
{
    public class EmployeeModel : BaseModel
    {
        public short BusinessEntityID { get; set; }

        public string JobTitle { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}
