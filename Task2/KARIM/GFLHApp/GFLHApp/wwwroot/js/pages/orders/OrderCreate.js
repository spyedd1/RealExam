// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- Strict mode -----
    'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

    // ----- Icon setup -----
    if (window.lucide) lucide.createIcons(); // Checks the condition before running the next script step.

    // ----- Order logic -----
    var checkoutForm = document.getElementById('checkoutForm'); // Stores the checkout form reference for server-rendered values.
    var DELIVERY_COSTS   = { 'Next Day': 5.99, 'Standard': 2.99, 'Economy': 0.99 }; // Stores the DELIVERY_COSTS value for later script logic.
    // ----- Server values -----
    var HAS_FREE_DELIVERY = checkoutForm && checkoutForm.dataset.hasFreeDelivery === 'true'; // Stores the HAS_FREE_DELIVERY value supplied by the Razor page.
    var BASE_TOTAL = parseFloat(checkoutForm ? checkoutForm.dataset.baseTotal : '0') || 0; // Stores the server-calculated total before delivery is selected.


    // Fulfilment radios (Delivery / Collection)
    // ----- DOM references -----
    var radioDelivery    = document.getElementById('fulfilmentDelivery'); // Stores the radioDelivery DOM element reference.
    var radioCollection  = document.getElementById('fulfilmentCollection'); // Stores the radioCollection DOM element reference.
    var cardDelivery     = document.getElementById('deliveryCard'); // Stores the cardDelivery DOM element reference.
    var cardCollection   = document.getElementById('collectionCard'); // Stores the cardCollection DOM element reference.
    // Hidden booleans kept in sync with the model
    var hiddenDelivery   = document.getElementById('DeliveryHidden'); // Stores the hiddenDelivery DOM element reference.
    var hiddenCollection = document.getElementById('CollectionHidden'); // Stores the hiddenCollection DOM element reference.

    // Panels shown/hidden based on fulfilment choice
    var panelDeliveryTypes  = document.getElementById('deliveryTypeGroup'); // Stores the panelDeliveryTypes DOM element reference.
    var panelCollectionDate = document.getElementById('collectionDateGroup'); // Stores the panelCollectionDate DOM element reference.
    // Delivery address section (entire card hidden when collection chosen)
    var sectionDeliveryAddr = document.getElementById('deliveryAddressGroup'); // Stores the sectionDeliveryAddr DOM element reference.

    // Delivery method radio cards
    var dmCards = document.querySelectorAll('.co-dm-card'); // Stores the dmCards DOM element reference.

    // Same-as-billing
    var chkSameAsBilling   = document.getElementById('sameAsBilling'); // Stores the chkSameAsBilling DOM element reference.
    var wrapDeliveryFields = document.getElementById('deliveryFieldsWrap'); // Stores the wrapDeliveryFields DOM element reference.
    // Visible delivery address inputs
    var inpDL1 = document.getElementById('DeliveryLine1'); // Stores the inpDL1 DOM element reference.
    var inpDL2 = document.getElementById('DeliveryLine2'); // Stores the inpDL2 DOM element reference.
    var inpDCi = document.getElementById('DeliveryCity'); // Stores the inpDCi DOM element reference.
    var inpDPc = document.getElementById('DeliveryPostcode'); // Stores the inpDPc DOM element reference.
    // Hidden fields that carry delivery address when same-as-billing is checked
    var hidDL1 = document.getElementById('DeliveryLine1Hidden'); // Stores the hidDL1 DOM element reference.
    var hidDL2 = document.getElementById('DeliveryLine2Hidden'); // Stores the hidDL2 DOM element reference.
    var hidDCi = document.getElementById('DeliveryCityHidden'); // Stores the hidDCi DOM element reference.
    var hidDPc = document.getElementById('DeliveryPostcodeHidden'); // Stores the hidDPc DOM element reference.
    // Billing address inputs (source for copy)
    var inpBL1 = document.getElementById('BillingLine1'); // Stores the inpBL1 DOM element reference.
    var inpBL2 = document.getElementById('BillingLine2'); // Stores the inpBL2 DOM element reference.
    var inpBCi = document.getElementById('BillingCity'); // Stores the inpBCi DOM element reference.
    var inpBPc = document.getElementById('BillingPostcode'); // Stores the inpBPc DOM element reference.

    // Order summary elements
    var elDeliveryCostRow   = document.getElementById('deliveryCostRow'); // Stores the elDeliveryCostRow DOM element reference.
    var elDeliveryCostLabel = document.getElementById('deliveryCostLabel'); // Stores the elDeliveryCostLabel DOM element reference.
    var elDeliveryCostValue = document.getElementById('deliveryCostValue'); // Stores the elDeliveryCostValue DOM element reference.
    var elOrderTotal        = document.getElementById('orderTotal'); // Stores the elOrderTotal DOM element reference.

    // Collection date
    var inpCollectionDate = document.getElementById('DateOfCollection'); // Stores the inpCollectionDate DOM element reference.


    // ----- Helpers -----
    function show(el) { if (el) el.style.display = 'block'; } // Defines the show helper function.
    function hide(el) { if (el) el.style.display = 'none'; } // Defines the hide helper function.
    function val(el)  { return el ? el.value : ''; } // Defines the val helper function.

    // ----- Order logic -----
    function getSelectedDeliveryMethod() { // Defines the getSelectedDeliveryMethod helper function.
        // ----- DOM references -----
        var checked = document.querySelector('input[name="DeliveryMethod"]:checked'); // Stores the checked DOM element reference.
        // ----- State updates -----
        return checked ? checked.value : null; // Returns the computed value to the caller.
    }

    // ----- Helpers -----
    var _animRaf = null; // Stores the _animRaf value for later script logic.
    // ----- Basket logic -----
    function animateTotal(target) { // Defines the animateTotal helper function.
        if (_animRaf) cancelAnimationFrame(_animRaf); // Checks the condition before running the next script step.
        var start   = parseFloat((elOrderTotal.textContent || '0').replace(/[^0-9.]/g, '')) || 0; // Stores the start value for later script logic.
        // ----- Helpers -----
        var t0      = performance.now(); // Stores the t0 value for later script logic.
        var dur     = 320; // Stores the dur value for later script logic.
        function tick(now) { // Defines the tick helper function.
            var p    = Math.min((now - t0) / dur, 1); // Stores the p value for later script logic.
            var ease = 1 - Math.pow(1 - p, 3); // Stores the ease value for later script logic.
            // ----- Basket logic -----
            elOrderTotal.textContent = '£' + (start + (target - start) * ease).toFixed(2); // Updates elOrderTotal.textContent for the current script state.
            // ----- Animation -----
            if (p < 1) { _animRaf = requestAnimationFrame(tick); } // Checks the condition before running the next script step.
            // ----- Basket logic -----
            else        { elOrderTotal.textContent = '£' + target.toFixed(2); _animRaf = null; } // Handles the fallback branch for the previous condition.
        }
        // ----- Animation -----
        _animRaf = requestAnimationFrame(tick); // Schedules smooth visual updates for the next animation frame.
    }


    // ----- Event wiring -----
    function onFulfilmentChange() { // Defines the onFulfilmentChange helper function.
        // ----- Order logic -----
        var isDelivery   = radioDelivery   && radioDelivery.checked; // Stores the isDelivery value for later script logic.
        var isCollection = radioCollection && radioCollection.checked; // Stores the isCollection value for later script logic.

        if (cardDelivery)   cardDelivery.classList.toggle('is-active', isDelivery); // Toggles a CSS class based on the current state.
        if (cardCollection) cardCollection.classList.toggle('is-active', isCollection); // Toggles a CSS class based on the current state.

        if (hiddenDelivery)   hiddenDelivery.value   = isDelivery   ? 'true' : 'false'; // Checks the condition before running the next script step.
        if (hiddenCollection) hiddenCollection.value = isCollection ? 'true' : 'false'; // Checks the condition before running the next script step.

        if (isDelivery) { // Checks the condition before running the next script step.
            show(panelDeliveryTypes); // Runs this JavaScript step for the page interaction.
            hide(panelCollectionDate); // Runs this JavaScript step for the page interaction.
            show(sectionDeliveryAddr); // Runs this JavaScript step for the page interaction.
        } else if (isCollection) { // Starts a JavaScript block for the current control flow.
            hide(panelDeliveryTypes); // Runs this JavaScript step for the page interaction.
            show(panelCollectionDate); // Runs this JavaScript step for the page interaction.
            hide(sectionDeliveryAddr); // Runs this JavaScript step for the page interaction.
        } else { // Starts a JavaScript block for the current control flow.
            hide(panelDeliveryTypes); // Runs this JavaScript step for the page interaction.
            hide(panelCollectionDate); // Runs this JavaScript step for the page interaction.
            // leave delivery address shown by default
        }

        updateOrderSummary(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Event wiring -----
    if (radioDelivery)   radioDelivery.addEventListener('change', onFulfilmentChange); // Registers an event handler for user or browser interaction.
    if (radioCollection) radioCollection.addEventListener('change', onFulfilmentChange); // Registers an event handler for user or browser interaction.

    // Run once on load to establish initial state
    onFulfilmentChange(); // Runs this JavaScript step for the page interaction.
    syncDeliveryMethodCards(); // Restores the selected delivery method card after validation refreshes.


    // ----- Order logic -----
    function syncDeliveryMethodCards() { // Defines the syncDeliveryMethodCards helper function.
        // ----- Helpers -----
        dmCards.forEach(function (card) { // Starts a JavaScript block for the current control flow.
            // ----- DOM references -----
            var radio = card.querySelector('input[type="radio"][name="DeliveryMethod"]'); // Stores the radio DOM element reference.
            // ----- State updates -----
            card.classList.toggle('is-active', !!(radio && radio.checked)); // Toggles a CSS class based on the current state.
        });
        // ----- Order logic -----
        updateOrderSummary(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    dmCards.forEach(function (card) { // Starts a JavaScript block for the current control flow.
        // ----- DOM references -----
        var radio = card.querySelector('input[type="radio"][name="DeliveryMethod"]'); // Stores the radio DOM element reference.
        if (radio) { // Checks the condition before running the next script step.
            // ----- Event wiring -----
            radio.addEventListener('change', syncDeliveryMethodCards); // Registers an event handler for user or browser interaction.
        }
        card.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
            // ----- Order logic -----
            setTimeout(syncDeliveryMethodCards, 0); // Runs this JavaScript step for the page interaction.
        });
    });


    function updateOrderSummary() { // Defines the updateOrderSummary helper function.
        var isDelivery = radioDelivery && radioDelivery.checked; // Stores the isDelivery value for later script logic.
        var method     = getSelectedDeliveryMethod(); // Stores the method value for later script logic.

        if (isDelivery && method && DELIVERY_COSTS[method] !== undefined) { // Checks the condition before running the next script step.
            elDeliveryCostRow.style.display = ''; // Updates inline style for a dynamic UI state.
            elDeliveryCostLabel.textContent = method + ' Delivery'; // Updates elDeliveryCostLabel.textContent for the current script state.

            if (HAS_FREE_DELIVERY) { // Checks the condition before running the next script step.
                elDeliveryCostValue.innerHTML = '<span style="color:var(--green);font-weight:700;">FREE</span>'; // Updates elDeliveryCostValue.innerHTML for the current script state.
                // ----- Basket logic -----
                animateTotal(BASE_TOTAL); // Runs this JavaScript step for the page interaction.
            } else { // Starts a JavaScript block for the current control flow.
                // ----- Order logic -----
                var cost = DELIVERY_COSTS[method]; // Stores the cost value for later script logic.
                elDeliveryCostValue.textContent = '£' + cost.toFixed(2); // Updates elDeliveryCostValue.textContent for the current script state.
                // ----- Basket logic -----
                animateTotal(BASE_TOTAL + cost); // Runs this JavaScript step for the page interaction.
            }
        } else { // Starts a JavaScript block for the current control flow.
            // ----- Order logic -----
            elDeliveryCostRow.style.display = 'none'; // Updates inline style for a dynamic UI state.
            // ----- Basket logic -----
            animateTotal(BASE_TOTAL); // Runs this JavaScript step for the page interaction.
        }
    }


    // ----- Helpers -----
    function copyBillingToHidden() { // Defines the copyBillingToHidden helper function.
        // ----- State updates -----
        if (hidDL1) hidDL1.value = val(inpBL1); // Checks the condition before running the next script step.
        if (hidDL2) hidDL2.value = val(inpBL2); // Checks the condition before running the next script step.
        if (hidDCi) hidDCi.value = val(inpBCi); // Checks the condition before running the next script step.
        if (hidDPc) hidDPc.value = val(inpBPc); // Checks the condition before running the next script step.
    }

    // ----- Order logic -----
    function clearHiddenDeliveryFields() { // Defines the clearHiddenDeliveryFields helper function.
        // ----- State updates -----
        if (hidDL1) hidDL1.value = ''; // Checks the condition before running the next script step.
        if (hidDL2) hidDL2.value = ''; // Checks the condition before running the next script step.
        if (hidDCi) hidDCi.value = ''; // Checks the condition before running the next script step.
        if (hidDPc) hidDPc.value = ''; // Checks the condition before running the next script step.
    }

    // ----- Event wiring -----
    function onSameAsBillingChange() { // Defines the onSameAsBillingChange helper function.
        // ----- Helpers -----
        var same = chkSameAsBilling && chkSameAsBilling.checked; // Stores the same value for later script logic.

        // ----- Order logic -----
        if (wrapDeliveryFields) { // Checks the condition before running the next script step.
            wrapDeliveryFields.style.display = same ? 'none' : ''; // Updates inline style for a dynamic UI state.
        }

        // ----- State updates -----
        if (inpDL1) inpDL1.disabled = same; // Checks the condition before running the next script step.
        if (inpDL2) inpDL2.disabled = same; // Checks the condition before running the next script step.
        if (inpDCi) inpDCi.disabled = same; // Checks the condition before running the next script step.
        if (inpDPc) inpDPc.disabled = same; // Checks the condition before running the next script step.

        if (same) { // Checks the condition before running the next script step.
            copyBillingToHidden(); // Runs this JavaScript step for the page interaction.
        } else { // Starts a JavaScript block for the current control flow.
            // ----- Order logic -----
            clearHiddenDeliveryFields(); // Runs this JavaScript step for the page interaction.
        }
    }

    if (chkSameAsBilling) { // Checks the condition before running the next script step.
        // ----- Event wiring -----
        chkSameAsBilling.addEventListener('change', onSameAsBillingChange); // Registers an event handler for user or browser interaction.
    }

    // ----- Helpers -----
    function makeBillingLiveSyncer(inpHidden) { // Defines the makeBillingLiveSyncer helper function.
        return function () { // Returns the computed value to the caller.
            // ----- State updates -----
            if (chkSameAsBilling && chkSameAsBilling.checked) { // Checks the condition before running the next script step.
                if (inpHidden) inpHidden.value = this.value; // Checks the condition before running the next script step.
            }
        };
    }
    // ----- Event wiring -----
    if (inpBL1) inpBL1.addEventListener('input', makeBillingLiveSyncer(hidDL1)); // Registers an event handler for user or browser interaction.
    if (inpBL2) inpBL2.addEventListener('input', makeBillingLiveSyncer(hidDL2)); // Registers an event handler for user or browser interaction.
    if (inpBCi) inpBCi.addEventListener('input', makeBillingLiveSyncer(hidDCi)); // Registers an event handler for user or browser interaction.
    if (inpBPc) inpBPc.addEventListener('input', makeBillingLiveSyncer(hidDPc)); // Registers an event handler for user or browser interaction.

    // Run once on load
    onSameAsBillingChange(); // Runs this JavaScript step for the page interaction.


    // ----- Order logic -----
    if (inpCollectionDate) { // Checks the condition before running the next script step.
        // ----- Helpers -----
        var today   = new Date(); today.setHours(0, 0, 0, 0); // Creates the today object used by the script.
        var minDate = new Date(today); minDate.setDate(minDate.getDate() + 2); // Creates the minDate object used by the script.
        var maxDate = new Date(today); maxDate.setDate(maxDate.getDate() + 14); // Creates the maxDate object used by the script.

        // ----- Order logic -----
        inpCollectionDate.min = minDate.toISOString().split('T')[0]; // Updates inpCollectionDate.min for the current script state.
        inpCollectionDate.max = maxDate.toISOString().split('T')[0]; // Updates inpCollectionDate.max for the current script state.

        // ----- Event wiring -----
        inpCollectionDate.addEventListener('change', function () { // Registers an event handler for user or browser interaction.
            // ----- DOM references -----
            var errEl = document.getElementById('collectionDateError'); // Stores the errEl DOM element reference.
            if (!errEl) return; // Checks the condition before running the next script step.
            // ----- Helpers -----
            var sel = new Date(this.value); sel.setHours(0, 0, 0, 0); // Creates the sel object used by the script.
            // ----- State updates -----
            errEl.style.display = 'none'; // Updates inline style for a dynamic UI state.
            errEl.textContent   = ''; // Updates errEl.textContent for the current script state.
            if (sel < minDate) { // Checks the condition before running the next script step.
                // ----- Order logic -----
                errEl.textContent   = 'Collection must be at least 2 days from today.'; // Updates errEl.textContent for the current script state.
                // ----- State updates -----
                errEl.style.display = 'block'; // Updates inline style for a dynamic UI state.
            } else if (sel > maxDate) { // Starts a JavaScript block for the current control flow.
                // ----- Order logic -----
                errEl.textContent   = 'Collection date must be within the next 14 days.'; // Updates errEl.textContent for the current script state.
                // ----- State updates -----
                errEl.style.display = 'block'; // Updates inline style for a dynamic UI state.
            }
        });
    }


    // ----- DOM references -----
    var placeBtn = document.getElementById('placeOrderBtn'); // Stores the placeBtn DOM element reference.
    if (placeBtn) { // Checks the condition before running the next script step.
        // ----- Helpers -----
        setTimeout(function () { // Starts a JavaScript block for the current control flow.
            // ----- State updates -----
            placeBtn.style.animation = 'none'; // Updates inline style for a dynamic UI state.
            placeBtn.style.transform = 'scale(1.04)'; // Updates inline style for a dynamic UI state.
            // ----- Helpers -----
            setTimeout(function () { // Starts a JavaScript block for the current control flow.
                // ----- State updates -----
                placeBtn.style.transform = ''; // Updates inline style for a dynamic UI state.
                placeBtn.style.animation = 'co-pulse 2.8s ease-in-out infinite'; // Updates inline style for a dynamic UI state.
            }, 320); // Runs this JavaScript step for the page interaction.
        }, 6000); // Runs this JavaScript step for the page interaction.
    }

}());
