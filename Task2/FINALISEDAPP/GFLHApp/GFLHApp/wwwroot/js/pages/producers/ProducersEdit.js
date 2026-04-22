// ----- Accessibility -----
(function () { // IIFE — keeps all variables out of global scope // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enable strict mode // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- Icon setup -----
    if (window.lucide) lucide.createIcons(); // Initialise lucide icons if the library is loaded // Checks the condition before running the next script step.

    // ----- DOM references -----
    var nameInput = document.getElementById('liveNameInput');    // Name input field // Stores the nameInput DOM element reference.
    var emailInput = document.getElementById('liveEmailInput');   // Email input field // Stores the emailInput DOM element reference.
    var infoInput = document.getElementById('liveInfoInput');    // Bio textarea // Stores the infoInput DOM element reference.
    var vatInput = document.getElementById('VATNumber'); // VAT number input // Stores the vatInput DOM element reference.
    var previewName = document.getElementById('previewName');      // Preview name element // Stores the previewName DOM element reference.
    var previewAvatar = document.getElementById('previewAvatar');    // Preview avatar element // Stores the previewAvatar DOM element reference.
    var previewEmailText = document.getElementById('previewEmailText'); // Preview email text element // Stores the previewEmailText DOM element reference.
    var previewInfo = document.getElementById('previewInfo');      // Preview bio element // Stores the previewInfo DOM element reference.
    var imgCard = document.getElementById('adminImgCard'); // Image preview card // Stores the imgCard DOM element reference.
    var nameDisplay = document.getElementById('adminImgName'); // Image card name // Stores the nameDisplay DOM element reference.
    var pathDisplay = document.getElementById('adminImgPath'); // Image path label // Stores the pathDisplay DOM element reference.
    var imageInput = document.getElementById('liveImageInput'); // Existing image dropdown // Stores the imageInput DOM element reference.
    var uploadInput = document.getElementById('imageUpload'); // Upload input // Stores the uploadInput DOM element reference.
    // ----- Helpers -----
    var originalPath = pathDisplay && pathDisplay.textContent !== 'no image set' ? pathDisplay.textContent.trim() : ''; // Stores the originalPath value for later script logic.

    // ----- Event wiring -----
    function ensureErrorAfter(input, id) { // Defines the ensureErrorAfter helper function.
        if (!input) return null; // Checks the condition before running the next script step.
        // ----- DOM references -----
        var existing = document.getElementById(id); // Stores the existing DOM element reference.
        if (existing) return existing; // Checks the condition before running the next script step.
        var error = document.createElement('span'); // Stores the error value for later script logic.
        // ----- Form validation -----
        error.id = id; // Updates error.id for the current script state.
        error.className = 'pw-field__error'; // Updates error.className for the current script state.
        // ----- Event wiring -----
        input.insertAdjacentElement('afterend', error); // Runs this JavaScript step for the page interaction.
        // ----- Form validation -----
        return error; // Returns the computed value to the caller.
    }

    // ----- Event wiring -----
    function setError(input, id, message) { // Defines the setError helper function.
        var node = ensureErrorAfter(input, id); // Stores the node value for later script logic.
        if (node) node.textContent = message || ''; // Checks the condition before running the next script step.
    }

    // ----- Form validation -----
    function validateName() { // Defines the validateName helper function.
        // ----- Event wiring -----
        var val = nameInput ? nameInput.value.trim() : ''; // Stores the val value for later script logic.
        if (!val) { setError(nameInput, 'nameError', 'Producer name is required.'); return false; } // Checks the condition before running the next script step.
        if (val.length < 3) { setError(nameInput, 'nameError', 'Producer name must be at least 3 characters.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 100) { setError(nameInput, 'nameError', 'Producer name must not exceed 100 characters.'); return false; } // Checks the condition before running the next script step.
        setError(nameInput, 'nameError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    // ----- Form validation -----
    function validateEmail() { // Defines the validateEmail helper function.
        // ----- Event wiring -----
        var val = emailInput ? emailInput.value.trim() : ''; // Stores the val value for later script logic.
        // ----- Server values -----
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Stores the emailRegex value supplied by the Razor page.
        // ----- Event wiring -----
        if (!val) { setError(emailInput, 'emailError', 'Email address is required.'); return false; } // Checks the condition before running the next script step.
        if (!emailRegex.test(val)) { setError(emailInput, 'emailError', 'Please enter a valid email address.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 150) { setError(emailInput, 'emailError', 'Email address must not exceed 150 characters.'); return false; } // Checks the condition before running the next script step.
        setError(emailInput, 'emailError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    // ----- Form validation -----
    function validateInfo() { // Defines the validateInfo helper function.
        // ----- Event wiring -----
        var val = infoInput ? infoInput.value.trim() : ''; // Stores the val value for later script logic.
        if (!val) { setError(infoInput, 'infoError', 'Producer information is required.'); return false; } // Checks the condition before running the next script step.
        if (val.length < 10) { setError(infoInput, 'infoError', 'Producer information must be at least 10 characters.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 500) { setError(infoInput, 'infoError', 'Producer information must not exceed 500 characters.'); return false; } // Checks the condition before running the next script step.
        setError(infoInput, 'infoError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    // ----- Form validation -----
    function validateVAT() { // Defines the validateVAT helper function.
        // ----- Producer logic -----
        if (!vatToggle || !vatToggle.checked) { // Checks the condition before running the next script step.
            // ----- Event wiring -----
            setError(vatInput, 'vatError', ''); // Runs this JavaScript step for the page interaction.
            return true; // Returns the computed value to the caller.
        }

        var val = vatInput ? vatInput.value.trim().toUpperCase() : ''; // Stores the val value for later script logic.
        if (!val) { setError(vatInput, 'vatError', 'VAT number is required if VAT registered.'); return false; } // Checks the condition before running the next script step.
        if (!/^GB[0-9]{9}$/.test(val)) { setError(vatInput, 'vatError', 'VAT number must start with GB followed by exactly 9 digits.'); return false; } // Checks the condition before running the next script step.
        if (vatInput) vatInput.value = val; // Checks the condition before running the next script step.
        setError(vatInput, 'vatError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    // ----- Form validation -----
    function validateImage() { // Defines the validateImage helper function.
        // ----- Event wiring -----
        var file = uploadInput && uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null; // Stores the file value for later script logic.
        var error = ensureErrorAfter(uploadInput, 'imageError'); // Stores the error value for later script logic.
        if (!file) { // Checks the condition before running the next script step.
            // ----- Form validation -----
            if (error) error.textContent = ''; // Checks the condition before running the next script step.
            return true; // Returns the computed value to the caller.
        }

        if (!/\.(jpe?g|png|webp)$/i.test(file.name) && ['image/jpeg', 'image/png', 'image/webp'].indexOf(file.type) === -1) { // Checks the condition before running the next script step.
            if (error) error.textContent = 'Upload a JPG, PNG, or WebP image.'; // Checks the condition before running the next script step.
            return false; // Returns the computed value to the caller.
        }

        if (file.size > 5 * 1024 * 1024) { // Checks the condition before running the next script step.
            if (error) error.textContent = 'Image files must be smaller than 5 MB.'; // Checks the condition before running the next script step.
            return false; // Returns the computed value to the caller.
        }

        if (error) error.textContent = ''; // Checks the condition before running the next script step.
        return true; // Returns the computed value to the caller.
    }

    // ----- Event wiring -----
    if (nameInput) { // Guard: only bind if element exists // Checks the condition before running the next script step.
        nameInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            var v = nameInput.value.trim() || 'Producer name'; // Fall back to placeholder if empty // Stores the v value for later script logic.
            if (previewName) previewName.textContent = v;                          // Update preview name // Checks the condition before running the next script step.
            // ----- Producer logic -----
            if (previewAvatar) previewAvatar.textContent = v.charAt(0).toUpperCase() || '?'; // Update avatar initial // Checks the condition before running the next script step.
            if (nameDisplay) nameDisplay.textContent = v; // Checks the condition before running the next script step.
        });
        // ----- Event wiring -----
        nameInput.addEventListener('blur', validateName); // Registers an event handler for user or browser interaction.
    }
    if (emailInput && previewEmailText) { // Guard: only bind if both elements exist // Checks the condition before running the next script step.
        emailInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            previewEmailText.textContent = emailInput.value.trim() || '—'; // Update preview email or show dash // Updates previewEmailText.textContent for the current script state.
        });
        emailInput.addEventListener('blur', validateEmail); // Registers an event handler for user or browser interaction.
    }
    if (infoInput && previewInfo) { // Guard: only bind if both elements exist // Checks the condition before running the next script step.
        infoInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            previewInfo.textContent = infoInput.value.trim() || 'No bio yet.'; // Update preview bio or show fallback // Updates previewInfo.textContent for the current script state.
        });
        infoInput.addEventListener('blur', validateInfo); // Registers an event handler for user or browser interaction.
    }

    // ----- DOM references -----
    var vatToggle = document.getElementById('vatToggle');      // VAT checkbox toggle // Stores the vatToggle DOM element reference.
    var vatNumberField = document.getElementById('vatNumberField'); // Collapsible VAT number field // Stores the vatNumberField DOM element reference.
    var previewVat = document.getElementById('previewVat');     // VAT badge in preview card // Stores the previewVat DOM element reference.

    // ----- Producer logic -----
    function syncVat() { // Defines the syncVat helper function.
        var on = vatToggle && vatToggle.checked;                          // True if toggle is checked // Stores the on value for later script logic.
        if (vatNumberField) vatNumberField.classList.toggle('is-open', on); // Show/hide VAT number field // Toggles a CSS class based on the current state.
        if (previewVat) previewVat.classList.toggle('is-visible', on);  // Show/hide VAT badge in preview // Toggles a CSS class based on the current state.
        // ----- Event wiring -----
        if (!on && vatInput) { // Checks the condition before running the next script step.
            vatInput.value = ''; // Updates vatInput.value for the current script state.
            setError(vatInput, 'vatError', ''); // Runs this JavaScript step for the page interaction.
        }
    }
    // ----- Producer logic -----
    if (vatToggle) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        vatToggle.addEventListener('change', syncVat); // Re-sync on toggle change // Registers an event handler for user or browser interaction.
        // ----- Producer logic -----
        syncVat(); // Run once on load to match initial state // Runs this JavaScript step for the page interaction.
    }
    // ----- Event wiring -----
    if (vatInput) vatInput.addEventListener('blur', validateVAT); // Registers an event handler for user or browser interaction.

    // ----- Helpers -----
    function renderPlaceholder() { // Defines the renderPlaceholder helper function.
        if (!imgCard) return; // Checks the condition before running the next script step.
        // ----- DOM references -----
        var existing = imgCard.querySelector('img'); // Stores the existing DOM element reference.
        if (existing) { // Checks the condition before running the next script step.
            var ph = document.createElement('div'); // Stores the ph value for later script logic.
            // ----- Admin dashboard logic -----
            ph.className = 'pw-admin-img-placeholder'; // Updates ph.className for the current script state.
            ph.id = 'adminImgEl'; // Updates ph.id for the current script state.
            // ----- Icon setup -----
            ph.innerHTML = '<i data-lucide="sprout" style="width:72px;height:72px;opacity:.45;"></i>'; // Updates ph.innerHTML for the current script state.
            existing.replaceWith(ph); // Runs this JavaScript step for the page interaction.
            if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.
        }
    }

    // ----- Helpers -----
    function renderImage(src) { // Defines the renderImage helper function.
        if (!imgCard) return; // Checks the condition before running the next script step.
        // ----- DOM references -----
        var existing = imgCard.querySelector('img'); // Stores the existing DOM element reference.
        var placeholder = imgCard.querySelector('.pw-admin-img-placeholder'); // Stores the placeholder DOM element reference.

        if (existing) { // Checks the condition before running the next script step.
            existing.src = src; // Updates existing.src for the current script state.
            // ----- Event wiring -----
            existing.alt = nameInput ? (nameInput.value || 'Producer image') : 'Producer image'; // Updates existing.alt for the current script state.
            return; // Returns the computed value to the caller.
        }

        // ----- DOM references -----
        var img = document.createElement('img'); // Stores the img value for later script logic.
        // ----- Event wiring -----
        img.alt = nameInput ? (nameInput.value || 'Producer image') : 'Producer image'; // Updates img.alt for the current script state.
        img.src = src; // Updates img.src for the current script state.
        if (placeholder) placeholder.replaceWith(img); // Checks the condition before running the next script step.
        // ----- DOM references -----
        else imgCard.insertBefore(img, imgCard.querySelector('.pw-admin-img-meta')); // Finds a page element needed by the script.
    }

    // ----- Event wiring -----
    if (imageInput) { // Checks the condition before running the next script step.
        imageInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            if (uploadInput && uploadInput.files && uploadInput.files.length > 0) return; // Checks the condition before running the next script step.

            var src = imageInput.value.trim(); // Stores the src value for later script logic.
            if (pathDisplay) pathDisplay.textContent = src || originalPath || 'no image set'; // Checks the condition before running the next script step.

            if (src) renderImage(src); // Checks the condition before running the next script step.
            else if (originalPath) renderImage(originalPath); // Checks the next fallback condition in this script flow.
            else renderPlaceholder(); // Handles the fallback branch for the previous condition.
        });
    }

    if (uploadInput) { // Checks the condition before running the next script step.
        uploadInput.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
            // ----- Form validation -----
            if (!validateImage()) return; // Checks the condition before running the next script step.
            // ----- Event wiring -----
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

            renderImage(URL.createObjectURL(file)); // Runs this JavaScript step for the page interaction.
            if (pathDisplay) pathDisplay.textContent = file.name; // Checks the condition before running the next script step.
        });
    }

    // ----- DOM references -----
    var card = document.getElementById('previewCard'); // Preview card element // Stores the card DOM element reference.
    if (card) { // Checks the condition before running the next script step.
        // ----- Helpers -----
        var ticking = false, lastE = {}; // rAF throttle flag and last mouse event cache // Stores the ticking value for later script logic.
        // ----- Event wiring -----
        card.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            lastE = e; // Cache latest mouse event // Updates lastE for the current script state.
            // ----- Startup -----
            if (ticking) return; // Skip if rAF already queued // Checks the condition before running the next script step.
            ticking = true; // Updates ticking for the current script state.
            // ----- Helpers -----
            requestAnimationFrame(function () { // Schedules smooth visual updates for the next animation frame.
                var r = card.getBoundingClientRect(); // Stores the r value for later script logic.
                var x = ((lastE.clientX - r.left) / r.width - 0.5) * 2; // Normalised X (-1 to 1) // Stores the x value for later script logic.
                var y = ((lastE.clientY - r.top) / r.height - 0.5) * 2; // Normalised Y (-1 to 1) // Stores the y value for later script logic.
                // ----- State updates -----
                card.style.transform = 'perspective(800px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 8) + 'deg) scale(1.02)'; // Apply 3D tilt and slight scale // Updates inline style for a dynamic UI state.
                card.style.boxShadow = '0 16px 48px rgba(0,0,0,.14)'; // Deepen shadow on hover // Updates inline style for a dynamic UI state.
                ticking = false; // Allow next rAF // Updates ticking for the current script state.
            });
        }, { passive: true }); // Passive listener for scroll performance // Runs this JavaScript step for the page interaction.
        // ----- Event wiring -----
        card.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            card.style.transform = ''; // Reset tilt on mouse leave // Updates inline style for a dynamic UI state.
            card.style.boxShadow = ''; // Reset shadow on mouse leave // Updates inline style for a dynamic UI state.
        });
    }

    if (imgCard) { // Checks the condition before running the next script step.
        // ----- Helpers -----
        var imgTicking = false, imgLastE = {}; // Stores the imgTicking value for later script logic.
        // ----- Event wiring -----
        imgCard.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            imgLastE = e; // Updates imgLastE for the current script state.
            if (imgTicking) return; // Checks the condition before running the next script step.
            imgTicking = true; // Updates imgTicking for the current script state.
            // ----- Helpers -----
            requestAnimationFrame(function () { // Schedules smooth visual updates for the next animation frame.
                var r = imgCard.getBoundingClientRect(); // Stores the r value for later script logic.
                var x = ((imgLastE.clientX - r.left) / r.width - 0.5) * 2; // Stores the x value for later script logic.
                var y = ((imgLastE.clientY - r.top) / r.height - 0.5) * 2; // Stores the y value for later script logic.
                // ----- State updates -----
                imgCard.style.transform = 'perspective(800px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 8) + 'deg) scale(1.02)'; // Updates inline style for a dynamic UI state.
                imgCard.style.boxShadow = '0 16px 48px rgba(0,0,0,.14)'; // Updates inline style for a dynamic UI state.
                imgTicking = false; // Updates imgTicking for the current script state.
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
    var form = document.getElementById('editProducerForm'); // Stores the form DOM element reference.
    if (form) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        form.addEventListener('submit', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Form validation -----
            var valid = validateName() & validateEmail() & validateInfo() & validateVAT() & validateImage(); // Stores the valid value for later script logic.
            if (!valid) e.preventDefault(); // Stops the browser's default action for this event.
        });
    }

    // ----- DOM references -----
    var summary = document.querySelector('[data-valmsg-summary]'); // Validation summary element // Stores the summary DOM element reference.
    if (summary && summary.querySelectorAll('li').length > 0) {    // Only show if errors exist // Finds a page element needed by the script.
        // ----- State updates -----
        summary.removeAttribute('hidden');                         // Make summary visible // Removes an attribute no longer needed by the UI state.
    }

}());
