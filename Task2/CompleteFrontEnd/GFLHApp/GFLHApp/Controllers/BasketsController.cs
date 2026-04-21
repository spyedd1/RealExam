using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GFLHApp.Data;
using GFLHApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GFLHApp.Controllers
{
    [Authorize(Roles = "Standard,Developer")]
    public class BasketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Baskets
        public async Task<IActionResult> Index()
        {
            // Start by finding the current logged in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is logged in
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user is not logged in
            }

            // Check if user has a basket, if they don't then we will create one for them
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Find the active basket for the user

            if (basket == null) // If the user doesn't have an active basket, create one
            {
                basket = new Basket // Create a new basket for the user
                {
                    UserId = userId, // Set the UserId to the current user's ID
                    Status = true, // Set the status to true (active)
                    CreatedAt = DateTime.UtcNow // Set the creation time to now
                };

                _context.Basket.Add(basket); // Add the new basket to the database context
                await _context.SaveChangesAsync(); // Save the changes to the database
            }

            // Next find the products in the basket, and find the subtotal of the basket
            var basketProducts = await _context.BasketProducts // Start a query on the BasketProducts table
                .Where(bp => bp.BasketId == basket.BasketId) // Find all products in the user's basket
                .Include(bp => bp.Basket) // Include the related basket
                .Include(bp => bp.Products) // Include the related product details
                .ToListAsync(); // Execute the query and get the results as a list

            decimal subtotal = 0m; // Initialize the subtotal variable

            foreach (var basketProduct in basketProducts) // Loop through each product in the basket
            {
                var productTotal = basketProduct.Products.ItemPrice * basketProduct.ProductQuantity; // Calculate the total price for the current product (price * quantity)
                subtotal += productTotal; // Add the product total to the subtotal
            }

            // Loyalty discount calculation
            var orderCount = await _context.Orders.CountAsync(o => o.UserId == userId); // Get the total number of orders the user has made

            // Health bundle discount: 10% off if basket contains broccoli, carrot, AND apple
            var productNames = basketProducts.Select(x => x.Products.ItemName.ToLower()).ToList();
            bool hasHealthBundle = productNames.Contains("broccoli") &&
                                   productNames.Contains("carrot") &&
                                   productNames.Contains("apple");

            decimal discount = 0m; // Initialize the discount variable

            if (orderCount % 5 == 4) // Check if this is the user's 5th, 10th, 15th... order
            {
                discount = subtotal * 0.15m; // Apply a 15% loyalty discount every 5th order
            }
            else if (hasHealthBundle) // Check if the basket contains the health bundle (broccoli, carrot, apple)
            {
                discount = subtotal * 0.10m; // Apply a 10% health bundle discount
            }

            decimal total = subtotal - discount; // Calculate the final total after applying the discount

            // Finally, send to the viewbag and return the view.
            ViewBag.Subtotal = subtotal; // Pass the subtotal to the view using ViewBag
            ViewBag.Discount = discount; // Pass the discount to the view using ViewBag
            ViewBag.Total = total; // Pass the total to the view using ViewBag
            ViewBag.OrderCount = orderCount; // Pass the order count to the view using ViewBag
            ViewBag.HasHealthBundle = hasHealthBundle; // Pass whether the health bundle discount applies to the view

            return View(basketProducts); // Return the view with the list of products in the basket
        }

        // GET: Baskets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .FirstOrDefaultAsync(m => m.BasketId == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // GET: Baskets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Baskets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BasketId,UserId,Status,CreatedAt")] Basket basket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(basket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(basket);
        }

        // GET: Baskets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket.FindAsync(id);
            if (basket == null)
            {
                return NotFound();
            }
            return View(basket);
        }

        // POST: Baskets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BasketId,UserId,Status,CreatedAt")] Basket basket)
        {
            if (id != basket.BasketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketExists(basket.BasketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(basket);
        }

        // GET: Baskets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .FirstOrDefaultAsync(m => m.BasketId == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // POST: Baskets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var basket = await _context.Basket.FindAsync(id);
            if (basket != null)
            {
                _context.Basket.Remove(basket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.BasketId == id);
        }
    }
}