// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Collections.Generic; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Threading; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authentication; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class ExternalLoginsModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly IUserStore<IdentityUser> _userStore; // Stores an injected service used by the page model.

        // ----- Authentication Logic -----
        public ExternalLoginsModel( // Handles external provider sign-in flow.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            IUserStore<IdentityUser> userStore) // Performs this page model step for the current request.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            _userStore = userStore; // Sets _userStore for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; } // Writes account flow information to the application log.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; } // Writes account flow information to the application log.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool ShowRemoveButton { get; set; } // Performs this page model step for the current request.

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

            CurrentLogins = await _userManager.GetLoginsAsync(user); // Writes account flow information to the application log.
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()) // Writes account flow information to the application log.
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider)) // Writes account flow information to the application log.
                .ToList(); // Performs this page model step for the current request.

            string passwordHash = null; // Sets passwordHash for the current page flow.
            if (_userStore is IUserPasswordStore<IdentityUser> userPasswordStore) // Checks the condition before continuing this page flow.
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted); // Runs the Identity operation asynchronously.
            }

            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1; // Writes account flow information to the application log.
            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey) // Handles POST requests submitted from this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey); // Writes account flow information to the application log.
            if (!result.Succeeded) // Checks the condition before continuing this page flow.
            {
                StatusMessage = "The external login was not removed."; // Stores a status message for the Razor Page to show.
                // ----- Redirects and Results -----
                return RedirectToPage(); // Redirects the browser after completing this step.
            }

            // ----- Injected Services -----
            await _signInManager.RefreshSignInAsync(user); // Signs the user in after the account action.
            StatusMessage = "The external login was removed."; // Stores a status message for the Razor Page to show.
            // ----- Redirects and Results -----
            return RedirectToPage(); // Redirects the browser after completing this step.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostLinkLoginAsync(string provider) // Handles POST requests submitted from this page.
        {
            // Clear the existing external cookie to ensure a clean login process
            // ----- Authentication Logic -----
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // Signs the current user out.

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback"); // Handles external provider sign-in flow.
            // ----- Injected Services -----
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User)); // Sets properties for the current page flow.
            // ----- Redirects and Results -----
            return new ChallengeResult(provider, properties); // Returns the result for this page handler.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetLinkLoginCallbackAsync() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            var info = await _signInManager.GetExternalLoginInfoAsync(userId); // Handles external provider sign-in flow.
            if (info == null) // Checks the condition before continuing this page flow.
            {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info."); // Performs this page model step for the current request.
            }

            var result = await _userManager.AddLoginAsync(user, info); // Writes account flow information to the application log.
            if (!result.Succeeded) // Checks the condition before continuing this page flow.
            {
                StatusMessage = "The external login was not added. External logins can only be associated with one account."; // Stores a status message for the Razor Page to show.
                // ----- Redirects and Results -----
                return RedirectToPage(); // Redirects the browser after completing this step.
            }

            // Clear the existing external cookie to ensure a clean login process
            // ----- Authentication Logic -----
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // Signs the current user out.

            StatusMessage = "The external login was added."; // Stores a status message for the Razor Page to show.
            // ----- Redirects and Results -----
            return RedirectToPage(); // Redirects the browser after completing this step.
        }
    }
}
