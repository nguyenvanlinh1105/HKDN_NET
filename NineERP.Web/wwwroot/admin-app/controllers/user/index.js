var UserController = function () {
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

        $("#ddlRoleName").on('change', function () {
            loadData(true);
        });

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) loadData(true);
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $('body').on('click', '.btn-active', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmChangeStatusTitle || "Change user status?",
                text: window.localization?.ConfirmChangeStatusText || "Do you want to lock/unlock this user?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/users/ChangeUserStatus", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete user?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this user?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/users/DeleteUser", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });
        $('body').on('submit', '#form-add-user', function (e) {
            e.preventDefault();
            const formData = $(this).serializeArray();
            const request = {};

            formData.forEach(field => {
                request[field.name] = field.value;
            });

            $.ajax({
                type: "POST",
                url: "/users/Register",
                contentType: "application/json",
                data: JSON.stringify({ request: request, origin: base.getOrigin() }),
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        $('#modal-add-user').modal('hide');
                        loadData(true);
                    } else {
                        base.notify(response.messages[0], 'error');
                    }
                },
                error: function () {
                    base.notify('Something went wrong.', 'error');
                }
            });
        });

    }

    function loadData(isPageChanged) {
        $.get("/users/GetAllUserPaging", {
            keyword: $('#txtKeyword').val(),
            roleName: $('#ddlRoleName').val(),
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
                        Email: item.email,
                        FullName: item.fullName,
                        Id: item.id,
                        AvatarUrl: item.avatarUrl
                            ? `<img src="${base.getOrigin()}${item.avatarUrl}" width="25" />`
                            : `<img src="/assets/images/users/no-avatar.png" width="25" />`,
                        CreatedOn: base.formatDateTime(item.createdOn),
                        Status: getUserStatus(item.lockoutEnabled, item.id)
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html('<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>');
            }
        });
    }

    function getUserStatus(status, id) {
        const label = status
            ? (window.localization?.Block || "Block")
            : (window.localization?.Active || "Active");
        const btnClass = status ? 'btn-danger' : 'btn-success';
        return `<button class="btn btn-sm ${btnClass} btn-active" data-id="${id}">${label}</button>`;
    }
};
