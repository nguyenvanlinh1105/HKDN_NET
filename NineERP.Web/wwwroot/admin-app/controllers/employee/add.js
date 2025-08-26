// File: /admin-app/controllers/employee/add.js

var EmployeeAdd = function () {
  this.initialize = function () {
    registerEvents();
  };

  function registerEvents() {
    // Khi mở modal thì reset form + init select2 + init datepicker
    $("#modalAddEmployee").on("show.bs.modal", function () {
      $("#form-add-employee")[0].reset();
      $("#txtEmployeeNo").val("Auto Generate").prop("readonly", true);
      loadNextEmployeeNo(false);
      $("#form-add-employee").find(".is-invalid").removeClass("is-invalid");
      $("#form-add-employee").find(".invalid-feedback").remove();

      initSelect2();
      initFlatpickr();
      setDefaultWorkingDateFrom();
    });

    $("#modalAddEmployee").on("shown.bs.modal", function () {
      Promise.all([
        loadDepartmentsOptions(),
        loadPositionOptions(),
        loadContractTypeOptions(),
        loadKbnEmployeeStatusOptions(),
      ]).then(() => {
        setTimeout(() => {
          setDefaultSelectValues();
        }, 200); // Cho flatpickr có thời gian init
      });
    });

    // Thu gọn card khi click tiêu đề
    $("body").on("click", ".toggle-card", function () {
      const target = $($(this).data("target"));
      const icon = $(this).find("i").last();
      target.stop(true, true).slideToggle(250);
      icon.toggleClass("mdi-chevron-up mdi-chevron-down");
    });

    // Khi chọn loại hợp đồng, tự động lấy số hợp đồng tiếp theo
    $("body").on("change", '[name="ContractTypeId"]', function () {
      const id = parseInt($(this).val());
      const contractType = getContractTypeGroupCode(id);
      loadNextContractNumber(contractType);
    });

    // Bấm vào nút Edit Employee No
    $("body").on("click", "#btn-edit-employee-no", function () {
      const $btn = $(this);
      const $icon = $btn.find("i");
      const $input = $("#txtEmployeeNoNumber");

      if ($icon.hasClass("mdi-pencil")) {
        // Bắt đầu sửa
        $input.prop("readonly", false)[0].focus();
        $btn
          .removeClass("btn-success btn-danger")
          .addClass("btn-outline-secondary");
        $icon.removeClass("mdi-pencil").addClass("mdi-check");
        $btn.attr("title", window.localization?.Check || "Check");
      } else {
        const newNumber = $input.val().trim();
        const isValid = /^\d{4}$/.test(newNumber);

        if (!isValid) {
          $btn
            .removeClass("btn-outline-secondary btn-success")
            .addClass("btn-danger");
          $btn.attr("title", window.localization?.InvalidFourDigits || "Invalid - must be 4 digits");
          return;
        }

        checkEmployeeNoExists(newNumber, function (exists) {
          if (exists) {
            $btn
              .removeClass("btn-outline-secondary btn-success")
              .addClass("btn-danger");
             $btn.attr("title", window.localization?.NumberExists || "Number already exists");
          } else {
            // ✅ Nếu hợp lệ
            $input.prop("readonly", true);
            $btn
              .removeClass("btn-outline-secondary btn-danger")
              .addClass("btn-success");
            $btn.attr("title", window.localization?.Valid || "Valid");

            setTimeout(() => {
              $btn.removeClass("btn-success").addClass("btn-outline-secondary");
              $icon.removeClass("mdi-check").addClass("mdi-pencil");
              $btn.attr("title", window.localization?.Edit || "Edit");
            }, 2000);
          }
        });
      }
    });

    // Bấm vào nút Edit Contract No
    $("body").on("click", "#btn-edit-contract-no", function () {
      const $btn = $(this);
      const $icon = $btn.find("i");
      const $input = $("#txtContractNoNumber");

      if ($icon.hasClass("mdi-pencil")) {
        // Bắt đầu sửa
        $input.prop("readonly", false)[0].focus();
        $btn
          .removeClass("btn-success btn-danger")
          .addClass("btn-outline-secondary");
        $icon.removeClass("mdi-pencil").addClass("mdi-check");
        $btn.attr("title", window.localization?.Check || "Check");
      } else {
        const newNumber = $input.val().trim();
        const isValid = /^\d{4}$/.test(newNumber);

        if (!isValid) {
          $btn
            .removeClass("btn-outline-secondary btn-success")
            .addClass("btn-danger");
          $btn.attr("title", window.localization?.InvalidFourDigits || "Invalid - must be 4 digits");
          return;
        }

        checkContractNoExists(newNumber, function (exists) {
          if (exists) {
            $btn
              .removeClass("btn-outline-secondary btn-success")
              .addClass("btn-danger");
            $btn.attr("title", window.localization?.NumberExists || "Number already exists");
          } else {
            // ✅ Nếu hợp lệ
            $input.prop("readonly", true);
            $btn
              .removeClass("btn-outline-secondary btn-danger")
              .addClass("btn-success");
            $btn.attr("title", window.localization?.Valid || "Valid");

            setTimeout(() => {
              $btn.removeClass("btn-success").addClass("btn-outline-secondary");
              $icon.removeClass("mdi-check").addClass("mdi-pencil");
              $btn.attr("title", window.localization?.Edit || "Edit");
            }, 2000);
          }
        });
      }
    });

    // Show/hide Contract section theo trạng thái nhân viên
    $("body").on("change", "#EmployeeStatusId", function () {
      const selected = parseInt($(this).val());
      if (selected === 1 || selected === 2) {
        $("#contractSectionWrapper").slideDown(250);
      } else {
        $("#contractSectionWrapper").slideUp(250);
      }
    });

    $("body").on("submit", "#form-add-employee", function (e) {
      e.preventDefault();
      if (!validateForm()) return;

      const $btnSave = $(this).find('button[type="submit"]');
      const originalText = $btnSave.html();
      $btnSave
        .prop("disabled", true)
        .html(`<span class="spinner-border spinner-border-sm" role="status"></span> ${window.localization?.Saving || "Saving..."}`);

      const getDate = (selector) => {
        const val = $(selector).val();
        const d = moment(val, ["DD/MM/YYYY", "MM/DD/YYYY"]);
        return d.isValid() ? d.format("YYYY-MM-DD") : null;
      };

      const employee = {
        EmployeeNo:
          $("#employeePrefix").text() + $("#txtEmployeeNoNumber").val(),
        FullName: $('input[name="FullName"]').val(),
        Email: $('input[name="Email"]').val(),
        PhoneNo: $('input[name="PhoneNo"]').val(),
        Birthday: getDate('input[name="Birthday"]'),
        Gender: parseInt($('input[name="Gender"]:checked').val()),

        Address: $('textarea[name="Address"]').val(),
        PlaceOfBirth: $('textarea[name="PlaceOfBirth"]').val(),

        DepartmentId: parseInt($('select[name="DepartmentId"]').val()) || null,
        PositionId: parseInt($('select[name="PositionId"]').val()) || null,
        EmployeeStatusId:
          parseInt($('select[name="EmployeeStatusId"]').val()) || null,
        WorkingDateFrom: getDate('input[name="WorkingDateFrom"]'),

        ContractNumber:
          $("#contractPrefix").text() + $("#txtContractNoNumber").val(),
        ContractTypeId:
          parseInt($('select[name="ContractTypeId"]').val()) || null,
        ContractFrom: getDate('input[name="ContractFrom"]'),
        ContractTo: getDate('input[name="ContractTo"]'),
      };

      const payload = {
        employee: employee,
        origin: base.getOrigin(),
      };


      $.ajax({
        type: "POST",
        url: "/employees/create",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function (response) {
          if (response.succeeded) {
            base.notify(response.messages[0], "success");
            $("#modalAddEmployee").modal("hide");
            loadData(true);
          } else {
            base.notify(response.messages[0], "error");
          }
        },
        error: function () {
            base.notify(window.localization?.SomethingWentWrong || "Something went wrong.", "error");
        },
        complete: function () {
          $btnSave.prop("disabled", false).html(originalText);
        },
      });
    });
  }

  function initSelect2() {
    $("#modalAddEmployee select").each(function () {
      $(this).select2({
        dropdownParent: $("#modalAddEmployee .modal-content"),
        width: "100%",
        minimumResultsForSearch:
          $(this).find("option").length > 10 ? 5 : Infinity,
      });
    });
  }

  function initFlatpickr() {
    const lang = getLang();

    $(".flatpickr-date").each(function () {
      flatpickr(this, {
        dateFormat: lang === "vi" ? "d/m/Y" : "m/d/Y",
        locale: lang === "vi" ? VietnameseFlatpickrLocale : "en",
        allowInput: true,
      });
    });
  }

  function loadDepartmentsOptions() {
    return $.get("/Departments/GetAll").then(function (res) {
      const $select = $('[name="DepartmentId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            window.localization?.SelectOption || "Select Option"
          }</option>`
        );

      const lang = getLang();
      res.data.forEach((item) => {
        let name =
          lang === "vi"
            ? item.nameVi
            : lang === "ja"
            ? item.nameJa || item.nameEn
            : item.nameEn;
        $select.append(`<option value="${item.id}">${name}</option>`);
      });

      // Gợi ý: gọi lại .trigger('change') nếu bạn muốn áp dụng sau khi append
      $select.trigger("change");
    });
  }

  function loadPositionOptions() {
    return $.get("/Positions/GetAll").then(function (res) {
      const $select = $('[name="PositionId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            window.localization?.SelectOption || "Select Option"
          }</option>`
        );

      const lang = getLang();
      res.data.forEach((item) => {
        let name =
          lang === "vi"
            ? item.nameVi || item.nameEn
            : lang === "ja"
            ? item.nameJp || item.nameEn
            : item.nameEn;
        $select.append(`<option value="${item.id}">${name}</option>`);
      });

      $select.trigger("change");
    });
  }

  function loadContractTypeOptions() {
    return $.get("/KbnContractTypes/GetAll").then(function (res) {
      const $select = $('[name="ContractTypeId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            window.localization?.SelectOption || "Select Option"
          }</option>`
        );

      const lang = getLang();
      res.data.forEach((item) => {
        let name =
          lang === "vi"
            ? item.nameVi || item.nameEn
            : lang === "ja"
            ? item.nameJp || item.nameEn
            : item.nameEn;
        $select.append(`<option value="${item.id}">${name}</option>`);
      });

      $select.trigger("change");
    });
  }

  function loadKbnEmployeeStatusOptions() {
    return $.get("/KbnEmployeeStatus/GetAll").then(function (res) {
      const $select = $('[name="EmployeeStatusId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            window.localization?.SelectOption || "Select Option"
          }</option>`
        );

      const lang = getLang();
      res.data.forEach((item) => {
        let name =
          lang === "vi"
            ? item.nameVi || item.nameEn
            : lang === "ja"
            ? item.nameJp || item.nameEn
            : item.nameEn;
        $select.append(`<option value="${item.id}">${name}</option>`);
      });

      $select.trigger("change");
    });
  }

  function getLang() {
    const langAttr = $("html").attr("lang") || "en-US";
    if (langAttr.startsWith("vi")) return "vi";
    if (langAttr.startsWith("ja")) return "ja";
    return "en";
  }

    function validateForm() {
        let valid = true;
        const $form = $("#form-add-employee");

        $form.find(".is-invalid").removeClass("is-invalid");
        $form.find(".invalid-feedback").remove();

        const fullName = $form.find('[name="FullName"]').val().trim();
        if (!fullName) {
            showError('[name="FullName"]', window.localization?.FullNameRequired || "Full name is required.");
            valid = false;
        }

        const email = $form.find('[name="Email"]').val().trim();
        if (!email || !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
            showError('[name="Email"]', window.localization?.EmailRequired || "A valid email is required.");
            valid = false;
        }

        const phone = $form.find('[name="PhoneNo"]').val().trim();
        if (phone && !/^\d{8,15}$/.test(phone)) {
            showError('[name="PhoneNo"]', window.localization?.PhoneInvalid || "Phone number must be 8-15 digits.");
            valid = false;
        }

        const from = $form.find('[name="ContractFrom"]').val();
        const to = $form.find('[name="ContractTo"]').val();
        if (from && to) {
            const dFrom = moment(from, ["DD/MM/YYYY", "MM/DD/YYYY"], true);
            const dTo = moment(to, ["DD/MM/YYYY", "MM/DD/YYYY"], true);
            if (dFrom.isValid() && dTo.isValid() && dFrom.isAfter(dTo)) {
                showError('[name="ContractTo"]', window.localization?.ContractToInvalid || "Contract To must be after Contract From.");
                valid = false;
            }
        }

        return valid;
    }

    function showError(selector, message) {
        const $input = $(selector);
        $input.addClass("is-invalid");
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }


  function loadNextEmployeeNo(isIntern) {
    $.get(
      "/Employees/GetNextEmployeeNo",
      { isIntern: isIntern },
      function (res) {
        if (res.succeeded && res.data) {
          $("#employeePrefix").text(res.data.prefix);
          $("#txtEmployeeNoNumber").val(res.data.number);
        }
      }
    );
  }

  function loadNextContractNumber(contractType) {
    if (!contractType) return;

    $.get(
      "/Employees/GetNextContractNumber",
      { contractType: contractType },
      function (res) {
        if (res.succeeded && res.data) {
          $("#contractPrefix").text(res.data.prefix);
          $("#txtContractNoNumber").val(res.data.number);
          setContractDates(contractType);
        }
      }
    );
  }

  function getContractTypeGroupCode(id) {
    switch (id) {
      case 1:
      case 2:
      case 3:
        return "CONTRACT_OFFICIAL_PREFIX";
      case 4:
        return "CONTRACT_PROBATION_PREFIX";
      case 5:
        return "CONTRACT_INTERNSHIP_PREFIX";
      case 6:
      case 7:
      case 8:
      case 9:
      case 10:
        return "CONTRACT_FLEXIBLE_PREFIX";
      default:
        return "";
    }
  }

    function checkEmployeeNoExists(employeeNo, callback) {
        if (!employeeNo) return callback(false);

        $.get(
            "/Employees/CheckEmployeeNoExists",
            { number: employeeNo },
            function (res) {
                if (res.succeeded) {
                    callback(res.data.exists);
                } else {
                    console.warn(
                        `${window.localization?.CheckEmployeeNoFailed || "Check employee number failed"}: ${res.messages?.[0]}`
                    );
                    callback(false);
                }
            }
        ).fail(function () {
            console.error(
                window.localization?.CheckEmployeeNoRequestFailed || "Failed to check employee number."
            );
            callback(false);
        });
    }
    function checkContractNoExists(contractNo, callback) {
        if (!contractNo) return callback(false);

        $.get(
            "/Employees/CheckContractNoExists",
            { number: contractNo },
            function (res) {
                if (res.succeeded) {
                    callback(res.data.exists);
                } else {
                    console.warn(
                        `${window.localization?.CheckContractNoFailed || "Check contract number failed"}: ${res.messages?.[0]}`
                    );
                    callback(false);
                }
            }
        ).fail(function () {
            console.error(
                window.localization?.CheckContractNoRequestFailed || "Failed to check contract number."
            );
            callback(false);
        });
    }
  function setDefaultSelectValues() {
    const $department = $('[name="DepartmentId"]');
    const $position = $('[name="PositionId"]');
    const $contractType = $('[name="ContractTypeId"]');
    const $status = $('[name="EmployeeStatusId"]');

    // Nếu có ít nhất 2 option (Select Option + 1 item), chọn item đầu tiên sau option mặc định
    if ($department.find("option").length > 1)
      $department.val($department.find("option:eq(3)").val()).trigger("change");
    if ($status.find("option").length > 1)
      $status.val($status.find("option:eq(1)").val()).trigger("change");
    if ($position.find("option").length > 1)
      $position.val($position.find("option:eq(5)").val()).trigger("change");
    if ($contractType.find("option").length > 1) {
      $contractType
        .val($contractType.find("option:eq(4)").val())
        .trigger("change");
    }
  }

    function setContractDates(contractType) {
        const today = moment();
        let toDate;

        switch (contractType) {
            case "CONTRACT_OFFICIAL_PREFIX":
                toDate = today.clone().add(12, "months"); // Mặc định 1 năm
                break;
            case "CONTRACT_PROBATION_PREFIX":
                toDate = today.clone().add(2, "months");
                break;
            case "CONTRACT_INTERNSHIP_PREFIX":
                toDate = today.clone().add(3, "months");
                break;
            case "CONTRACT_FLEXIBLE_PREFIX":
                toDate = today.clone().add(3, "months"); // trung bình
                break;
            case "CONTRACT_PROJECT_PREFIX":
                toDate = today.clone().add(2, "months"); // Project-based
                break;
            default:
                toDate = today.clone().add(1, "months");
        }

        const dateFormat = getLang() === "vi" ? "DD/MM/YYYY" : "MM/DD/YYYY";
        $('[name="ContractFrom"]').val(today.format(dateFormat));
        $('[name="ContractTo"]').val(toDate.format(dateFormat));
    }

  function setDefaultWorkingDateFrom() {
    const input = $('[name="WorkingDateFrom"]').get(0);
    const today = moment();
    const dateFormat = getLang() === "vi" ? "DD/MM/YYYY" : "MM/DD/YYYY";

    if (input && input._flatpickr) {
      input._flatpickr.setDate(today.toDate(), true);
    } else {
      $('[name="WorkingDateFrom"]').val(today.format(dateFormat));
    }
  }
};

// Locale Vietnam cho flatpickr
var VietnameseFlatpickrLocale = {
  weekdays: {
    shorthand: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
    longhand: [
      "Chủ Nhật",
      "Thứ Hai",
      "Thứ Ba",
      "Thứ Tư",
      "Thứ Năm",
      "Thứ Sáu",
      "Thứ Bảy",
    ],
  },
  months: {
    shorthand: [
      "Th1",
      "Th2",
      "Th3",
      "Th4",
      "Th5",
      "Th6",
      "Th7",
      "Th8",
      "Th9",
      "Th10",
      "Th11",
      "Th12",
    ],
    longhand: [
      "Tháng 1",
      "Tháng 2",
      "Tháng 3",
      "Tháng 4",
      "Tháng 5",
      "Tháng 6",
      "Tháng 7",
      "Tháng 8",
      "Tháng 9",
      "Tháng 10",
      "Tháng 11",
      "Tháng 12",
    ],
  },
  firstDayOfWeek: 1,
  rangeSeparator: " đến ",
  weekAbbreviation: "Tuần",
  scrollTitle: "Cuộn để thay đổi",
  toggleTitle: "Nhấn để chuyển đổi",
  time_24hr: true,
};
