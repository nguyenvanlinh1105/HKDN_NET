var RoleController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    function registerEvents() {

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) loadData(true);
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Thêm mới Role
        $('body').on('submit', '#form-add-role', function (e) {
            e.preventDefault();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(x => request[x.name] = x.value);

            $.ajax({
                type: "POST",
                url: "/roles/add",
                contentType: "application/json",
                data: JSON.stringify(request),
                success: function (response) {
                    base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                    if (response.succeeded) {
                        $('#modalAddRole').modal('hide');
                        loadData(true);
                    }
                }
            });
        });

        // Xóa Role
        $('body').on('click', '.btn-delete', function () {
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete role?",
                text: window.localization?.ConfirmDeleteText || "Are you sure?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/roles/delete", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });

        // Mở modal phân quyền
        $('body').on('click', '.btn-assign-permission', function () {
            const roleId = $(this).data('id');
            const roleName = $(this).data('name');
            $("#hidRoleId").val(roleId);
            $("#hidRoleName").val(roleName);

            $.get("/data/all-permissions", function (allPermissions) {
                $.get(`/roles/permissionwithrole?roleName=${roleName}`, function (currentPermissions) {
                    renderPermissionCheckbox(allPermissions, currentPermissions.map(x => x.permission));
                    $("#modal-permission").modal('show');
                });
            });
        });

        // Lưu phân quyền
        $('#btnSavePermission').on('click', function () {
            const roleName = $('#hidRoleName').val();
            const selectedPermissions = [];

            $('.permission-checkbox:checked').each(function () {
                selectedPermissions.push($(this).val());
            });

            $.post("/roles/savepermission", {
                roleName: roleName,
                listPermission: selectedPermissions
            }, function (response) {
                base.notify(response.messages, response.succeeded ? 'success' : 'error');
                if (response.succeeded) $('#modal-permission').modal('hide');
            });
        });
    }

    function renderPermissionCheckbox(groups, selectedPermissions) {
        let html = '';
        groups.forEach(group => {
            html += `<tr><td>${group.group}</td><td>`;
            group.permissions.forEach(p => {
                const checked = selectedPermissions.includes(p.value) ? 'checked' : '';
                html += `
                    <div class="form-check form-check-inline mb-1">
                        <input type="checkbox" class="form-check-input permission-checkbox" id="${p.value}" value="${p.value}" ${checked} />
                        <label class="form-check-label" for="${p.value}">${p.label}</label>
                    </div>
                `;
            });
            html += '</td></tr>';
        });

        $("#modal-permission tbody").html(html);
    }

    function loadData(isPageChanged) {
        $.get("/roles/getallpaging", {
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
                        Id: item.id,
                        Name: item.name,
                        Description: item.description,
                        CreatedOn: base.formatDateTime(item.createdOn)
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html('<tr><td colspan="5" class="text-center text-muted">No records found.</td></tr>');
            }
        });
    }
};
