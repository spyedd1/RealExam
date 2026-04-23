// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- DOM references -----
    var deliveryToggle = document.querySelector('[data-fulfilment-toggle="delivery"]'); // Stores the deliveryToggle DOM element reference.
    var collectionToggle = document.querySelector('[data-fulfilment-toggle="collection"]'); // Stores the collectionToggle DOM element reference.
    var panels = Array.prototype.slice.call(document.querySelectorAll('[data-conditional-panel]')); // Stores the panels DOM element reference.
    var form = document.getElementById('orderEditForm'); // Stores the form DOM element reference.

    // ----- Helpers -----
    function setPanelState() { // Defines the setPanelState helper function.
        // ----- Order logic -----
        var deliveryOn = deliveryToggle && deliveryToggle.checked; // Stores the deliveryOn value for later script logic.
        var collectionOn = collectionToggle && collectionToggle.checked; // Stores the collectionOn value for later script logic.

        // ----- Helpers -----
        panels.forEach(function (panel) { // Starts a JavaScript block for the current control flow.
            var type = panel.getAttribute('data-conditional-panel'); // Stores the type value for later script logic.
            // ----- Order logic -----
            var active = type === 'delivery' ? deliveryOn : collectionOn; // Stores the active value for later script logic.
            // ----- State updates -----
            panel.classList.toggle('is-muted', !active); // Toggles a CSS class based on the current state.
            // ----- Accessibility -----
            panel.setAttribute('aria-hidden', active ? 'false' : 'true'); // Sets an attribute required by the UI state.
        });
    }

    // ----- Helpers -----
    function bindExclusive(source, target) { // Defines the bindExclusive helper function.
        if (!source || !target) return; // Checks the condition before running the next script step.
        // ----- Event wiring -----
        source.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            if (source.checked) target.checked = false; // Checks the condition before running the next script step.
            if (!source.checked && !target.checked) source.checked = true; // Checks the condition before running the next script step.
            setPanelState(); // Runs this JavaScript step for the page interaction.
        });
    }

    // ----- Order logic -----
    bindExclusive(deliveryToggle, collectionToggle); // Runs this JavaScript step for the page interaction.
    bindExclusive(collectionToggle, deliveryToggle); // Runs this JavaScript step for the page interaction.
    setPanelState(); // Runs this JavaScript step for the page interaction.

    if (form) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        form.addEventListener('submit', function () { // Registers an event handler for user or browser interaction.
            // ----- DOM references -----
            var submit = form.querySelector('button[type="submit"]'); // Stores the submit DOM element reference.
            // ----- Event wiring -----
            if (!submit) return; // Checks the condition before running the next script step.
            submit.classList.add('is-saving'); // Adds a CSS class to update the element state.
            submit.setAttribute('aria-busy', 'true'); // Sets an attribute required by the UI state.
        });
    }
}());
