using Microsoft.AspNetCore.Mvc;
using devDynast.Data;
using devDynast.Models;
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace devDynast.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger) 
        {
            _context = context;
            _logger = logger; 
        }
        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Add user to database
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        // GET: User/Dashboard
    public IActionResult Dashboard()
    {
        return View(); 
    }

    // GET: User/EditProfile
public async Task<IActionResult> EditProfile()
{
    var userId = HttpContext.Session.GetInt32("UserId");

    if (userId == null)
    {
        return NotFound();
    }

    var user = await _context.Users.FindAsync(userId.Value);
    if (user == null)
    {
        return NotFound();
    }

    return View(user);
}

 // POST: User/EditProfile
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditProfile([Bind("Id,FirstName,LastName,Email,PhoneNumber,Role")] User user)
{
    _logger.LogInformation("EditProfile POST action called.");
    _logger.LogInformation("User profile requested for UserId: {UserId}", user.Id);

    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null)
    {
        _logger.LogWarning("UserId is null in session.");
        return NotFound();
    }

    user.Id = userId.Value;

    if (ModelState.IsValid)
    {
        try
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser != null)
            {
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
               

                _context.Update(existingUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User profile updated successfully for UserId: {UserId}", user.Id);
                return RedirectToAction("Dashboard", "User"); 
            }
            else
            {
                _logger.LogWarning("User not found for UserId: {UserId}", user.Id);
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for UserId: {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the profile. Please try again.");
        }
    }
    else
    {
        _logger.LogWarning("ModelState is invalid for UserId: {UserId}", user.Id);
        foreach (var modelStateKey in ModelState.Keys)
        {
            var modelStateVal = ModelState[modelStateKey];
            foreach (var error in modelStateVal.Errors)
            {
                _logger.LogError("ModelState Error: Key = {Key}, Error = {Error}", modelStateKey, error.ErrorMessage);
            }
        }
    }

    return View(user); // Return the view with validation errors
}

// GET: Feedback/Create
        public IActionResult Feedback()
        {
            _logger.LogInformation("Navigated to Feedback/Create page.");
            return View();
        }

        // GET: User/Index
        public IActionResult Index()
        {
            return View();
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Feedback(Feedback feedback)
{
    // Retrieve the user ID from the session
    var userId = HttpContext.Session.GetInt32("UserId");
    

    if (userId == null)
    {
        _logger.LogWarning("User not found in session.");
        return RedirectToAction("Error", "Home");
    }

    // Convert the nullable int to string
    feedback.UserId = userId.Value.ToString();

    if (ModelState.IsValid)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    // Log the errors in the model state
    var errors = ModelState.Values.SelectMany(v => v.Errors);
    foreach (var error in errors)
    {
        _logger.LogWarning("Model error: {Error}", error.ErrorMessage);
    }

    return View(feedback);
}

// GET: Ratings
        public IActionResult Items()
        {
            // Retrieve the user ID from the session as int
            var userIdInt = HttpContext.Session.GetInt32("UserId");
            
            // Convert to string
            var userId = userIdInt?.ToString();

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

        return RedirectToAction("Items");
    }

    _logger.LogWarning("ModelState is invalid. Returning the view with errors.");
    return View(rating);
}


public async Task<IActionResult> OrderHistory()
        {
            // Retrieve the user ID from the session
            var userIdInt = HttpContext.Session.GetInt32("UserId");

            //
            _logger.LogInformation("Retrieved UserId from session: {UserId}", userIdInt);

           
            if (userIdInt.HasValue)
            {
                var userId = userIdInt.Value.ToString();

                // Fetch past orders
                var pastOrders = _context.Cart
                    .Where(c => c.UserId == userId && c.Status == "paid")
                    .ToList();

                // Calculate total price
                var totalPrice = pastOrders.Sum(c => c.Price * c.Quantity);

                // Pass orders and total price to the view
                ViewBag.TotalPrice = totalPrice;
                return View(pastOrders);
            }
            else
            {
                
                return RedirectToAction("Login", "Account");
            }
        }




    }
}
