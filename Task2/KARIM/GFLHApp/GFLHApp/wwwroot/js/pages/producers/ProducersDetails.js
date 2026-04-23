// ----- Accessibility -----
(function () { // IIFE — keeps all variables out of global scope // Starts an isolated script scope for this page.

    // ----- Helpers -----
    var colors = ['#16A34A', '#4ade80', '#86efac', '#d9f99d', '#6ee7b7', '#a7f3d0', '#bbf7d0', '#34d399', '#10b981']; // Green palette for particles // Stores the colors value for later script logic.

    function spawnParticle(cx, cy) { // Defines the spawnParticle helper function.
        // ----- DOM references -----
        var el = document.createElement('div');                         // Create particle element // Stores the el value for later script logic.
        // ----- Helpers -----
        var color = colors[Math.floor(Math.random() * colors.length)];     // Random colour from palette // Stores the color value for later script logic.
        var size = 7 + Math.random() * 11;                                // Random size 7–18px // Stores the size value for later script logic.
        var angle = Math.random() * Math.PI * 2;                           // Random direction // Stores the angle value for later script logic.
        var speed = 80 + Math.random() * 160;                              // Random travel distance // Stores the speed value for later script logic.
        var dx = Math.cos(angle) * speed;                               // Horizontal displacement // Stores the dx value for later script logic.
        var dy = Math.sin(angle) * speed - 55;                          // Vertical displacement, biased upward // Stores the dy value for later script logic.
        var dur = 600 + Math.random() * 450;                             // Random animation duration // Stores the dur value for later script logic.
        // ----- State updates -----
        el.style.cssText = // Updates inline style for a dynamic UI state.
            'position:fixed;left:' + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
            'width:' + size + 'px;height:' + size + 'px;border-radius:50%;background:' + color + ';' + // Runs this JavaScript step for the page interaction.
            'pointer-events:none;z-index:9999;';                           // Circular, non-interactive, on top of everything // Runs this JavaScript step for the page interaction.
        // ----- DOM references -----
        document.body.appendChild(el);                                     // Add to DOM // Runs this JavaScript step for the page interaction.
        // ----- Animation -----
        el.animate( // Runs this JavaScript step for the page interaction.
            [{ transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
            { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0.05)', opacity: 0 }], // Fly out and shrink // Runs this JavaScript step for the page interaction.
            { duration: dur, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
        // ----- Helpers -----
        ).onfinish = function () { el.remove(); };                            // Remove from DOM when done // Updates ).onfinish for the current script state.
    }

    function burst(cx, cy, n) { // Defines the burst helper function.
        for (var i = 0; i < n; i++) { // Loops through matching items for this script step.
            (function (i) { // Starts an isolated script scope for this page.
                setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    spawnParticle(cx + (Math.random() - .5) * 70, cy + (Math.random() - .5) * 70); // Slight random spread // Runs this JavaScript step for the page interaction.
                }, i * 18); // Runs this JavaScript step for the page interaction.
            })(i); // Stagger each particle by 18ms // Runs this JavaScript step for the page interaction.
        }
    }

    // ----- DOM references -----
    var floats = document.querySelectorAll('.pd-hero__float'); // All floating icons // Stores the floats DOM element reference.
    // ----- Helpers -----
    var depths = [14, 20, 9, 17];                             // Parallax depth per icon // Stores the depths value for later script logic.
    // ----- DOM references -----
    document.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
        // ----- Helpers -----
        var cx = window.innerWidth / 2;     // Horizontal centre of viewport // Stores the cx value for later script logic.
        var cy = window.innerHeight / 2;     // Vertical centre of viewport // Stores the cy value for later script logic.
        var dx = (e.clientX - cx) / cx;      // Normalised horizontal offset (-1 to 1) // Stores the dx value for later script logic.
        var dy = (e.clientY - cy) / cy;      // Normalised vertical offset (-1 to 1) // Stores the dy value for later script logic.
        floats.forEach(function (el, i) { // Starts a JavaScript block for the current control flow.
            var d = depths[i] || 12;          // Depth for this icon // Stores the d value for later script logic.
            // ----- State updates -----
            el.style.transform = 'translate(' + (dx * d) + 'px,' + (dy * d) + 'px)'; // Apply parallax shift // Updates inline style for a dynamic UI state.
        });
    });

    // ----- DOM references -----
    var avatar = document.querySelector('.pd-hero__avatar'); // Producer avatar element // Stores the avatar DOM element reference.
    // ----- Producer logic -----
    if (avatar) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        avatar.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
            // ----- Producer logic -----
            var r = avatar.getBoundingClientRect();                    // Get avatar position // Stores the r value for later script logic.
            burst(r.left + r.width / 2, r.top + r.height / 2, 28);       // Burst 28 particles from centre // Runs this JavaScript step for the page interaction.
        });
    }

    // ----- Helpers -----
    var lastBurst = 0; // Timestamp of last burst, used to throttle // Stores the lastBurst value for later script logic.
    // ----- DOM references -----
    document.querySelectorAll('.pd-product-card').forEach(function (card) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        card.addEventListener('mouseenter', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var now = Date.now(); // Stores the now value for later script logic.
            if (now - lastBurst < 300) return; // Throttle: max one burst per 300ms // Checks the condition before running the next script step.
            lastBurst = now; // Updates lastBurst for the current script state.
            var r = card.getBoundingClientRect();          // Card position // Stores the r value for later script logic.
            burst(r.left + r.width / 2, r.top + 30, 14);   // Burst near top of card // Runs this JavaScript step for the page interaction.
        });
    });

    // ----- DOM references -----
    document.querySelectorAll('.pd-product-card').forEach(function (card) { // Finds a page element needed by the script.
        // ----- State updates -----
        card.style.transition = 'transform .25s ease, box-shadow .25s ease'; // Smooth tilt transition // Updates inline style for a dynamic UI state.
        // ----- Event wiring -----
        card.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = card.getBoundingClientRect(); // Stores the r value for later script logic.
            var x = (e.clientX - r.left) / r.width - 0.5; // Normalised X within card (-0.5 to 0.5) // Stores the x value for later script logic.
            var y = (e.clientY - r.top) / r.height - 0.5; // Normalised Y within card (-0.5 to 0.5) // Stores the y value for later script logic.
            // ----- State updates -----
            card.style.transform = // Updates inline style for a dynamic UI state.
                'translateY(-6px) rotateY(' + (x * 10) + 'deg) rotateX(' + (-y * 7) + 'deg)'; // Apply 3D tilt // Runs this JavaScript step for the page interaction.
        });
        // ----- Event wiring -----
        card.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            card.style.transform = ''; // Reset tilt on mouse leave // Updates inline style for a dynamic UI state.
        });
    });

    // ----- DOM references -----
    document.querySelectorAll('.pd-fact-card').forEach(function (card) { // Finds a page element needed by the script.
        // ----- Form validation -----
        card.style.position = 'relative'; // Required for ripple positioning // Updates inline style for a dynamic UI state.
        // ----- Event wiring -----
        card.addEventListener('click', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = card.getBoundingClientRect(); // Stores the r value for later script logic.
            var size = Math.max(r.width, r.height) * 1.4; // Ripple large enough to cover card // Stores the size value for later script logic.
            // ----- DOM references -----
            var el = document.createElement('span'); // Stores the el value for later script logic.
            // ----- State updates -----
            el.className = 'pd-ripple';                    // Styled via CSS // Updates el.className for the current script state.
            el.style.cssText = // Updates inline style for a dynamic UI state.
                'width:' + size + 'px;height:' + size + 'px;' + // Runs this JavaScript step for the page interaction.
                'left:' + (e.clientX - r.left - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
                // ----- Event wiring -----
                'top:' + (e.clientY - r.top - size / 2) + 'px;';  // Centre ripple on click point // Runs this JavaScript step for the page interaction.
            card.appendChild(el); // Runs this JavaScript step for the page interaction.
            // ----- Helpers -----
            setTimeout(function () { el.remove(); }, 600);  // Remove after animation completes // Runs this JavaScript step for the page interaction.
        });
    });

    // ----- DOM references -----
    document.querySelectorAll('.pd-stat-tile').forEach(function (tile) { // Finds a page element needed by the script.
        // ----- Form validation -----
        tile.style.position = 'relative'; tile.style.overflow = 'hidden'; // Required for ripple clipping // Updates inline style for a dynamic UI state.
        // ----- Event wiring -----
        tile.addEventListener('click', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = tile.getBoundingClientRect(); // Stores the r value for later script logic.
            var sz = Math.max(r.width, r.height) * 1.4;  // Ripple size // Stores the sz value for later script logic.
            // ----- DOM references -----
            var el = document.createElement('span'); // Stores the el value for later script logic.
            // ----- State updates -----
            el.className = 'pd-ripple';                   // Styled via CSS // Updates el.className for the current script state.
            el.style.cssText = // Updates inline style for a dynamic UI state.
                'width:' + sz + 'px;height:' + sz + 'px;' + // Runs this JavaScript step for the page interaction.
                'left:' + (e.clientX - r.left - sz / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
                // ----- Event wiring -----
                'top:' + (e.clientY - r.top - sz / 2) + 'px;';   // Centre on click point // Runs this JavaScript step for the page interaction.
            tile.appendChild(el); // Runs this JavaScript step for the page interaction.
            // ----- Helpers -----
            setTimeout(function () { el.remove(); }, 600); // Remove after animation completes // Runs this JavaScript step for the page interaction.
        });
    });

    // ----- DOM references -----
    var statEls = document.querySelectorAll('.pd-stat-tile__value[data-target]'); // All counter elements // Stores the statEls DOM element reference.
    // ----- Helpers -----
    var counted = false;                                                           // Guard: only run once // Stores the counted value for later script logic.
    function runCounters() { // Defines the runCounters helper function.
        if (counted) return; counted = true; // Prevent re-running on repeated intersections // Checks the condition before running the next script step.
        statEls.forEach(function (el) { // Starts a JavaScript block for the current control flow.
            var target = parseInt(el.dataset.target, 10); // Target number from data attribute // Stores the target value for later script logic.
            // ----- Form validation -----
            if (isNaN(target) || target === 0) return;    // Skip zero or invalid values // Checks the condition before running the next script step.
            // ----- Helpers -----
            var startTime = null, dur = 900;               // 900ms animation duration // Stores the startTime value for later script logic.
            (function step(ts) { // Starts an isolated script scope for this page.
                if (!startTime) startTime = ts; // Checks the condition before running the next script step.
                var p = Math.min((ts - startTime) / dur, 1); // Progress 0–1 // Stores the p value for later script logic.
                var ease = 1 - Math.pow(1 - p, 3);              // Cubic ease-out // Stores the ease value for later script logic.
                el.textContent = Math.round(ease * target);      // Update displayed number // Updates el.textContent for the current script state.
                // ----- Animation -----
                if (p < 1) requestAnimationFrame(step);          // Continue until complete // Checks the condition before running the next script step.
                // ----- State updates -----
                else el.textContent = target;                    // Snap to final value // Handles the fallback branch for the previous condition.
            })(performance.now()); // Runs this JavaScript step for the page interaction.
        });
    }
    // ----- Helpers -----
    new IntersectionObserver(function (entries) { // Starts a JavaScript block for the current control flow.
        entries.forEach(function (e) { if (e.isIntersecting) runCounters(); }); // Start counters when strip enters viewport // Runs this JavaScript step for the page interaction.
    }, { threshold: 0.3 }).observe( // Runs this JavaScript step for the page interaction.
        // ----- DOM references -----
        document.querySelector('.pd-stats-strip') || document.body // Observe the stats strip element // Finds a page element needed by the script.
    );
})();
