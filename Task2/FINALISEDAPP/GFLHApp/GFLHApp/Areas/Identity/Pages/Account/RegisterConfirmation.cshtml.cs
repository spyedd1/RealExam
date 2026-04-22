// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity.UI.Services; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.WebUtilities; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    [AllowAnonymous] // Applies metadata or validation to the following member.
    // ----- Page Model Declaration -----
    public class RegisterConfirmationModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly IEmailSender _sender; // Stores an injected service used by the page model.

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender) // Receives services from dependency injection.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _sender = sender; // Sets _sender for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        // ----- Email Logic -----
        public string Email { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; } // Performs this page model step for the current request.

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null) // Handles GET requests that display this page.
        {
            // ----- Email Logic -----
            if (email == null) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return RedirectToPage("/Index"); // Redirects the browser after completing this step.
            }
            returnUrl = returnUrl ?? Url.Content("~/"); // Sets returnUrl for the current page flow.

            // ----- Injected Services -----
            var user = await _userManager.FindByEmailAsync(email); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                // ----- Email Logic -----
                return NotFound($"Unable to load user with email '{email}'."); // Returns a not-found result when user data is missing.
            }

            Email = email; // Sets Email for the current page flow.
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true; // Sets DisplayConfirmAccountLink for the current page flow.
            if (DisplayConfirmAccountLink) // Checks the condition before continuing this page flow.
            {
                // ----- Injected Services -----
                var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an Identity token for an account action.
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
                // ----- Email Logic -----
                EmailConfirmationUrl = Url.Page( // Builds a URL to another Razor Page.
                    "/Account/ConfirmEmail", // Confirms the user's email address.
                    pageHandler: null, // Performs this page model step for the current request.
                    // ----- Redirects and Results -----
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl }, // Sets area for the current page flow.
                    protocol: Request.Scheme); // Performs this page model step for the current request.
            }

            return Page(); // Renders the current Razor Page.
        }
    }
}
