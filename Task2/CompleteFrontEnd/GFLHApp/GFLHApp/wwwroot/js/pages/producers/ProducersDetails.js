(function () { // IIFE — keeps all variables out of global scope

    var colors = ['#16A34A', '#4ade80', '#86efac', '#d9f99d', '#6ee7b7', '#a7f3d0', '#bbf7d0', '#34d399', '#10b981']; // Green palette for particles

    /* ── Particle helpers ─────────────────────────────── */
    function spawnParticle(cx, cy) {
        var el = document.createElement('div');                         // Create particle element
        var color = colors[Math.floor(Math.random() * colors.length)];     // Random colour from palette
        var size = 7 + Math.random() * 11;                                // Random size 7–18px
        var angle = Math.random() * Math.PI * 2;                           // Random direction
        var speed = 80 + Math.random() * 160;                              // Random travel distance
        var dx = Math.cos(angle) * speed;                               // Horizontal displacement
        var dy = Math.sin(angle) * speed - 55;                          // Vertical displacement, biased upward
        var dur = 600 + Math.random() * 450;                             // Random animation duration
        el.style.cssText =
            'position:fixed;left:' + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;' +
            'width:' + size + 'px;height:' + size + 'px;border-radius:50%;background:' + color + ';' +
            'pointer-events:none;z-index:9999;';                           // Circular, non-interactive, on top of everything
        document.body.appendChild(el);                                     // Add to DOM
        el.animate(
            [{ transform: 'translate(0,0) scale(1)', opacity: 1 },
            { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0.05)', opacity: 0 }], // Fly out and shrink
            { duration: dur, easing: 'ease-out', fill: 'forwards' }
        ).onfinish = function () { el.remove(); };                            // Remove from DOM when done
    }

    function burst(cx, cy, n) {
        for (var i = 0; i < n; i++) {
            (function (i) {
                setTimeout(function () {
                    spawnParticle(cx + (Math.random() - .5) * 70, cy + (Math.random() - .5) * 70); // Slight random spread
                }, i * 18);
            })(i); // Stagger each particle by 18ms
        }
    }

    /* ── Mouse parallax on hero floating icons ────────── */
    var floats = document.querySelectorAll('.pd-hero__float'); // All floating icons
    var depths = [14, 20, 9, 17];                             // Parallax depth per icon
    document.addEventListener('mousemove', function (e) {
        var cx = window.innerWidth / 2;     // Horizontal centre of viewport
        var cy = window.innerHeight / 2;     // Vertical centre of viewport
        var dx = (e.clientX - cx) / cx;      // Normalised horizontal offset (-1 to 1)
        var dy = (e.clientY - cy) / cy;      // Normalised vertical offset (-1 to 1)
        floats.forEach(function (el, i) {
            var d = depths[i] || 12;          // Depth for this icon
            el.style.transform = 'translate(' + (dx * d) + 'px,' + (dy * d) + 'px)'; // Apply parallax shift
        });
    });

    /* ── Avatar burst on click ────────────────────────── */
    var avatar = document.querySelector('.pd-hero__avatar'); // Producer avatar element
    if (avatar) {
        avatar.addEventListener('click', function () {
            var r = avatar.getBoundingClientRect();                    // Get avatar position
            burst(r.left + r.width / 2, r.top + r.height / 2, 28);       // Burst 28 particles from centre
        });
    }

    /* ── Particle burst on product card hover ─────────── */
    var lastBurst = 0; // Timestamp of last burst, used to throttle
    document.querySelectorAll('.pd-product-card').forEach(function (card) {
        card.addEventListener('mouseenter', function () {
            var now = Date.now();
            if (now - lastBurst < 300) return; // Throttle: max one burst per 300ms
            lastBurst = now;
            var r = card.getBoundingClientRect();          // Card position
            burst(r.left + r.width / 2, r.top + 30, 14);   // Burst near top of card
        });
    });

    /* ── 3-D tilt on product cards ────────────────────── */
    document.querySelectorAll('.pd-product-card').forEach(function (card) {
        card.style.transition = 'transform .25s ease, box-shadow .25s ease'; // Smooth tilt transition
        card.addEventListener('mousemove', function (e) {
            var r = card.getBoundingClientRect();
            var x = (e.clientX - r.left) / r.width - 0.5; // Normalised X within card (-0.5 to 0.5)
            var y = (e.clientY - r.top) / r.height - 0.5; // Normalised Y within card (-0.5 to 0.5)
            card.style.transform =
                'translateY(-6px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 7) + 'deg)'; // Apply 3D tilt
        });
        card.addEventListener('mouseleave', function () {
            card.style.transform = ''; // Reset tilt on mouse leave
        });
    });

    /* ── Ripple on fact cards ─────────────────────────── */
    document.querySelectorAll('.pd-fact-card').forEach(function (card) {
        card.style.position = 'relative'; // Required for ripple positioning
        card.addEventListener('click', function (e) {
            var r = card.getBoundingClientRect();
            var size = Math.max(r.width, r.height) * 1.4; // Ripple large enough to cover card
            var el = document.createElement('span');
            el.className = 'pd-ripple';                    // Styled via CSS
            el.style.cssText =
                'width:' + size + 'px;height:' + size + 'px;' +
                'left:' + (e.clientX - r.left - size / 2) + 'px;' +
                'top:' + (e.clientY - r.top - size / 2) + 'px;';  // Centre ripple on click point
            card.appendChild(el);
            setTimeout(function () { el.remove(); }, 600);  // Remove after animation completes
        });
    });

    /* ── Ripple on stat tiles ─────────────────────────── */
    document.querySelectorAll('.pd-stat-tile').forEach(function (tile) {
        tile.style.position = 'relative'; tile.style.overflow = 'hidden'; // Required for ripple clipping
        tile.addEventListener('click', function (e) {
            var r = tile.getBoundingClientRect();
            var sz = Math.max(r.width, r.height) * 1.4;  // Ripple size
            var el = document.createElement('span');
            el.className = 'pd-ripple';                   // Styled via CSS
            el.style.cssText =
                'width:' + sz + 'px;height:' + sz + 'px;' +
                'left:' + (e.clientX - r.left - sz / 2) + 'px;' +
                'top:' + (e.clientY - r.top - sz / 2) + 'px;';   // Centre on click point
            tile.appendChild(el);
            setTimeout(function () { el.remove(); }, 600); // Remove after animation completes
        });
    });

    /* ── Animated stat counters ───────────────────────── */
    var statEls = document.querySelectorAll('.pd-stat-tile__value[data-target]'); // All counter elements
    var counted = false;                                                           // Guard: only run once
    function runCounters() {
        if (counted) return; counted = true; // Prevent re-running on repeated intersections
        statEls.forEach(function (el) {
            var target = parseInt(el.dataset.target, 10); // Target number from data attribute
            if (isNaN(target) || target === 0) return;    // Skip zero or invalid values
            var startTime = null, dur = 900;               // 900ms animation duration
            (function step(ts) {
                if (!startTime) startTime = ts;
                var p = Math.min((ts - startTime) / dur, 1); // Progress 0–1
                var ease = 1 - Math.pow(1 - p, 3);              // Cubic ease-out
                el.textContent = Math.round(ease * target);      // Update displayed number
                if (p < 1) requestAnimationFrame(step);          // Continue until complete
                else el.textContent = target;                    // Snap to final value
            })(performance.now());
        });
    }
    new IntersectionObserver(function (entries) {
        entries.forEach(function (e) { if (e.isIntersecting) runCounters(); }); // Start counters when strip enters viewport
    }, { threshold: 0.3 }).observe(
        document.querySelector('.pd-stats-strip') || document.body // Observe the stats strip element
    );
})();