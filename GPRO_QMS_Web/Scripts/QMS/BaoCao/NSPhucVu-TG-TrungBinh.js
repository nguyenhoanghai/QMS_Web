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
            Get: '/BCDanhGia/Xe_GetNSPhucVu_TGTrungBinh'    
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
            window.location.href = '/BCDanhGia/Xe_GetNSPhucVu_TGTrungBinh_Excel?from=' + $('#fromDate').val() + '&to=' + $('#toDate').val() + '&type=' + $('#cb_option').val();
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
                var str = '<tr><td colspan="5">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '';
                    $.each(data, function (i, item) {
                        if (Global.Data.firstLoad && i == 0) {
                            $('#type-title').html(($('#cb_option').val() == '1' ? 'NHÂN VIÊN' : 'DỊCH VỤ'))
                            Global.Data.firstLoad = false;
                        }

                        str += '<tr>';
                        str += ' <td  >' + (i + 1) + '</td> ';
                        str += ' <td  >' + ($('#cb_option').val() == '1' ? item.UserName : item.ServiceName) + '</td> ';
                        str += ' <td  >' + item.Number + '</td> ';
                        str += ' <td  >' +  (item.dTongTGCho) + '</td> ';
                        str += ' <td  >' +  (item.dTGChoTruocSC) + '</td> ';
                        str += ' <td  >' +  (item.dTGXuLyTT) + '</td> ';
                        str += ' <td  >' +  (item.dTGChoSauSC) + '</td> ';
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