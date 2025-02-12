using BloggerWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggerWeb.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }
    }
}
