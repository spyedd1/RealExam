(function () { // IIFE — keeps all variables out of global scope
    'use strict'; // Enable strict mode

    if (window.lucide) lucide.createIcons(); // Initialise lucide icons if the library is loaded

    /* ── Live preview ──────────────────────────────────────────── */
    var nameInput = document.getElementById('liveNameInput');    // Name input field
    var emailInput = document.getElementById('liveEmailInput');   // Email input field
    var infoInput = document.getElementById('liveInfoInput');    // Bio textarea
    var vatInput = document.getElementById('VATNumber'); // VAT number input
    var previewName = document.getElementById('previewName');      // Preview name element
    var previewAvatar = document.getElementById('previewAvatar');    // Preview avatar element
    var previewEmailText = document.getElementById('previewEmailText'); // Preview email text element
    var previewInfo = document.getElementById('previewInfo');      // Preview bio element
    var imgCard = document.getElementById('adminImgCard'); // Image preview card
    var nameDisplay = document.getElementById('adminImgName'); // Image card name
    var pathDisplay = document.getElementById('adminImgPath'); // Image path label
    var imageInput = document.getElementById('liveImageInput'); // Existing image dropdown
    var uploadInput = document.getElementById('imageUpload'); // Upload input
    var originalPath = pathDisplay && pathDisplay.textContent !== 'no image set' ? pathDisplay.textContent.trim() : '';

    function ensureErrorAfter(input, id) {
        if (!input) return null;
        var existing = document.getElementById(id);
        if (existing) return existing;
        var error = document.createElement('span');
        error.id = id;
        error.className = 'pw-field__error';
        input.insertAdjacentElement('afterend', error);
        return error;
    }

    function setError(input, id, message) {
        var node = ensureErrorAfter(input, id);
        if (node) node.textContent = message || '';
    }

    function validateName() {
        var val = nameInput ? nameInput.value.trim() : '';
        if (!val) { setError(nameInput, 'nameError', 'Producer name is required.'); return false; }
        if (val.length < 3) { setError(nameInput, 'nameError', 'Producer name must be at least 3 characters.'); return false; }
        if (val.length > 100) { setError(nameInput, 'nameError', 'Producer name must not exceed 100 characters.'); return false; }
        setError(nameInput, 'nameError', '');
        return true;
    }

    function validateEmail() {
        var val = emailInput ? emailInput.value.trim() : '';
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!val) { setError(emailInput, 'emailError', 'Email address is required.'); return false; }
        if (!emailRegex.test(val)) { setError(emailInput, 'emailError', 'Please enter a valid email address.'); return false; }
        if (val.length > 150) { setError(emailInput, 'emailError', 'Email address must not exceed 150 characters.'); return false; }
        setError(emailInput, 'emailError', '');
        return true;
    }

    function validateInfo() {
        var val = infoInput ? infoInput.value.trim() : '';
        if (!val) { setError(infoInput, 'infoError', 'Producer information is required.'); return false; }
        if (val.length < 10) { setError(infoInput, 'infoError', 'Producer information must be at least 10 characters.'); return false; }
        if (val.length > 500) { setError(infoInput, 'infoError', 'Producer information must not exceed 500 characters.'); return false; }
        setError(infoInput, 'infoError', '');
        return true;
    }

    function validateVAT() {
        if (!vatToggle || !vatToggle.checked) {
            setError(vatInput, 'vatError', '');
            return true;
        }

        var val = vatInput ? vatInput.value.trim().toUpperCase() : '';
        if (!val) { setError(vatInput, 'vatError', 'VAT number is required if VAT registered.'); return false; }
        if (!/^GB[0-9]{9}$/.test(val)) { setError(vatInput, 'vatError', 'VAT number must start with GB followed by exactly 9 digits.'); return false; }
        if (vatInput) vatInput.value = val;
        setError(vatInput, 'vatError', '');
        return true;
    }

    function validateImage() {
        var file = uploadInput && uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null;
        var error = ensureErrorAfter(uploadInput, 'imageError');
        if (!file) {
            if (error) error.textContent = '';
            return true;
        }

        if (!/\.(jpe?g|png|webp)$/i.test(file.name) && ['image/jpeg', 'image/png', 'image/webp'].indexOf(file.type) === -1) {
            if (error) error.textContent = 'Upload a JPG, PNG, or WebP image.';
            return false;
        }

        if (file.size > 5 * 1024 * 1024) {
            if (error) error.textContent = 'Image files must be smaller than 5 MB.';
            return false;
        }

        if (error) error.textContent = '';
        return true;
    }

    if (nameInput) { // Guard: only bind if element exists
        nameInput.addEventListener('input', function () {
            var v = nameInput.value.trim() || 'Producer name'; // Fall back to placeholder if empty
            if (previewName) previewName.textContent = v;                          // Update preview name
            if (previewAvatar) previewAvatar.textContent = v.charAt(0).toUpperCase() || '?'; // Update avatar initial
            if (nameDisplay) nameDisplay.textContent = v;
        });
        nameInput.addEventListener('blur', validateName);
    }
    if (emailInput && previewEmailText) { // Guard: only bind if both elements exist
        emailInput.addEventListener('input', function () {
            previewEmailText.textContent = emailInput.value.trim() || '—'; // Update preview email or show dash
        });
        emailInput.addEventListener('blur', validateEmail);
    }
    if (infoInput && previewInfo) { // Guard: only bind if both elements exist
        infoInput.addEventListener('input', function () {
            previewInfo.textContent = infoInput.value.trim() || 'No bio yet.'; // Update preview bio or show fallback
        });
        infoInput.addEventListener('blur', validateInfo);
    }

    /* ── VAT toggle reveals VAT number field ───────────────────── */
    var vatToggle = document.getElementById('vatToggle');      // VAT checkbox toggle
    var vatNumberField = document.getElementById('vatNumberField'); // Collapsible VAT number field
    var previewVat = document.getElementById('previewVat');     // VAT badge in preview card

    function syncVat() {
        var on = vatToggle && vatToggle.checked;                          // True if toggle is checked
        if (vatNumberField) vatNumberField.classList.toggle('is-open', on); // Show/hide VAT number field
        if (previewVat) previewVat.classList.toggle('is-visible', on);  // Show/hide VAT badge in preview
        if (!on && vatInput) {
            vatInput.value = '';
            setError(vatInput, 'vatError', '');
        }
    }
    if (vatToggle) {
        vatToggle.addEventListener('change', syncVat); // Re-sync on toggle change
        syncVat(); // Run once on load to match initial state
    }
    if (vatInput) vatInput.addEventListener('blur', validateVAT);

    function renderPlaceholder() {
        if (!imgCard) return;
        var existing = imgCard.querySelector('img');
        if (existing) {
            var ph = document.createElement('div');
            ph.className = 'pw-admin-img-placeholder';
            ph.id = 'adminImgEl';
            ph.innerHTML = '<i data-lucide="sprout" style="width:72px;height:72px;opacity:.45;"></i>';
            existing.replaceWith(ph);
            if (window.lucide) lucide.createIcons();
        }
    }

    function renderImage(src) {
        if (!imgCard) return;
        var existing = imgCard.querySelector('img');
        var placeholder = imgCard.querySelector('.pw-admin-img-placeholder');

        if (existing) {
            existing.src = src;
            existing.alt = nameInput ? (nameInput.value || 'Producer image') : 'Producer image';
            return;
        }

        var img = document.createElement('img');
        img.alt = nameInput ? (nameInput.value || 'Producer image') : 'Producer image';
        img.src = src;
        if (placeholder) placeholder.replaceWith(img);
        else imgCard.insertBefore(img, imgCard.querySelector('.pw-admin-img-meta'));
    }

    if (imageInput) {
        imageInput.addEventListener('input', function () {
            if (uploadInput && uploadInput.files && uploadInput.files.length > 0) return;

            var src = imageInput.value.trim();
            if (pathDisplay) pathDisplay.textContent = src || originalPath || 'no image set';

            if (src) renderImage(src);
            else if (originalPath) renderImage(originalPath);
            else renderPlaceholder();
        });
    }

    if (uploadInput) {
        uploadInput.addEventListener('change', function () {
            if (!validateImage()) return;
            var file = uploadInput.files && uploadInput.files.length > 0 ? uploadInput.files[0] : null;

            if (!file) {
                var src = imageInput ? imageInput.value.trim() : '';
                if (src) {
                    renderImage(src);
                    if (pathDisplay) pathDisplay.textContent = src;
                } else if (originalPath) {
                    renderImage(originalPath);
                    if (pathDisplay) pathDisplay.textContent = originalPath;
                } else {
                    renderPlaceholder();
                    if (pathDisplay) pathDisplay.textContent = 'no image set';
                }
                return;
            }

            renderImage(URL.createObjectURL(file));
            if (pathDisplay) pathDisplay.textContent = file.name;
        });
    }

    /* ── 3-D tilt on preview card ──────────────────────────────── */
    var card = document.getElementById('previewCard'); // Preview card element
    if (card) {
        var ticking = false, lastE = {}; // rAF throttle flag and last mouse event cache
        card.addEventListener('mousemove', function (e) {
            lastE = e; // Cache latest mouse event
            if (ticking) return; // Skip if rAF already queued
            ticking = true;
            requestAnimationFrame(function () {
                var r = card.getBoundingClientRect();
                var x = ((lastE.clientX - r.left) / r.width - 0.5) * 2; // Normalised X (-1 to 1)
                var y = ((lastE.clientY - r.top) / r.height - 0.5) * 2; // Normalised Y (-1 to 1)
                card.style.transform = 'perspective(800px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 8) + 'deg) scale(1.02)'; // Apply 3D tilt and slight scale
                card.style.boxShadow = '0 16px 48px rgba(0,0,0,.14)'; // Deepen shadow on hover
                ticking = false; // Allow next rAF
            });
        }, { passive: true }); // Passive listener for scroll performance
        card.addEventListener('mouseleave', function () {
            card.style.transform = ''; // Reset tilt on mouse leave
            card.style.boxShadow = ''; // Reset shadow on mouse leave
        });
    }

    if (imgCard) {
        var imgTicking = false, imgLastE = {};
        imgCard.addEventListener('mousemove', function (e) {
            imgLastE = e;
            if (imgTicking) return;
            imgTicking = true;
            requestAnimationFrame(function () {
                var r = imgCard.getBoundingClientRect();
                var x = ((imgLastE.clientX - r.left) / r.width - 0.5) * 2;
                var y = ((imgLastE.clientY - r.top) / r.height - 0.5) * 2;
                imgCard.style.transform = 'perspective(800px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 8) + 'deg) scale(1.02)';
                imgCard.style.boxShadow = '0 16px 48px rgba(0,0,0,.14)';
                imgTicking = false;
            });
        }, { passive: true });
        imgCard.addEventListener('mouseleave', function () {
            imgCard.style.transform = '';
            imgCard.style.boxShadow = '';
        });
    }

    var form = document.getElementById('editProducerForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            var valid = validateName() & validateEmail() & validateInfo() & validateVAT() & validateImage();
            if (!valid) e.preventDefault();
        });
    }

    /* ── Show validation summary if it has errors ──────────────── */
    var summary = document.querySelector('[data-valmsg-summary]'); // Validation summary element
    if (summary && summary.querySelectorAll('li').length > 0) {    // Only show if errors exist
        summary.removeAttribute('hidden');                         // Make summary visible
    }

}());
