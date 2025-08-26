var GeneralSettingsController = function () {
    this.initialize = function () {
        loadSettings(); 
        registerEvents();
    };

    function toPascalCase(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    function loadSettings() {
        $.ajax({
            url: '/GeneralSettings/GetData',
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
        $('#btn-save-settings').on('click', function () {
            clearValidation();
            if (!validateRequiredFields()) return;
            saveSettings();
        });
    }

    function validateRequiredFields() {
        let isValid = true;
        let firstInvalidTabId = null;

        const requiredFields = [
            { id: 'PasswordDefault', message: window.localization.Required_PasswordDefault },
            { id: 'FiscalYearStartDay', message: window.localization.Required_FiscalYearStartDay },
            { id: 'AnnualLeaveDays', message: window.localization.Required_AnnualLeaveDays },
        ];

        const numberFields = [
            { id: 'AnnualLeaveDays', message: 'Phải là số' },
            { id: 'InsuranceCompanyPercent', message: 'Phải là số' },
            { id: 'AccidentInsuranceCompanyPercent', message: 'Phải là số' },
            { id: 'UnionCompanyPercent', message: 'Phải là số' },
            { id: 'HealthInsuranceCompanyPercent', message: 'Phải là số' },
            { id: 'InsuranceEmployeePercent', message: 'Phải là số' },
            { id: 'AccidentInsuranceEmployeePercent', message: 'Phải là số' },
            { id: 'UnionEmployeePercent', message: 'Phải là số' },
            { id: 'HealthInsuranceEmployeePercent', message: 'Phải là số' },
            { id: 'IncomeTaxPercent', message: 'Phải là số' }
        ];

        requiredFields.forEach(field => {
            const input = $('#' + field.id);
            if (!input.val()) {
                markInvalid(input, field.message);
                isValid = false;
                firstInvalidTabId ??= getTabIdByInputId(field.id);
            }
        });

        const fiscalDate = $('#FiscalYearStartDay').val();
        const isValidDateFormat = /^(\d{2})\/(\d{2})$/.test(fiscalDate);

        if (fiscalDate && isValidDateFormat) {
            const [day, month] = fiscalDate.split('/').map(Number);

            if (
                day < 1 || day > 31 ||
                month < 1 || month > 12
            ) {
                markInvalid($('#FiscalYearStartDay'), window.localization.InvalidFiscalYearFormat || 'Ngày hoặc tháng không hợp lệ');
                isValid = false;
                firstInvalidTabId ??= getTabIdByInputId('FiscalYearStartDay');
            }
        } else if (fiscalDate) {
            markInvalid($('#FiscalYearStartDay'), window.localization.InvalidFiscalYearFormat || 'Định dạng ngày không hợp lệ (dd/mm)');
            isValid = false;
            firstInvalidTabId ??= getTabIdByInputId('FiscalYearStartDay');
        }

        
        const email = $('#Email').val();
        if (email && !/^\S+@\S+\.\S+$/.test(email)) {
            markInvalid($('#Email'), window.localization.InvalidEmail);
            isValid = false;
            firstInvalidTabId ??= getTabIdByInputId('Email');
        }

        numberFields.forEach(field => {
            const input = $('#' + field.id);
            const val = input.val();
            if (val && isNaN(val)) {
                markInvalid(input, window.localization.MustBeNumber);
                isValid = false;
                firstInvalidTabId ??= getTabIdByInputId(field.id);
            }
        });

        if (!isValid && firstInvalidTabId) {
            showTab(firstInvalidTabId);
            toastr.error(window.localization.RequiredFields || 'Vui lòng kiểm tra thông tin nhập vào.');
        }

        return isValid;
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

    function getTabIdByInputId(inputId) {
        const tabMap = {
            CompanyName: 'general-info',
            ShortName: 'general-info',
            TaxCode: 'general-info',
            PhoneNumber: 'general-info',
            Email: 'general-info',
            Address: 'general-info',
            BankName: 'general-info',
            BankAccountNumber: 'general-info',
            AccountHolder: 'general-info',
            PasswordDefault: 'system-info',
            FiscalYearStartDay: 'system-info',
            AnnualLeaveDays: 'system-info',
            ApprovedBy: 'system-info',
            CancelBy: 'system-info',
            InsuranceCompanyPercent: 'tax-info',
            AccidentInsuranceCompanyPercent: 'tax-info',
            UnionCompanyPercent: 'tax-info',
            HealthInsuranceCompanyPercent: 'tax-info',
            InsuranceEmployeePercent: 'tax-info',
            AccidentInsuranceEmployeePercent: 'tax-info',
            UnionEmployeePercent: 'tax-info',
            HealthInsuranceEmployeePercent: 'tax-info',
            IncomeTaxPercent: 'tax-info'
        };

        return tabMap[inputId];
    }

    function showTab(tabId) {
        $(`a[href="#${tabId}"]`).tab('show');
    }

    function saveSettings() {
        const formData = new FormData();
        $('[id]').each(function () {
            formData.append($(this).attr('id'), $(this).val());
        });

        const $btn = $('#btn-save-settings');
        const $spinner = $('#spinner-save');
        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');

        $.ajax({
            url: '/GeneralSettings/SaveGeneralSettings',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.succeeded) {
                    toastr.success(response.message || window.localization.SaveSuccess);
                } else {
                    toastr.error(response.message || window.localization.SaveFailed);
                }
            },
            error: function () {
                toastr.error(window.localization.SaveError || 'Có lỗi xảy ra khi lưu.');
            },
            complete: function () {
                $btn.prop('disabled', false);
                $spinner.addClass('d-none');
            }
        });
    }
};
