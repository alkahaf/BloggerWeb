using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggerWeb.Models;
using BloggerWeb.Data;
using System.Linq;

namespace BloggerWeb.Areas.Editor.Controllers
{
    [Area("Editor")]
    public class BlogController : Controller
    {
        private readonly BlogDbContext _context;

        public BlogController(BlogDbContext context)
        {
            //_context = context;

            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Editor/Blog
        public IActionResult Index()
        {
            
            // Check if user is logged in
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Editor" });
            }

            // Get blogs created by the logged-in user and include User data
            var blogs = _context.Blogs.Include(b => b.User).ToList(); // Ensures User data is loaded
            return View(blogs);
        }

        // GET: Editor/Blog/Upsert/{id?}
        public IActionResult Upsert(int? id)
        {
            string? userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            Blog blog = new Blog();

            if (id == null) // Create
            {
                return View(blog);
            }

            // Edit - Only allow the owner to edit
            blog = _context.Blogs.FirstOrDefault(b => b.Id == id );
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Editor/Blog/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Blog blog)
        {
            string? userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Editor" });
            }

            if (blog.Id == 0) // Create
            {
                blog.UserId = userId; // Assign current user
                blog.CreatedAt = DateTime.Now;
                await _context.Blogs.AddAsync(blog);
            }
            else // Edit - Ensure only the owner can update
            {
                var existingBlog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blog.Id && b.UserId == userId);
                if (existingBlog == null)
                {
                    return NotFound();
                }

                // Update properties
                existingBlog.Title = blog.Title;
                existingBlog.Content = blog.Content;
                existingBlog.CreatedAt = blog.CreatedAt;

                _context.Blogs.Update(existingBlog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: Editor/Blog/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            string? userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Editor" });
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }


        // POST: Editor/Blog/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Editor" });
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: Editor/Blog/Details/5
        public IActionResult Details(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
    }
}
