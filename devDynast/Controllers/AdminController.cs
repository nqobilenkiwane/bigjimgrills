using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using devDynast.Data;
using devDynast.Models;
using System.Globalization;

namespace devDynast.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context, ILogger<UserController> logger) 
        {
            _context = context;
            _logger = logger; 
        }

        // GET: Admin/Index
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: Admin/EditUser/
        public IActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/EditUser/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/DeleteUser/
        public IActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
          {
            var user = _context.Users.Find(id);
            if (user == null)
           {
            return NotFound();
           }

           _context.Users.Remove(user);
           _context.SaveChanges();
           return RedirectToAction(nameof(Index));
        }

        // GET: Admin/EditMeal/
        public IActionResult EditMeal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = _context.MenuItems.Find(id);
            if (meal == null)
            {
                return NotFound();
            }

            return View(meal);
        }

        // POST: Admin/EditMeal/
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult EditMeal(int id, [Bind("Id,Name,Category,Description,ImageUrl,Price")] MenuItem meal)
{
    // Debugging: Check if the ID matches
    Console.WriteLine($"Received ID: {id}, Meal ID: {meal.Id}");

    if (id != meal.Id)
    {
        Console.WriteLine("ID mismatch - returning NotFound.");
        return NotFound();
    }

    // Debugging: Check if the model state is valid
    Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

    if (ModelState.IsValid)
    {
        try
        {
            // Debugging: Before updating
            Console.WriteLine($"Updating meal: {meal.Name}, Category: {meal.Category}, Price: {meal.Price}");

            _context.Update(meal);
            var affectedRows = _context.SaveChanges();

            // Debugging: After saving changes
            Console.WriteLine($"SaveChanges() affected {affectedRows} rows.");

            if (affectedRows == 0)
            {
                Console.WriteLine("No rows affected. Possible concurrency issue.");
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Debugging: Catching concurrency exception
            Console.WriteLine("Caught DbUpdateConcurrencyException.");
            Console.WriteLine($"Exception message: {ex.Message}");

            var databaseValues = _context.Entry(meal).GetDatabaseValues();

            if (databaseValues == null)
            {
                Console.WriteLine("Meal deleted by another user - returning error.");
                ModelState.AddModelError(string.Empty, "Unable to save changes. The meal was deleted by another user.");
                return RedirectToAction(nameof(MenuList));
            }
            else
            {
                var dbMeal = (Menu)databaseValues.ToObject();
                Console.WriteLine("Concurrency conflict - returning the current database values.");
                ModelState.AddModelError(string.Empty, "The meal was updated by another user. Your changes have been canceled.");
                return View(dbMeal);
            }
        }

        Console.WriteLine("Redirecting to MenuList.");
        return RedirectToAction(nameof(MenuList)); // Redirect to a different page, like the menu list
    }

    // Debugging: Model state is invalid
    Console.WriteLine("Model state is invalid. Returning the view with the meal data.");
    return View(meal);
}


        // GET: Admin/DeleteMeal/
public IActionResult DeleteMeal(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var meal = _context.MenuItems.Find(id);
    if (meal == null)
    {
        return NotFound();
    }

    return View(meal);
}

// POST: Admin/DeleteMeal/
[HttpPost, ActionName("DeleteMeal")]
[ValidateAntiForgeryToken]
public IActionResult DeleteMealConfirmed(int id)
{
    var meal = _context.MenuItems.Find(id);
    if (meal == null)
    {
        return NotFound();
    }

    _context.MenuItems.Remove(meal);
    _context.SaveChanges();

    // Redirect to MenuList after deletion
    return RedirectToAction(nameof(MenuList));
}


        // GET: Admin/MenuList
        public IActionResult MenuList()
        {
            var meals = _context.MenuItems.ToList();
            return View(meals);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult RegisterUser()
        {
            
            return View();
        }

        public IActionResult ShowMenuList()
        {
            
            return View();
        }

        public IActionResult UsersList()
        {
            
            return View();
        }

        public IActionResult Statistics()
        {
            
            return View();
        }

        public async Task<IActionResult> SalesCompare()
        {
            // Fetch the sales data and group by year and month
            var salesData = await _context.Cart
                .Where(c => c.Status == "paid" || c.Status == "pending")
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SalesCount = g.Count(c => c.Status == "paid"),
                    PendingCount = g.Count(c => c.Status == "pending")
                })
                .OrderBy(sd => sd.Year).ThenBy(sd => sd.Month)
                .ToListAsync();

            
            var result = salesData.Select(g => new SalesCompareViewModel
            {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month),
                SalesCount = g.SalesCount,
                PendingCount = g.PendingCount
            }).ToList();

            return View(result);
        }

        public async Task<IActionResult> TopSellingItems()
        {
            var report = await _context.Cart
                .Where(cart => cart.Status == "paid")
                .GroupBy(cart => cart.ProductId)
                .Select(group => new 
                {
                    ProductId = group.Key,
                    SalesCount = group.Sum(c => c.Quantity),
                    TotalRevenue = group.Sum(c => c.Price * (double)c.Quantity)
                })
                .Join(_context.MenuItems,
                    group => group.ProductId,
                    menuItem => menuItem.Id.ToString(),
                    (group, menuItem) => new TopSellingItemsViewModel
                    {
                        ItemName = menuItem.Name,
                        SalesCount = group.SalesCount,
                        TotalRevenue = group.TotalRevenue
                    })
                .OrderByDescending(item => item.SalesCount) // Rank by highest selling first
        .ToListAsync();

            return View(report);
        }

         public async Task<IActionResult> SalesReport()
{
    var salesData = await _context.Cart
        .Where(c => c.Status == "paid")
        .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
        .Select(g => new 
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            SalesCount = g.Count()
        })
        .ToListAsync();

    var salesDataViewModel = salesData
        .Select(data => new SalesDataViewModel
        {
            Month = new DateTime(data.Year, data.Month, 1).ToString("MMMM"),
            SalesCount = data.SalesCount
        })
        .OrderBy(sd => DateTime.ParseExact(sd.Month, "MMMM", CultureInfo.CurrentCulture).Month)
        .ToList();

    return View(salesDataViewModel);
}


    }
}
