// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var sections = document.querySelectorAll('.prv-section[id]'); // Stores the sections DOM element reference.
    var links    = document.querySelectorAll('#prv-toc-list .prv-toc__link'); // Stores the links DOM element reference.
    if (!sections.length || !links.length) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    var observer = new IntersectionObserver(function (entries) { // Creates the observer object used by the script.
        entries.forEach(function (entry) { // Starts a JavaScript block for the current control flow.
            if (!entry.isIntersecting) return; // Checks the condition before running the next script step.
            var id = entry.target.id; // Stores the id value for later script logic.
            links.forEach(function (link) { // Starts a JavaScript block for the current control flow.
                // ----- State updates -----
                link.classList.toggle('is-active', link.getAttribute('href') === '#' + id); // Toggles a CSS class based on the current state.
            });
        });
    }, { rootMargin: '-20% 0px -60% 0px' }); // Runs this JavaScript step for the page interaction.

    // ----- Helpers -----
    sections.forEach(function (s) { observer.observe(s); }); // Runs this JavaScript step for the page interaction.
}());
