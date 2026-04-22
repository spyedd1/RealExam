// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
        // ----- Strict mode -----
        'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

        // ----- DOM references -----
        var token = document.querySelector('input[name="__RequestVerificationToken"]'); // Stores the token DOM element reference.

        document.querySelectorAll('.pw-avail-toggle__input').forEach(function (checkbox) { // Finds a page element needed by the script.
            // ----- Event wiring -----
            checkbox.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
                // ----- Product logic -----
                var productId = checkbox.dataset.productId; // Stores the productId value for later script logic.
                var row       = checkbox.closest('.pw-product-row'); // Stores the row value for later script logic.

                // ----- Helpers -----
                var wasChecked = !checkbox.checked; // Stores the wasChecked value for later script logic.
                // ----- State updates -----
                checkbox.disabled = true; // Updates checkbox.disabled for the current script state.

                // ----- Helpers -----
                var formData = new FormData(); // Creates the formData object used by the script.
                // ----- Product logic -----
                formData.append('id', productId); // Loops through matching items for this script step.
                // ----- State updates -----
                if (token) formData.append('__RequestVerificationToken', token.value); // Checks the condition before running the next script step.

                // ----- Producer logic -----
                fetch('/ProducerDashboard/ToggleAvailability', { // Sends an asynchronous request to the server.
                    method: 'POST', // Runs this JavaScript step for the page interaction.
                    // ----- State updates -----
                    headers: { 'RequestVerificationToken': token ? token.value : '' }, // Runs this JavaScript step for the page interaction.
                    // ----- Ajax requests -----
                    body: formData // Runs this JavaScript step for the page interaction.
                })
                // ----- Helpers -----
                .then(function (r) { return r.json(); }) // Handles the next step after the async request completes.
                .then(function (d) { // Handles the next step after the async request completes.
                    if (!d.success) { // Checks the condition before running the next script step.
                        // ----- State updates -----
                        checkbox.checked = wasChecked; // Updates checkbox.checked for the current script state.
                    }
                    checkbox.disabled = false; // Updates checkbox.disabled for the current script state.
                })
                // ----- Helpers -----
                .catch(function () { // Handles errors from the async request.
                    // ----- State updates -----
                    checkbox.checked = wasChecked; // Updates checkbox.checked for the current script state.
                    checkbox.disabled = false; // Updates checkbox.disabled for the current script state.
                });
            });
        });

    }());
