// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Encodings.Web; // Imports a namespace needed by this page model.
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
    public class ResendEmailConfirmationModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly IEmailSender _emailSender; // Stores an injected service used by the page model.

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender) // Receives services from dependency injection.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            // ----- Email Logic -----
            _emailSender = emailSender; // Sets _emailSender for the current page flow.
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
        }

        // ----- Page Handlers -----
        public void OnGet() // Receives services from dependency injection.
        {
        }

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
                // ----- Validation Logic -----
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email."); // Adds a validation error for the page to display.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an Identity token for an account action.
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
            // ----- Redirects and Results -----
            var callbackUrl = Url.Page( // Builds a URL to another Razor Page.
                // ----- Email Logic -----
                "/Account/ConfirmEmail", // Confirms the user's email address.
                pageHandler: null, // Performs this page model step for the current request.
                values: new { userId = userId, code = code }, // Sets userId for the current page flow.
                protocol: Request.Scheme); // Performs this page model step for the current request.
            await _emailSender.SendEmailAsync( // Sends an account email to the user.
                Input.Email, // Reads or writes a submitted form input value.
                "Confirm your email", // Performs this page model step for the current request.
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Encodes user-facing URL or HTML content safely.

            // ----- Validation Logic -----
            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email."); // Adds a validation error for the page to display.
            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }
    }
}
