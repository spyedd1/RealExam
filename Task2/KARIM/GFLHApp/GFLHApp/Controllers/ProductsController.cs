// ----- Imports -----
using System; // Provides core .NET types used by this controller.
using System.IO; // Provides file and directory APIs for uploaded images.
using System.Linq; // Provides LINQ filtering, grouping, and sorting helpers.
using System.Security.Claims; // Provides access to the signed-in user's claim values.
using System.Threading.Tasks; // Provides asynchronous Task return types.
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.
using Microsoft.AspNetCore.Hosting; // Provides web root path information for file uploads.
using Microsoft.AspNetCore.Http; // Provides uploaded file abstractions.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.AspNetCore.Mvc.Rendering; // Provides SelectList helpers for form dropdowns.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{

    // ----- Controller Declaration -----
    public class ProductsController : Controller // Defines the MVC controller for product catalogue browsing and producer product management.
    {

        // ----- Controller Dependencies -----
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"]; // Lists the image extensions accepted by upload validation.
        private const long MaxImageUploadBytes = 5 * 1024 * 1024; // Sets the maximum accepted image upload size to five megabytes.

        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.
        private readonly IWebHostEnvironment _environment; // Holds web-host paths used when listing or saving image files.

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment) // Receives database and hosting services from dependency injection.
        {
            _context = context; // Stores the injected dependency on the controller field.
            _environment = environment; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        // Producer order splitting
        public async Task<IActionResult> Index(int? producerId) // Loads the main listing or dashboard view for this controller
        {
            // Role management
            if (User.IsInRole("Producer") && !producerId.HasValue) // Branches based on the signed-in user's role unless browsing a selected producer publicly.
            {
                // Set user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.

                if (userId == null) // Checks that a signed-in user id is available.
                    return Unauthorized(); // Returns 401 when the current user is not allowed to continue.

                // Producer lookup
                var producer = await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Fetches the first matching record or null if none exists.

                if (producer == null) // Checks whether the requested data was found.
                    return NotFound(); // Returns 404 when the requested record is missing or inaccessible.

                // Product lookup
                var producerProducts = await _context.Products // Sets producerProducts to the value needed by the workflow.
                    .Where(p => p.ProducersId == producer.ProducersId) // Filters the query to only the relevant records.
                    .Include(p => p.Producers) // Includes related records needed by the view or workflow.
                    .ToListAsync(); // Executes the query asynchronously and materializes the list.

                return View(producerProducts); // Renders the matching view with the supplied model data.
            }

            var query = _context.Products.Include(p => p.Producers).AsQueryable(); // Includes related records needed by the view or workflow.

            // Producer order splitting
            if (producerId.HasValue) // Checks the condition that controls the next action.
                query = query.Where(p => p.ProducersId == producerId.Value); // Filters the query to only the relevant records.

            return View(await query.ToListAsync()); // Renders the matching view with the supplied model data.
        }

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Product lookup
            var products = await _context.Products // Sets products to the value needed by the workflow.
                .Include(p => p.Producers) // Includes related records needed by the view or workflow.
                .FirstOrDefaultAsync(m => m.ProductsId == id); // Fetches the first matching record or null if none exists.

            if (products == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var allOthers = await _context.Products // Loads other products from the same producer for recommendations.
                // Product lookup
                .Where(p => p.ProducersId == products.ProducersId && p.ProductsId != id) // Filters the query to only the relevant records.
                .Include(p => p.Producers) // Includes related records needed by the view or workflow.
                .ToListAsync(); // Executes the query asynchronously and materializes the list.

            var rng = new Random(); // Creates the rng object used by the following workflow.
            ViewBag.OtherProducts = allOthers.OrderBy(_ => rng.Next()).Take(3).ToList(); // Sorts the query results for display or processing.

            return View(products); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Producer,Developer")] // Restricts access to users in the Producer,Developer role set.

        // ----- Create Actions -----
        // Create record
        public IActionResult Create() // Shows or processes the create form for a record
        {
            // Image path selection
            PopulateExistingImages(); // Loads existing product image options back into the form.
            // Availability check
            return View(new Products { Available = true }); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Producer,Developer")] // Restricts access to users in the Producer,Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        public async Task<IActionResult> Create([Bind("ItemName,ItemPrice,ImagePath,QuantityInStock,Available,Category,Description,Allergens")] Products products, IFormFile? imageUpload) // Shows or processes the create form for a record
        {
            // Producer lookup
            var producer = await GetCurrentProducerAsync(); // Sets producer to the value needed by the workflow.
            if (producer == null) // Checks whether the requested data was found.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Form validation
            ModelState.Remove(nameof(Products.Producers)); // Removes a field from validation because the controller supplies it.
            products.ProducersId = producer.ProducersId; // Sets products.ProducersId to the value needed by the workflow.
            // Image path selection
            products.ImagePath = NormalizeImagePath(products.ImagePath); // Normalizes form input before validation or saving.
            products.ItemName = products.ItemName?.Trim() ?? string.Empty; // Sets products.ItemName to the value needed by the workflow.
            // Filter logic
            products.Category = products.Category?.Trim() ?? string.Empty; // Sets products.Category to the value needed by the workflow.
            products.Description = products.Description?.Trim() ?? string.Empty; // Sets products.Description to the value needed by the workflow.
            // Allergen information system
            products.Allergens = NormalizeAllergens(products.Allergens); // Normalizes form input before validation or saving.

            // Form validation
            ValidateProduct(products); // Runs validation logic and records any model errors.
            ValidateImageUpload(imageUpload); // Runs validation logic and records any model errors.

            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Image path selection
                PopulateExistingImages(); // Loads existing product image options back into the form.
                return View(products); // Renders the matching view with the supplied model data.
            }

            // Image upload
            if (imageUpload != null) // Runs the next step only when the record exists.
            {
                products.ImagePath = await SaveProductImageAsync(imageUpload); // Sets products.ImagePath to the value needed by the workflow.
            }

            // Product lookup
            _context.Products.Add(products); // Queues the new entity to be inserted into the database.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Producer,Developer")] // Restricts access to users in the Producer,Developer role set.

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer lookup
            var producer = await GetCurrentProducerAsync(); // Sets producer to the value needed by the workflow.
            if (producer == null) // Checks whether the requested data was found.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Product lookup
            var products = await _context.Products // Sets products to the value needed by the workflow.
                .FirstOrDefaultAsync(p => p.ProductsId == id && p.ProducersId == producer.ProducersId); // Fetches the first matching record or null if none exists.

            if (products == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Image path selection
            PopulateExistingImages(); // Loads existing product image options back into the form.
            return View(products); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Producer,Developer")] // Restricts access to users in the Producer,Developer role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Product lookup
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,ItemName,ItemPrice,ImagePath,QuantityInStock,Available,Category,Description,Allergens")] Products products, IFormFile? imageUpload, string? existingImagePath) // Shows or processes edits for an existing record
        {
            if (id != products.ProductsId) // Checks the condition that controls the next action.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer lookup
            var producer = await GetCurrentProducerAsync(); // Sets producer to the value needed by the workflow.
            if (producer == null) // Checks whether the requested data was found.
            {
                return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
            }

            // Product lookup
            var existingProduct = await _context.Products // Sets existingProduct to the value needed by the workflow.
                .FirstOrDefaultAsync(p => p.ProductsId == id && p.ProducersId == producer.ProducersId); // Fetches the first matching record or null if none exists.

            // Edit record
            if (existingProduct == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Form validation
            ModelState.Remove(nameof(Products.Producers)); // Removes a field from validation because the controller supplies it.
            // Producer lookup
            products.ProducersId = producer.ProducersId; // Sets products.ProducersId to the value needed by the workflow.
            // Image path selection
            products.ImagePath = NormalizeImagePath(products.ImagePath); // Normalizes form input before validation or saving.
            products.ItemName = products.ItemName?.Trim() ?? string.Empty; // Sets products.ItemName to the value needed by the workflow.
            // Filter logic
            products.Category = products.Category?.Trim() ?? string.Empty; // Sets products.Category to the value needed by the workflow.
            products.Description = products.Description?.Trim() ?? string.Empty; // Sets products.Description to the value needed by the workflow.
            // Allergen information system
            products.Allergens = NormalizeAllergens(products.Allergens); // Normalizes form input before validation or saving.

            // Form validation
            ValidateProduct(products); // Runs validation logic and records any model errors.
            ValidateImageUpload(imageUpload); // Runs validation logic and records any model errors.

            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Image path selection
                if (string.IsNullOrWhiteSpace(products.ImagePath)) // Validates that required text was supplied.
                {
                    products.ImagePath = existingProduct.ImagePath ?? existingImagePath; // Sets products.ImagePath to the value needed by the workflow.
                }

                PopulateExistingImages(); // Loads existing product image options back into the form.
                return View(products); // Renders the matching view with the supplied model data.
            }

            // Edit record
            existingProduct.ItemName = products.ItemName; // Sets existingProduct.ItemName to the value needed by the workflow.
            existingProduct.ItemPrice = products.ItemPrice; // Sets existingProduct.ItemPrice to the value needed by the workflow.
            // Stock checks
            existingProduct.QuantityInStock = products.QuantityInStock; // Sets existingProduct.QuantityInStock to the value needed by the workflow.
            // Availability check
            existingProduct.Available = products.Available; // Sets existingProduct.Available to the value needed by the workflow.
            // Filter logic
            existingProduct.Category = products.Category; // Sets existingProduct.Category to the value needed by the workflow.
            // Edit record
            existingProduct.Description = products.Description; // Sets existingProduct.Description to the value needed by the workflow.
            // Allergen information system
            existingProduct.Allergens = products.Allergens; // Sets existingProduct.Allergens to the value needed by the workflow.

            // Image upload
            if (imageUpload != null) // Runs the next step only when the record exists.
            {
                existingProduct.ImagePath = await SaveProductImageAsync(imageUpload); // Sets existingProduct.ImagePath to the value needed by the workflow.
            }
            // Image path selection
            else if (!string.IsNullOrWhiteSpace(products.ImagePath)) // Checks the next fallback condition in the workflow.
            {
                existingProduct.ImagePath = products.ImagePath; // Sets existingProduct.ImagePath to the value needed by the workflow.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Producer,Developer,Admin")] // Restricts access to users in the Producer,Developer,Admin role set.

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            var productsQuery = _context.Products.Include(p => p.Producers).AsQueryable(); // Starts a product query with producer details.
            if (!User.IsInRole("Admin") && !User.IsInRole("Developer")) // Keeps producers scoped to their own products.
            {
                // Producer lookup
                var producer = await GetCurrentProducerAsync(); // Sets producer to the value needed by the workflow.
                if (producer == null) // Checks whether the requested data was found.
                {
                    return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
                }

                productsQuery = productsQuery.Where(m => m.ProducersId == producer.ProducersId); // Filters the query to the signed-in producer's products.
            }

            // Product lookup
            var products = await productsQuery.FirstOrDefaultAsync(m => m.ProductsId == id); // Fetches the first matching record or null if none exists.

            if (products == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            return View(products); // Renders the matching view with the supplied model data.
        }

        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Role management
        [Authorize(Roles = "Producer,Developer,Admin")] // Restricts access to users in the Producer,Developer,Admin role set.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            var productsQuery = _context.Products.AsQueryable(); // Starts a product query for delete confirmation.
            if (!User.IsInRole("Admin") && !User.IsInRole("Developer")) // Keeps producers scoped to their own products.
            {
                // Producer lookup
                var producer = await GetCurrentProducerAsync(); // Sets producer to the value needed by the workflow.
                if (producer == null) // Checks whether the requested data was found.
                {
                    return Unauthorized(); // Returns 401 when the current user is not allowed to continue.
                }

                productsQuery = productsQuery.Where(p => p.ProducersId == producer.ProducersId); // Filters the query to the signed-in producer's products.
            }

            // Product lookup
            var products = await productsQuery.FirstOrDefaultAsync(p => p.ProductsId == id); // Fetches the first matching record or null if none exists.

            if (products != null) // Runs the next step only when the record exists.
            {
                _context.Products.Remove(products); // Queues the entity for removal from the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Producer Lookup Helpers -----
        private async Task<Producers?> GetCurrentProducerAsync() // Finds the producer record attached to the signed-in user
        {
            // Set user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current signed-in user's Identity id from claims.
            if (string.IsNullOrWhiteSpace(userId)) // Checks that a signed-in user id is available.
            {
                return null; // Returns null because no usable value is available.
            }

            // Producer lookup
            return await _context.Producers.FirstOrDefaultAsync(p => p.UserId == userId); // Returns the computed result to the caller.
        }

        // ----- Product Image Helpers -----
        private void PopulateExistingImages() // Loads existing product image paths for the form dropdown
        {
            var imagesRoot = Path.Combine(_environment.WebRootPath, "images"); // Builds a file-system path from safe path segments.
            // Image path selection
            var imagePaths = Directory.Exists(imagesRoot) // Checks whether the image folder exists before listing files.
                // Search logic
                ? Directory.EnumerateFiles(imagesRoot, "*.*", SearchOption.AllDirectories) // Lists files only when the image directory exists.
                    // Image upload
                    .Where(path => AllowedImageExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) // Lists the image extensions accepted by upload validation.
                    .Select(path => "/" + Path.GetRelativePath(_environment.WebRootPath, path).Replace('\\', '/')) // Projects the query results into the shape needed next.
                    // Sort logic
                    .OrderBy(path => path) // Sorts the query results for display or processing.
                    .ToList() // Materializes the sequence into a list.
                : []; // Uses an empty image path list when the image folder is missing.

            // Image path selection
            ViewBag.ExistingImages = new SelectList(imagePaths); // Supplies ExistingImages to the view for display or form state.
        }

        // ----- Product Validation Helpers -----
        // Form validation
        private void ValidateProduct(Products products) // Adds validation errors for invalid product form fields
        {
            if (string.IsNullOrWhiteSpace(products.ItemName)) // Validates that required text was supplied.
            {
                ModelState.AddModelError(nameof(products.ItemName), "Enter a product name."); // Adds a validation message that the view can show to the user.
            }

            if (products.ItemPrice <= 0) // Checks the condition that controls the next action.
            {
                // Form validation
                ModelState.AddModelError(nameof(products.ItemPrice), "Price must be greater than 0."); // Adds a validation message that the view can show to the user.
            }

            // Stock checks
            if (products.QuantityInStock < 0) // Checks the condition that controls the next action.
            {
                ModelState.AddModelError(nameof(products.QuantityInStock), "Stock quantity cannot be negative."); // Adds a validation message that the view can show to the user.
            }

            // Filter logic
            if (string.IsNullOrWhiteSpace(products.Category)) // Validates that required text was supplied.
            {
                // Form validation
                ModelState.AddModelError(nameof(products.Category), "Select a category."); // Adds a validation message that the view can show to the user.
            }

            if (string.IsNullOrWhiteSpace(products.Description)) // Validates that required text was supplied.
            {
                ModelState.AddModelError(nameof(products.Description), "Enter a description."); // Adds a validation message that the view can show to the user.
            }

            // Image path selection
            if (!string.IsNullOrWhiteSpace(products.ImagePath) // Validates that required text was supplied.
                && !products.ImagePath.StartsWith("/") // Allows relative web paths that start from the site root.
                && !Uri.IsWellFormedUriString(products.ImagePath, UriKind.Absolute)) // Allows full external image URLs when they are well formed.
            {
                // Form validation
                ModelState.AddModelError(nameof(products.ImagePath), "Use a relative web path like /images/products/example.jpg or a full URL."); // Adds a validation message that the view can show to the user.
            }
        }

        // ----- Image Upload Helpers -----
        private void ValidateImageUpload(IFormFile? imageUpload) // Validates uploaded image type and size before saving
        {
            // Image upload
            if (imageUpload == null || imageUpload.Length == 0) // Checks whether the requested data was found.
            {
                return; // Leaves the helper early because there is nothing to validate.
            }

            var extension = Path.GetExtension(imageUpload.FileName).ToLowerInvariant(); // Reads the file extension for upload validation or naming.
            if (!AllowedImageExtensions.Contains(extension)) // Lists the image extensions accepted by upload validation.
            {
                // Form validation
                ModelState.AddModelError("imageUpload", "Upload a JPG, PNG, or WebP image."); // Adds a validation message that the view can show to the user.
            }

            // Image upload
            if (imageUpload.Length > MaxImageUploadBytes) // Sets the maximum accepted image upload size to five megabytes.
            {
                ModelState.AddModelError("imageUpload", "Image files must be smaller than 5 MB."); // Adds a validation message that the view can show to the user.
            }
        }

        private async Task<string> SaveProductImageAsync(IFormFile imageUpload) // Saves an uploaded product image and returns its public path
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products"); // Builds a file-system path from safe path segments.
            Directory.CreateDirectory(uploadsFolder); // Ensures the upload folder exists before writing the file.

            // Image upload
            var extension = Path.GetExtension(imageUpload.FileName).ToLowerInvariant(); // Reads the file extension for upload validation or naming.
            var fileName = $"product-{Guid.NewGuid():N}{extension}"; // Creates a unique file name to avoid upload collisions.
            var filePath = Path.Combine(uploadsFolder, fileName); // Builds a file-system path from safe path segments.

            await using var stream = new FileStream(filePath, FileMode.Create); // Opens the destination file stream for the upload.
            await imageUpload.CopyToAsync(stream); // Copies the uploaded image into the destination stream.

            return $"/images/products/{fileName}"; // Returns the computed result to the caller.
        }

        private static string? NormalizeImagePath(string? imagePath) // Turns an empty image path into null or trims a supplied path
        {
            // Image path selection
            if (string.IsNullOrWhiteSpace(imagePath)) // Validates that required text was supplied.
            {
                return null; // Returns null because no usable value is available.
            }

            return imagePath.Trim(); // Returns the computed result to the caller.
        }

        // ----- Product Validation Helpers -----
        private static string? NormalizeAllergens(string? allergens) // Normalizes comma-separated allergen text and removes duplicates
        {
            // Allergen information system
            if (string.IsNullOrWhiteSpace(allergens)) // Validates that required text was supplied.
            {
                return null; // Returns null because no usable value is available.
            }

            var normalized = allergens // Sets normalized to the value needed by the workflow.
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) // Continues the chained query or expression from the previous line.
                .Distinct(StringComparer.OrdinalIgnoreCase); // Continues the chained query or expression from the previous line.

            return string.Join(", ", normalized); // Returns the computed result to the caller.
        }

        // ----- Existence Helpers -----
        private bool ProductsExists(int id) // Checks whether a product record still exists
        {
            // Product lookup
            return _context.Products.Any(e => e.ProductsId == id); // Returns the computed result to the caller.
        }
    }
}
