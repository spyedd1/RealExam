(function () {
    var jumpLinks = Array.prototype.slice.call(document.querySelectorAll('.au-jump-link'));
    if (!jumpLinks.length || !('IntersectionObserver' in window)) return;

    var sections = jumpLinks.map(function (link) {
        var hash = link.getAttribute('href') || '';
        return hash.charAt(0) === '#' ? document.getElementById(hash.slice(1)) : null;
    }).filter(Boolean);

    if (!sections.length) return;

    function setActive(id) {
        jumpLinks.forEach(function (link) {
            var active = link.getAttribute('href') === '#' + id;
            link.classList.toggle('is-active', active);

            if (active) link.setAttribute('aria-current', 'location');
            else link.removeAttribute('aria-current');
        });
    }

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) setActive(entry.target.id);
        });
    }, {
        rootMargin: '-35% 0px -45% 0px',
        threshold: 0.15
    });

    sections.forEach(function (section) {
        observer.observe(section);
    });

    setActive(sections[0].id);
}());
