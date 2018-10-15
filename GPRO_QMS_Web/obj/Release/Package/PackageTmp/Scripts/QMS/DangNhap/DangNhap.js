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
GPRO.namespace('DangNhap');
GPRO.DangNhap = function () {
    var Global = {
        UrlAction: {
            Get: '/DangNhap/Get',
        },
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent(); 
    }

    var RegisterEvent = function () {
        $('[btn="Login"]').click(function () {
            Get();
        })
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: JSON.stringify({ 'UserName': $('#username').val(), 'Password': $('#password').val() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data.Result == 'OK')
                    window.location.href = "/Danh-Gia";
                else
                    alert("Thông tin đăng nhập không đúng");
            }
        });
    }
     
}

$(document).ready(function () {
    var home = new GPRO.DangNhap();
    home.Init();
});