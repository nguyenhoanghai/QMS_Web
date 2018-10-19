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
GPRO.namespace('BenhVien');
GPRO.BenhVien = function () {
    var Global = {
        UrlAction: {
            GetDayInfo: '/HienThiQuay/GetDayInfo_BV',
            Save: '/HienThiQuay/SaveBVConfig',
            GetConfig: '/HienThiQuay/GetConfig'
        },
        Data: {
            rows: 1,
            cols:1,
            lastRows: 0,
            Drawed: false,
            objs: []
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {

        RegisterEvent();
        GetConfig();
        Get();
        var e = setInterval(function () { Get(); }, 500);
        $('.marquee').marquee();
    }

    var RegisterEvent = function () {
        $('[btSave]').click(function () {
            SaveConfig();
        });

        $('[runmaquee]').click(function () { document.getElementsByTagName('marquee').start(); });

        $(".marquee").marquee();
    }
    function SaveConfig() {
        var obj = {
            PageTitle: $('#title').val(),
            PageTitle_Css: $('#title_css').val(),
            SubTitle: $('#subText').val(),
            SubTitle_Css: $('#sub_css').val(),
            Row1_Css: $('#css_r1').val(),
            Row2_Css: $('#css_r2').val(),
            Adv_Css: $('#run_css').val(),
            Row: $('#row').val(),
            Col: $('#column').val(),
            Services:'',
            SerName_Col1_Css: '',
            SerName_Col2_Css:'',
            Called_Col1_Css: '',
            Called_Col2_Css: '',
            css_box_info: $('#css_box_info').val(),
            css_box_video: $('#css_box_video').val(),
        }
        var str = JSON.stringify(obj);

        $.ajax({
            url: Global.UrlAction.Save,
            type: 'POST',
            data: JSON.stringify({ 'configStr': str, 'pageType': 3 }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data == "OK")
                    location.reload();
                else
                    alert(data);
            }
        });
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.GetDayInfo,
            type: 'POST',
            data: JSON.stringify({ 'counters': $('#fullscreen').attr('counters'), 'services': '1' }),
            contentType: 'application/json charset=utf-8',
            success: function (data ) {
                var objs = JSON.parse(data);
                // if (!Global.Data.Drawed || (Global.Data.lastRows != obj.Details.length))
                if (!Global.Data.Drawed)
                    DrawTable(objs.Details);
                else
                    BindTable(objs.Details);
            }
        });
    }

    function GetConfig() {
        $.ajax({
            url: Global.UrlAction.GetConfig,
            type: 'POST',
            data: JSON.stringify({ 'pageType': '3' }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj != null) { 
                    var css ='.title {'+obj.PageTitle_Css+'  }';
                    css +='.sub-title{'+obj.SubTitle_Css+' }'; 
                    css +='.cell {'+obj.Row1_Css+'  }'; 
                    css +='.row-color .cell {'+obj.Row2_Css+' }';
                    css += '.marquee{' + obj.Adv_Css  + '  }';
                    css += '.left-box{' + obj.css_box_info + '  }';
                    css += '.right-box{' + obj.css_box_video + '  }';

                    $("style").append(css);

                    $('#row').val(obj.Row);
                    Global.Data.rows = parseInt(obj.Row);
                    $('#column').val(obj.Col);
                    Global.Data.cols = parseInt(obj.Col);

                    $('.title').html(obj.PageTitle);
                    $('#title').val(obj.PageTitle);
                    $('#title_css').val(obj.PageTitle_Css);
                    $('#subText').val(obj.SubTitle);
                    $('#sub_css').val(obj.SubTitle_Css);
                    $('#css_r1').val(obj.Row1_Css);
                    $('#css_r2').val(obj.Row2_Css);
                    $('#run_css').val(obj.Adv_Css);
                    $('#css_box_info').val(obj.css_box_info);
                    $('#css_box_video').val(obj.css_box_video);

                    $('.materialize-textarea').trigger('autoresize');
                    Materialize.updateTextFields();
                }
            }
        });
    }

    function DrawTable(objs) {
        var str = '<div class="col m12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            var col = Global.Data.cols;
            var count = 0;
            str = '';
            var count = 0;
            var obj1 = {};
            for (var i = 0; i < Global.Data.rows; i++) {
                str += '<div class="' + (i % 2 == 0 ? 'row-color' : 'row') + '">';
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
}

$(document).ready(function () {
    var home = new GPRO.BenhVien();
    home.Init();
});