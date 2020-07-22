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
        $('#currentNumber, #danhgia_box').css('height', $(window).height());
        GetEvaluate();
        RegisterEvent();
    }

    var RegisterEvent = function () {

    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                switch (data) {
                    case -1:
                        window.location.href = "/DangNhap/Login";
                        break;
                    case 0:
                        $('#currentNumber').hide();
                        break;
                    default:
                        $("#num").html(data);
                        break;
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
                        str += '<div class="child_title">' + (i + 1) + '. ' + item.Name + '</div>';
                        if (item.Childs != null && item.Childs.length > 0) {
                            str += '<div class="child_ct">';
                            $.each(item.Childs, function (ii, citem) {
                                if (citem.IsDefault)
                                    str += '<input type="radio" checked  name="dg_' + item.Id + '" value="' + citem.Id + '" />' + citem.Name;
                                else
                                    str += '<input type="radio"   name="dg_' + item.Id + '" value="' + citem.Id + '" />' + citem.Name;

                            });
                            str += '</div>';
                        }
                    });
                }
                $('.ct').html(str);
            }
        });
    }
 }

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();

 
});