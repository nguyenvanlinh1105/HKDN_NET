$(document).ready(function () {
    // ✅ Logout handler
    $('#btnLogout').on('click', function (e) {
        e.preventDefault();

        const notify = window.localization?.LogoutProcessing || "Logging out...";
        const notifyDone = window.localization?.LogoutSuccess || "Logout successful";
        const notifyError = window.localization?.LogoutError || "Logout failed";

        if (typeof $.NotificationApp !== 'undefined') {
            $.NotificationApp.send(notify, "", "top-right", "rgba(0,0,0,0.2)", "info");
        }

        $.ajax({
            url: '/login/logout',
            type: 'POST',
            success: function () {
                window.location.href = '/login';
            },
            error: function () {
                if (typeof $.NotificationApp !== 'undefined') {
                    $.NotificationApp.send(notifyError, "", "top-right", "rgba(0,0,0,0.2)", "error");
                } else {
                    alert(notifyError);
                }
            }
        });
    });

    // ✅ CSRF token support for all Ajax POST/PUT
    $(document).ajaxSend(function (e, xhr, options) {
        if (["POST", "PUT"].includes(options.type.toUpperCase())) {
            const token = $('form input[name="__RequestVerificationToken"]').val();
            if (token) {
                xhr.setRequestHeader("RequestVerificationToken", token);
            }
        }
    });

    // ✅ Custom number validator
    if ($.validator && $.validator.addMethod) {
        $.validator.addMethod("customNumber", function (value) {
            return value.trim() !== "" && !isNaN(parseFloat(value)) && parseFloat(value) >= 0;
        }, window.localization?.ValidatorCustomNumber || "Requires a number >= 0");
    }
});