// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
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
    public class ResetPasswordModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.

        public ResetPasswordModel(UserManager<IdentityUser> userManager) // Receives services from dependency injection.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
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
            [EmailAddress] // Validates that this field contains an email address.
            // ----- Email Logic -----
            public string Email { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ----- Input Models -----
            [Required] // Requires this form field during validation.
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] // Limits the accepted length of this input value.
            [DataType(DataType.Password)] // Sets the intended display and input type.
            public string Password { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)] // Sets the intended display and input type.
            [Display(Name = "Confirm password")] // Sets the friendly label shown for this field.
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] // Applies metadata or validation to the following member.
            public string ConfirmPassword { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required] // Requires this form field during validation.
            public string Code { get; set; } // Performs this page model step for the current request.

        }

        // ----- Page Handlers -----
        public IActionResult OnGet(string code = null) // Receives services from dependency injection.
        {
            if (code == null) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return BadRequest("A code must be supplied for password reset."); // Returns the result for this page handler.
            }
            else // Handles the fallback branch for the previous condition.
            {
                Input = new InputModel // Defines the form fields posted by this page.
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)) // Sets Code for the current page flow.
                };
                return Page(); // Renders the current Razor Page.
            }
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
            var user = await _userManager.FindByEmailAsync(Input.Email); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                // Don't reveal that the user does not exist
                // ----- Redirects and Results -----
                return RedirectToPage("./ResetPasswordConfirmation"); // Redirects the browser after completing this step.
            }

            // ----- Injected Services -----
            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password); // Reads or writes a submitted form input value.
            if (result.Succeeded) // Checks the condition before continuing this page flow.
            {
                // ----- Redirects and Results -----
                return RedirectToPage("./ResetPasswordConfirmation"); // Redirects the browser after completing this step.
            }

            foreach (var error in result.Errors) // Loops through each item needed by the page flow.
            {
                // ----- Validation Logic -----
                ModelState.AddModelError(string.Empty, error.Description); // Adds a validation error for the page to display.
            }
            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }
    }
}
