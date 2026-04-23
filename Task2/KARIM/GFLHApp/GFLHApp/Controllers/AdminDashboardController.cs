// ----- Imports -----
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.
using Microsoft.AspNetCore.Identity; // Provides Identity user and role management services.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{
    // Role management
    [Authorize(Roles = "Admin,Developer")] // Restricts access to users in the Admin,Developer role set.

    // ----- Controller Declaration -----
    public class AdminDashboardController : Controller // Defines the MVC controller for admin dashboard reports, moderation tools, and role management.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.
        private readonly UserManager<IdentityUser> _userManager; // Holds the Identity user manager used to find and update users.
        private readonly RoleManager<IdentityRole> _roleManager; // Holds the Identity role manager used to inspect and create roles.

        public AdminDashboardController( // Receives database and Identity services from dependency injection.
            ApplicationDbContext context, // Accepts the EF Core database context from dependency injection.
            // Role management
            UserManager<IdentityUser> userManager, // Accepts the Identity user manager from dependency injection.
            RoleManager<IdentityRole> roleManager) // Accepts the Identity role manager from dependency injection.
        {
            _context = context; // Stores the injected dependency on the controller field.
            _userManager = userManager; // Stores the injected dependency on the controller field.
            _roleManager = roleManager; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            await PopulateAdminChromeAsync(); // Loads shared dashboard counts used in the sidebar and top-level admin chrome.
            var now = DateTime.UtcNow; // Sets now to the value needed by the workflow.

            // Order logic
            var orders = await _context.Orders.ToListAsync(); // Executes the query asynchronously and materializes the list.
            // Product lookup
            var products = await _context.Products.ToListAsync(); // Executes the query asynchronously and materializes the list.
            // Producer lookup
            var producers = await _context.Producers.ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Basket totals
            ViewBag.TotalUsers = _userManager.Users.Count(); // Counts matching records for display or validation.
            ViewBag.TotalOrders = orders.Count; // Supplies TotalOrders to the view for display or form state.
            ViewBag.TotalProducts = products.Count; // Supplies TotalProducts to the view for display or form state.
            ViewBag.TotalProducers = producers.Count; // Supplies TotalProducers to the view for display or form state.
            ViewBag.TotalRevenue = orders.Sum(o => o.OrdersTotal); // Calculates the total from the matching records.
            // Order logic
            ViewBag.PendingOrders = orders.Count(o => o.OrderStatus == "Pending"); // Counts matching records for display or validation.
            // Availability check
            ViewBag.LowStock = products.Count(p => p.QuantityInStock <= 5 && p.Available); // Counts matching records for display or validation.
            ViewBag.MonthRevenue = orders // Supplies MonthRevenue to the view for display or form state.
                .Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year) // Requires the order to belong to the current month.
                // Basket totals
                .Sum(o => o.OrdersTotal); // Calculates the total from the matching records.

            ViewBag.RecentOrders = await _context.Orders // Supplies RecentOrders to the view for display or form state.
                .Include(o => o.OrderProducts).ThenInclude(op => op.Products) // Includes related records needed by the view or workflow.
                // Order logic
                .OrderByDescending(o => o.OrderDate) // Sorts the query results for display or processing.
                .Take(6) // Limits the result set to the required number of records.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            var topProductIds = await _context.OrderProducts // Sets topProductIds to the value needed by the workflow.
                // Product lookup
                .GroupBy(op => op.ProductsId) // Groups records so totals can be calculated per key.
                .Select(g => new { ProductsId = g.Key, TotalOrdered = g.Sum(op => op.ProductQuantity) }) // Projects the query results into the shape needed next.
                // Sort logic
                .OrderByDescending(x => x.TotalOrdered) // Sorts the query results for display or processing.
                .Take(5) // Limits the result set to the required number of records.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.
            var pIds = topProductIds.Select(x => x.ProductsId).ToList(); // Projects the query results into the shape needed next.
            var pNames = await _context.Products.Where(p => pIds.Contains(p.ProductsId)) // Loads names for the best-selling product ids.
                             // Product lookup
                             .ToDictionaryAsync(p => p.ProductsId, p => p.ItemName); // Executes the query and builds a lookup dictionary.
            // Admin dashboard statistics
            ViewBag.TopProducts = topProductIds // Supplies TopProducts to the view for display or form state.
                .Where(x => pNames.ContainsKey(x.ProductsId)) // Filters the query to only the relevant records.
                .Select(x => new { Name = pNames[x.ProductsId], x.TotalOrdered }) // Projects the query results into the shape needed next.
                .ToList(); // Materializes the sequence into a list.

            var sixMonthsAgo = DateOnly.FromDateTime(now.AddMonths(-5)); // Sets sixMonthsAgo to the value needed by the workflow.
            ViewBag.MonthlyRevenue = orders // Supplies MonthlyRevenue to the view for display or form state.
                // Order logic
                .Where(o => o.OrderDate >= sixMonthsAgo) // Filters the query to only the relevant records.
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month }) // Requires the order to belong to the current month.
                .Select(g => new { // Projects the query results into the shape needed next.
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yy"), // Creates the Label object used by the following workflow.
                    // Basket totals
                    Revenue = g.Sum(o => o.OrdersTotal), // Calculates the total from the matching records.
                    g.Key.Year, // Carries the revenue group year forward for chronological sorting.
                    g.Key.Month // Carries the revenue group month forward for chronological sorting.
                })
                // Sort logic
                .OrderBy(x => x.Year).ThenBy(x => x.Month) // Sorts the query results for display or processing.
                .ToList(); // Materializes the sequence into a list.

            // Admin dashboard statistics
            ViewBag.StatusBreakdown = orders // Supplies StatusBreakdown to the view for display or form state.
                // Order logic
                .GroupBy(o => o.OrderStatus ?? "Unknown") // Groups records so totals can be calculated per key.
                .Select(g => new { Status = g.Key, Count = g.Count() }) // Projects the query results into the shape needed next.
                .ToList(); // Materializes the sequence into a list.
            return View(); // Renders the matching view with the supplied model data.
        }

        // ----- Admin Order Actions -----
        public async Task<IActionResult> Orders(string search = "", string orderStatus = "", string trackingStatus = "", string sort = "newest") // Loads the admin order list with search, filtering, and sorting
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the orders page.
            var query = _context.Orders // Sets query to the value needed by the workflow.
                .Include(o => o.OrderProducts).ThenInclude(op => op.Products) // Includes related records needed by the view or workflow.
                .AsQueryable(); // Continues the chained query or expression from the previous line.

            // Search logic
            if (!string.IsNullOrWhiteSpace(search)) // Validates that required text was supplied.
            {
                var s = search.Trim().ToLower(); // Sets s to the value needed by the workflow.
                if (int.TryParse(s, out int orderId)) // Checks the condition that controls the next action.
                    // Order logic
                    query = query.Where(o => o.OrdersId == orderId); // Filters the query to only the relevant records.
                else // Handles the fallback branch when earlier conditions did not match.
                    query = query.Where(o => o.UserId.ToLower().Contains(s) || // Filters the query to only the relevant records.
                                             // Billing address validation
                                             o.BillingCity.ToLower().Contains(s)); // Allows the admin order search to match billing city.
            }

            if (!string.IsNullOrWhiteSpace(orderStatus)) // Validates that required text was supplied.
                query = query.Where(o => o.OrderStatus == orderStatus); // Filters the query to only the relevant records.

            // Order status workflow
            if (!string.IsNullOrWhiteSpace(trackingStatus)) // Validates that required text was supplied.
                query = query.Where(o => o.TrackingStatus == trackingStatus); // Filters the query to only the relevant records.

            // Sort logic
            query = sort switch // Sets query to the value needed by the workflow.
            {
                // Order logic
                "oldest" => query.OrderBy(o => o.OrderDate), // Sorts the query results for display or processing.
                // Basket totals
                "total_asc" => query.OrderBy(o => o.OrdersTotal), // Sorts the query results for display or processing.
                "total_desc" => query.OrderByDescending(o => o.OrdersTotal), // Sorts the query results for display or processing.
                _ => query.OrderByDescending(o => o.OrderDate) // Sorts the query results for display or processing.
            };

            var orders = await query.ToListAsync(); // Executes the query asynchronously and materializes the list.

            // Search logic
            ViewBag.Search = search; // Supplies Search to the view for display or form state.
            // Order logic
            ViewBag.OrderStatus = orderStatus; // Supplies OrderStatus to the view for display or form state.
            // Order status workflow
            ViewBag.TrackingStatus = trackingStatus; // Supplies TrackingStatus to the view for display or form state.
            // Sort logic
            ViewBag.Sort = sort; // Supplies Sort to the view for display or form state.

            return View(orders); // Renders the matching view with the supplied model data.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.
        public async Task<IActionResult> UpdateTrackingStatus(int id, string status) // Updates an order tracking status from the admin dashboard
        {
            // Order logic
            var order = await _context.Orders.FindAsync(id); // Looks up the record by its primary key.
            // Order status workflow
            if (order != null) { order.TrackingStatus = status; await _context.SaveChangesAsync(); } // Runs the next step only when the record exists.
            return RedirectToAction(nameof(Orders)); // Redirects the browser to the next MVC action.
        }

        // ----- Admin Product Actions -----
        // Availability check
        public async Task<IActionResult> Products(string search = "", string category = "", string availability = "", string sort = "name") // Loads the admin product list with search, filters, and sorting
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the products page.
            // Product lookup
            var query = _context.Products.Include(p => p.Producers).AsQueryable(); // Includes related records needed by the view or workflow.

            // Search logic
            if (!string.IsNullOrWhiteSpace(search)) // Validates that required text was supplied.
            {
                var s = search.Trim().ToLower(); // Sets s to the value needed by the workflow.
                query = query.Where(p => p.ItemName.ToLower().Contains(s) || // Filters the query to only the relevant records.
                                         p.Category.ToLower().Contains(s) || // Allows the admin product search to match category.
                                         p.Producers.ProducerName.ToLower().Contains(s)); // Allows the admin product search to match producer name.
            }

            // Filter logic
            if (!string.IsNullOrWhiteSpace(category)) // Validates that required text was supplied.
                query = query.Where(p => p.Category == category); // Filters the query to only the relevant records.

            // Availability check
            query = availability switch // Sets query to the value needed by the workflow.
            {
                "available" => query.Where(p => p.Available), // Filters the query to only the relevant records.
                "unavailable" => query.Where(p => !p.Available), // Filters the query to only the relevant records.
                "lowstock" => query.Where(p => p.QuantityInStock <= 5 && p.Available), // Filters the query to only the relevant records.
                _ => query // Leaves the product query unchanged when no availability filter is selected.
            };

            // Sort logic
            query = sort switch // Sets query to the value needed by the workflow.
            {
                "price_asc" => query.OrderBy(p => p.ItemPrice), // Sorts the query results for display or processing.
                "price_desc" => query.OrderByDescending(p => p.ItemPrice), // Sorts the query results for display or processing.
                // Stock checks
                "stock_asc" => query.OrderBy(p => p.QuantityInStock), // Sorts the query results for display or processing.
                "stock_desc" => query.OrderByDescending(p => p.QuantityInStock), // Sorts the query results for display or processing.
                _ => query.OrderBy(p => p.ItemName) // Sorts the query results for display or processing.
            };

            var products = await query.ToListAsync(); // Executes the query asynchronously and materializes the list.
            // Product lookup
            var categories = await _context.Products.Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync(); // Sorts the query results for display or processing.

            // Search logic
            ViewBag.Search = search; // Supplies Search to the view for display or form state.
            // Filter logic
            ViewBag.Category = category; // Supplies Category to the view for display or form state.
            // Availability check
            ViewBag.Availability = availability; // Supplies Availability to the view for display or form state.
            // Sort logic
            ViewBag.Sort = sort; // Supplies Sort to the view for display or form state.
            ViewBag.Categories = categories; // Supplies Categories to the view for display or form state.

            return View(products); // Renders the matching view with the supplied model data.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.
        // Availability check
        public async Task<IActionResult> ToggleProductAvailability(int id) // Toggles whether a product can be bought and returns JSON
        {
            // Product lookup
            var product = await _context.Products.FindAsync(id); // Looks up the record by its primary key.
            if (product == null) return Json(new { success = false }); // Checks whether the requested data was found.
            product.Available = !product.Available; // Sets product.Available to the value needed by the workflow.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return Json(new { success = true, available = product.Available }); // Returns a JSON response for client-side code.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.
        public async Task<IActionResult> DeleteProduct(int id) // Deletes a product from the admin dashboard
        {
            // Product lookup
            var product = await _context.Products.FindAsync(id); // Looks up the record by its primary key.
            if (product != null) { _context.Products.Remove(product); await _context.SaveChangesAsync(); } // Runs the next step only when the record exists.
            TempData["AdminSuccess"] = "Product deleted."; // Stores a one-request notification message for the redirected page.
            return RedirectToAction(nameof(Products)); // Redirects the browser to the next MVC action.
        }

        // ----- Admin Producer Actions -----
        // Search logic
        public async Task<IActionResult> Producers(string search = "") // Loads the admin producer list and optional search results
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the producers page.
            // Producer lookup
            var query = _context.Producers // Sets query to the value needed by the workflow.
                .Include(p => p.Products) // Includes related records needed by the view or workflow.
                // Producer order splitting
                .Include(p => p.ProducerOrders) // Includes related records needed by the view or workflow.
                .AsQueryable(); // Continues the chained query or expression from the previous line.

            // Search logic
            if (!string.IsNullOrWhiteSpace(search)) // Validates that required text was supplied.
            {
                var s = search.Trim().ToLower(); // Sets s to the value needed by the workflow.
                query = query.Where(p => p.ProducerName.ToLower().Contains(s) || // Allows the admin product search to match producer name.
                                         p.ProducerEmail.ToLower().Contains(s)); // Allows the admin producer search to match producer email.
            }

            // Sort logic
            var producers = await query.OrderBy(p => p.ProducerName).ToListAsync(); // Sorts the query results for display or processing.
            // Search logic
            ViewBag.Search = search; // Supplies Search to the view for display or form state.
            return View(producers); // Renders the matching view with the supplied model data.
        }

        public async Task<IActionResult> Inquiries(string search = "", string status = "", string sort = "newest") // Loads customer contact form submissions for admins
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the inquiries page.
            var query = _context.ContactInquiries.AsQueryable(); // Starts a query for saved contact form submissions.

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower(); // Normalises the search term for matching.
                query = query.Where(i =>
                    i.FullName.ToLower().Contains(s) ||
                    i.EmailAddress.ToLower().Contains(s) ||
                    i.Subject.ToLower().Contains(s) ||
                    i.Message.ToLower().Contains(s)); // Lets admins search by sender, email, subject, or message text.
            }

            query = status switch
            {
                "unread" => query.Where(i => !i.IsRead),
                "read" => query.Where(i => i.IsRead),
                _ => query
            };

            query = sort switch
            {
                "oldest" => query.OrderBy(i => i.SubmittedAtUtc),
                _ => query.OrderByDescending(i => i.SubmittedAtUtc)
            };

            var inquiries = await query.ToListAsync(); // Materialises the filtered inquiry results for the page.

            ViewData["AdminSection"] = "inquiries"; // Highlights the current sidebar item.
            ViewBag.Search = search; // Keeps the search term sticky in the form.
            ViewBag.Status = status; // Keeps the status filter sticky in the form.
            ViewBag.Sort = sort; // Keeps the sort selection sticky in the form.
            return View(inquiries); // Renders the inquiry list for admin review.
        }

        public async Task<IActionResult> InquiryDetails(int id) // Shows the full message for one contact inquiry
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the inquiry detail page.
            var inquiry = await _context.ContactInquiries.FirstOrDefaultAsync(i => i.ContactInquiryId == id); // Loads the requested inquiry record.
            if (inquiry == null) return NotFound(); // Stops when the inquiry id is not valid.

            if (!inquiry.IsRead)
            {
                inquiry.IsRead = true; // Marks the inquiry as read once an admin opens it.
                inquiry.ReadAtUtc = DateTime.UtcNow; // Records when the inquiry was first reviewed.
                await _context.SaveChangesAsync(); // Persists the read status change.
            }

            ViewData["AdminSection"] = "inquiries"; // Highlights the current sidebar item.
            return View(inquiry); // Renders the inquiry detail page.
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyToInquiry(int id, string replyMessage) // Saves or updates an admin reply on a customer inquiry.
        {
            var inquiry = await _context.ContactInquiries.FindAsync(id); // Looks up the inquiry that is being answered.
            if (inquiry == null) return NotFound(); // Stops when the inquiry no longer exists.

            var trimmedReply = (replyMessage ?? string.Empty).Trim(); // Normalises the reply text before saving it.
            if (string.IsNullOrWhiteSpace(trimmedReply))
            {
                TempData["AdminSuccess"] = "Reply message cannot be empty."; // Gives the admin clear feedback when the reply box was left blank.
                return RedirectToAction(nameof(InquiryDetails), new { id }); // Returns to the same inquiry so the admin can try again.
            }

            inquiry.AdminReply = trimmedReply; // Stores the latest admin reply on the inquiry.
            inquiry.RepliedAtUtc = DateTime.UtcNow; // Records when the reply was sent.
            inquiry.RepliedByEmail = _userManager.GetUserName(User) ?? User.Identity?.Name ?? "Admin"; // Keeps track of which admin account responded.
            inquiry.IsRead = true; // Treats the replied inquiry as reviewed.
            inquiry.ReadAtUtc ??= DateTime.UtcNow; // Preserves the first-read time if it already existed.

            await _context.SaveChangesAsync(); // Saves the reply so the customer can see it on the contact page.
            TempData["AdminSuccess"] = "Reply saved for this inquiry."; // Confirms the reply was stored successfully.
            return RedirectToAction(nameof(InquiryDetails), new { id }); // Returns to the inquiry detail page after saving.
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleInquiryRead(int id) // Lets admins mark a contact inquiry as read or unread
        {
            var inquiry = await _context.ContactInquiries.FindAsync(id); // Looks up the target inquiry record.
            if (inquiry == null) return NotFound(); // Stops when the inquiry no longer exists.

            inquiry.IsRead = !inquiry.IsRead; // Flips the inquiry state for quick triage.
            inquiry.ReadAtUtc = inquiry.IsRead ? DateTime.UtcNow : null; // Keeps the read timestamp aligned with the current state.

            await _context.SaveChangesAsync(); // Saves the admin status update.
            TempData["AdminSuccess"] = inquiry.IsRead
                ? "Inquiry marked as read."
                : "Inquiry marked as unread."; // Gives admins clear feedback after the action.

            return RedirectToAction(nameof(Inquiries)); // Returns to the inquiry list after the update.
        }

        // ----- Admin User Actions -----
        public async Task<IActionResult> Users(string search = "", string role = "") // Loads users, their roles, and their order counts for administration
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the users page.
            var allUsers = _userManager.Users.OrderBy(u => u.Email).ToList(); // Sorts the query results for display or processing.
            var rows = new List<AdminUserRow>(); // Creates the rows object used by the following workflow.

            foreach (var user in allUsers) // Iterates through each matching item to process it.
            {
                var roles = await _userManager.GetRolesAsync(user); // Sets roles to the value needed by the workflow.

                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role)) continue; // Validates that required text was supplied.

                // Search logic
                if (!string.IsNullOrWhiteSpace(search)) // Validates that required text was supplied.
                {
                    var s = search.Trim().ToLower(); // Sets s to the value needed by the workflow.
                    if (!(user.Email?.ToLower().Contains(s) == true || // Checks whether matching data is present before continuing.
                          user.UserName?.ToLower().Contains(s) == true)) continue; // Allows the admin user search to match username.
                }

                // Loyalty system
                var orderCount = await _context.Orders.CountAsync(o => o.UserId == user.Id); // Counts matching records for display or validation.
                rows.Add(new AdminUserRow(user, roles, orderCount)); // Adds the user's admin row with roles and order count.
            }

            // Search logic
            ViewBag.Search = search; // Supplies Search to the view for display or form state.
            ViewBag.Role = role; // Supplies Role to the view for display or form state.
            // Sort logic
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).OrderBy(n => n).ToList(); // Sorts the query results for display or processing.

            return View(rows); // Renders the matching view with the supplied model data.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.

        // ----- Admin Role Actions -----
        // Role management
        public async Task<IActionResult> AssignRole(string userId, string roleName) // Adds a selected role to a selected user
        {
            var user = await _userManager.FindByIdAsync(userId); // Sets user to the value needed by the workflow.
            if (user == null) return NotFound(); // Checks whether the requested data was found.
            if (!await _roleManager.RoleExistsAsync(roleName)) // Checks whether the role needs to be created first.
                await _roleManager.CreateAsync(new IdentityRole(roleName)); // Creates the missing Identity role before assigning or listing it.
            await _userManager.AddToRoleAsync(user, roleName); // Assigns the new producer account to the Producer role.
            TempData["AdminSuccess"] = $"Role '{roleName}' assigned to {user.Email}."; // Stores a one-request notification message for the redirected page.
            return RedirectToAction(nameof(Users)); // Redirects the browser to the next MVC action.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.
        // Role management
        public async Task<IActionResult> RemoveRole(string userId, string roleName) // Removes a selected role from a selected user
        {
            var user = await _userManager.FindByIdAsync(userId); // Sets user to the value needed by the workflow.
            if (user == null) return NotFound(); // Checks whether the requested data was found.
            if (roleName == "Developer" && !User.IsInRole("Developer")) // Prevents non-developer admins from stripping the developer role.
            {
                TempData["AdminSuccess"] = "Only Developers can remove the Developer role."; // Stores a one-request notification message for the redirected page.
                return RedirectToAction(nameof(Users)); // Redirects the browser to the next MVC action.
            }
            await _userManager.RemoveFromRoleAsync(user, roleName); // Removes the selected role from the selected user.
            TempData["AdminSuccess"] = $"Role '{roleName}' removed from {user.Email}."; // Stores a one-request notification message for the redirected page.
            return RedirectToAction(nameof(Users)); // Redirects the browser to the next MVC action.
        }

        // ----- Admin Settings Actions -----
        public async Task<IActionResult> Settings() // Loads dashboard configuration counts and role data
        {
            await PopulateAdminChromeAsync(); // Keeps shared admin navigation counters available on the settings page.
            ViewBag.UserCount = _userManager.Users.Count(); // Counts matching records for display or validation.
            // Loyalty system
            ViewBag.OrderCount = await _context.Orders.CountAsync(); // Counts matching records for display or validation.
            // Product lookup
            ViewBag.ProductCount = await _context.Products.CountAsync(); // Counts matching records for display or validation.
            // Producer lookup
            ViewBag.ProducerCount = await _context.Producers.CountAsync(); // Counts matching records for display or validation.
            // Basket totals
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.OrdersTotal) ?? 0m; // Calculates the total from the matching records.
            // Sort logic
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).OrderBy(n => n).ToList(); // Sorts the query results for display or processing.
            ViewBag.ActiveBaskets = await _context.Basket.CountAsync(b => b.Status); // Counts matching records for display or validation.
            return View(); // Renders the matching view with the supplied model data.
        }

        // Form validation
        [HttpPost, ValidateAntiForgeryToken] // Marks the following action as handling POST form submissions.

        // ----- Admin Role Actions -----
        // Role management
        public async Task<IActionResult> CreateRole(string roleName) // Creates a new Identity role when it does not already exist
        {
            if (!string.IsNullOrWhiteSpace(roleName) && !await _roleManager.RoleExistsAsync(roleName.Trim())) // Checks whether the role needs to be created first.
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim())); // Creates the missing Identity role before assigning or listing it.
                TempData["AdminSuccess"] = $"Role '{roleName.Trim()}' created."; // Stores a one-request notification message for the redirected page.
            }
            return RedirectToAction(nameof(Settings)); // Redirects the browser to the next MVC action.
        }

        private async Task PopulateAdminChromeAsync() // Loads reusable admin counts shared by dashboard navigation and chrome.
        {
            ViewBag.UnreadInquiryCount = await _context.ContactInquiries.CountAsync(i => !i.IsRead); // Gives admins a persistent unread inquiry indicator in the sidebar.
        }
    }

    // Loyalty system
    public record AdminUserRow(IdentityUser User, IList<string> Roles, int OrderCount); // Defines the compact row model used to send user, role, and order-count data to the view.
}
