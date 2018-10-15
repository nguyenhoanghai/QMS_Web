if (typeof GPRO == 'undefined' || !GPRO) {
    var GPRO = {};
}

GPRO.namespace = function () {
    var a = arguments,
        o = null,
        i, j, d;
    for (i = 0; i < a.length; i = i + 1) {
        d = ('' + a[i]).split('.');
        o = GPRO;
        for (j = (d[0] == 'GPRO') ? 1 : 0; j < d.length; j = j + 1) {
            o[d[j]] = o[d[j]] || {};
            o = o[d[j]];
        }
    }
    return o;
}
GPRO.namespace('User');
GPRO.User = function () {
    var Global = {
        UrlAction: {
            Save: '/Admin/User/Save',
        },
        Element: {
        },
        Data: {
            Avartar: ''
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        GetUserSelect('trogiup');
    }

    var RegisterEvent = function () {
        $('#btnBack').click(function () {
            window.location.href = '/Admin/User/Index';
        });

        $('#btnSave').click(function () {
            if (CheckValidate()) {
                if ($('#pictureuploader').val() != '')
                    UploadPicture('pictureuploader', 'hid_avatar');
                else
                    Save();
            }
        });

        $('#hid_avatar').change(function () {
            Global.Data.Avartar = $('#hid_avatar').val();
            Save();
        });
    }

    function CheckValidate() {
        if ($('#tennv').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Tên Nhân Viên.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#username').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng Nhập tên tài khoản.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#password').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập mật khẩu.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }

    function Save() {
        var obj = {
            MANV: 0,
            TENNV: $('#tennv').val(),
            DIACHI: $('#diachi').val(),
            GIOITINH: $('#gioitinh').prop('checked'),
            TROGIUP: $('#trogiup').val(),
            USERNAME: $('#username').val(),
            PASSWORD: $('#password').val(),
            Hinh: Global.Data.Avartar,
            ChucVu: $('#chucvu').val(),
            QTCongTac: $('#qtcongtac').val(),
            ChuyenMon: $('#chuyenmon').val(),
        }
        $.ajax({
            url: Global.UrlAction.Save,
            type: 'POST',
            data: JSON.stringify({ 'nv': obj }),
            contentType: 'application/json charset=utf-8',
            success: function (result) {
                if (result.Result == "OK") {
                    $('#btnBack').click();
                }
                else
                    GlobalCommon.ShowMessageDialog(result.ErrorMessages[0].Message, function () { }, 'Lỗi');
            }
        });
    }

}

$(document).ready(function () {
    var user = new GPRO.User();
    user.Init();
});