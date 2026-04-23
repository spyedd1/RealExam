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
    public class BasketProductsController : Controller // Defines the MVC controller for adding, removing, and counting products inside customer baskets.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.

        public BasketProductsController(ApplicationDbContext context) // Receives dependencies from dependency injection and stores them on the controller.
        {
            _context = context; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            // Basket lookup
            var applicationDbContext = _context.BasketProducts.Include(b => b.Basket).Include(b => b.Products); // Includes related records needed by the view or workflow.
            return View(await applicationDbContext.ToListAsync()); // Renders the matching view with the supplied model data.
        }

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {

            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Basket lookup
            var basketProducts = await _context.BasketProducts // Sets basketProducts to the value needed by the workflow.
                .Include(b => b.Basket) // Includes related records needed by the view or workflow.
                .Include(b => b.Products) // Includes related records needed by the view or workflow.
                // Product lookup
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Fetches the first matching record or null if none exists.
            if (basketProducts == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(basketProducts); // Renders the matching view with the supplied model data.
        }

        // ----- Create Actions -----
        // Create record
        public IActionResult Create() // Shows or processes the create form for a record
        {
            // Basket lookup
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId"); // Supplies dropdown data to the view.
            // Product lookup
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId"); // Supplies dropdown data to the view.
            return View(); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        public async Task<IActionResult> Create(int ProductsId) // Shows or processes the create form for a record
        {
            // Product lookup
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId); // Fetches the first matching record or null if none exists.

            if (product == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Availability check
            if (!product.Available) // Checks the condition that controls the next action.
            {
                TempData["Error"] = "This product is currently unavailable and cannot be added to your basket."; // Stores a one-request notification message for the redirected page.
                return RedirectToAction("Index", "Products"); // Redirects the browser to the next MVC action.
            }

            if (product.QuantityInStock <= 0) // Checks whether the product has any stock remaining.
            {
                TempData["Error"] = "This product is out of stock and cannot be added to your basket."; // Stores a one-request notification message for the redirected page.
                return RedirectToAction("Index", "Products"); // Redirects the browser to the next MVC action.
            }

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
                    Status = true, // Sets Status to the value needed by the workflow.
                    // Set user ID
                    UserId = userId, // Sets UserId to the value needed by the workflow.
                    CreatedAt = DateTime.UtcNow // Sets CreatedAt to the value needed by the workflow.
                };

                _context.Basket.Add(basket); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            }

            // Basket lookup
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId); // Fetches the first matching record or null if none exists.

            if (basketProduct != null) // Runs the next step only when the record exists.
            {
                if (basketProduct.ProductQuantity >= product.QuantityInStock) // Checks whether the basket line is already at the stock limit.
                {
                    TempData["Error"] = $"You can only add up to {product.QuantityInStock} of {product.ItemName}."; // Stores a one-request notification message for the redirected page.
                    return RedirectToAction("Index", "Products"); // Redirects the browser to the next MVC action.
                }

                // Add to basket
                basketProduct.ProductQuantity++; // Increases the existing basket line quantity by one.
            }
            else // Handles the fallback branch when earlier conditions did not match.
            {
                basketProduct = new BasketProducts // Creates the basketProduct object used by the following workflow.
                {
                    // Basket lookup
                    BasketId = basket.BasketId, // Sets BasketId to the value needed by the workflow.
                    // Product lookup
                    ProductsId = ProductsId, // Sets ProductsId to the value needed by the workflow.
                    ProductQuantity = 1 // Sets ProductQuantity to the value needed by the workflow.
                };
                _context.BasketProducts.Add(basketProduct); // Queues the new entity to be inserted into the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            return RedirectToAction("Index", "Baskets"); // Redirects the browser to the next MVC action.
        }

        [HttpGet] // Marks the following action as handling GET requests.

        // ----- Basket Badge Actions -----
        public async Task<IActionResult> GetCount() // Returns the current user's live basket item count as JSON
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            if (userId == null) return Json(new { count = 0 }); // Checks that a signed-in user id is available.

            // Basket lookup
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Fetches the first matching record or null if none exists.
            if (basket == null) return Json(new { count = 0 }); // Checks whether the requested data was found.

            var count = await _context.BasketProducts // Sets count to the value needed by the workflow.
                .Where(bp => bp.BasketId == basket.BasketId) // Filters the query to only the relevant records.
                .SumAsync(bp => bp.ProductQuantity); // Calculates the total from the matching records.

            return Json(new { count }); // Returns a JSON response for client-side code.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Ajax Basket Actions -----
        // Add to basket
        public async Task<IActionResult> AddAjax(int ProductsId, int Quantity = 1) // Adds a product quantity to the basket and returns the updated count as JSON
        {
            if (Quantity < 1) Quantity = 1; // Checks the condition that controls the next action.
            if (Quantity > 99) Quantity = 99; // Checks the condition that controls the next action.

            // Product lookup
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId); // Fetches the first matching record or null if none exists.
            if (product == null) // Checks whether the requested data was found.
                return Json(new { success = false, message = "Product not found." }); // Returns a JSON response for client-side code.

            // Availability check
            if (!product.Available) // Checks the condition that controls the next action.
                return Json(new { success = false, message = "This product is currently unavailable." }); // Returns a JSON response for client-side code.

            if (product.QuantityInStock <= 0) // Checks whether the product has any stock remaining.
                return Json(new { success = false, message = "This product is out of stock." }); // Returns a JSON response for client-side code.

            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            if (userId == null) // Checks that a signed-in user id is available.
                return Json(new { success = false, message = "Please log in to add items to your basket." }); // Returns a JSON response for client-side code.

            // Basket lookup
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Fetches the first matching record or null if none exists.
            if (basket == null) // Checks whether the requested data was found.
            {
                basket = new Basket { Status = true, UserId = userId, CreatedAt = DateTime.UtcNow }; // Creates the basket object used by the following workflow.
                // Create basket
                _context.Basket.Add(basket); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            }

            // Basket lookup
            var basketProduct = await _context.BasketProducts // Sets basketProduct to the value needed by the workflow.
                .FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId); // Fetches the first matching record or null if none exists.

            var existingQuantity = basketProduct?.ProductQuantity ?? 0; // Reads the current basket quantity for stock validation.
            var remainingStock = product.QuantityInStock - existingQuantity; // Calculates how many more units can still be added.

            if (remainingStock <= 0) // Checks whether the user has already reached the stock limit for this basket line.
                return Json(new { success = false, message = $"You already have the maximum available stock of {product.ItemName} in your basket." }); // Returns a JSON response for client-side code.

            if (Quantity > remainingStock) // Checks whether the request would exceed the remaining stock.
                return Json(new { success = false, message = $"You can only add {remainingStock} more of {product.ItemName}." }); // Returns a JSON response for client-side code.

            if (basketProduct != null) // Runs the next step only when the record exists.
                // Add to basket
                basketProduct.ProductQuantity += Quantity; // Sets + to the value needed by the workflow.
            else // Handles the fallback branch when earlier conditions did not match.
                _context.BasketProducts.Add(new BasketProducts // Queues the new entity to be inserted into the database.
                {
                    // Basket lookup
                    BasketId = basket.BasketId, // Sets BasketId to the value needed by the workflow.
                    // Product lookup
                    ProductsId = ProductsId, // Sets ProductsId to the value needed by the workflow.
                    ProductQuantity = Quantity // Sets ProductQuantity to the value needed by the workflow.
                });

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            var basketCount = await _context.BasketProducts // Sets basketCount to the value needed by the workflow.
                .Where(bp => bp.BasketId == basket.BasketId) // Filters the query to only the relevant records.
                .SumAsync(bp => bp.ProductQuantity); // Calculates the total from the matching records.

            return Json(new { success = true, basketCount, itemName = product.ItemName, quantity = Quantity, maxStock = product.QuantityInStock, basketQuantity = existingQuantity + Quantity }); // Returns a JSON response for client-side code.
        }

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Basket lookup
            var basketProducts = await _context.BasketProducts.FindAsync(id); // Looks up the record by its primary key.
            if (basketProducts == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Supplies dropdown data to the view.
            // Product lookup
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", basketProducts.ProductsId); // Supplies dropdown data to the view.
            return View(basketProducts); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Basket lookup
        public async Task<IActionResult> Edit(int id, [Bind("BasketProductsId,BasketId,ProductsId,ProductQuantity")] BasketProducts basketProducts) // Shows or processes edits for an existing record
        {
            // Product lookup
            if (id != basketProducts.BasketProductsId) // Checks the condition that controls the next action.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Form validation
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                try // Starts a protected database update block.
                {
                    // Edit record
                    _context.Update(basketProducts); // Marks the entity as modified so EF Core saves its changes.
                    await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                }
                catch (DbUpdateConcurrencyException) // Handles database concurrency errors from the update attempt.
                {
                    // Product lookup
                    if (!BasketProductsExists(basketProducts.BasketProductsId)) // Checks the condition that controls the next action.
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
            // Basket lookup
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId); // Supplies dropdown data to the view.
            // Product lookup
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", basketProducts.ProductsId); // Supplies dropdown data to the view.
            return View(basketProducts); // Renders the matching view with the supplied model data.
        }

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Basket lookup
            var basketProducts = await _context.BasketProducts // Sets basketProducts to the value needed by the workflow.
                .Include(b => b.Basket) // Includes related records needed by the view or workflow.
                .Include(b => b.Products) // Includes related records needed by the view or workflow.
                // Product lookup
                .FirstOrDefaultAsync(m => m.BasketProductsId == id); // Fetches the first matching record or null if none exists.
            if (basketProducts == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(basketProducts); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Basket Item Actions -----
        // Remove basket
        public async Task<IActionResult> RemoveFromBasket(int id) // Removes a basket item owned by the signed-in user
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Basket lookup
            var basketProduct = await _context.BasketProducts // Sets basketProduct to the value needed by the workflow.
                .Include(bp => bp.Basket) // Includes related records needed by the view or workflow.
                // Product lookup
                .FirstOrDefaultAsync(bp => bp.BasketProductsId == id // Fetches the first matching record or null if none exists.
                    && bp.Basket != null // Requires the basket relationship to exist before checking ownership.
                    && bp.Basket.UserId == userId // Requires the basket item to belong to the signed-in user.
                    && bp.Basket.Status); // Requires the basket item to be in the user's active basket.

            if (basketProduct == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Basket lookup
            _context.BasketProducts.Remove(basketProduct); // Queues the entity for removal from the database.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            return RedirectToAction("Index", "Baskets"); // Redirects the browser to the next MVC action.
        }

        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Delete Actions -----
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            // Basket lookup
            var basketProducts = await _context.BasketProducts.FindAsync(id); // Looks up the record by its primary key.
            if (basketProducts != null) // Runs the next step only when the record exists.
            {
                _context.BasketProducts.Remove(basketProducts); // Queues the entity for removal from the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Existence Helpers -----
        private bool BasketProductsExists(int id) // Checks whether a basket-product record still exists
        {
            // Basket lookup
            return _context.BasketProducts.Any(e => e.BasketProductsId == id); // Returns the computed result to the caller.
        }
    }
}
