using System.ComponentModel.DataAnnotations;

namespace BloggerWeb.Models
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Dropdown for Admin/Editor
    }

}
