// File: /admin-app/controllers/employee/edit.js

var EmployeeEdit = function () {
  this.initialize = function () {
    registerEvents();
    loadDropdowns();
    initFlatpickr();
  };

  function registerEvents() {
    $("#btn-save-all, button[type='submit']").on("click", function (e) {
      e.preventDefault();
      saveAll();
    });
      $("#btn-back-employee").on("click", function () {
          const ref = sessionStorage.getItem("employee_referrer");

          if (ref === "detail") {
              const id = window.employeeModel.Id || null; // hoặc gắn thẻ data-employee-id
              window.location.href = `/employees/detail/${id}`;
          } else {
              window.location.href = "/employees";
          }

          sessionStorage.removeItem("employee_referrer");
      });

      $('input[name="SalaryBasic"]').on('input', function () {
          const value = $(this).val().replace(/[^0-9]/g, '');
          const formatted = new Intl.NumberFormat().format(value);
          $(this).val(formatted);
      });
      $('input[name="SalaryGross"]').on('input', function () {
          const value = $(this).val().replace(/[^0-9]/g, '');
          const formatted = new Intl.NumberFormat().format(value);
          $(this).val(formatted);
      });
    // Upload Avatar
    $("#imageUpload").on("change", function (e) {
      const file = e.target.files[0];
      if (file) {
        uploadImage(file, "avatar", function (url) {
          // ✅ Gọi đúng URL được trả về (đã là link nội bộ sẵn)
          const forceReloadUrl = url + "?t=" + new Date().getTime(); // prevent cache
          $("#avatarPreview").attr("src", forceReloadUrl);
        });
      }
    });

    // ✅ Helper function để tách fileId
    function extractDriveFileId(url) {
      const match = url.match(/(?:id=|\/d\/)([a-zA-Z0-9_-]{25,})/);
      return match ? match[1] : "";
    }

    // Upload CCCD front
    $("#fileFrontSide").on("change", function (e) {
      const file = e.target.files[0];
      if (file) {
        uploadImage(file, "id-front", function (url) {
          const fileId = extractDriveFileId(url);
          const finalUrl = "https://drive.google.com/thumbnail?id=" + fileId;
          const forceReloadUrl = finalUrl + "&t=" + new Date().getTime();
          $("#imgIdFrontPreview").attr("src", forceReloadUrl);
        });
      }
    });

    // Upload CCCD back
    $("#fileBackSide").on("change", function (e) {
      const file = e.target.files[0];
      if (file) {
        uploadImage(file, "id-back", function (url) {
          const fileId = extractDriveFileId(url);
          const finalUrl = "https://drive.google.com/thumbnail?id=" + fileId;
          const forceReloadUrl = finalUrl + "&t=" + new Date().getTime();
          $("#imgIdBackPreview").attr("src", forceReloadUrl);
        });
      }
    });

    document
      .getElementById("btn-upload-document")
      ?.addEventListener("click", function () {
        document.getElementById("fileDocument")?.click();
      });

    $("#fileDocument").on("change", function () {
      const file = this.files[0];
      if (!file) return;
      const allowedExtensions = [
        "pdf",
        "doc",
        "docx",
        "ppt",
        "pptx",
        "xls",
        "xlsx",
        "jpg",
        "jpeg",
        "png",
        "gif",
        "webp",
      ];
      const fileExtension = file.name.split(".").pop().toLowerCase();

      if (!allowedExtensions.includes(fileExtension)) {
        base.notify(
          window.localization?.UnsupportedFileFormat ||
            "Unsupported file format",
          "error"
        );
        $(this).val(""); // clear input
        return;
      }

      const formData = new FormData();
      formData.append("file", file);
      formData.append("employeeNo", $(this).data("employee"));

      $("#uploadProgressWrapper").removeClass("d-none");
      $("#uploadProgressBar")
        .css("width", "0%")
        .attr("aria-valuenow", 0)
        .text("0%");

      $.ajax({
        url: "/employees/upload-document",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        xhr: function () {
          const xhr = new window.XMLHttpRequest();
          xhr.upload.addEventListener(
            "progress",
            function (evt) {
              if (evt.lengthComputable) {
                const percent = Math.round((evt.loaded / evt.total) * 100);
                $("#uploadProgressBar")
                  .css("width", percent + "%")
                  .attr("aria-valuenow", percent)
                  .text(percent + "%");
              }
            },
            false
          );
          return xhr;
        },
        success: function (res) {
          if (res.succeeded) {
            base.notify(
              window.localization?.UploadSuccessful || "Upload successful",
              "success"
            );
            reloadDocumentList(); // ✅ load lại list document sau khi upload
          } else {
            base.notify(
              window.localization?.UploadFailed || "Upload failed",
              "error"
            );
          }
          $("#uploadProgressWrapper").addClass("d-none");
        },
        error: function () {
          base.notify(
            window.localization?.UploadFailed || "Upload failed",
            "error"
          );
          $("#uploadProgressWrapper").addClass("d-none");
        },
      });
    });

    $(document).off("click", ".btn-preview-document");
    $(document).on("click", ".btn-preview-document", function () {
      const fileId = $(this).data("fileid");
      if (!fileId) return;

      const extension = $(this).data("ext")?.toLowerCase();
      const previewUrl = `/preview/file?id=${fileId}`;

      $("#previewModal").modal("show");
      $("#previewFrame")
        .addClass("d-none")
        .attr("src", "about:blank")
        .removeAttr("srcdoc");
      $("#pdfCanvasContainer").addClass("d-none").empty();

      if (extension === "pdf") {
        const container = $("#pdfCanvasContainer");
        container.html(""); // xoá nội dung cũ
        $("#previewLoading").show();

        const loadingTask = pdfjsLib.getDocument(previewUrl);
        loadingTask.promise
          .then(function (pdf) {
            const totalPages = pdf.numPages;

            const renderPage = function (pageNumber) {
              return pdf.getPage(pageNumber).then(function (page) {
                const scale = 1.5;
                const viewport = page.getViewport({ scale });

                const canvas = document.createElement("canvas");
                const context = canvas.getContext("2d");
                canvas.width = viewport.width;
                canvas.height = viewport.height;
                canvas.classList.add("mb-3");

                container.append(canvas);

                return page.render({
                  canvasContext: context,
                  viewport,
                }).promise;
              });
            };

            let renderChain = Promise.resolve();
            for (let i = 1; i <= totalPages; i++) {
              renderChain = renderChain.then(() => renderPage(i));
            }

            return renderChain;
          })
          .then(() => {
            $("#pdfCanvasContainer").removeClass("d-none");
            $("#previewLoading").fadeOut(300);
          })
          .catch(() => {
            $("#previewLoading").fadeOut(200);
            alert("Failed to load PDF.");
          });
      } else if (
        ["doc", "docx", "ppt", "pptx", "xls", "xlsx"].includes(extension)
      ) {
        const officeViewer = `https://view.officeapps.live.com/op/embed.aspx?src=${encodeURIComponent(
          location.origin + previewUrl
        )}`;
        $("#previewFrame").removeClass("d-none").attr("src", officeViewer);
      } else if (["jpg", "jpeg", "png", "gif", "webp"].includes(extension)) {
        const imgHtml = `<html><body style="margin:0;text-align:center">
            <img src="${previewUrl}" class="img-fluid d-block mx-auto" style="max-width:100%;height:auto;" />
        </body></html>`;

        $("#previewFrame").removeClass("d-none").attr("srcdoc", imgHtml);
      } else {
        alert("Unsupported file format.");
      }
    });

    $("#previewModal").on("hidden.bs.modal", function () {
      $("#previewLoading").hide();
      $("#previewFrame").attr("src", "about:blank").removeAttr("srcdoc");
      $("#pdfCanvasContainer").html("").hide(); // reset PDF preview
    });

    $("body").on("click", ".btn-delete-document", function () {
      const id = $(this).data("id");

      Swal.fire({
        title:
          window.localization?.ConfirmDeleteDocumentTitle || "Delete document?",
        text:
          window.localization?.ConfirmDeleteDocumentText ||
          "Are you sure you want to delete this document?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: window.localization?.Yes || "Yes",
        cancelButtonText: window.localization?.No || "No",
      }).then(function (result) {
        if (result === true || result.value === true) {
          $.post("/employees/delete-document", { id }, function (response) {
            if (response.succeeded) {
              base.notify(
                window.localization?.DeleteSuccessful ||
                  "Deleted successfully.",
                "success"
              );
              reloadDocumentList();
            } else {
              base.notify(
                window.localization?.DeleteFailed || "Delete failed.",
                "error"
              );
            }
          }).fail(function () {
            base.notify(
              window.localization?.DeleteFailed || "Delete failed.",
              "error"
            );
          });
        }
      });
    });
  }
  function reloadDocumentList() {
    if (!employeeId) return;
    const id = employeeId;
    $.get("/Employees/GetById", { id }, function (res) {
      if (res?.data?.documents?.length > 0) {
        const html = res.data.documents
          .map(
            (doc) => `
      <div class="col-md-4 mb-3">
        <div class="border rounded p-2 h-100 position-relative">
          <p class="mb-1"><i class="mdi mdi-file-document-outline"></i> <strong>${doc.nameFile}</strong></p>
          <p class="text-muted small mb-1">${doc.sizeFile} - ${doc.typeFile}</p>
          <button class="btn btn-sm btn-outline-info btn-preview-document" data-fileid="${doc.googleDriveFileId}" data-ext="${doc.typeFile}">
            <i class="mdi mdi-eye"></i>
          </button>
          <a class="btn btn-sm btn-outline-primary" href="/preview/file?id=${doc.googleDriveFileId}" target="_blank" download>
            <i class="mdi mdi-download"></i>
          </a>
          <button class="btn btn-sm btn-outline-danger btn-delete-document" data-id="${doc.id}">
            <i class="mdi mdi-delete"></i>
          </button>
        </div>
      </div>
    `
          )
          .join("");

        $("#document-list").html(html);
      } else {
        $("#document-list").html(
          `<p class="text-muted font-italic">${
            window.localization?.NoDocumentsFound || "No documents found."
          }</p>`
        );
      }
    });
  }
  function uploadImage(file, type, onSuccess) {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("employeeNo", window.employeeModel.EmployeeNo);

    let uploadUrl = "/employees/upload-avatar";
    if (type === "id-front" || type === "id-back") {
      uploadUrl = "/employees/upload-identity";
      formData.append("side", type === "id-front" ? "front" : "back");
    }

    // Hiện overlay và reset progress
    $("#uploadOverlay").show();

    $.ajax({
      url: uploadUrl,
      type: "POST",
      data: formData,
      contentType: false,
      processData: false,
      success: function (res) {
        if (res.succeeded && res.data && onSuccess) {
          onSuccess(res.data.fileUrl); // ✅ render ảnh preview
          base.notify(
            localization.UpdateSuccess || "Upload thành công",
            "success"
          );
        } else {
          base.notify(res.messages?.[0] || "Upload thất bại", "error");
        }
      },
      error: function () {
        base.notify("Upload failed", "error");
      },
      complete: function () {
        $("#uploadOverlay").hide();
      },
    });
  }

  function loadDropdowns() {
    Promise.all([
      loadDepartmentsOptions(),
      loadPositionOptions(),
      loadContractTypeOptions(),
      loadEmployeeStatusOptions(),
    ]).then(() => {
      setTimeout(() => {
        setDefaultDropdownValues();
      }, 200);
    });

    loadPaymentTypeOptions();
  }

  function loadDepartmentsOptions() {
    return $.get("/Departments/GetAll").then(function (res) {
      const $select = $('[name="DepartmentId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            localization.SelectOption || "Select Option"
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
    });
  }

  function loadPositionOptions() {
    return $.get("/Positions/GetAll").then(function (res) {
      const $select = $('[name="PositionId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            localization.SelectOption || "Select Option"
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
    });
  }

  function loadContractTypeOptions() {
    return $.get("/KbnContractTypes/GetAll").then(function (res) {
      const $select = $('[name="ContractTypeId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            localization.SelectOption || "Select Option"
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
    });
  }

  function loadEmployeeStatusOptions() {
    return $.get("/KbnEmployeeStatus/GetAll").then(function (res) {
      const $select = $('[name="EmployeeStatusId"]');
      $select
        .empty()
        .append(
          `<option value="">${
            localization.SelectOption || "Select Option"
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
    });
  }

  function loadPaymentTypeOptions() {
    const $select = $('[name="PaymentType"]');
    const paymentOptions = [
      { id: 0, text: localization.Cash },
      { id: 1, text: localization.BankTransfer },
    ];
    $select.empty();
    paymentOptions.forEach((opt) => {
      $select.append(`<option value="${opt.id}">${opt.text}</option>`);
    });
  }

  function setDefaultDropdownValues() {
    if (window.employeeModel.DepartmentId !== null) {
      $('[name="DepartmentId"]')
        .val(window.employeeModel.DepartmentId)
        .trigger("change");
    }
    if (window.employeeModel.PositionId !== null) {
      $('[name="PositionId"]')
        .val(window.employeeModel.PositionId)
        .trigger("change");
    }
    if (window.employeeModel.ContractTypeId !== null) {
      $('[name="ContractTypeId"]')
        .val(window.employeeModel.ContractTypeId)
        .trigger("change");
    }
    if (window.employeeModel.EmployeeStatusId !== null) {
      $('[name="EmployeeStatusId"]')
        .val(window.employeeModel.EmployeeStatusId)
        .trigger("change");
    }
    if (window.employeeModel.PaymentType !== undefined) {
      $('[name="PaymentType"]')
        .val(window.employeeModel.PaymentType)
        .trigger("change");
    }
  }

  function getLang() {
    const langAttr = $("html").attr("lang") || "en-US";
    if (langAttr.startsWith("vi")) return "vi";
    if (langAttr.startsWith("ja")) return "ja";
    return "en";
  }

    function initFlatpickr() {
        const lang = getLang(); // ví dụ: "vi" hoặc "en"
        const dateFormat = lang === "vi" ? "d/m/Y" : "Y/m/d"; // dùng chuẩn ISO format hơn là "m/d/Y"

        $(".flatpickr-date").each(function () {
            flatpickr(this, {
                dateFormat: dateFormat,
                locale: lang === "vi" ? VietnameseFlatpickrLocale : "en",
                allowInput: true,
                defaultHour: 0,
                defaultMinute: 0,
                time_24hr: true,
                enableTime: false
            });
        });
    }


    function saveAll() {
        const $btnSave = $("#btn-save-all");
        const originalText = $btnSave.html();
        $btnSave.prop("disabled", true).html('<i class="fa fa-spinner fa-spin"></i>');

        const employee = {
            Id: window.employeeModel.Id || null,
            EmployeeNo: $('input[name="EmployeeNo"]').val(),
            FullName: $('input[name="FullName"]').val(),
            Email: $('input[name="Email"]').val(),
            PhoneNo: $('input[name="PhoneNo"]').val(),
            Birthday: getDate('input[name="Birthday"]'),
            Gender: parseInt($('input[name="Gender"]:checked').val() ?? "0"),
            Address: $('textarea[name="Address"]').val(),
            PlaceOfBirth: $('textarea[name="PlaceOfBirth"]').val(),
            DepartmentId: parseInt($('select[name="DepartmentId"]').val()) || null,
            PositionId: parseInt($('select[name="PositionId"]').val()) || null,
            EmployeeStatusId: parseInt($('select[name="EmployeeStatusId"]').val()) || null,
            MaritalStatus: Number($('select[name="MaritalStatus"]').val()),
            NumberChild: parseInt($('input[name="NumberChild"]').val()) || 0,
            WorkingDateFrom: getDate('input[name="WorkingDateFrom"]'),
            WorkingDateTo: getDate('input[name="WorkingDateTo"]'),
            ContractFrom: getDate('input[name="ContractFrom"]'),
            ContractTo: getDate('input[name="ContractTo"]'),
            ContractTypeId: parseInt($('select[name="ContractTypeId"]').val()) || null,
            ContractNumber: $('input[name="ContractNumber"]').val(), 
            NickName: $('input[name="NickName"]').val(),
            EducationLevel: $('input[name="EducationLevel"]').val(),
            SoftSkill: $('textarea[name="SoftSkill"]').val(),
            BusinessExp: $('textarea[name="BusinessExp"]').val(),
            TaxCode: $('input[name="TaxCode"]').val(),
            SocialInsuranceNumber: $('input[name="SocialInsuranceNumber"]').val(),
            NumberOfDependents: $('input[name="NumberOfDependents"]').val(),
            //IdentityCard: $('input[name="IdentityCard"]').val(),
            //ProvideDateIdentityCard: getDate('input[name="ProvideDateIdentityCard"]'),
            //ProvidePlaceIdentityCard: $('input[name="ProvidePlaceIdentityCard"]').val(),

            SalaryInfo: {
                SalaryBasic: parseFloat($('input[name="SalaryBasic"]').val().replace(/,/g, '')) || null,
                SalaryGross: parseFloat($('input[name="SalaryGross"]').val().replace(/,/g, '')) || null,
                BankName: $('input[name="BankName"]').val(),
                BankNumber: $('input[name="BankNumber"]').val(),
                BankAccountName: $('input[name="BankAccountName"]').val(),
                PaymentType: parseInt($('select[name="PaymentType"]').val()) || 0
            },

            IdentityInfo: {
                CitizenshipCard: $('input[name="CitizenshipCard"]').val(),
                ProvideDateCitizenshipCard: getDate('input[name="ProvideDateCitizenshipCard"]'),
                ProvidePlaceCitizenshipCard: $('input[name="ProvidePlaceCitizenshipCard"]').val(),
                PhotoBeforeCitizenshipCard: $('input[name="PhotoBeforeCitizenshipCard"]').val(),
                PhotoAfterCitizenshipCard: $('input[name="PhotoAfterCitizenshipCard"]').val()
            },

            EmergencyContact: {
                NamePrimaryContact: $('input[name="NamePrimaryContact"]').val(),
                RelationshipPrimaryContact: $('input[name="RelationshipPrimaryContact"]').val(),
                PhoneNoPrimaryContact: $('input[name="PhoneNoPrimaryContact"]').val(),
                NameSecondaryContact: $('input[name="NameSecondaryContact"]').val(),
                RelationshipSecondaryContact: $('input[name="RelationshipSecondaryContact"]').val(),
                PhoneNoSecondaryContact: $('input[name="PhoneNoSecondaryContact"]').val()
            }
        };
        const payload = {
            employee: employee
        };

        $.ajax({
            type: "PUT",
            url: "/employees/update",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (response) {
                if (response.succeeded) {
                    base.notify(response.messages[0], "success");
                    setTimeout(() => window.location.reload(), 1000);
                } else {
                    base.notify(response.messages[0], "error");
                }
            },
            error: function () {
                base.notify("Đã xảy ra lỗi trong quá trình lưu dữ liệu", "error");
            },
            complete: function () {
                $btnSave.prop("disabled", false).html(originalText);
            }
        });
    }
    function getDate(selector) {
        const value = $(selector).val()?.trim();
        if (!value) return null;

        const lang = getLang();
        const parts = value.split("/");

        if (lang === "vi" && parts.length === 3) {
            const [day, month, year] = parts;
            return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
        }

        if (lang !== "vi" && parts.length === 3) {
            const [year, month, day ] = parts;
            return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
        }

        return null;
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
