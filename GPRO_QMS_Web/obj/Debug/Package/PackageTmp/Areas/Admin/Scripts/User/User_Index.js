
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
        },
        Data: {
            NhanVienModel: {},
            FileName: '',
            avatar: '',
            Id: 0,
            folder: $('#jtableNhanVien').attr('imgFolder')
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
    }


    var RegisterEvent = function () {
        $('#hid_avatar').change(function () {
            Global.Data.avatar = $('#hid_avatar').val();
            Save();
        });


        $('#p-file-upload').change(function () {
            readURL(this);
        });

        $('#p-btn-file-upload').click(function () {
            $('#p-file-upload').click();
        });

        // Register event after upload file done the value of [filelist] will be change => call function save your Data 
        $('#p-file-upload').select(function () {
            Save();
        });
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
            },
            messages: {
                addNewRecord: 'Thêm mới', 
                selectShow: 'Ẩn hiện cột'
            },
            searchInput: {
                id: 'search-key',
                className: 'search-input',
                placeHolder: 'Nhập tên nhân viên | tài khoản...',
                keyup: function (evt) {
                    if (evt.keyCode == 13)
                        ReloadList();
                }
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
                Avatar: {
                    title: "Hình",
                    width: "7%",
                    columnClass: 'text-center',
                    sorting:false,
                    display: function (data) {
                        var txt;
                        if (data.record.Avatar != null)
                            txt = `<span><img src="${Global.Data.folder}${data.record.Avatar}" width="40px" height="40px"/></span>`;
                        else
                            txt = '<span>' + " " + '</span>';
                        return txt;
                    }
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
                    columnClass: 'text-center',
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
                            $('#sex').attr('checked', data.record.Sex).change();

                            $('#id').val(data.record.Id);
                            $('#name').val(data.record.Name);
                            $('#address').val(data.record.Address);
                            $('#user-name').val(data.record.UserName);
                            $('#password').prop('disabled', true);
                            // $('#password').val(data.record.Password);
                            $('#professional').val(data.record.Professional);
                            $('#position').val(data.record.Position);
                            $('#working-history').val(data.record.WorkingHistory);
                            $('#help').val(data.record.Help);
                            if (data.record.Avatar)
                                $('.img-avatar').attr('src', `${Global.Data.folder}${data.record.Avatar}`);
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
        $('#' + Global.Element.JtableNhanVien).jtable('load', { 'keyword': $('#search-key').val() });
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
                        DeleteFile(Global.Data.FileName,'image');
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
     
    readURL = (input) => {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.img-avatar').attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]); // convert to base64 string
        }
    }

    function Save() {
        var obj = {
            Id: $('#id').val(),
            Name: $('#name').val(),
            Sex: $('#sex').prop('checked'),
            Address: $('#address').val(),
            UserName: $('#user-name').val(),
            Password: $('#password').val(),
            Avatar: Global.Data.avatar,
            Professional: $('#professional').val(),
            Position: $('#position').val(),
            WorkingHistory: $('#working-history').val(),
            Help: $('#help').val(),
            Image: $('#p-file-upload').attr('newurl')
        };
        $.ajax({
            url: Global.UrlAction.Save,
            type: 'post',
            data: ko.toJSON(obj),
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
                if ($('#p-file-upload').val() != '')
                    UploadPicture("p-file-upload", 'p-file-upload');
                else {
                    Save();
                }
            }
        });
        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            resetForm();
        });
    }

    function CheckValidate() {
        if ($('#user-name').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Tên tài khoản.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#name').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Tên Nhân Viên.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#id').val() == '0' && $('#password').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập mật khẩu.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else
            return true;
    }

    resetForm = () => {
        $('#id').val(0);
        $('#user-name').val('');
        $('#password').val('');
        $('#password').prop('disabled', false);
        $('#name').val('');
        $('#sex').prop('checked', false).change();

        $('#professional').val('');
        $('#position').val('');
        $('#working-history').val('');
        $('#help').val('');
        $('#address').val('');

        $('.img-avatar').attr('src', '/Areas/Admin/Content/images/no-image.png');
        $('#p-file-upload').attr('newurl', '');
        $('#p-file-upload').val('');
    }
}
$(document).ready(function () {
    var NhanVien = new GPRO.NhanVien();
    NhanVien.Init();
})