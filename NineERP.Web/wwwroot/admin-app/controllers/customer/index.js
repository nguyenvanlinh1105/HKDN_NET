var CustomersController = function () {
    this.initialize = function () {
        locationHelper.initialize(function () {
            locationHelper.loadCountries($('#ddlCountry'), $('#ddlProvince'), null, null, window.localization);
            loadData();
            registerEvents();
        });
    };  
    function registerEvents() {
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) { // Enter key code
                e.preventDefault(); // Ngăn submit form nếu có
                $('#btn-search').click();
            }
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $('#modalAddCustomer').on('hidden.bs.modal', function () {
            $('#form-add-customer')[0].reset();
            $('#customerId').val('');
            loadStatus();
            loadType(1);
        });

        $('#btn-show-add-modal').on('click', function () {
            $('#form-add-customer')[0].reset();
            $('#customerId').val('');
            $('#modalAddCustomerLabel').text(window.localization.AddNewCustomer);
            loadStatus();
            loadType(1);
            $('#btnSaveAndContinue').show(); // Hiện lại nút
            $('#modalAddCustomer').modal('show');
            locationHelper.loadCountries($('#ddlCountry'), $('#ddlProvince'), null, null, window.localization);
            $('#ddlProvince').html(`<option value="">-- ${window.localization.SelectProvince} --</option>`).prop('disabled', true);
        });

        // Xử lý khi thay đổi loại khách hàng
        $('body').on('change', 'input[name="CustomerType"]', function () {
            toggleTaxNumberField();
        });

        $('#ddlCountry').on('change', function () {
            const selectedCountry = $(this).val();
            if (selectedCountry) {
                $('#ddlProvince').prop('disabled', false);
                locationHelper.loadProvinces($('#ddlProvince'), selectedCountry, null, window.localization);
            } else {
                $('#ddlProvince').html(`<option value="">-- ${window.localization.SelectProvince} --</option>`).prop('disabled', true);
            }
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete customer?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this customer?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/customers/Delete", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            getById(id);
        });

        $('body').on('submit', '#form-add-customer', function (e) {
            e.preventDefault();
            const id = $('#customerId').val();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => {
                request[field.name] = field.value;
            });
            const isSaveAndContinue = $(document.activeElement).attr('id') === 'btnSaveAndContinue';
            const isEdit = !!id;
            const payload = {
                datcustomer: {
                    id: isEdit ? id : undefined,
                    customerType: $('input[name="CustomerType"]:checked').val(),
                    companyName: request.CompanyName,
                    provinceId: $('#ddlProvince').val(),
                    countryId: $('#ddlCountry').val(),
                    website: request.Website,
                    phoneNo: request.PhoneNo,
                    email: request.Email,
                    taxNumber: request.TaxNumber,
                    address: request.Address,
                    bankAccountName: request.BankAccountName,
                    bankAccountNumber: request.BankAccountNumber,
                    bankName: request.BankName,
                    postalCode: request.PostalCode,
                    status: $('#ddlCustomerStatusId').val(),
                }
            };
            $.ajax({
                type: isEdit ? "PUT" : "POST",
                url: isEdit ? "/customers/update" : "/customers/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $('#form-add-customer button[type="submit"]').prop('disabled', true);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        if (isSaveAndContinue) {
                            $('#form-add-customer')[0].reset();
                            $('#CompanyName').focus();
                        } else {
                            $('#modalAddCustomer').modal('hide');
                        }
                        loadData(true);
                    } else {
                        base.notify(response.messages[0], 'error');
                    }
                },
                error: function () {
                    base.notify('Something went wrong.', 'error');
                },
                complete: function () {
                    $('#modalAddCustomer button[type="submit"]').prop('disabled', false);
                }
            });
        });
    }
    function loadData(isPageChanged) {
        $.get("/customers/getallpaging", {
            keyword: $('#txtKeyword').val(),
            pageSize: base.configs.pageSize,
            pageNumber: base.configs.pageIndex
        }, function (response) {
            const template = $('#table-template').html();
            const customerTypeMap = {
                1: "Company",
                2: "Personal"
            };
            const customerStatusMap = {
                1: "NEW",
                2: "CONNECTION",
                3: "UNCONNECTION"
            };            
            let render = "";
            $("#lbl-total-records").text(response.totalCount);
            
            if (response.totalCount > 0) {
                let no = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                $.each(response.data, function (i, item) {
                    render += Mustache.render(template, {
                        DisplayOrder: no++,
                        CustomerType: window.localization[customerTypeMap[item.customerType]] || customerTypeMap[item.customerType],
                        CompanyName: item.companyName,
                        CountryId: locationHelper.getCountryMap()[item.countryId] || item.countryId,
                        ProvinceId: locationHelper.getProvinceMap()[item.provinceId] || item.provinceId,
                        Website: item.website,
                        Status: window.localization[customerStatusMap[item.status]] || customerStatusMap[item.status],
                        PhoneNo: item.phoneNo,
                        Email: item.email,
                        TaxNumber: item.taxNumber,
                        Address: item.address,
                        BankAccountName: item.bankAccountName,
                        BankAccountNumber: item.bankAccountNumber,
                        BankName: item.bankName,
                        PostalCode: item.postalCode,
                        Id: item.id
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html(`<tr><td colspan="8" class="text-center text-muted">${window.localization?.NoRecordsFound || 'No records found.'}</td></tr>`);
            }
        });
    }
    function getById(id) {
        $.get("/customers/getbyid", { id: id }, function (response) {
            if (response && response.data) {
                const data = response.data;
                // Set tiêu đề modal
                $('#modalAddCustomerLabel').text(window.localization.EditCustomer);
                $('#btnSaveAndContinue').hide();
                // Đổ dữ liệu vào form
                $('#CompanyName').val(data.companyName);
                $('#Website').val(data.website);
                $('#Email').val(data.email);
                $('#PhoneNo').val(data.phoneNo);
                $('#TaxNumber').val(data.taxNumber);
                $('#Address').val(data.address);
                $('#PostalCode').val(data.postalCode);
                $('#BankAccountName').val(data.bankAccountName);
                $('#BankAccountNumber').val(data.bankAccountNumber);
                $('#BankName').val(data.bankName);
                loadStatus(data.status);
                $('#customerId').val(data.id);
                loadType(data.customerType);
                locationHelper.loadCountries($('#ddlCountry'), $('#ddlProvince'), data.countryId, data.provinceId, window.localization);
                // Hiển thị modal
                $('#modalAddCustomer').modal('show');
            } else {
                base.notify(response.messages[0], 'error');
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
    function loadStatus(selectedValue) {
        $.get("/customers/getstatus", function (response) {
            if (response.succeeded) {
                let options = "";
                $.each(response.data, function (i, item) {
                    options += `<option value="${item.id}" ${selectedValue == item.id ? 'selected' : ''}>${window.localization[item.name] || item.name}</option>`;
                });
                $("#ddlCustomerStatusId").html(options);
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
    function loadType(selectedValue) {
        $.get("/customers/gettype", function (response) {
            if (response.succeeded) {
                let html = '';
                $.each(response.data, function (i, item) {
                    const isChecked = selectedValue == item.id ? 'checked' : '';
                    const label = window.localization[item.name] || item.name;

                    html += `
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="CustomerType" id="CustomerType_${item.id}" value="${item.id}" ${isChecked} />
                        <label class="form-check-label" for="CustomerType_${item.id}">${label}</label>
                    </div>
                `;
                });
                $("#customerTypeContainer").html(html);
                toggleTaxNumberField();
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
    function toggleTaxNumberField() {
        const selectedType = $('input[name="CustomerType"]:checked').val();
        if (selectedType == 2) { // Personal
            $('#taxNumberContainer').hide();
        } else {
            $('#taxNumberContainer').show();
        }
    }
};
