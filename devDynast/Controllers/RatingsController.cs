using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using devDynast.Data;
using devDynast.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace devDynast.Controllers
{
    [Authorize]
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedbackController> _logger;

        public RatingsController(ApplicationDbContext context, ILogger<FeedbackController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ratings
        public IActionResult Items()
        {
            // Retrieve the user ID from the session as int
            var userIdInt = 1; // HttpContext.Session.GetInt32("UserId");
            
            // Convert to string
            var userId = "1"; // userIdInt?.ToString();

            // Log the retrieved UserId
            _logger.LogInformation("Retrieved UserId from session: {UserId}", userId);

            // Check if the userId is not null
            if (!string.IsNullOrEmpty(userId))
            {
                var cartItems = _context.Cart
                    .Where(c => c.UserId == userId && c.Status == "paid")
                    .ToList();

                
                return View(cartItems);
            }
            else
            {
                
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Ratings/Create
        [HttpGet] 
        public IActionResult Create(int itemId)
        {
            ViewBag.ItemId = itemId; // Pass the Item ID to the view
            return View();
        }

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Rating rating)
{
    // Log the start of the Create action
    _logger.LogInformation("Create action started with Rating: {@Rating}", rating);

    // Check if the ItemId exists in the MenuItem table
    var menuItemExists = await _context.MenuItems.AnyAsync(m => m.Id == rating.ItemId);
    
    
    _logger.LogInformation("MenuItem existence check for ItemId {ItemId}: {Exists}", rating.ItemId, menuItemExists);

    if (!menuItemExists)
    {
        _logger.LogWarning("The specified item with ItemId {ItemId} does not exist.", rating.ItemId);
        ModelState.AddModelError("ItemId", "The specified item does not exist.");
        return View(rating);
    }

    if (ModelState.IsValid)
    {
        _logger.LogInformation("ModelState is valid. Saving the rating.");
        
        
        rating.CreatedAt = DateTime.UtcNow; 

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Rating saved successfully with Id: {RatingId}", rating.Id);

        return RedirectToAction("Index");
    }

    _logger.LogWarning("ModelState is invalid. Returning the view with errors.");
    return View(rating);
}


    }
}
