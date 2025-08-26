var KbnLeaveTypesController = function () {
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

        $('#modaladdkbnleavetype').on('hidden.bs.modal', function () {
            $('#form-add-kbn-leave-type')[0].reset();
            $('#kbnleavetypeid').val('');
            loadLeaveTypeFlag();
        });

        $('#btn-show-add-modal').on('click', function () {
            $('#form-add-kbn-leave-type')[0].reset();
            $('#kbnleavetypeid').val('');
            $('#modaladdkbnleavetypeLabel').text(window.localization.AddNewKbnLeaveType);
            loadLeaveTypeFlag();
            $('#btnSaveAndContinue').show(); // Hiện lại nút
            $('#modaladdkbnleavetype').modal('show');
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete kbn leave type?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this kbn leave type?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/kbnleavetypes/Delete", { id }, function (response) {
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

        $('body').on('submit', '#form-add-kbn-leave-type', function (e) {
            e.preventDefault();
            const id = $('#kbnleavetypeid').val();
            const formData = $(this).serializeArray();             
            const request = {};
            formData.forEach(field => {
                request[field.name] = field.value;
            });        
            const isSaveAndContinue = $(document.activeElement).attr('id') === 'btnSaveAndContinue';
            const isEdit = !!id;
            const payload = {
                kbnleavetype: {
                    id: isEdit ? id : undefined,
                    leavetypeflag: $('#ddlLeaveTypeFlagId').val(),
                    acronym: request.Acronym,
                    nameVi: request.NameVi,
                    nameEn: request.NameEn,
                    nameJp: request.NameJp,
                    language: request.Language,
                    description: request.Description,                    
                }
            };            
            $.ajax({
                type: isEdit ? "PUT" : "POST",
                url: isEdit ? "/kbnleavetypes/update" : "/kbnleavetypes/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $('#form-add-kbn-leave-type button[type="submit"]').prop('disabled', true);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        if (isSaveAndContinue) {
                            $('#form-add-kbn-leave-type')[0].reset();
                            $('#kbnleavetypeNameVi').focus();
                        } else {
                            $('#modaladdkbnleavetype').modal('hide');
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
                    $('#modaladdkbnleavetype button[type="submit"]').prop('disabled', false);
                }
            });
        });
    }

    function loadData(isPageChanged) {
        $.get("/kbnleavetypes/getallpaging", {
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
                        LeaveTypeFlag: window.localization[item.leaveTypeFlag] || item.leaveTypeFlag,
                        Acronym: item.acronym,
                        NameVi: item.nameVi,
                        NameEn: item.nameEn,
                        NameJp: item.nameJp,
                        Description: item.description,
                        Id: item.id,                        
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
        $.get("/kbnleavetypes/GetById", { id: id }, function (response) {
            if (response && response.data) {
                const data = response.data;

                // Set tiêu đề modal
                $('#modaladdkbnleavetypeLabel').text(window.localization.EditKbnLeaveType);
                $('#btnSaveAndContinue').hide();
                // Đổ dữ liệu vào form                
                $('#kbnleavetypeNameVi').val(data.nameVi);
                $('#kbnleavetypeNameEn').val(data.nameEn);
                $('#kbnleavetypeNameJp').val(data.nameJp);
                $('#kbnleavetypelanguage').val(data.language?.toLowerCase() ?? 'vi'); // dùng 'vi' mặc định
                $('#kbnleavetypedescription').val(data.description);
                $('#kbnleavetypeid').val(data.id); 
                $('#kbnleavetypeacronym').val(data.acronym); 
                loadLeaveTypeFlag(data.leaveTypeFlag);
                // Hiển thị modal
                $('#modaladdkbnleavetype').modal('show');
            } else {
                base.notify(response.messages[0], 'error');
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }

    function loadLeaveTypeFlag(selectedValue) {
        $.get("/kbnleavetypes/getleavetypeflag", function (response) {
            if (response.succeeded) {
                let options = "";
                $.each(response.data, function (i, item) {
                    options += `<option value="${item.id}" ${selectedValue == item.id ? 'selected' : ''}>${window.localization[item.name] || item.name}</option>`;
                });
                $("#ddlLeaveTypeFlagId").html(options);
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
};
