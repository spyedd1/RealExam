// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
			// ----- Icon setup -----
			if (window.lucide) lucide.createIcons(); // Initialize lucide icons in case they haven't been rendered yet (e.g. if using partial views or dynamic content) // Checks the condition before running the next script step.

			// ----- DOM references -----
			var checkbox  = document.getElementById('confirmDelete'); // Get the confirmation checkbox element // Stores the checkbox DOM element reference.
			var deleteBtn = document.getElementById('deleteBtn'); // Get the delete button element // Stores the deleteBtn DOM element reference.

			// ----- Form validation -----
			if (checkbox && deleteBtn) { // Check if both elements exist to avoid errors // Checks the condition before running the next script step.
				// ----- Event wiring -----
				checkbox.addEventListener('change', function () { // Add event listener for when the checkbox state changes // Registers an event handler for user or browser interaction.
					// ----- Helpers -----
					var ready = checkbox.checked; // Determine if the checkbox is checked (true) or not (false) // Stores the ready value for later script logic.
					// ----- State updates -----
					deleteBtn.disabled = !ready; // Enable the delete button if ready is true, otherwise disable it // Updates deleteBtn.disabled for the current script state.
					// ----- Accessibility -----
					deleteBtn.setAttribute('aria-disabled', String(!ready)); // Update aria-disabled attribute for accessibility to reflect the current state of the button // Sets an attribute required by the UI state.
                    // ----- State updates -----
                    deleteBtn.classList.toggle('is-ready', ready); // Toggles a CSS class based on the current state.
        });
    }
}());
