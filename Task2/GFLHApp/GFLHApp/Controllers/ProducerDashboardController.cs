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
        private readonly ApplicationDbContext _context; // load the application database context to interact with the database and perform CRUD operations on producers, products, orders, etc.

        public ProducerDashboardController(ApplicationDbContext context) // constructor that takes the application db context as a parameter and assigns it to the private field _context, allowing the controller to interact with the database throughout its actions.
        {
            _context = context; // make it so _context is the database of the application, so we can use it to query for producers, products, orders, etc.
        }

        // GET: ProducerDashboard - This action method retrieves the current producer's products and orders, calculates some summary statistics, and passes all this data to the view for display on the producer dashboard.
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


        // This method recalculates the overall order status based on the statuses of all producer slices for a given order. It checks if all slices are cancelled, all accepted, or a mix of both to determine the final order status.
        private async Task RecalculateOrderStatus(int ordersId)
        {
            var allSlices = await _context.ProducerOrders // Get all producer slices for the given order ID
                .Where(x => x.OrdersId == ordersId) // Filter by the order ID
                .ToListAsync(); // Execute the query and get the list of producer slices

            var order = await _context.Orders.FindAsync(ordersId); // Find the parent order by its ID
            if (order == null) return; //   If the order is not found, exit the method

            bool allCancelled = allSlices.All(x => x.TrackingStatus == "Cancelled"); // Check if all producer slices have a tracking status of "Cancelled"
            bool allAccepted = allSlices.All(x => x.TrackingStatus == "Accepted"); // Check if all producer slices have a tracking status of "Accepted"
            bool anyCancelled = allSlices.Any(x => x.TrackingStatus == "Cancelled"); //     Check if any producer slice has a tracking status of "Cancelled"
            bool anyAccepted = allSlices.Any(x => x.TrackingStatus == "Accepted"); //    Check if any producer slice has a tracking status of "Accepted"

            if (allCancelled) // If all producer slices are cancelled, set the order status to "Cancelled"
                order.OrderStatus = "Cancelled";
            else if (allAccepted) // If all producer slices are accepted, set the order status to "Accepted"
                order.OrderStatus = "Accepted";
            else if (anyCancelled && anyAccepted) // If there is a mix of cancelled and accepted slices, set the order status to "Partially Complete"
                order.OrderStatus = "Partially Complete";
            else // If there are still pending slices (not all accepted or cancelled), set the order status to "Pending"
                order.OrderStatus = "Pending";

            await _context.SaveChangesAsync(); // Save the changes to the database to update the order status
        }

        // GET: ProducerDashboard/CancelProducerOrder/5
        // This action method retrieves a specific producer order slice based on the provided ID, checks if it belongs to the current producer, and if it's not already cancelled, it returns a view to confirm the cancellation of that producer order slice.
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

        // This action method handles the cancellation of a specific order item (order product) within a producer order slice. It checks if the order product belongs to the current producer, restocks the product quantity, updates the producer and order totals, checks if the entire producer slice should be cancelled, removes the order product from the database, and then recalculates the overall order status before redirecting back to the dashboard.
        public async Task<IActionResult> CancelOrderItem(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            var orderProduct = await _context.OrderProducts // Find the order product by its ID, ensuring it belongs to the current producer through the producer order relationship
                .Where(x => x.OrderProductsId == id) // Filter by the order product ID
                .Include(x => x.Products) // Include the product to restock
                .Include(x => x.ProducerOrders) // Include the producer order to update totals and check status
                    .ThenInclude(x => x.Orders) // Include the parent order to update totals and check status
                .FirstOrDefaultAsync(); // Execute the query and get the order product

            if (orderProduct == null || orderProduct.ProducerOrders.ProducerId != userId) // Check if the order product exists and belongs to the current producer
                return NotFound(); // Return a 404 Not Found response if the order product doesn't exist or doesn't belong to the producer

            // Restock
            orderProduct.Products.QuantityInStock += orderProduct.ProductQuantity; // Increase the product's stock quantity by the quantity of the cancelled order product

            // Deduct totals
            var lineTotal = orderProduct.Products.ItemPrice * orderProduct.ProductQuantity; // Calculate the total price of the cancelled order product line (price multiplied by quantity)
            orderProduct.ProducerOrders.ProducerSubtotal -= lineTotal; // Deduct the line total from the producer order's subtotal
            orderProduct.ProducerOrders.Orders.OrdersTotal -= lineTotal; // Deduct the line total from the overall order's total

            // Check remaining items in this producer slice
            var remainingItems = await _context.OrderProducts // Query the order products to count how many items remain in this producer slice after the cancellation
                .CountAsync(x => x.ProducerOrdersId == orderProduct.ProducerOrdersId // Count how many order products remain in this producer slice
                              && x.OrderProductsId != id); // Exclude the cancelled order product from the count

            if (remainingItems == 0)
                orderProduct.ProducerOrders.TrackingStatus = "Cancelled"; // If there are no remaining items in this producer slice, set the tracking status of the producer order to "Cancelled"

            int ordersId = orderProduct.ProducerOrders.OrdersId; // Store the parent order ID before we remove the order product, so we can recalculate the order status afterwards
            _context.OrderProducts.Remove(orderProduct); // Remove the cancelled order product from the database
            await _context.SaveChangesAsync(); // Save the changes to the database to update stock, totals, and remove the order product

            await RecalculateOrderStatus(ordersId); // Recalculate the overall order status based on the statuses of all producer slices for this order, in case cancelling this item has changed the status of the producer slice or the entire order

            return RedirectToAction("Index"); // Redirect back to the producer dashboard index after cancelling the order item
        }

        // POST: ProducerDashboard/AcceptProducerOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        // This action method handles the acceptance of a producer order slice. It checks if the producer order slice belongs to the current producer and is in a "Pending" state, updates its tracking status to "Accepted", saves the changes, recalculates the overall order status, and then redirects back to the dashboard.
        public async Task<IActionResult> AcceptProducerOrder(int id) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            var producerOrder = await _context.ProducerOrders // Find the producer order slice by its ID, ensuring it belongs to the current producer through the producer ID
                .Include(x => x.Orders) // Include the parent order to update status if needed
                .FirstOrDefaultAsync(x => x.ProducerOrdersId == id && x.ProducerId == userId); // Filter by the producer order ID and ensure it belongs to the current producer

            if (producerOrder == null) // Check if the producer order exists and belongs to the current producer
                return NotFound(); // Return a 404 Not Found response if the producer order doesn't exist or doesn't belong to the producer

            if (producerOrder.TrackingStatus == "Pending") // Only allow accepting if the current status is "Pending"
            {
                producerOrder.TrackingStatus = "Accepted"; // Set the tracking status of the producer order slice to "Accepted"
                await _context.SaveChangesAsync(); // Save the changes to the database to update the tracking status
                await RecalculateOrderStatus(producerOrder.OrdersId); // Recalculate the overall order status based on the statuses of all producer slices for this order, in case accepting this slice has changed the status of the entire order
            }

            return RedirectToAction("Index"); // Redirect back to the producer dashboard index after accepting the producer order slice
        }
    }
}