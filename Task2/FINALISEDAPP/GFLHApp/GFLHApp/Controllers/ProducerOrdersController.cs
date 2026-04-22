// ----- Imports -----
using System; // Provides core .NET types used by this controller.
using System.Collections.Generic; // Provides collection types such as lists and dictionaries.
using System.Linq; // Provides LINQ filtering, grouping, and sorting helpers.
using System.Threading.Tasks; // Provides asynchronous Task return types.
using Microsoft.AspNetCore.Mvc; // Provides MVC controller, action result, and response helpers.
using Microsoft.AspNetCore.Mvc.Rendering; // Provides SelectList helpers for form dropdowns.
using Microsoft.EntityFrameworkCore; // Provides Entity Framework Core query and save APIs.
using GFLHApp.Data; // Provides the application database context.
using GFLHApp.Models; // Provides the MVC model classes used by this controller.
using Microsoft.AspNetCore.Authorization; // Provides role-based authorization attributes.

// ----- Namespace -----
namespace GFLHApp.Controllers // Places these MVC controller types in the application controllers namespace.
{
    // Role management
    [Authorize(Roles = "Admin,Developer")] // Restricts access to users in the Admin,Developer role set.

    // ----- Controller Declaration -----
    public class ProducerOrdersController : Controller // Defines the MVC controller for producer order slice administration.
    {

        // ----- Controller Dependencies -----
        private readonly ApplicationDbContext _context; // Holds the injected database context for queries and saves.

        // Producer order splitting
        public ProducerOrdersController(ApplicationDbContext context) // Receives dependencies from dependency injection and stores them on the controller.
        {
            _context = context; // Stores the injected dependency on the controller field.
        }

        // ----- Listing and Dashboard Actions -----
        public async Task<IActionResult> Index() // Loads the main listing or dashboard view for this controller
        {
            var applicationDbContext = _context.ProducerOrders.Include(p => p.Orders); // Includes related records needed by the view or workflow.
            return View(await applicationDbContext.ToListAsync()); // Renders the matching view with the supplied model data.
        }

        // ----- Details Actions -----
        public async Task<IActionResult> Details(int? id) // Loads one record and sends it to the details view
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer order splitting
            var producerOrders = await _context.ProducerOrders // Sets producerOrders to the value needed by the workflow.
                .Include(p => p.Orders) // Includes related records needed by the view or workflow.
                // Order logic
                .FirstOrDefaultAsync(m => m.ProducerOrdersId == id); // Fetches the first matching record or null if none exists.
            if (producerOrders == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer order splitting
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        // ----- Create Actions -----
        // Create record
        public IActionResult Create() // Shows or processes the create form for a record
        {
            // Order logic
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId"); // Supplies dropdown data to the view.
            return View(); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        public async Task<IActionResult> Create([Bind("ProducerOrdersId,OrdersId,ProducerId,ProducerSubtotal,TrackingStatus")] ProducerOrders producerOrders) // Shows or processes the create form for a record
        {
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                // Producer order splitting
                _context.Add(producerOrders); // Queues the new entity to be inserted into the database.
                await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
            }
            // Order logic
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId", producerOrders.OrdersId); // Supplies dropdown data to the view.
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        // ----- Edit Actions -----
        // Edit record
        public async Task<IActionResult> Edit(int? id) // Shows or processes edits for an existing record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer order splitting
            var producerOrders = await _context.ProducerOrders.FindAsync(id); // Looks up the record by its primary key.
            if (producerOrders == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }
            // Order logic
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId", producerOrders.OrdersId); // Supplies dropdown data to the view.
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        [HttpPost] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        public async Task<IActionResult> Edit(int id, [Bind("ProducerOrdersId,OrdersId,ProducerId,ProducerSubtotal,TrackingStatus")] ProducerOrders producerOrders) // Shows or processes edits for an existing record
        {
            // Order logic
            if (id != producerOrders.ProducerOrdersId) // Checks the condition that controls the next action.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Form validation
            if (ModelState.IsValid) // Checks whether validation passed before changing data.
            {
                try // Starts a protected database update block.
                {
                    // Producer order splitting
                    _context.Update(producerOrders); // Marks the entity as modified so EF Core saves its changes.
                    await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
                }
                catch (DbUpdateConcurrencyException) // Handles database concurrency errors from the update attempt.
                {
                    // Order logic
                    if (!ProducerOrdersExists(producerOrders.ProducerOrdersId)) // Checks the condition that controls the next action.
                    {
                        return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
                    }
                    else // Handles the fallback branch when earlier conditions did not match.
                    {
                        throw; // Rethrows unexpected concurrency failures for higher-level handling.
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
            }
            // Order logic
            ViewData["OrdersId"] = new SelectList(_context.Orders, "OrdersId", "OrdersId", producerOrders.OrdersId); // Supplies dropdown data to the view.
            // Producer order splitting
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        // ----- Delete Actions -----
        public async Task<IActionResult> Delete(int? id) // Shows the delete confirmation view for a record
        {
            if (id == null) // Checks that the request included a record id.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer order splitting
            var producerOrders = await _context.ProducerOrders // Sets producerOrders to the value needed by the workflow.
                .Include(p => p.Orders) // Includes related records needed by the view or workflow.
                // Order logic
                .FirstOrDefaultAsync(m => m.ProducerOrdersId == id); // Fetches the first matching record or null if none exists.
            if (producerOrders == null) // Checks whether the requested data was found.
            {
                return NotFound(); // Returns 404 when the requested record is missing or inaccessible.
            }

            // Producer order splitting
            return View(producerOrders); // Renders the matching view with the supplied model data.
        }

        [HttpPost, ActionName("Delete")] // Marks the following action as handling POST form submissions.
        // Form validation
        [ValidateAntiForgeryToken] // Requires a valid anti-forgery token for the form post.
        // Delete record
        public async Task<IActionResult> DeleteConfirmed(int id) // Removes the confirmed record and returns to the listing
        {
            // Producer order splitting
            var producerOrders = await _context.ProducerOrders.FindAsync(id); // Looks up the record by its primary key.
            if (producerOrders != null) // Runs the next step only when the record exists.
            {
                _context.ProducerOrders.Remove(producerOrders); // Queues the entity for removal from the database.
            }

            await _context.SaveChangesAsync(); // Persists pending database changes asynchronously.
            return RedirectToAction(nameof(Index)); // Redirects the browser to the next MVC action.
        }

        // ----- Existence Helpers -----
        // Order logic
        private bool ProducerOrdersExists(int id) // Checks whether a producer order slice still exists
        {
            return _context.ProducerOrders.Any(e => e.ProducerOrdersId == id); // Returns the computed result to the caller.
        }
    }
}
