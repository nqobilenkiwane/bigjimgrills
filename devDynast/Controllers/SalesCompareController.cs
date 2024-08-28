using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using devDynast.Data;
using devDynast.Models;
using System.Linq;
using System.Threading.Tasks;

namespace devDynast.Controllers
{
    public class SalesCompareController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesCompareController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SalesCompareReport()
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
    }
}
