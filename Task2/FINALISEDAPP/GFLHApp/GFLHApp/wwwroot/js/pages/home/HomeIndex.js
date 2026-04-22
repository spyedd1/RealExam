// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var frame = document.querySelector('.pw-spotlight__img-frame'); // Stores the frame DOM element reference.
    if (!frame) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    var colors = ['#16A34A','#F59E0B','#F97316','#EC4899','#8B5CF6','#4ADE80','#FEF08A','#FB923C','#A78BFA']; // Stores the colors value for later script logic.
    var lastSpawn = 0; // Stores the lastSpawn value for later script logic.

    function spawnParticle(cx, cy) { // Defines the spawnParticle helper function.
        // ----- DOM references -----
        var el = document.createElement('div'); // Stores the el value for later script logic.
        // ----- Helpers -----
        var color = colors[Math.floor(Math.random() * colors.length)]; // Stores the color value for later script logic.
        var size  = 9 + Math.random() * 13; // Stores the size value for later script logic.
        var angle = Math.random() * Math.PI * 2; // Stores the angle value for later script logic.
        var speed = 100 + Math.random() * 220; // Stores the speed value for later script logic.
        var dx    = Math.cos(angle) * speed; // Stores the dx value for later script logic.
        var dy    = Math.sin(angle) * speed - 80; // Stores the dy value for later script logic.
        var dur   = 750 + Math.random() * 550; // Stores the dur value for later script logic.

        // ----- State updates -----
        el.style.cssText = // Updates inline style for a dynamic UI state.
            'position:fixed;' + // Runs this JavaScript step for the page interaction.
            'left:' + (cx - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
            'top:'  + (cy - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
            'width:' + size + 'px;height:' + size + 'px;' + // Runs this JavaScript step for the page interaction.
            'border-radius:50%;background:' + color + ';' + // Runs this JavaScript step for the page interaction.
            'pointer-events:none;z-index:9999;'; // Runs this JavaScript step for the page interaction.

        // ----- DOM references -----
        document.body.appendChild(el); // Runs this JavaScript step for the page interaction.

        // ----- Animation -----
        el.animate( // Runs this JavaScript step for the page interaction.
            [
                { transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
                { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0.05)', opacity: 0 } // Runs this JavaScript step for the page interaction.
            ],
            { duration: dur, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
        ).onfinish = function() { el.remove(); }; // Updates ).onfinish for the current script state.
    }

    // ----- Helpers -----
    function burst(cx, cy, count) { // Defines the burst helper function.
        for (var i = 0; i < count; i++) { // Loops through matching items for this script step.
            // ----- Startup -----
            (function(i) { // Starts an isolated script scope for this page.
                // ----- Animation -----
                setTimeout(function() { // Starts a JavaScript block for the current control flow.
                    spawnParticle( // Runs this JavaScript step for the page interaction.
                        cx + (Math.random() - 0.5) * 100, // Runs this JavaScript step for the page interaction.
                        cy + (Math.random() - 0.5) * 100 // Runs this JavaScript step for the page interaction.
                    );
                }, i * 20); // Runs this JavaScript step for the page interaction.
            })(i); // Runs this JavaScript step for the page interaction.
        }
    }

    // ----- Event wiring -----
    frame.addEventListener('mouseenter', function() { // Registers an event handler for user or browser interaction.
        // ----- Helpers -----
        var r = frame.getBoundingClientRect(); // Stores the r value for later script logic.
        burst(r.left + r.width / 2, r.top + r.height / 2, 26); // Runs this JavaScript step for the page interaction.
    });

    // ----- Event wiring -----
    frame.addEventListener('mousemove', function(e) { // Registers an event handler for user or browser interaction.
        // ----- Helpers -----
        var now = Date.now(); // Stores the now value for later script logic.
        if (now - lastSpawn < 75) return; // Checks the condition before running the next script step.
        lastSpawn = now; // Updates lastSpawn for the current script state.
        for (var i = 0; i < 3; i++) // Loops through matching items for this script step.
            spawnParticle( // Runs this JavaScript step for the page interaction.
                e.clientX + (Math.random() - 0.5) * 20, // Runs this JavaScript step for the page interaction.
                e.clientY + (Math.random() - 0.5) * 20 // Runs this JavaScript step for the page interaction.
            );
    });
})();
