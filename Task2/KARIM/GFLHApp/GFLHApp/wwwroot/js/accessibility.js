// ----- Strict mode -----
'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var root = document.documentElement; // Stores the root value for later script logic.
    // ----- Helpers -----
    var storage = window.localStorage; // Stores the storage value for later script logic.
    var fontSizeKey = 'gflh-font-size'; // Stores the fontSizeKey value for later script logic.
    var darkModeKey = 'gflh-dark-mode'; // Stores the darkModeKey value for later script logic.
    // ----- Accessibility -----
    var contrastKey = 'gflh-high-contrast'; // Stores the contrastKey value for later script logic.
    // ----- Helpers -----
    var defaultFontSize = 16; // Stores the defaultFontSize value for later script logic.
    var minFontSize = 14; // Stores the minFontSize value for later script logic.
    var maxFontSize = 22; // Stores the maxFontSize value for later script logic.
    var fontStep = 2; // Stores the fontStep value for later script logic.
    var isSpeaking = false; // Stores the isSpeaking value for later script logic.

    function setStoredValue(key, value) { // Defines the setStoredValue helper function.
        try { // Starts a JavaScript block for the current control flow.
            // ----- State updates -----
            storage.setItem(key, value); // Runs this JavaScript step for the page interaction.
        } catch (e) { } // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function getStoredValue(key) { // Defines the getStoredValue helper function.
        try { // Starts a JavaScript block for the current control flow.
            return storage.getItem(key); // Returns the computed value to the caller.
        } catch (e) { // Starts a JavaScript block for the current control flow.
            return null; // Returns the computed value to the caller.
        }
    }

    function setPressed(selector, pressed) { // Defines the setPressed helper function.
        // ----- DOM references -----
        document.querySelectorAll(selector).forEach(function (el) { // Finds a page element needed by the script.
            // ----- Accessibility -----
            el.setAttribute('aria-pressed', String(pressed)); // Sets an attribute required by the UI state.
        });
    }

    // ----- Helpers -----
    function setLabel(name, value) { // Defines the setLabel helper function.
        // ----- DOM references -----
        document.querySelectorAll('[data-accessibility-label="' + name + '"]').forEach(function (el) { // Finds a page element needed by the script.
            // ----- State updates -----
            el.textContent = value; // Updates el.textContent for the current script state.
        });
    }

    // ----- Helpers -----
    function refreshUi() { // Defines the refreshUi helper function.
        // ----- DOM references -----
        var isDark = document.body.classList.contains('dark-mode'); // Stores the isDark value for later script logic.
        var isHighContrast = document.body.classList.contains('high-contrast'); // Stores the isHighContrast value for later script logic.

        setPressed('[data-accessibility-action="dark-mode"]', isDark); // Updates setPressed('[data-accessibility-action for the current script state.
        // ----- Accessibility -----
        setPressed('[data-accessibility-action="high-contrast"]', isHighContrast); // Updates setPressed('[data-accessibility-action for the current script state.
        setPressed('[data-accessibility-action="speak"]', isSpeaking); // Updates setPressed('[data-accessibility-action for the current script state.

        setLabel('dark-mode', isDark ? 'Light mode' : 'Dark mode'); // Runs this JavaScript step for the page interaction.
        setLabel('high-contrast', isHighContrast ? 'Normal contrast' : 'High contrast'); // Runs this JavaScript step for the page interaction.
        setLabel('speak', isSpeaking ? 'Stop reading' : 'Read page aloud'); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function applyStoredPreferences() { // Defines the applyStoredPreferences helper function.
        // ----- State updates -----
        if (getStoredValue(darkModeKey) === 'true' || getStoredValue('theme') === 'dark') { // Checks the condition before running the next script step.
            // ----- DOM references -----
            document.body.classList.add('dark-mode'); // Adds a CSS class to update the element state.
        }

        // ----- Accessibility -----
        if (getStoredValue(contrastKey) === 'true' || getStoredValue('contrastMode') === 'high-contrast') { // Checks the condition before running the next script step.
            // ----- DOM references -----
            document.body.classList.add('high-contrast'); // Adds a CSS class to update the element state.
        }

        // ----- Helpers -----
        var savedFontSize = parseInt(getStoredValue(fontSizeKey) || getStoredValue('fontSize'), 10); // Stores the savedFontSize value for later script logic.
        if (!Number.isNaN(savedFontSize)) { // Checks the condition before running the next script step.
            setFontSize(savedFontSize, false); // Runs this JavaScript step for the page interaction.
        }

        refreshUi(); // Runs this JavaScript step for the page interaction.
    }

    function toggleDarkMode() { // Defines the toggleDarkMode helper function.
        // ----- DOM references -----
        document.body.classList.toggle('dark-mode'); // Toggles a CSS class based on the current state.
        setStoredValue(darkModeKey, String(document.body.classList.contains('dark-mode'))); // Runs this JavaScript step for the page interaction.
        refreshUi(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Accessibility -----
    function toggleHighContrast() { // Defines the toggleHighContrast helper function.
        // ----- DOM references -----
        document.body.classList.toggle('high-contrast'); // Toggles a CSS class based on the current state.
        setStoredValue(contrastKey, String(document.body.classList.contains('high-contrast'))); // Runs this JavaScript step for the page interaction.
        refreshUi(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function setFontSize(size, persist) { // Defines the setFontSize helper function.
        var nextSize = Math.max(minFontSize, Math.min(maxFontSize, size)); // Stores the nextSize value for later script logic.

        // ----- State updates -----
        root.style.fontSize = nextSize === defaultFontSize ? '' : nextSize + 'px'; // Updates inline style for a dynamic UI state.

        if (persist !== false) { // Checks the condition before running the next script step.
            if (nextSize === defaultFontSize) { // Checks the condition before running the next script step.
                try { // Starts a JavaScript block for the current control flow.
                    storage.removeItem(fontSizeKey); // Runs this JavaScript step for the page interaction.
                } catch (e) { } // Runs this JavaScript step for the page interaction.
            } else { // Starts a JavaScript block for the current control flow.
                setStoredValue(fontSizeKey, String(nextSize)); // Runs this JavaScript step for the page interaction.
            }
        }
    }

    // ----- Event wiring -----
    function changeFontSize(action) { // Defines the changeFontSize helper function.
        // ----- Helpers -----
        var currentSize = parseFloat(window.getComputedStyle(root).fontSize) || defaultFontSize; // Stores the currentSize value for later script logic.

        if (action === 'increase') { // Checks the condition before running the next script step.
            setFontSize(currentSize + fontStep); // Runs this JavaScript step for the page interaction.
        } else if (action === 'decrease') { // Starts a JavaScript block for the current control flow.
            setFontSize(currentSize - fontStep); // Runs this JavaScript step for the page interaction.
        } else { // Starts a JavaScript block for the current control flow.
            setFontSize(defaultFontSize); // Runs this JavaScript step for the page interaction.
        }
    }

    function getReadableText() { // Defines the getReadableText helper function.
        // ----- DOM references -----
        var main = document.getElementById('main-content') || document.querySelector('main') || document.body; // Stores the main DOM element reference.
        return main.innerText.replace(/\s+/g, ' ').trim(); // Returns the computed value to the caller.
    }

    // ----- Helpers -----
    function speakText() { // Defines the speakText helper function.
        if (!('speechSynthesis' in window) || !('SpeechSynthesisUtterance' in window)) { // Checks the condition before running the next script step.
            return; // Returns the computed value to the caller.
        }

        if (isSpeaking) { // Checks the condition before running the next script step.
            window.speechSynthesis.cancel(); // Runs this JavaScript step for the page interaction.
            isSpeaking = false; // Updates isSpeaking for the current script state.
            refreshUi(); // Runs this JavaScript step for the page interaction.
            return; // Returns the computed value to the caller.
        }

        var text = getReadableText(); // Stores the text value for later script logic.
        if (!text) return; // Checks the condition before running the next script step.

        var speech = new SpeechSynthesisUtterance(text); // Creates the speech object used by the script.
        speech.lang = 'en-GB'; // Updates speech.lang for the current script state.
        speech.rate = 0.95; // Updates speech.rate for the current script state.
        // ----- Form validation -----
        speech.onend = speech.onerror = function () { // Starts a JavaScript block for the current control flow.
            isSpeaking = false; // Updates isSpeaking for the current script state.
            refreshUi(); // Runs this JavaScript step for the page interaction.
        };

        window.speechSynthesis.cancel(); // Runs this JavaScript step for the page interaction.
        isSpeaking = true; // Updates isSpeaking for the current script state.
        refreshUi(); // Runs this JavaScript step for the page interaction.
        window.speechSynthesis.speak(speech); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function closeAccessibilityMenu() { // Defines the closeAccessibilityMenu helper function.
        // ----- DOM references -----
        var menu = document.getElementById('accessibilityMenu'); // Stores the menu DOM element reference.
        var toggle = document.getElementById('accessibilityToggle'); // Stores the toggle DOM element reference.
        var dropdown = document.getElementById('accessibilityDropdown'); // Stores the dropdown DOM element reference.

        if (!menu || !toggle) return; // Checks the condition before running the next script step.

        // ----- State updates -----
        menu.classList.remove('is-open'); // Removes a CSS class to update the element state.
        // ----- Accessibility -----
        toggle.setAttribute('aria-expanded', 'false'); // Sets an attribute required by the UI state.
        if (dropdown) dropdown.hidden = true; // Checks the condition before running the next script step.
    }

    // ----- Helpers -----
    function openAccessibilityMenu() { // Defines the openAccessibilityMenu helper function.
        // ----- DOM references -----
        var menu = document.getElementById('accessibilityMenu'); // Stores the menu DOM element reference.
        var toggle = document.getElementById('accessibilityToggle'); // Stores the toggle DOM element reference.
        var dropdown = document.getElementById('accessibilityDropdown'); // Stores the dropdown DOM element reference.

        if (!menu || !toggle || !dropdown) return; // Checks the condition before running the next script step.

        // ----- State updates -----
        menu.classList.add('is-open'); // Adds a CSS class to update the element state.
        // ----- Accessibility -----
        toggle.setAttribute('aria-expanded', 'true'); // Sets an attribute required by the UI state.
        dropdown.hidden = false; // Updates dropdown.hidden for the current script state.
    }

    // ----- Helpers -----
    function initAccessibilityMenu() { // Defines the initAccessibilityMenu helper function.
        // ----- DOM references -----
        var menu = document.getElementById('accessibilityMenu'); // Stores the menu DOM element reference.
        var toggle = document.getElementById('accessibilityToggle'); // Stores the toggle DOM element reference.
        var dropdown = document.getElementById('accessibilityDropdown'); // Stores the dropdown DOM element reference.

        if (!menu || !toggle || !dropdown) return; // Checks the condition before running the next script step.

        // ----- Event wiring -----
        toggle.addEventListener('click', function (event) { // Registers an event handler for user or browser interaction.
            event.stopPropagation(); // Runs this JavaScript step for the page interaction.
            // ----- State updates -----
            menu.classList.contains('is-open') ? closeAccessibilityMenu() : openAccessibilityMenu(); // Runs this JavaScript step for the page interaction.
        });

        // ----- Event wiring -----
        dropdown.addEventListener('click', function (event) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var actionButton = event.target.closest('[data-accessibility-action]'); // Stores the actionButton value for later script logic.
            if (!actionButton) return; // Checks the condition before running the next script step.

            var action = actionButton.getAttribute('data-accessibility-action'); // Stores the action value for later script logic.

            if (action === 'dark-mode') toggleDarkMode(); // Checks the condition before running the next script step.
            // ----- Event wiring -----
            if (action === 'increase-font') changeFontSize('increase'); // Checks the condition before running the next script step.
            if (action === 'decrease-font') changeFontSize('decrease'); // Checks the condition before running the next script step.
            if (action === 'reset-font') changeFontSize('reset'); // Checks the condition before running the next script step.
            // ----- Accessibility -----
            if (action === 'high-contrast') toggleHighContrast(); // Checks the condition before running the next script step.
            if (action === 'speak') speakText(); // Checks the condition before running the next script step.

            if (action !== 'speak') closeAccessibilityMenu(); // Checks the condition before running the next script step.
        });

        // ----- DOM references -----
        document.addEventListener('click', function (event) { // Registers an event handler for user or browser interaction.
            if (!menu.contains(event.target)) closeAccessibilityMenu(); // Checks the condition before running the next script step.
        });

        document.addEventListener('keydown', function (event) { // Registers an event handler for user or browser interaction.
            if (event.key === 'Escape') closeAccessibilityMenu(); // Checks the condition before running the next script step.
        });
    }

    document.addEventListener('DOMContentLoaded', function () { // Registers an event handler for user or browser interaction.
        applyStoredPreferences(); // Runs this JavaScript step for the page interaction.
        // ----- Startup -----
        initAccessibilityMenu(); // Runs this JavaScript step for the page interaction.
    });

    window.toggleDarkMode = toggleDarkMode; // Updates window.toggleDarkMode for the current script state.
    // ----- Accessibility -----
    window.toggleHighContrast = toggleHighContrast; // Updates window.toggleHighContrast for the current script state.
    // ----- Event wiring -----
    window.changeFontSize = changeFontSize; // Updates window.changeFontSize for the current script state.
    window.speakText = speakText; // Updates window.speakText for the current script state.
}());
