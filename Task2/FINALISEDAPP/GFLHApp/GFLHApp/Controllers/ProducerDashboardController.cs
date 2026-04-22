// ----- Imports -----
using GFLHApp.Data; // Provides the application database context.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.
using System.Security.Claims; // Provides access to the signed-in user's claim values.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{
    // Role management
    [Authorize(Roles = "Producer,Developer")] // Restricts access to users in the Producer,Developer role set.

    // ----- Controller Declaration -----
    public class ProducerDashboardController : Controller // Defines the MVC controller for producer dashboard metrics and producer order workflow actions.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.

        public ProducerDashboardController(ApplicationDbContext context) // Receives dependencies from dependency injection and stores them on the controller.
        {
            _context = context; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            // Producer lookup
            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Fetches the first matching record or null if none exists.

            if (producer == null) return NotFound(); // Checks whether the requested data was found.

            // Product lookup
            var products = await _context.Products // Sets products to the value needed by the workflow.
                .Where(p => p.ProducersId == producer.ProducersId) // Filters the query to only the relevant records.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Producer order splitting
            var producerOrders = await _context.ProducerOrders // Sets producerOrders to the value needed by the workflow.
                .Where(x => x.ProducerId == userId) // Filters the query to only the relevant records.
                .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                .Include(x => x.OrderProducts) // Includes related records needed by the view or workflow.
                    .ThenInclude(x => x.Products) // Includes nested related records for the query result.
                // Order logic
                .OrderByDescending(x => x.Orders.OrderDate) // Sorts the query results for display or processing.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            var now = DateTime.UtcNow; // Sets now to the value needed by the workflow.

            // Producer lookup
            ViewBag.ProducerName = producer.ProducerName; // Supplies ProducerName to the view for display or form state.
            // Basket totals
            ViewBag.TotalProducts = products.Count; // Supplies TotalProducts to the view for display or form state.
            // Availability check
            ViewBag.LowStockCount = products.Count(p => p.QuantityInStock <= 5 && p.Available); // Counts matching records for display or validation.
            ViewBag.TotalStock = products.Sum(p => p.QuantityInStock); // Calculates the total from the matching records.
            // Order status workflow
            ViewBag.PendingCount = producerOrders.Count(o => o.TrackingStatus == "Pending"); // Counts matching records for display or validation.
            ViewBag.TotalRevenue = producerOrders.Where(o => o.TrackingStatus == "Accepted").Sum(o => o.ProducerSubtotal); // Calculates the total from the matching records.
            // Producer order splitting
            ViewBag.ThisMonthRevenue = producerOrders // Supplies ThisMonthRevenue to the view for display or form state.
                .Where(o => o.TrackingStatus == "Accepted" // Filters the query to only the relevant records.
                         // Order logic
                         && o.Orders.OrderDate.Month == now.Month // Requires the order to belong to the current month.
                         && o.Orders.OrderDate.Year == now.Year) // Requires the order to belong to the current year.
                .Sum(o => o.ProducerSubtotal); // Calculates the total from the matching records.
            ViewBag.ProducerOrders = producerOrders; // Supplies ProducerOrders to the view for display or form state.

            return View(products); // Renders the matching view with the supplied model data.
        }

        // ----- Producer Order Views -----
        public async Task<IActionResult> AllOrders() // Loads all producer order slices for the signed-in producer
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            // Producer order splitting
            var producerOrders = await _context.ProducerOrders // Sets producerOrders to the value needed by the workflow.
                .Where(x => x.ProducerId == userId) // Filters the query to only the relevant records.
                .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                .Include(x => x.OrderProducts) // Includes related records needed by the view or workflow.
                    .ThenInclude(x => x.Products) // Includes nested related records for the query result.
                // Order logic
                .OrderByDescending(x => x.Orders.OrderDate) // Sorts the query results for display or processing.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Producer order splitting
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Producer Product Actions -----
        // Availability check
        public async Task<IActionResult> ToggleAvailability(int id) // Toggles one of the signed-in producer's products and returns JSON
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            // Producer lookup
            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Fetches the first matching record or null if none exists.
            if (producer == null) return Json(new { success = false }); // Checks whether the requested data was found.

            // Product lookup
            var product = await _context.Products // Sets product to the value needed by the workflow.
                .FirstOrDefaultAsync(p => p.ProductsId == id && p.ProducersId == producer.ProducersId); // Fetches the first matching record or null if none exists.
            if (product == null) return Json(new { success = false }); // Checks whether the requested data was found.

            // Availability check
            product.Available = !product.Available; // Sets product.Available to the value needed by the workflow.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return Json(new { success = true, available = product.Available }); // Returns a JSON response for client-side code.
        }

        // ----- Order Status Helpers -----
        // Order logic
        private async Task RecalculateOrderStatus(int ordersId) // Refreshes the parent order status after the producer slice changed.
        {
            // Producer order splitting
            var allSlices = await _context.ProducerOrders // Loads every producer slice attached to the parent order.
                .Where(x => x.OrdersId == ordersId) // Filters the query to only the relevant records.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            var order = await _context.Orders.FindAsync(ordersId); // Looks up the record by its primary key.
            if (order == null) return; // Checks whether the requested data was found.

            // Order status workflow
            bool allCancelled = allSlices.All(x => x.TrackingStatus == "Cancelled"); // Checks whether every producer slice has been cancelled.
            bool allAccepted = allSlices.All(x => x.TrackingStatus == "Accepted"); // Checks whether every producer slice has been accepted.
            bool anyCancelled = allSlices.Any(x => x.TrackingStatus == "Cancelled"); // Checks whether at least one producer slice has been cancelled.
            bool anyAccepted = allSlices.Any(x => x.TrackingStatus == "Accepted"); // Checks whether at least one producer slice has been accepted.

            if (allCancelled) // Checks the condition that controls the next action.
                // Order logic
                order.OrderStatus = "Cancelled"; // Sets order.OrderStatus to the value needed by the workflow.
            else if (allAccepted) // Checks the next fallback condition in the workflow.
                order.OrderStatus = "Accepted"; // Sets order.OrderStatus to the value needed by the workflow.
            // Order status workflow
            else if (anyCancelled && anyAccepted) // Checks the next fallback condition in the workflow.
                order.OrderStatus = "Partially Complete"; // Sets order.OrderStatus to the value needed by the workflow.
            else // Handles the fallback branch when earlier conditions did not match.
                order.OrderStatus = "Pending"; // Sets order.OrderStatus to the value needed by the workflow.

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        [ActionName("CancelProducerOrder")] // Exposes this action under the specified route action name.

        // ----- Producer Cancellation Actions -----
        public async Task<IActionResult> CancelProducerOrderPost(int id) // Cancels an entire producer order slice and restocks its products
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            // Producer order splitting
            var producerOrder = await _context.ProducerOrders // Sets producerOrder to the value needed by the workflow.
                // Order logic
                .Where(x => x.ProducerOrdersId == id && x.ProducerId == userId) // Filters the query to only the relevant records.
                .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                .Include(x => x.OrderProducts) // Includes related records needed by the view or workflow.
                    .ThenInclude(x => x.Products) // Includes nested related records for the query result.
                .FirstOrDefaultAsync(); // Fetches the first matching record or null if none exists.

            if (producerOrder == null) return NotFound(); // Checks whether the requested data was found.
            // Order status workflow
            if (producerOrder.TrackingStatus == "Cancelled") return RedirectToAction("Index"); // Checks the condition that controls the next action.

            foreach (var item in producerOrder.OrderProducts) // Iterates through each matching item to process it.
            {
                // Stock checks
                item.Products.QuantityInStock += item.ProductQuantity; // Sets + to the value needed by the workflow.
                // Basket totals
                producerOrder.Orders.OrdersTotal -= item.Products.ItemPrice * item.ProductQuantity; // Sets - to the value needed by the workflow.
            }

            // Producer order splitting
            producerOrder.ProducerSubtotal = 0; // Calculates the producerOrder.ProducerSubtotal value used for totals.
            // Order status workflow
            producerOrder.TrackingStatus = "Cancelled"; // Sets producerOrder.TrackingStatus to the value needed by the workflow.
            // Delete record
            _context.OrderProducts.RemoveRange(producerOrder.OrderProducts); // Queues the selected entities for removal from the database.

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            // Order logic
            await RecalculateOrderStatus(producerOrder.OrdersId); // Refreshes the parent order status after the producer slice changed.

            return RedirectToAction("Index"); // Redirects the browser to the next MVC action.
        }

        public async Task<IActionResult> CancelProducerOrder(int? id) // Loads the confirmation screen for cancelling a producer order slice
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            // Producer order splitting
            var producerOrder = await _context.ProducerOrders // Sets producerOrder to the value needed by the workflow.
                // Order logic
                .Where(x => x.ProducerOrdersId == id && x.ProducerId == userId) // Filters the query to only the relevant records.
                .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                .Include(x => x.OrderProducts) // Includes related records needed by the view or workflow.
                    .ThenInclude(x => x.Products) // Includes nested related records for the query result.
                .FirstOrDefaultAsync(); // Fetches the first matching record or null if none exists.

            if (producerOrder == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Order status workflow
            if (producerOrder.TrackingStatus == "Cancelled") // Checks the condition that controls the next action.
            {
                return RedirectToAction("Index"); // Redirects the browser to the next MVC action.
            }

            return View(producerOrder); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        public async Task<IActionResult> CancelOrderItem(int id) // Cancels one line item from a producer order slice and restocks it
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            var orderProduct = await _context.OrderProducts // Sets orderProduct to the value needed by the workflow.
                // Product lookup
                .Where(x => x.OrderProductsId == id) // Filters the query to only the relevant records.
                .Include(x => x.Products) // Includes related records needed by the view or workflow.
                // Producer order splitting
                .Include(x => x.ProducerOrders) // Includes related records needed by the view or workflow.
                    .ThenInclude(x => x.Orders) // Includes nested related records for the query result.
                .FirstOrDefaultAsync(); // Fetches the first matching record or null if none exists.

            if (orderProduct == null || orderProduct.ProducerOrders.ProducerId != userId) // Checks that a signed-in user id is available.
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.

            // Stock checks
            orderProduct.Products.QuantityInStock += orderProduct.ProductQuantity; // Sets + to the value needed by the workflow.

            var lineTotal = orderProduct.Products.ItemPrice * orderProduct.ProductQuantity; // Calculates the lineTotal value used for totals.
            // Producer order splitting
            orderProduct.ProducerOrders.ProducerSubtotal -= lineTotal; // Sets - to the value needed by the workflow.
            // Basket totals
            orderProduct.ProducerOrders.Orders.OrdersTotal -= lineTotal; // Sets - to the value needed by the workflow.

            var remainingItems = await _context.OrderProducts // Sets remainingItems to the value needed by the workflow.
                // Order logic
                .CountAsync(x => x.ProducerOrdersId == orderProduct.ProducerOrdersId // Counts matching records for display or validation.
                              // Product lookup
                              && x.OrderProductsId != id); // Excludes the item currently being cancelled from the remaining-item count.

            if (remainingItems == 0) // Checks the condition that controls the next action.
                // Order status workflow
                orderProduct.ProducerOrders.TrackingStatus = "Cancelled"; // Sets orderProduct.ProducerOrders.TrackingStatus to the value needed by the workflow.

            int ordersId = orderProduct.ProducerOrders.OrdersId; // Sets ordersId to the value needed by the workflow.
            // Delete record
            _context.OrderProducts.Remove(orderProduct); // Queues the entity for removal from the database.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            // Order logic
            await RecalculateOrderStatus(ordersId); // Refreshes the parent order status after the producer slice changed.

            return RedirectToAction("Index"); // Redirects the browser to the next MVC action.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Producer Acceptance Actions -----
        public async Task<IActionResult> AcceptProducerOrder(int id) // Marks a producer order slice as accepted and refreshes the parent order status
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            // Producer order splitting
            var producerOrder = await _context.ProducerOrders // Sets producerOrder to the value needed by the workflow.
                .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                // Order logic
                .FirstOrDefaultAsync(x => x.ProducerOrdersId == id && x.ProducerId == userId); // Fetches the first matching record or null if none exists.

            if (producerOrder == null) // Checks whether the requested data was found.
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.

            // Order status workflow
            if (producerOrder.TrackingStatus == "Pending") // Checks the condition that controls the next action.
            {
                producerOrder.TrackingStatus = "Accepted"; // Sets producerOrder.TrackingStatus to the value needed by the workflow.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                // Order logic
                await RecalculateOrderStatus(producerOrder.OrdersId); // Refreshes the parent order status after the producer slice changed.
            }

            return RedirectToAction("Index"); // Redirects the browser to the next MVC action.
        }
    }
}
