// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Icon setup -----
    if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

    // ----- DOM references -----
    var checkbox  = document.getElementById('confirmDelete'); // Stores the checkbox DOM element reference.
    var deleteBtn = document.getElementById('deleteBtn'); // Stores the deleteBtn DOM element reference.

    if (checkbox && deleteBtn) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        checkbox.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var ready = checkbox.checked; // Stores the ready value for later script logic.
            // ----- State updates -----
            deleteBtn.disabled = !ready; // Updates deleteBtn.disabled for the current script state.
            // ----- Accessibility -----
            deleteBtn.setAttribute('aria-disabled', String(!ready)); // Sets an attribute required by the UI state.
            // ----- State updates -----
            deleteBtn.classList.toggle('is-ready', ready); // Toggles a CSS class based on the current state.
        });
    }
}());
