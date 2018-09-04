using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Data.Entity.Views.Users
{
    [Table("Users.Users")]
    public partial class vUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string DisplayName { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        public bool? TestUser { get; set; }

        public bool? Locked { get; set; }

        public string Roles { get; set; }

        public string RolesTxt { get; set; }

        public int? MinRoleCode { get; set; }

        public int? MaxRoleCode { get; set; }
    }
}
