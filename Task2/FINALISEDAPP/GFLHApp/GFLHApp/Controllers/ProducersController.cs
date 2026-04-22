// ----- Imports -----
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.
using Microsoft.AspNetCore.Hosting; // Provides web root path information for file uploads.
using Microsoft.AspNetCore.Identity; // Provides Identity user and role management services.
using Microsoft.AspNetCore.Http; // Provides uploaded file abstractions.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.AspNetCore.Mvc.Rendering; // Provides SelectList helpers for form dropdowns.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.
using System; // Provides core .NET types used by this controller.
using System.Collections.Generic; // Provides collection types such as lists and dictionaries.
using System.IO; // Provides file and directory APIs for uploaded images.
using System.Linq; // Provides LINQ filtering, grouping, and sorting helpers.
using System.Threading.Tasks; // Provides asynchronous Task return types.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{

    // ----- Controller Declaration -----
    public class ProducersController : Controller // Defines the MVC controller for producer profiles, account creation, VAT details, and producer images.
    {

        // ----- Controller Dependencies -----
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"]; // Lists the image extensions accepted by upload validation.
        private const long MaxImageUploadBytes = 5 * 1024 * 1024; // Sets the maximum accepted image upload size to five megabytes.

        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.
        // Role management
        private readonly UserManager<IdentityUser> _userManager; // Holds the Identity user manager used to find and update users.
        private readonly RoleManager<IdentityRole> _roleManager; // Holds the Identity role manager used to inspect and create roles.
        private readonly IWebHostEnvironment _environment; // Holds web-host paths used when listing or saving image files.

        public ProducersController(ApplicationDbContext context, // Receives data, Identity, and hosting services from dependency injection.
                                   UserManager<IdentityUser> userManager, // Accepts the Identity user manager from dependency injection.
                                   RoleManager<IdentityRole> roleManager, // Holds the Identity role manager used to inspect and create roles.
                                   IWebHostEnvironment environment) // Accepts hosting environment paths from dependency injection.
        {
            _context = context; // Stores the injected dependency on the controller field.
            _userManager = userManager; // Stores the injected dependency on the controller field.
            _roleManager = roleManager; // Stores the injected dependency on the controller field.
            _environment = environment; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            // Producer lookup
            return View(await _context.Producers.ToListAsync()); // Renders the matching view with the supplied model data.
        }

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {
            if (id == null) return NotFound(); // Checks that the request included a record id.

            var producers = await _context.Producers // Sets producers to the value needed by the workflow.
                .Include(p => p.Products) // Includes related records needed by the view or workflow.
                .FirstOrDefaultAsync(m => m.ProducersId == id); // Fetches the first matching record or null if none exists.

            if (producers == null) return NotFound(); // Checks whether the requested data was found.

            return View(producers); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.

        // ----- Create Actions -----
        // Create record
        public IActionResult Create() // Shows or processes the create form for a record
        {
            // Image path selection
            PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
            return View(); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.
        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.

        // Create record
        public async Task<IActionResult> Create( // Shows or processes the create form for a record
            // Invoice VAT system
            [Bind("ProducersId,ProducerName,ProducerEmail,ProducerInformation,ImagePath,IsVATRegistered,VATNumber")] // Limits model binding to the listed form fields.
            Producers producers, // Receives the producer form values submitted by the user.
            // Image upload
            IFormFile? imageUpload, // Receives an optional uploaded image from the form.
            string AccountPassword) // Receives the password used to create the producer's Identity account.
        {

            // Form validation
            ModelState.Remove("UserId"); // Removes a field from validation because the controller supplies it.
            ModelState.Remove("ProducerOrders"); // Removes a field from validation because the controller supplies it.
            NormalizeProducer(producers); // Normalizes form input before validation or saving.
            ValidateProducerFields(producers); // Runs validation logic and records any model errors.
            ValidateImageUpload(imageUpload); // Runs validation logic and records any model errors.

            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                foreach (var kvp in ModelState) // Iterates through each matching item to process it.
                    foreach (var err in kvp.Value.Errors) // Iterates through each matching item to process it.
                        System.Diagnostics.Debug.WriteLine($"{kvp.Key}: {err.ErrorMessage}"); // Writes validation details to debug output for troubleshooting.
                // Image path selection
                PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
                return View(producers); // Renders the matching view with the supplied model data.
            }

            // Producer lookup
            bool emailInUse = await _context.Producers // Checks whether another producer already uses this email address.
                .AnyAsync(p => p.ProducerEmail.ToLower() == producers.ProducerEmail.ToLower()); // Continues the chained query or expression from the previous line.
            if (emailInUse) // Checks the condition that controls the next action.
                // Form validation
                ModelState.AddModelError("ProducerEmail", "This email address is already registered to a producer."); // Adds a validation message that the view can show to the user.

            bool nameInUse = await _context.Producers // Checks whether another producer already uses this producer name.
                .AnyAsync(p => p.ProducerName.ToLower() == producers.ProducerName.ToLower()); // Continues the chained query or expression from the previous line.
            if (nameInUse) // Checks the condition that controls the next action.
                ModelState.AddModelError("ProducerName", "This producer name is already taken."); // Adds a validation message that the view can show to the user.

            // Invoice VAT system
            if (producers.IsVATRegistered && !string.IsNullOrWhiteSpace(producers.VATNumber)) // Validates that required text was supplied.
            {
                bool vatInUse = await _context.Producers // Checks whether another producer already uses this VAT number.
                    .AnyAsync(p => p.VATNumber == producers.VATNumber); // Continues the chained query or expression from the previous line.
                if (vatInUse) // Checks the condition that controls the next action.
                    // Form validation
                    ModelState.AddModelError("VATNumber", "This VAT number is already registered to another producer."); // Adds a validation message that the view can show to the user.
            }

            if (string.IsNullOrWhiteSpace(AccountPassword)) // Validates that required text was supplied.
                ModelState.AddModelError("AccountPassword", "A password is required."); // Adds a validation message that the view can show to the user.

            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Image path selection
                PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
                return View(producers); // Renders the matching view with the supplied model data.
            }

            var identityUser = new IdentityUser // Creates the identityUser object used by the following workflow.
            {
                // Producer lookup
                UserName = producers.ProducerEmail, // Sets UserName to the value needed by the workflow.
                Email = producers.ProducerEmail, // Sets Email to the value needed by the workflow.
                EmailConfirmed = true // Sets EmailConfirmed to the value needed by the workflow.
            };

            var createResult = await _userManager.CreateAsync(identityUser, AccountPassword); // Sets createResult to the value needed by the workflow.

            if (!createResult.Succeeded) // Checks the condition that controls the next action.
            {
                foreach (var error in createResult.Errors) // Iterates through each matching item to process it.
                    // Form validation
                    ModelState.AddModelError("AccountPassword", error.Description); // Adds a validation message that the view can show to the user.
                // Image path selection
                PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
                return View(producers); // Renders the matching view with the supplied model data.
            }

            // Role management
            if (!await _roleManager.RoleExistsAsync("Producer")) // Checks whether the role needs to be created first.
                await _roleManager.CreateAsync(new IdentityRole("Producer")); // Creates the missing Identity role before assigning or listing it.

            await _userManager.AddToRoleAsync(identityUser, "Producer"); // Assigns the new producer account to the Producer role.

            producers.UserId = identityUser.Id; // Sets producers.UserId to the value needed by the workflow.
            // Image upload
            if (imageUpload != null) // Runs the next step only when the record exists.
            {
                producers.ImagePath = await SaveProducerImageAsync(imageUpload); // Sets producers.ImagePath to the value needed by the workflow.
            }

            // Create record
            _context.Add(producers); // Queues the new entity to be inserted into the database.
            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.

            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) return NotFound(); // Checks that the request included a record id.

            // Producer lookup
            var producers = await _context.Producers.FindAsync(id); // Looks up the record by its primary key.
            if (producers == null) return NotFound(); // Checks whether the requested data was found.

            // Image path selection
            PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
            return View(producers); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.
        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Edit record
        public async Task<IActionResult> Edit(int id, // Shows or processes edits for an existing record
            // Invoice VAT system
            [Bind("ProducersId,UserId,ProducerName,ProducerEmail,ProducerInformation,ImagePath,IsVATRegistered,VATNumber")] // Limits model binding to the listed form fields.
            Producers producers, // Receives the producer form values submitted by the user.
            // Image upload
            IFormFile? imageUpload, // Receives an optional uploaded image from the form.
            // Image path selection
            string? existingImagePath) // Receives the previously selected image path when no new file is uploaded.
        {
            // Producer lookup
            if (id != producers.ProducersId) return NotFound(); // Checks the condition that controls the next action.

            var existingProducer = await _context.Producers.FirstOrDefaultAsync(p => p.ProducersId == id); // Fetches the first matching record or null if none exists.
            // Edit record
            if (existingProducer == null) return NotFound(); // Checks whether the requested data was found.

            NormalizeProducer(producers); // Normalizes form input before validation or saving.
            // Form validation
            ValidateProducerFields(producers); // Runs validation logic and records any model errors.
            ValidateImageUpload(imageUpload); // Runs validation logic and records any model errors.

            // Producer lookup
            bool emailInUse = await _context.Producers // Checks whether another producer already uses this email address.
                .AnyAsync(p => p.ProducerEmail.ToLower() == producers.ProducerEmail.ToLower() // Continues the chained query or expression from the previous line.
                            && p.ProducersId != id); // Excludes the current producer record from the uniqueness check.
            if (emailInUse) // Checks the condition that controls the next action.
                ModelState.AddModelError("ProducerEmail", "This email address is already registered to another producer."); // Adds a validation message that the view can show to the user.

            bool nameInUse = await _context.Producers // Checks whether another producer already uses this producer name.
                .AnyAsync(p => p.ProducerName.ToLower() == producers.ProducerName.ToLower() // Continues the chained query or expression from the previous line.
                            && p.ProducersId != id); // Excludes the current producer record from the uniqueness check.
            if (nameInUse) // Checks the condition that controls the next action.
                // Form validation
                ModelState.AddModelError("ProducerName", "This producer name is already taken."); // Adds a validation message that the view can show to the user.

            // Invoice VAT system
            if (producers.IsVATRegistered && !string.IsNullOrWhiteSpace(producers.VATNumber)) // Validates that required text was supplied.
            {
                bool vatInUse = await _context.Producers // Checks whether another producer already uses this VAT number.
                    .AnyAsync(p => p.VATNumber == producers.VATNumber // Continues the chained query or expression from the previous line.
                               // Producer lookup
                               && p.ProducersId != id); // Excludes the current producer record from the uniqueness check.
                if (vatInUse) // Checks the condition that controls the next action.
                    // Form validation
                    ModelState.AddModelError("VATNumber", "This VAT number is already registered to another producer."); // Adds a validation message that the view can show to the user.
            }

            if (!ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Image path selection
                if (string.IsNullOrWhiteSpace(producers.ImagePath)) // Validates that required text was supplied.
                {
                    producers.ImagePath = existingProducer.ImagePath ?? existingImagePath; // Sets producers.ImagePath to the value needed by the workflow.
                }

                PopulateExistingProducerImages(); // Loads existing producer image options back into the form.
                return View(producers); // Renders the matching view with the supplied model data.
            }

            try // Starts a protected database update block.
            {
                // Edit record
                existingProducer.UserId = producers.UserId; // Sets existingProducer.UserId to the value needed by the workflow.
                // Producer lookup
                existingProducer.ProducerName = producers.ProducerName; // Sets existingProducer.ProducerName to the value needed by the workflow.
                existingProducer.ProducerEmail = producers.ProducerEmail; // Sets existingProducer.ProducerEmail to the value needed by the workflow.
                existingProducer.ProducerInformation = producers.ProducerInformation; // Sets existingProducer.ProducerInformation to the value needed by the workflow.
                // Invoice VAT system
                existingProducer.IsVATRegistered = producers.IsVATRegistered; // Sets existingProducer.IsVATRegistered to the value needed by the workflow.
                existingProducer.VATNumber = producers.IsVATRegistered ? producers.VATNumber : null; // Sets existingProducer.VATNumber to the value needed by the workflow.

                // Image upload
                if (imageUpload != null) // Runs the next step only when the record exists.
                {
                    existingProducer.ImagePath = await SaveProducerImageAsync(imageUpload); // Sets existingProducer.ImagePath to the value needed by the workflow.
                }
                // Image path selection
                else if (!string.IsNullOrWhiteSpace(producers.ImagePath)) // Checks the next fallback condition in the workflow.
                {
                    existingProducer.ImagePath = producers.ImagePath; // Sets existingProducer.ImagePath to the value needed by the workflow.
                }

                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            }
            catch (DbUpdateConcurrencyException) // Handles database concurrency errors from the update attempt.
            {
                // Producer lookup
                if (!ProducersExists(producers.ProducersId)) return NotFound(); // Checks the condition that controls the next action.
                else throw; // Handles the fallback branch when earlier conditions did not match.
            }

            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) return NotFound(); // Checks that the request included a record id.

            // Producer lookup
            var producers = await _context.Producers // Sets producers to the value needed by the workflow.
                .FirstOrDefaultAsync(m => m.ProducersId == id); // Fetches the first matching record or null if none exists.

            if (producers == null) return NotFound(); // Checks whether the requested data was found.

            return View(producers); // Renders the matching view with the supplied model data.
        }

        // Role management
        [Authorize(Roles = "Developer,Admin")] // Restricts access to users in the Developer,Admin role set.
        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            // Producer lookup
            var producers = await _context.Producers.FindAsync(id); // Looks up the record by its primary key.
            if (producers != null) // Runs the next step only when the record exists.
                _context.Producers.Remove(producers); // Queues the entity for removal from the database.

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Existence Helpers -----
        private bool ProducersExists(int id) // Checks whether a producer record still exists
        {
            // Producer lookup
            return _context.Producers.Any(e => e.ProducersId == id); // Returns the computed result to the caller.
        }

        // ----- Producer Image Helpers -----
        private void PopulateExistingProducerImages() // Loads existing producer image paths for the form dropdown
        {
            var imagesRoot = Path.Combine(_environment.WebRootPath, "images", "producers"); // Builds a file-system path from safe path segments.
            // Image path selection
            var imagePaths = Directory.Exists(imagesRoot) // Checks whether the image folder exists before listing files.
                // Search logic
                ? Directory.EnumerateFiles(imagesRoot, "*.*", SearchOption.TopDirectoryOnly) // Lists files only when the image directory exists.
                    // Image upload
                    .Where(path => AllowedImageExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) // Lists the image extensions accepted by upload validation.
                    .Select(path => "/images/producers/" + Path.GetFileName(path)) // Projects the query results into the shape needed next.
                    // Sort logic
                    .OrderBy(path => path) // Sorts the query results for display or processing.
                    .ToList() // Materializes the sequence into a list.
                : []; // Uses an empty image path list when the image folder is missing.

            // Image path selection
            ViewBag.ExistingImages = new SelectList(imagePaths); // Supplies ExistingImages to the view for display or form state.
        }

        // ----- Producer Validation Helpers -----
        private void NormalizeProducer(Producers producers) // Trims and normalizes producer form input before validation
        {
            // Producer lookup
            producers.ProducerName = producers.ProducerName?.Trim() ?? string.Empty; // Sets producers.ProducerName to the value needed by the workflow.
            producers.ProducerEmail = producers.ProducerEmail?.Trim() ?? string.Empty; // Sets producers.ProducerEmail to the value needed by the workflow.
            producers.ProducerInformation = producers.ProducerInformation?.Trim() ?? string.Empty; // Sets producers.ProducerInformation to the value needed by the workflow.
            // Image path selection
            producers.ImagePath = NormalizeImagePath(producers.ImagePath); // Normalizes form input before validation or saving.
            // Invoice VAT system
            producers.VATNumber = producers.IsVATRegistered // Sets producers.VATNumber to the value needed by the workflow.
                ? producers.VATNumber?.Trim().ToUpperInvariant() // Uses the value when the preceding condition is true.
                : null; // Clears the VAT number when the producer is not VAT registered.
        }

        // Form validation
        private void ValidateProducerFields(Producers producers) // Adds validation errors for invalid producer form fields
        {
            // Producer lookup
            if (string.IsNullOrWhiteSpace(producers.ProducerName)) // Validates that required text was supplied.
            {
                ModelState.AddModelError(nameof(producers.ProducerName), "Producer name is required."); // Adds a validation message that the view can show to the user.
            }
            else if (producers.ProducerName.Length < 3 || producers.ProducerName.Length > 100) // Checks the next fallback condition in the workflow.
            {
                // Form validation
                ModelState.AddModelError(nameof(producers.ProducerName), "Producer name must be between 3 and 100 characters."); // Adds a validation message that the view can show to the user.
            }

            // Producer lookup
            if (string.IsNullOrWhiteSpace(producers.ProducerEmail)) // Validates that required text was supplied.
            {
                ModelState.AddModelError(nameof(producers.ProducerEmail), "Email address is required."); // Adds a validation message that the view can show to the user.
            }
            else if (producers.ProducerEmail.Length > 150 || !producers.ProducerEmail.Contains('@')) // Checks the next fallback condition in the workflow.
            {
                // Form validation
                ModelState.AddModelError(nameof(producers.ProducerEmail), "Enter a valid email address."); // Adds a validation message that the view can show to the user.
            }

            if (string.IsNullOrWhiteSpace(producers.ProducerInformation)) // Validates that required text was supplied.
            {
                ModelState.AddModelError(nameof(producers.ProducerInformation), "Producer information is required."); // Adds a validation message that the view can show to the user.
            }
            else if (producers.ProducerInformation.Length < 10 || producers.ProducerInformation.Length > 500) // Checks the next fallback condition in the workflow.
            {
                // Form validation
                ModelState.AddModelError(nameof(producers.ProducerInformation), "Producer information must be between 10 and 500 characters."); // Adds a validation message that the view can show to the user.
            }

            // Invoice VAT system
            if (producers.IsVATRegistered) // Checks the condition that controls the next action.
            {
                if (string.IsNullOrWhiteSpace(producers.VATNumber)) // Validates that required text was supplied.
                {
                    ModelState.AddModelError(nameof(producers.VATNumber), "VAT number is required if VAT registered."); // Adds a validation message that the view can show to the user.
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(producers.VATNumber, "^GB[0-9]{9}$")) // Checks the next fallback condition in the workflow.
                {
                    // Form validation
                    ModelState.AddModelError(nameof(producers.VATNumber), "VAT number must start with GB followed by exactly 9 digits."); // Adds a validation message that the view can show to the user.
                }
            }

            // Image path selection
            if (!string.IsNullOrWhiteSpace(producers.ImagePath) // Validates that required text was supplied.
                && !producers.ImagePath.StartsWith("/") // Allows relative web paths that start from the site root.
                && !Uri.IsWellFormedUriString(producers.ImagePath, UriKind.Absolute)) // Allows full external image URLs when they are well formed.
            {
                // Form validation
                ModelState.AddModelError(nameof(producers.ImagePath), "Use a relative web path like /images/producers/example.jpg or a full URL."); // Adds a validation message that the view can show to the user.
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

        private async Task<string> SaveProducerImageAsync(IFormFile imageUpload) // Saves an uploaded producer image and returns its public path
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "producers"); // Builds a file-system path from safe path segments.
            Directory.CreateDirectory(uploadsFolder); // Ensures the upload folder exists before writing the file.

            // Image upload
            var extension = Path.GetExtension(imageUpload.FileName).ToLowerInvariant(); // Reads the file extension for upload validation or naming.
            var fileName = $"producer-{Guid.NewGuid():N}{extension}"; // Creates a unique file name to avoid upload collisions.
            var filePath = Path.Combine(uploadsFolder, fileName); // Builds a file-system path from safe path segments.

            await using var stream = new FileStream(filePath, FileMode.Create); // Opens the destination file stream for the upload.
            await imageUpload.CopyToAsync(stream); // Copies the uploaded image into the destination stream.

            return $"/images/producers/{fileName}"; // Returns the computed result to the caller.
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
    }
}
