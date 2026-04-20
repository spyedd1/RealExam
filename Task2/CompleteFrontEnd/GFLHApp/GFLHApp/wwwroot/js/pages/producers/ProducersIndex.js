(function () { // IIFE — keeps all variables out of global scope

    /* ── Particle burst on polaroid hover ── */
    document.querySelectorAll('.gw-polaroid[data-burst]').forEach(function (el) { // All polaroids with data-burst attribute
        el.addEventListener('mouseenter', function () {
            var r = el.getBoundingClientRect();                          // Get polaroid position
            burst(r.left + r.width / 2, r.top + r.height / 2, 18);    // Burst 18 particles from centre
        });
    });

    /* ── Particle burst on producer card image hover ── */
    var colors = ['#16A34A', '#F59E0B', '#F97316', '#EC4899', '#8B5CF6', '#4ADE80', '#FEF08A', '#FB923C', '#A78BFA', '#6EE7B7']; // Multi-colour palette for particles
    var lastSpawn = 0; // Timestamp of last spawn, used to throttle trail particles

    function spawnParticle(cx, cy) {
        var el = document.createElement('div');                         // Create particle element
        var color = colors[Math.floor(Math.random() * colors.length)];     // Random colour from palette
        var size = 7 + Math.random() * 11;                                // Random size 7–18px
        var angle = Math.random() * Math.PI * 2;                           // Random direction
        var speed = 80 + Math.random() * 180;                              // Random travel distance
        var dx = Math.cos(angle) * speed;                               // Horizontal displacement
        var dy = Math.sin(angle) * speed - 60;                          // Vertical displacement, biased upward
        var dur = 650 + Math.random() * 500;                             // Random animation duration

        el.style.cssText =
            'position:fixed;left:' + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;' +
            'width:' + size + 'px;height:' + size + 'px;border-radius:50%;background:' + color + ';' +
            'pointer-events:none;z-index:9999;';                           // Circular, non-interactive, on top of everything
        document.body.appendChild(el); // Add to DOM

        el.animate(
            [{ transform: 'translate(0,0) scale(1)', opacity: 1 },
            { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0.05)', opacity: 0 }], // Fly out and shrink
            { duration: dur, easing: 'ease-out', fill: 'forwards' }
        ).onfinish = function () { el.remove(); }; // Remove from DOM when done
    }

    function burst(cx, cy, n) {
        for (var i = 0; i < n; i++) {
            (function (i) {
                setTimeout(function () {
                    spawnParticle(
                        cx + (Math.random() - 0.5) * 80, // Random horizontal spread
                        cy + (Math.random() - 0.5) * 80  // Random vertical spread
                    );
                }, i * 18); // Stagger each particle by 18ms
            })(i);
        }
    }

    document.querySelectorAll('.pw-card__burst-target').forEach(function (el) { // All card image panels
        el.addEventListener('mouseenter', function () {
            var r = el.getBoundingClientRect();                          // Get panel position
            burst(r.left + r.width / 2, r.top + r.height / 2, 22);    // Burst 22 particles on enter
        });
        el.addEventListener('mousemove', function (e) {
            var now = Date.now();
            if (now - lastSpawn < 80) return; // Throttle: max one trail spawn per 80ms
            lastSpawn = now;
            for (var i = 0; i < 2; i++) // Spawn 2 trail particles per move event
                spawnParticle(
                    e.clientX + (Math.random() - 0.5) * 16, // Small random spread around cursor
                    e.clientY + (Math.random() - 0.5) * 16
                );
        });
    });

    /* ── Tilt effect on producer cards ── */
    document.querySelectorAll('.pw-card--producer').forEach(function (card) { // All producer cards
        card.addEventListener('mousemove', function (e) {
            var rect = card.getBoundingClientRect();
            var x = (e.clientX - rect.left) / rect.width - 0.5; // Normalised X within card (-0.5 to 0.5)
            var y = (e.clientY - rect.top) / rect.height - 0.5; // Normalised Y within card (-0.5 to 0.5)
            card.style.transform = 'translateY(-6px) rotateY(' + (x * 8) + 'deg) rotateX(' + (-y * 5) + 'deg)'; // Apply 3D tilt
        });
        card.addEventListener('mouseleave', function () {
            card.style.transform = ''; // Reset tilt on mouse leave
        });
    });
})();