namespace AdventureWorks.Model.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string FullName => (FirstName + " " + LastName).Trim();

        public bool ChangeEnabled { get; set; }
    }
}
