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

namespace GFLHApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? producerId)
        {
            if (User.IsInRole("Producer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                    return Unauthorized();

                var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId);

                if (producer == null)
                    return NotFound();

                var producerProducts = await _context.Products
                    .Where(p => p.ProducersId == producer.ProducersId)
                    .Include(p => p.Producers)
                    .ToListAsync();

                return View(producerProducts);
            }
            else
            {
                var query = _context.Products.Include(p => p.Producers).AsQueryable();

                if (producerId.HasValue)
                    query = query.Where(p => p.ProducersId == producerId.Value);

                return View(await query.ToListAsync());
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Producers)
                .FirstOrDefaultAsync(m => m.ProductsId == id);

            if (products == null)
            {
                return NotFound();
            }

            // Pass up to 3 random other products from the same producer for the "More from" section
            var allOthers = await _context.Products
                .Where(p => p.ProducersId == products.ProducersId && p.ProductsId != id)
                .Include(p => p.Producers)
                .ToListAsync();

            var rng = new Random();
            ViewBag.OtherProducts = allOthers.OrderBy(_ => rng.Next()).Take(3).ToList();

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProducersId"] = new SelectList(_context.Producers, "ProducersId", "ProducersId");
            return View();
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId);
            if (producer == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductsId == id && p.ProducersId == producer.ProducersId);

            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,ItemName,ItemPrice,ImagePath,QuantityInStock,Available,Category,Description,Allergens")] Products products)
        {
            if (id != products.ProductsId)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            if (userId == null)
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user is not logged in
            }

            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Find the producer associated with the current user
            if (producer == null)
            {
                return NotFound(); // Return a 404 Not Found response if the producer is not found
            }

            products.ProducersId = producer.ProducersId; // Set the ProducersId of the product to the producer's ID
            ModelState.Remove("ProducersId"); // Remove the ProducersId from the model state to prevent validation errors

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductsId))
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
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            if (userId == null)
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user is not logged in
            }

            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Find the producer associated with the current user
            if (producer == null)
            {
                return NotFound(); // Return a 404 Not Found response if the producer is not found
            }

            var products = await _context.Products
                .Include(p => p.Producers)
                .FirstOrDefaultAsync(m => m.ProductsId == id && m.ProducersId == producer.ProducersId); // Verify the product belongs to this producer

            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            if (userId == null)
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user is not logged in
            }

            var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Find the producer associated with the current user
            if (producer == null)
            {
                return NotFound(); // Return a 404 Not Found response if the producer is not found
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductsId == id && p.ProducersId == producer.ProducersId); // Verify the product belongs to this producer before deleting

            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductsId == id);
        }
    }
}