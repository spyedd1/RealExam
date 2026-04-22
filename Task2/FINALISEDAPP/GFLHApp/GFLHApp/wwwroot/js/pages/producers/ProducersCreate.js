// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- DOM references -----
    var form = document.getElementById('producerForm'); // Stores the form DOM element reference.
    // ----- Icon setup -----
    if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

    // ----- DOM references -----
    var nameInput = document.getElementById('producerName'); // Stores the nameInput DOM element reference.
    var emailInput = document.getElementById('producerEmail'); // Stores the emailInput DOM element reference.
    var infoInput = document.getElementById('producerInfo'); // Stores the infoInput DOM element reference.
    var vatToggle = document.getElementById('isVATRegistered'); // Stores the vatToggle DOM element reference.
    var vatNumberGroup = document.getElementById('vatNumberGroup'); // Stores the vatNumberGroup DOM element reference.
    var vatInput = document.getElementById('vatNumber'); // Stores the vatInput DOM element reference.
    var passwordInput = document.getElementById('accountPassword'); // Stores the passwordInput DOM element reference.
    var confirmPasswordInput = document.getElementById('confirmPassword'); // Stores the confirmPasswordInput DOM element reference.
    var uploadInput = document.getElementById('imageUpload'); // Stores the uploadInput DOM element reference.
    var pathInput = document.getElementById('liveImagePathInput'); // Stores the pathInput DOM element reference.
    var imgCard = document.getElementById('adminImgCard'); // Stores the imgCard DOM element reference.
    var previewHost = document.getElementById('adminImgEl'); // Stores the previewHost DOM element reference.
    var pathLabel = document.getElementById('adminImgPath'); // Stores the pathLabel DOM element reference.
    var nameLabel = document.getElementById('adminImgName'); // Stores the nameLabel DOM element reference.
    var previewName = document.getElementById('previewName'); // Stores the previewName DOM element reference.
    var previewAvatar = document.getElementById('previewAvatar'); // Stores the previewAvatar DOM element reference.
    var previewEmailText = document.getElementById('previewEmailText'); // Stores the previewEmailText DOM element reference.
    var previewInfo = document.getElementById('previewInfo'); // Stores the previewInfo DOM element reference.
    var previewVat = document.getElementById('previewVat'); // Stores the previewVat DOM element reference.

    // ----- Form validation -----
    function setError(id, message) { // Defines the setError helper function.
        // ----- DOM references -----
        var node = document.getElementById(id); // Stores the node DOM element reference.
        if (node) node.textContent = message || ''; // Checks the condition before running the next script step.
    }

    // ----- Producer logic -----
    function toggleVATNumber() { // Defines the toggleVATNumber helper function.
        var isOpen = vatToggle && vatToggle.checked; // Stores the isOpen value for later script logic.
        if (vatNumberGroup) vatNumberGroup.classList.toggle('is-open', isOpen); // Toggles a CSS class based on the current state.
        if (previewVat) previewVat.classList.toggle('is-visible', isOpen); // Toggles a CSS class based on the current state.
        // ----- Event wiring -----
        if (!isOpen && vatInput) { // Checks the condition before running the next script step.
            vatInput.value = ''; // Updates vatInput.value for the current script state.
            // ----- Form validation -----
            setError('vatError', ''); // Runs this JavaScript step for the page interaction.
        }
    }

    function validateName() { // Defines the validateName helper function.
        // ----- Event wiring -----
        var val = (nameInput ? nameInput.value : '').trim(); // Stores the val value for later script logic.
        // ----- Form validation -----
        if (!val) { setError('nameError', 'Producer name is required.'); return false; } // Checks the condition before running the next script step.
        if (val.length < 3) { setError('nameError', 'Producer name must be at least 3 characters.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 100) { setError('nameError', 'Producer name must not exceed 100 characters.'); return false; } // Checks the condition before running the next script step.
        setError('nameError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validateEmail() { // Defines the validateEmail helper function.
        // ----- Event wiring -----
        var val = (emailInput ? emailInput.value : '').trim(); // Stores the val value for later script logic.
        // ----- Server values -----
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Stores the emailRegex value supplied by the Razor page.
        // ----- Form validation -----
        if (!val) { setError('emailError', 'Email address is required.'); return false; } // Checks the condition before running the next script step.
        if (!emailRegex.test(val)) { setError('emailError', 'Please enter a valid email address.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 150) { setError('emailError', 'Email address must not exceed 150 characters.'); return false; } // Checks the condition before running the next script step.
        setError('emailError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validateInfo() { // Defines the validateInfo helper function.
        // ----- Event wiring -----
        var val = (infoInput ? infoInput.value : '').trim(); // Stores the val value for later script logic.
        // ----- Form validation -----
        if (!val) { setError('infoError', 'Producer information is required.'); return false; } // Checks the condition before running the next script step.
        if (val.length < 10) { setError('infoError', 'Producer information must be at least 10 characters.'); return false; } // Checks the condition before running the next script step.
        if (val.length > 500) { setError('infoError', 'Producer information must not exceed 500 characters.'); return false; } // Checks the condition before running the next script step.
        setError('infoError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validateVAT() { // Defines the validateVAT helper function.
        // ----- Producer logic -----
        if (!vatToggle || !vatToggle.checked) { // Checks the condition before running the next script step.
            // ----- Form validation -----
            setError('vatError', ''); // Runs this JavaScript step for the page interaction.
            return true; // Returns the computed value to the caller.
        }

        // ----- Event wiring -----
        var val = (vatInput ? vatInput.value : '').trim().toUpperCase(); // Stores the val value for later script logic.
        // ----- Form validation -----
        var vatRegex = /^GB[0-9]{9}$/; // Stores the vatRegex value for later script logic.
        if (!val) { setError('vatError', 'VAT number is required if VAT registered.'); return false; } // Checks the condition before running the next script step.
        if (!vatRegex.test(val)) { setError('vatError', 'VAT number must start with GB followed by exactly 9 digits.'); return false; } // Checks the condition before running the next script step.
        // ----- Event wiring -----
        if (vatInput) vatInput.value = val; // Checks the condition before running the next script step.
        // ----- Form validation -----
        setError('vatError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validatePassword() { // Defines the validatePassword helper function.
        // ----- Event wiring -----
        var val = passwordInput ? passwordInput.value : ''; // Stores the val value for later script logic.
        // ----- Form validation -----
        var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$/; // Stores the passwordRegex value for later script logic.
        if (!val) { setError('passwordError', 'Password is required.'); return false; } // Checks the condition before running the next script step.
        if (!passwordRegex.test(val)) { // Checks the condition before running the next script step.
            setError('passwordError', 'Password must be at least 8 characters and include uppercase, lowercase, a number, and a special character.'); // Runs this JavaScript step for the page interaction.
            return false; // Returns the computed value to the caller.
        }
        setError('passwordError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validateConfirmPassword() { // Defines the validateConfirmPassword helper function.
        // ----- Event wiring -----
        var pass = passwordInput ? passwordInput.value : ''; // Stores the pass value for later script logic.
        var confirm = confirmPasswordInput ? confirmPasswordInput.value : ''; // Stores the confirm value for later script logic.
        // ----- Form validation -----
        if (!confirm) { setError('confirmPasswordError', 'Please confirm the password.'); return false; } // Checks the condition before running the next script step.
        if (pass !== confirm) { setError('confirmPasswordError', 'Passwords do not match.'); return false; } // Checks the condition before running the next script step.
        setError('confirmPasswordError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    function validateImage() { // Defines the validateImage helper function.
        // ----- Event wiring -----
        var file = uploadInput && uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null; // Stores the file value for later script logic.
        // ----- Helpers -----
        var allowed = ['image/jpeg', 'image/png', 'image/webp']; // Stores the allowed value for later script logic.
        var allowedExtensions = /\.(jpe?g|png|webp)$/i; // Stores the allowedExtensions value for later script logic.
        if (!file) { // Checks the condition before running the next script step.
            // ----- Form validation -----
            setError('imageError', ''); // Runs this JavaScript step for the page interaction.
            return true; // Returns the computed value to the caller.
        }

        if (allowed.indexOf(file.type) === -1 && !allowedExtensions.test(file.name)) { // Checks the condition before running the next script step.
            setError('imageError', 'Upload a JPG, PNG, or WebP image.'); // Runs this JavaScript step for the page interaction.
            return false; // Returns the computed value to the caller.
        }

        if (file.size > 5 * 1024 * 1024) { // Checks the condition before running the next script step.
            setError('imageError', 'Image files must be smaller than 5 MB.'); // Runs this JavaScript step for the page interaction.
            return false; // Returns the computed value to the caller.
        }

        setError('imageError', ''); // Runs this JavaScript step for the page interaction.
        return true; // Returns the computed value to the caller.
    }

    // ----- Helpers -----
    function setPlaceholder() { // Defines the setPlaceholder helper function.
        if (!previewHost) return; // Checks the condition before running the next script step.
        // ----- Icon setup -----
        previewHost.outerHTML = '<div class="pw-admin-img-placeholder" id="adminImgEl"><i data-lucide="sprout" style="width:72px;height:72px;opacity:.45;"></i></div>'; // Updates previewHost.outerHTML for the current script state.
        // ----- DOM references -----
        previewHost = document.getElementById('adminImgEl'); // Finds a page element needed by the script.
        // ----- Icon setup -----
        if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.
    }

    // ----- Helpers -----
    function setImage(src, label) { // Defines the setImage helper function.
        if (!previewHost) return; // Checks the condition before running the next script step.
        // ----- Producer logic -----
        previewHost.outerHTML = '<img id="adminImgEl" src="' + src + '" alt="Producer image preview" />'; // Updates previewHost.outerHTML for the current script state.
        // ----- DOM references -----
        previewHost = document.getElementById('adminImgEl'); // Finds a page element needed by the script.
        if (pathLabel) pathLabel.textContent = label; // Checks the condition before running the next script step.
    }

    // ----- Producer logic -----
    if (vatToggle) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        vatToggle.addEventListener('change', toggleVATNumber); // Registers an event handler for user or browser interaction.
        // ----- Producer logic -----
        toggleVATNumber(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Event wiring -----
    if (nameInput && nameLabel) { // Checks the condition before running the next script step.
        nameLabel.textContent = nameInput.value || 'New producer'; // Updates nameLabel.textContent for the current script state.
        nameInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            var value = nameInput.value.trim() || 'Producer name'; // Stores the value value for later script logic.
            // ----- Producer logic -----
            nameLabel.textContent = value === 'Producer name' ? 'New producer' : value; // Runs this JavaScript step for the page interaction.
            // ----- State updates -----
            if (previewName) previewName.textContent = value; // Checks the condition before running the next script step.
            // ----- Producer logic -----
            if (previewAvatar) previewAvatar.textContent = value.charAt(0).toUpperCase() || '?'; // Checks the condition before running the next script step.
        });
        // ----- Event wiring -----
        nameInput.addEventListener('blur', validateName); // Registers an event handler for user or browser interaction.
    }
    if (emailInput) { // Checks the condition before running the next script step.
        emailInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            if (previewEmailText) previewEmailText.textContent = emailInput.value.trim() || '-'; // Checks the condition before running the next script step.
        });
        emailInput.addEventListener('blur', validateEmail); // Registers an event handler for user or browser interaction.
    }
    if (infoInput) { // Checks the condition before running the next script step.
        infoInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            if (previewInfo) previewInfo.textContent = infoInput.value.trim() || 'No bio yet.'; // Checks the condition before running the next script step.
        });
        infoInput.addEventListener('blur', validateInfo); // Registers an event handler for user or browser interaction.
    }
    if (vatInput) vatInput.addEventListener('blur', validateVAT); // Registers an event handler for user or browser interaction.
    if (passwordInput) passwordInput.addEventListener('blur', validatePassword); // Registers an event handler for user or browser interaction.
    if (confirmPasswordInput) confirmPasswordInput.addEventListener('blur', validateConfirmPassword); // Registers an event handler for user or browser interaction.

    if (pathInput) { // Checks the condition before running the next script step.
        pathInput.addEventListener('input', function () { // Registers an event handler for user or browser interaction.
            if (uploadInput && uploadInput.files && uploadInput.files.length > 0) return; // Checks the condition before running the next script step.
            var src = pathInput.value.trim(); // Stores the src value for later script logic.
            if (src) setImage(src, src); // Checks the condition before running the next script step.
            else { // Handles the fallback branch for the previous condition.
                setPlaceholder(); // Runs this JavaScript step for the page interaction.
                if (pathLabel) pathLabel.textContent = 'no image selected'; // Checks the condition before running the next script step.
            }
        });
    }

    if (uploadInput) { // Checks the condition before running the next script step.
        uploadInput.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
            // ----- Form validation -----
            if (!validateImage()) return; // Checks the condition before running the next script step.
            // ----- Event wiring -----
            var file = uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null; // Stores the file value for later script logic.
            if (!file) { // Checks the condition before running the next script step.
                var src = pathInput ? pathInput.value.trim() : ''; // Stores the src value for later script logic.
                if (src) setImage(src, src); // Checks the condition before running the next script step.
                else { // Handles the fallback branch for the previous condition.
                    setPlaceholder(); // Runs this JavaScript step for the page interaction.
                    if (pathLabel) pathLabel.textContent = 'no image selected'; // Checks the condition before running the next script step.
                }
                return; // Returns the computed value to the caller.
            }

            setImage(URL.createObjectURL(file), file.name); // Runs this JavaScript step for the page interaction.
        });
    }

    if (form) { // Checks the condition before running the next script step.
        form.addEventListener('submit', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Form validation -----
            var valid = validateName() & validateEmail() & validateInfo() & validateVAT() & validatePassword() & validateConfirmPassword() & validateImage(); // Stores the valid value for later script logic.
            if (!valid) e.preventDefault(); // Stops the browser's default action for this event.
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
