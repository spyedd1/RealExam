(function () {
    // Hooks the contact page fields so validation happens in the browser before submit.
    var form = document.getElementById('contactForm');
    var message = document.getElementById('Message');
    var counter = document.getElementById('messageCounter');
    var summary = document.getElementById('contactFormSummary');
    var summaryText = document.getElementById('contactFormSummaryText');

    if (!form || !message || !counter) return;

    var fields = [
        {
            element: document.getElementById('FullName'),
            max: 120,
            validate: function (value) {
                if (!value) return 'Please enter your full name.';
                if (value.length > 120) return 'Full name must be 120 characters or fewer.';
                return '';
            }
        },
        {
            element: document.getElementById('EmailAddress'),
            max: 160,
            validate: function (value) {
                var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

                if (!value) return 'Please enter your email address.';
                if (value.length > 160) return 'Email address must be 160 characters or fewer.';
                if (!emailPattern.test(value)) return 'Please enter a valid email address.';
                return '';
            }
        },
        {
            element: document.getElementById('Subject'),
            max: 160,
            validate: function (value) {
                if (!value) return 'Please enter a subject.';
                if (value.length > 160) return 'Subject must be 160 characters or fewer.';
                return '';
            }
        },
        {
            element: message,
            max: 3000,
            validate: function (value) {
                if (!value) return 'Please enter a message.';
                if (value.length > 3000) return 'Message must be 3000 characters or fewer.';
                return '';
            }
        }
    ];

    function updateCounter() {
        counter.textContent = message.value.length + ' / 3000';
    }

    function getErrorNode(field) {
        return form.querySelector('[data-valmsg-for="' + field.element.id + '"]');
    }

    function showSummary(text) {
        if (!summary || !summaryText) return;

        summaryText.textContent = text;
        summary.classList.remove('cu-alert--hidden');
    }

    function hideSummary() {
        if (!summary) return;
        summary.classList.add('cu-alert--hidden');
    }

    function validateField(field) {
        var errorNode = getErrorNode(field);
        var value = field.element.value;
        var errorMessage = field.validate(value.trim());
        field.element.classList.toggle('is-invalid', !!errorMessage);

        if (errorNode) {
            errorNode.textContent = errorMessage;
        }

        return !errorMessage;
    }

    function validateForm() {
        var hasErrors = false;

        fields.forEach(function (field) {
            if (!validateField(field)) {
                hasErrors = true;
            }
        });

        if (hasErrors) {
            showSummary('Please fix the highlighted fields and try again.');
        } else {
            hideSummary();
        }

        return !hasErrors;
    }

    fields.forEach(function (field) {
        if (!field.element) return;

        field.element.addEventListener('input', function () {
            validateField(field);
            if (fields.every(validateField)) {
                hideSummary();
            }
        });

        field.element.addEventListener('blur', function () {
            validateField(field);
        });
    });

    form.addEventListener('submit', function (event) {
        if (!validateForm()) {
            event.preventDefault();
        }
    });

    message.addEventListener('input', updateCounter);
    updateCounter();
}());
