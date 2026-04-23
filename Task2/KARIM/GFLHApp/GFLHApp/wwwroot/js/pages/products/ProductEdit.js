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
                var step    = parseFloat(input.step) || 1; // Stores the step value for later script logic.
                var current = parseFloat(input.value) || 0; // Stores the current value for later script logic.
                var min     = input.min !== '' ? parseFloat(input.min) : -Infinity; // Stores the min value for later script logic.
                // ----- Helpers -----
                var next    = btn.dataset.action === 'inc' ? current + step : current - step; // Stores the next value for later script logic.
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
                var idx = current.map(function (s) { return s.toLowerCase(); }).indexOf(chip.dataset.allergen.toLowerCase()); // Stores the idx value for later script logic.
                if (idx === -1) current.push(chip.dataset.allergen); // Checks the condition before running the next script step.
                else current.splice(idx, 1); // Handles the fallback branch for the previous condition.
                // ----- Event wiring -----
                allergensInput.value = current.join(', '); // Updates allergensInput.value for the current script state.
                syncChips(); // Runs this JavaScript step for the page interaction.
            });
        });
        allergensInput.addEventListener('input', syncChips); // Registers an event handler for user or browser interaction.
        syncChips(); // Runs this JavaScript step for the page interaction.

        // ----- DOM references -----
        var imgCard      = document.getElementById('adminImgCard'); // Stores the imgCard DOM element reference.
        var nameDisplay  = document.getElementById('adminImgName'); // Stores the nameDisplay DOM element reference.
        var pathDisplay  = document.getElementById('adminImgPath'); // Stores the pathDisplay DOM element reference.
        var nameInput    = document.getElementById('liveNameInput'); // Stores the nameInput DOM element reference.
        var imageInput   = document.getElementById('liveImageInput'); // Stores the imageInput DOM element reference.
        var uploadInput  = document.getElementById('imageUpload'); // Stores the uploadInput DOM element reference.
        // ----- Helpers -----
        var originalPath = imgCard ? (imgCard.dataset.originalPath || '') : ''; // Stores the originalPath value for later script logic.

        // ----- Event wiring -----
        if (nameInput && nameDisplay) { // Checks the condition before running the next script step.
            nameInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
                nameDisplay.textContent = nameInput.value || 'Product name'; // Updates nameDisplay.textContent for the current script state.
            });
        }

        // ----- Helpers -----
        function renderPlaceholder() { // Defines the renderPlaceholder helper function.
            // ----- DOM references -----
            var existing = imgCard.querySelector('img'); // Stores the existing DOM element reference.
            if (existing) { // Checks the condition before running the next script step.
                var ph = document.createElement('div'); // Stores the ph value for later script logic.
                // ----- Admin dashboard logic -----
                ph.className = 'pw-admin-img-placeholder'; // Updates ph.className for the current script state.
                ph.id = 'adminImgEl'; // Updates ph.id for the current script state.
                // ----- State updates -----
                ph.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="opacity:.45"><path d="M12 22V12"/><path d="M5 12H2a10 10 0 0 0 20 0h-3"/><path d="M12 12C12 7 8 3 8 3s0 4 4 4"/><path d="M12 12C12 7 16 3 16 3s0 4-4 4"/></svg>'; // Updates ph.innerHTML for the current script state.
                existing.replaceWith(ph); // Runs this JavaScript step for the page interaction.
            }
        }

        // ----- Helpers -----
        function renderImage(src) { // Defines the renderImage helper function.
            // ----- DOM references -----
            var existing = imgCard.querySelector('img'); // Stores the existing DOM element reference.
            var placeholder = imgCard.querySelector('.pw-admin-img-placeholder'); // Stores the placeholder DOM element reference.

            if (existing) { // Checks the condition before running the next script step.
                existing.src = src; // Updates existing.src for the current script state.
                // ----- Event wiring -----
                existing.alt = nameInput ? (nameInput.value || 'Product image') : 'Product image'; // Updates existing.alt for the current script state.
                return; // Returns the computed value to the caller.
            }

            // ----- DOM references -----
            var img = document.createElement('img'); // Stores the img value for later script logic.
            // ----- State updates -----
            img.style.cssText = 'width:100%;aspect-ratio:1/1;object-fit:cover;display:block;'; // Updates inline style for a dynamic UI state.
            // ----- Event wiring -----
            img.alt = nameInput ? (nameInput.value || 'Product image') : 'Product image'; // Updates img.alt for the current script state.
            img.src = src; // Updates img.src for the current script state.
            if (placeholder) placeholder.replaceWith(img); // Checks the condition before running the next script step.
            // ----- DOM references -----
            else imgCard.insertBefore(img, imgCard.querySelector('.pw-admin-img-meta')); // Finds a page element needed by the script.
        }

        // ----- Event wiring -----
        if (imageInput && imgCard) { // Checks the condition before running the next script step.
            imageInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
                if (uploadInput && uploadInput.files && uploadInput.files.length > 0) return; // Checks the condition before running the next script step.

                var src = imageInput.value.trim(); // Stores the src value for later script logic.
                if (pathDisplay) pathDisplay.textContent = src || originalPath || 'no image set'; // Checks the condition before running the next script step.

                if (src) { // Checks the condition before running the next script step.
                    renderImage(src); // Runs this JavaScript step for the page interaction.
                } else if (originalPath) { // Starts a JavaScript block for the current control flow.
                    renderImage(originalPath); // Runs this JavaScript step for the page interaction.
                } else { // Starts a JavaScript block for the current control flow.
                    renderPlaceholder(); // Runs this JavaScript step for the page interaction.
                }
            });
        }

        if (uploadInput && imgCard) { // Checks the condition before running the next script step.
            uploadInput.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
                var file = uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null; // Stores the file value for later script logic.

                if (!file) { // Checks the condition before running the next script step.
                    var src = imageInput ? imageInput.value.trim() : ''; // Stores the src value for later script logic.
                    if (src) { // Checks the condition before running the next script step.
                        renderImage(src); // Runs this JavaScript step for the page interaction.
                        if (pathDisplay) pathDisplay.textContent = src; // Checks the condition before running the next script step.
                    } else if (originalPath) { // Starts a JavaScript block for the current control flow.
                        renderImage(originalPath); // Runs this JavaScript step for the page interaction.
                        if (pathDisplay) pathDisplay.textContent = originalPath; // Checks the condition before running the next script step.
                    } else { // Starts a JavaScript block for the current control flow.
                        renderPlaceholder(); // Runs this JavaScript step for the page interaction.
                        if (pathDisplay) pathDisplay.textContent = 'no image set'; // Checks the condition before running the next script step.
                    }
                    return; // Returns the computed value to the caller.
                }

                // ----- Helpers -----
                var url = URL.createObjectURL(file); // Stores the url value for later script logic.
                renderImage(url); // Runs this JavaScript step for the page interaction.
                if (pathDisplay) pathDisplay.textContent = file.name; // Checks the condition before running the next script step.
            });
        }

        if (imgCard) { // Checks the condition before running the next script step.
            var ticking = false; // Stores the ticking value for later script logic.
            var lastE   = {}; // Stores the lastE value for later script logic.
            // ----- Event wiring -----
            imgCard.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
                lastE = e; // Updates lastE for the current script state.
                if (ticking) return; // Checks the condition before running the next script step.
                ticking = true; // Updates ticking for the current script state.
                // ----- Helpers -----
                requestAnimationFrame(function () { // Schedules smooth visual updates for the next animation frame.
                    var r = imgCard.getBoundingClientRect(); // Stores the r value for later script logic.
                    var x = ((lastE.clientX - r.left) / r.width  - 0.5) * 2; // Stores the x value for later script logic.
                    var y = ((lastE.clientY - r.top)  / r.height - 0.5) * 2; // Stores the y value for later script logic.
                    // ----- State updates -----
                    imgCard.style.transform = 'perspective(800px) rotateY(' + (x * 11) + 'deg) rotateX(' + (-y * 9) + 'deg) scale(1.02)'; // Updates inline style for a dynamic UI state.
                    imgCard.style.boxShadow = '0 16px 48px rgba(0,0,0,.14)'; // Updates inline style for a dynamic UI state.
                    ticking = false; // Updates ticking for the current script state.
                });
            }, { passive: true }); // Runs this JavaScript step for the page interaction.
            // ----- Event wiring -----
            imgCard.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
                // ----- State updates -----
                imgCard.style.transform = ''; // Updates inline style for a dynamic UI state.
                imgCard.style.boxShadow = ''; // Updates inline style for a dynamic UI state.
            });
        }

        // ----- DOM references -----
        var summary = document.querySelector('[data-valmsg-summary]'); // Stores the summary DOM element reference.
        if (summary && summary.querySelectorAll('li').length > 0) { // Finds a page element needed by the script.
            // ----- State updates -----
            summary.removeAttribute('hidden'); // Removes an attribute no longer needed by the UI state.
        }

    }());
