// ----- Accessibility -----
(function () { // IIFE — keeps all variables out of global scope // Starts an isolated script scope for this page.

    // ----- DOM references -----
    document.querySelectorAll('.gw-polaroid[data-burst]').forEach(function (el) { // All polaroids with data-burst attribute // Finds a page element needed by the script.
        // ----- Event wiring -----
        el.addEventListener('mouseenter', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = el.getBoundingClientRect();                          // Get polaroid position // Stores the r value for later script logic.
            burst(r.left + r.width / 2, r.top + r.height / 2, 18);    // Burst 18 particles from centre // Runs this JavaScript step for the page interaction.
        });
    });

    var colors = ['#16A34A', '#F59E0B', '#F97316', '#EC4899', '#8B5CF6', '#4ADE80', '#FEF08A', '#FB923C', '#A78BFA', '#6EE7B7']; // Multi-colour palette for particles // Stores the colors value for later script logic.
    var lastSpawn = 0; // Timestamp of last spawn, used to throttle trail particles // Stores the lastSpawn value for later script logic.

    function spawnParticle(cx, cy) { // Defines the spawnParticle helper function.
        // ----- DOM references -----
        var el = document.createElement('div');                         // Create particle element // Stores the el value for later script logic.
        // ----- Helpers -----
        var color = colors[Math.floor(Math.random() * colors.length)];     // Random colour from palette // Stores the color value for later script logic.
        var size = 7 + Math.random() * 11;                                // Random size 7–18px // Stores the size value for later script logic.
        var angle = Math.random() * Math.PI * 2;                           // Random direction // Stores the angle value for later script logic.
        var speed = 80 + Math.random() * 180;                              // Random travel distance // Stores the speed value for later script logic.
        var dx = Math.cos(angle) * speed;                               // Horizontal displacement // Stores the dx value for later script logic.
        var dy = Math.sin(angle) * speed - 60;                          // Vertical displacement, biased upward // Stores the dy value for later script logic.
        var dur = 650 + Math.random() * 500;                             // Random animation duration // Stores the dur value for later script logic.

        // ----- State updates -----
        el.style.cssText = // Updates inline style for a dynamic UI state.
            'position:fixed;left:' + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
            'width:' + size + 'px;height:' + size + 'px;border-radius:50%;background:' + color + ';' + // Runs this JavaScript step for the page interaction.
            'pointer-events:none;z-index:9999;';                           // Circular, non-interactive, on top of everything // Runs this JavaScript step for the page interaction.
        // ----- DOM references -----
        document.body.appendChild(el); // Add to DOM // Runs this JavaScript step for the page interaction.

        // ----- Animation -----
        el.animate( // Runs this JavaScript step for the page interaction.
            [{ transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
            { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0.05)', opacity: 0 }], // Fly out and shrink // Runs this JavaScript step for the page interaction.
            { duration: dur, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
        // ----- Helpers -----
        ).onfinish = function () { el.remove(); }; // Remove from DOM when done // Updates ).onfinish for the current script state.
    }

    function burst(cx, cy, n) { // Defines the burst helper function.
        for (var i = 0; i < n; i++) { // Loops through matching items for this script step.
            (function (i) { // Starts an isolated script scope for this page.
                setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    spawnParticle( // Runs this JavaScript step for the page interaction.
                        cx + (Math.random() - 0.5) * 80, // Random horizontal spread // Runs this JavaScript step for the page interaction.
                        cy + (Math.random() - 0.5) * 80  // Random vertical spread // Runs this JavaScript step for the page interaction.
                    );
                }, i * 18); // Stagger each particle by 18ms // Runs this JavaScript step for the page interaction.
            })(i); // Runs this JavaScript step for the page interaction.
        }
    }

    // ----- DOM references -----
    document.querySelectorAll('.pw-card__burst-target').forEach(function (el) { // All card image panels // Finds a page element needed by the script.
        // ----- Event wiring -----
        el.addEventListener('mouseenter', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = el.getBoundingClientRect();                          // Get panel position // Stores the r value for later script logic.
            burst(r.left + r.width / 2, r.top + r.height / 2, 22);    // Burst 22 particles on enter // Runs this JavaScript step for the page interaction.
        });
        // ----- Event wiring -----
        el.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var now = Date.now(); // Stores the now value for later script logic.
            if (now - lastSpawn < 80) return; // Throttle: max one trail spawn per 80ms // Checks the condition before running the next script step.
            lastSpawn = now; // Updates lastSpawn for the current script state.
            for (var i = 0; i < 2; i++) // Spawn 2 trail particles per move event // Loops through matching items for this script step.
                spawnParticle( // Runs this JavaScript step for the page interaction.
                    e.clientX + (Math.random() - 0.5) * 16, // Small random spread around cursor // Runs this JavaScript step for the page interaction.
                    e.clientY + (Math.random() - 0.5) * 16 // Runs this JavaScript step for the page interaction.
                );
        });
    });

    // ----- DOM references -----
    document.querySelectorAll('.pw-card--producer').forEach(function (card) { // All producer cards // Finds a page element needed by the script.
        // ----- Event wiring -----
        card.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var rect = card.getBoundingClientRect(); // Stores the rect value for later script logic.
            var x = (e.clientX - rect.left) / rect.width - 0.5; // Normalised X within card (-0.5 to 0.5) // Stores the x value for later script logic.
            var y = (e.clientY - rect.top) / rect.height - 0.5; // Normalised Y within card (-0.5 to 0.5) // Stores the y value for later script logic.
            // ----- State updates -----
            card.style.transform = 'translateY(-6px) rotateY(' + (x * 8) + 'deg) rotateX(' + (-y * 5) + 'deg)'; // Apply 3D tilt // Updates inline style for a dynamic UI state.
        });
        // ----- Event wiring -----
        card.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            card.style.transform = ''; // Reset tilt on mouse leave // Updates inline style for a dynamic UI state.
        });
    });
})();
