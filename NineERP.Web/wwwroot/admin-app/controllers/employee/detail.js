var EmployeeDetail = function () {
  this.initialize = function () {
    registerEvents();
    applyStatusBadge();
  };

  function registerEvents() {
    $("#employeeTabs a").on("click", function (e) {
      e.preventDefault();
      $(this).tab("show");
    });
      $(".btn-edit-from-detail").on("click", function () {
          sessionStorage.setItem("employee_referrer", "detail");
      });

      $("#btn-export-contract").on("click", function () {
          if (!employeeId) return;

          base.notify(window.localization?.Exporting || "Exporting...", "info");

          window.location.href = `/employees/export-contract/${employeeId}`;
      });

      document.getElementById("btn-delete-employee")?.addEventListener("click", function () {
          Swal.fire({
              title: window.localization?.ConfirmDeleteEmployeeTitle || "Delete employee?",
              text: window.localization?.ConfirmDeleteEmployeeText || "Are you sure you want to delete this employee?",
              icon: "warning",
              showCancelButton: true,
              confirmButtonText: window.localization?.Yes || "Yes",
              cancelButtonText: window.localization?.No || "No",
          }).then(function (result) {
              if (result === true || result.value === true) {
                  $.post("/employees/delete", { id: employeeId }, function (response) {
                      if (response.succeeded) {
                          base.notify(window.localization?.DeleteSuccessful || "Deleted successfully.", "success");
                          window.location.href = "/employees";
                      } else {
                          base.notify(window.localization?.DeleteFailed || "Delete failed.", "error");
                      }
                  }).fail(function () {
                      base.notify(window.localization?.DeleteFailed || "Delete failed.", "error");
                  });
              }
          });
      });

      document.getElementById("btn-back-employee")?.addEventListener("click", function () {
          const stored = sessionStorage.getItem("employeeSearchParams");
          if (stored) {
              const params = JSON.parse(stored);
              const query = new URLSearchParams(params).toString();
              window.location.href = `/employees?${query}`;
          } else {
              window.location.href = "/employees";
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
      const allowedExtensions = ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx', 'jpg', 'jpeg', 'png', 'gif', 'webp'];
      const fileExtension = file.name.split('.').pop().toLowerCase();

      if (!allowedExtensions.includes(fileExtension)) {
        base.notify(window.localization?.UnsupportedFileFormat || "Unsupported file format", "error");
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
            base.notify(window.localization?.UploadSuccessful || "Upload successful", "success");
            reloadDocumentList(); // ✅ load lại list document sau khi upload
          } else {
            base.notify(window.localization?.UploadFailed || "Upload failed", "error");
          }
          $("#uploadProgressWrapper").addClass("d-none");
        },
        error: function () {
          base.notify(window.localization?.UploadFailed || "Upload failed", "error");
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
          title: window.localization?.ConfirmDeleteDocumentTitle || "Delete document?",
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
              base.notify(window.localization?.DeleteSuccessful || "Deleted successfully.", "success");
              reloadDocumentList();
            } else {
              base.notify(window.localization?.DeleteFailed || "Delete failed.", "error");
            }
          }).fail(function () {
            base.notify(window.localization?.DeleteFailed || "Delete failed.", "error");
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
                .map((doc) => `
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
    `).join("");

            $("#document-list").html(html);
        } else {
            $("#document-list").html(`<p class="text-muted font-italic">${window.localization?.NoDocumentsFound || "No documents found."}</p>`);
        }

    });
  }

  function applyStatusBadge() {
    const $status = $("#employee-status");
    const statusText = $status.data("status");
    if (!statusText) return;

    const statusKey = statusText.trim().toLowerCase();
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

    const className = classMap[statusKey] || "btn-outline-secondary";
    $status
      .removeAttr("data-status")
      .removeClass()
      .addClass(
        `status-badge btn btn-sm ${className} waves-effect waves-light ml-2`
      )
      .text(statusText);
  }
};
