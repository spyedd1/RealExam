// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Security.Claims; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Encodings.Web; // Imports a namespace needed by this page model.
using System.Threading; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Options; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity.UI.Services; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.WebUtilities; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    [AllowAnonymous] // Applies metadata or validation to the following member.
    // ----- Page Model Declaration -----
    public class ExternalLoginModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly IUserStore<IdentityUser> _userStore; // Stores an injected service used by the page model.
        private readonly IUserEmailStore<IdentityUser> _emailStore; // Stores an injected service used by the page model.
        private readonly IEmailSender _emailSender; // Stores an injected service used by the page model.
        private readonly ILogger<ExternalLoginModel> _logger; // Stores an injected service used by the page model.

        // ----- Authentication Logic -----
        public ExternalLoginModel( // Handles external provider sign-in flow.
            // ----- Injected Services -----
            SignInManager<IdentityUser> signInManager, // Performs this page model step for the current request.
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            IUserStore<IdentityUser> userStore, // Performs this page model step for the current request.
            ILogger<ExternalLoginModel> logger, // Handles external provider sign-in flow.
            IEmailSender emailSender) // Sends an account email to the user.
        {
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            _userManager = userManager; // Sets _userManager for the current page flow.
            _userStore = userStore; // Sets _userStore for the current page flow.
            // ----- Email Logic -----
            _emailStore = GetEmailStore(); // Sets _emailStore for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
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
        public string ProviderDisplayName { get; set; } // Performs this page model step for the current request.

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
        }

        // ----- Page Handlers -----
        public IActionResult OnGet() => RedirectToPage("./Login"); // Receives services from dependency injection.

        public IActionResult OnPost(string provider, string returnUrl = null) // Receives services from dependency injection.
        {
            // Request a redirect to the external login provider.
            // ----- Authentication Logic -----
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl }); // Handles external provider sign-in flow.
            // ----- Injected Services -----
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl); // Sets properties for the current page flow.
            // ----- Redirects and Results -----
            return new ChallengeResult(provider, properties); // Returns the result for this page handler.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null) // Handles GET requests that display this page.
        {
            // ----- Redirects and Results -----
            returnUrl = returnUrl ?? Url.Content("~/"); // Sets returnUrl for the current page flow.
            if (remoteError != null) // Checks the condition before continuing this page flow.
            {
                ErrorMessage = $"Error from external provider: {remoteError}"; // Sets ErrorMessage for the current page flow.
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl }); // Redirects the browser after completing this step.
            }
            // ----- Injected Services -----
            var info = await _signInManager.GetExternalLoginInfoAsync(); // Handles external provider sign-in flow.
            if (info == null) // Checks the condition before continuing this page flow.
            {
                ErrorMessage = "Error loading external login information."; // Sets ErrorMessage for the current page flow.
                // ----- Redirects and Results -----
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl }); // Redirects the browser after completing this step.
            }

            // Sign in the user with this external login provider if the user already has a login.
            // ----- Injected Services -----
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true); // Handles two-factor authentication state or flow.
            if (result.Succeeded) // Checks the condition before continuing this page flow.
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider); // Writes account flow information to the application log.
                // ----- Redirects and Results -----
                return LocalRedirect(returnUrl); // Redirects the browser after completing this step.
            }
            if (result.IsLockedOut) // Checks the condition before continuing this page flow.
            {
                return RedirectToPage("./Lockout"); // Redirects the browser after completing this step.
            }
            else // Handles the fallback branch for the previous condition.
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl; // Sets ReturnUrl for the current page flow.
                ProviderDisplayName = info.ProviderDisplayName; // Sets ProviderDisplayName for the current page flow.
                // ----- Email Logic -----
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email)) // Checks the condition before continuing this page flow.
                {
                    Input = new InputModel // Defines the form fields posted by this page.
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email) // Sets Email for the current page flow.
                    };
                }
                // ----- Redirects and Results -----
                return Page(); // Renders the current Razor Page.
            }
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null) // Handles POST requests submitted from this page.
        {
            // ----- Redirects and Results -----
            returnUrl = returnUrl ?? Url.Content("~/"); // Sets returnUrl for the current page flow.
            // Get the information about the user from the external login provider
            // ----- Injected Services -----
            var info = await _signInManager.GetExternalLoginInfoAsync(); // Handles external provider sign-in flow.
            if (info == null) // Checks the condition before continuing this page flow.
            {
                ErrorMessage = "Error loading external login information during confirmation."; // Sets ErrorMessage for the current page flow.
                // ----- Redirects and Results -----
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl }); // Redirects the browser after completing this step.
            }

            // ----- Validation Logic -----
            if (ModelState.IsValid) // Checks whether submitted form values passed validation.
            {
                var user = CreateUser(); // Sets user for the current page flow.

                // ----- Email Logic -----
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None); // Reads or writes a submitted form input value.
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // Reads or writes a submitted form input value.

                // ----- Injected Services -----
                var result = await _userManager.CreateAsync(user); // Runs the Identity operation asynchronously.
                if (result.Succeeded) // Checks the condition before continuing this page flow.
                {
                    result = await _userManager.AddLoginAsync(user, info); // Writes account flow information to the application log.
                    if (result.Succeeded) // Checks the condition before continuing this page flow.
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider); // Writes account flow information to the application log.

                        var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an Identity token for an account action.
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
                        // ----- Redirects and Results -----
                        var callbackUrl = Url.Page( // Builds a URL to another Razor Page.
                            // ----- Email Logic -----
                            "/Account/ConfirmEmail", // Confirms the user's email address.
                            pageHandler: null, // Performs this page model step for the current request.
                            values: new { area = "Identity", userId = userId, code = code }, // Sets area for the current page flow.
                            protocol: Request.Scheme); // Performs this page model step for the current request.

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", // Sends an account email to the user.
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Encodes user-facing URL or HTML content safely.

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        // ----- Injected Services -----
                        if (_userManager.Options.SignIn.RequireConfirmedAccount) // Checks the condition before continuing this page flow.
                        {
                            // ----- Email Logic -----
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email }); // Redirects the browser after completing this step.
                        }

                        // ----- Injected Services -----
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider); // Signs the user in after the account action.
                        // ----- Redirects and Results -----
                        return LocalRedirect(returnUrl); // Redirects the browser after completing this step.
                    }
                }
                foreach (var error in result.Errors) // Loops through each item needed by the page flow.
                {
                    // ----- Validation Logic -----
                    ModelState.AddModelError(string.Empty, error.Description); // Adds a validation error for the page to display.
                }
            }

            ProviderDisplayName = info.ProviderDisplayName; // Sets ProviderDisplayName for the current page flow.
            // ----- Redirects and Results -----
            ReturnUrl = returnUrl; // Sets ReturnUrl for the current page flow.
            return Page(); // Renders the current Razor Page.
        }

        private IdentityUser CreateUser() // Performs this page model step for the current request.
        {
            try // Starts a protected operation block.
            {
                return Activator.CreateInstance<IdentityUser>(); // Returns the result for this page handler.
            }
            catch // Handles an error from the protected operation.
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " + // Performs this page model step for the current request.
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " + // Performs this page model step for the current request.
                    // ----- Authentication Logic -----
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml"); // Handles external provider sign-in flow.
            }
        }

        // ----- Email Logic -----
        private IUserEmailStore<IdentityUser> GetEmailStore() // Performs this page model step for the current request.
        {
            // ----- Injected Services -----
            if (!_userManager.SupportsUserEmail) // Checks the condition before continuing this page flow.
            {
                // ----- Email Logic -----
                throw new NotSupportedException("The default UI requires a user store with email support."); // Performs this page model step for the current request.
            }
            return (IUserEmailStore<IdentityUser>)_userStore; // Returns the result for this page handler.
        }
    }
}
