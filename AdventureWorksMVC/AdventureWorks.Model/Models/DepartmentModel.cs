using System;
using AdventureWorks.Model.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Model.Models
{
    public class DepartmentModel : BaseModel
    {
        [Key]
        public short DepartmentID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
