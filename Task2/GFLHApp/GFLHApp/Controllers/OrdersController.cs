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
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GFLHApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create(int basketId) // Accept the BasketId as a parameter
        {
            ViewBag.BasketId = basketId; // Pass the BasketId to the view using ViewBag
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryMethod,DateOfCollection,BillingLine1,BillingLine2,BillingCity,BillingPostcode,DeliveryLine1,DeliveryLine2,DeliveryCity,DeliveryPostcode")] Orders orders, int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is authenticated
            {
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                return View(orders); // Return the view with the current order data
            }

            // Assign values
            orders.UserId = userId; // Set the UserId to the current user's ID
            ModelState.Remove("UserId"); // Remove UserId from model state validation

            // Set the OrderDate to the current date
            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today); // Set the OrderDate to the current date
            ModelState.Remove("OrderDate"); // Remove OrderDate from model state validation

            // Set the TrackingStatus to Pending
            orders.TrackingStatus = "Pending"; // Set the TrackingStatus to Pending when the order is first created
            ModelState.Remove("TrackingStatus"); // Remove TrackingStatus from model state validation

            // Validate the model state after removing the properties

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.BasketId == basketId && x.Status); // Retrieve the basket using the provided BasketId

            if (basket == null) // Check if the basket exists and is active
            {
                return NotFound(); // Return a 404 Not Found if the basket is not found or inactive
            }

            // Get basket products
            var basketProducts = await _context.BasketProducts.Where(x => x.BasketId == basketId).Include(x => x.Products).ToListAsync(); // Retrieve the products in the basket

            if (!basketProducts.Any())
            {
                ModelState.AddModelError("", "Your basket is currently empty.");
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                return View(orders); // Return the view with the current order data
            }

            // Subtotal Calculation
            decimal subtotal = 0.00m; // Initialize the subtotal variable
            foreach (var basketProduct in basketProducts)
            {
                var productTotal = basketProduct.Products.ItemPrice * basketProduct.ProductQuantity; // Calculate the total price for each product in the basket
                subtotal = productTotal + subtotal; // Add the product total to the subtotal
            }

            // Discount Calculator
            var orderCouut = await _context.Orders.CountAsync(x => x.UserId == userId); // Count the number of orders placed by the current user

            // Discount logic based on the number of orders placed by the user
            decimal discount = 0m; // Initialize the discount variable

            if (orderCouut >= 5)
            {
                discount = subtotal * 0.10m; // Apply a 10% discount if the user has placed 5 or more orders
            }

            orders.OrdersTotal = subtotal - discount; // Set the order subtotal after applying the discount

            // Removing model states
            ModelState.Remove("OrdersTotal"); // Remove OrdersTotal from model state validation
            ModelState.Remove("BillingLine2"); // Remove BillingLine2 from model state validation as it is optional
            ModelState.Remove("DeliveryLine1"); // Remove DeliveryLine1 from model state validation as it is optional
            ModelState.Remove("DeliveryLine2"); // Remove DeliveryLine2 from model state validation as it is optional
            ModelState.Remove("DeliveryCity"); // Remove DeliveryCity from model state validation as it is optional
            ModelState.Remove("DeliveryPostcode"); // Remove DeliveryPostcode from model state validation as it is optional

            // Billing address validation
            if (string.IsNullOrWhiteSpace(orders.BillingLine1)) // Check if the BillingLine1 is not provided
            {
                ModelState.AddModelError("BillingLine1", "Please enter the first line of your billing address."); // Add a model error if the BillingLine1 is not provided
            }

            if (string.IsNullOrWhiteSpace(orders.BillingCity)) // Check if the BillingCity is not provided
            {
                ModelState.AddModelError("BillingCity", "Please enter your billing city."); // Add a model error if the BillingCity is not provided
            }

            if (string.IsNullOrWhiteSpace(orders.BillingPostcode)) // Check if the BillingPostcode is not provided
            {
                ModelState.AddModelError("BillingPostcode", "Please enter your billing postcode."); // Add a model error if the BillingPostcode is not provided
            }

            // Delivery or Collection validation logic
            if (!orders.Collection && !orders.Delivery) // Check if neither delivery nor collection is selected
            {
                ModelState.AddModelError("Delivery", "Please select either Delivery or Collection."); // Add a model error if neither delivery nor collection is selected
            }

            if (orders.Collection) // Check if collection is selected
            {
                ModelState.Remove("DeliveryMethod"); // Remove DeliveryMethod from model state validation

                if (orders.DateOfCollection == null) // Check if the DateOfCollection is not provided
                {
                    ModelState.AddModelError("DateOfCollection", "Please provide a date for collection."); // Add a model error if the DateOfCollection is not provided
                }
                else
                {
                    var earliestCollectionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)); // Set the earliest collection date to 2 days from today

                    if (orders.DateOfCollection.Value < earliestCollectionDate) // Check if the DateOfCollection is less than 2 days from today
                    {
                        ModelState.AddModelError("CollectionDate", "Collection must be at least 2 days from today."); // Add a model error if the DateOfCollection is less than 2 days from today
                    }
                }
            }

            if (orders.Delivery) // Check if delivery is selected
            {
                ModelState.Remove("CollectionDate"); // Remove CollectionDate from model state validation

                if (string.IsNullOrWhiteSpace(orders.DeliveryMethod)) // Check if the DeliveryMethod is not provided
                {
                    ModelState.AddModelError("DeliveryMethod", "Please select a delivery method."); // Add a model error if the DeliveryMethod is not provided
                }
            }

            // If the model state is valid, save the order to the database error check
            if (!ModelState.IsValid)
            {
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                return View(orders); // Return the view with the current order data and validation errors
            }

            // Stock validation before saving the order
            foreach (var basketProduct in basketProducts) // Iterate through each product in the basket to check stock levels
            {
                if (basketProduct.Products.QuantityInStock < basketProduct.ProductQuantity) // Check if the quantity of the product in stock is less than the quantity in the basket
                {
                    ModelState.AddModelError("", $"The stock is too low for {basketProduct.Products.ItemName}"); // Add a model error if the stock is too low for the product
                    ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                    return View(orders); // Return the view with the current order data and validation errors
                }
            }

            // Create the order and save it to the database
            _context.Orders.Add(orders); // Add the order to the database context
            await _context.SaveChangesAsync(); // Save the changes to the database

            foreach (var basketProduct in basketProducts) // Iterate through each product in the basket
            {
                var orderProduct = new OrderProducts // Create a new OrderProducts object to represent the product in the order
                {
                    OrdersId = orders.OrdersId, // Set the OrdersId to the ID of the created order
                    ProductsId = basketProduct.ProductsId, // Set the ProductsId to the ID of the product in the basket
                    ProductQuantity = basketProduct.ProductQuantity // Set the Quantity to the quantity of the product in the basket
                };

                _context.OrderProducts.Add(orderProduct); // Add the OrderProducts object to the database context

                basketProduct.Products.QuantityInStock -= basketProduct.ProductQuantity; // Reduce the quantity in stock for the product by the quantity in the basket
            }

            // Close the basket by setting its status to false
            basket.Status = false; // Set the status of the basket to false to indicate that it is closed
            await _context.SaveChangesAsync(); // Save the changes to the database

            return RedirectToAction("Index", "Home"); // Redirect the user to the home page after successfully creating the order
        }
        

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,OrderDate,DeliveryMethod,Delivery,Collection,OrdersTotal,TrackingStatus,DateOfCollection,BillingLine1,BillingLine2,BillingCity,BillingPostcode,DeliveryLine1,DeliveryLine2,DeliveryCity,DeliveryPostcode")] Orders orders)
        {
            if (id != orders.OrdersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.OrdersId))
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
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrdersId == id);
        }
    }
}
