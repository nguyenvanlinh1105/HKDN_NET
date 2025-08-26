var DepartmentController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    function registerEvents() {
        $('#ddl-show-page').select2({
            minimumResultsForSearch: -1,
            width: '30%'
        });
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                $('#btn-search').click();
            }
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#btn-view-tree").on('click', function () {
            $.get("/departments/GetTree", function (response) {
                if (response.succeeded) {
                    $('#org-chart-container').html("");
                    $('#department-tree-view').removeClass('d-none');
                    $('#department-list-view').addClass('d-none');
                    $('#btn-view-tree').addClass('d-none');
                    $('#btn-view-table').removeClass('d-none');
                    $('#tree-loading').removeClass('d-none');

                    setTimeout(() => {
                        new Treant(response.data);
                        $('#tree-loading').addClass('d-none');
                    }, 200);
                } else {
                    base.notify(window.localization.OperationFailed || 'Something went wrong', 'error');
                }
            });
        });

        $("#btn-view-table").on('click', function () {
            $('#department-tree-view').addClass('d-none');
            $('#department-list-view').removeClass('d-none');
            $('#btn-view-tree').removeClass('d-none');
            $('#btn-view-table').addClass('d-none');
        });

        $('#check-all').on('change', function () {
            $('.check-row').prop('checked', this.checked);
        });

        $('#btn-delete-multiple').on('click', function () {
            const ids = $('.check-row:checked').map(function () {
                return parseInt($(this).val());
            }).get();

            if (ids.length === 0) {
                base.notify(window.localization.SelectAtLeastOne || 'Please select at least one department', 'warning');
                return;
            }

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete departments?",
                text: `${window.localization?.ConfirmDeleteMultipleText || 'You are about to delete'} ${ids.length} ${window.localization?.Departments || 'departments'}`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.ajax({
                        type: "POST",
                        url: "/departments/DeleteMultiple",
                        contentType: "application/json",
                        data: JSON.stringify(ids),
                        success: function (response) {
                            const msg = response.messages?.[0] || window.localization.OperationFailed || 'Something went wrong';
                            base.notify(msg, response.succeeded ? 'success' : 'error');
                            if (response.succeeded) loadData(true);
                        }
                    });
                }
            });
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            $.get(`/departments/GetById?id=${id}`, function (response) {
                if (response.succeeded) {
                    const d = response.data;
                    $('#form-edit-department [name="Id"]').val(d.id);
                    $('#form-edit-department [name="NameVi"]').val(d.nameVi);
                    $('#form-edit-department [name="NameEn"]').val(d.nameEn);
                    $('#form-edit-department [name="NameJa"]').val(d.nameJa);
                    $('#form-edit-department [name="ParentId"]').val(d.parentId || '');
                    $('#form-edit-department [name="Description"]').val(d.description);
                    loadParentDepartments('#ddlParentIdEdit');
                    $('#modalEditDepartment').modal('show');
                }
            });
        });

        $('body').on('submit', '#form-edit-department', function (e) {
            e.preventDefault();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => request[field.name] = field.value);

            const payload = {
                department: {
                    id: parseInt(request.Id),
                    nameVi: request.NameVi,
                    nameEn: request.NameEn,
                    nameJa: request.NameJa,
                    parentId: request.ParentId ? parseInt(request.ParentId) : null,
                    description: request.Description
                }
            };

            $.ajax({
                type: "PUT",
                url: "/departments/update",
                contentType: "application/json",
                data: JSON.stringify(payload),
                success: function (response) {
                    const msg = response.messages?.[0] || window.localization.OperationFailed || 'Something went wrong';
                    base.notify(msg, response.succeeded ? 'success' : 'error');
                    if (response.succeeded) {
                        $('#modalEditDepartment').modal('hide');
                        loadData(true);
                        resetForm('#form-edit-department'); 
                    }
                }
            });
        });

        $('body').on('submit', '#form-add-department', function (e) {
            e.preventDefault();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => request[field.name] = field.value);

            const payload = {
                department: {
                    nameVi: request.NameVi,
                    nameEn: request.NameEn,
                    nameJa: request.NameJa,
                    parentId: request.ParentId ? parseInt(request.ParentId) : null,
                    description: request.Description
                }
            };

            $.ajax({
                type: "POST",
                url: "/departments/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                success: function (response) {
                    const msg = response.messages?.[0] || window.localization.OperationFailed || 'Something went wrong';
                    base.notify(msg, response.succeeded ? 'success' : 'error');
                    if (response.succeeded) {
                        $('#modalAddDepartment').modal('hide');
                        loadData(true);
                        resetForm('#form-add-department'); 
                    }
                }
            });
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete department?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this department?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/departments/Delete", { id }, function (response) {
                        const msg = response.messages?.[0] || window.localization.OperationFailed || 'Something went wrong';
                        base.notify(msg, response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });
    }

    function loadData(isPageChanged) {
        $.get("/departments/GetAllPaging", {
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
                        NameVi: item.nameVi,
                        NameEn: item.nameEn,
                        Description: item.description,
                        ParentName: item.parentName,
                        Id: item.id
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html('<tr><td colspan="6" class="text-center text-muted">No records found.</td></tr>');
            }
        });
    }

    function loadParentDepartments(selectId = "#ddlParentId") {
        $.get("/departments/GetAll", function (response) {
            if (response.succeeded) {
                let options = `<option value="">${window.localization?.RootDepartment || 'Root Department'}</option>`;
                $.each(response.data, function (i, item) {
                    options += `<option value="${item.id}">${item.nameVi}</option>`; 
                });
                $(selectId).html(options);
                $(selectId).select2({
                    minimumResultsForSearch: -1,
                    width: '100%',
                }); 
            } else {
                console.log("Error loading parent departments:", response);
            }
        });
    }
    

    $('#modalAddDepartment').on('shown.bs.modal', function () {
        loadParentDepartments('#ddlParentId'); 
        $('#ddlParentId').select2({
            minimumResultsForSearch: -1,
            width: '100%',
        });
    });
    
    function resetForm(formSelector) {
        $(formSelector)[0].reset();  // Reset form
        $(formSelector + ' select').val('').trigger('change');  // Reset dropdowns trong form (ví dụ: Phòng ban cha)
        
        // Re-initialize select2 cho dropdowns trong form
        $(formSelector + ' select').select2({
            minimumResultsForSearch: -1,
            width: '100%',
        });
    }

};