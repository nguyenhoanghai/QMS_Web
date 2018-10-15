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
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
      //  $('#currentNumber, #danhgia_box').css('height', $(window).height());
        GetEvaluate();
        RegisterEvent();
    }

    var RegisterEvent = function () {
        setInterval(function () { Get(); }, 2000);
    }

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
                    //if (arr[1] == '1') {
                    //    $('#currentNumber').hide();
                    //    $('#danhgia_box').show();
                    //}
                    //else {
                    //    $('#currentNumber').show();
                    //    $('#danhgia_box').hide();
                    //}

                    $('#currentNumber').show();
                    $('#danhgia_box').show();
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
                        str += '<div class="i_title"><div style="float:left"><img src="/Content/logo_tienthu.png" /></div><div style="padding-left:110px">' + item.Name + '</div><div style="clear:both"></div></div>';
                        str += "<div><ul>";
                        if (item.Childs != null && item.Childs.length > 0) {
                            $.each(item.Childs, function (ii, citem) {
                                str += '<li><div class="btn" onclick="danhgia(\'' + item.Id + '_' + citem.Id + '\')"><span>' + citem.Name + '</span><div> <img src="/Content/' + citem.Icon + '" /></div><div class="clearfix"></div></div></li>';
                            });
                        }
                        str += "</div></ul>";
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