(function () {
    var inquiryTable = document.querySelector('.adm-table');
    if (!inquiryTable) return;

    inquiryTable.querySelectorAll('tr').forEach(function (row) {
        row.addEventListener('mouseenter', function () {
            row.classList.add('is-hovered');
        });

        row.addEventListener('mouseleave', function () {
            row.classList.remove('is-hovered');
        });
    });
}());
