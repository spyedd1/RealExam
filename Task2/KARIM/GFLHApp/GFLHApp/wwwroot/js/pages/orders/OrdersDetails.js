// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- DOM references -----
    var hero = document.querySelector('.od-hero'); // Stores the hero DOM element reference.
    var floats = Array.prototype.slice.call(document.querySelectorAll('.od-hero__float')); // Stores the floats DOM element reference.
    // ----- Helpers -----
    var depths = [18, 12, 24]; // Stores the depths value for later script logic.
    var targetX = 0; // Stores the targetX value for later script logic.
    var targetY = 0; // Stores the targetY value for later script logic.
    var currentX = 0; // Stores the currentX value for later script logic.
    var currentY = 0; // Stores the currentY value for later script logic.
    var raf = null; // Stores the raf value for later script logic.

    function lerp(a, b, t) { // Defines the lerp helper function.
        return a + (b - a) * t; // Returns the computed value to the caller.
    }

    function draw() { // Defines the draw helper function.
        currentX = lerp(currentX, targetX, 0.08); // Updates currentX for the current script state.
        currentY = lerp(currentY, targetY, 0.08); // Updates currentY for the current script state.

        floats.forEach(function (el, index) { // Starts a JavaScript block for the current control flow.
            var depth = depths[index] || 14; // Stores the depth value for later script logic.
            // ----- State updates -----
            el.style.setProperty('--od-x', (currentX * depth).toFixed(2) + 'px'); // Updates inline style for a dynamic UI state.
            el.style.setProperty('--od-y', (currentY * depth).toFixed(2) + 'px'); // Updates inline style for a dynamic UI state.
        });

        if (Math.abs(currentX - targetX) > 0.002 || Math.abs(currentY - targetY) > 0.002) { // Checks the condition before running the next script step.
            // ----- Animation -----
            raf = requestAnimationFrame(draw); // Schedules smooth visual updates for the next animation frame.
        } else { // Starts a JavaScript block for the current control flow.
            raf = null; // Updates raf for the current script state.
        }
    }

    // ----- Helpers -----
    function requestDraw() { // Defines the requestDraw helper function.
        // ----- Animation -----
        if (!raf) raf = requestAnimationFrame(draw); // Checks the condition before running the next script step.
    }

    if (hero && floats.length) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        hero.addEventListener('mousemove', function (event) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var rect = hero.getBoundingClientRect(); // Stores the rect value for later script logic.
            targetX = (event.clientX - rect.left) / rect.width - 0.5; // Updates targetX for the current script state.
            targetY = (event.clientY - rect.top) / rect.height - 0.5; // Updates targetY for the current script state.
            requestDraw(); // Runs this JavaScript step for the page interaction.
        }, { passive: true }); // Runs this JavaScript step for the page interaction.

        // ----- Event wiring -----
        hero.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            targetX = 0; // Updates targetX for the current script state.
            targetY = 0; // Updates targetY for the current script state.
            requestDraw(); // Runs this JavaScript step for the page interaction.
        }, { passive: true }); // Runs this JavaScript step for the page interaction.
    }

    // ----- DOM references -----
    document.querySelectorAll('.od-panel').forEach(function (panel) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        panel.addEventListener('mouseenter', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            panel.style.boxShadow = 'var(--shadow-md)'; // Updates inline style for a dynamic UI state.
        });

        // ----- Event wiring -----
        panel.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            panel.style.boxShadow = ''; // Updates inline style for a dynamic UI state.
        });
    });
}());
