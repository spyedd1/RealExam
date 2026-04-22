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

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class SetPasswordModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.

        public SetPasswordModel( // Performs this page model step for the current request.
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager) // Performs this page model step for the current request.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
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
        [TempData] // Applies metadata or validation to the following member.
        public string StatusMessage { get; set; } // Stores a status message for the Razor Page to show.

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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] // Limits the accepted length of this input value.
            [DataType(DataType.Password)] // Sets the intended display and input type.
            [Display(Name = "New password")] // Sets the friendly label shown for this field.
            public string NewPassword { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)] // Sets the intended display and input type.
            [Display(Name = "Confirm new password")] // Sets the friendly label shown for this field.
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")] // Applies metadata or validation to the following member.
            public string ConfirmPassword { get; set; } // Performs this page model step for the current request.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetAsync() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            var hasPassword = await _userManager.HasPasswordAsync(user); // Runs the Identity operation asynchronously.

            if (hasPassword) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return RedirectToPage("./ChangePassword"); // Redirects the browser after completing this step.
            }

            return Page(); // Renders the current Razor Page.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostAsync() // Handles POST requests submitted from this page.
        {
            // ----- Validation Logic -----
            if (!ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword); // Reads or writes a submitted form input value.
            if (!addPasswordResult.Succeeded) // Checks the condition before continuing this page flow.
            {
                foreach (var error in addPasswordResult.Errors) // Loops through each item needed by the page flow.
                {
                    // ----- Validation Logic -----
                    ModelState.AddModelError(string.Empty, error.Description); // Adds a validation error for the page to display.
                }
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            await _signInManager.RefreshSignInAsync(user); // Signs the user in after the account action.
            StatusMessage = "Your password has been set."; // Stores a status message for the Razor Page to show.

            // ----- Redirects and Results -----
            return RedirectToPage(); // Redirects the browser after completing this step.
        }
    }
}
