// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Globalization; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Encodings.Web; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class EnableAuthenticatorModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly ILogger<EnableAuthenticatorModel> _logger; // Stores an injected service used by the page model.
        private readonly UrlEncoder _urlEncoder; // Stores an injected service used by the page model.

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6"; // Sets AuthenticatorUriFormat for the current page flow.

        public EnableAuthenticatorModel( // Performs this page model step for the current request.
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            ILogger<EnableAuthenticatorModel> logger, // Writes account flow information to the application log.
            UrlEncoder urlEncoder) // Encodes user-facing URL or HTML content safely.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
            _urlEncoder = urlEncoder; // Sets _urlEncoder for the current page flow.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string SharedKey { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string AuthenticatorUri { get; set; } // Performs this page model step for the current request.

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
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] // Limits the accepted length of this input value.
            [DataType(DataType.Text)] // Sets the intended display and input type.
            [Display(Name = "Verification Code")] // Sets the friendly label shown for this field.
            public string Code { get; set; } // Performs this page model step for the current request.
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

            await LoadSharedKeyAndQrCodeUriAsync(user); // Runs the Identity operation asynchronously.

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

            // ----- Validation Logic -----
            if (!ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                await LoadSharedKeyAndQrCodeUriAsync(user); // Runs the Identity operation asynchronously.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // Strip spaces and hyphens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty); // Reads or writes a submitted form input value.

            // ----- Injected Services -----
            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync( // Handles two-factor authentication state or flow.
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode); // Performs this page model step for the current request.

            if (!is2faTokenValid) // Checks the condition before continuing this page flow.
            {
                // ----- Validation Logic -----
                ModelState.AddModelError("Input.Code", "Verification code is invalid."); // Adds a validation error for the page to display.
                await LoadSharedKeyAndQrCodeUriAsync(user); // Runs the Identity operation asynchronously.
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }

            // ----- Injected Services -----
            await _userManager.SetTwoFactorEnabledAsync(user, true); // Handles two-factor authentication state or flow.
            var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId); // Writes account flow information to the application log.

            StatusMessage = "Your authenticator app has been verified."; // Stores a status message for the Razor Page to show.

            if (await _userManager.CountRecoveryCodesAsync(user) == 0) // Handles two-factor recovery code validation.
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10); // Handles two-factor authentication state or flow.
                // ----- Authentication Logic -----
                RecoveryCodes = recoveryCodes.ToArray(); // Handles two-factor recovery code validation.
                return RedirectToPage("./ShowRecoveryCodes"); // Handles two-factor recovery code validation.
            }
            else // Handles the fallback branch for the previous condition.
            {
                return RedirectToPage("./TwoFactorAuthentication"); // Handles two-factor authentication state or flow.
            }
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(IdentityUser user) // Performs this page model step for the current request.
        {
            // Load the authenticator key & QR code URI to display on the form
            // ----- Injected Services -----
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user); // Runs the Identity operation asynchronously.
            if (string.IsNullOrEmpty(unformattedKey)) // Checks the condition before continuing this page flow.
            {
                await _userManager.ResetAuthenticatorKeyAsync(user); // Runs the Identity operation asynchronously.
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user); // Runs the Identity operation asynchronously.
            }

            SharedKey = FormatKey(unformattedKey); // Sets SharedKey for the current page flow.

            var email = await _userManager.GetEmailAsync(user); // Runs the Identity operation asynchronously.
            // ----- Email Logic -----
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey); // Sets AuthenticatorUri for the current page flow.
        }

        private string FormatKey(string unformattedKey) // Performs this page model step for the current request.
        {
            var result = new StringBuilder(); // Sets result for the current page flow.
            int currentPosition = 0; // Sets currentPosition for the current page flow.
            while (currentPosition + 4 < unformattedKey.Length) // Performs this page model step for the current request.
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' '); // Performs this page model step for the current request.
                currentPosition += 4; // Sets + for the current page flow.
            }
            if (currentPosition < unformattedKey.Length) // Checks the condition before continuing this page flow.
            {
                result.Append(unformattedKey.AsSpan(currentPosition)); // Performs this page model step for the current request.
            }

            // ----- Redirects and Results -----
            return result.ToString().ToLowerInvariant(); // Returns the result for this page handler.
        }

        // ----- Email Logic -----
        private string GenerateQrCodeUri(string email, string unformattedKey) // Performs this page model step for the current request.
        {
            // ----- Redirects and Results -----
            return string.Format( // Returns the result for this page handler.
                CultureInfo.InvariantCulture, // Performs this page model step for the current request.
                AuthenticatorUriFormat, // Performs this page model step for the current request.
                // ----- Injected Services -----
                _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"), // Performs this page model step for the current request.
                _urlEncoder.Encode(email), // Performs this page model step for the current request.
                unformattedKey); // Performs this page model step for the current request.
        }
    }
}
