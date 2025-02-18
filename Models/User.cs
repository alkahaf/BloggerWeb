using System.ComponentModel.DataAnnotations;

namespace BloggerWeb.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } // "Admin" or "Editor"


        public ICollection<Blog> Blogs { get; set; } // Navigation property (optional)
    }
}
