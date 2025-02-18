using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BloggerWeb.Data;
using BloggerWeb.Models;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BloggerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly BlogDbContext _context;

        public AccountController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handle Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password are required.";
                return View();
            }

            // Hash input password to match the stored hash
            string hashedPassword = HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Set session variables
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);

            // Redirect based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Blog", new { area = "Admin" });
            }
            else if (user.Role == "Editor")
            {
                return RedirectToAction("Index", "Blog", new { area = "Editor" });
            }

            return RedirectToAction("Login");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = HashPassword(model.Password), // Hash password
                    Role = model.Role // Admin or Editor
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            return View(model);
        }

        // Hashing Passwords with SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
