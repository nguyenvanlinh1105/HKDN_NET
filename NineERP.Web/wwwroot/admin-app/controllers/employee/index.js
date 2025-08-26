var EmployeeIndex = function () {
  this.initialize = function () {
    restoreSearchParams();
    base.configs.pageIndex = 1;
    loadData();
    loadEmployeeStatistics();
    registerEvents();
  };

  function registerEvents() {
    $("#ddl-show-page").on("change", function () {
      base.configs.pageSize = $(this).val();
      base.configs.pageIndex = 1;
      storeSearchParams();
      loadData(true);
    });

    $('[data-toggle="tooltip"]').tooltip();

    $(document).on("click", ".btn-edit", function () {
      sessionStorage.setItem("employee_referrer", "list");
    });

    $("#txtKeyword").on("keypress", function (e) {
      if (e.which === 13) {
        base.configs.pageIndex = 1;
        storeSearchParams();
        loadData(true);
      }
    });

    $("#btn-search").on("click", function () {
      base.configs.pageIndex = 1;
      storeSearchParams();
      loadData(true);
    });

    $("body").on("click", ".btn-delete", function (e) {
      e.preventDefault();
      const id = $(this).data("id");

      // ✅ Nếu chưa có id
      if (!id) {
        base.notify(
          window.localization?.NoEmployeeSelected || "No employee selected.",
          "warning"
        );
        return;
      }

      Swal.fire({
        title: window.localization?.ConfirmDeleteTitle || "Delete employee?",
        text:
          window.localization?.ConfirmDeleteText ||
          "Are you sure you want to delete this employee?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: window.localization?.Yes || "Yes",
        cancelButtonText: window.localization?.No || "No",
      }).then((result) => {
        if (result.value) {
          $(this).attr("disabled", true);
          $.post("/employees/Delete", { id }, function (response) {
            base.notify(
              response.messages[0],
              response.succeeded ? "success" : "error"
            );
            if (response.succeeded) loadData(true);
            $(".btn-delete").removeAttr("disabled");
          }).fail(function () {
            base.notify(
              window.localization?.DeleteFailed || "Delete failed.",
              "error"
            );
            $(".btn-delete").removeAttr("disabled");
          });
        }
      });
    });

    $("#btn-delete-multiple").on("click", function (e) {
      e.preventDefault();

      const ids = [];
      $("input[name='selectItem']:checked").each(function () {
        ids.push($(this).val());
      });

      if (ids.length === 0) {
        base.notify(
          window.localization?.NoEmployeeSelected || "No employee selected.",
          "warning"
        );
        return;
      }

      Swal.fire({
        title: window.localization?.ConfirmDeleteTitle || "Confirm Delete",
        text:
          window.localization?.ConfirmDeleteText ||
          "Are you sure you want to delete the selected employees?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: window.localization?.Yes || "Yes",
        cancelButtonText: window.localization?.No || "No",
      }).then((result) => {
        if (result.value) {
          $.ajax({
            url: "/employees/DeleteMultiple",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(ids),
            success: function (response) {
              base.notify(
                response.messages[0],
                response.succeeded ? "success" : "error"
              );
              if (response.succeeded) loadData(true);
            },
            error: function () {
              base.notify(
                window.localization?.DeleteFailed || "Delete failed.",
                "error"
              );
            },
          });
        }
      });
    });

    // Load more trên mobile
    $('#btn-load-more').on('click', function () {
      base.configs.pageIndex++;
      loadData(true);
    });
  }

  function getStatusClassAndText(status) {
    if (!status)
      return {
        StatusClass:
          "btn btn-sm btn-outline-secondary waves-effect waves-light",
        StatusText: "-",
      };

    const key = status.trim().toLowerCase();
    const classMap = {
      "đang làm": "btn-outline-success",
      "chính thức": "btn-outline-success",
      "thử việc": "btn-outline-warning",
      "thực tập": "btn-outline-info",
      "bán thời gian": "btn-outline-primary",
      "hợp đồng": "btn-outline-dark",
      "theo dự án": "btn-outline-purple",
      "đã nghỉ hưu": "btn-outline-danger",
      active: "btn-outline-success",
      official: "btn-outline-success",
      probation: "btn-outline-warning",
      internship: "btn-outline-info",
      "part-time": "btn-outline-primary",
      contractor: "btn-outline-dark",
      "project-based": "btn-outline-purple",
      retired: "btn-outline-danger",
    };

    const css = classMap[key] || "btn-outline-secondary";
    return {
      StatusClass: `btn btn-sm ${css} waves-effect waves-light`,
      StatusText: status,
    };
  }

  // Load data for the employee list
  function loadData(append = false) {
    $.get("/employees/GetAllPaging", {
      keyword: $('#txtKeyword').val(),
      pageSize: base.configs.pageSize,
      pageNumber: base.configs.pageIndex
    }, function (response) {
      const isMobile = window.innerWidth < 768;
      if (response.totalCount > 0) {
        if (!isMobile) {
          // --- desktop: reset luôn nội dung bảng ---
          let html = renderTable(response.data);
          $('#tbl-content').html(html);
          $('#tbl-view').show();
          $('#card-view').hide();

          // vẽ phân trang cũ
          base.wrapPaging(response.totalCount, loadData, false);
        } else {
          // --- mobile: nếu append thì thêm vào, còn không thì reset ---
          let html = renderCards(response.data);
          if (append) {
            $('#card-view').append(html);
          } else {
            $('#card-view').html(html);
          }
          $('#card-view').show();
          $('#tbl-view').hide();

          // ẩn phân trang cũ, không dùng nữa
          $('.pagination-wrapper').hide();

          // Hiện hoặc ẩn nút Load more
          const loadedSoFar = base.configs.pageIndex * base.configs.pageSize;
          if (loadedSoFar < response.totalCount) {
            $('#load-more-wrapper').show();
          } else {
            $('#load-more-wrapper').hide();
          }
        }
      } else {
        // Không có record
        $('#tbl-content').html('<tr><td colspan="9" class="text-center text-muted">No records found.</td></tr>');
        $('#card-view').html('<div class="col-12 text-center text-muted">No records found.</div>');
        $('#tbl-view').toggle(!isMobile);
        $('#card-view').toggle(isMobile);
        $('#load-more-wrapper').hide();
      }
    });
  }

function renderTable(data) {
  let html = '';
  let no = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
  data.forEach((item, i) => {
    const status = getStatusClassAndText(item.status);
    const dropup = (i === data.length - 1) ? 'dropup' : '';
    html += Mustache.render($('#table-template').html(), {
      DisplayOrder: no++,
      Id: item.id,
      EmployeeNo: item.employeeNo,
      FullName: item.fullName,
      AvatarUrl: item.avatarUrl
        ? `${base.getOrigin()}${item.avatarUrl}`
        : '/assets/images/users/no-avatar.png',
      PositionName: item.positionName || '-',
      DepartmentName: item.departmentName || '-',
      ContractTypeName: item.contractTypeName || '-',
      StatusClass: status.StatusClass,
      StatusText: status.StatusText,
      ActionDropdown: `
        <div class="dropdown ${dropup}">
          <a class="text-muted dropdown-toggle" href="#" role="button" data-toggle="dropdown">
            <i class="mdi mdi-settings-outline"></i>
          </a>
          <div class="dropdown-menu">
            <a class="dropdown-item btn-edit" href="/employees/edit/${item.id}">${window.localization?.Edit || 'Edit'}</a>
            <a class="dropdown-item text-danger btn-delete" href="#" data-id="${item.id}">${window.localization?.Delete || 'Delete'}</a>
          </div>
        </div>`
    });
  });
  return html;
}

  function renderCards(data) {
    let html = '';
    data.forEach(item => {
      const status = getStatusClassAndText(item.status);
      html += Mustache.render($('#card-template').html(), {
        Id: item.id,
        EmployeeNo: item.employeeNo,
        FullName: item.fullName,
        AvatarUrl: item.avatarUrl ? `${base.getOrigin()}${item.avatarUrl}` : '/assets/images/users/no-avatar.png',
        PositionName: item.positionName||'-',
        DepartmentName: item.departmentName||'-',
        ContractTypeName: item.contractTypeName||'-',
        StatusClass: status.StatusClass,
        StatusText: status.StatusText
      });
    });
    return html;
  }
  

  function storeSearchParams() {
    const params = {
      keyword: $("#txtKeyword").val(),
      page: base.configs.pageIndex,
      pageSize: base.configs.pageSize,
    };
    sessionStorage.setItem("employeeSearchParams", JSON.stringify(params));
  }
  function restoreSearchParams() {
    const stored = sessionStorage.getItem("employeeSearchParams");
    if (stored) {
      const params = JSON.parse(stored);
      $("#txtKeyword").val(params.keyword || "");
      base.configs.pageIndex = parseInt(params.page) || 1;
      base.configs.pageSize =
        parseInt(params.pageSize) || base.configs.pageSize;
      $("#ddl-show-page").val(base.configs.pageSize);
    }
  }

  function loadEmployeeStatistics() {
    $.get("/employees/statistics", function (res) {
      $("#total-employees").text(res.data.total);
      $("#official-employees").text(res.data.official);
      $("#probation-employees").text(res.data.probation);
      $("#intern-employees").text(res.data.intern);
    }).fail(function () {
      const msg =
        window.localization?.LoadStatisticsFailed ||
        "Failed to load employee statistics.";
      base.notify(msg, "error");
    });
  }
};
