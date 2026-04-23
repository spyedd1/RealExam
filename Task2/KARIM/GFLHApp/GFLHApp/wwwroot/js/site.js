// ----- Strict mode -----
'use strict'; // Enables stricter JavaScript parsing and safer runtime behavior.

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    document.querySelectorAll('a[href^="#"]').forEach(function (a) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        a.addEventListener('click', function (e) { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            var id = this.getAttribute('href').slice(1); // Stores the id value for later script logic.
            // ----- DOM references -----
            var target = id ? document.getElementById(id) : null; // Stores the target DOM element reference.
            if (!target) return; // Checks the condition before running the next script step.
            e.preventDefault(); // Stops the browser's default action for this event.
            target.scrollIntoView({ behavior: 'smooth', block: 'start' }); // Runs this JavaScript step for the page interaction.
        });
    });
}());

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var nav = document.getElementById('mainNav'); // Stores the nav DOM element reference.
    if (!nav) return; // Checks the condition before running the next script step.
    // ----- Helpers -----
    function tick() { nav.classList.toggle('is-scrolled', window.scrollY > 40); } // Defines the tick helper function.
    // ----- Event wiring -----
    window.addEventListener('scroll', tick, { passive: true }); // Registers an event handler for user or browser interaction.
    tick(); // Runs this JavaScript step for the page interaction.
}());

ScrollReveal().reveal('.reveal', { // Starts a JavaScript block for the current control flow.
    distance: '24px', // Runs this JavaScript step for the page interaction.
    duration: 600, // Runs this JavaScript step for the page interaction.
    easing: 'ease', // Runs this JavaScript step for the page interaction.
    origin: 'bottom', // Runs this JavaScript step for the page interaction.
    opacity: 0, // Runs this JavaScript step for the page interaction.
    interval: 0, // Runs this JavaScript step for the page interaction.
    reset: false, // Runs this JavaScript step for the page interaction.
    viewFactor: 0.1 // Runs this JavaScript step for the page interaction.
});


// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var hero = document.querySelector('.pw-hero'); // Stores the hero DOM element reference.
    // ----- Helpers -----
    var floats = Array.prototype.slice.call( // Stores the floats value for later script logic.
        // ----- DOM references -----
        document.querySelectorAll('.pw-hero__float')); // Finds a page element needed by the script.
    if (!hero || !floats.length) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    var depths = [14, 20, 9, 16, 7, 22]; // Stores the depths value for later script logic.
    var tX = 0, tY = 0, cX = 0, cY = 0; // Stores the tX value for later script logic.
    var raf = null; // Stores the raf value for later script logic.

    function lerp(a, b, t) { return a + (b - a) * t; } // Defines the lerp helper function.

    function step() { // Defines the step helper function.
        cX = lerp(cX, tX, 0.07); // Updates cX for the current script state.
        cY = lerp(cY, tY, 0.07); // Updates cY for the current script state.
        floats.forEach(function (f, i) { // Starts a JavaScript block for the current control flow.
            var d = depths[i] || 10; // Stores the d value for later script logic.
            // ----- State updates -----
            f.style.setProperty('--px', (cX * d).toFixed(2) + 'px'); // Updates inline style for a dynamic UI state.
            f.style.setProperty('--py', (cY * d).toFixed(2) + 'px'); // Updates inline style for a dynamic UI state.
        });
        raf = (Math.abs(cX - tX) < 0.002 && Math.abs(cY - tY) < 0.002) // Updates raf for the current script state.
            ? null // Runs this JavaScript step for the page interaction.
            // ----- Animation -----
            : requestAnimationFrame(step); // Schedules smooth visual updates for the next animation frame.
    }

    // ----- Event wiring -----
    hero.addEventListener('mousemove', function (e) { // Registers an event handler for user or browser interaction.
        // ----- Helpers -----
        var r = hero.getBoundingClientRect(); // Stores the r value for later script logic.
        tX = (e.clientX - r.left) / r.width - 0.5; // Updates tX for the current script state.
        tY = (e.clientY - r.top) / r.height - 0.5; // Updates tY for the current script state.
        // ----- Animation -----
        if (!raf) raf = requestAnimationFrame(step); // Checks the condition before running the next script step.
    }, { passive: true }); // Runs this JavaScript step for the page interaction.

    // ----- Event wiring -----
    hero.addEventListener('mouseleave', function () { // Registers an event handler for user or browser interaction.
        tX = 0; tY = 0; // Updates tX for the current script state.
        // ----- Animation -----
        if (!raf) raf = requestAnimationFrame(step); // Checks the condition before running the next script step.
    }, { passive: true }); // Runs this JavaScript step for the page interaction.
}());

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var bar = document.getElementById('scrollProgress'); // Stores the bar DOM element reference.
    if (!bar) return; // Checks the condition before running the next script step.
    var doc = document.documentElement; // Stores the doc value for later script logic.

    // ----- Helpers -----
    function update() { // Defines the update helper function.
        // ----- Basket logic -----
        var total = doc.scrollHeight - doc.clientHeight; // Stores the total value for later script logic.
        // ----- DOM references -----
        var pct = total > 0 ? (doc.scrollTop || document.body.scrollTop) / total * 100 : 0; // Stores the pct value for later script logic.
        // ----- State updates -----
        bar.style.width = pct.toFixed(2) + '%'; // Updates inline style for a dynamic UI state.
    }

    // ----- Event wiring -----
    window.addEventListener('scroll', update, { passive: true }); // Registers an event handler for user or browser interaction.
    update(); // Runs this JavaScript step for the page interaction.
}());

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var toggle   = document.getElementById('navToggle'); // Stores the toggle DOM element reference.
    var drawer   = document.getElementById('navDrawer'); // Stores the drawer DOM element reference.
    var backdrop = document.getElementById('navBackdrop'); // Stores the backdrop DOM element reference.
    if (!toggle || !drawer) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    function openMenu() { // Defines the openMenu helper function.
        // ----- Accessibility -----
        toggle.setAttribute('aria-expanded', 'true'); // Sets an attribute required by the UI state.
        // ----- State updates -----
        toggle.classList.add('is-active'); // Adds a CSS class to update the element state.
        drawer.classList.add('is-open'); // Adds a CSS class to update the element state.
        // ----- Accessibility -----
        drawer.setAttribute('aria-hidden', 'false'); // Sets an attribute required by the UI state.
        // ----- State updates -----
        backdrop.classList.add('is-visible'); // Adds a CSS class to update the element state.
        // ----- DOM references -----
        document.body.classList.add('pw-no-scroll'); // Adds a CSS class to update the element state.
    }

    // ----- Helpers -----
    function closeMenu() { // Defines the closeMenu helper function.
        // ----- Accessibility -----
        toggle.setAttribute('aria-expanded', 'false'); // Sets an attribute required by the UI state.
        // ----- State updates -----
        toggle.classList.remove('is-active'); // Removes a CSS class to update the element state.
        drawer.classList.remove('is-open'); // Removes a CSS class to update the element state.
        // ----- Accessibility -----
        drawer.setAttribute('aria-hidden', 'true'); // Sets an attribute required by the UI state.
        // ----- State updates -----
        backdrop.classList.remove('is-visible'); // Removes a CSS class to update the element state.
        // ----- DOM references -----
        document.body.classList.remove('pw-no-scroll'); // Removes a CSS class to update the element state.
    }

    // ----- Event wiring -----
    toggle.addEventListener('click', function () { // Registers an event handler for user or browser interaction.
        // ----- Accessibility -----
        toggle.getAttribute('aria-expanded') === 'true' ? closeMenu() : openMenu(); // Runs this JavaScript step for the page interaction.
    });

    // ----- Event wiring -----
    backdrop.addEventListener('click', closeMenu); // Registers an event handler for user or browser interaction.

    // ----- DOM references -----
    document.addEventListener('keydown', function (e) { // Registers an event handler for user or browser interaction.
        if (e.key === 'Escape') closeMenu(); // Checks the condition before running the next script step.
    });

    drawer.querySelectorAll('a').forEach(function (a) { // Finds a page element needed by the script.
        // ----- Event wiring -----
        a.addEventListener('click', closeMenu); // Registers an event handler for user or browser interaction.
    });
}());

