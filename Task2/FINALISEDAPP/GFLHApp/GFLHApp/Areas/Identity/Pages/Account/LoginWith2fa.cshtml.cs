// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class LoginWith2faModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly ILogger<LoginWith2faModel> _logger; // Stores an injected service used by the page model.

        public LoginWith2faModel( // Writes account flow information to the application log.
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            ILogger<LoginWith2faModel> logger) // Writes account flow information to the application log.
        {
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            _userManager = userManager; // Sets _userManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        // ----- Input Models -----
        [BindProperty] // Binds posted form values to this property.
        public InputModel Input { get; set; } // Defines the form fields posted by this page.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        // ----- Redirects and Results -----
        public string ReturnUrl { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        // ----- Page Model Declaration -----
        public class InputModel // Defines the Razor Page model class for this page.
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ----- Input Models -----
            [Required] // Requires this form field during validation.
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] // Limits the accepted length of this input value.
            [DataType(DataType.Text)] // Sets the intended display and input type.
            [Display(Name = "Authenticator code")] // Sets the friendly label shown for this field.
            // ----- Authentication Logic -----
            public string TwoFactorCode { get; set; } // Handles two-factor authentication state or flow.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ----- Input Models -----
            [Display(Name = "Remember this machine")] // Sets the friendly label shown for this field.
            public bool RememberMachine { get; set; } // Performs this page model step for the current request.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null) // Handles GET requests that display this page.
        {
            // Ensure the user has gone through the username & password screen first
            // ----- Injected Services -----
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync(); // Handles two-factor authentication state or flow.

            if (user == null) // Checks the condition before continuing this page flow.
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user."); // Performs this page model step for the current request.
            }

            // ----- Redirects and Results -----
            ReturnUrl = returnUrl; // Sets ReturnUrl for the current page flow.
            RememberMe = rememberMe; // Sets RememberMe for the current page flow.

            return Page(); // Renders the current Razor Page.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null) // Handles POST requests submitted from this page.
        {
            // ----- Validation Logic -----
            if (!ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            returnUrl = returnUrl ?? Url.Content("~/"); // Sets returnUrl for the current page flow.

            // ----- Injected Services -----
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync(); // Handles two-factor authentication state or flow.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user."); // Performs this page model step for the current request.
            }

            // ----- Authentication Logic -----
            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty); // Handles two-factor authentication state or flow.

            // ----- Injected Services -----
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine); // Handles two-factor authentication state or flow.

            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.

            if (result.Succeeded) // Checks the condition before continuing this page flow.
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id); // Writes account flow information to the application log.
                // ----- Redirects and Results -----
                return LocalRedirect(returnUrl); // Redirects the browser after completing this step.
            }
            else if (result.IsLockedOut) // Handles the fallback branch for the previous condition.
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id); // Writes account flow information to the application log.
                return RedirectToPage("./Lockout"); // Redirects the browser after completing this step.
            }
            else // Handles the fallback branch for the previous condition.
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id); // Writes account flow information to the application log.
                // ----- Validation Logic -----
                ModelState.AddModelError(string.Empty, "Invalid authenticator code."); // Adds a validation error for the page to display.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }
        }
    }
}
