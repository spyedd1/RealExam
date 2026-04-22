// ----- Icon setup -----
if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var sidebar = document.getElementById('adminSidebar'); // Stores the sidebar DOM element reference.
    var overlay = document.getElementById('adminOverlay'); // Stores the overlay DOM element reference.
    var hamburger = document.getElementById('adminHamburger'); // Stores the hamburger DOM element reference.

    if (!sidebar || !overlay || !hamburger) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    function open() { // Defines the open helper function.
        // ----- State updates -----
        sidebar.classList.add('is-open'); // Adds a CSS class to update the element state.
        overlay.classList.add('is-open'); // Adds a CSS class to update the element state.
        // ----- Accessibility -----
        hamburger.setAttribute('aria-expanded', 'true'); // Sets an attribute required by the UI state.
    }

    // ----- Helpers -----
    function close() { // Defines the close helper function.
        // ----- State updates -----
        sidebar.classList.remove('is-open'); // Removes a CSS class to update the element state.
        overlay.classList.remove('is-open'); // Removes a CSS class to update the element state.
        // ----- Accessibility -----
        hamburger.setAttribute('aria-expanded', 'false'); // Sets an attribute required by the UI state.
    }

    // ----- Event wiring -----
    hamburger.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
        // ----- State updates -----
        sidebar.classList.contains('is-open') ? close() : open(); // Runs this JavaScript step for the page interaction.
    });
    // ----- Event wiring -----
    overlay.addEventListener('click', close); // Registers an event handler for user or browser interaction.
    // ----- DOM references -----
    document.addEventListener('keydown', function (e) { // Registers an event handler for user or browser interaction.
        if (e.key === 'Escape') close(); // Checks the condition before running the next script step.
    });
}());
