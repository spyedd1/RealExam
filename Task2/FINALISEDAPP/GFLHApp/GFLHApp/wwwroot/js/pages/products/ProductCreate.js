// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
        // ----- Strict mode -----
        'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

        // ----- DOM references -----
        document.querySelectorAll('.pw-stepper__btn').forEach(function (btn) { // Finds a page element needed by the script.
            // ----- Event wiring -----
            btn.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
                // ----- DOM references -----
                var input = document.getElementById(btn.dataset.target); // Stores the input DOM element reference.
                // ----- Event wiring -----
                if (!input) return; // Checks the condition before running the next script step.
                var step = parseFloat(input.step) || 1; // Stores the step value for later script logic.
                var current = parseFloat(input.value) || 0; // Stores the current value for later script logic.
                var min = input.min !== '' ? parseFloat(input.min) : -Infinity; // Stores the min value for later script logic.
                // ----- Helpers -----
                var next = btn.dataset.action === 'inc' ? current + step : current - step; // Stores the next value for later script logic.
                if (next < min) next = min; // Checks the condition before running the next script step.
                var decimals = (step.toString().split('.')[1] || '').length; // Stores the decimals value for later script logic.
                // ----- Event wiring -----
                input.value = next.toFixed(decimals); // Updates input.value for the current script state.
                input.dispatchEvent(new Event('change', { bubbles: true })); // Runs this JavaScript step for the page interaction.
            });
        });

        // ----- DOM references -----
        var allergensInput = document.getElementById('allergensInput'); // Stores the allergensInput DOM element reference.

        // ----- Product logic -----
        function getAllergens() { // Defines the getAllergens helper function.
            // ----- Event wiring -----
            return allergensInput.value.split(',') // Returns the computed value to the caller.
                // ----- Helpers -----
                .map(function (s) { return s.trim(); }) // Runs this JavaScript step for the page interaction.
                .filter(function (s) { return s.length > 0; }); // Runs this JavaScript step for the page interaction.
        }

        function syncChips() { // Defines the syncChips helper function.
            // ----- Product logic -----
            var active = getAllergens().map(function (s) { return s.toLowerCase(); }); // Stores the active value for later script logic.
            // ----- DOM references -----
            document.querySelectorAll('.pw-allergen-chip').forEach(function (chip) { // Finds a page element needed by the script.
                // ----- Product logic -----
                chip.classList.toggle('is-active', active.indexOf(chip.dataset.allergen.toLowerCase()) !== -1); // Toggles a CSS class based on the current state.
            });
        }

        // ----- DOM references -----
        document.querySelectorAll('.pw-allergen-chip').forEach(function (chip) { // Finds a page element needed by the script.
            // ----- Event wiring -----
            chip.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
                // ----- Product logic -----
                var current = getAllergens(); // Stores the current value for later script logic.
                var index = current.map(function (s) { return s.toLowerCase(); }).indexOf(chip.dataset.allergen.toLowerCase()); // Stores the index value for later script logic.
                if (index === -1) current.push(chip.dataset.allergen); // Checks the condition before running the next script step.
                else current.splice(index, 1); // Handles the fallback branch for the previous condition.
                // ----- Event wiring -----
                allergensInput.value = current.join(', '); // Updates allergensInput.value for the current script state.
                syncChips(); // Runs this JavaScript step for the page interaction.
            });
        });

        if (allergensInput) { // Checks the condition before running the next script step.
            allergensInput.addEventListener('input', syncChips); // Registers an event handler for user or browser interaction.
            syncChips(); // Runs this JavaScript step for the page interaction.
        }

        // ----- DOM references -----
        var previewHost = document.getElementById('adminImgPreview'); // Stores the previewHost DOM element reference.
        var pathLabel = document.getElementById('adminImgPath'); // Stores the pathLabel DOM element reference.
        var nameInput = document.getElementById('liveNameInput'); // Stores the nameInput DOM element reference.
        var nameLabel = document.getElementById('adminImgName'); // Stores the nameLabel DOM element reference.
        var uploadInput = document.getElementById('imageUpload'); // Stores the uploadInput DOM element reference.
        var pathInput = document.getElementById('liveImagePathInput'); // Stores the pathInput DOM element reference.

        // ----- Helpers -----
        function setPlaceholder() { // Defines the setPlaceholder helper function.
            // ----- Icon setup -----
            previewHost.outerHTML = '<div class="pw-admin-img-placeholder" id="adminImgPreview"><i data-lucide="sprout" style="width:72px;height:72px;opacity:.45;"></i></div>'; // Updates previewHost.outerHTML for the current script state.
            // ----- DOM references -----
            previewHost = document.getElementById('adminImgPreview'); // Finds a page element needed by the script.
            // ----- Icon setup -----
            if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.
        }

        // ----- Helpers -----
        function setImage(src, label) { // Defines the setImage helper function.
            // ----- Product logic -----
            previewHost.outerHTML = '<img id="adminImgPreview" src="' + src + '" alt="Product image preview" />'; // Updates previewHost.outerHTML for the current script state.
            // ----- DOM references -----
            previewHost = document.getElementById('adminImgPreview'); // Finds a page element needed by the script.
            pathLabel.textContent = label; // Updates pathLabel.textContent for the current script state.
        }

        // ----- Event wiring -----
        if (nameInput && nameLabel) { // Checks the condition before running the next script step.
            nameLabel.textContent = nameInput.value || 'New product'; // Updates nameLabel.textContent for the current script state.
            nameInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
                nameLabel.textContent = nameInput.value || 'New product'; // Updates nameLabel.textContent for the current script state.
            });
        }

        if (pathInput) { // Checks the condition before running the next script step.
            pathInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
                if (uploadInput && uploadInput.files && uploadInput.files.length > 0) return; // Checks the condition before running the next script step.
                var src = pathInput.value.trim(); // Stores the src value for later script logic.
                if (src) setImage(src, src); // Checks the condition before running the next script step.
                else { // Handles the fallback branch for the previous condition.
                    setPlaceholder(); // Runs this JavaScript step for the page interaction.
                    pathLabel.textContent = 'no image selected'; // Updates pathLabel.textContent for the current script state.
                }
            });
        }

        if (uploadInput) { // Checks the condition before running the next script step.
            uploadInput.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
                var file = uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null; // Stores the file value for later script logic.
                if (!file) { // Checks the condition before running the next script step.
                    var src = pathInput ? pathInput.value.trim() : ''; // Stores the src value for later script logic.
                    if (src) setImage(src, src); // Checks the condition before running the next script step.
                    else { // Handles the fallback branch for the previous condition.
                        setPlaceholder(); // Runs this JavaScript step for the page interaction.
                        pathLabel.textContent = 'no image selected'; // Updates pathLabel.textContent for the current script state.
                    }
                    return; // Returns the computed value to the caller.
                }

                // ----- Helpers -----
                var url = URL.createObjectURL(file); // Stores the url value for later script logic.
                setImage(url, file.name); // Runs this JavaScript step for the page interaction.
            });
        }

        // ----- DOM references -----
        var summary = document.querySelector('[data-valmsg-summary]'); // Stores the summary DOM element reference.
        if (summary && summary.querySelectorAll('li').length > 0) { // Finds a page element needed by the script.
            // ----- State updates -----
            summary.removeAttribute('hidden'); // Removes an attribute no longer needed by the UI state.
        }

        // ----- Event wiring -----
        if (pathInput && pathInput.value.trim()) { // Checks the condition before running the next script step.
            setImage(pathInput.value.trim(), pathInput.value.trim()); // Runs this JavaScript step for the page interaction.
        }
    }());
