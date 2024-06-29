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
GPRO.namespace('Evaluate');
GPRO.Evaluate = function () {
    var Global = {
        UrlAction: {
            GetList: '/Evaluate/PagedList',
            Save: '/Evaluate/Save',
            Delete: '/Evaluate/Delete',

            GetList_c: '/EvaluateDetail/PagedList?type=',
            Save_c: '/EvaluateDetail/Save',
            Delete_c: '/EvaluateDetail/Delete',
        },
        Element: {
            Jtable: 'jtable',
            Popup: 'popup',
            Search: 'Search',

            Popup_c: 'popup_',
        },
        Data: {
            Parent: {},
            Child: {},
            ParentId: 0,
            Avartar: '',
            folder: $('#jtable').attr('imgFolder')
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
    }

    var RegisterEvent = function () {

        $('.box-sms').hide();

        $('#Eval_c_sendSMS').change(function () {
            if ($('#Eval_c_sendSMS').prop('checked'))
                $('.box-sms').show();
            else
                $('.box-sms').hide();
        });

        $('#p-file-upload').change(function () {
            readURL(this);
        });

        $('#p-btn-file-upload').click(function () {
            $('#p-file-upload').click();
        });

        // Register event after upload file done the value of [filelist] will be change => call function save your Data 
        $('#p-file-upload').select(function () {
            SaveChild();
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


    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách tiêu chí đánh giá',
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
                id: 'keyword',
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
                Index: {
                    title: "STT",
                    width: "5%",
                    columnClass: 'text-center',
                },
                Name: {
                    visibility: 'fixed',
                    title: "Tiêu chí đánh giá",
                    width: "30%"
                },
                Note: {
                    title: "Ghi chú",
                    width: "50%",
                    sorting: false
                },
                actions: {
                    title: '',
                    width: '10%',
                    sorting: false,
                    columnClass: 'text-center',
                    display: function (parent) {
                        var div = $('<div></div>');

                        var $img = $('<i style="margin-right:10px" class="fa fa-list-ol clickable red aaa" title="Click Xem thang đánh giá ' + parent.record.Name + '"></i>');
                        $img.click(function () {
                            Global.Data.ParentId = parent.record.Id;
                            $('#' + Global.Element.Jtable).jtable('openChildTable',
                                $img.closest('tr'),
                                {
                                    title: '<span class="red">Danh sách thang đánh giá : ' + parent.record.Name + '</span>',
                                    paging: true,
                                    pageSize: 20,
                                    pageSizeChange: true,
                                    sorting: true,
                                    selectShow: true,
                                    actions: {
                                        listAction: Global.UrlAction.GetList_c + '' + parent.record.Id,
                                        createAction: Global.Element.Popup_c,
                                    },
                                    messages: {
                                        addNewRecord: 'Thêm thang ĐG',
                                        // searchRecord: 'Tìm kiếm',
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
                                            columnClass: 'text-center',
                                        },
                                        Name: {
                                            title: "Tên",
                                            width: "10%",
                                        },
                                        IsDefault: {
                                            title: "Mặc định",
                                            width: "10%",
                                            columnClass: 'text-center',
                                            display: function (data) {
                                                var text = '';
                                                if (!data.record.IsDefault)
                                                    text = $('<i class="fa fa-square-o" style="font-size:26px"></i> ');
                                                else
                                                    text = $('<i class="fa fa-check-square-o blue"  style="font-size:26px"></i> ');
                                                return text;
                                            }
                                        },
                                        Icon: {
                                            title: "Hình",
                                            width: "7%",
                                            columnClass: 'text-center',
                                            sorting: false,
                                            display: function (data) {
                                                var txt;
                                                if (data.record.Icon != null)
                                                    txt = `<span><img src="${Global.Data.folder}${data.record.Icon}" width="40px" height="40px"/></span>`;
                                                else
                                                    txt = '<span>' + " " + '</span>';
                                                return txt;
                                            }
                                        },
                                        IsSendSMS: {
                                            title: "Gửi tin nhắn",
                                            width: "10%",
                                            columnClass: 'text-center',
                                            display: function (data) {
                                                var text = '';
                                                if (!data.record.IsSendSMS)
                                                    text = $('<i class="fa fa-square-o" style="font-size:26px"></i> ');
                                                else
                                                    text = $('<i class="fa fa-check-square-o blue"  style="font-size:26px"></i> ');
                                                return text;
                                            }
                                        },
                                        SmsContent: {
                                            title: "Tin nhắn",
                                            width: "20%",
                                            sorting: false,
                                        },
                                        Note: {
                                            title: "Ghi Chú",
                                            width: "20%",
                                            sorting: false,
                                        },
                                        edit: {
                                            title: '',
                                            width: '1%',
                                            sorting: false,
                                            columnClass: 'text-center',
                                            display: function (data) {
                                                var text = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup_c + '" title="Chỉnh sửa thông tin" class="fa fa_tb fa-pencil-square-o clickable blue"  ></i>');
                                                text.click(function () {
                                                    $('#Eval_c_id').val(data.record.Id);
                                                    $('#Eval_c_name').val(data.record.Name);
                                                    $('#Eval_c_note').val(data.record.Note);
                                                    $('#Eval_c_index').val(data.record.Index);
                                                    $('#SmsContent').val(data.record.SmsContent);
                                                    $('#Eval_c_default').prop('checked', data.record.IsDefault).change();
                                                    $('#Eval_c_sendSMS').prop('checked', data.record.IsSendSMS).change();

                                                    if (data.record.Icon)
                                                        $('.img-avatar').attr('src', `${Global.Data.folder}${data.record.Icon}`);
                                                });
                                                return text;
                                            }
                                        },
                                        Delete: {
                                            title: ' ',
                                            width: "3%",
                                            sorting: false,
                                            display: function (data) {
                                                var text = $('<button title="Xóa" class="jtable-command-button jtable-delete-command-button"><span>Xóa</span></button>');
                                                text.click(function () {
                                                    GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                                        DeleteChild(data.record.Id);
                                                    }, function () { }, 'Đồng ý', 'Hủy bỏ', 'Thông báo');
                                                });
                                                return text;
                                            }
                                        }
                                    }
                                }, function (data) { //opened handler
                                    data.childTable.jtable('load');
                                });
                        });
                        div.append($img);


                        var btnEdit = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue"  ></i>');
                        btnEdit.click(function () {
                            $('#p-id').val(parent.record.Id);
                            $('#p-name').val(parent.record.Name);
                            $('#p-note').val(parent.record.Note);
                            $('#p-index').val(parent.record.Index);
                        });
                        div.append(btnEdit);

                        var btnDelete = $('<i title="Xóa" class="fa fa-trash-o clickable red i-delete"></i>');
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
        $('#' + Global.Element.Jtable).jtable('load', { 'keyword': $('#keyword').val() });
    }

    //#region parent
    function Save() {
        var obj = {
            Id: $('#p-id').val(),
            Name: $('#p-name').val(),
            Note: $('#p-note').val(),
            Index: $('#p-index').val(),
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

    function Delete(Id) {
        $.ajax({
            url: Global.UrlAction.Delete,
            type: 'POST',
            data: JSON.stringify({ 'Id': Id }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                GlobalCommon.CallbackProcess(data, function () {
                    if (data.Result == "OK") {
                        ReloadList();
                    }
                    else
                        GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra trong quá trình sử lý.");
                }, false, Global.Element.PopupEvaluate, true, true, function () {

                    var msg = GlobalCommon.GetErrorMessage(data);
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra.");
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
                Save();
            }
        });
        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            $('#p-id').val(0);
            $('#p-name').val('');
            $('#p-note').val('');
            $('#p-index').val('');
        });
    }

    function CheckValidate() {
        if ($('#p-name').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập tên đánh giá.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#p-index').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự hiển thị.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }
    //#endregion

    //#region child
    function SaveChild() {
        var obj = {
            Id: $('#Eval_c_id').val(),
            Name: $('#Eval_c_name').val(),
            Note: $('#Eval_c_note').val(),
            Index: $('#Eval_c_index').val(),
            SmsContent: $('#SmsContent').val(),
            EvaluateId: Global.Data.ParentId,
            IsDefault: $("#Eval_c_default").prop('checked'),//.data("kendoMobileSwitch").check(),
            IsSendSMS: $("#Eval_c_sendSMS").prop('checked'),
            Image: $('#p-file-upload').attr('newurl')
        }
        $.ajax({
            url: Global.UrlAction.Save_c,
            type: 'post',
            data: ko.toJSON(obj),
            contentType: 'application/json',
            beforeSend: function () { $('#loading').show(); },
            success: function (result) {
                $('#loading').hide();
                GlobalCommon.CallbackProcess(result, function () {
                    if (result.Result == "OK") {
                        ReloadList();
                        $("#" + Global.Element.Popup_c + ' button[cancel]').click();
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

    function DeleteChild(Id) {
        $.ajax({
            url: Global.UrlAction.Delete_c,
            type: 'POST',
            data: JSON.stringify({ 'Id': Id }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                GlobalCommon.CallbackProcess(data, function () {
                    if (data.Result == "OK") {
                        ReloadList();
                    }
                    else
                        GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra trong quá trình sử lý.");
                }, false, Global.Element.PopupEvaluate, true, true, function () {

                    var msg = GlobalCommon.GetErrorMessage(data);
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra.");
                });
            }
        });
    }

    function InitPopupChild() {
        $("#" + Global.Element.Popup_c).modal({
            keyboard: false,
            show: false
        });
        $("#" + Global.Element.Popup_c + ' button[save]').click(function () {
            if (CheckValidateChild()) {
                if ($('#p-file-upload').val() != '')
                    UploadPicture("p-file-upload", 'p-file-upload');
                else
                    SaveChild();
            }
        });
        $("#" + Global.Element.Popup_c + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup_c).modal("hide");
            $('#Eval_c_id').val(0);
            $('#Eval_c_name').val('');
            $('#Eval_c_note').val('');
            $('#Eval_c_pictureuploader').val('');
            $('#Eval_c_hid_avatar').val('');
            $('#SmsContent').val('');
            $('#Eval_c_index').val(0);
            $('#Eval_c_default').prop('checked', true);
            $('#Eval_c_sendSMS').prop('checked', false).change();

            $('.img-avatar').attr('src', '/Areas/Admin/Content/images/no-image.png');
            $('#p-file-upload').attr('newurl', '');
            $('#p-file-upload').val('');
        });
    }

    function CheckValidateChild() {
        if ($('#Eval_c_name').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập tên đánh giá.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#Eval_c_index').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự hiển thị.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }
    //#endregion
}
$(document).ready(function () {
    var Evaluate = new GPRO.Evaluate();
    Evaluate.Init();
})










