
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
GPRO.namespace('VideoSchedule');
GPRO.VideoSchedule = function () {
    var Global = {
        UrlAction: {
            Gets: '/Admin/VideoSchedule/Gets', 
            Delete: '/Admin/VideoSchedule/Delete',
            Save: '/Admin/VideoSchedule/Save',

            _Gets: '/Admin/VideoSchedule/_Gets',
            _Delete: '/Admin/VideoSchedule/_Delete',
            _Save: '/Admin/VideoSchedule/_Save'
        },
        Element: {
            Jtable: 'jtable',
            Popup: 'popup',
            PopupChild:'popup-child'
        },
        Data: { 
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
        InitPopupChild();
        GetVideoSelect('video');
    }

    var RegisterEvent = function () {
    }

    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách lịch phát video',
            paging: true,
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: false,
            actions: {
                listAction: Global.UrlAction.Gets,
                createAction: Global.Element.Popup,
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
                TemplateName: {
                    visibility: 'fixed',
                    title: "Tên mẫu",
                    width: "30%",
                },
                IsActive: {
                    title: "Trạng thái",
                    width: "7%",
                    columnClass: 'text-center',
                    display: function (data) {
                        var text = '';
                        if (data.record.IsActive)
                            text = $('<i class="fa fa-check-square-o"  ></i> ');
                        else
                            text = $('<i class="fa fa-square-o" ></i> ');
                        return text;
                    }
                },
                Note: {
                    title: "Ghi chú",
                    width: "53%", 
                    sorting:false
                },
                actions: {
                    title: '',
                    width: "10%",
                    sorting: false,
                    columnClass: 'text-center',
                    display: function (parent) {
                        var div = $('<div></div>');
                        var $img = $('<i style="margin-right:10px" class="fa fa-list-ol clickable red aaa" title="Danh sách nghiệp vụ nhân viên: ' + parent.record.Name + '"></i>');
                        $img.click(function () {
                            Global.Data.ParentId = parent.record.Id;
                            $('#' + Global.Element.Jtable).jtable('openChildTable',
                                $img.closest('tr'),
                                {
                                    title: `<div>Danh sách tệp video của <span class="red">${parent.record.TemplateName}</span></div>`,
                                    paging: true,
                                    pageSize: 20,
                                    pageSizeChange: true,
                                    sorting: true,
                                    selectShow: true,
                                    actions: {
                                        listAction: Global.UrlAction._Gets + '?tmpId=' + parent.record.Id,
                                        createAction: Global.Element.PopupChild,
                                    },
                                    messages: {
                                        addNewRecord: 'Thêm video',
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
                                            columnClass: 'text-center'
                                        },
                                        FileName: {
                                            title: "Tệp video",
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

                                                var btnEdit = $('<i data-toggle="modal" data-target="#' + Global.Element.PopupChild + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue"  ></i>');
                                                btnEdit.click(function () {
                                                    $('#id-c').val(data.record.Id);
                                                    $('#video').val(data.record.VideoId);
                                                    $('#index').val(data.record.Index);
                                                });
                                                div.append(btnEdit)

                                                var btnDelete = $('<i title="Xóa" class="fa fa-trash-o clickable red i-delete"></i>');
                                                btnDelete.click(function () {
                                                    GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                                        DeleteChild(data.record.Id);
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
                        div.append($img)

                        var btnEdit = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa" class="fa fa-pencil-square-o clickable blue"  ></i>');
                        btnEdit.click(function () { 
                            $('#id').val(parent.record.Id); 
                            $('#template-name').val(parent.record.TemplateName); 
                            $('#note').val(parent.record.Note);
                            $('#active').prop('checked', parent.record.IsActive).change();
                        });
                        div.append(btnEdit);

                        var btnDelete = $('<i title="Xóa" class="fa fa-trash red i-delete clickable"></i>');
                        btnDelete.click(function () {
                            GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                Delete(parent.record.Id);
                            }, function () { }, 'Đồng ý', 'Hủy bỏ', 'Thông báo');
                        });
                        div.append(btnDelete);
                        return div;
                    }
                }

            }
        });
    }

    function ReloadList() {
        $('#' + Global.Element.Jtable).jtable('load', { 'keyword': $('#search-key').val() });
    }

    //#region
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
            TemplateName: $('#template-name').val(),
            IsActive: $('#active').prop('checked'),
            Note: $('#note').val()
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
            $('#template-name').val('');
            $('#note').val('');
            $('#active').prop('checked', false).change();
        });
    }

    function CheckValidate() {
        if ($('#template-name').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn tên mẫu phát.", function () { $('#template-name').focus() }, "Lỗi Nhập liệu");
            return false;
        } 
        else
            return true;
    }
    //#endregion

    //#region

    function SaveChild() {
        var obj = {
            Id: $('#id-c').val(),
            VideoId: $('#video').val(), 
            Index: $('#index').val(),
            TemplateId: Global.Data.ParentId
        }
        $.ajax({
            url: Global.UrlAction._Save,
            type: 'post',
            data: ko.toJSON(obj),
            contentType: 'application/json',
            beforeSend: function () { $('#loading').show(); },
            success: function (result) {
                $('#loading').hide();
                GlobalCommon.CallbackProcess(result, function () {
                    if (result.Result == "OK") {
                        ReloadList();
                        $("#" + Global.Element.PopupChild + ' button[cancel]').click();
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

    function InitPopupChild() {
        $("#" + Global.Element.PopupChild).modal({
            keyboard: false,
            show: false
        });
        $("#" + Global.Element.PopupChild + ' button[save]').click(function () {
            if (CheckValidateChild())
                SaveChild();
        });

        $("#" + Global.Element.PopupChild + ' button[cancel]').click(function () {
            $("#" + Global.Element.PopupChild).modal("hide");
            $('#id-c').val(0);
            $('#video').val('');
            $('#index').val(0); 
        });
    }

    function CheckValidateChild() {
        if ($('#video').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn tệp video.", function () { $('#template-name').focus() }, "Lỗi Nhập liệu");
            return false;
        }
       else if ($('#index').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn nhập thứ tự phát video.", function () { $('#template-name').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else
            return true;
    }

    function DeleteChild(Id) {
        $.ajax({
            url: Global.UrlAction._Delete,
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

    //#endregion
}
$(document).ready(function () {
    var obj = new GPRO.VideoSchedule();
    obj.Init();
})