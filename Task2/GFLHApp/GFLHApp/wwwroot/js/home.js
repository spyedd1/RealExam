(function () {
    var ease = 'cubic-bezier(0.5, 0, 0, 1)';

    var sr = ScrollReveal({
        distance: '40px',
        duration: 700,
        easing: ease,
        viewFactor: 0.15,
        reset: false
    });

    // Hero
    sr.reveal('.section-hero .title__main', { origin: 'bottom', delay: 100 });
    sr.reveal('.section-hero .title__sub', { origin: 'bottom', delay: 220 });
    sr.reveal('.section-hero .btn-secondary', { origin: 'bottom', delay: 340 });
    sr.reveal('.action-btn-wrap .btn', { origin: 'bottom', delay: 100, interval: 120 });

    // Text banner
    sr.reveal('.text-banner', { origin: 'left', distance: '60px', duration: 800, delay: 80 });

    // Live Local
    sr.reveal('.section-live-local .img-grid', { origin: 'left', distance: '60px', duration: 800 });
    sr.reveal('.section-live-local .pre-header', { origin: 'bottom', delay: 100 });
    sr.reveal('.section-live-local h2', { origin: 'bottom', delay: 180 });
    sr.reveal('.section-live-local p', { origin: 'bottom', delay: 260 });
    sr.reveal('.section-live-local .btn-primary', { origin: 'bottom', delay: 340 });

    // Why
    sr.reveal('.section-why h2', { origin: 'left', distance: '50px', delay: 80 });
    sr.reveal('.section-why .ul-icon-list > li', { origin: 'left', distance: '40px', interval: 90, delay: 120 });
    sr.reveal('.section-why .btn-secondary', { origin: 'bottom', delay: 80 });
    sr.reveal('.section-why .img-grid', { origin: 'right', distance: '60px', duration: 800, delay: 100 });

    // One Box Shop
    sr.reveal('.section-img-grid .title__main', { origin: 'bottom', delay: 80 });
    sr.reveal('.grid-info', { origin: 'bottom', distance: '50px', interval: 140, duration: 750 });
    sr.reveal('.section-img-grid > .d-flex .btn', { origin: 'bottom', delay: 100 });

    // How it Works
    sr.reveal('.section-how .how-title', { origin: 'bottom', delay: 80 });
    sr.reveal('.section-how .how-desc', { origin: 'bottom', delay: 160 });
    sr.reveal('.how-step', { origin: 'bottom', distance: '50px', interval: 150, delay: 120 });
    sr.reveal('.section-how .row.mt-3 .btn', { origin: 'bottom', interval: 120, delay: 200 });

    // Categories
    sr.reveal('.section-categories .title__main', { origin: 'bottom', delay: 80 });
    sr.reveal('.card-category', { origin: 'bottom', distance: '40px', interval: 100, duration: 650 });

    // Delivery
    sr.reveal('.section-delivery .img-grid', { origin: 'left', distance: '60px', duration: 800 });
    sr.reveal('.section-delivery h2', { origin: 'right', distance: '50px', delay: 100 });
    sr.reveal('.section-delivery p', { origin: 'right', distance: '50px', delay: 200 });

    // Testimonials
    sr.reveal('.section-testimonials .title__main', { origin: 'bottom', delay: 80 });
    sr.reveal('.testimonial', { origin: 'bottom', distance: '40px', interval: 130, duration: 700 });
    sr.reveal('.section-testimonials .d-flex .btn', { origin: 'bottom', delay: 100 });

    // Impact grid cards
    sr.reveal('.grid-card', { origin: 'bottom', distance: '30px', scale: 0.96, interval: 80, duration: 600 });

    // Pre-footer CTA
    sr.reveal('.pre-footer h2', { origin: 'left', distance: '50px', duration: 800 });
    sr.reveal('.pre-footer form', { origin: 'right', distance: '50px', duration: 800, delay: 120 });
})();