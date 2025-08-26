var forgotPasswordController = function () {
    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {
        $('#formForgotPassword').on('submit', function (e) {
            e.preventDefault();
            clearError();

            let email = $('#email').val().trim();
            let isValid = true;

            if (!email) {
                showError(forgotPasswordResources.emailRequired);
                isValid = false;
            } else if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
                showError(forgotPasswordResources.invalidEmail);
                isValid = false;
            }

            if (isValid) {
                showLoading();
                e.currentTarget.submit(); // ✅ Submit form đúng cách
            } else {
                hideLoading();
            }
        });
    };

    var showError = function (message) {
        $('#emailError').text(message);
    };

    var clearError = function () {
        $('#emailError').text('');
    };

    var showLoading = function () {
        $('#btnSubmit')
            .html('<i class="mdi mdi-loading mdi-spin"></i> ' + forgotPasswordResources.sending)
            .prop('disabled', true);
    };

    var hideLoading = function () {
        $('#btnSubmit')
            .html(forgotPasswordResources.sendResetLink)
            .prop('disabled', false);
    };
};
