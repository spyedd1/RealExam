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

            var producerOrders = await _context.ProducerOrders
                .Where(x => x.ProducerId == userId) // Only get this producer's slices
                .Include(x => x.Orders) // Include the parent order for date and order ID
                .Include(x => x.OrderProducts) // Include the order products in this slice
                    .ThenInclude(x => x.Products) // Include the products for display
                .ToListAsync(); // Get the producer order slices

            ViewBag.TotalProducts = products.Count; // Pass the total number of products to the view using ViewBag
            ViewBag.LowStockCount = products.Count(p => p.QuantityInStock <= 5); // Pass the count of low stock products to the view using ViewBag
            ViewBag.TotalStock = products.Sum(p => p.QuantityInStock); // Pass the total stock units to the view using ViewBag
            ViewBag.ProducerOrders = producerOrders; // Pass the producer order slices to the view using ViewBag

            return View(products);
        }
        // GET: ProducerDashboard/CancelProducerOrder/5
        public async Task<IActionResult> CancelProducerOrder(int? id)
        {
            if (id == null) // Check if the ID is provided
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            var producerOrder = await _context.ProducerOrders
                .Where(x => x.ProducerOrdersId == id && x.ProducerId == userId) // Only allow the producer to view their own slice
                .Include(x => x.Orders) // Include the parent order
                .Include(x => x.OrderProducts) // Include the order products in this slice
                    .ThenInclude(x => x.Products) // Include the products for display
                .FirstOrDefaultAsync();

            if (producerOrder == null) // Check if the producer order exists and belongs to the current producer
            {
                return NotFound();
            }

            if (producerOrder.TrackingStatus == "Cancelled") // Check if the producer order is already cancelled
            {
                return RedirectToAction("Index"); // Redirect back if already cancelled
            }

            return View(producerOrder); // Return the confirmation view with the producer order data
        }

        // POST: ProducerDashboard/CancelOrderItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrderItem(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderProduct = await _context.OrderProducts
                .Where(x => x.OrderProductsId == id)
                .Include(x => x.Products)
                .Include(x => x.ProducerOrders)
                    .ThenInclude(x => x.Orders)
                .FirstOrDefaultAsync();

            if (orderProduct == null || orderProduct.ProducerOrders.ProducerId != userId)
            {
                return NotFound();
            }

            // Restock the item
            orderProduct.Products.QuantityInStock += orderProduct.ProductQuantity;

            // Deduct from producer subtotal and order total
            var lineTotal = orderProduct.Products.ItemPrice * orderProduct.ProductQuantity;
            orderProduct.ProducerOrders.ProducerSubtotal -= lineTotal;
            orderProduct.ProducerOrders.Orders.OrdersTotal -= lineTotal;

            // If no items remain, cancel the whole producer slice
            var remainingItems = await _context.OrderProducts
                .CountAsync(x => x.ProducerOrdersId == orderProduct.ProducerOrdersId && x.OrderProductsId != id);

            if (remainingItems == 0)
            {
                orderProduct.ProducerOrders.TrackingStatus = "Cancelled";
            }

            _context.OrderProducts.Remove(orderProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}