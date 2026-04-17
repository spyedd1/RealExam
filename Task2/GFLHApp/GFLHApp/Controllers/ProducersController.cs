using GFLHApp.Data;
using GFLHApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GFLHApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProducersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProducersController(ApplicationDbContext context,
                                   UserManager<IdentityUser> userManager,
                                   RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Producers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Producers.ToListAsync());
        }

        // GET: Producers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producers = await _context.Producers
                .FirstOrDefaultAsync(m => m.ProducersId == id);

            if (producers == null) return NotFound();

            return View(producers);
        }

        // GET: Producers/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Producers/Create

        
        [HttpPost]
        [ValidateAntiForgeryToken]

        
        public async Task<IActionResult> Create(
            [Bind("ProducersId,ProducerName,ProducerEmail,ProducerInformation,IsVATRegistered,VATNumber")]
            Producers producers,
            string AccountPassword) // Bind the form fields to the Producers model and also accept an additional parameter for the account password, which is not part of the Producers model but is needed to create the associated Identity user account. 
        {
            
            

            ModelState.Remove("UserId"); // Remove validation for UserId since it will be set programmatically after creating the Identity user account. This prevents model validation from failing due to the UserId being null at this point.
            ModelState.Remove("ProducerOrders"); // Remove validation for ProducerOrders since it's a navigation property and not part of the form input. This prevents model validation from failing due to ProducerOrders being null at this point.

            if (!ModelState.IsValid) // If the model state is invalid, log the validation errors to the debug output and return the view with the current producers object to display validation messages to the user.
            {
                foreach (var kvp in ModelState) // Iterate through the model state dictionary to find any validation errors and log them for debugging purposes. This can help identify why the model state is invalid when testing the form submission.
                    foreach (var err in kvp.Value.Errors) // For each entry in the model state, check if there are any errors and log the error messages along with the corresponding field name (key) to the debug output.
                        System.Diagnostics.Debug.WriteLine($"{kvp.Key}: {err.ErrorMessage}"); // Log the field name (key) and the associated error message to the debug output. This can help developers understand which fields are causing validation issues when the form is submitted with invalid data.
                return View(producers); // Return the view with the current producers object to display validation messages to the user. This allows the user to correct any errors in the form and resubmit it.
            }

            // --- Uniqueness checks ----

            bool emailInUse = await _context.Producers // 
                .AnyAsync(p => p.ProducerEmail.ToLower() == producers.ProducerEmail.ToLower());
            if (emailInUse)
                ModelState.AddModelError("ProducerEmail", "This email address is already registered to a producer.");

            bool nameInUse = await _context.Producers
                .AnyAsync(p => p.ProducerName.ToLower() == producers.ProducerName.ToLower());
            if (nameInUse)
                ModelState.AddModelError("ProducerName", "This producer name is already taken.");

            if (producers.IsVATRegistered && !string.IsNullOrWhiteSpace(producers.VATNumber))
            {
                bool vatInUse = await _context.Producers
                    .AnyAsync(p => p.VATNumber == producers.VATNumber);
                if (vatInUse)
                    ModelState.AddModelError("VATNumber", "This VAT number is already registered to another producer.");
            }

            // --- Password validation ---
            if (string.IsNullOrWhiteSpace(AccountPassword))
                ModelState.AddModelError("AccountPassword", "A password is required.");

            if (!ModelState.IsValid)
                return View(producers);

            // --- Create the Identity user account ---
            var identityUser = new IdentityUser
            {
                UserName = producers.ProducerEmail,
                Email = producers.ProducerEmail,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(identityUser, AccountPassword);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("AccountPassword", error.Description);
                return View(producers);
            }

            // --- Ensure Producer role exists, then assign it ---
            if (!await _roleManager.RoleExistsAsync("Producer"))
                await _roleManager.CreateAsync(new IdentityRole("Producer"));

            await _userManager.AddToRoleAsync(identityUser, "Producer");

            // --- Link the new Identity userId to the producer record ---
            producers.UserId = identityUser.Id;

            _context.Add(producers);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Producers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producers = await _context.Producers.FindAsync(id);
            if (producers == null) return NotFound();

            return View(producers);
        }

        // POST: Producers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ProducersId,UserId,ProducerName,ProducerEmail,ProducerInformation,IsVATRegistered,VATNumber")]
            Producers producers)
        {
            if (id != producers.ProducersId) return NotFound();

            // Uniqueness checks excluding the current record
            bool emailInUse = await _context.Producers
                .AnyAsync(p => p.ProducerEmail.ToLower() == producers.ProducerEmail.ToLower()
                            && p.ProducersId != id);
            if (emailInUse)
                ModelState.AddModelError("ProducerEmail", "This email address is already registered to another producer.");

            bool nameInUse = await _context.Producers
                .AnyAsync(p => p.ProducerName.ToLower() == producers.ProducerName.ToLower()
                            && p.ProducersId != id);
            if (nameInUse)
                ModelState.AddModelError("ProducerName", "This producer name is already taken.");

            if (producers.IsVATRegistered && !string.IsNullOrWhiteSpace(producers.VATNumber))
            {
                bool vatInUse = await _context.Producers
                    .AnyAsync(p => p.VATNumber == producers.VATNumber
                               && p.ProducersId != id);
                if (vatInUse)
                    ModelState.AddModelError("VATNumber", "This VAT number is already registered to another producer.");
            }

            if (!ModelState.IsValid) return View(producers);

            try
            {
                _context.Update(producers);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProducersExists(producers.ProducersId)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Producers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producers = await _context.Producers
                .FirstOrDefaultAsync(m => m.ProducersId == id);

            if (producers == null) return NotFound();

            return View(producers);
        }

        // POST: Producers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producers = await _context.Producers.FindAsync(id);
            if (producers != null)
                _context.Producers.Remove(producers);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProducersExists(int id)
        {
            return _context.Producers.Any(e => e.ProducersId == id);
        }
    }
}