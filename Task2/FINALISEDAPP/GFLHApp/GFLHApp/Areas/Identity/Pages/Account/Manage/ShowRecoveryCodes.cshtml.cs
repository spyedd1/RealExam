// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using Microsoft.AspNetCore.Identity; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.
using Microsoft.Extensions.Logging; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages.Account.Manage // Places this page model in the Identity area namespace.
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    // ----- Page Model Declaration -----
    public class ShowRecoveryCodesModel : PageModel // Defines the Razor Page model class for this page.
    {
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
        // ----- Page Handlers -----
        public IActionResult OnGet() // Receives services from dependency injection.
        {
            // ----- Authentication Logic -----
            if (RecoveryCodes == null || RecoveryCodes.Length == 0) // Handles two-factor recovery code validation.
            {
                return RedirectToPage("./TwoFactorAuthentication"); // Handles two-factor authentication state or flow.
            }

            // ----- Redirects and Results -----
            return Page(); // Renders the current Razor Page.
        }
    }
}
