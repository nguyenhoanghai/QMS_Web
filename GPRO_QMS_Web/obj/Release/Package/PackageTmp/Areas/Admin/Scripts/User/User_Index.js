
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
GPRO.NhanVien = function () {
    var Global = {
        UrlAction: {
            GetList: '/Admin/User/GetList',
            Create: '/Admin/User/Create',
            Delete: '/Admin/User/Delete',
            Save: '/Admin/User/Save'
        },
        Element: {
            JtableNhanVien: 'jtableNhanVien',
            Popup: 'popup',
            PopupSearch: 'popup_Search'
        },
        Data: {
            NhanVienModel: {},
            FileName: '',
            avatar: ''
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        InitList();
        ReloadList();
        InitPopup();
        BindData(null);
    }


    var RegisterEvent = function () {

        $('[search]').click(function () {
            ReloadList();
            $('[close]').click();
        });

        $('[close]').click(function () {
            $('#keyword').val('');
        });

        $('#hid_avatar').change(function () {
            Global.Data.NhanVienModel.Hinh = $('#hid_avatar').val();
            Save();
        });
    }

    function InitViewModel(NhanVien) {
        var ViewModel = {
            MANV: 0,
            TENNV: '',
            GIOITINH: false,
            DIACHI: '',
            USERNAME: '',
            PASSWORD: '',
            Hinh: '',
            ChuyenMon: '',
            ChucVu: '',
            QTCongTac: '',
            TROGIUP: ''
        };
        if (NhanVien != null) {
            ViewModel = {
                MANV: ko.observable(NhanVien.MANV),
                TENNV: ko.observable(NhanVien.TENNV),
                GIOITINH: ko.observable(NhanVien.GIOITINH),
                DIACHI: ko.observable(NhanVien.DIACHI),
                USERNAME: ko.observable(NhanVien.USERNAME),
                PASSWORD: ko.observable(NhanVien.PASSWORD),
                Hinh: ko.observable(NhanVien.Hinh),
                ChuyenMon: ko.observable(NhanVien.ChuyenMon),
                ChucVu: ko.observable(NhanVien.ChucVu),
                QTCongTac: ko.observable(NhanVien.QTCongTac),
                TROGIUP: ko.observable(NhanVien.TROGIUP)
            };
        }
        return ViewModel;
    }

    function BindData(NhanVien) {
        Global.Data.NhanVienModel = InitViewModel(NhanVien);
        ko.applyBindings(Global.Data.NhanVienModel);
    }

    function InitList() {
        $('#' + Global.Element.JtableNhanVien).jtable({
            title: 'Thông Tin Nhân Viên',
            paging: true,
            pageSize: 10,
            pageSizeChange: true,
            sorting: true,
            selectShow: true,
            actions: {
                listAction: Global.UrlAction.GetList,
                createActionUrl: Global.UrlAction.Create,
                searchAction: Global.Element.PopupSearch,
            },
            messages: {
                addNewRecord: 'Thêm mới',
                searchRecord: 'Tìm kiếm',
                selectShow: 'Ẩn hiện cột'
            },
            datas: {
                jtableId: Global.Element.JtableNhanVien
            },
            fields: {
                MANV: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                TENNV: {
                    visibility: 'fixed',
                    title: "Tên NV",
                    width: "15%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + data.record.TENNV + '</span>';
                        return txt;
                    }
                },
                GIOITINH: {
                    title: "Giới Tính",
                    width: "7%",
                    display: function (data) {
                        var text = '';
                        if (data.record.GIOITINH)
                            text = $('<i class="fa fa-male" style="font-size:26px"></i> ');
                        else
                            text = $('<i class="fa fa-female blue"  style="font-size:26px"></i> ');
                        return text;
                    }
                },
                DIACHI: {
                    title: "Địa Chỉ",
                    width: "15%",
                },
                Hinh: {
                    title: "Hình",
                    width: "7%",
                    display: function (data) {
                        var txt;
                        if (data.record.Hinh != null)
                            txt = '<span><img src="' +($('#'+Global.Element.JtableNhanVien).attr('imgFolder') + data.record.Hinh )+ '" width="40px" height="40px"/></span>';
                        else
                            txt = '<span>' + " " + '</span>';
                        return txt;
                    }
                },
                ChuyenMon: {
                    title: "Chuyên Môn",
                    width: "10%",
                },
                ChucVu: {
                    title: "Chức Vụ",
                    width: "15%",
                },
                QTCongTac: {
                    title: "QT Công Tác",
                    width: "15%",
                },
                TROGIUP: {
                    title: "Trợ Giúp",
                    width: "15%",
                },
                edit: {
                    title: '',
                    width: '1%',
                    sorting: false,
                    display: function (data) {
                        var text = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue"  ></i>');
                        text.click(function () {
                            BindData(data.record);
                            $('#sex').attr('checked', data.record.GIOITINH).change();
                            Global.Data.avatar = data.record.Hinh;
                        });
                        return text;
                    }
                },
                Delete: {
                    title: '',
                    width: "3%",
                    sorting: false,
                    display: function (data) {
                        var text = $('<button title="Xóa" class="jtable-command-button jtable-delete-command-button"><span>Xóa</span></button>');
                        text.click(function () {
                            GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                Global.Data.FileName = data.record.Hinh;
                                Delete(data.record.MANV);
                            }, function () { }, 'Đồng ý', 'Hủy bỏ', 'Thông báo');
                        });
                        return text;
                    }
                }
            }
        });
    }

    function ReloadList() {
        $('#' + Global.Element.JtableNhanVien).jtable('load', { 'keyword': $('#keyword').val(), 'searchBy': $('#searchBy').val() });
        $('#' + Global.Element.PopupSearch).modal('hide');
    }

    function Delete(manv) {
        $.ajax({
            url: Global.UrlAction.Delete,
            type: 'POST',
            data: JSON.stringify({ 'MANV': manv }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $('#loading').show(); },
            success: function (data) {
                GlobalCommon.CallbackProcess(data, function () {
                    if (data.Result == "OK") {
                        DeleteFile();
                        ReloadList();
                        $('#loading').hide();
                    }
                }, false, Global.Element.PopupNhanVien, true, true, function () {

                    var msg = GlobalCommon.GetErrorMessage(data);
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra.");
                });
            }
        });
    }

    //Xóa Hình Ảnh
    function DeleteFile() {
        var filename = Global.Data.FileName;
        $.ajax({
            url: '/Admin/Upload/DeleteFile',
            type: "POST",
            data: JSON.stringify({ 'filename': filename }),
            contentType: 'application/json charset=utf-8',
        });
    }

    function Save() {       
        Global.Data.NhanVienModel.GIOITINH = $('#sex').prop('checked');
        $.ajax({
            url: Global.UrlAction.Save,
            type: 'post',
            data: ko.toJSON(Global.Data.NhanVienModel),
            contentType: 'application/json',
            beforeSend: function () { $('#loading').show(); },
            success: function (result) {
                $('#loading').hide();
                GlobalCommon.CallbackProcess(result, function () {
                    if (result.Result == "OK") {
                        ReloadList();
                        $("#" + Global.Element.Popup + ' button[cancel]').click();
                    }
                    else
                        GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra trong quá trình sử lý.");
                }, false, Global.Element.PopupModule, true, true, function () {
                    var msg = GlobalCommon.GetErrorMessage(result);
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra trong quá trình sử lý.");
                });
            }
        });
    }

    function InitPopup() {
        $("#" + Global.Element.Popup).modal({
            keyboard: false,
            show: false
        });
        $("#" + Global.Element.Popup + ' button[save]').click(function () {
            if (CheckValidate()) {
                if ($('#avatar').val() != '')
                     UploadPicture('avatar', 'hid_avatar');
                else {
                    Global.Data.NhanVienModel.Hinh = Global.Data.avatar;
                    Save();
                }
            }
        });
        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            BindData(null);
            $('#sex').attr('checked', false).change();
            $('#hid_avatar').val('');
        });
    }
    function CheckValidate() {
        return true;
    }

}
$(document).ready(function () {
    var NhanVien = new GPRO.NhanVien();
    NhanVien.Init();
})