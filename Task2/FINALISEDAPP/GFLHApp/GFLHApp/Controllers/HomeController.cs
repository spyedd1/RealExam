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

        public HomeController(ILogger<HomeController> logger) // Holds the injected logger for this controller.
        {
            _logger = logger; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public IActionResult Index() // Loads the main listing or dashboard view for this controller
        {
            return View(); // Renders the matching view with the supplied model data.
        }

        // ----- Static Page Actions -----
        public IActionResult Privacy() // Loads the privacy page
        {
            return View(); // Renders the matching view with the supplied model data.
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disables response caching for the error response.

        // ----- Error Actions -----
        public IActionResult Error() // Loads the error page with the current request identifier
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Renders the matching view with the supplied model data.
        }
    }
}
