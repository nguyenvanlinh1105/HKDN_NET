var IntegrationSettingsController = function () {
    this.initialize = function () {
        loadSettings();
        registerEvents();
    };

    function toPascalCase(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    function loadSettings() {
        $.ajax({
            url: '/IntegrationSettings/GetAll',
            type: 'GET',
            success: function (data) {
                if (!data) return;

                data.forEach(setting => {
                    const prefix = toPascalCase(setting.type.toLowerCase());

                    for (const key in setting) {
                        if (setting.hasOwnProperty(key) && key !== 'type') {
                            const input = $('#' + prefix + '_' + toPascalCase(key));
                            if (input.length) {
                                input.val(setting[key]);
                            }
                        }
                    }
                });
            },
            error: function () {
                toastr.error(window.localization.LoadError || 'Không thể tải dữ liệu cấu hình.');
            }
        });
    }

    function registerEvents() {
        $('#btn-save-integration-settings').on('click', function () {
            clearValidation();
            if (!validateFields()) return;
            saveSettings();
        });

        $('#btn-test-upload').on('click', function () {
            sendTestUpload();
        });
    }

    function validateFields() {
        let isValid = true;
        let firstTab = null;

        const requiredPerType = {
            Drive: ['ServiceAccountJson', 'ParentFolderId'],
            //S3: ['AccessKey', 'SecretKey', 'Region', 'BucketName'],
            //ReCAPTCHA: ['PublicKey', 'PrivateKey'],
            //Calendar: ['ClientId', 'ClientSecret']
        };

        for (const type in requiredPerType) {
            const fields = requiredPerType[type];
            fields.forEach(field => {
                const id = `${type}_${field}`;
                const input = $('#' + id);
                if (input.length && !input.val()) {
                    markInvalid(input, window.localization.RequiredFields || 'Vui lòng nhập đầy đủ trường bắt buộc');
                    isValid = false;
                    firstTab ??= type;
                }
            });
        }

        // ✅ Validate JSON Format for Google Drive
        const driveJson = $('#Drive_ServiceAccountJson').val();
        if (driveJson) {
            try {
                JSON.parse(driveJson);
            } catch {
                markInvalid($('#Drive_ServiceAccountJson'), window.localization.InvalidJson || 'Dữ liệu JSON không hợp lệ.');
                isValid = false;
                firstTab ??= 'Drive';
            }
        }

        // ✅ Validate BucketName (must be lowercase letters, numbers, and hyphens)
        const bucketName = $('#S3_BucketName').val();
        if (bucketName && !/^[a-z0-9\-]{3,63}$/.test(bucketName)) {
            markInvalid($('#S3_BucketName'), window.localization.InvalidBucketName || 'Tên bucket không hợp lệ.');
            isValid = false;
            firstTab ??= 'S3';
        }

        // ✅ Validate Region
        const region = $('#S3_Region').val();
        if (region && !/^[a-z\-]+$/.test(region)) {
            markInvalid($('#S3_Region'), window.localization.InvalidRegion || 'Region không hợp lệ.');
            isValid = false;
            firstTab ??= 'S3';
        }

        if (!isValid && firstTab) {
            showTab(firstTab.toLowerCase());
            toastr.error(window.localization.RequiredFields || 'Vui lòng kiểm tra thông tin bắt buộc.');
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

    function showTab(tabId) {
        $(`a[href="#${tabId}-tab"]`).tab('show');
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
            url: '/IntegrationSettings/Save',
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
                toastr.error(window.localization.SaveError || 'Có lỗi khi lưu cấu hình');
            },
            complete: function () {
                $btn.prop('disabled', false);
                $spinner.addClass('d-none');
            }
        });
    }

    function sendTestUpload() {
        const fileInput = $('#TestUploadFile')[0];
        const folder = $('#TestUploadFolder').val();

        if (fileInput.files.length === 0) {
            toastr.warning(window.localization.SelectFileFirst || 'Vui lòng chọn file để upload thử.');
            return;
        }

        const formData = new FormData();
        formData.append('file', fileInput.files[0]);
        formData.append('subFolder', folder);

        const $btn = $('#btn-test-upload');
        const $spinner = $('#spinner-upload');
        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');

        $.ajax({
            url: '/IntegrationSettings/TestUpload',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.succeeded) {
                    toastr.success(window.localization.UploadSuccess || 'Tải file lên thành công.');
                    if (res.data) {
                        $('#UploadResultLink').html(`<a href="${res.data}" target="_blank">${window.localization.ViewFile || 'Xem file'}</a>`);
                    }
                } else {
                    toastr.error(res.message || window.localization.UploadFailed);
                }
            },
            error: function () {
                toastr.error(window.localization.UploadError);
            },
            complete: function () {
                $btn.prop('disabled', false);
                $spinner.addClass('d-none');
            }
        });
    }
};
