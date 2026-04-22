// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
        // ----- Strict mode -----
        'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.
        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

        // ----- DOM references -----
        var tabs = document.querySelectorAll('.pw-tab'); // Stores the tabs DOM element reference.
        var cards = document.querySelectorAll('.pw-order-card'); // Stores the cards DOM element reference.

        // ----- Accessibility -----
        tabs.forEach(function (tab) { // Starts a JavaScript block for the current control flow.
            // ----- Event wiring -----
            tab.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
                // ----- Accessibility -----
                tabs.forEach(function (t) { t.classList.remove('is-active'); }); // Removes a CSS class to update the element state.
                tab.classList.add('is-active'); // Adds a CSS class to update the element state.

                var filter = tab.dataset.filter; // Stores the filter value for later script logic.
                // ----- Helpers -----
                cards.forEach(function (card) { // Starts a JavaScript block for the current control flow.
                    // ----- State updates -----
                    if (filter === 'all' || card.dataset.status === filter) { // Checks the condition before running the next script step.
                        card.style.display = ''; // Updates inline style for a dynamic UI state.
                    } else { // Starts a JavaScript block for the current control flow.
                        card.style.display = 'none'; // Updates inline style for a dynamic UI state.
                    }
                });
            });
        });
    }());
