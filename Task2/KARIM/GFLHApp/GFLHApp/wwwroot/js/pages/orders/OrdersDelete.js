// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- DOM references -----
    var checkbox = document.getElementById('confirmDeleteOrder'); // Stores the checkbox DOM element reference.
    var button = document.getElementById('deleteOrderButton'); // Stores the button DOM element reference.
    var form = document.getElementById('orderDeleteForm'); // Stores the form DOM element reference.

    // ----- Helpers -----
    function syncButton() { // Defines the syncButton helper function.
        if (!checkbox || !button) return; // Checks the condition before running the next script step.
        // ----- State updates -----
        button.disabled = !checkbox.checked; // Updates button.disabled for the current script state.
        button.classList.toggle('is-ready', checkbox.checked); // Toggles a CSS class based on the current state.
    }

    if (checkbox && button) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        checkbox.addEventListener('change', syncButton); // Registers an event handler for user or browser interaction.
        syncButton(); // Runs this JavaScript step for the page interaction.
    }

    if (form && button) { // Checks the condition before running the next script step.
        form.addEventListener('submit', function (event) { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            if (button.disabled) { // Checks the condition before running the next script step.
                event.preventDefault(); // Stops the browser's default action for this event.
                return; // Returns the computed value to the caller.
            }

            // ----- Accessibility -----
            button.setAttribute('aria-busy', 'true'); // Sets an attribute required by the UI state.
        });
    }
}());
