
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
            avatar: '',
            Id : 0
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
            Global.Data.NhanVienModel.Avatar = $('#hid_avatar').val();
            Save();
        });
    }

    function InitViewModel(NhanVien) {
        var ViewModel = {
            Id: 0,
            Name: '',
            Sex: false,
            Address: '',
            UserName: '',
            Password: '',
            Avatar: '',
            Professional: '',
            Position: '',
            WorkingHistory: '',
            Help: ''
        };
        if (NhanVien != null) {
            ViewModel = {
                Id: ko.observable(NhanVien.Id),
                Name: ko.observable(NhanVien.Name),
                Sex: ko.observable(NhanVien.Sex),
                Address: ko.observable(NhanVien.Address),
                UserName: ko.observable(NhanVien.UserName),
                Password: ko.observable(NhanVien.Password),
                Avatar: ko.observable(NhanVien.Avatar),
                Professional: ko.observable(NhanVien.Professional),
                Position: ko.observable(NhanVien.Position),
                WorkingHistory: ko.observable(NhanVien.WorkingHistory),
                Help: ko.observable(NhanVien.Help)
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
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: true,
            actions: {
                listAction: Global.UrlAction.GetList,
                createAction: Global.Element.Popup,
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
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                UserName: {
                    visibility: 'fixed',
                    title: "Tài khoản",
                    width: "5%", 
                },
                Name: { 
                    title: "Tên NV",
                    width: "15%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + data.record.Name + '</span>';
                        return txt;
                    }
                },
                Sex: {
                    title: "Giới Tính",
                    width: "7%",
                    display: function (data) {
                        var text = '';
                        if (data.record.Sex)
                            text = $('<i class="fa fa-male" style="font-size:26px"></i> ');
                        else
                            text = $('<i class="fa fa-female blue"  style="font-size:26px"></i> ');
                        return text;
                    }
                },
                Address: {
                    title: "Địa Chỉ",
                    width: "15%",
                },
                Avatar: {
                    title: "Hình",
                    width: "7%",
                    display: function (data) {
                        var txt;
                        if (data.record.Avatar != null)
                            txt = '<span><img src="' +($('#'+Global.Element.JtableNhanVien).attr('imgFolder') + data.record.Avatar )+ '" width="40px" height="40px"/></span>';
                        else
                            txt = '<span>' + " " + '</span>';
                        return txt;
                    }
                },
                Professional: {
                    title: "Chuyên Môn",
                    width: "10%",
                },
                Position: {
                    title: "Chức Vụ",
                    width: "15%",
                },
                WorkingHistory: {
                    title: "QT Công Tác",
                    width: "15%",
                },
                Help: {
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
                            $('#sex').attr('checked', data.record.Sex).change();
                            $('#username').val(  data.record.UserName) ;
                            $('#password').val( data.record.Password) ;
                            Global.Data.avatar = data.record.Avatar;
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
                                Global.Data.FileName = data.record.Avatar;
                                Delete(data.record.Id);
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

    function Delete(Id) {
        $.ajax({
            url: Global.UrlAction.Delete,
            type: 'POST',
            data: JSON.stringify({ 'manv': Id }),
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
        Global.Data.NhanVienModel.Sex = $('#sex').prop('checked');
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
                    Global.Data.NhanVienModel.Avatar = Global.Data.avatar;
                    Save();
                }
            }
        });
        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            BindData(null);
            $('#sex').attr('checked', false).change();
            $('#hid_avatar').val('');  
            $('#avatar').val(''); 
        });
    }
    function CheckValidate() {
        if ($('#username').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Tên tài khoản.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#name').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Tên Nhân Viên.", function () { }, "Lỗi Nhập liệu");
            return false;
        } 
        else if (Global.Data.Id == 0 && $('#password').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập mật khẩu.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else
        return true;
    }

}
$(document).ready(function () {
    var NhanVien = new GPRO.NhanVien();
    NhanVien.Init();
})