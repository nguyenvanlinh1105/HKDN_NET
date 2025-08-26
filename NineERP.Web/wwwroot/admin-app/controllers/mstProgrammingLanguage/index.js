var MstProgrammingLanguagesController = function () {
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

        $('#modalAddMstProgrammingLanguage').on('hidden.bs.modal', function () {
            $('#form-add-mst-programming-language')[0].reset();
            $('#mstProgrammingLanguageId').val('');
        });

        $('#btn-show-add-modal').on('click', function () {
            $('#form-add-mst-programming-language')[0].reset();
            $('#mstProgrammingLanguageId').val('');
            $('#modalAddMstProgrammingLanguageLabel').text(window.localization.AddNewMstProgrammingLanguage);
            $('#btnSaveAndContinue').show(); // Hiện lại nút
            $('#modalAddMstProgrammingLanguage').modal('show');
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete programming language?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this programming language?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/mstprogramminglanguages/Delete", { id }, function (response) {
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

        $('body').on('submit', '#form-add-mst-programming-language', function (e) {
            e.preventDefault();
            const id = $('#mstProgrammingLanguageId').val();
            const formData = $(this).serializeArray();
            const request = {};
            formData.forEach(field => {
                request[field.name] = field.value;
            });        
            
            const isSaveAndContinue = $(document.activeElement).attr('id') === 'btnSaveAndContinue';
            const isEdit = !!id;
            const payload = {
                mstprogramminglanguage: {
                    id: isEdit ? id : undefined,
                    nameVi: request.NameVi,
                    nameEn: request.NameEn,
                    nameJp: request.NameJp,
                    description: request.Description
                }
            };
            $.ajax({
                type: isEdit ? "PUT" : "POST",
                url: isEdit ? "/mstprogramminglanguages/update" : "/mstprogramminglanguages/create",
                contentType: "application/json",
                data: JSON.stringify(payload),
                beforeSend: function () {
                    $('#form-add-mst-programming-language button[type="submit"]').prop('disabled', true);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        if (isSaveAndContinue) {
                            $('#form-add-mst-programming-language')[0].reset();
                            $('#mstProgrammingLanguageNameVi').focus();
                        } else {
                            $('#modalAddMstProgrammingLanguage').modal('hide');
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
                    $('#modalAddMstProgrammingLanguage button[type="submit"]').prop('disabled', false);
                }
            });
        });
    }

    function loadData(isPageChanged) {
        $.get("/mstprogramminglanguages/getallpaging", {
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
                        NameJp: item.nameJp,
                        Description: item.description,
                        Id: item.id
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html(`<tr><td colspan="6" class="text-center text-muted">${window.localization?.NoRecordsFound || 'No records found.'}</td></tr>`);
            }
        });
    }

    function getById(id) {
        $.get("/mstprogramminglanguages/GetById", { id: id }, function (response) {
            if (response && response.data) {
                const data = response.data;
                // Set tiêu đề modal
                $('#modalAddMstProgrammingLanguageLabel').text(window.localization.EditMstProgrammingLanguage);
                $('#btnSaveAndContinue').hide();
                // Đổ dữ liệu vào form
                $('#mstProgrammingLanguageNameVi').val(data.nameVi);
                $('#mstProgrammingLanguageNameEn').val(data.nameEn);
                $('#mstProgrammingLanguageNameJp').val(data.nameJp);
                $('#mstProgrammingLanguageDescription').val(data.description);
                $('#mstProgrammingLanguageId').val(data.id); 

                // Hiển thị modal
                $('#modalAddMstProgrammingLanguage').modal('show');
            } else {
                base.notify(response.messages[0], 'error');
            }
        }).fail(function () {
            base.notify('Something went wrong.', 'error');
        });
    }
};
