using Microsoft.AspNetCore.Mvc;
using devDynast.Data;
using devDynast.Models;

namespace devDynast.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Menu/AddMeal
        public IActionResult AddMeal()
        {
            return View();
        }

        // POST: Menu/AddMeal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMeal(Menu meal)
        {
            if (ModelState.IsValid)
            {
                _context.Menus.Add(meal);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(meal);
        }

        // GET: Menu/AddMenuItem
        public IActionResult AddMenuItem()
        {
            return View();
        }


        // POST: Menu/AddMenuItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMenuItem(MenuItem meal) 
        {
            if (ModelState.IsValid)
            {
                _context.MenuItems.Add(meal);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home"); 
            }
            return View(meal);
        }
    }
}
