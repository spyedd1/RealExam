// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable // Performs this page model step for the current request.

// ----- Imports -----
using System.Diagnostics; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Authorization; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc; // Imports a namespace needed by this page model.
using Microsoft.AspNetCore.Mvc.RazorPages; // Imports a namespace needed by this page model.

// ----- Namespace -----
namespace GFLHApp.Areas.Identity.Pages // Places this page model in the Identity area namespace.
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous] // Applies metadata or validation to the following member.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Applies metadata or validation to the following member.
    // ----- Page Model Declaration -----
    public class ErrorModel : PageModel // Defines the Razor Page model class for this page.
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string RequestId { get; set; } // Performs this page model step for the current request.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Receives services from dependency injection.

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        // ----- Page Handlers -----
        public void OnGet() // Receives services from dependency injection.
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier; // Sets RequestId for the current page flow.
        }
    }
}
