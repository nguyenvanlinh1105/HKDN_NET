var MstShiftsController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
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

        $('#modalAddMstShift').on('hidden.bs.modal', function () {
            $('#form-add-mst-shift')[0].reset();
            $('#mstShiftId').val('');
        });

        $('#btn-show-add-modal').on('click', function () {
            $('#form-add-mst-shift')[0].reset();
            $('#mstShiftId').val('');
            $('#modalAddMstShiftLabel').text(window.localization.AddNewMstShift);
            $('#btnSaveAndContinue').show(); // Hiện lại nút
            $('#isDefault').closest('.form-group').show();  // hiển thị checkbox Default
            // Gán giá trị mặc định giờ
            $('#morningStartTime').val("08:00");
            $('#morningEndTime').val("12:00");
            $('#afternoonStartTime').val("13:00");
            $('#afternoonEndTime').val("17:00");
            $('#modalAddMstShift').modal('show');
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete Shift?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this shift?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/mstshifts/Delete", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            // Gọi hàm getById bạn đã viết
            getById(id);
        });

        $('body').on('change', '.radio-default-shift', function () {
            const id = $(this).data('id');
            Swal.fire({
                title: window.localization?.ConfirmDefaultTitle || 'Confirm Change Default Shift',
                text: window.localization?.ConfirmDefaultText || 'Do you want to change the default shift?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || 'Yes',
                cancelButtonText: window.localization?.No || 'No'
            }).then(result => {
                if (result.value) {
                    $.ajax({
                        url: "/mstshifts/setdefault",
                        type: 'PUT',
                        contentType: "application/json",
                        data: JSON.stringify({ id: id }),
                        success: function (response) {
                            base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                            if (response.succeeded) {
                                loadData(true);
                            } else {
                                $(this).prop('checked', false);
                            }
                        },
                        error: function () {
                            base.notify('Something went wrong.', 'error');
                            $(this).prop('checked', false);
                        }
                    });
                } else {
                    $(this).prop('checked', false);
                }
            });
        });

        $('body').on('submit', '#form-add-mst-shift', function (e) {
            e.preventDefault();
            const id = $('#mstShiftId ').val();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => {
                request[field.name] = field.value;
            });

            const isSaveAndContinue = $(document.activeElement).attr('id') === 'btnSaveAndContinue';
            const isEdit = !!id;
            const payload = {
                mstshift: {
                    id: isEdit ? id : undefined,
                    morningStartTime: request.MorningStartTime,
                    morningEndTime: request.MorningEndTime,
                    afternoonStartTime: request.AfternoonStartTime,
                    afternoonEndTime: request.AfternoonEndTime,
                    description: request.Description,
                    isDefault: $('#isDefault').is(':checked'),
                    totalHour: calculateTotalHour()
                }
            };
            $.ajax({
                type: isEdit ? "PUT" : "POST",
                url: isEdit ? "/mstshifts/update" : "/mstshifts/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $('#form-add-mst-shift button[type="submit"]').prop('disabled', true);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        if (isSaveAndContinue) {
                            $('#form-add-mst-shift')[0].reset();
                        } else {
                            $('#modalAddMstShift').modal('hide');
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
                    $('#modalAddMstShift button[type="submit"]').prop('disabled', false);
                }
            });
        });

        $('body').on('click', '.btn-detail', function (e) {
            e.preventDefault();
            const shiftId = $(this).data('id');
            window.currentShiftId = shiftId;

            $.get("/mstshifts/getemployeesbyshift", { shiftId: shiftId }, function (response) {
                const $modal = $('#employee-detail-modal-content');
                const $tbody = $modal.find('#employee-detail-body');
                $tbody.empty();

                if (response.succeeded && response.data.length > 0) {
                    response.data.forEach(emp => {
                        const row = `
                <tr>
                    <td><input type="checkbox" class="employee-checkbox" value="${emp.employeeCode}" /></td>
                    <td>${emp.employeeCode}</td>
                    <td>${emp.fullName}</td>
                </tr>`;
                        $tbody.append(row);
                    });

                    $modal.show(); // hiện modal (table)
                    updateMoveButtonState();

                } else {
                    $modal.hide(); // không có data thì ẩn đi
                    base.notify(window.localization?.NoEmployeesFound || 'No employees found for this shift.', 'warning');
                }
            }).fail(function () {
                base.notify('Failed to load employee list.', 'error');
            });
        });

        // Event khi click check all
        $('#select-all-employees').on('change', function () {
            const isChecked = $(this).is(':checked');
            $('.employee-checkbox').prop('checked', isChecked);
            updateMoveButtonState();
        });

        // Event khi click checkbox từng người
        $(document).on('change', '.employee-checkbox', function () {
            updateMoveButtonState();
        });

        // Move employee
        $(document).on('click', '.btn-move-employees', function () {
            const selected = $('.employee-checkbox:checked').map(function () {
                return $(this).val();
            }).get();

            if (selected.length === 0) return;

            $.get('/mstshifts/getallpaging', function (response) {
                if (response.succeeded) {
                    // Lọc bỏ ca hiện tại (không cho chọn lại)
                    const shifts = response.data.filter(x => x.id !== window.currentShiftId);
                    shifts.forEach((s, i) => {
                        s.DisplayOrder = i + 1;
                        s.MorningWork = `${s.morningStartTime} - ${s.morningEndTime}`;
                        s.AfternoonWork = `${s.afternoonStartTime} - ${s.afternoonEndTime}`;
                        s.TotalHour = s.totalHour || '';
                        s.NumberOfEmployee = s.numberOfEmployee || 0;
                        s.Description = s.description || '';
                    });
                    // Gắn số thứ tự
                    shifts.forEach((s, i) => s.DisplayOrder = i + 1);

                    const template = $('#move-shift-template').html();
                    const rendered = Mustache.render(template, { shifts });  // ✅ Sửa ở đây

                    const table = `
			            <div class="table-responsive">
                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th>${window.localization?.Number || '#'}</th>
                                        <th>${window.localization?.MorningWork || 'Morning Work'}</th>
                                        <th>${window.localization?.AfternoonWork || 'Afternoon Work'}</th>
                                        <th>${window.localization?.TotalHour || 'Total Hour'}</th>
                                        <th>${window.localization?.NumberOfEmployee || 'Employees'}</th>
                                        <th>${window.localization?.Action || 'Action'}</th>
                                    </tr>
                                </thead>
                                <tbody>${rendered}</tbody>
                            </table>
			            </div>`;

                    Swal.fire({
                        title: window.localization.SelectTargetShift,
                        html: table,
                        width: 'auto', // tự động theo nội dung
                        showConfirmButton: false
                    });
                    // ✅ Đợi Swal vẽ xong, rồi mới gán sự kiện
                    setTimeout(() => {
                        const btns = $('.btn-choose-shift');

                        btns.off('click').on('click', function () {
                            const toShiftId = $(this).data('id');

                            $.ajax({
                                type: 'PUT',
                                url: '/mstshifts/moveemployees',
                                contentType: 'application/json',
                                data: JSON.stringify({
                                    employeeCodes: selected,
                                    fromShiftId: window.currentShiftId,
                                    toShiftId: toShiftId
                                }),
                                success: function (res) {
                                    if (res.succeeded) {
                                        Swal.close();
                                        base.notify(res.message || window.localization.MovedSuccessfully, 'success');
                                        $('#employee-detail-modal-content').hide(); // ẩn modal nếu cần
                                        loadData(true);
                                    } else {
                                        base.notify(res.message || 'Move failed.', 'error');
                                    }
                                },
                                error: function () {
                                    base.notify('Server error.', 'error');
                                }
                            });
                        });
                    }, 100);
                }
            });
        });
    }

    function loadData(isPageChanged) {
        $.get("/mstshifts/getallpaging", {
            keyword: $('#txtKeyword').val(),
            pageSize: base.configs.pageSize,
            pageNumber: base.configs.pageIndex
        }, function (response) {
            const template = $('#table-template').html();
            let render = "";
            $("#lbl-total-records").text(response.totalCount);
            if (response.totalCount > 0) {
                let no = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                $.each(response.data, function (i, item) {
                    render += Mustache.render(template, {
                        DisplayOrder: no++,
                        MorningWork: `${item.morningStartTime} - ${item.morningEndTime}`,
                        AfternoonWork: `${item.afternoonStartTime} - ${item.afternoonEndTime}`,
                        Description: item.description,
                        IsDefault: item.isDefault,
                        NumberOfEmployee: item.numberOfEmployee,
                        TotalHour: item.totalHour,
                        Id: item.id,
                        HasEmployee: item.numberOfEmployee > 0 // ✅ truyền điều kiện
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
        $.get("/mstshifts/GetById", { id: id }, function (response) {
            if (response && response.data) {
                const data = response.data;
                // Set tiêu đề modal
                $('#modalAddMstShiftLabel').text(window.localization.EditMstShift);
                $('#btnSaveAndContinue').hide();
                // Đổ dữ liệu vào form
                $('#morningStartTime').val(data.morningStartTime);
                $('#morningEndTime').val(data.morningEndTime);
                $('#afternoonStartTime').val(data.afternoonStartTime);
                $('#afternoonEndTime').val(data.afternoonEndTime);
                $('#mstShiftDescription').val(data.description);
                $('#isDefault').prop('checked', data.isDefault);
                $('#mstShiftId').val(data.id);
                // Ẩn checkbox Default khi edit
                $('#isDefault').closest('.form-group').hide();

                // Hiển thị modal
                $('#modalAddMstShift').modal('show');
            } else {
                base.notify(response.messages[0], 'error');
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
    function calculateTotalHour() {
        const morningStart = $('#morningStartTime').val();
        const morningEnd = $('#morningEndTime').val();
        const afternoonStart = $('#afternoonStartTime').val();
        const afternoonEnd = $('#afternoonEndTime').val();

        function getMinutes(timeStr) {
            const parts = timeStr.split(":");
            return parseInt(parts[0]) * 60 + parseInt(parts[1]);
        }

        let totalMinutes = 0;

        if (morningStart && morningEnd) {
            totalMinutes += getMinutes(morningEnd) - getMinutes(morningStart);
        }
        if (afternoonStart && afternoonEnd) {
            totalMinutes += getMinutes(afternoonEnd) - getMinutes(afternoonStart);
        }

        return (totalMinutes / 60).toFixed(2);
    }
    function updateMoveButtonState() {
        const anyChecked = $('.employee-checkbox:checked').length > 0;
        $('.btn-move-employees').prop('disabled', !anyChecked);
    }
};
