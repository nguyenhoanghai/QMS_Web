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
            Get: '/BCDanhGia/Xe_GetNSPhucVu_ChiTietTungPhieu'
        },
        Data: {
            firstLoad: true,
            useQMS: true
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

        $('#export-excel').click(() => {
            window.location.href = '/BCDanhGia/Xe_GetNSPhucVu_ChiTietTungPhieu_Excel?from=' + $('#fromDate').val() + '&to=' + $('#toDate').val();
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
        var url = Global.UrlAction.Get;
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify({ 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'type': $('#cb_option').val() }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $("#get-excel").attr("disabled", true); },
            success: function (data) {
                $("#get-excel").attr("disabled", false);
                var str = '<tr><td colspan="11">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '';
                    $.each(data, function (i, item) {
                        str += '<tr>';
                        str += ' <td  >' + (i + 1) + '</td> ';
                        str += ' <td  >' + item.STT_PhongKham + '</td> ';
                        str += ' <td  >' + item.ServiceName + '</td> ';

                        str += ' <td  >' + hhmmss(item.Start) + '</td> ';
                        str += ' <td  >' + hhmmss(item.TongTGChoTB) + '</td> ';
                        str += ' <td  >' + hhmmss(item.ServeTime) + '</td> ';
                        str += ' <td  >' + hhmmss(item.TGXuLyTT) + '</td> ';
                        str += ' <td  >' + hhmmss(item.TGChoTruocSC) + '</td> ';
                        str += ' <td  >' + hhmmss(item.TGChoSauSC) + '</td> ';
                        str += ' <td  >' + item.UserName + '</td> ';
                        str += ' <td  >' + (item.PhatSinh ? "Có" : "Không") + '</td> ';
                        str += '<tr>';
                    });
                }
                $('#tb_export tbody').empty().html(str);
            },
            error: function (err) {
                alert("Dữ liệu trả về quá lớn không thể hiển thị. Vui lòng chọn 'Xuất Báo Cáo' để xem báo cáo trên file excel.");
                console.log(err);
            }
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
    $('select').formSelect();
});