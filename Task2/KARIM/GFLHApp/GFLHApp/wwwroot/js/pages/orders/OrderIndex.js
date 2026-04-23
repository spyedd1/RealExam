// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- DOM references -----
    var list       = document.getElementById('ordersList'); // Stores the list DOM element reference.
    var searchEl   = document.getElementById('ordersSearch'); // Stores the searchEl DOM element reference.
    var sortEl     = document.getElementById('ordersSort'); // Stores the sortEl DOM element reference.
    var countNum   = document.getElementById('ordersCountNum'); // Stores the countNum DOM element reference.
    var noResults  = document.getElementById('noResults'); // Stores the noResults DOM element reference.
    var clearBtn   = document.getElementById('clearFiltersBtn'); // Stores the clearBtn DOM element reference.
    // ----- Helpers -----
    var filterBtns = Array.prototype.slice.call( // Stores the filterBtns value for later script logic.
                         // ----- DOM references -----
                         document.querySelectorAll('.pw-orders-filter-btn')); // Finds a page element needed by the script.

    // ----- Order logic -----
    if (!list) return; // no orders on page // Checks the condition before running the next script step.

    // ----- DOM references -----
    var cards = Array.prototype.slice.call(list.querySelectorAll('.pw-order-card')); // Stores the cards DOM element reference.
    // ----- Helpers -----
    var activeFilter = 'all'; // Stores the activeFilter value for later script logic.

    function getVal(card, attr) { return card.getAttribute('data-' + attr) || ''; } // Defines the getVal helper function.

    function applyFilters() { // Defines the applyFilters helper function.
        var query = searchEl ? searchEl.value.trim().toLowerCase() : ''; // Stores the query value for later script logic.
        var sort  = sortEl  ? sortEl.value : 'newest'; // Stores the sort value for later script logic.

        var visible = cards.filter(function (card) { // Stores the visible value for later script logic.
            var matchStatus = (activeFilter === 'all') || // Stores the matchStatus value for later script logic.
                              getVal(card, 'status') === activeFilter; // Runs this JavaScript step for the page interaction.
            var matchSearch = !query || // Stores the matchSearch value for later script logic.
                              ('pf-' + getVal(card, 'id').padStart(6, '0')).includes(query) || // Runs this JavaScript step for the page interaction.
                              ('#pf-' + getVal(card, 'id').padStart(6, '0')).includes(query) || // Runs this JavaScript step for the page interaction.
                              getVal(card, 'id').includes(query); // Runs this JavaScript step for the page interaction.
            return matchStatus && matchSearch; // Returns the computed value to the caller.
        });

        visible.sort(function (a, b) { // Starts a JavaScript block for the current control flow.
            switch (sort) { // Starts a JavaScript block for the current control flow.
                case 'oldest':  return getVal(a, 'date') < getVal(b, 'date') ? -1 : 1; // Runs this JavaScript step for the page interaction.
                // ----- State updates -----
                case 'highest': return parseFloat(getVal(b, 'value')) - parseFloat(getVal(a, 'value')); // Runs this JavaScript step for the page interaction.
                case 'lowest':  return parseFloat(getVal(a, 'value')) - parseFloat(getVal(b, 'value')); // Runs this JavaScript step for the page interaction.
                default:        return getVal(a, 'date') < getVal(b, 'date') ? 1 : -1; // newest // Runs this JavaScript step for the page interaction.
            }
        });

        // ----- Helpers -----
        cards.forEach(function (c) { c.style.display = 'none'; }); // Updates inline style for a dynamic UI state.
        visible.forEach(function (c) { // Starts a JavaScript block for the current control flow.
            // ----- State updates -----
            c.style.display = ''; // Updates inline style for a dynamic UI state.
            list.appendChild(c); // Runs this JavaScript step for the page interaction.
        });

        if (countNum) countNum.textContent = visible.length; // Checks the condition before running the next script step.

        if (noResults) noResults.style.display = visible.length ? 'none' : 'flex'; // Updates inline style for a dynamic UI state.
    }

    if (searchEl) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        searchEl.addEventListener('input', applyFilters); // Registers an event handler for user or browser interaction.
    }

    if (sortEl) { // Checks the condition before running the next script step.
        sortEl.addEventListener('change', applyFilters); // Registers an event handler for user or browser interaction.
    }

    // ----- Helpers -----
    filterBtns.forEach(function (btn) { // Starts a JavaScript block for the current control flow.
        // ----- Event wiring -----
        btn.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            filterBtns.forEach(function (b) { b.classList.remove('is-active'); }); // Removes a CSS class to update the element state.
            // ----- State updates -----
            btn.classList.add('is-active'); // Adds a CSS class to update the element state.
            activeFilter = btn.getAttribute('data-filter'); // Updates activeFilter for the current script state.
            applyFilters(); // Runs this JavaScript step for the page interaction.
        });
    });

    if (clearBtn) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        clearBtn.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
            // ----- State updates -----
            if (searchEl) searchEl.value = ''; // Checks the condition before running the next script step.
            if (sortEl)   sortEl.value   = 'newest'; // Checks the condition before running the next script step.
            activeFilter = 'all'; // Updates activeFilter for the current script state.
            // ----- Helpers -----
            filterBtns.forEach(function (b) { // Starts a JavaScript block for the current control flow.
                // ----- State updates -----
                b.classList.toggle('is-active', b.getAttribute('data-filter') === 'all'); // Toggles a CSS class based on the current state.
            });
            applyFilters(); // Runs this JavaScript step for the page interaction.
        });
    }

    // ----- Helpers -----
    var colors = ['#16A34A','#F59E0B','#F97316','#4ADE80','#FEF08A','#86EFAC']; // Stores the colors value for later script logic.
    // ----- DOM references -----
    document.querySelectorAll('.pw-confirm-btn').forEach(function (btn) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        btn.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var r = btn.getBoundingClientRect(); // Stores the r value for later script logic.
            var cx = r.left + r.width  / 2; // Stores the cx value for later script logic.
            var cy = r.top  + r.height / 2; // Stores the cy value for later script logic.
            for (var i = 0; i < 18; i++) { // Loops through matching items for this script step.
                (function (i) { // Starts an isolated script scope for this page.
                    setTimeout(function () { // Starts a JavaScript block for the current control flow.
                        // ----- DOM references -----
                        var el    = document.createElement('div'); // Stores the el value for later script logic.
                        // ----- Helpers -----
                        var color = colors[Math.floor(Math.random() * colors.length)]; // Stores the color value for later script logic.
                        var size  = 7 + Math.random() * 10; // Stores the size value for later script logic.
                        var angle = Math.random() * Math.PI * 2; // Stores the angle value for later script logic.
                        var speed = 80 + Math.random() * 160; // Stores the speed value for later script logic.
                        var dx    = Math.cos(angle) * speed; // Stores the dx value for later script logic.
                        var dy    = Math.sin(angle) * speed - 60; // Stores the dy value for later script logic.
                        var dur   = 600 + Math.random() * 400; // Stores the dur value for later script logic.
                        // ----- State updates -----
                        el.style.cssText = // Updates inline style for a dynamic UI state.
                            'position:fixed;border-radius:50%;pointer-events:none;z-index:9999;' + // Runs this JavaScript step for the page interaction.
                            'left:' + (cx - size/2) + 'px;' + // Runs this JavaScript step for the page interaction.
                            'top:'  + (cy - size/2) + 'px;' + // Runs this JavaScript step for the page interaction.
                            'width:'  + size + 'px;' + // Runs this JavaScript step for the page interaction.
                            'height:' + size + 'px;' + // Runs this JavaScript step for the page interaction.
                            'background:' + color + ';'; // Runs this JavaScript step for the page interaction.
                        // ----- DOM references -----
                        document.body.appendChild(el); // Runs this JavaScript step for the page interaction.
                        // ----- Animation -----
                        el.animate( // Runs this JavaScript step for the page interaction.
                            [{ transform:'translate(0,0) scale(1)', opacity:1 }, // Runs this JavaScript step for the page interaction.
                             { transform:'translate('+dx+'px,'+dy+'px) scale(.05)', opacity:0 }], // Runs this JavaScript step for the page interaction.
                            { duration:dur, easing:'ease-out', fill:'forwards' } // Runs this JavaScript step for the page interaction.
                        // ----- Helpers -----
                        ).onfinish = function () { el.remove(); }; // Updates ).onfinish for the current script state.
                    }, i * 25); // Runs this JavaScript step for the page interaction.
                })(i); // Runs this JavaScript step for the page interaction.
            }
        });
    });

})();
