// ----- Icon setup -----
lucide.createIcons(); // Runs this JavaScript step for the page interaction.
        // ----- DOM references -----
        document.documentElement.classList.add('js-loaded'); // Adds a CSS class to update the element state.

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
            // ----- Basket logic -----
            window.updateBasketBadge = function (count) { // Starts a JavaScript block for the current control flow.
                // ----- DOM references -----
                var badge = document.getElementById('navBasketCount'); // Stores the badge DOM element reference.
                if (!badge) return; // Checks the condition before running the next script step.
                badge.textContent = count > 99 ? '99+' : count; // Updates badge.textContent for the current script state.
                badge.hidden = count < 1; // Updates badge.hidden for the current script state.
                // ----- Basket logic -----
                badge.classList.remove('pw-basket-badge--pop'); // Removes a CSS class to update the element state.
                void badge.offsetWidth; // Runs this JavaScript step for the page interaction.
                badge.classList.add('pw-basket-badge--pop'); // Adds a CSS class to update the element state.
                setTimeout(function () { badge.classList.remove('pw-basket-badge--pop'); }, 380); // Removes a CSS class to update the element state.
            };
            fetch('/BasketProducts/GetCount') // Sends an asynchronous request to the server.
                // ----- Helpers -----
                .then(function (r) { return r.json(); }) // Handles the next step after the async request completes.
                // ----- Basket logic -----
                .then(function (d) { window.updateBasketBadge(d.count); }) // Handles the next step after the async request completes.
                // ----- Helpers -----
                .catch(function () {}); // Handles errors from the async request.
        })();
