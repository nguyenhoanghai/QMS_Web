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
            Report: '/ThongKe/Excel',
            GetObj: '/ThongKe/GetObj',
        },
        Data: {
            NV: 'STT,số phiếu,nhân viên,nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ',
            NgVu: 'STT,số phiếu,nhân viên,quầy,nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ',
            DV: 'STT,số phiếu,dịch vụ,giờ lấy phiếu,trạng thái',
            DV_TT: 'STT,số phiếu,dịch vụ,giờ lấy phiếu,Giờ bắt đầu giao dịch,Giờ kết thúc,Giờ thu ngân gọi KH,Thời gian giao dịch,Thời gian chờ trước sửa chữa (phút),Thời gian sau trước sửa chữa (phút),trạng thái',
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
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
            contentType: 'application/json charset=utf-8',
            success: function (objs) {
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
            success: function (objs) {
                $('#view').attr({ 'disabled': false });
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
                tr.append('<td>' + (item.PrintTime != null ? parseDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.Start != null ? parseDate(item.Start) : "") + '</td');
                tr.append('<td>' + (item.End != null ? parseDate(item.End) : "") + '</td');
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
                tr.append('<td>' + (item.CounterName != null ? item.CounterName : "") + '</td');
                tr.append('<td>' + (item.MajorName != null ? item.MajorName : "") + '</td');
                tr.append('<td>' + (item.PrintTime != null ? parseDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.Start != null ? parseDate(item.Start) : "") + '</td');
                tr.append('<td>' + (item.End != null ? parseDate(item.End) : "") + '</td');
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
                tr.append('<td>' + (item.PrintTime != null ? parseDate(item.PrintTime) : "") + '</td');
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
                tr.append('<td>' + (item.PrintTime != null ? parseDate(item.PrintTime) : "") + '</td');
                tr.append('<td>' + (item.str_Start != null ?  item.str_Start  : "") + '</td');
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
}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
});