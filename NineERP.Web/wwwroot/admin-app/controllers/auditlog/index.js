var AuditLogController = function () {
    this.initialize = function () {
        registerEvents();
        loadData(true);
    };

    function registerEvents() {
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#ddlAction").on('change', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) loadData(true);
        });

        $('#btn-search').on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btn-export').on('click', function () {
            exportExcel();
        });

        $('#tbl-content').on('click', '.btn-detail', function () {
            const oldVal = $(this).data('old');
            const newVal = $(this).data('new');
            const timestamp = $(this).data('time');

            $('#old-values').html(formatDiff(oldVal, newVal, 'old'));
            $('#new-values').html(formatDiff(oldVal, newVal, 'new'));
            $('#log-timestamp').text(timestamp);
            $('#modal-detail').modal('show');
        });
    }

    function loadData(isPageChanged) {
        $.ajax({
            url: '/AuditLogs/GetAuditLogsPaging',
            type: 'GET',
            data: {
                Keyword: $('#txtKeyword').val(),
                ActionType: $('#ddlAction').val(),
                PageNumber: base.configs.pageIndex,
                PageSize: base.configs.pageSize,
                OrderBy: 'ActionTimestamp desc'
            },
            success: function (response) {
                const template = $('#table-template').html();
                let render = "";
                if (response.totalCount > 0) {
                    $.each(response.data, function (i, item) {
                        render += Mustache.render(template, {
                            UserName: item.userName,
                            TableName: item.tableName,
                            ActionType: item.actionType,
                            IpAddress: item.ipAddress,
                            ActionTimestamp: base.formatDateTime(item.actionTimestamp),
                            OldValues: encodeURIComponent(JSON.stringify(item.oldValues)),
                            NewValues: encodeURIComponent(JSON.stringify(item.newValues))
                        });
                    });
                    $('#tbl-content').html(render);
                    base.wrapPaging(response.totalCount, loadData, isPageChanged);
                } else {
                    base.notify(localization?.NoDataFound || 'No audit log data found.', 'info');
                    $('#tbl-content').html('<tr><td colspan="6" class="text-center">No data available</td></tr>');
                }
            }
        });
    }

    function exportExcel() {
        const keyword = $('#txtKeyword').val();
        const action = $('#ddlAction').val();
        const url = `/AuditLogs/ExportExcel?Keyword=${encodeURIComponent(keyword)}&ActionType=${action}`;
        window.location.href = url;
    }

    function formatDiff(oldVal, newVal, type) {
        try {
            const oldObj = JSON.parse(decodeURIComponent(oldVal));
            const newObj = JSON.parse(decodeURIComponent(newVal));

            const target = type === 'old' ? oldObj : newObj;
            const comparison = type === 'old' ? newObj : oldObj;

            let html = '';
            for (let key in target) {
                const original = comparison[key];
                const current = target[key];
                if (original !== current) {
                    html += `<div class="p-1" style="background-color: ${type === 'old' ? '#f8d7da' : '#d4edda'}"><strong>${key}</strong>: ${current}</div>`;
                } else {
                    html += `<div class="p-1">${key}: ${current}</div>`;
                }
            }
            return html || '-';
        } catch {
            return '-';
        }
    }
};
