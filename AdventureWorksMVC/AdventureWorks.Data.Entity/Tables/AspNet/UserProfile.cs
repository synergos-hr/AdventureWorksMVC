using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Data.Entity.Tables
{
    public partial class UserProfile
    {
        [Key]
        public int UserId { get; set; }

        [StringLength(50)]
        [DefaultValue("")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [DefaultValue("")]
        public string LastName { get; set; }

        [Required]
        [StringLength(1)]
        [DefaultValue("F")]
        public string Gender { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

    }
}
