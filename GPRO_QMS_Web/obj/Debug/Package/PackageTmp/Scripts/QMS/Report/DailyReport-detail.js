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
GPRO.namespace('EvaluateReport');
GPRO.EvaluateReport = function () {
    var Global = {
        UrlAction: {
            Get: '/Report/GetDailyReport_details',  //co su dung qms  theo nghiep vu
            Get_: '/Report/GetDailyReport_NotUseQMS'     // ko su dung qms theo nv
        },
        Data: {
            firstLoad: true,
            useQMS: true,
            reportForUser: false
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        $('.datepicker').datepicker({ format: 'dd/mm/yyyy', defaultDate: new Date(), setDefaultDate: true });
        RegisterEvent();
        $('#filter-type').change();

    }

    var RegisterEvent = function () {
        Get();
        //  setInterval(function () { Get(); }, 2000)

        $('#export-excel').click(() => {
            window.location.href = '/Report/Excel_Dgia_Ctiet?useQMS=' + Global.Data.useQMS + '&fromDate=' + $('#fromDate').val() + '&toDate=' + $('#toDate').val();
        });

        $('#filter-type').change(() => {
            Global.Data.firstLoad = true;
            Global.Data.reportForUser = ($('#filter-type').val() == '0' ? true : false);
        });
    }
    function Get() {
        var url = Global.UrlAction.Get_;
        if (Global.Data.useQMS)
            url = Global.UrlAction.Get;
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify({ 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var str = '<tr><td colspan="8">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '';
                    $.each(data, function (i, item) {
                        str += '<tr>';
                        str += ' <td  >' + item.Number + '</td> ';
                        str += ' <td  >' + item.UserName + '</td> ';
                        str += ' <td  >' + item.ServiceName + '</td> ';
                        str += ' <td  >' + moment(item.PrintTime).format('DD/MM/YYYY h:mm:ss a') + '</td> ';
                        for (var i = 1; i <= 4; i++) {
                            str += '<td>' + (('1_' + i) == item.Score ? "<i class='fa fa-check'/>" : "") + '</td>';
                        }
                        str += '<tr>';
                    });
                }
                $('#tb_export tbody').empty().html(str);
            }
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.EvaluateReport();
    home.Init();
    M.updateTextFields();
});