using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggerWeb.Models;
using System.Linq;
using System.Threading.Tasks;
using BloggerWeb.Data;
using Microsoft.AspNetCore.Http;
using System;

namespace BloggerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly BlogDbContext _context;

        public BlogController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: Blog
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            var blogs = await _context.Blogs.Include(b => b.User).ToListAsync();
            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            Blog blog = new Blog();

            if (id.HasValue) // Editing mode
            {
                blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
                if (blog == null)
                {
                    return NotFound();
                }
            }

            return View(blog);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Blog blog)
        {
            // Check if UserId is in session
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            // Log the userId for debugging purposes
            Console.WriteLine(userId);

           

            if (blog.Id == 0) // Create Mode
            {
                blog.UserId = userId; // Assign current user
                blog.CreatedAt = DateTime.Now;
                await _context.Blogs.AddAsync(blog);
            }
            else // Update Mode
            {
                var existingBlog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blog.Id);
                if (existingBlog != null)
                {
                    blog.UserId = existingBlog.UserId; // Ensure UserId remains unchanged
                    blog.CreatedAt = existingBlog.CreatedAt; // Preserve original creation date
                }

                _context.Blogs.Update(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
    }
}
