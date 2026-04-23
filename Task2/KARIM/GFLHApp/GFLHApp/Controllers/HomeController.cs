// ----- Imports -----
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.
using Microsoft.AspNetCore.Identity; // Provides Identity user and role management services.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.
using System.Diagnostics; // Provides diagnostics information used by the error page.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{

    // ----- Controller Declaration -----
    public class HomeController : Controller // Defines the MVC controller for site landing, privacy, and error pages.
    {

        // ----- Controller Dependencies -----
        private readonly ILogger<HomeController> _logger; // Holds the injected logger for this controller.
        private readonly ApplicationDbContext _context; // Holds the injected database context for page data queries.
        private readonly UserManager<IdentityUser> _userManager; // Holds the Identity user manager used to read the signed-in account email.

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager) // Holds the injected logger for this controller.
        {
            _logger = logger; // Stores the injected dependency on the controller field.
            _context = context; // Stores the injected dependency on the controller field.
            _userManager = userManager; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            var model = new HomeIndexViewModel
            {
                GrowerCount = await _context.Producers.CountAsync(),
                AvailableProductCount = await _context.Products.CountAsync(p => p.Available),
                OrderCount = await _context.Orders.CountAsync(),
                CategoryCount = await _context.Products
                    .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                    .Select(p => p.Category)
                    .Distinct()
                    .CountAsync()
            };

            return View(model); // Renders the matching view with the supplied model data.
        }

        // ----- Static Page Actions -----
        public IActionResult Privacy() // Loads the privacy page
        {
            return View(); // Renders the matching view with the supplied model data.
        }

        public async Task<IActionResult> Contact() // Loads the contact page
        {
            return View(await BuildContactPageViewModelAsync()); // Renders the matching view with the supplied model data.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactPageViewModel model) // Saves a submitted contact inquiry
        {
            var signedInEmail = await GetSignedInEmailAsync(); // Reads the signed-in user's email so the inquiry stays linked to their account.
            var finalEmail = string.IsNullOrWhiteSpace(signedInEmail)
                ? (model.EmailAddress ?? string.Empty).Trim()
                : signedInEmail;

            var inquiry = new ContactInquiry
            {
                FullName = (model.FullName ?? string.Empty).Trim(),
                EmailAddress = finalEmail,
                Subject = (model.Subject ?? string.Empty).Trim(),
                Message = (model.Message ?? string.Empty).Trim(),
                SubmittedAtUtc = DateTime.UtcNow,
                IsRead = false
            };

            _context.ContactInquiries.Add(inquiry); // Persists the inquiry so admins can review it later.
            await _context.SaveChangesAsync(); // Saves the newly submitted inquiry.

            TempData["ContactSuccess"] = "Thanks for reaching out. Your message has been sent to the Greenfield team."; // Stores a one-request success message for the contact page.
            return RedirectToAction(nameof(Contact)); // Uses PRG so refreshing the page does not re-submit the form.
        }

        public async Task<IActionResult> About() // Loads the about page
        {
            var model = new AboutViewModel
            {
                GrowerCount = await _context.Producers.CountAsync(),
                AvailableProductCount = await _context.Products.CountAsync(p => p.Available),
                OrderCount = await _context.Orders.CountAsync()
            };

            return View(model); // Renders the matching view with the supplied model data.
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disables response caching for the error response.

        // ----- Error Actions -----
        public IActionResult Error() // Loads the error page with the current request identifier
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Renders the matching view with the supplied model data.
        }

        private async Task<ContactPageViewModel> BuildContactPageViewModelAsync() // Builds the contact page model with any signed-in user context and replies.
        {
            var model = new ContactPageViewModel(); // Creates the page model used by the contact form and conversation history.
            var signedInEmail = await GetSignedInEmailAsync(); // Gets the current account email when the visitor is signed in.

            if (string.IsNullOrWhiteSpace(signedInEmail))
            {
                return model; // Leaves the page in guest mode when no account email is available.
            }

            model.IsSignedIn = true; // Tells the view the visitor has an account email we can reuse.
            model.CurrentUserEmail = signedInEmail; // Supplies the account email to the view for display.
            model.EmailAddress = signedInEmail; // Prefills the contact form email field automatically.
            model.MyInquiries = await _context.ContactInquiries
                .Where(i => i.EmailAddress.ToLower() == signedInEmail.ToLower())
                .OrderByDescending(i => i.SubmittedAtUtc)
                .ToListAsync(); // Loads the signed-in user's previous inquiries and any admin replies.

            return model; // Returns the contact page model with conversation data attached.
        }

        private async Task<string> GetSignedInEmailAsync() // Reads the current signed-in user's email if one exists.
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return string.Empty; // Returns an empty value when the visitor is not signed in.
            }

            var user = await _userManager.GetUserAsync(User); // Loads the current Identity user record.
            return user?.Email?.Trim() ?? string.Empty; // Returns the account email used for contact reply matching.
        }
    }
}
