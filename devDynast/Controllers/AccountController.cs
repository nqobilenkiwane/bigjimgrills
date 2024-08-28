using Microsoft.AspNetCore.Mvc;
using devDynast.Data;
using devDynast.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace devDynast.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                
                if (user != null)
                {
                    int userId = user.Id ?? 0;

                    // Create claims for the user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email ?? string.Empty), 
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role ?? "User") 
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Store the user ID in the session
                    HttpContext.Session.SetInt32("UserId", userId);

                    // Redirect based on the user's role
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin"); // Redirect to Admin Dashboard
                    }
                    else if (user.Role == "User") // Check if the role is User
            {
                return RedirectToAction("Dashboard", "User"); // Redirect to User Dashboard
            }

                    return RedirectToAction("Index", "Home"); // Redirect to Home for regular users
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear the session
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

         public IActionResult Dashboard()
        {
            return View(); // Return the Dashboard view
        }
    }
}
