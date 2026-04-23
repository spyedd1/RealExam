// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class LogoutModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly SignInManager<IdentityUser> _signInManager; // Stores an injected service used by the page model.
        private readonly ILogger<LogoutModel> _logger; // Stores an injected service used by the page model.

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger) // Receives services from dependency injection.
        {
            _signInManager = signInManager; // Sets _signInManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnPost(string returnUrl = null) // Handles POST requests submitted from this page.
        {
            // ----- Injected Services -----
            await _signInManager.SignOutAsync(); // Signs the current user out.
            _logger.LogInformation("User logged out."); // Writes account flow information to the application log.
            // ----- Redirects and Results -----
            if (returnUrl != null) // Checks the condition before continuing this page flow.
            {
                return LocalRedirect(returnUrl); // Redirects the browser after completing this step.
            }
            else // Handles the fallback branch for the previous condition.
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage(); // Redirects the browser after completing this step.
            }
        }
    }
}
