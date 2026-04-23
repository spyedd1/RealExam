// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class GenerateRecoveryCodesModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly ILogger<GenerateRecoveryCodesModel> _logger; // Stores an injected service used by the page model.

        // ----- Authentication Logic -----
        public GenerateRecoveryCodesModel( // Handles two-factor recovery code validation.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            ILogger<GenerateRecoveryCodesModel> logger) // Handles two-factor recovery code validation.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData] // Applies metadata or validation to the following member.
        // ----- Authentication Logic -----
        public string[] RecoveryCodes { get; set; } // Handles two-factor recovery code validation.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData] // Applies metadata or validation to the following member.
        public string StatusMessage { get; set; } // Stores a status message for the Razor Page to show.

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetAsync() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user); // Handles two-factor authentication state or flow.
            // ----- Authentication Logic -----
            if (!isTwoFactorEnabled) // Handles two-factor authentication state or flow.
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user because they do not have 2FA enabled."); // Performs this page model step for the current request.
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

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user); // Handles two-factor authentication state or flow.
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            // ----- Authentication Logic -----
            if (!isTwoFactorEnabled) // Handles two-factor authentication state or flow.
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user as they do not have 2FA enabled."); // Performs this page model step for the current request.
            }

            // ----- Injected Services -----
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10); // Handles two-factor authentication state or flow.
            // ----- Authentication Logic -----
            RecoveryCodes = recoveryCodes.ToArray(); // Handles two-factor recovery code validation.

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId); // Writes account flow information to the application log.
            StatusMessage = "You have generated new recovery codes."; // Stores a status message for the Razor Page to show.
            return RedirectToPage("./ShowRecoveryCodes"); // Handles two-factor recovery code validation.
        }
    }
}
