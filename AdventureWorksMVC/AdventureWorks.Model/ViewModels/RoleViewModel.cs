using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Model.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Translation { get; set; }

        public string DisplayName => Translation ?? Name;
    }
}
