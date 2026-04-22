// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Collections.Generic; // Imports a namespace needed by this page model.
using System.ComponentModel.DataAnnotations; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Encodings.Web; // Imports a namespace needed by this page model.
using System.Threading; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authentication; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity.UI.Services; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.WebUtilities; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class RegisterModel : PageModel // This class represents the model for the registration page in the ASP.NET Core Identity system. It handles the logic for registering a new user, including creating the user, assigning roles, and sending confirmation emails. // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly SignInManager<IdentityUser> _signInManager; // This is a service that provides methods for signing in users, including password sign-in and external login sign-in. // Stores an injected service used by the page model.
        private readonly UserManager<IdentityUser> _userManager; // This is a service that provides methods for managing users, including creating users, finding users, and managing user roles. // Stores an injected service used by the page model.
        private readonly IUserStore<IdentityUser> _userStore; // This is an interface that defines methods for storing and retrieving user information. It is used by the UserManager to interact with the underlying user store (e.g., a database). // Stores an injected service used by the page model.
        private readonly IUserEmailStore<IdentityUser> _emailStore; // This is an interface that defines methods for storing and retrieving user email information. It is used by the UserManager to manage user email addresses. // Stores an injected service used by the page model.
        private readonly ILogger<RegisterModel> _logger; // This is a logging service that allows the application to log information, warnings, and errors. It is used in this class to log when a new user account is created. // Defines the Razor Page model class for this page.
        private readonly IEmailSender _emailSender; // This is a service that provides methods for sending emails. It is used in this class to send confirmation emails to users after they register. // Stores an injected service used by the page model.
        private readonly RoleManager<IdentityRole> _roleManager; // This is a service that provides methods for managing roles, including creating roles and checking if roles exist. It is used in this class to assign the "Standard" role to new users. // Stores an injected service used by the page model.

        public RegisterModel( // This is the constructor for the RegisterModel class. It takes several services as parameters and assigns them to private fields for use in the class methods. // Defines the Razor Page model class for this page.
            UserManager<IdentityUser> userManager, // This parameter is the UserManager service, which is used to manage user accounts. // Performs this page model step for the current request.
            IUserStore<IdentityUser> userStore, // This parameter is the IUserStore service, which is used to interact with the user store. // Performs this page model step for the current request.
            SignInManager<IdentityUser> signInManager, // This parameter is the SignInManager service, which is used to handle user sign-in operations. // Performs this page model step for the current request.
            ILogger<RegisterModel> logger, // This parameter is the ILogger service, which is used for logging information about the registration process. // Writes account flow information to the application log.
            IEmailSender emailSender, //    This parameter is the IEmailSender service, which is used to send confirmation emails to users after they register. // Sends an account email to the user.
            RoleManager<IdentityRole> roleManager) // This parameter is the RoleManager service, which is used to manage user roles and assign roles to users. // Performs this page model step for the current request.
        {
            _userManager = userManager; // Assign the UserManager service to the private field _userManager for use in the class methods. // Sets _userManager for the current page flow.
            _userStore = userStore; // Assign the IUserStore service to the private field _userStore for use in the class methods. // Sets _userStore for the current page flow.
            // ----- Email Logic -----
            _emailStore = GetEmailStore(); // Call the GetEmailStore method to get an instance of IUserEmailStore and assign it to the private field _emailStore for use in the class methods. // Sets _emailStore for the current page flow.
            // ----- Injected Services -----
            _signInManager = signInManager; // Assign the SignInManager service to the private field _signInManager for use in the class methods. // Sets _signInManager for the current page flow.
            _logger = logger; // Assign the ILogger service to the private field _logger for use in the class methods to log information about the registration process. // Writes account flow information to the application log.
            _emailSender = emailSender; // Assign the IEmailSender service to the private field _emailSender for use in the class methods to send confirmation emails to users after they register. // Sends an account email to the user.
            _roleManager = roleManager;  // Assign the RoleManager service to the private field _roleManager for use in the class methods to manage user roles and assign roles to users. // Sets _roleManager for the current page flow.
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
        // ----- Redirects and Results -----
        public string ReturnUrl { get; set; } // Performs this page model step for the current request.

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
            [Display(Name = "Email")] // Sets the friendly label shown for this field.
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
            [Display(Name = "Password")] // Sets the friendly label shown for this field.
            public string Password { get; set; } // Performs this page model step for the current request.

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)] // Sets the intended display and input type.
            [Display(Name = "Confirm password")] // Sets the friendly label shown for this field.
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] // Applies metadata or validation to the following member.
            public string ConfirmPassword { get; set; } // Performs this page model step for the current request.
        }


        // ----- Page Handlers -----
        public async Task OnGetAsync(string returnUrl = null) // Handles GET requests that display this page.
        {
            // ----- Redirects and Results -----
            ReturnUrl = returnUrl; // Sets ReturnUrl for the current page flow.
            // ----- Injected Services -----
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Handles external provider sign-in flow.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPostAsync(string returnUrl = null) // This method is called when the user submits the registration form. It handles the logic for creating a new user account, assigning roles, and sending confirmation emails. It also handles validation and error handling for the registration process. // Handles POST requests submitted from this page.
        {
            // ----- Redirects and Results -----
            returnUrl ??= Url.Content("~/"); // If the returnUrl parameter is null, set it to the root URL of the application. This is used to redirect the user after successful registration. // Sets ?? for the current page flow.
            // ----- Injected Services -----
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Get the list of external authentication schemes (e.g., Google, Facebook) and assign it to the ExternalLogins property for use in the registration page. // Handles external provider sign-in flow.
            // ----- Input Models -----
            if (ModelState.IsValid) // Check if the model state is valid, which means that all required fields are filled out correctly and any validation attributes are satisfied. If the model state is not valid, the registration form will be redisplayed with validation error messages. // Checks whether submitted form values passed validation.
            {
                var user = CreateUser(); // Create a new instance of the IdentityUser class using the CreateUser method. This represents the new user account that will be created. // Sets user for the current page flow.
                // ----- Email Logic -----
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None); // Set the username of the new user to the email address provided in the registration form using the IUserStore service. // Reads or writes a submitted form input value.
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // Set the email address of the new user to the email address provided in the registration form using the IUserEmailStore service. // Reads or writes a submitted form input value.
                // ----- Injected Services -----
                var result = await _userManager.CreateAsync(user, Input.Password); // Create the new user account using the UserManager service, passing in the user object and the password provided in the registration form. The result of this operation is stored in the result variable, which indicates whether the user creation was successful and contains any errors that occurred during the process. // Reads or writes a submitted form input value.
                // ----- Email Logic -----
                if (result.Succeeded) // Check if the user creation was successful. If it was, proceed with assigning roles and sending confirmation emails. If it was not successful, the registration form will be redisplayed with error messages. // Checks the condition before continuing this page flow.
                {
                    // ----- Injected Services -----
                    _logger.LogInformation("User created a new account with password."); // Log an informational message indicating that a new user account was created successfully using the ILogger service. // Writes account flow information to the application log.

                    //  Assign "Standard" role to every new user
                    if (!await _roleManager.RoleExistsAsync("Standard")) // Check if the "Standard" role already exists using the RoleManager service. If it does not exist, create the role. // Checks the condition before continuing this page flow.
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Standard")); // Create the "Standard" role using the RoleManager service by passing in a new instance of the IdentityRole class with the name "Standard". This ensures that every new user will be assigned to the "Standard" role, which can be used for authorization and access control in the application. // Runs the Identity operation asynchronously.
                    }
                    await _userManager.AddToRoleAsync(user, "Standard"); // Assign the newly created user to the "Standard" role using the UserManager service by calling the AddToRoleAsync method and passing in the user object and the name of the role. This ensures that every new user will have the permissions and access associated with the "Standard" role in the application. // Runs the Identity operation asynchronously.

                    var userId = await _userManager.GetUserIdAsync(user); // Runs the Identity operation asynchronously.
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Generates an Identity token for an account action.
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)); // Sets code for the current page flow.
                    // ----- Redirects and Results -----
                    var callbackUrl = Url.Page( // Builds a URL to another Razor Page.
                        // ----- Email Logic -----
                        "/Account/ConfirmEmail", // Confirms the user's email address.
                        pageHandler: null, // Performs this page model step for the current request.
                        // ----- Redirects and Results -----
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl }, // Sets area for the current page flow.
                        protocol: Request.Scheme); // Performs this page model step for the current request.
                    // ----- Email Logic -----
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", // Sends an account email to the user.
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); // Encodes user-facing URL or HTML content safely.
                    // ----- Injected Services -----
                    if (_userManager.Options.SignIn.RequireConfirmedAccount) // Checks the condition before continuing this page flow.
                    {
                        // ----- Email Logic -----
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl }); // Redirects the browser after completing this step.
                    }
                    else // Handles the fallback branch for the previous condition.
                    {
                        // ----- Injected Services -----
                        await _signInManager.SignInAsync(user, isPersistent: false); // Signs the user in after the account action.
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

            // If we got this far, something failed, redisplay form
            // ----- Redirects and Results -----
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
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml"); // Performs this page model step for the current request.
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
