// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
            // ----- Strict mode -----
            'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

            // ----- Helpers -----
            (function () { // Starts an isolated script scope for this page.
                // ----- DOM references -----
                var cards = document.querySelectorAll('[data-product-card]'); // Stores the cards DOM element reference.
                if (!cards.length) return; // Checks the condition before running the next script step.
                // ----- Helpers -----
                setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    cards.forEach(function (card) { // Starts a JavaScript block for the current control flow.
                        // ----- State updates -----
                        card.style.opacity    = ''; // Updates inline style for a dynamic UI state.
                        card.style.transform  = ''; // Updates inline style for a dynamic UI state.
                        card.style.visibility = ''; // Updates inline style for a dynamic UI state.
                    });
                }, 900); // 600 ms SR duration + 300 ms buffer // Runs this JavaScript step for the page interaction.
            }());

            // ----- DOM references -----
            document.querySelectorAll('.pw-qty-control').forEach(function (ctrl) { // Finds a page element needed by the script.
                var dec     = ctrl.querySelector('.pw-qty-dec'); // Stores the dec DOM element reference.
                var inc     = ctrl.querySelector('.pw-qty-inc'); // Stores the inc DOM element reference.
                var display = ctrl.querySelector('.pw-qty-display'); // Stores the display DOM element reference.
                // ----- Helpers -----
                var group   = ctrl.closest('.pw-add-group'); // Stores the group value for later script logic.
                // ----- DOM references -----
                var hidden  = group ? group.querySelector('.pw-qty-value') : null; // Stores the hidden DOM element reference.
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
                var cx = r.left + r.width  / 2; // Stores the cx value for later script logic.
                var cy = r.top  + r.height / 2; // Stores the cy value for later script logic.
                for (var i = 0; i < 14; i++) { // Loops through matching items for this script step.
                    (function (i) { // Starts an isolated script scope for this page.
                        setTimeout(function () { // Starts a JavaScript block for the current control flow.
                            // ----- DOM references -----
                            var p     = document.createElement('div'); // Stores the p value for later script logic.
                            // ----- Helpers -----
                            var size  = 5 + Math.random() * 9; // Stores the size value for later script logic.
                            var angle = Math.random() * Math.PI * 2; // Stores the angle value for later script logic.
                            var speed = 50 + Math.random() * 110; // Stores the speed value for later script logic.
                            var dx    = Math.cos(angle) * speed; // Stores the dx value for later script logic.
                            var dy    = Math.sin(angle) * speed - 55; // Stores the dy value for later script logic.
                            // ----- State updates -----
                            p.style.cssText = // Updates inline style for a dynamic UI state.
                                'position:fixed;border-radius:50%;pointer-events:none;z-index:9999;' + // Runs this JavaScript step for the page interaction.
                                'left:'   + (cx - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
                                'top:'    + (cy - size / 2) + 'px;' + // Runs this JavaScript step for the page interaction.
                                'width:'  + size + 'px;height:' + size + 'px;' + // Runs this JavaScript step for the page interaction.
                                'background:' + colors[Math.floor(Math.random() * colors.length)] + ';'; // Runs this JavaScript step for the page interaction.
                            // ----- DOM references -----
                            document.body.appendChild(p); // Runs this JavaScript step for the page interaction.
                            // ----- Animation -----
                            p.animate( // Runs this JavaScript step for the page interaction.
                                [{ transform: 'translate(0,0) scale(1)', opacity: 1 }, // Runs this JavaScript step for the page interaction.
                                 { transform: 'translate(' + dx + 'px,' + dy + 'px) scale(0)', opacity: 0 }], // Runs this JavaScript step for the page interaction.
                                { duration: 550 + Math.random() * 300, easing: 'ease-out', fill: 'forwards' } // Runs this JavaScript step for the page interaction.
                            // ----- Helpers -----
                            ).onfinish = function () { p.remove(); }; // Updates ).onfinish for the current script state.
                        }, i * 25); // Runs this JavaScript step for the page interaction.
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
                            btn.innerHTML = '✓'; // Updates btn.innerHTML for the current script state.
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

            // ----- DOM references -----
            var searchInput    = document.getElementById('filterSearch'); // Stores the searchInput DOM element reference.
            var categorySelect = document.getElementById('filterCategory'); // Stores the categorySelect DOM element reference.
            var producerSelect = document.getElementById('filterProducer'); // Stores the producerSelect DOM element reference.
            var sortSelect     = document.getElementById('filterSort'); // Stores the sortSelect DOM element reference.
            var countEl        = document.getElementById('productCount'); // Stores the countEl DOM element reference.
            var clearBtn       = document.getElementById('filterClear'); // Stores the clearBtn DOM element reference.
            var emptyState     = document.getElementById('emptyState'); // Stores the emptyState DOM element reference.
            var grid           = document.getElementById('productsGrid'); // Stores the grid DOM element reference.
            var pills          = document.querySelectorAll('[data-avail]'); // Stores the pills DOM element reference.
            // ----- Helpers -----
            var activeAvail    = 'all'; // Stores the activeAvail value for later script logic.

            function getCards() { // Defines the getCards helper function.
                // ----- DOM references -----
                return Array.from(grid.querySelectorAll('[data-product-card]')); // Finds a page element needed by the script.
            }

            // ----- Helpers -----
            function applyFilters() { // Defines the applyFilters helper function.
                // ----- Event wiring -----
                var search   = searchInput.value.toLowerCase().trim(); // Stores the search value for later script logic.
                // ----- Helpers -----
                var category = categorySelect.value; // Stores the category value for later script logic.
                // ----- Producer logic -----
                var producer = producerSelect.value; // Stores the producer value for later script logic.
                // ----- Helpers -----
                var sort     = sortSelect.value; // Stores the sort value for later script logic.
                var cards    = getCards(); // Stores the cards value for later script logic.

                // Sort: reorder DOM nodes before filtering
                if (sort !== 'default') { // Checks the condition before running the next script step.
                    cards.sort(function (a, b) { // Starts a JavaScript block for the current control flow.
                        // ----- Product logic -----
                        if (sort === 'price-asc')  return parseFloat(a.dataset.price) - parseFloat(b.dataset.price); // Checks the condition before running the next script step.
                        if (sort === 'price-desc') return parseFloat(b.dataset.price) - parseFloat(a.dataset.price); // Checks the condition before running the next script step.
                        // ----- State updates -----
                        if (sort === 'name-asc')   return a.dataset.name.localeCompare(b.dataset.name); // Checks the condition before running the next script step.
                        if (sort === 'name-desc')  return b.dataset.name.localeCompare(a.dataset.name); // Checks the condition before running the next script step.
                        return 0; // Returns the computed value to the caller.
                    });
                    // ----- Helpers -----
                    cards.forEach(function (c) { grid.insertBefore(c, emptyState); }); // Runs this JavaScript step for the page interaction.
                }

                // Show/hide by filter criteria
                var visible = 0; // Stores the visible value for later script logic.
                cards.forEach(function (card) { // Starts a JavaScript block for the current control flow.
                    var show = true; // Stores the show value for later script logic.
                    // ----- State updates -----
                    if (search   && !card.dataset.name.includes(search))      show = false; // Checks the condition before running the next script step.
                    if (category && card.dataset.category !== category)        show = false; // Checks the condition before running the next script step.
                    // ----- Producer logic -----
                    if (producer && card.dataset.producer !== producer)        show = false; // Checks the condition before running the next script step.
                    // ----- Product logic -----
                    if (activeAvail === 'instock'    && card.dataset.instock !== 'true') show = false; // Checks the condition before running the next script step.
                    if (activeAvail === 'outofstock' && card.dataset.instock === 'true') show = false; // Checks the condition before running the next script step.

                    if (show) { // Checks the condition before running the next script step.
                        // ----- State updates -----
                        card.style.display = ''; // Updates inline style for a dynamic UI state.
                        // ScrollReveal injects inline opacity/transform/visibility on elements it
                        // has already processed. When a card re-appears after being filtered out
                        // SR won't re-trigger, so we must clear those inline styles ourselves so
                        // the card is fully visible and takes up its proper space in the grid.
                        card.style.opacity    = ''; // Updates inline style for a dynamic UI state.
                        card.style.transform  = ''; // Updates inline style for a dynamic UI state.
                        card.style.visibility = ''; // Updates inline style for a dynamic UI state.
                        visible++; // Runs this JavaScript step for the page interaction.
                    } else { // Starts a JavaScript block for the current control flow.
                        card.style.display = 'none'; // Updates inline style for a dynamic UI state.
                    }
                });

                // Update count label
                if (countEl) countEl.textContent = visible; // Checks the condition before running the next script step.

                // Toggle empty state
                if (emptyState) emptyState.classList.toggle('is-visible', visible === 0); // Toggles a CSS class based on the current state.

                // Toggle clear button
                // ----- Producer logic -----
                var hasFilters = search || category || producer || activeAvail !== 'all' || sort !== 'default'; // Stores the hasFilters value for later script logic.
                // ----- State updates -----
                if (clearBtn) clearBtn.classList.toggle('is-visible', !!hasFilters); // Toggles a CSS class based on the current state.
            }

            // Wire up inputs
            // ----- Event wiring -----
            if (searchInput)    searchInput.addEventListener('input', applyFilters); // Registers an event handler for user or browser interaction.
            if (categorySelect) categorySelect.addEventListener('change', applyFilters); // Registers an event handler for user or browser interaction.
            if (producerSelect) producerSelect.addEventListener('change', applyFilters); // Registers an event handler for user or browser interaction.
            if (sortSelect)     sortSelect.addEventListener('change', applyFilters); // Registers an event handler for user or browser interaction.

            // Availability pills
            // ----- Helpers -----
            pills.forEach(function (pill) { // Starts a JavaScript block for the current control flow.
                // ----- Event wiring -----
                pill.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
                    // ----- Helpers -----
                    pills.forEach(function (p) { // Starts a JavaScript block for the current control flow.
                        // ----- State updates -----
                        p.classList.remove('is-active'); // Removes a CSS class to update the element state.
                        // ----- Accessibility -----
                        p.setAttribute('aria-pressed', 'false'); // Sets an attribute required by the UI state.
                    });
                    // ----- State updates -----
                    this.classList.add('is-active'); // Adds a CSS class to update the element state.
                    // ----- Accessibility -----
                    this.setAttribute('aria-pressed', 'true'); // Sets an attribute required by the UI state.
                    // ----- State updates -----
                    activeAvail = this.dataset.avail; // Updates activeAvail for the current script state.
                    applyFilters(); // Runs this JavaScript step for the page interaction.
                });
            });

            // Clear — defined first so addEventListener can reference it,
            // and exposed globally so the empty-state button's onclick can call it
            // ----- Helpers -----
            function clearAllFilters() { // Defines the clearAllFilters helper function.
                // ----- Event wiring -----
                searchInput.value    = ''; // Updates searchInput.value for the current script state.
                // ----- State updates -----
                categorySelect.value = ''; // Updates categorySelect.value for the current script state.
                // ----- Producer logic -----
                producerSelect.value = ''; // Updates producerSelect.value for the current script state.
                // ----- State updates -----
                sortSelect.value     = 'default'; // Updates sortSelect.value for the current script state.
                activeAvail          = 'all'; // Updates activeAvail for the current script state.
                // ----- Helpers -----
                pills.forEach(function (p) { // Starts a JavaScript block for the current control flow.
                    var isAll = p.dataset.avail === 'all'; // Stores the isAll value for later script logic.
                    // ----- State updates -----
                    p.classList.toggle('is-active', isAll); // Toggles a CSS class based on the current state.
                    // ----- Accessibility -----
                    p.setAttribute('aria-pressed', isAll ? 'true' : 'false'); // Sets an attribute required by the UI state.
                });
                applyFilters(); // Runs this JavaScript step for the page interaction.
            }
            window.clearAllFilters = clearAllFilters; // Updates window.clearAllFilters for the current script state.

            // ----- Event wiring -----
            if (clearBtn) clearBtn.addEventListener('click', clearAllFilters); // Registers an event handler for user or browser interaction.

            // Sticky filter bar — track live nav height so it always sits flush below the nav
            // ----- Helpers -----
            (function () { // Starts an isolated script scope for this page.
                // ----- DOM references -----
                var nav       = document.getElementById('mainNav'); // Stores the nav DOM element reference.
                var filterBar = document.querySelector('.pw-filters-bar'); // Stores the filterBar DOM element reference.
                if (!nav || !filterBar) return; // Checks the condition before running the next script step.
                // ----- Helpers -----
                function syncTop() { filterBar.style.top = nav.offsetHeight + 'px'; } // Defines the syncTop helper function.
                syncTop(); // Runs this JavaScript step for the page interaction.
                // ----- Event wiring -----
                window.addEventListener('scroll', syncTop, { passive: true }); // Registers an event handler for user or browser interaction.
                window.addEventListener('resize', syncTop, { passive: true }); // Registers an event handler for user or browser interaction.
            }());

            // Auto-dismiss error toast
            // ----- DOM references -----
            var toast = document.getElementById('errorToast'); // Stores the toast DOM element reference.
            if (toast) { // Checks the condition before running the next script step.
                // ----- Helpers -----
                setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    // ----- State updates -----
                    toast.style.transition = 'opacity .5s'; // Updates inline style for a dynamic UI state.
                    toast.style.opacity    = '0'; // Updates inline style for a dynamic UI state.
                    // ----- Helpers -----
                    setTimeout(function () { toast.remove(); }, 500); // Runs this JavaScript step for the page interaction.
                }, 4000); // Runs this JavaScript step for the page interaction.
            }

        }());
