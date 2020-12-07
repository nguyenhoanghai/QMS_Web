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
GPRO.namespace('GoiSo3');
GPRO.GoiSo3 = function () {
    var Global = {
        UrlAction: {
            get: '/HuuNghi/GetDayInfo_VP_PT'
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
        //  Get();
         setInterval(function () { Get(); }, 1000);
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
            data: JSON.stringify({ 'counters': $('#counterid').val(), 'services': $('#serviceIds').val(), 'userId': $('#userid').val() }), 
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
                  
                setValue(obj.DSChoKham, 'cho-kham');
                setValue(obj.DSQuaLuotKham, 'qua-luot-kham'); 

                if (!Global.Data.Drawed)
                    DrawTable(obj.Details);
                else
                    BindTable(obj.Details);
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

    function DrawTable(objs) {
        var str = '<div class="col m12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            var col = Global.Data.cols;
            Global.Data.rows = objs.length;
            var count = 0;
            str = '';
            var count = 0;
            var obj1 = {};
            for (var i = 0; i < Global.Data.rows; i++) {
                str += '<div class="tb2-content ' + (i % 2 == 0 ? 'row-color' : 'row') + '">';
                for (var z = 1; z <= col; z++) {
                    obj1 = ((count < objs.length) ? { tk: objs[count].TicketNumber, room: objs[count].TableCode, change: objs[count].StartStr } : { tk: '', room: '' });
                    str += '   <div class="col m' + (12 / (col * 2)) + ' s' + (12 / (col * 2)) + ' cell" tk_' + count + '>' + obj1.tk + '</div>';
                    str += '   <div class="col m' + (12 / (col * 2)) + ' s' + (12 / (col * 2)) + ' cell ' + (z == col ? 'cell-last' : '') + '" room_' + count + '>' + obj1.room + '</div>';
                    count++;
                    Global.Data.objs.push(obj1);
                }
                str += ' <div class="clearfix"></div></div>';
            }
            $('[content]').empty().html(str);
            Global.Data.lastRows = 1;
            Global.Data.Drawed = true;

        }
    }

    function BindTable(objs) {
        $.each(objs, function (i, item) {
            $('[tk_' + i + ']').html(objs[i].TicketNumber);
            $('[room_' + i + ']').html(objs[i].room);
            if (Global.Data.objs[i] != null && Global.Data.objs[i].TicketNumber != objs[i].TicketNumber) {
                Global.Data.objs[i].TicketNumber = objs[i].TicketNumber;
                $('[tk_' + i + '],[room_' + i + ']').addClass('doi');
                var inter = setInterval(function () {
                    $('[tk_' + i + '],[room_' + i + ']').removeClass('doi');
                    clearInterval(inter);
                }, 5000);
            }
        });
    }
};
$(document).ready(function () {
    var home = new GPRO.GoiSo3();
    home.Init();
});