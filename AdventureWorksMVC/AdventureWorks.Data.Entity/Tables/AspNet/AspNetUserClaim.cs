using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Data.Entity.Tables
{
    public partial class AspNetUserClaim
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
