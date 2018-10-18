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
            Get: '/ThongKe/GetReportTH',
            Report: '/ThongKe/ExcelTH',
            GetObj: '/ThongKe/GetObj',
        },
        Data: {
            NV: 'stt,đối tượng,số lượt giao dịch,TỔNG THỜI GIAN GIAO DỊCH(PHÚT),THỜI GIAN GIAO DỊCH TRUNG BÌNH (PHÚT/GD)',
            DV: 'stt,đối tượng,số lượt giao dịch,TỔNG THỜI GIAN GIAO DỊCH(PHÚT),THỜI GIAN GIAO DỊCH TRUNG BÌNH (PHÚT/GD),Thời gian chờ trước sửa chữa (phút/gd),Thời gian chờ sau sửa chữa (phút/gd)',
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
    }

    var RegisterEvent = function () {
        $("#from,#to").kendoDateTimePicker({
            format: "dd/MM/yyyy HH:mm",
            value: new Date()
        });

        $('#view').click(function () {
            Get();
            $('#view').attr({ 'disabled': true });
        });


        $('#export').click(function () {
            window.location.href = Global.UrlAction.Report + '?typeOfSearch=' + parseInt($('#cb_option').val()) + '&&from=' + $("#from").val() + '&&to=' + $("#to").val() + '&&thungan=' + $('#box-date').attr('thungan')
        });

        $('#cb_option').change(function () {
            switch ($(this).val()) {
                case '1':
                case '2':
                case '3': DrawHeader(Global.Data.NV.split(',')); break;
                case '4': DrawHeader(Global.Data.DV.split(',')); break;
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
            data: JSON.stringify({ 'typeOfSearch': parseInt($('#cb_option').val()), 'from': $("#from").data("kendoDateTimePicker").value(), 'to': $("#to").data("kendoDateTimePicker").value(), 'thungan': parseInt($('#box-date').attr('thungan')) }),
            contentType: 'application/json charset=utf-8',
            success: function (objs) {
                $('#view').attr({ 'disabled': false });
              
                switch ($('#cb_option').val()) {
                    case '1':
                    case '2':
                    case '3': BindNhanVien(objs); break;
                    case '4': BindDichVu(objs); break;
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
                tr.append('<td>' + item.Index + '</td');
                tr.append('<td>' + item.Name + '</td');
                tr.append('<td>' + item.TotalTransaction + '</td');
                tr.append('<td>' + item.TotalTransTime + '</td');
                tr.append('<td>' + item.AverageTimePerTrans + '</td');
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
                tr.append('<td>' + (i+1) + '</td');
                tr.append('<td>' + item.Name + '</td');
                tr.append('<td>' + item.TotalTransaction + '</td');
                tr.append('<td>' + item.TotalTransTime + '</td');
                tr.append('<td>' + item.AverageTimePerTrans + '</td');
                tr.append('<td>' + item.AverageTimeWaitingBeforePerTrans + '</td');
                tr.append('<td>' + item.AverageTimeWaitingAfterPerTrans + '</td');
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
            return (date.getDay() + "/" + date.getMonth() + "/" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes());
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