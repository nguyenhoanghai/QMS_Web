
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
GPRO.namespace('Service');
GPRO.Service = function () {
    var Global = {
        UrlAction: {
            Gets: '/Admin/Service/Gets',
            Delete: '/Admin/Service/Delete',
            Save: '/Admin/Service/Save'
        },
        Element: {
            Jtable: 'jtable-service',
            Popup: 'popup'
        },
        Data: {
            Id: 0
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
        $('.bs-timepicker').timepicker();

        $('#auto-end').change(() => {
            if ($('#auto-end').prop('checked'))
                $('#time-process-auto').prop('disabled', false);
            else
                $('#time-process-auto').prop('disabled', true);
        })
    }


    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách dịch vụ',
            paging: true,
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: true,
            actions: {
                listAction: Global.UrlAction.Gets,
                createAction: Global.Element.Popup,
                searchAction: Global.Element.PopupSearch,
            },
            messages: {
                addNewRecord: 'Thêm mới',
                searchRecord: 'Tìm kiếm',
                selectShow: 'Ẩn hiện cột'
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
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                Code: {
                    visibility: 'fixed',
                    title: "Mã dịch vụ",
                    width: "5%",
                    headerClass: 'text-left'
                },
                Name: {
                    title: "Tên dịch vụ",
                    width: "15%",
                    headerClass: 'text-left'
                },
                StartNumber: {
                    title: "STT B.Đầu",
                    width: "5%",
                    columnClass: 'text-center',
                },
                EndNumber: {
                    title: "STT K.Thúc",
                    width: "5%",
                    columnClass: 'text-center',
                },
                TimeProcess: {
                    title: "TG xử lý (hh:mm)",
                    width: "5%",
                    columnClass: 'text-center',
                    display: function (data) {
                        return `<span>${HHmm(data.record.TimeProcess)}</span>`;
                    }
                },
                AutoEnd: {
                    title: "Tự kết thúc",
                    width: "7%",
                    columnClass: 'text-center',
                    display: function (data) {
                        var text = '';
                        if (data.record.AutoEnd)
                            text = $('<i class="fa fa-check-square-o"  ></i> ');
                        else
                            text = $('<i class="fa fa-square-o" ></i> ');
                        return text;
                    }
                },
                TimeAutoEnd: {
                    title: "TG tự kết thúc (hh:mm)",
                    width: "5%",
                    columnClass: 'text-center',
                    display: function (data) {
                        return `<span>${HHmm(data.record.TimeAutoEnd)}</span>`;
                    }
                },
                Note: {
                    title: "Ghi chú",
                    width: "30%",
                    sorting: false,
                    headerClass: 'text-left'
                },
                IsActived: {
                    title: "TT",
                    width: "3%",
                    columnClass: 'text-center',
                    display: function (data) {
                        var text = '';
                        if (data.record.IsActived)
                            text = $('<i class="fa fa-check-square-o"  ></i> ');
                        else
                            text = $('<i class="fa fa-square-o" ></i> ');
                        return text;
                    }
                },
                actions: {
                    title: '',
                    width: "3%",
                    sorting: false,
                    columnClass: 'text-center',
                    display: function (data) {
                        var div = $('<div></div>');
                        var btnEdit = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Chỉnh sửa" class="fa fa-pencil-square-o clickable blue"  ></i>');
                        btnEdit.click(function () {
                            $('#active').prop('checked', data.record.IsActived).change();
                            $('#auto-end').prop('checked', data.record.AutoEnd).change();
                            if (data.record.AutoEnd) {
                                $('#time-process-auto').prop('disabled', false);
                                $('#time-process-auto').val(HHmm(data.record.TimeAutoEnd));
                            }
                            $('#id').val(data.record.Id);
                            $('#code').val(data.record.Code);
                            $('#name').val(data.record.Name);
                            $('#start-number').val(data.record.StartNumber);
                            $('#end-number').val(data.record.EndNumber);
                            $('#time-process').val(HHmm(data.record.TimeProcess));
                            $('#note').val(data.record.Note);
                        });
                        div.append(btnEdit);

                        var btnDelete = $('<i title="Xóa" class="fa fa-trash red i-delete clickable"></i>');
                        btnDelete.click(function () {
                            GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                Delete(data.record.Id);
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

    function Save() {
        var obj = {
            Id: $('#id').val(),
            Code: $('#code').val(),
            Name: $('#name').val(),
            StartNumber: parseInt($('#start-number').val()),
            EndNumber: parseInt($('#end-number').val()),
            TimeProcess: new Date(`2020-01-01 ${$('#time-process').val()}:00`),
            IsActived: $('#active').prop('checked'),
            AutoEnd: $('#auto-end').prop('checked'),
            TimeAutoEnd: null,
            Note: $('#note').val(),
        }
        if (obj.AutoEnd)
            obj.TimeAutoEnd = new Date(`2020-01-01 ${$('#time-process-auto').val()}:00`);
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
                Save();
            }
        });
        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            resetForm();
        });
    }

    function CheckValidate() {
        if ($('#code').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập mã dịch vụ.", function () { $('#code').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#name').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập tên dịch vụ.", function () { $('#name').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#start-number').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự bắt đầu.", function () { $('#start-number').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#end-number').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự kết thúc.", function () { $('#end-number').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if (parseInt($('#start-number').val()) > parseInt($('#end-number').val())) {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập số thứ tự kết thúc lớn hơn số thứ tự bắt đầu.", function () { $('#end-number').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#time-process').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập thời gian xử lý.", function () { $('#time-process').focus() }, "Lỗi Nhập liệu");
            return false;
        }

        else if ($('#auto-end').prop('checked') && $('#time-process-auto').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập thời gian tự kết thúc.", function () { $('#time-process-auto').focus() }, "Lỗi Nhập liệu");
            return false;
        }

        else
            return true;
    }

    resetForm = () => {
        $('#active').prop('checked', true).change();
        $('#auto-end').prop('checked', false).change();
        $('#time-process-auto').val('');
        $('#id').val(0);
        $('#code').val('');
        $('#name').val('');
        $('#start-number').val(1);
        $('#end-number').val(1);
        $('#time-process').val('');
        $('#note').val('');
    }

}
$(document).ready(function () {
    var obj = new GPRO.Service();
    obj.Init();
})