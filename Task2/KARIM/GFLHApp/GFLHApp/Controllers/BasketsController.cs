// ----- Imports -----
using System; // Provides core .NET types used by this controller.
using System.Collections.Generic; // Provides collection types such as lists and dictionaries.
using System.Linq; // Provides LINQ filtering, grouping, and sorting helpers.
using System.Threading.Tasks; // Provides asynchronous Task return types.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.AspNetCore.Mvc.Rendering; // Provides SelectList helpers for form dropdowns.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using System.Security.Claims; // Provides access to the signed-in user's claim values.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{
    // Role management
    [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.

    // ----- Controller Declaration -----
    public class BasketsController : Controller // Defines the MVC controller for customer basket display, totals, and basket record maintenance.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.

        public BasketsController(ApplicationDbContext context) // Receives dependencies from dependency injection and stores them on the controller.
        {
            _context = context; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Basket lookup
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Fetches the first matching record or null if none exists.

            if (basket == null) // Checks whether the requested data was found.
            {
                // Create basket
                basket = new Basket // Creates the basket object used by the following workflow.
                {
                    // Set user ID
                    UserId = userId, // Sets UserId to the value needed by the workflow.
                    Status = true, // Sets Status to the value needed by the workflow.
                    CreatedAt = DateTime.UtcNow // Sets CreatedAt to the value needed by the workflow.
                };

                _context.Basket.Add(basket); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            }

            // Basket lookup
            var basketProducts = await _context.BasketProducts // Sets basketProducts to the value needed by the workflow.
                .Where(bp => bp.BasketId == basket.BasketId) // Filters the query to only the relevant records.
                .Include(bp => bp.Basket) // Includes related records needed by the view or workflow.
                .Include(bp => bp.Products) // Includes related records needed by the view or workflow.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Basket totals
            decimal subtotal = 0m; // Calculates the subtotal value used for totals.

            foreach (var basketProduct in basketProducts) // Iterates through each matching item to process it.
            {
                var productTotal = basketProduct.Products.ItemPrice * basketProduct.ProductQuantity; // Calculates the productTotal value used for totals.
                subtotal += productTotal; // Sets + to the value needed by the workflow.
            }

            // Loyalty system
            var orderCount = await _context.Orders.CountAsync(o => o.UserId == userId); // Counts matching records for display or validation.

            var productNames = basketProducts.Select(x => x.Products.ItemName.ToLower()).ToList(); // Projects the query results into the shape needed next.
            // Health bundle
            bool hasHealthBundle = productNames.Contains("broccoli") && // Starts the health bundle check by looking for broccoli.
                                   productNames.Contains("carrot") && // Continues the health bundle check by requiring carrot.
                                   productNames.Contains("apple"); // Completes the health bundle check by requiring apple.

            decimal discount = 0m; // Calculates the discount value used for totals.

            if (orderCount % 5 == 4) // Checks the condition that controls the next action.
            {
                discount = subtotal * 0.15m; // Calculates the discount value used for totals.
            }
            // Health bundle
            else if (hasHealthBundle) // Checks the next fallback condition in the workflow.
            {
                discount = subtotal * 0.10m; // Calculates the discount value used for totals.
            }

            decimal total = subtotal - discount; // Calculates the total value used for totals.

            // Basket totals
            ViewBag.Subtotal = subtotal; // Supplies Subtotal to the view for display or form state.
            ViewBag.Discount = discount; // Supplies Discount to the view for display or form state.
            ViewBag.Total = total; // Supplies Total to the view for display or form state.
            // Loyalty system
            ViewBag.OrderCount = orderCount; // Supplies OrderCount to the view for display or form state.
            // Health bundle
            ViewBag.HasHealthBundle = hasHealthBundle; // Supplies HasHealthBundle to the view for display or form state.

            return View(basketProducts); // Renders the matching view with the supplied model data.
        }

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var basket = await _context.Basket // Sets basket to the value needed by the workflow.
                // Basket lookup
                .FirstOrDefaultAsync(m => m.BasketId == id); // Fetches the first matching record or null if none exists.
            if (basket == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(basket); // Renders the matching view with the supplied model data.
        }

        // ----- Create Actions -----
        // Create record
        public IActionResult Create() // Shows or processes the create form for a record
        {
            return View(); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Basket lookup
        public async Task<IActionResult> Create([Bind("BasketId,UserId,Status,CreatedAt")] Basket basket) // Shows or processes the create form for a record
        {
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Create record
                _context.Add(basket); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
            }
            return View(basket); // Renders the matching view with the supplied model data.
        }

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var basket = await _context.Basket.FindAsync(id); // Looks up the record by its primary key.
            if (basket == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }
            return View(basket); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Basket lookup
        public async Task<IActionResult> Edit(int id, [Bind("BasketId,UserId,Status,CreatedAt")] Basket basket) // Shows or processes edits for an existing record
        {
            if (id != basket.BasketId) // Checks the condition that controls the next action.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Form validation
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                try // Starts a protected database update block.
                {
                    // Edit record
                    _context.Update(basket); // Marks the entity as modified so EF Core saves its changes.
                    await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                }
                catch (DbUpdateConcurrencyException) // Handles database concurrency errors from the update attempt.
                {
                    // Basket lookup
                    if (!BasketExists(basket.BasketId)) // Checks the condition that controls the next action.
                    {
                        return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
                    }
                    else // Handles the fallback branch when earlier conditions did not match.
                    {
                        throw; // Rethrows unexpected concurrency failures for higher-level handling.
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
            }
            return View(basket); // Renders the matching view with the supplied model data.
        }

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var basket = await _context.Basket // Sets basket to the value needed by the workflow.
                // Basket lookup
                .FirstOrDefaultAsync(m => m.BasketId == id); // Fetches the first matching record or null if none exists.
            if (basket == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(basket); // Renders the matching view with the supplied model data.
        }

        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            var basket = await _context.Basket.FindAsync(id); // Looks up the record by its primary key.
            if (basket != null) // Runs the next step only when the record exists.
            {
                // Remove basket
                _context.Basket.Remove(basket); // Queues the entity for removal from the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Existence Helpers -----
        private bool BasketExists(int id) // Checks whether a basket record still exists
        {
            // Basket lookup
            return _context.Basket.Any(e => e.BasketId == id); // Returns the computed result to the caller.
        }
    }
}
