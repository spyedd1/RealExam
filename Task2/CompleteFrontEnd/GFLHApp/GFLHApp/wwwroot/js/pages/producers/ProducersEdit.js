(function () { // IIFE — keeps all variables out of global scope
    'use strict'; // Enable strict mode

    if (window.lucide) lucide.createIcons(); // Initialise lucide icons if the library is loaded

    /* ── Live preview ──────────────────────────────────────────── */
    var nameInput = document.getElementById('liveNameInput');    // Name input field
    var emailInput = document.getElementById('liveEmailInput');   // Email input field
    var infoInput = document.getElementById('liveInfoInput');    // Bio textarea
    var previewName = document.getElementById('previewName');      // Preview name element
    var previewAvatar = document.getElementById('previewAvatar');    // Preview avatar element
    var previewEmailText = document.getElementById('previewEmailText'); // Preview email text element
    var previewInfo = document.getElementById('previewInfo');      // Preview bio element

    if (nameInput) { // Guard: only bind if element exists
        nameInput.addEventListener('input', function () {
            var v = nameInput.value.trim() || 'Producer name'; // Fall back to placeholder if empty
            if (previewName) previewName.textContent = v;                          // Update preview name
            if (previewAvatar) previewAvatar.textContent = v.charAt(0).toUpperCase() || '?'; // Update avatar initial
        });
    }
    if (emailInput && previewEmailText) { // Guard: only bind if both elements exist
        emailInput.addEventListener('input', function () {
            previewEmailText.textContent = emailInput.value.trim() || '—'; // Update preview email or show dash
        });
    }
    if (infoInput && previewInfo) { // Guard: only bind if both elements exist
        infoInput.addEventListener('input', function () {
            previewInfo.textContent = infoInput.value.trim() || 'No bio yet.'; // Update preview bio or show fallback
        });
    }

    /* ── VAT toggle reveals VAT number field ───────────────────── */
    var vatToggle = document.getElementById('vatToggle');      // VAT checkbox toggle
    var vatNumberField = document.getElementById('vatNumberField'); // Collapsible VAT number field
    var previewVat = document.getElementById('previewVat');     // VAT badge in preview card

    function syncVat() {
        var on = vatToggle && vatToggle.checked;                          // True if toggle is checked
        if (vatNumberField) vatNumberField.classList.toggle('is-open', on); // Show/hide VAT number field
        if (previewVat) previewVat.classList.toggle('is-visible', on);  // Show/hide VAT badge in preview
    }
    if (vatToggle) {
        vatToggle.addEventListener('change', syncVat); // Re-sync on toggle change
        syncVat(); // Run once on load to match initial state
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

    /* ── Show validation summary if it has errors ──────────────── */
    var summary = document.querySelector('[data-valmsg-summary]'); // Validation summary element
    if (summary && summary.querySelectorAll('li').length > 0) {    // Only show if errors exist
        summary.removeAttribute('hidden');                         // Make summary visible
    }

}());