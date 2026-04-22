// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class ResetAuthenticatorModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly ILogger<ResetAuthenticatorModel> _logger; // Stores an injected service used by the page model.

        public ResetAuthenticatorModel( // Performs this page model step for the current request.
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            ILogger<ResetAuthenticatorModel> logger) // Writes account flow information to the application log.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData] // Applies metadata or validation to the following member.
        public string StatusMessage { get; set; } // Stores a status message for the Razor Page to show.

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGet() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostAsync() // Handles POST requests submitted from this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false); // Handles two-factor authentication state or flow.
            await _userManager.ResetAuthenticatorKeyAsync(user); // Runs the Identity operation asynchronously.
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id); // Writes account flow information to the application log.

            await _signInManager.RefreshSignInAsync(user); // Signs the user in after the account action.
            StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key."; // Stores a status message for the Razor Page to show.

            // ----- Redirects and Results -----
            return RedirectToPage("./EnableAuthenticator"); // Redirects the browser after completing this step.
        }
    }
}
