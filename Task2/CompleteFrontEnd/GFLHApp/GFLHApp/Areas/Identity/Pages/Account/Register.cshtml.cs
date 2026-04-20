// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace GFLHApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel // This class represents the model for the registration page in the ASP.NET Core Identity system. It handles the logic for registering a new user, including creating the user, assigning roles, and sending confirmation emails.
    {
        private readonly SignInManager<IdentityUser> _signInManager; // This is a service that provides methods for signing in users, including password sign-in and external login sign-in.
        private readonly UserManager<IdentityUser> _userManager; // This is a service that provides methods for managing users, including creating users, finding users, and managing user roles.
        private readonly IUserStore<IdentityUser> _userStore; // This is an interface that defines methods for storing and retrieving user information. It is used by the UserManager to interact with the underlying user store (e.g., a database).
        private readonly IUserEmailStore<IdentityUser> _emailStore; // This is an interface that defines methods for storing and retrieving user email information. It is used by the UserManager to manage user email addresses.
        private readonly ILogger<RegisterModel> _logger; // This is a logging service that allows the application to log information, warnings, and errors. It is used in this class to log when a new user account is created.
        private readonly IEmailSender _emailSender; // This is a service that provides methods for sending emails. It is used in this class to send confirmation emails to users after they register.
        private readonly RoleManager<IdentityRole> _roleManager; // This is a service that provides methods for managing roles, including creating roles and checking if roles exist. It is used in this class to assign the "Standard" role to new users.

        public RegisterModel( // This is the constructor for the RegisterModel class. It takes several services as parameters and assigns them to private fields for use in the class methods.
            UserManager<IdentityUser> userManager, // This parameter is the UserManager service, which is used to manage user accounts.
            IUserStore<IdentityUser> userStore, // This parameter is the IUserStore service, which is used to interact with the user store.
            SignInManager<IdentityUser> signInManager, // This parameter is the SignInManager service, which is used to handle user sign-in operations.
            ILogger<RegisterModel> logger, // This parameter is the ILogger service, which is used for logging information about the registration process. 
            IEmailSender emailSender, //    This parameter is the IEmailSender service, which is used to send confirmation emails to users after they register.
            RoleManager<IdentityRole> roleManager) // This parameter is the RoleManager service, which is used to manage user roles and assign roles to users.
        {
            _userManager = userManager; // Assign the UserManager service to the private field _userManager for use in the class methods. 
            _userStore = userStore; // Assign the IUserStore service to the private field _userStore for use in the class methods.
            _emailStore = GetEmailStore(); // Call the GetEmailStore method to get an instance of IUserEmailStore and assign it to the private field _emailStore for use in the class methods.
            _signInManager = signInManager; // Assign the SignInManager service to the private field _signInManager for use in the class methods.
            _logger = logger; // Assign the ILogger service to the private field _logger for use in the class methods to log information about the registration process.
            _emailSender = emailSender; // Assign the IEmailSender service to the private field _emailSender for use in the class methods to send confirmation emails to users after they register.
            _roleManager = roleManager;  // Assign the RoleManager service to the private field _roleManager for use in the class methods to manage user roles and assign roles to users.
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) // This method is called when the user submits the registration form. It handles the logic for creating a new user account, assigning roles, and sending confirmation emails. It also handles validation and error handling for the registration process.
        {
            returnUrl ??= Url.Content("~/"); // If the returnUrl parameter is null, set it to the root URL of the application. This is used to redirect the user after successful registration.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Get the list of external authentication schemes (e.g., Google, Facebook) and assign it to the ExternalLogins property for use in the registration page.
            if (ModelState.IsValid) // Check if the model state is valid, which means that all required fields are filled out correctly and any validation attributes are satisfied. If the model state is not valid, the registration form will be redisplayed with validation error messages.
            {
                var user = CreateUser(); // Create a new instance of the IdentityUser class using the CreateUser method. This represents the new user account that will be created.
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None); // Set the username of the new user to the email address provided in the registration form using the IUserStore service.
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // Set the email address of the new user to the email address provided in the registration form using the IUserEmailStore service.
                var result = await _userManager.CreateAsync(user, Input.Password); // Create the new user account using the UserManager service, passing in the user object and the password provided in the registration form. The result of this operation is stored in the result variable, which indicates whether the user creation was successful and contains any errors that occurred during the process.
                if (result.Succeeded) // Check if the user creation was successful. If it was, proceed with assigning roles and sending confirmation emails. If it was not successful, the registration form will be redisplayed with error messages.
                {
                    _logger.LogInformation("User created a new account with password."); // Log an informational message indicating that a new user account was created successfully using the ILogger service.

                    //  Assign "Standard" role to every new user 
                    if (!await _roleManager.RoleExistsAsync("Standard")) // Check if the "Standard" role already exists using the RoleManager service. If it does not exist, create the role.
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Standard")); // Create the "Standard" role using the RoleManager service by passing in a new instance of the IdentityRole class with the name "Standard". This ensures that every new user will be assigned to the "Standard" role, which can be used for authorization and access control in the application.
                    }
                    await _userManager.AddToRoleAsync(user, "Standard"); // Assign the newly created user to the "Standard" role using the UserManager service by calling the AddToRoleAsync method and passing in the user object and the name of the role. This ensures that every new user will have the permissions and access associated with the "Standard" role in the application.

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
