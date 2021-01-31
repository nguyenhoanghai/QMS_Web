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
GPRO.namespace('ThongTinKH');
GPRO.ThongTinKH = function () {
    var Global = {
        UrlAction: {
            get: '/Xe_TT/GetCustInfo'
        },
        Data: {
            showinfo: true
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        Get();
       // setInterval(function () { Get(); }, 2000);
    }

    var RegisterEvent = function () {
        $('#audio').on('ended', function () {
            playSound();
        });
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.get,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var video = document.getElementById("video");
                if (data && !data.IsShowVideo) {
                    if (!Global.Data.showinfo)
                        video.pause();
                    $('.div-info').removeClass('hide');
                    $('.div-video').addClass('hide');
                    var obj = JSON.parse(data.JsonKhachHang);
                    console.log(obj.JsonKhachHang);
                    $('#tenkh').html(": " + obj.TenKH);
                    $('#bsx').html(": " + obj.bSX);
                    $('#solan').html(": " + obj.SoLan + ' lần.');
                    $('#ngaysua').html(": " + obj.NgaySua);
                    $('#cua-hang').html(obj.CuaHang);
                    var cvs = obj.CongViecs ? obj.CongViecs.split('|'):[];
                    $('#ds-congviec').empty();
                    for (var i = 0; i < cvs.length; i++) {
                        $('#ds-congviec').append(`<li>${(i + 1)} : ${cvs[i]}</li>`);
                    }
                    Global.Data.showinfo = true;
                }
                else if (data && data.IsShowVideo) {
                    if (Global.Data.showinfo)
                        video.play();
                    $('.div-video').removeClass('hide');
                    $('.div-info').addClass('hide');
                    Global.Data.showinfo = false;
                }
            }
        });
    }

    function playSound() {
        var audioE = document.getElementById('audio');
        if (Global.Data.sounds.length > 0) {
            audioE.src = '/audios/' + Global.Data.sounds[0];
            Global.Data.sounds.splice(0, 1);
            audioE.play();
        }
        else {
            Global.Data.reading = false;
        }
    }
};
$(document).ready(function () {
    var obj = new GPRO.ThongTinKH();
    obj.Init();
});