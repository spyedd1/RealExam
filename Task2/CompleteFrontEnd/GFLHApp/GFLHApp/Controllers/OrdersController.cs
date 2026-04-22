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
using Microsoft.AspNetCore.Authorization;

namespace GFLHApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task SyncDeliveryStatuses(List<Orders> orders)
        {
            foreach (var order in orders.Where(o => o.Delivery))
            {
                double days = (DateTime.Now - order.OrderDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
                int preparingDays = order.DeliveryMethod switch
                {
                    "Next Day" => 1,
                    "Standard" => 3,
                    "Economy" => 7,
                };

                string newStatus;
                if (order.DeliveryConfirmed || days >= preparingDays + 3)
                    newStatus = "Delivered";
                else if (days >= preparingDays)
                    newStatus = "Awaiting Confirmation";
                else
                    newStatus = "Preparing Delivery";

                if (order.TrackingStatus != newStatus)
                    order.TrackingStatus = newStatus;
            }

            await _context.SaveChangesAsync();
        }

        // POST: Orders/ConfirmDelivery/5 - This action method allows a user to confirm the delivery of an order. It checks if the order belongs to the current user, updates the delivery confirmation status, and saves the changes to the database.

        [HttpPost]
        [Authorize(Roles = "Standard,Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.OrdersId == id && x.UserId == userId);

            if (order == null) return NotFound();

            order.DeliveryConfirmed = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // or wherever your order history pag e is
        }

        [Authorize(Roles = "Standard,Developer,Admin")]
        // GET: Orders
        public async Task<IActionResult> Index(List<Orders> orders)
        {


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null)
            {
                return Unauthorized(); // Return a 401 Unauthorized if the user is not authenticated
            }

            if (User.IsInRole("Admin")) // Check if the user is in the Admin role
            {
                var allOrders = await _context.Orders.Include(x => x.OrderProducts).ThenInclude(x => x.Products).ToListAsync(); // Retrieve all orders, including the related order products and products data for admin users
                return View(allOrders); // Return the view with all orders for admin users
            }
            else if (User.IsInRole("Producer"))
            {
                var producerOrders = await _context.ProducerOrders // Retrieve the producer orders for the current producer, including the related orders, order products, and products data
                    .Where(x => x.ProducerId == userId)
                    .Include(x => x.Orders)
                    .Include(x => x.OrderProducts)
                        .ThenInclude(x => x.Products)
                    .ToListAsync();

                return View(producerOrders.Select(op => op.Orders).Distinct().ToList()); // separate view for producers
            }
            else
            {
                var userOrders = await _context.Orders.Where(x => x.UserId == userId).Include(x => x.OrderProducts).ThenInclude(x => x.Products).ToListAsync(); // Retrieve the orders for the current user, including the related order products and products data
                return View(userOrders); // Return the view with the user's orders
            }
        }

        [Authorize(Roles = "Standard,Developer")]
        // GET: Orders/Details/5  
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID  

            var order = await _context.Orders // Start a query on the Orders table
                .Include(o => o.OrderProducts) // Include the related OrderProducts
                .ThenInclude(op => op.Products) // Then include the related Products for each OrderProduct
                .FirstOrDefaultAsync(o => o.OrdersId == id && o.UserId == userId); // Find the order with the specified ID that belongs to the current user

            if (order == null) // Check if the order was found and belongs to the user
            {
                return Unauthorized(); // Return a 401 Unauthorized if the order is not found or does not belong to the user
            }

            return View(order); // Return the order object to the view  
        }

        [Authorize(Roles = "Standard,Developer")]
        // GET: Orders/Create
        public async Task<IActionResult> Create(int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is authenticated
            {
                return Unauthorized();
            }

            var basketProducts = await _context.BasketProducts
                            .Where(x => x.BasketId == basketId)
                            .Include(x => x.Products)
                            .ThenInclude(x => x.Producers) // Include producers so we can check VAT registration
                            .ToListAsync(); // Retrieve the products in the basket

            decimal subtotal = 0m; // Initialize the subtotal variable
            foreach (var item in basketProducts)
            {
                subtotal += item.Products.ItemPrice * item.ProductQuantity; // Calculate the subtotal
            }

            var orderCount = await _context.Orders.CountAsync(x => x.UserId == userId); // Count the number of orders placed by the current user

            // Health bundle discount: 10% off if basket contains broccoli, carrot, AND apple
            var productNames = basketProducts.Select(x => x.Products.ItemName.ToLower()).ToList();
            bool hasHealthBundle = productNames.Contains("broccoli") &&
                                   productNames.Contains("carrot") &&
                                   productNames.Contains("apple");

            // Loyalty discount calculation
            decimal discount = 0m; // Initialize the discount variable
            if (orderCount % 5 == 4) // Check if this is the user's 5th, 10th, 15th... order
            {
                discount = subtotal * 0.15m; // Apply a 15% loyalty discount every 5th order
            }
            else if (hasHealthBundle) // Check if the basket contains the health bundle (broccoli, carrot, apple)
            {
                discount = subtotal * 0.10m; // Apply a 10% health bundle discount
            }
            decimal discountedSubtotal = subtotal - discount; // Calculate the discounted subtotal

            ViewBag.BasketId = basketId; // Pass the BasketId to the view using ViewBag
            ViewBag.Subtotal = discountedSubtotal; // Pass the discounted subtotal to the view using ViewBag
            ViewBag.HasFreeDelivery = orderCount % 3 == 2; // Pass whether the user has free delivery to the view using ViewBag
            ViewBag.HasHealthBundle = hasHealthBundle; // Pass whether the health bundle discount applies to the view
            ViewBag.BasketProducts = basketProducts; // Pass the basket products to the view for allergen display
            ViewBag.DeliveryCosts = new Dictionary<string, decimal>
    {
        { "Next Day",  5.99m },
        { "Standard",  2.99m },
        { "Economy",   0.99m }
    }; // Pass the delivery costs to the view using ViewBag

            return View();
        }


        [Authorize(Roles = "Standard,Developer")]
        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryMethod,DateOfCollection,BillingLine1,BillingLine2,BillingCity,BillingPostcode,DeliveryLine1,DeliveryLine2,DeliveryCity,DeliveryPostcode,TermsAccepted")] Orders orders, int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is authenticated
            {
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                return View(orders); // Return the view with the current order data
            }



            // Terms and conditions validation
            if (!orders.TermsAccepted) // Check if the user has accepted the terms and conditions
            {
                ModelState.AddModelError("TermsAccepted", "You must accept the terms and conditions to place an order."); // Add a model error if the terms have not been accepted
            }

            ModelState.Remove("InvoiceNumber"); // Remove InvoiceNumber from model state validation as it is auto-generated

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
            var basketProducts = await _context.BasketProducts.Where(x => x.BasketId == basketId).Include(x => x.Products).ThenInclude(x => x.Producers).ToListAsync(); // Retrieve the products in the basket

            if (!basketProducts.Any())
            {
                ModelState.AddModelError("", "Your basket is currently empty.");
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                ViewBag.BasketProducts = basketProducts; // Pass basket products back to the view for allergen display
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

            // Health bundle discount: 10% off if basket contains broccoli, carrot, AND apple
            var basketProductNames = basketProducts.Select(x => x.Products.ItemName.ToLower()).ToList();
            bool hasHealthBundle = basketProductNames.Contains("broccoli") &&
                                   basketProductNames.Contains("carrot") &&
                                   basketProductNames.Contains("apple");

            // Delivery cost
            decimal deliveryCost = 0m; // Initialize the delivery cost variable
            if (orders.Delivery) // Check if delivery is selected
            {
                if (orderCouut % 3 == 2) // Check if the user is eligible for free delivery
                {
                    deliveryCost = 0m; // Free delivery every 3rd order
                }
                else
                {
                    deliveryCost = orders.DeliveryMethod switch // Calculate the delivery cost based on the selected delivery method
                    {
                        "Next Day" => 5.99m,
                        "Standard" => 2.99m,
                        "Economy" => 0.99m,
                        _ => 0m
                    };
                }
            }

            // Discount logic: loyalty (5+ orders) OR health bundle (broccoli + carrot + apple), whichever applies
            decimal discount = 0m; // Initialize the discount variable
            if (orderCouut % 5 == 4) // Check if this is the user's 5th, 10th, 15th... order
            {
                discount = subtotal * 0.15m; // Apply a 15% loyalty discount every 5th order
            }
            else if (hasHealthBundle) // Check if the basket contains the health bundle (broccoli, carrot, apple)
            {
                discount = subtotal * 0.10m; // Apply a 10% health bundle discount
            }

            orders.OrdersTotal = (subtotal - discount) + deliveryCost; // Set the order total after applying the discount and delivery cost

            ViewBag.BasketId = basketId; // Pass the BasketId back to the view
            ViewBag.Subtotal = subtotal; // Pass the subtotal back to the view
            ViewBag.HasFreeDelivery = orderCouut % 3 == 2; // Pass whether the user has free delivery back to the view
            ViewBag.HasHealthBundle = hasHealthBundle; // Pass whether the health bundle discount applies to the view
            ViewBag.DeliveryCosts = new Dictionary<string, decimal>
            {
                { "Next Day",  5.99m },
                { "Standard",  2.99m },
                { "Economy",   0.99m }
            }; // Pass the delivery costs back to the view

            // Removing model states
            ModelState.Remove("OrdersTotal"); // Remove OrdersTotal from model state validation
            ModelState.Remove("BillingLine2"); // Remove BillingLine2 from model state validation as it is optional
            ModelState.Remove("DeliveryLine2"); // Remove DeliveryLine2 from model state validation as it is optional

            // Regex patterns
            var addressRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9 #\-,\.]{1,40}$"); // Allow letters, numbers, spaces, and specific punctuation
            var postcodeRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z]{1,2}[0-9][0-9A-Z]?\s?[0-9][A-Z]{2}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase); // British postcode format

            // Billing address validation
            if (string.IsNullOrWhiteSpace(orders.BillingLine1)) // Check if the BillingLine1 is not provided
            {
                ModelState.AddModelError("BillingLine1", "Please enter the first line of your billing address."); // Add a model error if the BillingLine1 is not provided
            }
            else if (orders.BillingLine1.Length > 40) // Check if the BillingLine1 exceeds 40 characters
            {
                ModelState.AddModelError("BillingLine1", "Address line 1 must not exceed 40 characters."); // Add a model error if the BillingLine1 exceeds 40 characters
            }
            else if (!addressRegex.IsMatch(orders.BillingLine1)) // Check if the BillingLine1 contains invalid characters
            {
                ModelState.AddModelError("BillingLine1", "Address line 1 can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the BillingLine1 contains invalid characters
            }

            if (!string.IsNullOrWhiteSpace(orders.BillingLine2)) // Only validate BillingLine2 if it is provided as it is optional
            {
                if (orders.BillingLine2.Length > 40) // Check if the BillingLine2 exceeds 40 characters
                {
                    ModelState.AddModelError("BillingLine2", "Address line 2 must not exceed 40 characters."); // Add a model error if the BillingLine2 exceeds 40 characters
                }
                else if (!addressRegex.IsMatch(orders.BillingLine2)) // Check if the BillingLine2 contains invalid characters
                {
                    ModelState.AddModelError("BillingLine2", "Address line 2 can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the BillingLine2 contains invalid characters
                }
            }

            if (string.IsNullOrWhiteSpace(orders.BillingCity)) // Check if the BillingCity is not provided
            {
                ModelState.AddModelError("BillingCity", "Please enter your billing city."); // Add a model error if the BillingCity is not provided
            }
            else if (orders.BillingCity.Length > 40) // Check if the BillingCity exceeds 40 characters
            {
                ModelState.AddModelError("BillingCity", "City must not exceed 40 characters."); // Add a model error if the BillingCity exceeds 40 characters
            }
            else if (!addressRegex.IsMatch(orders.BillingCity)) // Check if the BillingCity contains invalid characters
            {
                ModelState.AddModelError("BillingCity", "City can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the BillingCity contains invalid characters
            }

            if (string.IsNullOrWhiteSpace(orders.BillingPostcode)) // Check if the BillingPostcode is not provided
            {
                ModelState.AddModelError("BillingPostcode", "Please enter your billing postcode."); // Add a model error if the BillingPostcode is not provided
            }
            else if (!postcodeRegex.IsMatch(orders.BillingPostcode.Trim())) // Check if the BillingPostcode is a valid British postcode
            {
                ModelState.AddModelError("BillingPostcode", "Please enter a valid UK postcode (e.g. B1 1BB or SW1A 2AA)."); // Add a model error if the BillingPostcode is not a valid British postcode
            }

            // Delivery address validation
            if (orders.Delivery) // Check if delivery is selected
            {
                if (string.IsNullOrWhiteSpace(orders.DeliveryLine1)) // Check if the DeliveryLine1 is not provided
                {
                    ModelState.AddModelError("DeliveryLine1", "Please enter the first line of your delivery address."); // Add a model error if the DeliveryLine1 is not provided
                }
                else if (orders.DeliveryLine1.Length > 40) // Check if the DeliveryLine1 exceeds 40 characters
                {
                    ModelState.AddModelError("DeliveryLine1", "Address line 1 must not exceed 40 characters."); // Add a model error if the DeliveryLine1 exceeds 40 characters
                }
                else if (!addressRegex.IsMatch(orders.DeliveryLine1)) // Check if the DeliveryLine1 contains invalid characters
                {
                    ModelState.AddModelError("DeliveryLine1", "Address line 1 can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the DeliveryLine1 contains invalid characters
                }

                if (!string.IsNullOrWhiteSpace(orders.DeliveryLine2)) // Only validate DeliveryLine2 if it is provided as it is optional
                {
                    if (orders.DeliveryLine2.Length > 40) // Check if the DeliveryLine2 exceeds 40 characters
                    {
                        ModelState.AddModelError("DeliveryLine2", "Address line 2 must not exceed 40 characters."); // Add a model error if the DeliveryLine2 exceeds 40 characters
                    }
                    else if (!addressRegex.IsMatch(orders.DeliveryLine2)) // Check if the DeliveryLine2 contains invalid characters
                    {
                        ModelState.AddModelError("DeliveryLine2", "Address line 2 can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the DeliveryLine2 contains invalid characters
                    }
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryCity)) // Check if the DeliveryCity is not provided
                {
                    ModelState.AddModelError("DeliveryCity", "Please enter your delivery city."); // Add a model error if the DeliveryCity is not provided
                }
                else if (orders.DeliveryCity.Length > 40) // Check if the DeliveryCity exceeds 40 characters
                {
                    ModelState.AddModelError("DeliveryCity", "City must not exceed 40 characters."); // Add a model error if the DeliveryCity exceeds 40 characters
                }
                else if (!addressRegex.IsMatch(orders.DeliveryCity)) // Check if the DeliveryCity contains invalid characters
                {
                    ModelState.AddModelError("DeliveryCity", "City can only contain letters, numbers, spaces, and the following: # - , ."); // Add a model error if the DeliveryCity contains invalid characters
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryPostcode)) // Check if the DeliveryPostcode is not provided
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter your delivery postcode."); // Add a model error if the DeliveryPostcode is not provided
                }
                else if (!postcodeRegex.IsMatch(orders.DeliveryPostcode.Trim())) // Check if the DeliveryPostcode is a valid British postcode
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter a valid UK postcode (e.g. B1 1BB or SW1A 2AA)."); // Add a model error if the DeliveryPostcode is not a valid British postcode
                }
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
                    var today = DateOnly.FromDateTime(DateTime.Today); // Get today's date
                    var earliestCollectionDate = today.AddDays(2); // Set the earliest collection date to 2 days from today
                    var latestCollectionDate = today.AddDays(14); // Set the latest collection date to 14 days from today

                    if (orders.DateOfCollection.Value < today) // Check if the date is in the past
                    {
                        ModelState.AddModelError("DateOfCollection", "Collection date must be in the present or future."); // Add a model error if the date is in the past
                    }
                    else if (orders.DateOfCollection.Value < earliestCollectionDate) // Check if the DateOfCollection is less than 2 days from today
                    {
                        ModelState.AddModelError("DateOfCollection", "Collection must be at least 2 days from today."); // Add a model error if the DateOfCollection is less than 2 days from today
                    }
                    else if (orders.DateOfCollection.Value > latestCollectionDate) // Check if the DateOfCollection is more than 14 days from today
                    {
                        ModelState.AddModelError("DateOfCollection", "Collection date must be within the next 14 days from today."); // Add a model error if the DateOfCollection is more than 14 days from today
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

                // Only validate delivery address fields if same as billing checkbox was not used
                if (string.IsNullOrWhiteSpace(orders.DeliveryLine1)) // Check if the DeliveryLine1 is not provided
                {
                    ModelState.AddModelError("DeliveryLine1", "Please enter the first line of your delivery address."); // Add a model error if the DeliveryLine1 is not provided
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryCity)) // Check if the DeliveryCity is not provided
                {
                    ModelState.AddModelError("DeliveryCity", "Please enter your delivery city."); // Add a model error if the DeliveryCity is not provided
                }

                if (string.IsNullOrWhiteSpace(orders.DeliveryPostcode)) // Check if the DeliveryPostcode is not provided
                {
                    ModelState.AddModelError("DeliveryPostcode", "Please enter your delivery postcode."); // Add a model error if the DeliveryPostcode is not provided
                }
            }

            // If the model state is valid, save the order to the database error check
            if (!ModelState.IsValid)
            {
                ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                ViewBag.BasketProducts = basketProducts; // Pass basket products back to the view for allergen display
                return View(orders); // Return the view with the current order data and validation errors
            }

            // Stock validation before saving the order
            foreach (var basketProduct in basketProducts) // Iterate through each product in the basket to check stock levels
            {
                if (basketProduct.Products.QuantityInStock < basketProduct.ProductQuantity) // Check if the quantity of the product in stock is less than the quantity in the basket
                {
                    ModelState.AddModelError("", $"The stock is too low for {basketProduct.Products.ItemName}"); // Add a model error if the stock is too low for the product
                    ViewBag.BasketId = basketId; // Pass the BasketId back to the view
                    ViewBag.BasketProducts = basketProducts; // Pass basket products back to the view for allergen display
                    return View(orders); // Return the view with the current order data and validation errors
                }
            }

            // Create the order and save it to the database
            _context.Orders.Add(orders); // Add the order to the database context
            await _context.SaveChangesAsync(); // Save the changes to the database

            // Group basket products by producer to create producer order slices
            var groupedByProducer = basketProducts.GroupBy(x => x.Products.Producers.UserId); ; // ← changed from .UserId

            foreach (var producerGroup in groupedByProducer) // Iterate through each producer group to create a producer order slice for each producer involved in the order
            {
                decimal producerSubtotal = 0m; // Initialize the producer subtotal variable for this producer's slice of the order
                foreach (var item in producerGroup) // Iterate through each product in the producer group to calculate the subtotal for this producer's slice of the order
                {
                    producerSubtotal += item.Products.ItemPrice * item.ProductQuantity; // Calculate the subtotal for this producer's slice of the order by summing the total price for each product in the producer group
                }

                var producerOrder = new ProducerOrders
                {
                    OrdersId = orders.OrdersId,
                    ProducerId = producerGroup.Key,        // ← now correctly an int ProducersId
                    ProducerSubtotal = producerSubtotal,
                    TrackingStatus = "Pending"
                };

                _context.ProducerOrders.Add(producerOrder); // Add the ProducerOrders object to the database context
                await _context.SaveChangesAsync(); // Save to get the ProducerOrdersId generated

                // Create order products linked to this producer order slice
                foreach (var basketProduct in producerGroup) // Iterate through each product in the producer group
                {
                    var orderProduct = new OrderProducts // Create a new OrderProducts object to represent the product in the order
                    {
                        OrdersId = orders.OrdersId, // Set the OrdersId to the ID of the created order
                        ProducerOrdersId = producerOrder.ProducerOrdersId, // Link to this producer's slice
                        ProductsId = basketProduct.ProductsId, // Set the ProductsId to the ID of the product in the basket
                        ProductQuantity = basketProduct.ProductQuantity, // Set the Quantity to the quantity of the product in the basket
                        InvoiceNumber = (basketProduct.Products.Producers != null && basketProduct.Products.Producers.IsVATRegistered)
                            ? $"INV-{orders.OrderDate:yyyyMMdd}-{orders.OrdersId:D6}-{basketProduct.Products.Producers.ProducersId}"
                            : null // Only generate an invoice number if the producer is VAT registered
                    };

                    _context.OrderProducts.Add(orderProduct); // Add the OrderProducts object to the database context
                    basketProduct.Products.QuantityInStock -= basketProduct.ProductQuantity; // Reduce the quantity in stock for the product
                }
            }

            // Close the basket by setting its status to false
            basket.Status = false; // Set the status of the basket to false to indicate that it is closed
            await _context.SaveChangesAsync(); // Save the changes to the database

            return RedirectToAction("Confirmation", "Orders", new { id = orders.OrdersId }); // Redirect to the confirmation page with the order ID
        }


        // GET: Orders/Edit/5
        [Authorize(Roles = "Developer")]
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
        [Authorize(Roles = "Developer")]
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
        [Authorize(Roles = "Developer")]
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
        [Authorize(Roles = "Developer")]
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


        // GET: Orders/Confirmation

        // This action method retrieves the details of a specific order for the confirmation page. It checks if the order belongs to the current user, includes related data such as products and producers, and returns the order data to the confirmation view.
        [Authorize(Roles = "Standard,Developer")]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (userId == null) // Check if the user is authenticated
            {
                return Unauthorized();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Products)
                .ThenInclude(p => p.Producers) // Include producers so we can check VAT registration on the confirmation page
                .FirstOrDefaultAsync(o => o.OrdersId == id && o.UserId == userId); // Retrieve the order with its products, ensuring it belongs to the current user

            if (order == null) // Check if the order exists and belongs to the current user
            {
                return NotFound();
            }

            return View(order); // Return the confirmation view with the order data
        }



    }
}