// ----- Helpers -----
(function () { // Starts an isolated script scope for this page.
    // ----- DOM references -----
    var els = document.querySelectorAll('.pw-stat__value[data-target]'); // Stores the els DOM element reference.
    if (!els.length || !('IntersectionObserver' in window)) return; // Checks the condition before running the next script step.

    // ----- Helpers -----
    function countUp(el) { // Defines the countUp helper function.
        var target = parseFloat(el.dataset.target); // Stores the target value for later script logic.
        var suffix = el.dataset.suffix || ''; // Stores the suffix value for later script logic.
        var duration = 1400; // Stores the duration value for later script logic.
        var t0 = performance.now(); // Stores the t0 value for later script logic.

        (function tick(now) { // Starts an isolated script scope for this page.
            var p = Math.min((now - t0) / duration, 1); // Stores the p value for later script logic.
            var ease = 1 - Math.pow(1 - p, 3); // Stores the ease value for later script logic.
            el.textContent = Math.round(ease * target).toLocaleString() + suffix; // Updates el.textContent for the current script state.
            // ----- Animation -----
            if (p < 1) requestAnimationFrame(tick); // Checks the condition before running the next script step.
        }(t0)); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    var io = new IntersectionObserver(function (entries) { // Creates the io object used by the script.
        entries.forEach(function (entry) { // Starts a JavaScript block for the current control flow.
            if (!entry.isIntersecting) return; // Checks the condition before running the next script step.
            countUp(entry.target); // Runs this JavaScript step for the page interaction.
            io.unobserve(entry.target); // Runs this JavaScript step for the page interaction.
        });
    }, { threshold: 0.5 }); // Runs this JavaScript step for the page interaction.

    els.forEach(function (el) { io.observe(el); }); // Runs this JavaScript step for the page interaction.
}());

//
// SmoothScroll for websites v1.4.10 (Balazs Galambosi)
// http://www.smoothscroll.net/
//
// Licensed under the terms of the MIT license.
//
// You may use it in your theme if you credit me.
// It is also free to use on any individual website.
//
// Exception:
// The only restriction is to not publish any
// extension for browsers or native application
// without getting a written permission first.
//

(function () { // Starts an isolated script scope for this page.

    // Scroll Variables (tweakable)
    var defaultOptions = { // Stores the defaultOptions value for later script logic.

        // Scrolling Core
        frameRate: 150, // [Hz] // Runs this JavaScript step for the page interaction.
        animationTime: 600, // [ms] // Runs this JavaScript step for the page interaction.
        stepSize: 100, // [px] // Runs this JavaScript step for the page interaction.

        // Pulse (less tweakable)
        // ratio of "tail" to "acceleration"
        pulseAlgorithm: true, // Runs this JavaScript step for the page interaction.
        pulseScale: 4, // Runs this JavaScript step for the page interaction.
        pulseNormalize: 1, // Runs this JavaScript step for the page interaction.

        // Acceleration
        accelerationDelta: 50,  // 50 // Runs this JavaScript step for the page interaction.
        accelerationMax: 3,   // 3 // Runs this JavaScript step for the page interaction.

        // Keyboard Settings
        // ----- Accessibility -----
        keyboardSupport: true,  // option // Runs this JavaScript step for the page interaction.
        arrowScroll: 50,    // [px] // Runs this JavaScript step for the page interaction.

        // Other
        fixedBackground: true, // Runs this JavaScript step for the page interaction.
        excluded: '' // Runs this JavaScript step for the page interaction.
    };

    // ----- Helpers -----
    var options = defaultOptions; // Stores the options value for later script logic.


    // Other Variables
    var isExcluded = false; // Stores the isExcluded value for later script logic.
    var isFrame = false; // Stores the isFrame value for later script logic.
    var direction = { x: 0, y: 0 }; // Stores the direction value for later script logic.
    var initDone = false; // Stores the initDone value for later script logic.
    // ----- DOM references -----
    var root = document.documentElement; // Stores the root value for later script logic.
    // ----- Helpers -----
    var activeElement; // Stores the activeElement value for later script logic.
    var observer; // Stores the observer value for later script logic.
    var refreshSize; // Stores the refreshSize value for later script logic.
    // ----- Accessibility -----
    var deltaBuffer = []; // Stores the deltaBuffer value for later script logic.
    var deltaBufferTimer; // Stores the deltaBufferTimer value for later script logic.
    // ----- Helpers -----
    var isMac = /^Mac/.test(navigator.platform); // Stores the isMac value for later script logic.

    var key = { // Stores the key value for later script logic.
        left: 37, up: 38, right: 39, down: 40, spacebar: 32, // Runs this JavaScript step for the page interaction.
        pageup: 33, pagedown: 34, end: 35, home: 36 // Runs this JavaScript step for the page interaction.
    };
    var arrowKeys = { 37: 1, 38: 1, 39: 1, 40: 1 }; // Stores the arrowKeys value for later script logic.


    function initTest() { // Defines the initTest helper function.
        // ----- Accessibility -----
        if (options.keyboardSupport) { // Checks the condition before running the next script step.
            addEvent('keydown', keydown); // Runs this JavaScript step for the page interaction.
        }
    }

    // ----- Helpers -----
    function init() { // Defines the init helper function.

        // ----- DOM references -----
        if (initDone || !document.body) return; // Checks the condition before running the next script step.

        // ----- Startup -----
        initDone = true; // Updates initDone for the current script state.

        // ----- DOM references -----
        var body = document.body; // Stores the body value for later script logic.
        var html = document.documentElement; // Stores the html value for later script logic.
        // ----- Helpers -----
        var windowHeight = window.innerHeight; // Stores the windowHeight value for later script logic.
        var scrollHeight = body.scrollHeight; // Stores the scrollHeight value for later script logic.

        // check compat mode for root element
        // ----- DOM references -----
        root = (document.compatMode.indexOf('CSS') >= 0) ? html : body; // Updates root for the current script state.
        activeElement = body; // Updates activeElement for the current script state.

        // ----- Startup -----
        initTest(); // Runs this JavaScript step for the page interaction.


        if (top != self) { // Checks the condition before running the next script step.
            isFrame = true; // Updates isFrame for the current script state.
        }

        else if (isOldSafari && // Checks the next fallback condition in this script flow.
            scrollHeight > windowHeight && // Runs this JavaScript step for the page interaction.
            (body.offsetHeight <= windowHeight || // Updates < for the current script state.
                html.offsetHeight <= windowHeight)) { // Starts a JavaScript block for the current control flow.

            // ----- DOM references -----
            var fullPageElem = document.createElement('div'); // Stores the fullPageElem value for later script logic.
            // ----- State updates -----
            fullPageElem.style.cssText = 'position:absolute; z-index:-10000; ' + // Updates inline style for a dynamic UI state.
                'top:0; left:0; right:0; height:' + // Runs this JavaScript step for the page interaction.
                root.scrollHeight + 'px'; // Runs this JavaScript step for the page interaction.
            // ----- DOM references -----
            document.body.appendChild(fullPageElem); // Runs this JavaScript step for the page interaction.

            // DOM changed (throttled) to fix height
            // ----- Helpers -----
            var pendingRefresh; // Stores the pendingRefresh value for later script logic.
            refreshSize = function () { // Starts a JavaScript block for the current control flow.
                if (pendingRefresh) return; // could also be: clearTimeout(pendingRefresh); // Checks the condition before running the next script step.
                pendingRefresh = setTimeout(function () { // Starts a JavaScript block for the current control flow.
                    if (isExcluded) return; // could be running after cleanup // Checks the condition before running the next script step.
                    // ----- State updates -----
                    fullPageElem.style.height = '0'; // Updates inline style for a dynamic UI state.
                    fullPageElem.style.height = root.scrollHeight + 'px'; // Updates inline style for a dynamic UI state.
                    pendingRefresh = null; // Updates pendingRefresh for the current script state.
                }, 500); // act rarely to stay fast // Runs this JavaScript step for the page interaction.
            };

            // ----- Animation -----
            setTimeout(refreshSize, 10); // Runs this JavaScript step for the page interaction.

            addEvent('resize', refreshSize); // Runs this JavaScript step for the page interaction.

            // TODO: attributeFilter?
            // ----- Helpers -----
            var config = { // Stores the config value for later script logic.
                attributes: true, // Runs this JavaScript step for the page interaction.
                childList: true, // Runs this JavaScript step for the page interaction.
                characterData: false // Runs this JavaScript step for the page interaction.
                // subtree: true
            };

            observer = new MutationObserver(refreshSize); // Updates observer for the current script state.
            observer.observe(body, config); // Runs this JavaScript step for the page interaction.

            if (root.offsetHeight <= windowHeight) { // Checks the condition before running the next script step.
                // ----- DOM references -----
                var clearfix = document.createElement('div'); // Stores the clearfix value for later script logic.
                // ----- State updates -----
                clearfix.style.clear = 'both'; // Updates inline style for a dynamic UI state.
                body.appendChild(clearfix); // Runs this JavaScript step for the page interaction.
            }
        }

        // disable fixed background
        if (!options.fixedBackground && !isExcluded) { // Checks the condition before running the next script step.
            body.style.backgroundAttachment = 'scroll'; // Updates inline style for a dynamic UI state.
            html.style.backgroundAttachment = 'scroll'; // Updates inline style for a dynamic UI state.
        }
    }

    // ----- Helpers -----
    function cleanup() { // Defines the cleanup helper function.
        observer && observer.disconnect(); // Runs this JavaScript step for the page interaction.
        removeEvent(wheelEvent, wheel); // Runs this JavaScript step for the page interaction.
        removeEvent('mousedown', mousedown); // Runs this JavaScript step for the page interaction.
        removeEvent('keydown', keydown); // Runs this JavaScript step for the page interaction.
        removeEvent('resize', refreshSize); // Runs this JavaScript step for the page interaction.
        // ----- Startup -----
        removeEvent('load', init); // Runs this JavaScript step for the page interaction.
    }



    // ----- Helpers -----
    var que = []; // Stores the que value for later script logic.
    var pending = false; // Stores the pending value for later script logic.
    var lastScroll = Date.now(); // Stores the lastScroll value for later script logic.

    function scrollArray(elem, left, top) { // Defines the scrollArray helper function.

        directionCheck(left, top); // Runs this JavaScript step for the page interaction.

        if (options.accelerationMax != 1) { // Checks the condition before running the next script step.
            var now = Date.now(); // Stores the now value for later script logic.
            var elapsed = now - lastScroll; // Stores the elapsed value for later script logic.
            if (elapsed < options.accelerationDelta) { // Checks the condition before running the next script step.
                var factor = (1 + (50 / elapsed)) / 2; // Stores the factor value for later script logic.
                if (factor > 1) { // Checks the condition before running the next script step.
                    factor = Math.min(factor, options.accelerationMax); // Updates factor for the current script state.
                    left *= factor; // Updates * for the current script state.
                    top *= factor; // Updates * for the current script state.
                }
            }
            lastScroll = Date.now(); // Updates lastScroll for the current script state.
        }

        // push a scroll command
        que.push({ // Starts a JavaScript block for the current control flow.
            x: left, // Runs this JavaScript step for the page interaction.
            y: top, // Runs this JavaScript step for the page interaction.
            lastX: (left < 0) ? 0.99 : -0.99, // Runs this JavaScript step for the page interaction.
            lastY: (top < 0) ? 0.99 : -0.99, // Runs this JavaScript step for the page interaction.
            start: Date.now() // Runs this JavaScript step for the page interaction.
        });

        // don't act if there's a pending queue
        if (pending) { // Checks the condition before running the next script step.
            return; // Returns the computed value to the caller.
        }

        var scrollRoot = getScrollRoot(); // Stores the scrollRoot value for later script logic.
        // ----- DOM references -----
        var isWindowScroll = (elem === scrollRoot || elem === document.body); // Stores the isWindowScroll value for later script logic.

        // if we haven't already fixed the behavior,
        // and it needs fixing for this sesh
        if (elem.$scrollBehavior == null && isScrollBehaviorSmooth(elem)) { // Checks the condition before running the next script step.
            // ----- State updates -----
            elem.$scrollBehavior = elem.style.scrollBehavior; // Updates inline style for a dynamic UI state.
            elem.style.scrollBehavior = 'auto'; // Updates inline style for a dynamic UI state.
        }

        // ----- Helpers -----
        var step = function (time) { // Stores the step value for later script logic.

            var now = Date.now(); // Stores the now value for later script logic.
            var scrollX = 0; // Stores the scrollX value for later script logic.
            var scrollY = 0; // Stores the scrollY value for later script logic.

            for (var i = 0; i < que.length; i++) { // Loops through matching items for this script step.

                var item = que[i]; // Stores the item value for later script logic.
                var elapsed = now - item.start; // Stores the elapsed value for later script logic.
                var finished = (elapsed >= options.animationTime); // Stores the finished value for later script logic.

                // scroll position: [0, 1]
                var position = (finished) ? 1 : elapsed / options.animationTime; // Stores the position value for later script logic.

                // easing [optional]
                if (options.pulseAlgorithm) { // Checks the condition before running the next script step.
                    position = pulse(position); // Updates position for the current script state.
                }

                // only need the difference
                var x = (item.x * position - item.lastX) >> 0; // Stores the x value for later script logic.
                var y = (item.y * position - item.lastY) >> 0; // Stores the y value for later script logic.

                // add this to the total scrolling
                scrollX += x; // Updates + for the current script state.
                scrollY += y; // Updates + for the current script state.

                // update last values
                item.lastX += x; // Updates + for the current script state.
                item.lastY += y; // Updates + for the current script state.

                // delete and step back if it's over
                if (finished) { // Checks the condition before running the next script step.
                    que.splice(i, 1); i--; // Runs this JavaScript step for the page interaction.
                }
            }

            // scroll left and top
            if (isWindowScroll) { // Checks the condition before running the next script step.
                window.scrollBy(scrollX, scrollY); // Runs this JavaScript step for the page interaction.
            }
            else { // Handles the fallback branch for the previous condition.
                if (scrollX) elem.scrollLeft += scrollX; // Checks the condition before running the next script step.
                if (scrollY) elem.scrollTop += scrollY; // Checks the condition before running the next script step.
            }

            // clean up if there's nothing left to do
            if (!left && !top) { // Checks the condition before running the next script step.
                que = []; // Updates que for the current script state.
            }

            if (que.length) { // Checks the condition before running the next script step.
                requestFrame(step, elem, (1000 / options.frameRate + 1)); // Runs this JavaScript step for the page interaction.
            } else { // Starts a JavaScript block for the current control flow.
                pending = false; // Updates pending for the current script state.
                // restore default behavior at the end of scrolling sesh
                if (elem.$scrollBehavior != null) { // Checks the condition before running the next script step.
                    // ----- State updates -----
                    elem.style.scrollBehavior = elem.$scrollBehavior; // Updates inline style for a dynamic UI state.
                    elem.$scrollBehavior = null; // Updates elem.$scrollBehavior for the current script state.
                }
            }
        };

        // start a new queue of actions
        requestFrame(step, elem, 0); // Runs this JavaScript step for the page interaction.
        pending = true; // Updates pending for the current script state.
    }



    // ----- Helpers -----
    function wheel(event) { // Defines the wheel helper function.

        // ----- Startup -----
        if (!initDone) { // Checks the condition before running the next script step.
            init(); // Runs this JavaScript step for the page interaction.
        }

        // ----- Helpers -----
        var target = event.target; // Stores the target value for later script logic.

        // leave early if default action is prevented
        // or it's a zooming event with CTRL
        if (event.defaultPrevented || event.ctrlKey) { // Checks the condition before running the next script step.
            return true; // Returns the computed value to the caller.
        }

        // leave embedded content alone (flash & pdf)
        if (isNodeName(activeElement, 'embed') || // Checks the condition before running the next script step.
            (isNodeName(target, 'embed') && /\.pdf/i.test(target.src)) || // Runs this JavaScript step for the page interaction.
            isNodeName(activeElement, 'object') || // Runs this JavaScript step for the page interaction.
            target.shadowRoot) { // Starts a JavaScript block for the current control flow.
            return true; // Returns the computed value to the caller.
        }

        var deltaX = -event.wheelDeltaX || event.deltaX || 0; // Stores the deltaX value for later script logic.
        var deltaY = -event.wheelDeltaY || event.deltaY || 0; // Stores the deltaY value for later script logic.

        if (isMac) { // Checks the condition before running the next script step.
            if (event.wheelDeltaX && isDivisible(event.wheelDeltaX, 120)) { // Checks the condition before running the next script step.
                deltaX = -120 * (event.wheelDeltaX / Math.abs(event.wheelDeltaX)); // Updates deltaX for the current script state.
            }
            if (event.wheelDeltaY && isDivisible(event.wheelDeltaY, 120)) { // Checks the condition before running the next script step.
                deltaY = -120 * (event.wheelDeltaY / Math.abs(event.wheelDeltaY)); // Updates deltaY for the current script state.
            }
        }

        // use wheelDelta if deltaX/Y is not available
        if (!deltaX && !deltaY) { // Checks the condition before running the next script step.
            deltaY = -event.wheelDelta || 0; // Updates deltaY for the current script state.
        }

        // line based scrolling (Firefox mostly)
        if (event.deltaMode === 1) { // Checks the condition before running the next script step.
            deltaX *= 40; // Updates * for the current script state.
            deltaY *= 40; // Updates * for the current script state.
        }

        var overflowing = overflowingAncestor(target); // Stores the overflowing value for later script logic.

        // nothing to do if there's no element that's scrollable
        if (!overflowing) { // Checks the condition before running the next script step.
            // except Chrome iframes seem to eat wheel events, which we need to
            // propagate up, if the iframe has nothing overflowing to scroll
            if (isFrame && isChrome) { // Checks the condition before running the next script step.
                // change target to iframe element itself for the parent frame
                // ----- State updates -----
                Object.defineProperty(event, "target", { value: window.frameElement }); // Runs this JavaScript step for the page interaction.
                return parent.wheel(event); // Returns the computed value to the caller.
            }
            return true; // Returns the computed value to the caller.
        }

        // check if it's a touchpad scroll that should be ignored
        if (isTouchpad(deltaY)) { // Checks the condition before running the next script step.
            return true; // Returns the computed value to the caller.
        }

        // scale by step size
        // delta is 120 most of the time
        // synaptics seems to send 1 sometimes
        if (Math.abs(deltaX) > 1.2) { // Checks the condition before running the next script step.
            deltaX *= options.stepSize / 120; // Updates * for the current script state.
        }
        if (Math.abs(deltaY) > 1.2) { // Checks the condition before running the next script step.
            deltaY *= options.stepSize / 120; // Updates * for the current script state.
        }

        scrollArray(overflowing, deltaX, deltaY); // Runs this JavaScript step for the page interaction.
        event.preventDefault(); // Stops the browser's default action for this event.
        scheduleClearCache(); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function keydown(event) { // Defines the keydown helper function.

        var target = event.target; // Stores the target value for later script logic.
        var modifier = event.ctrlKey || event.altKey || event.metaKey || // Stores the modifier value for later script logic.
            (event.shiftKey && event.keyCode !== key.spacebar); // Runs this JavaScript step for the page interaction.

        // our own tracked active element could've been removed from the DOM
        // ----- DOM references -----
        if (!document.body.contains(activeElement)) { // Checks the condition before running the next script step.
            activeElement = document.activeElement; // Updates activeElement for the current script state.
        }

        // do nothing if user is editing text
        // or using a modifier key (except shift)
        // or in a dropdown
        // or inside interactive elements
        // ----- Event wiring -----
        var inputNodeNames = /^(textarea|select|embed|object)$/i; // Stores the inputNodeNames value for later script logic.
        var buttonTypes = /^(button|submit|radio|checkbox|file|color|image)$/i; // Stores the buttonTypes value for later script logic.
        if (event.defaultPrevented || // Checks the condition before running the next script step.
            inputNodeNames.test(target.nodeName) || // Runs this JavaScript step for the page interaction.
            isNodeName(target, 'input') && !buttonTypes.test(target.type) || // Runs this JavaScript step for the page interaction.
            isNodeName(activeElement, 'video') || // Runs this JavaScript step for the page interaction.
            isInsideYoutubeVideo(event) || // Runs this JavaScript step for the page interaction.
            // ----- Accessibility -----
            target.isContentEditable || // Runs this JavaScript step for the page interaction.
            modifier) { // Starts a JavaScript block for the current control flow.
            return true; // Returns the computed value to the caller.
        }

        // [spacebar] should trigger button press, leave it alone
        if ((isNodeName(target, 'button') || // Checks the condition before running the next script step.
            // ----- Event wiring -----
            isNodeName(target, 'input') && buttonTypes.test(target.type)) && // Runs this JavaScript step for the page interaction.
            event.keyCode === key.spacebar) { // Starts a JavaScript block for the current control flow.
            return true; // Returns the computed value to the caller.
        }

        // [arrwow keys] on radio buttons should be left alone
        if (isNodeName(target, 'input') && target.type == 'radio' && // Checks the condition before running the next script step.
            arrowKeys[event.keyCode]) { // Starts a JavaScript block for the current control flow.
            return true; // Returns the computed value to the caller.
        }

        // ----- Helpers -----
        var shift, x = 0, y = 0; // Stores the shift value for later script logic.
        var overflowing = overflowingAncestor(activeElement); // Stores the overflowing value for later script logic.

        if (!overflowing) { // Checks the condition before running the next script step.
            // Chrome iframes seem to eat key events, which we need to
            // propagate up, if the iframe has nothing overflowing to scroll
            return (isFrame && isChrome) ? parent.keydown(event) : true; // Returns the computed value to the caller.
        }

        var clientHeight = overflowing.clientHeight; // Stores the clientHeight value for later script logic.

        // ----- DOM references -----
        if (overflowing == document.body) { // Checks the condition before running the next script step.
            clientHeight = window.innerHeight; // Updates clientHeight for the current script state.
        }

        switch (event.keyCode) { // Starts a JavaScript block for the current control flow.
            case key.up: // Runs this JavaScript step for the page interaction.
                y = -options.arrowScroll; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.down: // Runs this JavaScript step for the page interaction.
                y = options.arrowScroll; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.spacebar: // (+ shift) // Runs this JavaScript step for the page interaction.
                shift = event.shiftKey ? 1 : -1; // Updates shift for the current script state.
                y = -shift * clientHeight * 0.9; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.pageup: // Runs this JavaScript step for the page interaction.
                y = -clientHeight * 0.9; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.pagedown: // Runs this JavaScript step for the page interaction.
                y = clientHeight * 0.9; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.home: // Runs this JavaScript step for the page interaction.
                if (overflowing == document.body && document.scrollingElement) // Checks the condition before running the next script step.
                    overflowing = document.scrollingElement; // Updates overflowing for the current script state.
                y = -overflowing.scrollTop; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.end: // Runs this JavaScript step for the page interaction.
                // ----- Helpers -----
                var scroll = overflowing.scrollHeight - overflowing.scrollTop; // Stores the scroll value for later script logic.
                var scrollRemaining = scroll - clientHeight; // Stores the scrollRemaining value for later script logic.
                y = (scrollRemaining > 0) ? scrollRemaining + 10 : 0; // Updates y for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.left: // Runs this JavaScript step for the page interaction.
                x = -options.arrowScroll; // Updates x for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            case key.right: // Runs this JavaScript step for the page interaction.
                x = options.arrowScroll; // Updates x for the current script state.
                break; // Runs this JavaScript step for the page interaction.
            default: // Runs this JavaScript step for the page interaction.
                return true; // a key we don't care about // Returns the computed value to the caller.
        }

        scrollArray(overflowing, x, y); // Runs this JavaScript step for the page interaction.
        event.preventDefault(); // Stops the browser's default action for this event.
        scheduleClearCache(); // Runs this JavaScript step for the page interaction.
    }

    function mousedown(event) { // Defines the mousedown helper function.
        activeElement = event.target; // Updates activeElement for the current script state.
    }



    var uniqueID = (function () { // Stores the uniqueID value for later script logic.
        var i = 0; // Stores the i value for later script logic.
        return function (el) { // Returns the computed value to the caller.
            return el.uniqueID || (el.uniqueID = i++); // Returns the computed value to the caller.
        };
    })();

    var cacheX = {}; // cleared out after a scrolling session // Stores the cacheX value for later script logic.
    var cacheY = {}; // cleared out after a scrolling session // Stores the cacheY value for later script logic.
    var clearCacheTimer; // Stores the clearCacheTimer value for later script logic.
    var smoothBehaviorForElement = {}; // Stores the smoothBehaviorForElement value for later script logic.

    //setInterval(function () { cache = {}; }, 10 * 1000);

    function scheduleClearCache() { // Defines the scheduleClearCache helper function.
        clearTimeout(clearCacheTimer); // Runs this JavaScript step for the page interaction.
        clearCacheTimer = setInterval(function () { // Starts a JavaScript block for the current control flow.
            cacheX = cacheY = smoothBehaviorForElement = {}; // Updates cacheX for the current script state.
        }, 1 * 1000); // Runs this JavaScript step for the page interaction.
    }

    function setCache(elems, overflowing, x) { // Defines the setCache helper function.
        var cache = x ? cacheX : cacheY; // Stores the cache value for later script logic.
        for (var i = elems.length; i--;) // Loops through matching items for this script step.
            cache[uniqueID(elems[i])] = overflowing; // Updates cache[uniqueID(elems[i])] for the current script state.
        return overflowing; // Returns the computed value to the caller.
    }

    function getCache(el, x) { // Defines the getCache helper function.
        return (x ? cacheX : cacheY)[uniqueID(el)]; // Returns the computed value to the caller.
    }

    //  (body)                (root)
    //         | hidden | visible | scroll |  auto  |
    // hidden  |   no   |    no   |   YES  |   YES  |
    // visible |   no   |   YES   |   YES  |   YES  |
    // scroll  |   no   |   YES   |   YES  |   YES  |
    // auto    |   no   |   YES   |   YES  |   YES  |

    function overflowingAncestor(el) { // Defines the overflowingAncestor helper function.
        var elems = []; // Stores the elems value for later script logic.
        // ----- DOM references -----
        var body = document.body; // Stores the body value for later script logic.
        // ----- Helpers -----
        var rootScrollHeight = root.scrollHeight; // Stores the rootScrollHeight value for later script logic.
        do { // Starts a JavaScript block for the current control flow.
            var cached = getCache(el, false); // Stores the cached value for later script logic.
            if (cached) { // Checks the condition before running the next script step.
                return setCache(elems, cached); // Returns the computed value to the caller.
            }
            elems.push(el); // Runs this JavaScript step for the page interaction.
            if (rootScrollHeight === el.scrollHeight) { // Checks the condition before running the next script step.
                var topOverflowsNotHidden = overflowNotHidden(root) && overflowNotHidden(body); // Stores the topOverflowsNotHidden value for later script logic.
                var isOverflowCSS = topOverflowsNotHidden || overflowAutoOrScroll(root); // Stores the isOverflowCSS value for later script logic.
                if (isFrame && isContentOverflowing(root) || // Checks the condition before running the next script step.
                    !isFrame && isOverflowCSS) { // Starts a JavaScript block for the current control flow.
                    return setCache(elems, getScrollRoot()); // Returns the computed value to the caller.
                }
            } else if (isContentOverflowing(el) && overflowAutoOrScroll(el)) { // Starts a JavaScript block for the current control flow.
                return setCache(elems, el); // Returns the computed value to the caller.
            }
        } while ((el = el.parentElement)); // Updates ((el for the current script state.
    }

    function isContentOverflowing(el) { // Defines the isContentOverflowing helper function.
        return (el.clientHeight + 10 < el.scrollHeight); // Returns the computed value to the caller.
    }

    // typically for <body> and <html>
    function overflowNotHidden(el) { // Defines the overflowNotHidden helper function.
        var overflow = getComputedStyle(el, '').getPropertyValue('overflow-y'); // Stores the overflow value for later script logic.
        return (overflow !== 'hidden'); // Returns the computed value to the caller.
    }

    // for all other elements
    function overflowAutoOrScroll(el) { // Defines the overflowAutoOrScroll helper function.
        var overflow = getComputedStyle(el, '').getPropertyValue('overflow-y'); // Stores the overflow value for later script logic.
        return (overflow === 'scroll' || overflow === 'auto'); // Returns the computed value to the caller.
    }

    // for all other elements
    function isScrollBehaviorSmooth(el) { // Defines the isScrollBehaviorSmooth helper function.
        var id = uniqueID(el); // Stores the id value for later script logic.
        if (smoothBehaviorForElement[id] == null) { // Checks the condition before running the next script step.
            var scrollBehavior = getComputedStyle(el, '')['scroll-behavior']; // Stores the scrollBehavior value for later script logic.
            smoothBehaviorForElement[id] = ('smooth' == scrollBehavior); // Runs this JavaScript step for the page interaction.
        }
        return smoothBehaviorForElement[id]; // Returns the computed value to the caller.
    }



    function addEvent(type, fn, arg) { // Defines the addEvent helper function.
        // ----- Event wiring -----
        window.addEventListener(type, fn, arg || false); // Registers an event handler for user or browser interaction.
    }

    // ----- Helpers -----
    function removeEvent(type, fn, arg) { // Defines the removeEvent helper function.
        window.removeEventListener(type, fn, arg || false); // Runs this JavaScript step for the page interaction.
    }

    function isNodeName(el, tag) { // Defines the isNodeName helper function.
        return el && (el.nodeName || '').toLowerCase() === tag.toLowerCase(); // Returns the computed value to the caller.
    }

    function directionCheck(x, y) { // Defines the directionCheck helper function.
        x = (x > 0) ? 1 : -1; // Updates x for the current script state.
        y = (y > 0) ? 1 : -1; // Updates y for the current script state.
        if (direction.x !== x || direction.y !== y) { // Checks the condition before running the next script step.
            direction.x = x; // Updates direction.x for the current script state.
            direction.y = y; // Updates direction.y for the current script state.
            que = []; // Updates que for the current script state.
            lastScroll = 0; // Updates lastScroll for the current script state.
        }
    }

    // ----- Accessibility -----
    if (window.localStorage && localStorage.SS_deltaBuffer) { // Checks the condition before running the next script step.
        // ----- Producer logic -----
        try { // #46 Safari throws in private browsing for localStorage // Runs this JavaScript step for the page interaction.
            // ----- Accessibility -----
            deltaBuffer = localStorage.SS_deltaBuffer.split(','); // Updates deltaBuffer for the current script state.
        } catch (e) { } // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function isTouchpad(deltaY) { // Defines the isTouchpad helper function.
        if (!deltaY) return; // Checks the condition before running the next script step.
        // ----- Accessibility -----
        if (!deltaBuffer.length) { // Checks the condition before running the next script step.
            deltaBuffer = [deltaY, deltaY, deltaY]; // Updates deltaBuffer for the current script state.
        }
        deltaY = Math.abs(deltaY); // Updates deltaY for the current script state.
        deltaBuffer.push(deltaY); // Runs this JavaScript step for the page interaction.
        deltaBuffer.shift(); // Runs this JavaScript step for the page interaction.
        clearTimeout(deltaBufferTimer); // Runs this JavaScript step for the page interaction.
        deltaBufferTimer = setTimeout(function () { // Starts a JavaScript block for the current control flow.
            // ----- Producer logic -----
            try { // #46 Safari throws in private browsing for localStorage // Runs this JavaScript step for the page interaction.
                // ----- Accessibility -----
                localStorage.SS_deltaBuffer = deltaBuffer.join(','); // Updates localStorage.SS_deltaBuffer for the current script state.
            } catch (e) { } // Runs this JavaScript step for the page interaction.
        }, 1000); // Runs this JavaScript step for the page interaction.
        // ----- Helpers -----
        var dpiScaledWheelDelta = deltaY > 120 && allDeltasDivisableBy(deltaY); // win64 // Stores the dpiScaledWheelDelta value for later script logic.
        var tp = !allDeltasDivisableBy(120) && !allDeltasDivisableBy(100) && !dpiScaledWheelDelta; // Stores the tp value for later script logic.
        if (deltaY < 50) return true; // Checks the condition before running the next script step.
        return tp; // Returns the computed value to the caller.
    }

    function isDivisible(n, divisor) { // Defines the isDivisible helper function.
        return (Math.floor(n / divisor) == n / divisor); // Returns the computed value to the caller.
    }

    function allDeltasDivisableBy(divisor) { // Defines the allDeltasDivisableBy helper function.
        // ----- Accessibility -----
        return (isDivisible(deltaBuffer[0], divisor) && // Returns the computed value to the caller.
            isDivisible(deltaBuffer[1], divisor) && // Runs this JavaScript step for the page interaction.
            isDivisible(deltaBuffer[2], divisor)); // Runs this JavaScript step for the page interaction.
    }

    // ----- Helpers -----
    function isInsideYoutubeVideo(event) { // Defines the isInsideYoutubeVideo helper function.
        var elem = event.target; // Stores the elem value for later script logic.
        var isControl = false; // Stores the isControl value for later script logic.
        // ----- DOM references -----
        if (document.URL.indexOf('www.youtube.com/watch') != -1) { // Checks the condition before running the next script step.
            do { // Starts a JavaScript block for the current control flow.
                // ----- State updates -----
                isControl = (elem.classList && // Updates isControl for the current script state.
                    elem.classList.contains('html5-video-controls')); // Runs this JavaScript step for the page interaction.
                if (isControl) break; // Checks the condition before running the next script step.
            } while ((elem = elem.parentNode)); // Updates ((elem for the current script state.
        }
        return isControl; // Returns the computed value to the caller.
    }

    // ----- Helpers -----
    var requestFrame = (function () { // Stores the requestFrame value for later script logic.
        // ----- Animation -----
        return (window.requestAnimationFrame || // Returns the computed value to the caller.
            window.webkitRequestAnimationFrame || // Runs this JavaScript step for the page interaction.
            window.mozRequestAnimationFrame || // Runs this JavaScript step for the page interaction.
            // ----- Helpers -----
            function (callback, element, delay) { // Defines a helper function for this script.
                // ----- Animation -----
                window.setTimeout(callback, delay || (1000 / 60)); // Runs this JavaScript step for the page interaction.
            });
    })();

    // ----- Helpers -----
    var MutationObserver = (window.MutationObserver || // Stores the MutationObserver value for later script logic.
        window.WebKitMutationObserver || // Runs this JavaScript step for the page interaction.
        window.MozMutationObserver); // Runs this JavaScript step for the page interaction.

    var getScrollRoot = (function () { // Stores the getScrollRoot value for later script logic.
        // ----- DOM references -----
        var SCROLL_ROOT = document.scrollingElement; // Stores the SCROLL_ROOT value for later script logic.
        // ----- Helpers -----
        return function () { // Returns the computed value to the caller.
            if (!SCROLL_ROOT) { // Checks the condition before running the next script step.
                // ----- DOM references -----
                var dummy = document.createElement('div'); // Stores the dummy value for later script logic.
                // ----- State updates -----
                dummy.style.cssText = 'height:10000px;width:1px;'; // Updates inline style for a dynamic UI state.
                // ----- DOM references -----
                document.body.appendChild(dummy); // Runs this JavaScript step for the page interaction.
                var bodyScrollTop = document.body.scrollTop; // Stores the bodyScrollTop value for later script logic.
                var docElScrollTop = document.documentElement.scrollTop; // Stores the docElScrollTop value for later script logic.
                window.scrollBy(0, 3); // Runs this JavaScript step for the page interaction.
                if (document.body.scrollTop != bodyScrollTop) // Checks the condition before running the next script step.
                    (SCROLL_ROOT = document.body); // Updates (SCROLL_ROOT for the current script state.
                else // Handles the fallback branch for the previous condition.
                    (SCROLL_ROOT = document.documentElement); // Updates (SCROLL_ROOT for the current script state.
                window.scrollBy(0, -3); // Runs this JavaScript step for the page interaction.
                document.body.removeChild(dummy); // Runs this JavaScript step for the page interaction.
            }
            return SCROLL_ROOT; // Returns the computed value to the caller.
        };
    })();



    // ----- Helpers -----
    function pulse_(x) { // Defines the pulse_ helper function.
        var val, start, expx; // Stores the val value for later script logic.
        // test
        x = x * options.pulseScale; // Updates x for the current script state.
        if (x < 1) { // acceleartion // Checks the condition before running the next script step.
            val = x - (1 - Math.exp(-x)); // Updates val for the current script state.
        } else {     // tail // Runs this JavaScript step for the page interaction.
            // the previous animation ended here:
            start = Math.exp(-1); // Updates start for the current script state.
            // simple viscous drag
            x -= 1; // Updates - for the current script state.
            expx = 1 - Math.exp(-x); // Updates expx for the current script state.
            val = start + (expx * (1 - start)); // Updates val for the current script state.
        }
        return val * options.pulseNormalize; // Returns the computed value to the caller.
    }

    function pulse(x) { // Defines the pulse helper function.
        if (x >= 1) return 1; // Checks the condition before running the next script step.
        if (x <= 0) return 0; // Checks the condition before running the next script step.

        if (options.pulseNormalize == 1) { // Checks the condition before running the next script step.
            options.pulseNormalize /= pulse_(1); // Updates / for the current script state.
        }
        return pulse_(x); // Returns the computed value to the caller.
    }



    var userAgent = window.navigator.userAgent; // Stores the userAgent value for later script logic.
    var isEdge = /Edge/.test(userAgent); // thank you MS // Stores the isEdge value for later script logic.
    var isChrome = /chrome/i.test(userAgent) && !isEdge; // Stores the isChrome value for later script logic.
    var isSafari = /safari/i.test(userAgent) && !isEdge; // Stores the isSafari value for later script logic.
    var isMobile = /mobile/i.test(userAgent); // Stores the isMobile value for later script logic.
    var isIEWin7 = /Windows NT 6.1/i.test(userAgent) && /rv:11/i.test(userAgent); // Stores the isIEWin7 value for later script logic.
    var isOldSafari = isSafari && (/Version\/8/i.test(userAgent) || /Version\/9/i.test(userAgent)); // Stores the isOldSafari value for later script logic.
    var isEnabledForBrowser = (isChrome || isSafari || isIEWin7) && !isMobile; // Stores the isEnabledForBrowser value for later script logic.

    var supportsPassive = false; // Stores the supportsPassive value for later script logic.
    try { // Starts a JavaScript block for the current control flow.
        // ----- Event wiring -----
        window.addEventListener("test", null, Object.defineProperty({}, 'passive', { // Registers an event handler for user or browser interaction.
            // ----- Helpers -----
            get: function () { // Starts a JavaScript block for the current control flow.
                supportsPassive = true; // Updates supportsPassive for the current script state.
            }
        }));
    } catch (e) { } // Runs this JavaScript step for the page interaction.

    var wheelOpt = supportsPassive ? { passive: false } : false; // Stores the wheelOpt value for later script logic.
    // ----- DOM references -----
    var wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel'; // Stores the wheelEvent value for later script logic.

    if (wheelEvent && isEnabledForBrowser) { // Checks the condition before running the next script step.
        addEvent(wheelEvent, wheel, wheelOpt); // Runs this JavaScript step for the page interaction.
        addEvent('mousedown', mousedown); // Runs this JavaScript step for the page interaction.
        // ----- Startup -----
        addEvent('load', init); // Runs this JavaScript step for the page interaction.
    }



    // ----- Helpers -----
    function SmoothScroll(optionsToSet) { // Defines the SmoothScroll helper function.
        for (var key in optionsToSet) // Loops through matching items for this script step.
            if (defaultOptions.hasOwnProperty(key)) // Checks the condition before running the next script step.
                options[key] = optionsToSet[key]; // Updates options[key] for the current script state.
    }
    SmoothScroll.destroy = cleanup; // Updates SmoothScroll.destroy for the current script state.

    if (window.SmoothScrollOptions) // async API // Checks the condition before running the next script step.
        SmoothScroll(window.SmoothScrollOptions); // Runs this JavaScript step for the page interaction.

    if (typeof define === 'function' && define.amd) // Checks the condition before running the next script step.
        define(function () { // Starts a JavaScript block for the current control flow.
            return SmoothScroll; // Returns the computed value to the caller.
        });
    else if ('object' == typeof exports) // Checks the next fallback condition in this script flow.
        module.exports = SmoothScroll; // Updates module.exports for the current script state.
    else // Handles the fallback branch for the previous condition.
        window.SmoothScroll = SmoothScroll; // Updates window.SmoothScroll for the current script state.

})();
