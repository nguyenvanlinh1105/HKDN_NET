var KbnEmployeeStatusController = function () {
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

        $('#modaladdkbnemployeestatus').on('hidden.bs.modal', function () {
            $('#form-add-kbn-employee-status')[0].reset();
            $('#kbnemployeestatusid').val('');
            loadStatus();
        });

        $('#btn-show-add-modal').on('click', function () {
            $('#form-add-kbn-employee-status')[0].reset();
            $('#kbnemployeestatusid').val('');
            $('#modaladdkbnemployeestatusLabel').text(window.localization.AddNewKbnEmployeeStatus);
            loadStatus();
            $('#btnSaveAndContinue').show(); // Hiện lại nút
            $('#modaladdkbnemployeestatus').modal('show');
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete kbn employee status?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this kbn employee status?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/kbnemployeestatus/Delete", { id }, function (response) {
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

        $('body').on('submit', '#form-add-kbn-employee-status', function (e) {
            e.preventDefault();
            const id = $('#kbnemployeestatusid').val();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => {
                request[field.name] = field.value;
            });        
            
            const isSaveAndContinue = $(document.activeElement).attr('id') === 'btnSaveAndContinue';
            const isEdit = !!id;
            const payload = {
                kbnemployeestatus: {
                    id: isEdit ? id : undefined,
                    status: $('#ddlStatusId').val(),
                    nameVi: request.NameVi,
                    nameEn: request.NameEn,
                    nameJp: request.NameJp,
                    language: request.Language,
                    description: request.Description
                }
            };
            $.ajax({
                type: isEdit ? "PUT" : "POST",
                url: isEdit ? "/kbnemployeestatus/update" : "/kbnemployeestatus/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $('#form-add-kbn-employee-status button[type="submit"]').prop('disabled', true);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        if (isSaveAndContinue) {
                            $('#form-add-kbn-employee-status')[0].reset();
                            $('#kbnemployeestatusNameVi').focus();
                        } else {
                            $('#modaladdkbnemployeestatus').modal('hide');
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
                    $('#modaladdkbnemployeestatus button[type="submit"]').prop('disabled', false);
                }
            });
        });
    }

    function loadData(isPageChanged) {
        $.get("/kbnemployeestatus/getallpaging", {
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
                        Status: window.localization[item.status] || item.status,
                        NameVi: item.nameVi,
                        NameEn: item.nameEn,
                        NameJp: item.nameJp,
                        Description: item.description,
                        Id: item.id
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html(`<tr><td colspan="7" class="text-center text-muted">${window.localization?.NoRecordsFound || 'No records found.'}</td></tr>`);
            }
        });
    }

    function getById(id) {
        $.get("/kbnemployeestatus/GetById", { id: id }, function (response) {
            if (response && response.data) {
                const data = response.data;

                // Set tiêu đề modal
                $('#modaladdkbnemployeestatusLabel').text(window.localization.EditKbnEmployeeStatus);
                $('#btnSaveAndContinue').hide();
                // Đổ dữ liệu vào form                
                $('#kbnemployeestatusNameVi').val(data.nameVi);
                $('#kbnemployeestatusNameEn').val(data.nameEn);
                $('#kbnemployeestatusNameJp').val(data.nameJp);
                $('#kbnemployeestatuslanguage').val(data.language?.toLowerCase() ?? 'vi'); // dùng 'vi' mặc định
                $('#kbnemployeestatusdescription').val(data.description);
                $('#kbnemployeestatusid').val(data.id); 
                loadStatus(data.status);
                // Hiển thị modal
                $('#modaladdkbnemployeestatus').modal('show');
            } else {
                base.notify(response.messages[0], 'error');
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }

    function loadStatus(selectedValue) {
        $.get("/kbnemployeestatus/getstatus", function (response) {
            if (response.succeeded) {
                let options = "";
                $.each(response.data, function (i, item) {
                    options += `<option value="${item.id}" ${selectedValue == item.id ? 'selected' : ''}>${window.localization[item.name] || item.name}</option>`;
                });
                $("#ddlStatusId").html(options);
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
};
