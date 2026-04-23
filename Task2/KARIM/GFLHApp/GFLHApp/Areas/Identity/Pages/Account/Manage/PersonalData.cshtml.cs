// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class PersonalDataModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly ILogger<PersonalDataModel> _logger; // Stores an injected service used by the page model.

        // ----- Personal Data -----
        public PersonalDataModel( // Performs this page model step for the current request.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            ILogger<PersonalDataModel> logger) // Writes account flow information to the application log.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        // ----- Page Handlers -----
        public async Task<IActionResult> OnGet() // Handles GET requests that display this page.
        {
            // ----- Injected Services -----
            var user = await _userManager.GetUserAsync(User); // Looks up the Identity user needed by this request.
            if (user == null) // Checks the condition before continuing this page flow.
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); // Returns a not-found result when user data is missing.
            }

            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }
    }
}
