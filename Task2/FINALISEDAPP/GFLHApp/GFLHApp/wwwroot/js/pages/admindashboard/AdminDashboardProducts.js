// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.
        // ----- DOM references -----
        var token = document.querySelector('input[name="__RequestVerificationToken"]'); // Stores the token DOM element reference.

        document.querySelectorAll('.adm-toggle-input').forEach(function (cb) { // Finds a page element needed by the script.
            // ----- Event wiring -----
            cb.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
                // ----- Product logic -----
                var id = cb.dataset.productId; // Stores the id value for later script logic.
                // ----- State updates -----
                cb.disabled = true; // Updates cb.disabled for the current script state.
                // ----- Helpers -----
                var fd = new FormData(); // Creates the fd object used by the script.
                fd.append('id', id); // Runs this JavaScript step for the page interaction.
                // ----- State updates -----
                if (token) fd.append('__RequestVerificationToken', token.value); // Checks the condition before running the next script step.
                // ----- Product logic -----
                fetch('/AdminDashboard/ToggleProductAvailability', { // Sends an asynchronous request to the server.
                    method: 'POST', // Runs this JavaScript step for the page interaction.
                    // ----- State updates -----
                    headers: { 'RequestVerificationToken': token ? token.value : '' }, // Runs this JavaScript step for the page interaction.
                    body: fd // Runs this JavaScript step for the page interaction.
                // ----- Helpers -----
                }).then(function (r) { return r.json(); }) // Handles the next step after the async request completes.
                  .then(function (d) { if (!d.success) cb.checked = !cb.checked; cb.disabled = false; }) // Handles the next step after the async request completes.
                  .catch(function () { cb.checked = !cb.checked; cb.disabled = false; }); // Handles errors from the async request.
            });
        });
    }());
