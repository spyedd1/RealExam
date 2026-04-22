// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

        // ----- DOM references -----
        var cancelAllForm = document.getElementById('cancelAllForm'); // Stores the cancelAllForm DOM element reference.
        if (cancelAllForm) { // Checks the condition before running the next script step.
            // ----- Event wiring -----
            cancelAllForm.addEventListener('submit', function (e) { // Registers an event handler for user or browser interaction.
                // ----- Order logic -----
                if (!confirm('Cancel your entire slice for order #' + (cancelAllForm.dataset.orderId || '') + '? This cannot be undone.')) { // Checks the condition before running the next script step.
                    e.preventDefault(); // Stops the browser's default action for this event.
                }
            });
        }
    }());
