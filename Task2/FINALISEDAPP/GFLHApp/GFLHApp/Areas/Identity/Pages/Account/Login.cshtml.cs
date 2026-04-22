// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Collections.Generic; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authentication; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity.UI.Services; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class LoginModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly ILogger<LoginModel> _logger; // Stores an injected service used by the page model.

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger) // Receives services from dependency injection.
        {
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
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
        // ----- Authentication Logic -----
        public IList<AuthenticationScheme> ExternalLogins { get; set; } // Handles external provider sign-in flow.

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
        [TempData] // Applies metadata or validation to the following member.
        public string ErrorMessage { get; set; } // Performs this page model step for the current request.

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
            [EmailAddress] // Validates that this field contains an email address.
            // ----- Email Logic -----
            public string Email { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ----- Input Models -----
            [Required] // Requires this form field during validation.
            [DataType(DataType.Password)] // Sets the intended display and input type.
            public string Password { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")] // Sets the friendly label shown for this field.
            public bool RememberMe { get; set; } // Performs this page model step for the current request.
        }

        // ----- Page Handlers -----
        public async Task OnGetAsync(string returnUrl = null) // Handles GET requests that display this page.
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) // Checks the condition before continuing this page flow.
            {
                // ----- Validation Logic -----
                ModelState.AddModelError(string.Empty, ErrorMessage); // Adds a validation error for the page to display.
            }

            // ----- Redirects and Results -----
            returnUrl ??= Url.Content("~/"); // Sets ?? for the current page flow.

            // Clear the existing external cookie to ensure a clean login process
            // ----- Authentication Logic -----
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // Signs the current user out.

            // ----- Injected Services -----
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Handles external provider sign-in flow.

            // ----- Redirects and Results -----
            ReturnUrl = returnUrl; // Sets ReturnUrl for the current page flow.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostAsync(string returnUrl = null) // Handles POST requests submitted from this page.
        {
            // ----- Redirects and Results -----
            returnUrl ??= Url.Content("~/"); // Sets ?? for the current page flow.

            // ----- Injected Services -----
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Handles external provider sign-in flow.

            // ----- Validation Logic -----
            if (ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                // ----- Injected Services -----
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false); // Attempts password sign-in with the submitted credentials.
                if (result.Succeeded) // Checks the condition before continuing this page flow.
                {
                    _logger.LogInformation("User logged in."); // Writes account flow information to the application log.
                    // ----- Redirects and Results -----
                    return LocalRedirect(returnUrl); // Redirects the browser after completing this step.
                }
                // ----- Authentication Logic -----
                if (result.RequiresTwoFactor) // Handles two-factor authentication state or flow.
                {
                    // ----- Redirects and Results -----
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe }); // Redirects the browser after completing this step.
                }
                if (result.IsLockedOut) // Checks the condition before continuing this page flow.
                {
                    _logger.LogWarning("User account locked out."); // Writes account flow information to the application log.
                    return RedirectToPage("./Lockout"); // Redirects the browser after completing this step.
                }
                else // Handles the fallback branch for the previous condition.
                {
                    // ----- Validation Logic -----
                    ModelState.AddModelError(string.Empty, "Invalid login attempt."); // Adds a validation error for the page to display.
                    // ----- Redirects and Results -----
                    return Page(); // Renders the current Razor Page.
                }
            }

            // If we got this far, something failed, redisplay form
            return Page(); // Renders the current Razor Page.
        }
    }
}
