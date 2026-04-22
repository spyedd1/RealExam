// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System; // Imports a namespace needed by this page model.
using System.Collections.Generic; // Imports a namespace needed by this page model.
using System.Linq; // Imports a namespace needed by this page model.
using System.Text; // Imports a namespace needed by this page model.
using System.Text.Json; // Imports a namespace needed by this page model.
using System.Threading.Tasks; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    // ----- Page Model Declaration -----
    public class DownloadPersonalDataModel : PageModel // Defines the Razor Page model class for this page.
    {
        // ----- Injected Services -----
        private readonly UserManager<IdentityUser> _userManager; // Stores an injected service used by the page model.
        private readonly ILogger<DownloadPersonalDataModel> _logger; // Stores an injected service used by the page model.

        // ----- Personal Data -----
        public DownloadPersonalDataModel( // Performs this page model step for the current request.
            // ----- Injected Services -----
            UserManager<IdentityUser> userManager, // Performs this page model step for the current request.
            ILogger<DownloadPersonalDataModel> logger) // Writes account flow information to the application log.
        {
            _userManager = userManager; // Sets _userManager for the current page flow.
            _logger = logger; // Sets _logger for the current page flow.
        }

        // ----- Page Handlers -----
        public IActionResult OnGet() // Receives services from dependency injection.
        {
            // ----- Redirects and Results -----
            return NotFound(); // Returns a not-found result when user data is missing.
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

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User)); // Writes account flow information to the application log.

            // Only include personal data for download
            // ----- Personal Data -----
            var personalData = new Dictionary<string, string>(); // Sets personalData for the current page flow.
            var personalDataProps = typeof(IdentityUser).GetProperties().Where( // Sets personalDataProps for the current page flow.
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute))); // Performs this page model step for the current request.
            foreach (var p in personalDataProps) // Loops through each item needed by the page flow.
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null"); // Performs this page model step for the current request.
            }

            // ----- Injected Services -----
            var logins = await _userManager.GetLoginsAsync(user); // Writes account flow information to the application log.
            foreach (var l in logins) // Loops through each item needed by the page flow.
            {
                // ----- Personal Data -----
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey); // Writes account flow information to the application log.
            }

            // ----- Injected Services -----
            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user)); // Runs the Identity operation asynchronously.

            // ----- Personal Data -----
            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json"); // Sets filename for the current page flow.
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json"); // Returns the result for this page handler.
        }
    }
}
