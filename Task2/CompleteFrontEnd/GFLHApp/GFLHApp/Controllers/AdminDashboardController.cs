using GFLHApp.Data;
using GFLHApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GFLHApp.Controllers
{
    [Authorize(Roles = "Admin,Developer")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context; // Database context for accessing data
        private readonly UserManager<IdentityUser> _userManager; // User manager for handling user-related operations
        private readonly RoleManager<IdentityRole> _roleManager; // Role manager for handling role-related operations

        public AdminDashboardController( // Constructor to initialize the controller with necessary services
            ApplicationDbContext context, // Database context injected through dependency injection
            UserManager<IdentityUser> userManager, // User manager injected through dependency injection 
            RoleManager<IdentityRole> roleManager) // Role manager injected through dependency injection 
        {
            _context = context; // Assign the injected database context to the private field
            _userManager = userManager; // Assign the injected user manager to the private field
            _roleManager = roleManager; // Assign the injected role manager to the private field
        }

        // ── Overview ─────────────────────────────────────────────────────
        public async Task<IActionResult> Index() // Action method for the overview page of the admin dashboard, which gathers various statistics and data to display on the dashboard
        {
            var now = DateTime.UtcNow; // Get the current date and time in UTC to use for filtering data based on time periods

            var orders = await _context.Orders.ToListAsync(); // Retrieve all orders from the database asynchronously and store them in a list
            var products = await _context.Products.ToListAsync(); // Retrieve all products from the database asynchronously and store them in a list
            var producers = await _context.Producers.ToListAsync(); // Retrieve all producers from the database asynchronously and store them in a list

            ViewBag.TotalUsers = _userManager.Users.Count(); // Count the total number of users using the user manager and store it in the ViewBag to be displayed on the dashboard
            ViewBag.TotalOrders = orders.Count; // Count the total number of orders from the retrieved list and store it in the ViewBag to be displayed on the dashboard
            ViewBag.TotalProducts = products.Count; // Count the total number of products from the retrieved list and store it in the ViewBag to be displayed on the dashboard
            ViewBag.TotalProducers = producers.Count; // Count the total number of producers from the retrieved list and store it in the ViewBag to be displayed on the dashboard
            ViewBag.TotalRevenue = orders.Sum(o => o.OrdersTotal); // Calculate the total revenue by summing the OrdersTotal property of all orders and store it in the ViewBag to be displayed on the dashboard
            ViewBag.PendingOrders = orders.Count(o => o.OrderStatus == "Pending"); // Count the number of orders with a status of "Pending" and store it in the ViewBag to be displayed on the dashboard
            ViewBag.LowStock = products.Count(p => p.QuantityInStock <= 5 && p.Available); // Count the number of products that are low in stock (quantity less than or equal to 5 and available) and store it in the ViewBag to be displayed on the dashboard
            ViewBag.MonthRevenue = orders // Calculate the revenue for the current month by filtering orders based on the order date and summing their total
                .Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year) // Filter orders to include only those that were placed in the current month and year
                .Sum(o => o.OrdersTotal); // Sum the OrdersTotal property of the filtered orders to get the total revenue for the current month and store it in the ViewBag to be displayed on the dashboard

            ViewBag.RecentOrders = await _context.Orders // Retrieve the 6 most recent orders from the database, including their related order products and product details, and store them in the ViewBag to be displayed on the dashboard
                .Include(o => o.OrderProducts).ThenInclude(op => op.Products) // Include the related OrderProducts and their associated Products to have access to product details when displaying recent orders 
                .OrderByDescending(o => o.OrderDate) // Order the orders by their order date in descending order to get the most recent orders first
                .Take(6) // Take only the top 6 orders from the ordered list to limit the number of recent orders displayed on the dashboard
                .ToListAsync(); // Execute the query asynchronously and convert the result to a list to be stored in the ViewBag for display on the dashboard

            var topProductIds = await _context.OrderProducts // Calculate the top 5 best-selling products by grouping order products by their product ID, summing the quantity ordered for each product, and selecting the top 5 based on total quantity ordered
                .GroupBy(op => op.ProductsId) // Group the order products by their associated product ID to aggregate the quantity ordered for each product
                .Select(g => new { ProductsId = g.Key, TotalOrdered = g.Sum(op => op.ProductQuantity) }) // Select a new anonymous object for each group that contains the product ID and the total quantity ordered for that product by summing the ProductQuantity property of the order products in each group
                .OrderByDescending(x => x.TotalOrdered) // Order the resulting list of products by the total quantity ordered in descending order to get the best-selling products at the top of the list
                .Take(5) // Take only the top 5 products from the ordered list to limit the number of best-selling products displayed on the dashboard
                .ToListAsync(); // Execute the query asynchronously and convert the result to a list to be stored in the variable for further processing to get the product names for display on the dashboard
            var pIds = topProductIds.Select(x => x.ProductsId).ToList(); // Extract the product IDs from the list of top products to use for retrieving the product names from the database
            var pNames = await _context.Products.Where(p => pIds.Contains(p.ProductsId)) // 
                             .ToDictionaryAsync(p => p.ProductsId, p => p.ItemName);
            ViewBag.TopProducts = topProductIds
                .Where(x => pNames.ContainsKey(x.ProductsId))
                .Select(x => new { Name = pNames[x.ProductsId], x.TotalOrdered })
                .ToList();

            var sixMonthsAgo = DateOnly.FromDateTime(now.AddMonths(-5));
            ViewBag.MonthlyRevenue = orders
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yy"),
                    Revenue = g.Sum(o => o.OrdersTotal),
                    g.Key.Year,
                    g.Key.Month
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            ViewBag.StatusBreakdown = orders
                .GroupBy(o => o.OrderStatus ?? "Unknown")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            return View();
        }

        // ── Orders ───────────────────────────────────────────────────────
        public async Task<IActionResult> Orders(string search = "", string orderStatus = "", string trackingStatus = "", string sort = "newest")
        {
            var query = _context.Orders
                .Include(o => o.OrderProducts).ThenInclude(op => op.Products)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int orderId))
                    query = query.Where(o => o.OrdersId == orderId);
                else
                    query = query.Where(o => o.UserId.ToLower().Contains(s) ||
                                             o.BillingCity.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(orderStatus))
                query = query.Where(o => o.OrderStatus == orderStatus);

            if (!string.IsNullOrWhiteSpace(trackingStatus))
                query = query.Where(o => o.TrackingStatus == trackingStatus);

            query = sort switch
            {
                "oldest" => query.OrderBy(o => o.OrderDate),
                "total_asc" => query.OrderBy(o => o.OrdersTotal),
                "total_desc" => query.OrderByDescending(o => o.OrdersTotal),
                _ => query.OrderByDescending(o => o.OrderDate)
            };

            var orders = await query.ToListAsync();

            ViewBag.Search = search;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.TrackingStatus = trackingStatus;
            ViewBag.Sort = sort;

            return View(orders);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTrackingStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null) { order.TrackingStatus = status; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Orders));
        }

        // ── Products ─────────────────────────────────────────────────────
        public async Task<IActionResult> Products(string search = "", string category = "", string availability = "", string sort = "name")
        {
            var query = _context.Products.Include(p => p.Producers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.ItemName.ToLower().Contains(s) ||
                                         p.Category.ToLower().Contains(s) ||
                                         p.Producers.ProducerName.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category == category);

            query = availability switch
            {
                "available" => query.Where(p => p.Available),
                "unavailable" => query.Where(p => !p.Available),
                "lowstock" => query.Where(p => p.QuantityInStock <= 5 && p.Available),
                _ => query
            };

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.ItemPrice),
                "price_desc" => query.OrderByDescending(p => p.ItemPrice),
                "stock_asc" => query.OrderBy(p => p.QuantityInStock),
                "stock_desc" => query.OrderByDescending(p => p.QuantityInStock),
                _ => query.OrderBy(p => p.ItemName)
            };

            var products = await query.ToListAsync();
            var categories = await _context.Products.Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync();

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Availability = availability;
            ViewBag.Sort = sort;
            ViewBag.Categories = categories;

            return View(products);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProductAvailability(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false });
            product.Available = !product.Available;
            await _context.SaveChangesAsync();
            return Json(new { success = true, available = product.Available });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) { _context.Products.Remove(product); await _context.SaveChangesAsync(); }
            TempData["AdminSuccess"] = "Product deleted.";
            return RedirectToAction(nameof(Products));
        }

        // ── Producers ────────────────────────────────────────────────────
        public async Task<IActionResult> Producers(string search = "")
        {
            var query = _context.Producers
                .Include(p => p.Products)
                .Include(p => p.ProducerOrders)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.ProducerName.ToLower().Contains(s) ||
                                         p.ProducerEmail.ToLower().Contains(s));
            }

            var producers = await query.OrderBy(p => p.ProducerName).ToListAsync();
            ViewBag.Search = search;
            return View(producers);
        }

        // ── Users ────────────────────────────────────────────────────────
        public async Task<IActionResult> Users(string search = "", string role = "")
        {
            var allUsers = _userManager.Users.OrderBy(u => u.Email).ToList();
            var rows = new List<AdminUserRow>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role)) continue;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim().ToLower();
                    if (!(user.Email?.ToLower().Contains(s) == true ||
                          user.UserName?.ToLower().Contains(s) == true)) continue;
                }

                var orderCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);
                rows.Add(new AdminUserRow(user, roles, orderCount));
            }

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).OrderBy(n => n).ToList();

            return View(rows);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            await _userManager.AddToRoleAsync(user, roleName);
            TempData["AdminSuccess"] = $"Role '{roleName}' assigned to {user.Email}.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            await _userManager.RemoveFromRoleAsync(user, roleName);
            TempData["AdminSuccess"] = $"Role '{roleName}' removed from {user.Email}.";
            return RedirectToAction(nameof(Users));
        }

        // ── Settings ─────────────────────────────────────────────────────
        public async Task<IActionResult> Settings()
        {
            ViewBag.UserCount = _userManager.Users.Count();
            ViewBag.OrderCount = await _context.Orders.CountAsync();
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.ProducerCount = await _context.Producers.CountAsync();
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.OrdersTotal) ?? 0m;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).OrderBy(n => n).ToList();
            ViewBag.ActiveBaskets = await _context.Basket.CountAsync(b => b.Status);
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName) && !await _roleManager.RoleExistsAsync(roleName.Trim()))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                TempData["AdminSuccess"] = $"Role '{roleName.Trim()}' created.";
            }
            return RedirectToAction(nameof(Settings));
        }
    }

    public record AdminUserRow(IdentityUser User, IList<string> Roles, int OrderCount);
}
