using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloggerWeb.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key for User
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Navigation property
    }
}
