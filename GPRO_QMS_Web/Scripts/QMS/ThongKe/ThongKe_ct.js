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
GPRO.namespace('Home');
GPRO.Home = function () {
    var Global = {
        UrlAction: {
            Get: '/ThongKe/GetReport',
            GetJtableData: '/ThongKe/GetJtableData',
            Report: '/ThongKe/Excel',
            GetObj: '/ThongKe/GetObj',
        },
        Data: {
            NV: 'STT,số phiếu,nhân viên,nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ',
            NgVu: 'STT,số phiếu,nhân viên, nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ',
            DV: 'STT,số phiếu,dịch vụ,giờ lấy phiếu,trạng thái',
            DV_TT: 'STT,số phiếu,dịch vụ,giờ lấy phiếu,Giờ bắt đầu giao dịch,Giờ kết thúc,Giờ thu ngân gọi KH,Thời gian giao dịch,Thời gian chờ trước sửa chữa,Thời gian chờ sau sửa chữa,trạng thái',
        },
        Element: {
            Jtable: 'jtable-view'
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        //InitJtable();
        $('#cb_option').change();
    }

    var RegisterEvent = function () {
        $("#from,#to").kendoDateTimePicker({
            format: "dd/MM/yyyy HH:mm",
            value: new Date()
        });

        $('#view').click(function () {
            $('#view').attr({ 'disabled': true });
            Get();
        });


        $('#export').click(function () {
            window.location.href = Global.UrlAction.Report + '?objId=' + parseInt($('#cb_obj').val()) + '&&typeOfSearch=' + parseInt($('#cb_option').val()) + '&&from=' + $("#from").val() + '&&to=' + $("#to").val() + '&&thungan=' + $('#box-date').attr('thungan')
        });

        $('#cb_option').change(function () {
            GetObj($(this).val());
            switch ($(this).val()) {
                case '1': DrawHeader(Global.Data.NV.split(',')); break;
                case '2': DrawHeader(Global.Data.NgVu.split(',')); break;
                case '3': DrawHeader(Global.Data.DV.split(',')); break;
                case '4': DrawHeader(Global.Data.DV_TT.split(',')); break;
            }
        });
    }

    function GetObj(type) {
        $.ajax({
            url: Global.UrlAction.GetObj,
            type: 'POST',
            data: JSON.stringify({ 'type': type }),
            beforeSend: function () { $('#loading').show(); },
            contentType: 'application/json charset=utf-8',
            success: function (objs) {
                $('#loading').hide();
                var cb = $('#cb_obj');
                cb.empty();
                $.each(objs, function (i, item) {
                    cb.append('<option value="' + item.Id + '">' + item.Name + '</option');
                });
            }
        });
    }

    function DrawHeader(titles) {
        var tb = $('#tb_export thead');
        tb.empty();
        if (titles != null && titles.length > 0) {
            var tr = $('<tr></tr>');
            $.each(titles, function (i, item) {
                tr.append('<td>' + item + '</td');
            });
            tb.append(tr);
        }
        $('#tb_export tbody').empty().append('<tr><td colspan="' + $($('#tb_export thead tr')[0]).find('td').length + '">Không có dữ liệu</td></tr>')
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: JSON.stringify({ 'objId': parseInt($('#cb_obj').val()), 'typeOfSearch': parseInt($('#cb_option').val()), 'from': $("#from").data("kendoDateTimePicker").value(), 'to': $("#to").data("kendoDateTimePicker").value(), 'thungan': parseInt($('#box-date').attr('thungan')) }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $('#loading').show(); },
            success: function (strObjs) {
                $('#loading').hide();
                $('#view').attr({ 'disabled': false });
                var objs = JSON.parse(strObjs);
                switch ($('#cb_option').val()) {
                    case '1': BindNhanVien(objs); break;
                    case '2': BindNghiepVu(objs); break;
                    case '3': BindDichVu(objs); break;
                    case '4': BindDichVu_TT(objs); break;
                }
            }
        });
    }

    function BindNhanVien(objs) {
        var tb = $('#tb_export tbody');
        tb.empty();
        if (objs != null && objs.length > 0) {
            $.each(objs, function (i, item) {
                var tr = $('<tr></tr>');
                tr.append('<td>' + item.stt + '</td');
                tr.append('<td>' + item.Number + '</td');
                tr.append('<td>' + (item.UserName != null ? item.UserName : "") + '</td');
                tr.append('<td>' + (item.MajorName != null ? item.MajorName : "") + '</td');
                tr.append('<td>' + (item.PrintTime != null ? getDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.Start != null ? getDate(item.Start) : "") + '</td');
                tr.append('<td>' + (item.End != null ? getDate(item.End) : "") + '</td');
                tr.append('<td>' + (item.ProcessTime != null ? item.ProcessTime : "") + '</td');
                tr.append('<td>' + (item.WaitingTime != null ? item.WaitingTime : "") + '</td');
                tb.append(tr);
            });
        }
        else
            tb.append('<tr><td colspan="' + $($('#tb_export thead tr')[0]).find('td').length + '">Không có dữ liệu</td></tr>')
    }

    function BindNghiepVu(objs) {
        var tb = $('#tb_export tbody');
        tb.empty();
        if (objs != null && objs.length > 0) {
            $.each(objs, function (i, item) {
                var tr = $('<tr></tr>');
                tr.append('<td>' + item.stt + '</td');
                tr.append('<td>' + item.Number + '</td');
                tr.append('<td>' + (item.UserName != null ? item.UserName : "") + '</td');
                //tr.append('<td>' + (item.CounterName != null ? item.CounterName : "") + '</td');
                tr.append('<td>' + (item.MajorName != null ? item.MajorName : "") + '</td');
                tr.append('<td>' + (item.PrintTime != null ? getDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.Start != null ? getDate(item.Start) : "") + '</td');
                tr.append('<td>' + (item.End != null ? getDate(item.End) : "") + '</td');
                tr.append('<td>' + (item.ProcessTime != null ? item.ProcessTime : "") + '</td');
                tr.append('<td>' + (item.WaitingTime != null ? item.WaitingTime : "") + '</td');
                tb.append(tr);
            });
        }
        else
            tb.append('<tr><td colspan="' + $($('#tb_export thead tr')[0]).find('td').length + '">Không có dữ liệu</td></tr>')
    }

    function BindDichVu(objs) {
        var tb = $('#tb_export tbody');
        tb.empty();
        if (objs != null && objs.length > 0) {
            $.each(objs, function (i, item) {
                var tr = $('<tr></tr>');
                tr.append('<td>' + item.stt + '</td');
                tr.append('<td>' + item.Number + '</td');
                tr.append('<td>' + (item.ServiceName != null ? item.ServiceName : "") + '</td');
                tr.append('<td>' + (item.PrintTime != null ? getDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.StatusName != null ? item.StatusName : "") + '</td');
                tb.append(tr);
            });
        }
        else
            tb.append('<tr><td colspan="' + $($('#tb_export thead tr')[0]).find('td').length + '">Không có dữ liệu</td></tr>')
    }

    function BindDichVu_TT(objs) {
        var tb = $('#tb_export tbody');
        tb.empty();
        if (objs != null && objs.length > 0) {
            $.each(objs, function (i, item) {
                var tr = $('<tr></tr>');
                tr.append('<td>' + item.stt + '</td');
                tr.append('<td>' + item.Number + '</td');
                tr.append('<td>' + (item.ServiceName != null ? item.ServiceName : "") + '</td');
                tr.append('<td>' + (item.PrintTime != null ? getDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.str_Start != null ? item.str_Start : "") + '</td');
                tr.append('<td>' + (item.str_End != null ? item.str_End : "") + '</td');
                tr.append('<td>' + (item.str_StartTN != null ? item.str_StartTN : "") + '</td');
                tr.append('<td>' + (item.ProcessTime != null ? item.ProcessTime : "") + '</td');
                tr.append('<td>' + (item.WaitingTime != null ? item.WaitingTime : "") + '</td');
                tr.append('<td>' + (item.WaitingTimeTN != null ? item.WaitingTimeTN : "") + '</td');
                tr.append('<td>' + (item.StatusName != null ? item.StatusName : "") + '</td');
                tb.append(tr);
            });
        }
        else
            tb.append('<tr><td colspan="' + $($('#tb_export thead tr')[0]).find('td').length + '">Không có dữ liệu</td></tr>')
    }

    function parseDate(jsonDate) {
        if (jsonDate != null && jsonDate != '') {
            var dateString = jsonDate.substr(6);
            var date = new Date(parseInt(dateString));
            return (date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes());
        }
        else {
            return null;
        }
    }



    function InitJtable() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Thống kê chi tiết giao dịch',
            paging: true,
            pageSize: 500,
            pageSizeChange: true,
            sorting: true,
            selectShow: false,
            actions: {
                listAction: Global.UrlAction.GetList,
                //createAction: Global.Element.Popup,
            },
            messages: {
                // addNewRecord: 'Thêm mới',
                // selectShow: 'Ẩn hiện cột'
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
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                stt: {
                    visibility: 'fixed',
                    title: "STT",
                    width: "5%",
                },
                Number: {
                    visibility: 'fixed',
                    title: "Số phiếu",
                    width: "5%",
                },
                ServiceName: {
                    title: "Dịch vụ",
                    width: "15%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + getString(data.record.ServiceName) + '</span>';
                        return txt;
                    }
                },
                ServiceName: {
                    title: "Giờ lấy phiếu",
                    width: "15%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + getDate(data.record.ServiceName) + '</span>';
                        return txt;
                    }
                },
                StatusName: {
                    title: "Trạng thái",
                    width: "10%",
                    display: function (data) {
                        var txt = ""
                        txt = '<span class="">' + getString(data.record.StatusName) + '</span>';
                        return txt;
                    }
                }
            }
        });
    }

    function ReloadJtable() {
        $('#' + Global.Element.Jtable).jtable('load', { 'objId': parseInt($('#cb_obj').val()), 'typeOfSearch': parseInt($('#cb_option').val()), 'from': $("#from").data("kendoDateTimePicker").value(), 'to': $("#to").data("kendoDateTimePicker").value(), 'thungan': parseInt($('#box-date').attr('thungan')), 'keyword': $('#search-key').val() });
    }

    function getString(value) {
        if (value)
            return value;
        return '';
    }
    function getDate(value) {
        if (value)
            return moment(value).format("DD/MM/YYYY hh:mm");
        return '';
    }
}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
});