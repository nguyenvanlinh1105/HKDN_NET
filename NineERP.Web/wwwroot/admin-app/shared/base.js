// ✅ base.js - core utilities only (đa ngôn ngữ dùng biến `localization` inject từ Razor)
const base = {
    configs: {
        pageSize: 10,
        pageIndex: 1
    },

    notify(message, type = 'info') {
        if (typeof toastr !== 'undefined') {
            switch (type) {
                case 'success':
                    toastr.success(message);
                    break;
                case 'warning':
                    toastr.warning(message);
                    break;
                case 'error':
                    toastr.error(message);
                    break;
                default:
                    toastr.info(message);
            }
        } else {
            alert(message);
        }
    },

    confirm(message, okCallback) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: localization?.confirmTitle || 'Are you sure?',
                text: message,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: localization?.confirmYes || 'Yes',
                cancelButtonText: localization?.confirmNo || 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    okCallback();
                }
            });
        } else {
            if (confirm(message)) okCallback();
        }
    },

    formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    },

    formatDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN');
    },

    startLoading() {
        $('.dv-loading').removeClass('hide');
    },

    stopLoading() {
        $('.dv-loading').addClass('hide');
    },

    getStatus(status) {
        return status === 1
            ? `<span class="badge bg-success">${localization?.Active || 'Active'}</span>`
            : `<span class="badge bg-danger">${localization?.Blocked || 'Blocked'}</span>`;
    },

    formatNumber(number, precision = 2) {
        if (!isFinite(number)) return number.toString();
        return number.toFixed(precision).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },

    convertToSlug(text) {
        return text.toLowerCase().replace(/ /g, '-').replace(/[^a-z0-9-]/g, '');
    },

    wrapPaging(recordCount, callBack, changePageSize = false) {
        const totalPages = Math.ceil(recordCount / base.configs.pageSize);

        if ($('#paginationUL a').length === 0 || changePageSize) {
            $('#paginationUL').empty().removeData("twbs-pagination").unbind("page");
        }

        $('#paginationUL').twbsPagination({
            totalPages: totalPages,
            visiblePages: 7,
            first: '<<',
            prev: '<',
            next: '>',
            last: '>>',
            onPageClick: function (event, page) {
                if (base.configs.pageIndex !== page) {
                    base.configs.pageIndex = page;
                    setTimeout(callBack, 200);
                }
            }
        });
    },

    convertNumbers(input) {
        const stringNumber = input.toString();
        return stringNumber.includes('.')
            ? parseFloat(input).toFixed(2).replace('.', ',')
            : input;
    },
    getOrigin() {
        return window.location.origin || "";
    }
};
