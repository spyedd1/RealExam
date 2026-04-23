// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class DeletePersonalDataModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly ILogger<DeletePersonalDataModel> _logger; // Stores an injected service used by the page model.

        // ----- Personal Data -----
        public DeletePersonalDataModel( // Performs this page model step for the current request.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            ILogger<DeletePersonalDataModel> logger) // Writes account flow information to the application log.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
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
        // ----- Page Model Declaration -----
        public class InputModel // Defines the Razor Page model class for this page.
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ----- Input Models -----
            [Required] // Requires this form field during validation.
            [DataType(DataType.Password)] // Sets the intended display and input type.
            public string Password { get; set; } // Performs this page model step for the current request.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; } // Performs this page model step for the current request.

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGet() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            RequirePassword = await _userManager.HasPasswordAsync(user); // Runs the Identity operation asynchronously.
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

            RequirePassword = await _userManager.HasPasswordAsync(user); // Runs the Identity operation asynchronously.
            if (RequirePassword) // Checks the condition before continuing this page flow.
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password)) // Checks the condition before continuing this page flow.
                {
                    // ----- Validation Logic -----
                    ModelState.AddModelError(string.Empty, "Incorrect password."); // Adds a validation error for the page to display.
                    // ----- Redirects and Results -----
                    return Page(); // Renders the current Razor Page.
                }
            }

            // ----- Injected Services -----
            var result = await _userManager.DeleteAsync(user); // Runs the Identity operation asynchronously.
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            if (!result.Succeeded) // Checks the condition before continuing this page flow.
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user."); // Performs this page model step for the current request.
            }

            await _signInManager.SignOutAsync(); // Signs the current user out.

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId); // Writes account flow information to the application log.

            // ----- Redirects and Results -----
            return Redirect("~/"); // Redirects the browser after completing this step.
        }
    }
}
