$(document).ready(function () {
    // ✅ Allow only numeric + comma input
    $(document).on('input', '.numeric-comma-input', function () {
        $(this).val($(this).val().replace(/[^0-9,]/g, ''));
    });

    // ✅ Format currency input (optional auto formatting)
    $(document).on('blur', '.format-currency', function () {
        const value = $(this).val().replaceAll(',', '').trim();
        if (!isNaN(value)) {
            const num = parseFloat(value).toFixed(2);
            $(this).val(num.replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        }
    });

    // ✅ Auto-slug generator
    $(document).on('keyup', '.auto-slug', function () {
        const source = $(this).val();
        const slug = source.toLowerCase()
            .normalize("NFD").replace(/\p{Diacritic}/gu, '')
            .replace(/[^a-z0-9]+/g, '-')
            .replace(/^-+|-+$/g, '');

        const targetSelector = $(this).data('slug-target');
        if (targetSelector) {
            $(targetSelector).val(slug);
        }
    });
});
