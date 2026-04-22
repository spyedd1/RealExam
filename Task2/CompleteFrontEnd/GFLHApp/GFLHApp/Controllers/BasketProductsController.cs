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
    public class BasketProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasketProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BasketProducts
        public async Task<IActionResult> Index() // This action method retrieves a list of all basket products from the database, including their associated basket and product details, and returns the view to display them
        {
            // Query the BasketProducts table and include the related Basket and Products details in the results, then convert the results to a list asynchronously and pass it to the view for display
            var applicationDbContext = _context.BasketProducts.Include(b => b.Basket).Include(b => b.Products);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BasketProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            // Check if the provided id is null, if it is then return a 404 Not Found response since we cannot find a basket product without an ID
            if (id == null)
            {
                return NotFound();
            }

            var basketProducts = await _context.BasketProducts
                .Include(b => b.Basket)
                .Include(b => b.Products)
                .FirstOrDefaultAsync(m => m.BasketProductsId == id);
            if (basketProducts == null)
            {
                return NotFound(); // Return a 404 Not Found response if the specified basket product does not exist in the database
            }

            return View(basketProducts); // Return the details view for the specific basket product if it exists, otherwise return a 404 Not Found response
        }

        // GET: BasketProducts/Create
        public IActionResult Create()
        {
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId");
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId");
            return View();
        }

        // POST: BasketProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ProductsId)
        {
            // First find the product selected by the user using the provided ProductsId
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId); // Get the product details from the database

            if (product == null) // Check if the product exists
            {
                return NotFound(); // Return a 404 Not Found response if the product doesn't exist
            }

            // Check product availability, if it isn't available then give error message
            if (!product.Available) // Check if the product is available
            {
                TempData["Error"] = "This product is currently unavailable and cannot be added to your basket.";
                return RedirectToAction("Index", "Products"); // Redirect back to the products page
            }

            // Find the current logged in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is logged in
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user is not logged in
            }

            // Find the user's basket, if no basket then create a new one
            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Get the active basket for the user

            if (basket == null) // If the user doesn't have an active basket, create a new one
            {
                basket = new Basket // Create a new basket for the user
                {
                    Status = true, // Set the basket status to active
                    UserId = userId, // Set the user ID for the basket
                    CreatedAt = DateTime.UtcNow // Set the creation time for the basket
                };

                _context.Basket.Add(basket); // Add the new basket to the database context
                await _context.SaveChangesAsync(); // Save the changes to the database
            }

            // Add the products to the basket
            var basketProduct = await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId); // Check if the product is already in the basket

            if (basketProduct != null) // If the product is already in the basket, update the quantity
            {
                basketProduct.ProductQuantity++; // Increment the product quantity by 1
            }
            else // If the product is not in the basket, add it as a new entry
            {
                basketProduct = new BasketProducts // Create a new BasketProducts entry
                {
                    BasketId = basket.BasketId, // Set the BasketId to the user's basket
                    ProductsId = ProductsId, // Set the ProductsId to the selected product
                    ProductQuantity = 1 // Set the initial quantity to 1
                };
                _context.BasketProducts.Add(basketProduct); // Add the new BasketProducts entry to the database context
            }

            await _context.SaveChangesAsync(); // Save the changes to the database

            return RedirectToAction("Index", "Baskets"); // Redirect the user to the basket index page after adding the product
        }


        // GET: BasketProducts/GetCount — returns current basket item total as JSON for the live badge
        [HttpGet]

        public async Task<IActionResult> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Json(new { count = 0 });

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status);
            if (basket == null) return Json(new { count = 0 });

            var count = await _context.BasketProducts
                .Where(bp => bp.BasketId == basket.BasketId)
                .SumAsync(bp => bp.ProductQuantity);

            return Json(new { count });
        }

        // POST: BasketProducts/AddAjax — AJAX add with quantity, returns JSON so the page doesn't reload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAjax(int ProductsId, int Quantity = 1)
        {
            if (Quantity < 1) Quantity = 1;
            if (Quantity > 99) Quantity = 99;

            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId);
            if (product == null)
                return Json(new { success = false, message = "Product not found." });

            if (!product.Available)
                return Json(new { success = false, message = "This product is currently unavailable." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Json(new { success = false, message = "Please log in to add items to your basket." });

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status);
            if (basket == null)
            {
                basket = new Basket { Status = true, UserId = userId, CreatedAt = DateTime.UtcNow };
                _context.Basket.Add(basket);
                await _context.SaveChangesAsync();
            }

            var basketProduct = await _context.BasketProducts
                .FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId);

            if (basketProduct != null)
                basketProduct.ProductQuantity += Quantity;
            else
                _context.BasketProducts.Add(new BasketProducts
                {
                    BasketId = basket.BasketId,
                    ProductsId = ProductsId,
                    ProductQuantity = Quantity
                });

            await _context.SaveChangesAsync();

            var basketCount = await _context.BasketProducts
                .Where(bp => bp.BasketId == basket.BasketId)
                .SumAsync(bp => bp.ProductQuantity);

            return Json(new { success = true, basketCount, itemName = product.ItemName, quantity = Quantity });
        }


        // GET: BasketProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basketProducts = await _context.BasketProducts.FindAsync(id);
            if (basketProducts == null)
            {
                return NotFound();
            }
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", basketProducts.ProductsId);
            return View(basketProducts);
        }

        // POST: BasketProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BasketProductsId,BasketId,ProductsId,ProductQuantity")] BasketProducts basketProducts)
        {
            if (id != basketProducts.BasketProductsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basketProducts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketProductsExists(basketProducts.BasketProductsId))
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
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "ProductsId", "ProductsId", basketProducts.ProductsId);
            return View(basketProducts);
        }

        // GET: BasketProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basketProducts = await _context.BasketProducts
                .Include(b => b.Basket)
                .Include(b => b.Products)
                .FirstOrDefaultAsync(m => m.BasketProductsId == id);
            if (basketProducts == null)
            {
                return NotFound();
            }

            return View(basketProducts);
        }

        [HttpPost]
        [Authorize(Roles = "Standard,Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromBasket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var basketProduct = await _context.BasketProducts
                .Include(bp => bp.Basket)
                .FirstOrDefaultAsync(bp => bp.BasketProductsId == id
                    && bp.Basket != null
                    && bp.Basket.UserId == userId
                    && bp.Basket.Status);

            if (basketProduct == null)
            {
                return NotFound();
            }

            _context.BasketProducts.Remove(basketProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Baskets");
        }

        // POST: BasketProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var basketProducts = await _context.BasketProducts.FindAsync(id);
            if (basketProducts != null)
            {
                _context.BasketProducts.Remove(basketProducts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasketProductsExists(int id)
        {
            return _context.BasketProducts.Any(e => e.BasketProductsId == id);
        }
    }
}
