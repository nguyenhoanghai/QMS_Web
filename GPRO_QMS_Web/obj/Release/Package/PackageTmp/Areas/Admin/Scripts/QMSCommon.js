﻿function UploadPicture(controlId, hiddenId) {
    var str = '';
    if (window.FormData !== undefined) {
        var fileUpload = $('#' + controlId).get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        $.ajax({
            url: '/Admin/Upload/UploadFile',
            type: "POST",
            data: fileData,
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            success: function (result) {
                str = result;
                $('#' + hiddenId).val(result).change();
            },
            error: function (err) {
                alert("Lỗi up hình : " + err.statusText);
            }
        });
    }
    else {
        alert("FormData is not supported.");
    }
    return str;
}

function GetUserSelect(controlName) {
    $.ajax({
        url: '/Admin/User/GetUserSelect',
        type: 'POST',
        data: '',
        contentType: 'application/json charset=utf-8',
        success: function (data) {
            if (data.Result == "OK") {
                var str = '<option value="">Không có dữ liệu</option>';
                if (data.Data.length > 0) {
                    str = '<option value="0">Tất cả nhân viên</option>';
                    $.each(data.Data, function (index, item) {
                        str += '<option value="' + item.Value + '" >' + item.Name + '</option>';
                    });
                }

                $('#' + controlName).empty().append(str);
                $('#' + controlName).change();
            }
            else
                alert("Đã có lỗi xảy ra trong quá trình sử lý.");

        }
    });
}

function GetServiceSelect(controlName) {
    $.ajax({
        url: '/DangKyOnline/GetServiceSelect',
        type: 'POST',
        data: '',
        contentType: 'application/json charset=utf-8',
        success: function (data) {
            GlobalCommon.CallbackProcess(data, function () {
                if (data.Result == "OK") {
                    var str = '';
                    $.each(data.Data, function (index, item) {
                        str += '<option value="' + item.Value + '" >' + item.Name + '</option>';
                    });
                    $('#' + controlName).empty().append(str);
                    $('#' + controlName).change();
                }
                else
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra trong quá trình sử lý.");
            }, false, '', true, true, function () {
                var msg = GlobalCommon.GetErrorMessage(data);
                GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra.");
            });
        }
    });
}

/**********************************************************************************************************************
 *                                                  CHECK KHÔNG CHO NHẬP CHỮ                                          *      
 **********************************************************************************************************************/
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode == 59 || charCode == 46)
        return true;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
    { GlobalCommon.ShowMessageDialog("Vui lòng nhập số.", function () { }, "Lỗi Nhập liệu"); }
}

