// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.WebUtilities; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class ConfirmEmailModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.

        public ConfirmEmailModel(UserManager<IdentityUser> userManager) // Receives services from dependency injection.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData] // Applies metadata or validation to the following member.
        public string StatusMessage { get; set; } // Stores a status message for the Razor Page to show.
        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetAsync(string userId, string code) // Handles GET requests that display this page.
        {
            if (userId == null || code == null) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return RedirectToPage("/Index"); // Redirects the browser after completing this step.
            }

            // ----- Injected Services -----
            var user = await _userManager.FindByIdAsync(userId); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return NotFound($"Unable to load user with ID '{userId}'."); // Returns a not-found result when user data is missing.
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)); // Sets code for the current page flow.
            // ----- Injected Services -----
            var result = await _userManager.ConfirmEmailAsync(user, code); // Confirms the user's email address.
            // ----- Email Logic -----
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email."; // Stores a status message for the Razor Page to show.
            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }
    }
}
