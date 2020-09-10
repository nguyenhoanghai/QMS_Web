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
GPRO.namespace('ReceiverSMS');
GPRO.ReceiverSMS = function () {
    var Global = {
        UrlAction: {
            GetList: '/ReceiverSMS/GetList',
            Save: '/ReceiverSMS/Save',
            Delete: '/ReceiverSMS/Delete',
            getUsers:'/User/GetUserSelect'
        },
        Element: {
            Jtable: 'jtableSMS',
            Popup: 'popup',
            Search: 'Search',
        },
        Data: {
            Model: {},
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        GetUsers();
        InitList();
        ReloadList();

        InitSearchPopup();
        InitPopup();
      
    }

    var RegisterEvent = function () {
    }

    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách số điện thoại nhận tin nhắn đánh giá',
            paging: true,
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: true,
            actions: {
                listAction: Global.UrlAction.GetList,
                createAction: Global.Element.Popup,
                searchAction: Global.Element.Search,
            },
            messages: {
                addNewRecord: 'Thêm mới',
                searchRecord: 'Tìm kiếm',
                selectShow: 'Ẩn hiện cột'
            },
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                PhoneNumber: {
                    visibility: 'fixed',
                    title: "Số điện thoại",
                    width: "20%"
                },
                IsActive: {
                    title: "Sử dụng",
                    width: "10%",
                    display: function (data) {
                        var text = '';
                        if (!data.record.IsActive)
                            text = $('<i class="fa fa-square-o" style="font-size:26px"></i> ');
                        else
                            text = $('<i class="fa fa-check-square-o blue"  style="font-size:26px"></i> ');
                        return text;
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
                        var text = $('<i data-toggle="modal" href="#' + Global.Element.Popup + '" title="Chỉnh sửa thông tin" class="fa fa-pencil-square-o clickable blue modal-trigger"  ></i>');
                        text.click(function () {
                            $('#Id').val(data.record.Id);
                            $('#PhoneNumber').val(data.record.PhoneNumber);
                            $('#isActive').prop('checked', data.record.IsActive);
                            $('#txtNote').val(data.record.Note);
                            if (data.record.UserIds != null)
                                $("#txtUserIds").data("kendoMultiSelect").value(data.record.UserIds.split(','));
                            else
                                $("#txtUserIds").data("kendoMultiSelect").value('');
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
     

    function Save() { 
        var obj = {
            Id: $('#Id').val(),
            PhoneNumber: $('#PhoneNumber').val(),
            IsActive: $('#isActive').prop('checked'), 
            Note: $('#txtNote').val(),
            UserIds: $("#txtUserIds").data("kendoMultiSelect").value().toString()
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
                }, false, Global.Element.PopupReceiverSMS, true, true, function () {

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
            $("#txtUserIds").data("kendoMultiSelect").value('');
            $('#Id').val(0);
            $('#PhoneNumber').val('');
            $('#isActive').prop('checked', true);
            $('#txtNote').val('');
            $("#txtUserIds").data("kendoMultiSelect").value('');
        });
    }

    function CheckValidate() {
        if ($('#phonenumber').val() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số điện thoại.", function () { }, "Lỗi Nhập liệu");
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

    function GetUsers() { 
        $.ajax({
            url: Global.UrlAction.getUsers,
            type: 'post',
            contentType: 'application/json',
            beforeSend: function () { $('#loading').show(); },
            success: function (result) {
                $('#loading').hide();
                GlobalCommon.CallbackProcess(result, function () {
                    if (result.Result == "OK") {
                        var strOpt = '<option value=0>Không có dữ liệu nhân viên</option>';
                        if (result.Data.length > 0) {
                            strOpt = '';
                            $.each(result.Data, function(i, item)  {
                                strOpt += '<option value="' + item.Id + '">' + item.Name + '</option>'
                            });
                        }
                        $("#txtUserIds").empty().html(strOpt).kendoMultiSelect().data("kendoMultiSelect");
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

}
$(document).ready(function () {
    var ReceiverSMS = new GPRO.ReceiverSMS();
    ReceiverSMS.Init();

})