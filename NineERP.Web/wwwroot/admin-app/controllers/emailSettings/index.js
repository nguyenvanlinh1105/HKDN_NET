var EmailSettingsController = function () {
    this.initialize = function () {
        loadSettings(); 
        registerEvents();
    };
    function toPascalCase(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    function loadSettings() {
        $.ajax({
            url: '/EmailSettings/GetData',
            type: 'GET',
            success: function (data) {
                if (!data) return;

                for (const key in data) {
                    if (data.hasOwnProperty(key)) {
                        const pascalKey = toPascalCase(key);
                        const input = $('#' + pascalKey);
                        if (input.length) {
                            input.val(data[key]);
                        }
                    }
                }
            },
            error: function () {
                toastr.error(window.localization.LoadError || 'Không thể tải dữ liệu cấu hình.');
            }
        });
    }

    function registerEvents() {
        $('#btn-save-email-settings').on('click', function () {
            clearValidation();
            if (!validateForm()) return;
            saveSettings();
        });

        $('#btn-send-test-email').on('click', function () {
            clearValidation();
            if (!validateTestEmail()) return;
            sendTestEmail();
        });
    }

    function validateForm() {
        let isValid = true;

        const protocol = $('#Protocol').val();

        require('SenderEmail', window.localization.Required_SenderEmail);
        require('SenderName', window.localization.Required_SenderName);

        if (protocol === 'SMTP') {
            require('SmtpHost', window.localization.Required_SmtpHost);
            require('SmtpPort', window.localization.Required_SmtpPort);
            require('SmtpUser', window.localization.Required_SmtpUser);
            require('SmtpPassword', window.localization.Required_SmtpPassword);
        }

        if (protocol === 'GmailOAuth2') {
            require('ClientId', window.localization.Required_ClientId);
            require('ClientSecret', window.localization.Required_ClientSecret);
            require('RefreshToken', window.localization.Required_RefreshToken);
        }

        if (protocol === 'MicrosoftOAuth2') {
            require('ClientId', window.localization.Required_ClientId);
            require('ClientSecret', window.localization.Required_ClientSecret);
            require('TenantId', window.localization.Required_TenantId);
        }

        return isValid;

        function require(id, message) {
            const $el = $('#' + id);
            if (!$el.val()) {
                markInvalid($el, message);
                isValid = false;
            }
        }
    }

    function validateTestEmail() {
        const email = $('#TestEmail').val();
        const regex = /^\S+@\S+\.\S+$/;

        if (!email) {
            markInvalid($('#TestEmail'), window.localization.Required_TestEmail);
            return false;
        }

        if (!regex.test(email)) {
            markInvalid($('#TestEmail'), window.localization.InvalidEmail);
            return false;
        }

        return true;
    }

    function markInvalid($el, message) {
        $el.addClass('is-invalid');
        if ($el.next('.invalid-feedback').length === 0) {
            $el.after(`<div class="invalid-feedback">${message}</div>`);
        } else {
            $el.next('.invalid-feedback').text(message);
        }
    }

    function clearValidation() {
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    }

    function saveSettings() {
        const formData = new FormData();
        $('[id]').each(function () {
            formData.append($(this).attr('id'), $(this).val());
        });

        const $btn = $('#btn-save-email-settings');
        const $spinner = $('#spinner-email');
        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');

        $.ajax({
            url: '/EmailSettings/SaveEmailSettings',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.succeeded) {
                    toastr.success(window.localization.SaveSuccess);
                } else {
                    toastr.error(response.message || window.localization.SaveFailed);
                }
            },
            error: function () {
                toastr.error(window.localization.SaveError);
            },
            complete: function () {
                $btn.prop('disabled', false);
                $spinner.addClass('d-none');
            }
        });
    }

    function sendTestEmail() {
        const email = $('#TestEmail').val();

        $.ajax({
            url: '/EmailSettings/SendTestEmail',
            type: 'POST',
            data: { testEmail: email },
            success: function (response) {
                if (response.succeeded) {
                    toastr.success(window.localization.SendTestSuccess);
                } else {
                    toastr.error(response.message || window.localization.SendTestFailed);
                }
            },
            error: function () {
                toastr.error(window.localization.SendTestError);
            }
        });
    }
};
