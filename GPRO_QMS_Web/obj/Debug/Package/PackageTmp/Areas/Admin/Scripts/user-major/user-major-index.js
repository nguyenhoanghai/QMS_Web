
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
GPRO.namespace('UserMajor');
GPRO.UserMajor = function () {
    var Global = {
        UrlAction: {
            Gets: '/Admin/UserMajor/Gets',
            GetUsers: '/Admin/User/GetList',
            Delete: '/Admin/UserMajor/Delete',
            Save: '/Admin/UserMajor/Save'
        },
        Element: {
            Jtable: 'table',
            Popup: 'popup'
        },
        Data: {
            Id: 0,
            folder: $('#table').attr('imgFolder'),
            ParentId:0
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
        GetMajorSelect('major');
    }

    var RegisterEvent = function () {
    }

    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách nhân viên',
            paging: true,
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: false,
            actions: {
                listAction: Global.UrlAction.GetUsers 
            },
            messages: {
                addNewRecord: 'Thêm mới',  
            },
            searchInput: {
                id: 'search-key',
                className: 'search-input',
                placeHolder: 'Nhập từ khóa ...',
                keyup: function (evt) {
                    if (evt.keyCode == 13)
                        ReloadList();
                }
            },
            datas: {
                jtableId: Global.Element.Jtable
            },
            rowInserted: function (event, data) {
                if (data.record.Id == Global.Data.ParentId) {
                    var $a = $('#' + Global.Element.Jtable).jtable('getRowByKey', data.record.Id);
                    $($a.children().find('.aaa')).click();
                }
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
                    width: "3%",
                    columnClass: 'text-center',
                    sorting: false,
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
                    title: "Nhân viên",
                    width: "15%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + data.record.Name + '</span>';
                        return txt;
                    }
                },
                Detail: {
                    title: 'DS nghiệp vụ',
                    width: '3%',
                    sorting: false,
                    edit: false,
                    columnClass:'text-center',
                    display: function (parent) {
                        var $img = $('<i class="fa fa-list-ol clickable red aaa" title="Danh sách nghiệp vụ nhân viên: ' + parent.record.Name + '"></i>');
                        $img.click(function () {
                            Global.Data.ParentId = parent.record.Id;
                            $('#' + Global.Element.Jtable).jtable('openChildTable',
                                $img.closest('tr'),
                                {
                                    title: `<div>Danh sách nghiệp vụ của <span class="red">${parent.record.Name}</span></div>`,
                                    paging: true,
                                    pageSize: 20,
                                    pageSizeChange: true,
                                    sorting: true,
                                    selectShow: true,
                                    actions: {
                                        listAction: Global.UrlAction.Gets + '?userId=' + parent.record.Id,
                                        createAction: Global.Element.Popup,
                                    },
                                    messages: {
                                        addNewRecord: 'Thêm nghiệp vụ', 
                                        selectShow: 'Ẩn hiện cột'
                                    },
                                    fields: {
                                        OrderId: {
                                            type: 'hidden',
                                            defaultValue: parent.record.Id
                                        },
                                        Id: {
                                            key: true,
                                            create: false,
                                            edit: false,
                                            list: false
                                        },
                                        Index: {
                                            title: "STT",
                                            width: "10%",
                                            columnClass:'text-center'
                                        },
                                        MajorName: {
                                            title: "Nghiệp vụ",
                                            width: "80%",
                                            columnClass: 'text-center'
                                        },                                         
                                        actions: {
                                            title: '',
                                            width: '10%',
                                            sorting: false,
                                            columnClass: 'text-center',
                                            display: function (data) {
                                                var div = $('<div></div>');

                                                var btnEdit = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue"  ></i>');
                                                btnEdit.click(function () {
                                                    $('#id').val(data.record.Id);
                                                    $('#major').val(data.record.MajorId);
                                                    $('#index').val(data.record.Index); 
                                                });
                                                div.append(btnEdit)

                                                var btnDelete = $('<i title="Xóa" class="fa fa-trash-o clickable red i-delete"></i>');
                                                btnDelete.click(function () {
                                                    GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                                        Delete(data.record.Id);
                                                    }, function () { }, 'Đồng ý', 'Hủy bỏ', 'Thông báo');
                                                });
                                                div.append(btnDelete);
                                                return div;
                                            }
                                        }, 
                                    }
                                }, function (data) { //opened handler
                                    data.childTable.jtable('load');
                                });
                        });
                        return $img;
                    }
                },
            }
        });
    }

    function ReloadList() {
        $('#' + Global.Element.Jtable).jtable('load', { 'keyword': $('#search-key').val() });
    }

    function Delete(Id) {
        $.ajax({
            url: Global.UrlAction.Delete,
            type: 'POST',
            data: JSON.stringify({ 'Id': Id }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $('#loading').show(); },
            success: function (data) {
                GlobalCommon.CallbackProcess(data, function () {
                    if (data.Result == "OK") {
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

    function Save() {
        var obj = {
            Id: $('#id').val(),
            MajorId: $('#major').val(),
            Index: $('#index').val(),
            UserId: Global.Data.ParentId
        }
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
            if (CheckValidate())
                Save();
        });

        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            $('#id').val(0);
            $('#major').val('');
            $('#index').val('');
        });
    }

    function CheckValidate() {
        if ($('#major').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn nghiệp vụ.", function () { $('#major').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#index').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự.", function () { $('#index').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else
            return true;
    }

}
$(document).ready(function () {
    var obj = new GPRO.UserMajor();
    obj.Init();
})