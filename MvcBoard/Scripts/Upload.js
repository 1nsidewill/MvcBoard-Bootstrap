var upload = {
    uploadFile: null,
    init: function () {
        this.buttons.init();
    },
    buttons: {
        init: function () {
            this.btnInputFile.change();
            this.btnUpload.click();
        },
        btnInputFile: {
            change: function () {
                $('#inputFile').change(function (e) {
                    $.each(e.target.files, function () {
                        page.importFile(this);
                    });
                    return false;
                });
            }
        },
        btnUpload: {
            click: function () {
                $('#btnUpload').click(function () {
                    if (page.uploadFile == null) {
                        alert('파일을 선택해 주세요.');
                        return;
                    }

                    var $form = $(this).closest('form')
                    var formData = new FormData($form.get(0));
                    formData.append("file", page.uploadFile);

                    $.ajax({
                        type: 'POST',
                        url: '/Home/Upload',
                        data: formData,
                        contentType: false,
                        processData: false,
                        success: function (data) {
                            if (data.IsSuccess) {
                                alert('업로드 성공');
                            } else {
                                alert('업로드 실패');
                            }
                            page.importFileReset();
                        },
                        error: function (data) {
                            alert('업로드 에러');
                            page.importFileReset();
                        }
                    });

                    return false;
                });
            }
        }
    },
    importFile: function (file) {
        $('#uploadFile').replaceWith('<div id="uploadFile">' + file.name + '</div>');
        page.uploadFile = file;
    },
    importFileReset: function () {
        $('#uploadFile').replaceWith('<div id="uploadFile">선택된 파일이 없습니다.</div>');
        page.uploadFile = null;
    }
}