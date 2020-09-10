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
GPRO.namespace('GoiSo1');
GPRO.GoiSo1 = function () {
    var Global = {
        UrlAction: {
            get: '/HuuNghi/GetDayInfo'
        },
        Data: {
            rows: 1,
            cols: 1,
            lastRows: 0,
            Drawed: false,
            objs: [],
            sounds: [],
            index: 0,
            reading: false,
            first: true
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        Get();
       // setInterval(function () { Get(); }, 1000);
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
            data: JSON.stringify({ 'GetKL': true, 'counters': $('#counterid').val()}),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj.Sounds != null && obj.Sounds.length > 0)
                    $.each(obj.Sounds, (i, item) => {
                        Global.Data.sounds = Global.Data.sounds.concat(item.split('|'));
                    })

                if (Global.Data.sounds.length > 0 && !Global.Data.reading) {
                    Global.Data.reading = true;
                    playSound();
                }    
                
                if ($('.num-stt-dangkham').html() == obj.STTDangKham.toString()) {
                    $('.box-dk').addClass('doi');
                    var inter = setInterval(function () {
                        $('.box-dk').removeClass('doi');
                        clearInterval(inter);
                    }, 5000);
                }
                $('.num-stt-dangkham').html(obj.STTDangKham);

                if ($('.num-stt-ketluan').html() == obj.STTDangKham.toString()) {
                    $('.box-kl').addClass('doi');
                    var inter = setInterval(function () {
                        $('.box-kl').removeClass('doi');
                        clearInterval(inter);
                    }, 5000);
                } 
                $('.num-stt-ketluan').html(obj.STTDangKetLuan);
                setValue(obj.DSChoKham, 'cho-kham');
                setValue(obj.DSQuaLuotKham, 'qua-luot-kham');
                setValue(obj.DSChoKL, 'cho-ket-luan');
                setValue(obj.DSQuaLuotKL, 'qua-luot-ket-luan');
            }
        });
    }

    function setValue(objs, tableName) {
        for (var i = 0; i < parseInt($('#rows').val()); i++) {
            var cols = $('.' + tableName + ' .r' + i + ' div');
            var item = objs[i];
            if (item != null) {
                $(cols[0]).html(item.Name);
                $(cols[1]).html(item.Code);
                $(cols[2]).html(item.Data);
            }
            else {
                $(cols[0]).html('');
                $(cols[1]).html('');
                $(cols[2]).html('');
            }
        }
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
    var home = new GPRO.GoiSo1();
    home.Init();
});