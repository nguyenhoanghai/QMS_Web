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
            Get: '/Home/GetNumer',
            GetEvaluate: '/Admin/Evaluate/GetList'
        },
        Data: {
            number: -1
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        GetEvaluate();
        RegisterEvent();
    }

    var RegisterEvent = function () { setInterval(function () { Get(); }, 2000); }

    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data == '-1') {
                    window.location.href = "/DangNhap/Login";
                }
                else {
                    var arr = data.split(','); 
                    $("#num").html(arr[0]);
                    $('#currentNumber').show();
                    $('#danhgia_box').show();

                    if(parseInt(arr[0]) != 0   ){
                        $('#noidung').show();
                        $('#alert').html('').hide();
                    }
                    if (parseInt(arr[0]) == 0&&  Global.Data.number != parseInt(arr[0])) {
                        $('#noidung').hide();
                        $('#alert').html('<marquee direction="left" scrollamount="15" style=" text-transform: uppercase;font-size:54px"> Xin cám ơn.</marquee> ').show();
                    }
                    Global.Data.number  = parseInt(arr[0]);
                }
            }
        });
    }

    function GetEvaluate() {
        $.ajax({
            url: Global.UrlAction.GetEvaluate,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var str = '<div class="child_title">Không có dữ liệu</div>';
                if (data.Data != null && data.Data.length > 0) {
                    str = '';
                    $.each(data.Data, function (i, item) {
                        if (item.Childs != null && item.Childs.length > 0) {
                            str += '<div class="col-md-12" >';
                            $.each(item.Childs, function (ii, citem) {
                                if (ii == 0)
                                    str += '<div class="col-md-5" >';
                                else {
                                    str += '<div class="col-md-7" >';
                                }
                                str += '<div class="nut" onclick="danhgia(\'' + item.Id + '_' + citem.Id + '\')"><span>' + citem.Name + '</span> ';
                                str += '<img src="/Content/' + citem.Icon + '" /></div> ';
                                str += '</div>';
                            });
                            str += '<div class="clearfix"></div></div>';
                        }
                    });
                }
                $('#noidung').html(str);
            }
        });
    }


}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
});