using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggerWeb.Models;
using System.Linq;
using BloggerWeb.Data;

public class BlogController : Controller
{
    private readonly BlogDbContext _context;

    public BlogController(BlogDbContext context)
    {
        _context = context;
    }
    // GET: Blog
    public ActionResult Index()
    {
        return View(_context.Blogs.ToList());
    }

    // GET: Blog/Upsert/{id?} - If id is provided, edit; else create new
    public IActionResult Upsert(int? id)
    {
        Blog blog = new Blog();

        if (id == null) // Create Mode
        {
            return View(blog);
        }

        // Edit Mode - Fetch existing blog
        blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null)
        {
            return NotFound();
        }

        return View(blog);
    }

    // POST: Blog/Upsert
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Blog blog)
    {
        if (ModelState.IsValid)
        {
            if (blog.Id == 0) // New Blog (Create)
            {
                _context.Blogs.Add(blog);
            }
            else // Existing Blog (Update)
            {
                _context.Blogs.Update(blog);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(blog);
    }

    // GET: Blog/Delete/5
    public IActionResult Delete(int id)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null)
        {
            return NotFound();
        }
        return View(blog);
    }

    // POST: Blog/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var blog = _context.Blogs.Find(id);
        if (blog == null)
        {
            return NotFound();
        }

        _context.Blogs.Remove(blog);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
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
