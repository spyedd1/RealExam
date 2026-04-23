// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- Helpers -----
    (function () { // Starts an isolated script scope for this page.
        // ----- DOM references -----
        var card = document.getElementById('detailImgCard'); // Stores the card DOM element reference.
        if (!card) return; // Checks the condition before running the next script step.
        // ----- Helpers -----
        var ticking = false; // Stores the ticking value for later script logic.
        var lastE; // Stores the lastE value for later script logic.

        // ----- Event wiring -----
        card.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
            lastE = e; // Updates lastE for the current script state.
            if (ticking) return; // Checks the condition before running the next script step.
            ticking = true; // Updates ticking for the current script state.
            // ----- Helpers -----
            requestAnimationFrame(function () { // Schedules smooth visual updates for the next animation frame.
                var r  = card.getBoundingClientRect(); // Stores the r value for later script logic.
                var x  = ((lastE.clientX - r.left) / r.width  - 0.5) * 2; // Stores the x value for later script logic.
                var y  = ((lastE.clientY - r.top)  / r.height - 0.5) * 2; // Stores the y value for later script logic.
                // ----- State updates -----
                card.style.transform = // Updates inline style for a dynamic UI state.
                    'perspective(900px) rotateY(' + (x * 13) + 'deg) rotateX(' + (-y * 11) + 'deg) scale(1.03)'; // Runs this JavaScript step for the page interaction.
                card.style.boxShadow = // Updates inline style for a dynamic UI state.
                    '0 32px 72px rgba(0,0,0,.2), 0 0 0 4px var(--green-200)'; // Runs this JavaScript step for the page interaction.
                ticking = false; // Updates ticking for the current script state.
            });
        }, { passive: true }); // Runs this JavaScript step for the page interaction.

        // ----- Event wiring -----
        card.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            card.style.transform = ''; // Updates inline style for a dynamic UI state.
            card.style.boxShadow = ''; // Updates inline style for a dynamic UI state.
        });
    }());

    // ----- DOM references -----
    document.querySelectorAll('.pw-qty-control').forEach(function (ctrl) { // Finds a page element needed by the script.
        var dec     = ctrl.querySelector('.pw-qty-dec'); // Stores the dec DOM element reference.
        var inc     = ctrl.querySelector('.pw-qty-inc'); // Stores the inc DOM element reference.
        var display = ctrl.querySelector('.pw-qty-display'); // Stores the display DOM element reference.
        // ----- Helpers -----
        var parent  = ctrl.closest('.pw-add-group') || ctrl.closest('.pw-detail__add-row'); // Stores the parent value for later script logic.
        // ----- DOM references -----
        var hidden  = parent ? parent.querySelector('.pw-qty-value') : null; // Stores the hidden DOM element reference.
        var maxStock = parseInt(ctrl.getAttribute('data-max-stock') || '99', 10); // Reads the maximum selectable quantity for this product.
        if (!Number.isFinite(maxStock) || maxStock < 1) maxStock = 1; // Falls back to a safe minimum when the stock value is missing.

        // ----- Helpers -----
        function getQty() { return parseInt(display.textContent) || 1; } // Defines the getQty helper function.
        function setQty(n) { // Defines the setQty helper function.
            n = Math.max(1, Math.min(maxStock, n)); // Updates n for the current script state.
            display.textContent = n; // Updates display.textContent for the current script state.
            // ----- State updates -----
            if (hidden) hidden.value = n; // Checks the condition before running the next script step.
            if (dec) dec.disabled = n <= 1; // Updates the decrease button state at the boundaries.
            if (inc) inc.disabled = n >= maxStock; // Updates the increase button state at the boundaries.
        }
        // ----- Event wiring -----
        if (dec) dec.addEventListener('click', function () { setQty(getQty() - 1); }); // Registers an event handler for user or browser interaction.
        if (inc) inc.addEventListener('click', function () { setQty(getQty() + 1); }); // Registers an event handler for user or browser interaction.
        setQty(getQty()); // Syncs the initial display, hidden input, and button states.
    });

    // ----- Helpers -----
    function burstParticles(el) { // Defines the burstParticles helper function.
        var colors = ['#16A34A','#F59E0B','#F97316','#EC4899','#8B5CF6','#4ADE80','#FEF08A','#FB923C']; // Stores the colors value for later script logic.
        var r  = el.getBoundingClientRect(); // Stores the r value for later script logic.
        var cx = r.left + r.width / 2; // Stores the cx value for later script logic.
        var cy = r.top  + r.height / 2; // Stores the cy value for later script logic.
        for (var i = 0; i < 18; i++) { // Loops through matching items for this script step.
            (function (i) { // Starts an isolated script scope for this page.
                setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    // ----- DOM references -----
                    var p     = document.createElement('div'); // Stores the p value for later script logic.
                    // ----- Helpers -----
                    var size  = 5 + Math.random() * 11; // Stores the size value for later script logic.
                    var angle = Math.random() * Math.PI * 2; // Stores the angle value for later script logic.
                    var speed = 55 + Math.random() * 130; // Stores the speed value for later script logic.
                    var dx    = Math.cos(angle) * speed; // Stores the dx value for later script logic.
                    var dy    = Math.sin(angle) * speed - 60; // Stores the dy value for later script logic.
                    // ----- State updates -----
                    p.style.cssText = // Updates inline style for a dynamic UI state.
                        'position:fixed;border-radius:50%;pointer-events:none;z-index:9999;' + // Runs this JavaScript step for the page interaction.
                        'left:'   + (cx - size / 2) + 'px;top:' + (cy - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
                        'width:'  + size + 'px;height:' + size + 'px;' + // Runs this JavaScript step for the page interaction.
                        'background:' + colors[Math.floor(Math.random() * colors.length)] + ';'; // Runs this JavaScript step for the page interaction.
                    // ----- DOM references -----
                    document.body.appendChild(p); // Runs this JavaScript step for the page interaction.
                    // ----- Animation -----
                    p.animate( // Runs this JavaScript step for the page interaction.
                        [{ transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
                         { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0)', opacity: 0 }], // Runs this JavaScript step for the page interaction.
                        { duration: 550 + Math.random() * 350, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
                    // ----- Helpers -----
                    ).onfinish = function () { p.remove(); }; // Updates ).onfinish for the current script state.
                }, i * 22); // Runs this JavaScript step for the page interaction.
            }(i)); // Runs this JavaScript step for the page interaction.
        }
    }

    function escHtml(s) { // Defines the escHtml helper function.
        return s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;'); // Returns the computed value to the caller.
    }
    function showAddToast(msg) { // Defines the showAddToast helper function.
        // ----- DOM references -----
        var t = document.createElement('div'); // Stores the t value for later script logic.
        t.className = 'pw-add-toast'; // Updates t.className for the current script state.
        // ----- State updates -----
        t.setAttribute('role', 'status'); // Sets an attribute required by the UI state.
        // ----- Accessibility -----
        t.setAttribute('aria-live', 'polite'); // Sets an attribute required by the UI state.
        t.innerHTML = '🛒 ' + msg; // Updates t.innerHTML for the current script state.
        // ----- DOM references -----
        document.body.appendChild(t); // Runs this JavaScript step for the page interaction.
        // ----- Helpers -----
        requestAnimationFrame(function () { // Schedules smooth visual updates for the next animation frame.
            requestAnimationFrame(function () { t.classList.add('is-visible'); }); // Adds a CSS class to update the element state.
        });
        setTimeout(function () { // Starts a JavaScript block for the current control flow.
            // ----- State updates -----
            t.classList.remove('is-visible'); // Removes a CSS class to update the element state.
            // ----- Helpers -----
            setTimeout(function () { t.remove(); }, 350); // Runs this JavaScript step for the page interaction.
        }, 2800); // Runs this JavaScript step for the page interaction.
    }

    // ----- DOM references -----
    document.querySelectorAll('.pw-add-form').forEach(function (form) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        form.addEventListener('submit', function (e) { // Registers an event handler for user or browser interaction.
            e.preventDefault(); // Stops the browser's default action for this event.

            // ----- DOM references -----
            var btn      = form.querySelector('.pw-add-btn'); // Stores the btn DOM element reference.
            var qtyInput = form.querySelector('.pw-qty-value'); // Stores the qtyInput DOM element reference.
            // ----- Event wiring -----
            var qty      = parseInt(qtyInput ? qtyInput.value : '1') || 1; // Stores the qty value for later script logic.
            // ----- DOM references -----
            var tokenEl  = form.querySelector('[name="__RequestVerificationToken"]'); // Stores the tokenEl DOM element reference.
            // ----- Helpers -----
            var token    = tokenEl ? tokenEl.value : ''; // Stores the token value for later script logic.
            var origHTML = btn.innerHTML; // Stores the origHTML value for later script logic.

            // ----- State updates -----
            btn.disabled  = true; // Updates btn.disabled for the current script state.
            btn.innerHTML = '<span class="pw-spinner"></span>'; // Updates btn.innerHTML for the current script state.

            // ----- Ajax requests -----
            fetch(form.action, { // Sends an asynchronous request to the server.
                method: 'POST', // Runs this JavaScript step for the page interaction.
                headers: { // Starts a JavaScript block for the current control flow.
                    'Content-Type': 'application/x-www-form-urlencoded', // Runs this JavaScript step for the page interaction.
                    'RequestVerificationToken': token // Runs this JavaScript step for the page interaction.
                },
                body: new URLSearchParams({ // Starts a JavaScript block for the current control flow.
                    // ----- DOM references -----
                    ProductsId: form.querySelector('[name="ProductsId"]').value, // Finds a page element needed by the script.
                    // ----- Basket logic -----
                    Quantity:   qty, // Runs this JavaScript step for the page interaction.
                    __RequestVerificationToken: token // Runs this JavaScript step for the page interaction.
                })
            })
            // ----- Helpers -----
            .then(function (r) { return r.json(); }) // Handles the next step after the async request completes.
            .then(function (data) { // Handles the next step after the async request completes.
                if (data.success) { // Checks the condition before running the next script step.
                    btn.innerHTML        = '✓'; // Updates btn.innerHTML for the current script state.
                    // ----- State updates -----
                    btn.style.background = 'var(--green-700)'; // Updates inline style for a dynamic UI state.

                    // ----- Basket logic -----
                    if (window.updateBasketBadge) window.updateBasketBadge(data.basketCount); // Checks the condition before running the next script step.
                    burstParticles(btn); // Runs this JavaScript step for the page interaction.
                    showAddToast( // Runs this JavaScript step for the page interaction.
                        (qty > 1 ? '<strong>' + qty + '×</strong> ' : '') + // Runs this JavaScript step for the page interaction.
                        escHtml(data.itemName) + ' added to basket!' // Runs this JavaScript step for the page interaction.
                    );

                    // ----- Helpers -----
                    setTimeout(function () { // Starts a JavaScript block for the current control flow.
                        // ----- State updates -----
                        btn.disabled         = false; // Updates btn.disabled for the current script state.
                        btn.innerHTML        = origHTML; // Updates btn.innerHTML for the current script state.
                        btn.style.background = ''; // Updates inline style for a dynamic UI state.
                    }, 1800); // Runs this JavaScript step for the page interaction.
                } else { // Starts a JavaScript block for the current control flow.
                    btn.disabled         = false; // Updates btn.disabled for the current script state.
                    btn.innerHTML        = origHTML; // Updates btn.innerHTML for the current script state.
                    btn.style.background = '#DC2626'; // Updates inline style for a dynamic UI state.
                    // ----- Basket logic -----
                    showAddToast(escHtml(data.message || 'Could not add to basket.')); // Runs this JavaScript step for the page interaction.
                    // ----- Helpers -----
                    setTimeout(function () { btn.style.background = ''; }, 1800); // Updates inline style for a dynamic UI state.
                }
            })
            .catch(function () { // Handles errors from the async request.
                // ----- State updates -----
                btn.disabled  = false; // Updates btn.disabled for the current script state.
                btn.innerHTML = origHTML; // Updates btn.innerHTML for the current script state.
                showAddToast('Something went wrong — please try again.'); // Runs this JavaScript step for the page interaction.
            });
        });
    });

}());
