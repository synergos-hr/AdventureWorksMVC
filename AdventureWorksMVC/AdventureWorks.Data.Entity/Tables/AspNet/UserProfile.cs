using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Data.Entity.Tables.AspNet
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

        [StringLength(250)]
        public string PictureFileName { get; set; }

        public bool? TestUser { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
