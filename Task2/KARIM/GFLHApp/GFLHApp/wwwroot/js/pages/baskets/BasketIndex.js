// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Icon setup -----
    if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

    // ----- DOM references -----
    document.querySelectorAll('.bk-item__remove').forEach(function (btn) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        btn.addEventListener('click', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var colors = ['#fca5a5','#f87171','#ef4444','#fecaca']; // Stores the colors value for later script logic.
            var r = btn.getBoundingClientRect(); // Stores the r value for later script logic.
            var cx = r.left + r.width / 2; // Stores the cx value for later script logic.
            var cy = r.top  + r.height / 2; // Stores the cy value for later script logic.
            for (var i = 0; i < 10; i++) { // Loops through matching items for this script step.
                (function (i) { // Starts an isolated script scope for this page.
                    setTimeout(function () { // Starts a JavaScript block for the current control flow.
                        // ----- DOM references -----
                        var el    = document.createElement('div'); // Stores the el value for later script logic.
                        // ----- Helpers -----
                        var size  = 5 + Math.random() * 7; // Stores the size value for later script logic.
                        var angle = Math.random() * Math.PI * 2; // Stores the angle value for later script logic.
                        var speed = 40 + Math.random() * 80; // Stores the speed value for later script logic.
                        var dx    = Math.cos(angle) * speed; // Stores the dx value for later script logic.
                        var dy    = Math.sin(angle) * speed - 30; // Stores the dy value for later script logic.
                        var color = colors[Math.floor(Math.random() * colors.length)]; // Stores the color value for later script logic.
                        // ----- State updates -----
                        el.style.cssText = // Updates inline style for a dynamic UI state.
                            'position:fixed;pointer-events:none;z-index:9999;border-radius:50%;' + // Runs this JavaScript step for the page interaction.
                            'width:' + size + 'px;height:' + size + 'px;background:' + color + ';' + // Runs this JavaScript step for the page interaction.
                            'left:' + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;'; // Runs this JavaScript step for the page interaction.
                        // ----- DOM references -----
                        document.body.appendChild(el); // Runs this JavaScript step for the page interaction.
                        // ----- Animation -----
                        el.animate( // Runs this JavaScript step for the page interaction.
                            [{ transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
                             { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0)', opacity: 0 }], // Runs this JavaScript step for the page interaction.
                            { duration: 450, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
                        // ----- Helpers -----
                        ).onfinish = function () { el.remove(); }; // Updates ).onfinish for the current script state.
                    }, i * 20); // Runs this JavaScript step for the page interaction.
                }(i)); // Runs this JavaScript step for the page interaction.
            }
        });
    });

    // ----- DOM references -----
    var totalEl = document.querySelector('.bk-summary__total-value'); // Stores the totalEl DOM element reference.
    // ----- Basket logic -----
    if (totalEl) { // Checks the condition before running the next script step.
        var raw = parseFloat(totalEl.textContent.replace('£', '')) || 0; // Stores the raw value for later script logic.
        // ----- Helpers -----
        var t0  = performance.now(); // Stores the t0 value for later script logic.
        var dur = 700; // Stores the dur value for later script logic.
        (function tick(now) { // Starts an isolated script scope for this page.
            var p    = Math.min((now - t0) / dur, 1); // Stores the p value for later script logic.
            var ease = 1 - Math.pow(1 - p, 3); // Stores the ease value for later script logic.
            // ----- Basket logic -----
            totalEl.textContent = '£' + (ease * raw).toFixed(2); // Updates totalEl.textContent for the current script state.
            // ----- Animation -----
            if (p < 1) requestAnimationFrame(tick); // Checks the condition before running the next script step.
        }(t0)); // Runs this JavaScript step for the page interaction.
    }

    // ----- DOM references -----
    var checkoutBtn = document.querySelector('.bk-checkout-btn'); // Stores the checkoutBtn DOM element reference.
    if (checkoutBtn) { // Checks the condition before running the next script step.
        // ----- Helpers -----
        setTimeout(function () { // Starts a JavaScript block for the current control flow.
            // ----- State updates -----
            checkoutBtn.style.animation = 'none'; // Updates inline style for a dynamic UI state.
            checkoutBtn.style.transform = 'scale(1.04)'; // Updates inline style for a dynamic UI state.
            // ----- Helpers -----
            setTimeout(function () { // Starts a JavaScript block for the current control flow.
                // ----- State updates -----
                checkoutBtn.style.transform = ''; // Updates inline style for a dynamic UI state.
                checkoutBtn.style.animation = 'bk-checkout-pulse 2.8s ease-in-out infinite'; // Updates inline style for a dynamic UI state.
            }, 300); // Runs this JavaScript step for the page interaction.
        }, 4000); // Runs this JavaScript step for the page interaction.
    }
}());
