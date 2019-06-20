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
          // Get: '/Report/GetDailyReport'   //co su dung qms  theo nghiep vu
            Get: '/Report/GetDailyReport_NotUseQMS'     // ko su dung qms theo nv
        },
        Data: {
            firstLoad: true
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () { 
        RegisterEvent(); 
    }

    var RegisterEvent = function () {
        Get();
         //setInterval(function () { Get(); },1000)
    } 
    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: '',   
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var str = '<tr><td colspan="5">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '';
                    $.each(data, function (i, item) {
                        if (Global.Data.firstLoad && i == 0) { 
                            var tr = $('<tr></tr>');
                            tr.append('<td>STT</td>');
                            tr.append('<td>NHÂN VIÊN</td>'); 
                            $.each(item.Details, function (ii, child) {
                                tr.append('<td>' + child.Name + '</td>');
                            })
                            $('#tb_export thead').empty().append(tr);
                            Global.Data.firstLoad = false;
                        }

                        str += '<tr>';
                        str += ' <td  >' + (i+1) + '</td> ';
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
            }
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
});