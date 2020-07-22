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
            Get: '/Report/GetDailyReport' ,  //co su dung qms  theo nghiep vu
            Get_: '/Report/GetDailyReport_NotUseQMS'     // ko su dung qms theo nv
        },
        Data: {
            firstLoad: true,
            useQMS: true,
            reportForUser: true
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
       
       // setInterval(function () { Get(); }, 2000)

        $('#export-excel').click(() => {
            window.location.href = '/Report/Excel_Dgia?useQMS=' + Global.Data.useQMS + '&reportForUser=' + Global.Data.reportForUser + '&fromDate=' + $('#fromDate').val() + '&toDate=' + $('#toDate').val();
        });

        $('#filter-type').change(() => {
            Global.Data.firstLoad = true;
            Global.Data.reportForUser = ($('#filter-type').val() == '0' ? true : false);
        });

        $('#get-excel').click(function () {
 Get();
        })
    }
    function Get() {
        var url = Global.UrlAction.Get_;
        if (Global.Data.useQMS)
            url = Global.UrlAction.Get;
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify({ 'reportForUser': Global.Data.reportForUser, 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val() }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $("#get-excel").attr("disabled", true);},
            success: function (data) {
                $("#get-excel").attr("disabled", false);
                var str = '<tr><td colspan="5">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '';
                    $.each(data, function (i, item) {
                        if (Global.Data.firstLoad && i == 0) {
                            var tr = $('<tr></tr>');
                            tr.append('<td>STT</td>');
                            tr.append('<td>' + (Global.Data.reportForUser ? 'NHÂN VIÊN' : 'DỊCH VỤ') + '</td>');
                            $.each(item.Details, function (ii, child) {
                                tr.append('<td>' + child.Name + '</td>');
                            })
                            $('#tb_export thead').empty().append(tr);
                            Global.Data.firstLoad = false;
                        }

                        str += '<tr>';
                        str += ' <td  >' + (i + 1) + '</td> ';
                        str += ' <td  >' + item.ServiceName + '</td> ';
                        $.each(item.Details, function (ii, child) {
                            str += ' <td  >' + child.Id + '</td> ';
                        })
                        //str += ' <td  >' + item.tc1 + '</td> ';
                        //str += ' <td  >' + item.tc2 + '</td> ';
                        //str += ' <td  >' + item.tc3 + '</td> ';
                        str += '<tr>';
                    });
                }
                $('#tb_export tbody').empty().html(str);
            },
            error: function (err) {
                alert("Dữ liệu trả về quá lớn không thể hiển thị. Vui lòng chọn 'Xuất Báo Cáo' để xem báo cáo trên file excel.");
                console.log(err);
                $("#get-excel").attr("disabled", false);
            }
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
    $('select').formSelect();
});