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
using Microsoft.Extensions.Configuration.UserSecrets; // Provides user secret configuration support referenced by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{

    // ----- Controller Declaration -----
    public class OrdersController : Controller // Defines the MVC controller for checkout, order history, delivery confirmation, and reordering.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.

        public OrdersController(ApplicationDbContext context) // Receives dependencies from dependency injection and stores them on the controller.
        {
            _context = context; // Stores the injected dependency on the controller field.
        }

        // ----- Delivery Helpers -----
        // Delivery logic
        private async Task SyncDeliveryStatuses(List<Orders> orders) // Updates delivery tracking text based on order age and method
        {
            foreach (var order in orders.Where(o => o.Delivery)) // Iterates through each matching item to process it.
            {
                // Order logic
                double days = (DateTime.Now - order.OrderDate.ToDateTime(TimeOnly.MinValue)).TotalDays; // Sets days to the value needed by the workflow.
                int preparingDays = order.DeliveryMethod switch // Sets preparingDays to the value needed by the workflow.
                {
                    "Next Day" => 1, // Maps next-day delivery to one preparation day.
                    "Standard" => 3, // Maps standard delivery to three preparation days.
                    "Economy" => 7, // Maps economy delivery to seven preparation days.
                };

                string newStatus; // Declares the delivery tracking status that will be calculated next.
                // Delivery logic
                if (order.DeliveryConfirmed || days >= preparingDays + 3) // Checks the condition that controls the next action.
                    newStatus = "Delivered"; // Sets newStatus to the value needed by the workflow.
                else if (days >= preparingDays) // Checks the next fallback condition in the workflow.
                    // Order logic
                    newStatus = "Awaiting Confirmation"; // Sets newStatus to the value needed by the workflow.
                else // Handles the fallback branch when earlier conditions did not match.
                    newStatus = "Preparing Delivery"; // Sets newStatus to the value needed by the workflow.

                // Order status workflow
                if (order.TrackingStatus != newStatus) // Checks the condition that controls the next action.
                    order.TrackingStatus = newStatus; // Sets order.TrackingStatus to the value needed by the workflow.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Standard,Developer,Admin")] // Restricts access to users in the Standard,Developer,Admin role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Delivery Actions -----
        // Delivery logic
        public async Task<IActionResult> ConfirmDelivery(int id) // Marks one of the current user's orders as delivery-confirmed
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            // Order logic
            var order = await _context.Orders // Sets order to the value needed by the workflow.
                .FirstOrDefaultAsync(x => x.OrdersId == id && x.UserId == userId); // Fetches the first matching record or null if none exists.

            if (order == null) return NotFound(); // Checks whether the requested data was found.

            // Delivery logic
            order.DeliveryConfirmed = true; // Sets order.DeliveryConfirmed to the value needed by the workflow.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            return RedirectToAction("Index"); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Standard,Developer,Admin")] // Restricts access to users in the Standard,Developer,Admin role set.

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index(List<Orders> orders) // Loads the main listing or dashboard view for this controller
        {

            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Role management
            if (User.IsInRole("Admin")) // Branches based on the signed-in user's role.
            {
                // Order logic
                var allOrders = await _context.Orders.Include(x => x.OrderProducts).ThenInclude(x => x.Products).ToListAsync(); // Includes related records needed by the view or workflow.
                return View(allOrders); // Renders the matching view with the supplied model data.
            }
            else if (User.IsInRole("Producer")) // Checks the next fallback condition in the workflow.
            {
                // Producer order splitting
                var producerOrders = await _context.ProducerOrders // Sets producerOrders to the value needed by the workflow.
                    .Where(x => x.ProducerId == userId) // Filters the query to only the relevant records.
                    .Include(x => x.Orders) // Includes related records needed by the view or workflow.
                    .Include(x => x.OrderProducts) // Includes related records needed by the view or workflow.
                        .ThenInclude(x => x.Products) // Includes nested related records for the query result.
                    .ToListAsync(); // Executes the query asynchronously and materializes the list.

                return View(producerOrders.Select(op => op.Orders).Distinct().ToList()); // Renders the matching view with the supplied model data.
            }
            else // Handles the fallback branch when earlier conditions did not match.
            {
                // Order logic
                var userOrders = await _context.Orders.Where(x => x.UserId == userId).Include(x => x.OrderProducts).ThenInclude(x => x.Products).ToListAsync(); // Includes related records needed by the view or workflow.
                return View(userOrders); // Renders the matching view with the supplied model data.
            }
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // ----- Reorder Actions -----
        // Order logic
        public async Task<IActionResult> Reorder(int id) // Replaces the user's active basket with available items from a previous order
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Order logic
            var order = await _context.Orders // Sets order to the value needed by the workflow.
                .Include(o => o.OrderProducts) // Includes related records needed by the view or workflow.
                .ThenInclude(op => op.Products) // Includes nested related records for the query result.
                .FirstOrDefaultAsync(o => o.OrdersId == id && o.UserId == userId); // Fetches the first matching record or null if none exists.

            if (order == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var orderProducts = order.OrderProducts? // Sets orderProducts to the value needed by the workflow.
                .Where(op => op.Products != null && op.ProductQuantity > 0) // Filters the query to only the relevant records.
                .ToList() ?? new List<OrderProducts>(); // Materializes the sequence into a list.

            if (!orderProducts.Any()) // Checks whether matching data is present before continuing.
            {
                // Order logic
                TempData["ReorderWarning"] = "That order does not have any items to add back to your basket."; // Stores a one-request notification message for the redirected page.
                return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
            }

            var basket = await _context.Basket // Sets basket to the value needed by the workflow.
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Fetches the first matching record or null if none exists.

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
            var currentBasketProducts = await _context.BasketProducts // Sets currentBasketProducts to the value needed by the workflow.
                .Where(bp => bp.BasketId == basket.BasketId) // Filters the query to only the relevant records.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            if (currentBasketProducts.Any()) // Checks whether matching data is present before continuing.
            {
                _context.BasketProducts.RemoveRange(currentBasketProducts); // Queues the selected entities for removal from the database.
            }

            var addedCount = 0; // Sets addedCount to the value needed by the workflow.
            var skippedItems = new List<string>(); // Creates the skippedItems object used by the following workflow.

            // Product lookup
            foreach (var orderProductGroup in orderProducts.GroupBy(op => op.ProductsId)) // Iterates through each matching item to process it.
            {
                var orderProduct = orderProductGroup.First(); // Sets orderProduct to the value needed by the workflow.
                var product = orderProduct.Products; // Sets product to the value needed by the workflow.
                var orderedQuantity = orderProductGroup.Sum(op => op.ProductQuantity); // Calculates the total from the matching records.

                // Availability check
                if (product == null || !product.Available || product.QuantityInStock <= 0) // Checks whether the requested data was found.
                {
                    // Create record
                    skippedItems.Add(product?.ItemName ?? "An item"); // Records an item that could not be fully restored to the basket.
                    continue; // Skips the rest of this loop iteration and moves to the next item.
                }

                // Stock checks
                var quantityToAdd = Math.Min(orderedQuantity, product.QuantityInStock); // Sets quantityToAdd to the value needed by the workflow.

                if (quantityToAdd <= 0) // Checks the condition that controls the next action.
                {
                    skippedItems.Add(product.ItemName); // Records an item that could not be fully restored to the basket.
                    continue; // Skips the rest of this loop iteration and moves to the next item.
                }

                // Basket lookup
                _context.BasketProducts.Add(new BasketProducts // Queues the new entity to be inserted into the database.
                {
                    BasketId = basket.BasketId, // Sets BasketId to the value needed by the workflow.
                    // Product lookup
                    ProductsId = orderProduct.ProductsId, // Sets ProductsId to the value needed by the workflow.
                    ProductQuantity = quantityToAdd // Sets ProductQuantity to the value needed by the workflow.
                });

                addedCount += quantityToAdd; // Sets + to the value needed by the workflow.

                if (quantityToAdd < orderedQuantity) // Checks the condition that controls the next action.
                {
                    // Availability check
                    skippedItems.Add($"{product.ItemName} (only {quantityToAdd} available)"); // Records an item that could not be fully restored to the basket.
                }
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            if (addedCount > 0) // Checks the condition that controls the next action.
            {
                // Order logic
                TempData["ReorderSuccess"] = $"Your basket was replaced with {addedCount} item{(addedCount == 1 ? "" : "s")} from order #PF-{order.OrdersId:D6}."; // Stores a one-request notification message for the redirected page.

                if (skippedItems.Any()) // Checks whether matching data is present before continuing.
                {
                    TempData["ReorderWarning"] = "Some items could not be fully re-added: " + string.Join(", ", skippedItems.Distinct()) + "."; // Stores a one-request notification message for the redirected page.
                }

                return RedirectToAction("Index", "Baskets"); // Redirects the browser to the next MVC action.
            }

            // Availability check
            TempData["ReorderWarning"] = "Your basket was cleared, but none of the items from that order are currently available to re-order."; // Stores a one-request notification message for the redirected page.
            return RedirectToAction("Index", "Baskets"); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            bool canViewAnyOrder = User.IsInRole("Admin") || User.IsInRole("Developer"); // Lets privileged users inspect any order record.

            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Order logic
            var orderQuery = _context.Orders // Sets orderQuery to the value needed by the workflow.
                .Include(o => o.OrderProducts) // Includes related records needed by the view or workflow.
                .ThenInclude(op => op.Products) // Includes nested related records for the query result.
                .AsQueryable(); // Keeps the query composable for role-based filtering.

            if (!canViewAnyOrder) // Limits standard users to their own orders only.
            {
                orderQuery = orderQuery.Where(o => o.UserId == userId); // Filters the query to only the relevant records.
            }

            var order = await orderQuery.FirstOrDefaultAsync(o => o.OrdersId == id); // Fetches the first matching record or null if none exists.

            if (order == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(order); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.

        // ----- Create Actions -----
        // Basket lookup
        public async Task<IActionResult> Create(int basketId) // Shows or processes the create form for a record
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Basket lookup
            var basketProducts = await _context.BasketProducts // Sets basketProducts to the value needed by the workflow.
                            .Where(x => x.BasketId == basketId) // Filters the query to only the relevant records.
                            .Include(x => x.Products) // Includes related records needed by the view or workflow.
                            .ThenInclude(x => x.Producers) // Includes nested related records for the query result.
                            .ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Loyalty system
            var orderCount = await _context.Orders.CountAsync(x => x.UserId == userId); // Counts matching records for display or validation.
            var totals = CalculateCheckoutTotals(basketProducts, orderCount, false, null); // Calculates all checkout totals from server-side basket data.

            // Basket lookup
            ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
            ViewBag.Subtotal = totals.Subtotal; // Supplies Subtotal to the view for display or form state.
            ViewBag.Discount = totals.Discount; // Supplies Discount to the view for display or form state.
            ViewBag.Total = totals.Total; // Supplies Total to the view for display or form state.
            // Loyalty system
            ViewBag.HasFreeDelivery = orderCount % 3 == 2; // Supplies HasFreeDelivery to the view for display or form state.
            // Health bundle
            ViewBag.HasHealthBundle = totals.HasHealthBundle; // Supplies HasHealthBundle to the view for display or form state.
            ViewBag.BasketProducts = basketProducts; // Supplies BasketProducts to the view for display or form state.
            // Delivery logic
            ViewBag.DeliveryCosts = new Dictionary<string, decimal> // Supplies DeliveryCosts to the view for display or form state.
    {
        { "Next Day",  5.99m }, // Sets the next-day delivery charge.
        { "Standard",  2.99m }, // Sets the standard delivery charge.
        { "Economy",   0.99m } // Sets the economy delivery charge.
    };

            return View(); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.
        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Basket lookup
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryMethod,DateOfCollection,BillingLine1,BillingLine2,BillingCity,BillingPostcode,DeliveryLine1,DeliveryLine2,DeliveryCity,DeliveryPostcode,TermsAccepted")] Orders orders, int basketId) // Shows or processes the create form for a record
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            if (userId == null) // Checks that a signed-in user id is available.
            {
                ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
                return View(orders); // Renders the matching view with the supplied model data.
            }

            // Order status workflow
            if (!orders.TermsAccepted) // Checks the condition that controls the next action.
            {
                ModelState.AddModelError("TermsAccepted", "You must accept the terms and conditions to place an order."); // Adds a validation message that the view can show to the user.
            }

            // Form validation
            ModelState.Remove("InvoiceNumber"); // Removes a field from validation because the controller supplies it.

            // Set user ID
            orders.UserId = userId; // Sets orders.UserId to the value needed by the workflow.
            ModelState.Remove("UserId"); // Removes a field from validation because the controller supplies it.

            // Order logic
            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today); // Sets orders.OrderDate to the value needed by the workflow.
            ModelState.Remove("OrderDate"); // Removes a field from validation because the controller supplies it.

            // Order status workflow
            orders.TrackingStatus = "Pending"; // Sets orders.TrackingStatus to the value needed by the workflow.
            ModelState.Remove("TrackingStatus"); // Removes a field from validation because the controller supplies it.

            // Basket lookup
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.BasketId == basketId && x.Status); // Fetches the first matching record or null if none exists.

            if (basket == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var basketProducts = await _context.BasketProducts.Where(x => x.BasketId == basketId).Include(x => x.Products).ThenInclude(x => x.Producers).ToListAsync(); // Includes related records needed by the view or workflow.

            if (!basketProducts.Any()) // Checks whether matching data is present before continuing.
            {
                // Form validation
                ModelState.AddModelError("", "Your basket is currently empty."); // Adds a validation message that the view can show to the user.
                // Basket lookup
                ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
                ViewBag.BasketProducts = basketProducts; // Supplies BasketProducts to the view for display or form state.
                return View(orders); // Renders the matching view with the supplied model data.
            }

            // Order logic
            var orderCouut = await _context.Orders.CountAsync(x => x.UserId == userId); // Counts matching records for display or validation.
            var totals = CalculateCheckoutTotals(basketProducts, orderCouut, orders.Delivery, orders.DeliveryMethod); // Calculates all checkout totals from server-side basket data.

            // Basket totals
            orders.OrdersTotal = totals.Total; // Calculates the orders.OrdersTotal value used for totals.

            // Basket lookup
            ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
            ViewBag.Subtotal = totals.Subtotal; // Supplies Subtotal to the view for display or form state.
            ViewBag.Discount = totals.Discount; // Supplies Discount to the view for display or form state.
            ViewBag.Total = totals.Total; // Supplies Total to the view for display or form state.
            // Delivery logic
            ViewBag.HasFreeDelivery = orderCouut % 3 == 2; // Supplies HasFreeDelivery to the view for display or form state.
            // Health bundle
            ViewBag.HasHealthBundle = totals.HasHealthBundle; // Supplies HasHealthBundle to the view for display or form state.
            ViewBag.DeliveryCosts = new Dictionary<string, decimal> // Supplies DeliveryCosts to the view for display or form state.
            {
                { "Next Day",  5.99m }, // Sets the next-day delivery charge.
                { "Standard",  2.99m }, // Sets the standard delivery charge.
                { "Economy",   0.99m } // Sets the economy delivery charge.
            };

            // Basket totals
            ModelState.Remove("OrdersTotal"); // Removes a field from validation because the controller supplies it.
            // Billing address validation
            ModelState.Remove("BillingLine2"); // Removes a field from validation because the controller supplies it.
            // Delivery logic
            ModelState.Remove("DeliveryLine2"); // Removes a field from validation because the controller supplies it.

            // Form validation
            var addressRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9 #\-,\.]{1,40}$"); // Creates the addressRegex object used by the following workflow.
            var postcodeRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z]{1,2}[0-9][0-9A-Z]?\s?[0-9][A-Z]{2}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase); // Creates the postcodeRegex object used by the following workflow.

            if (string.IsNullOrWhiteSpace(orders.BillingLine1)) // Validates that required text was supplied.
            {
                // Billing address validation
                ModelState.AddModelError("BillingLine1", "Please enter the first line of your billing address."); // Adds a validation message that the view can show to the user.
            }
            else if (orders.BillingLine1.Length > 40) // Checks the next fallback condition in the workflow.
            {
                ModelState.AddModelError("BillingLine1", "Address line 1 must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
            }
            else if (!addressRegex.IsMatch(orders.BillingLine1)) // Checks the next fallback condition in the workflow.
            {
                // Billing address validation
                ModelState.AddModelError("BillingLine1", "Address line 1 can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
            }

            if (!string.IsNullOrWhiteSpace(orders.BillingLine2)) // Validates that required text was supplied.
            {
                if (orders.BillingLine2.Length > 40) // Checks the condition that controls the next action.
                {
                    ModelState.AddModelError("BillingLine2", "Address line 2 must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
                }
                // Billing address validation
                else if (!addressRegex.IsMatch(orders.BillingLine2)) // Checks the next fallback condition in the workflow.
                {
                    ModelState.AddModelError("BillingLine2", "Address line 2 can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
                }
            }

            if (string.IsNullOrWhiteSpace(orders.BillingCity)) // Validates that required text was supplied.
            {
                ModelState.AddModelError("BillingCity", "Please enter your billing city."); // Adds a validation message that the view can show to the user.
            }
            // Billing address validation
            else if (orders.BillingCity.Length > 40) // Checks the next fallback condition in the workflow.
            {
                ModelState.AddModelError("BillingCity", "City must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
            }
            else if (!addressRegex.IsMatch(orders.BillingCity)) // Checks the next fallback condition in the workflow.
            {
                ModelState.AddModelError("BillingCity", "City can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
            }

            // Billing address validation
            if (string.IsNullOrWhiteSpace(orders.BillingPostcode)) // Validates that required text was supplied.
            {
                ModelState.AddModelError("BillingPostcode", "Please enter your billing postcode."); // Adds a validation message that the view can show to the user.
            }
            else if (!postcodeRegex.IsMatch(orders.BillingPostcode.Trim())) // Checks the next fallback condition in the workflow.
            {
                ModelState.AddModelError("BillingPostcode", "Please enter a valid UK postcode (e.g. B1 1BB or SW1A 2AA)."); // Adds a validation message that the view can show to the user.
            }

            // Delivery logic
            if (orders.Delivery) // Checks the condition that controls the next action.
            {
                if (string.IsNullOrWhiteSpace(orders.DeliveryLine1)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryLine1", "Please enter the first line of your delivery address."); // Adds a validation message that the view can show to the user.
                }
                else if (orders.DeliveryLine1.Length > 40) // Checks the next fallback condition in the workflow.
                {
                    // Delivery logic
                    ModelState.AddModelError("DeliveryLine1", "Address line 1 must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
                }
                else if (!addressRegex.IsMatch(orders.DeliveryLine1)) // Checks the next fallback condition in the workflow.
                {
                    ModelState.AddModelError("DeliveryLine1", "Address line 1 can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
                }

                if (!string.IsNullOrWhiteSpace(orders.DeliveryLine2)) // Validates that required text was supplied.
                {
                    // Delivery logic
                    if (orders.DeliveryLine2.Length > 40) // Checks the condition that controls the next action.
                    {
                        ModelState.AddModelError("DeliveryLine2", "Address line 2 must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
                    }
                    else if (!addressRegex.IsMatch(orders.DeliveryLine2)) // Checks the next fallback condition in the workflow.
                    {
                        ModelState.AddModelError("DeliveryLine2", "Address line 2 can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
                    }
                }

                // Delivery logic
                if (string.IsNullOrWhiteSpace(orders.DeliveryCity)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryCity", "Please enter your delivery city."); // Adds a validation message that the view can show to the user.
                }
                else if (orders.DeliveryCity.Length > 40) // Checks the next fallback condition in the workflow.
                {
                    ModelState.AddModelError("DeliveryCity", "City must not exceed 40 characters."); // Adds a validation message that the view can show to the user.
                }
                // Delivery logic
                else if (!addressRegex.IsMatch(orders.DeliveryCity)) // Checks the next fallback condition in the workflow.
                {
                    ModelState.AddModelError("DeliveryCity", "City can only contain letters, numbers, spaces, and the following: # - , ."); // Adds a validation message that the view can show to the user.
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryPostcode)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter your delivery postcode."); // Adds a validation message that the view can show to the user.
                }
                // Delivery logic
                else if (!postcodeRegex.IsMatch(orders.DeliveryPostcode.Trim())) // Checks the next fallback condition in the workflow.
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter a valid UK postcode (e.g. B1 1BB or SW1A 2AA)."); // Adds a validation message that the view can show to the user.
                }
            }

            if (!orders.Collection && !orders.Delivery) // Checks the condition that controls the next action.
            {
                ModelState.AddModelError("Delivery", "Please select either Delivery or Collection."); // Adds a validation message that the view can show to the user.
            }

            // Collection logic
            if (orders.Collection) // Checks the condition that controls the next action.
            {
                // Delivery logic
                ModelState.Remove("DeliveryMethod"); // Removes a field from validation because the controller supplies it.

                if (orders.DateOfCollection == null) // Checks whether the requested data was found.
                {
                    ModelState.AddModelError("DateOfCollection", "Please provide a date for collection."); // Adds a validation message that the view can show to the user.
                }
                else // Handles the fallback branch when earlier conditions did not match.
                {
                    var today = DateOnly.FromDateTime(DateTime.Today); // Sets today to the value needed by the workflow.
                    // Collection logic
                    var earliestCollectionDate = today.AddDays(2); // Sets earliestCollectionDate to the value needed by the workflow.
                    var latestCollectionDate = today.AddDays(14); // Sets latestCollectionDate to the value needed by the workflow.

                    if (orders.DateOfCollection.Value < today) // Checks the condition that controls the next action.
                    {
                        ModelState.AddModelError("DateOfCollection", "Collection date must be in the present or future."); // Adds a validation message that the view can show to the user.
                    }
                    else if (orders.DateOfCollection.Value < earliestCollectionDate) // Checks the next fallback condition in the workflow.
                    {
                        // Collection logic
                        ModelState.AddModelError("DateOfCollection", "Collection must be at least 2 days from today."); // Adds a validation message that the view can show to the user.
                    }
                    else if (orders.DateOfCollection.Value > latestCollectionDate) // Checks the next fallback condition in the workflow.
                    {
                        ModelState.AddModelError("DateOfCollection", "Collection date must be within the next 14 days from today."); // Adds a validation message that the view can show to the user.
                    }
                }
            }

            // Delivery logic
            if (orders.Delivery) // Checks the condition that controls the next action.
            {
                // Collection logic
                ModelState.Remove("CollectionDate"); // Removes a field from validation because the controller supplies it.

                if (string.IsNullOrWhiteSpace(orders.DeliveryMethod)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryMethod", "Please select a delivery method."); // Adds a validation message that the view can show to the user.
                }

                // Delivery logic
                if (string.IsNullOrWhiteSpace(orders.DeliveryLine1)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryLine1", "Please enter the first line of your delivery address."); // Adds a validation message that the view can show to the user.
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryCity)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryCity", "Please enter your delivery city."); // Adds a validation message that the view can show to the user.
                }

                // Delivery logic
                if (string.IsNullOrWhiteSpace(orders.DeliveryPostcode)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter your delivery postcode."); // Adds a validation message that the view can show to the user.
                }
            }

            // Form validation
            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Basket lookup
                ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
                ViewBag.BasketProducts = basketProducts; // Supplies BasketProducts to the view for display or form state.
                return View(orders); // Renders the matching view with the supplied model data.
            }

            foreach (var basketProduct in basketProducts) // Iterates through each matching item to process it.
            {
                // Stock checks
                if (basketProduct.Products.QuantityInStock < basketProduct.ProductQuantity) // Checks the condition that controls the next action.
                {
                    ModelState.AddModelError("", $"The stock is too low for {basketProduct.Products.ItemName}"); // Adds a validation message that the view can show to the user.
                    // Basket lookup
                    ViewBag.BasketId = basketId; // Supplies BasketId to the view for display or form state.
                    ViewBag.BasketProducts = basketProducts; // Supplies BasketProducts to the view for display or form state.
                    return View(orders); // Renders the matching view with the supplied model data.
                }
            }

            // Order logic
            _context.Orders.Add(orders); // Queues the new entity to be inserted into the database.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            // Producer order splitting
            var groupedByProducer = basketProducts.GroupBy(x => x.Products.Producers.UserId); ; // Groups records so totals can be calculated per key.

            foreach (var producerGroup in groupedByProducer) // Iterates through each matching item to process it.
            {
                decimal producerSubtotal = 0m; // Calculates the producerSubtotal value used for totals.
                foreach (var item in producerGroup) // Iterates through each matching item to process it.
                {
                    producerSubtotal += item.Products.ItemPrice * item.ProductQuantity; // Sets + to the value needed by the workflow.
                }

                // Producer order splitting
                var producerOrder = new ProducerOrders // Creates the producerOrder object used by the following workflow.
                {
                    // Order logic
                    OrdersId = orders.OrdersId, // Sets OrdersId to the value needed by the workflow.
                    ProducerId = producerGroup.Key, // Sets ProducerId to the value needed by the workflow.
                    ProducerSubtotal = producerSubtotal, // Calculates the ProducerSubtotal value used for totals.
                    // Order status workflow
                    TrackingStatus = "Pending" // Sets TrackingStatus to the value needed by the workflow.
                };

                // Producer order splitting
                _context.ProducerOrders.Add(producerOrder); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

                foreach (var basketProduct in producerGroup) // Iterates through each matching item to process it.
                {
                    // Create order
                    var orderProduct = new OrderProducts // Creates the orderProduct object used by the following workflow.
                    {
                        // Order logic
                        OrdersId = orders.OrdersId, // Sets OrdersId to the value needed by the workflow.
                        ProducerOrdersId = producerOrder.ProducerOrdersId, // Sets ProducerOrdersId to the value needed by the workflow.
                        // Product lookup
                        ProductsId = basketProduct.ProductsId, // Sets ProductsId to the value needed by the workflow.
                        ProductQuantity = basketProduct.ProductQuantity, // Sets ProductQuantity to the value needed by the workflow.
                        // Invoice VAT system
                        InvoiceNumber = (basketProduct.Products.Producers != null && basketProduct.Products.Producers.IsVATRegistered) // Starts VAT invoice number generation for VAT-registered producers.
                            ? $"INV-{orders.OrderDate:yyyyMMdd}-{orders.OrdersId:D6}-{basketProduct.Products.Producers.ProducersId}" // Uses the value when the preceding condition is true.
                            : null // Stores null when the preceding condition is false.
                    };

                    // Create order
                    _context.OrderProducts.Add(orderProduct); // Queues the new entity to be inserted into the database.
                    // Stock checks
                    basketProduct.Products.QuantityInStock -= basketProduct.ProductQuantity; // Sets - to the value needed by the workflow.
                }
            }

            // Remove basket
            basket.Status = false; // Sets basket.Status to the value needed by the workflow.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            // Order logic
            return RedirectToAction("Confirmation", "Orders", new { id = orders.OrdersId }); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Developer")] // Restricts access to users in the Developer role set.

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Order logic
            var orders = await _context.Orders.FindAsync(id); // Looks up the record by its primary key.
            if (orders == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }
            return View(orders); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Developer")] // Restricts access to users in the Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Basket totals
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,OrderDate,DeliveryMethod,Delivery,Collection,OrdersTotal,TrackingStatus,DateOfCollection,BillingLine1,BillingLine2,BillingCity,BillingPostcode,DeliveryLine1,DeliveryLine2,DeliveryCity,DeliveryPostcode")] Orders orders) // Shows or processes edits for an existing record
        {
            // Order logic
            if (id != orders.OrdersId) // Checks the condition that controls the next action.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Form validation
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                try // Starts a protected database update block.
                {
                    // Edit record
                    _context.Update(orders); // Marks the entity as modified so EF Core saves its changes.
                    await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                }
                catch (DbUpdateConcurrencyException) // Handles database concurrency errors from the update attempt.
                {
                    // Order logic
                    if (!OrdersExists(orders.OrdersId)) // Checks the condition that controls the next action.
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

            return View(orders); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Developer")] // Restricts access to users in the Developer role set.

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Order logic
            var orders = await _context.Orders // Sets orders to the value needed by the workflow.
                .FirstOrDefaultAsync(m => m.OrdersId == id); // Fetches the first matching record or null if none exists.
            if (orders == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(orders); // Renders the matching view with the supplied model data.
        }

        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Developer")] // Restricts access to users in the Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            // Order logic
            var orders = await _context.Orders.FindAsync(id); // Looks up the record by its primary key.
            if (orders != null) // Runs the next step only when the record exists.
            {
                _context.Orders.Remove(orders); // Queues the entity for removal from the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Existence Helpers -----
        private static (decimal Subtotal, decimal Discount, decimal DeliveryCost, decimal Total, bool HasHealthBundle) CalculateCheckoutTotals(
            IEnumerable<BasketProducts> basketProducts,
            int orderCount,
            bool isDelivery,
            string? deliveryMethod) // Calculates checkout pricing from basket contents instead of trusting posted form values
        {
            var items = basketProducts.ToList(); // Materializes the basket so totals and bundle checks use the same item set.
            var subtotal = items.Sum(item => item.Products.ItemPrice * item.ProductQuantity); // Calculates the basket subtotal from current product prices.
            var productNames = items.Select(item => item.Products.ItemName.ToLower()).ToList(); // Projects product names for bundle discount checks.
            var hasHealthBundle = productNames.Contains("broccoli")
                                  && productNames.Contains("carrot")
                                  && productNames.Contains("apple"); // Checks whether the basket qualifies for the health bundle discount.

            var discount = 0m; // Stores the discount applied to the basket subtotal.
            if (orderCount % 5 == 4) // Applies the loyalty reward on every fifth order.
            {
                discount = subtotal * 0.15m; // Calculates the loyalty discount.
            }
            else if (hasHealthBundle) // Applies health bundle discount when loyalty reward is not active.
            {
                discount = subtotal * 0.10m; // Calculates the health bundle discount.
            }

            var deliveryCost = 0m; // Stores delivery cost, if delivery is selected.
            if (isDelivery && orderCount % 3 != 2) // Every third order gets free delivery.
            {
                deliveryCost = deliveryMethod switch
                {
                    "Next Day" => 5.99m,
                    "Standard" => 2.99m,
                    "Economy" => 0.99m,
                    _ => 0m
                };
            }

            var total = subtotal - discount + deliveryCost; // Calculates the final order total.
            return (subtotal, discount, deliveryCost, total, hasHealthBundle); // Returns every value the checkout view and order save need.
        }

        // Order logic
        private bool OrdersExists(int id) // Checks whether an order record still exists
        {
            return _context.Orders.Any(e => e.OrdersId == id); // Returns the computed result to the caller.
        }

        // Role management
        [Authorize(Roles = "Standard,Developer")] // Restricts access to users in the Standard,Developer role set.

        // ----- Checkout Confirmation -----
        public async Task<IActionResult> Confirmation(int id) // Loads the completed order for the checkout confirmation page
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

            if (userId == null) // Checks that a signed-in user id is available.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Order logic
            var order = await _context.Orders // Sets order to the value needed by the workflow.
                .Include(o => o.OrderProducts) // Includes related records needed by the view or workflow.
                .ThenInclude(op => op.Products) // Includes nested related records for the query result.
                .ThenInclude(p => p.Producers) // Includes nested related records for the query result.
                .FirstOrDefaultAsync(o => o.OrdersId == id && o.UserId == userId); // Fetches the first matching record or null if none exists.

            if (order == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(order); // Renders the matching view with the supplied model data.
        }

    }
}
