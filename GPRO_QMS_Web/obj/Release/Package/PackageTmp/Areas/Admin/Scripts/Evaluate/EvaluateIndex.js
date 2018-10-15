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
            Avartar:''
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        InitList();
        ReloadList();

        InitSearchPopup();
        InitPopup();
        InitPopupChild();

        BindParent(null);
    }

    var RegisterEvent = function () {
        $('#Eval_c_hid_avatar').change(function () {
            Global.Data.Avartar = $('#Eval_c_hid_avatar').val();
            SaveChild();
        });
    }

    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách tiêu chí đánh giá',
            paging: true,
            pageSize: 25,
            pageSizeChange: true,
            sorting: true,
            selectShow: true,
            actions: {
                listAction: Global.UrlAction.GetList,
                createAction: Global.Element.Popup,
                createObjDefault: InitParentModel(null),
                searchAction: Global.Element.Search,
            },
            messages: {
                addNewRecord: 'Thêm mới',
                searchRecord: 'Tìm kiếm',
                selectShow: 'Ẩn hiện cột'
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
                    title: "Số TT",
                    width: "5%",
                },
                Name: {
                    visibility: 'fixed',
                    title: "Tiêu chí đánh giá",
                    width: "20%"
                },
                Detail: {
                    title: 'Thang đánh giá',
                    width: '3%',
                    sorting: false,
                    edit: false,
                    display: function (parent) {
                        var $img = $('<i class="fa fa-list-ol clickable red aaa" title="Click Xem thang đánh giá ' + parent.record.Name + '"></i>');
                        $img.click(function () {
                            Global.Data.ParentId = parent.record.Id;
                            $('#' + Global.Element.Jtable).jtable('openChildTable',
                                    $img.closest('tr'),
                                    {
                                        title: '<span class="red">Danh sách thang đánh giá : ' + parent.record.Name + '</span>',
                                        paging: true,
                                        pageSize: 10,
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
                                            },
                                            Name: {
                                                title: "Tên",
                                                width: "10%",
                                            },
                                            IsDefault: {
                                                title: "Mặc định",
                                                width: "10%",
                                                display: function (data) {
                                                    var text = '';
                                                    if (!data.record.IsDefault)
                                                        text = $('<i class="fa fa-square-o" style="font-size:26px"></i> ');
                                                    else
                                                        text = $('<i class="fa fa-check-square-o blue"  style="font-size:26px"></i> ');
                                                    return text;
                                                }
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
                                                display: function (data) {
                                                    var text = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup_c + '" title="Chỉnh sửa thông tin" class="fa fa_tb fa-pencil-square-o clickable blue"  ></i>');
                                                    text.click(function () {
                                                        $('#Eval_c_id').val(data.record.Id);
                                                        $('#Eval_c_name').val(data.record.Name);
                                                        $('#Eval_c_note').val(data.record.Note);
                                                        $('#Eval_c_index').val(data.record.Index);
                                                        Global.Data.Avartar = data.record.Icon; 
                                                        $('#Eval_c_default').attr('checked', data.record.IsDefault);
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
                        return $img;
                    }
                },
                Note: {
                    title: "Ghi chú",
                    width: "50%",
                    sorting: false
                },
                edit: {
                    title: '',
                    width: '1%',
                    sorting: false,
                    display: function (data) {
                        var text = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue"  ></i>');
                        text.click(function () {
                            BindParent(data.record);
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
        $('#' + Global.Element.Jtable).jtable('load', { 'keyword': $('#keyword').val() });
    }

    function InitParentModel(obj) {
        var ViewModel = {
            Id: 0,
            Name: '',
            Note: '',
            Index: 0,
        };
        if (obj != null) {
            ViewModel = {
                Id: ko.observable(obj.Id),
                Name: ko.observable(obj.Name),
                Note: ko.observable(obj.Note),
                Index: ko.observable(obj.Index),
            };
        }
        return ViewModel;
    }

    function BindParent(obj) {
        Global.Data.Parent = InitParentModel(obj);
        ko.applyBindings(Global.Data.Parent, document.getElementById(Global.Element.Popup));
    }

    function Save() {
        $.ajax({
            url: Global.UrlAction.Save,
            type: 'post',
            data: ko.toJSON(Global.Data.Parent),
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
            BindParent(null);
        });
    }

    function CheckValidate() {
        if ($('#name').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập tên đánh giá.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#index').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự hiển thị.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }


    function InitSearchPopup() {
        $('#' + Global.Element.Search + ' button[search]').click(function () {
            ReloadList();
        });

        $('#' + Global.Element.Search + ' button[close]').click(function () {
            $("#" + Global.Element.Search).modal("hide");
        });
    }
 


    function SaveChild() {
        var obj = {
            Id: $('#Eval_c_id').val(),
            Name: $('#Eval_c_name').val(),
            Note: $('#Eval_c_note').val(),
            Index: $('#Eval_c_index').val(),
            EvaluateId:  Global.Data.ParentId,
            IsDefault: $('#Eval_c_default').prop('checked'),
            Icon:  Global.Data.Avartar
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
                if ($('#Eval_c_pictureuploader').val() != '')
                    UploadPicture('Eval_c_pictureuploader', 'Eval_c_hid_avatar');
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
            $('#Eval_c_index').val(0); 
            Global.Data.Avartar = '';
            $('#Eval_c_default').attr('checked', true);

            
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
    
}
$(document).ready(function () {
    var Evaluate = new GPRO.Evaluate();
    Evaluate.Init();
})










