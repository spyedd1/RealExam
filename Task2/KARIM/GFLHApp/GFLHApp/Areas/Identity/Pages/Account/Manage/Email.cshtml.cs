// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Encodings.Web; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity.UI.Services; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.WebUtilities; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class EmailModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly IEmailSender _emailSender; // Stores an injected service used by the page model.

        // ----- Email Logic -----
        public EmailModel( // Performs this page model step for the current request.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            IEmailSender emailSender) // Sends an account email to the user.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            // ----- Email Logic -----
            _emailSender = emailSender; // Sets _emailSender for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool IsEmailConfirmed { get; set; } // Performs this page model step for the current request.

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
            [Display(Name = "New email")] // Sets the friendly label shown for this field.
            // ----- Email Logic -----
            public string NewEmail { get; set; } // Performs this page model step for the current request.
        }

        private async Task LoadAsync(IdentityUser user) // Performs this page model step for the current request.
        {
            // ----- Injected Services -----
            var email = await _userManager.GetEmailAsync(user); // Runs the Identity operation asynchronously.
            // ----- Email Logic -----
            Email = email; // Sets Email for the current page flow.

            Input = new InputModel // Defines the form fields posted by this page.
            {
                NewEmail = email, // Sets NewEmail for the current page flow.
            };

            // ----- Injected Services -----
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user); // Runs the Identity operation asynchronously.
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

            await LoadAsync(user); // Runs the Identity operation asynchronously.
            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostChangeEmailAsync() // Handles POST requests submitted from this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            // ----- Validation Logic -----
            if (!ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                await LoadAsync(user); // Runs the Identity operation asynchronously.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            var email = await _userManager.GetEmailAsync(user); // Runs the Identity operation asynchronously.
            // ----- Email Logic -----
            if (Input.NewEmail != email) // Checks the condition before continuing this page flow.
            {
                // ----- Injected Services -----
                var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail); // Generates an Identity token for an account action.
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
                // ----- Redirects and Results -----
                var callbackUrl = Url.Page( // Builds a URL to another Razor Page.
                    // ----- Email Logic -----
                    "/Account/ConfirmEmailChange", // Confirms the user's email address.
                    pageHandler: null, // Performs this page model step for the current request.
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code }, // Reads or writes a submitted form input value.
                    protocol: Request.Scheme); // Performs this page model step for the current request.
                await _emailSender.SendEmailAsync( // Sends an account email to the user.
                    Input.NewEmail, // Reads or writes a submitted form input value.
                    "Confirm your email", // Performs this page model step for the current request.
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Encodes user-facing URL or HTML content safely.

                StatusMessage = "Confirmation link to change email sent. Please check your email."; // Stores a status message for the Razor Page to show.
                // ----- Redirects and Results -----
                return RedirectToPage(); // Redirects the browser after completing this step.
            }

            // ----- Email Logic -----
            StatusMessage = "Your email is unchanged."; // Stores a status message for the Razor Page to show.
            // ----- Redirects and Results -----
            return RedirectToPage(); // Redirects the browser after completing this step.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostSendVerificationEmailAsync() // Handles POST requests submitted from this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            // ----- Validation Logic -----
            if (!ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                await LoadAsync(user); // Runs the Identity operation asynchronously.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            var email = await _userManager.GetEmailAsync(user); // Runs the Identity operation asynchronously.
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an Identity token for an account action.
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
            // ----- Redirects and Results -----
            var callbackUrl = Url.Page( // Builds a URL to another Razor Page.
                // ----- Email Logic -----
                "/Account/ConfirmEmail", // Confirms the user's email address.
                pageHandler: null, // Performs this page model step for the current request.
                values: new { area = "Identity", userId = userId, code = code }, // Sets area for the current page flow.
                protocol: Request.Scheme); // Performs this page model step for the current request.
            await _emailSender.SendEmailAsync( // Sends an account email to the user.
                email, // Performs this page model step for the current request.
                "Confirm your email", // Performs this page model step for the current request.
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Encodes user-facing URL or HTML content safely.

            StatusMessage = "Verification email sent. Please check your email."; // Stores a status message for the Razor Page to show.
            // ----- Redirects and Results -----
            return RedirectToPage(); // Redirects the browser after completing this step.
        }
    }
}
