using GFLHApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GFLHApp.Controllers
{


    [Authorize(Roles = "Producer")] // This attribute ensures that only users with the "Producer" role can access the actions in this controller.
    public class ProducerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProducerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Find the producer associated with the current user

            if (producer == null)
            {
                return NotFound(); // Return a 404 Not Found response if the producer doesn't exist
            }

            var products = await _context.Products.Where(p => p.ProducersId == producer.ProducersId).ToListAsync(); // Get the products associated with the producer

            var orders = await _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Products).Where(o => o.OrderProducts.Any(op => op.Products.ProducersId == producer.ProducersId)).ToListAsync(); // Get the orders that include products from the producer)

            ViewBag.TotalProducts = products.Count; // Pass the total number of products to the view using ViewBag
            ViewBag.LowStockCount = products.Count(p => p.QuantityInStock <= 5); // Pass the count of low stock products to the view using ViewBag
            ViewBag.RecentOrders = orders; // Pass the recent orders to the view using ViewBag


            return View(products);
        }
    }
}
