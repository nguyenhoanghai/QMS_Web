﻿if (typeof GPRO == 'undefined' || !GPRO) {
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
GPRO.namespace('RHM_DT');
GPRO.RHM_DT = function () {
    var Global = {
        UrlAction: {
            GetDayInfo: '/BVRangHamMat/GetDayInfo_BV',
            Save: '/HienThiQuay/SaveBVConfig',
            GetConfig: '/HienThiQuay/GetConfig'
        },
        Data: {
            rows: 1,
            cols: 1,
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
    }

    var RegisterEvent = function () {
        $('[btSave]').click(function () {
            SaveConfig();
        });

        $('[runmaquee]').click(function () { document.getElementsByTagName('marquee').start(); });

        $(".marquee").marquee();
    }
    function getData() {
        //Get();
        var e = setInterval(function () { Get(); }, 2000);
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
            Services: '',
            SerName_Col1_Css: '',
            SerName_Col2_Css: '',
            Called_Col1_Css: '',
            Called_Col2_Css: '',
            CounterIds: $('#counterIds').val(),
            ServiceIds: $('#serviceIds').val(),
            TimerLat: $('#timer_lat').val()
        }
        var str = JSON.stringify(obj);

        $.ajax({
            url: Global.UrlAction.Save,
            type: 'POST',
            data: JSON.stringify({
                'configStr': str, 'pageType': "RHM_DT"
            }),
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
            data: JSON.stringify({ 'counters': $('#counterIds').val(), 'services': $('#serviceIds').val(), 'serType': 2 }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
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
            data: JSON.stringify({ 'pageType': 'RHM_DT' }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj != null) {
                    var css = '.title {' + obj.PageTitle_Css + '  }';
                    css += '.sub-title{' + obj.SubTitle_Css + ' }';
                    css += '.cell {' + obj.Row1_Css + '  }';
                    css += '.row-color .cell {' + obj.Row2_Css + ' }';
                    css += '.marquee{' + obj.Adv_Css + '  }';
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
                    $('#counterIds').val(obj.CounterIds);
                    $('#serviceIds').val(obj.ServiceIds);
                    $('#timer_lat').val(obj.TimerLat);
                }

                getData();
            }
        });
    }

    function DrawTable(objs) {
        var arrWidth = ["10%", "15%", "20%", "10%", "15%", "10%", "20%"];

        var str = '<div class="col-md-12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            var col = Global.Data.cols;
            var count = 0;
            str = '';
            var count = 0;
            var obj1 = {};
            //for (var i = 0; i < Global.Data.rows; i++) {
            //    str += '<div class="' + (i % 2 == 0 ? 'row-color' : 'row') + '">';
            //    for (var z = 1; z <= col; z++) {
            //        obj1 = ((count < objs.length) ? { tk: objs[count].TicketNumber, room: objs[count].TableCode, change: objs[count].StartStr } : { tk: '', room: '' });
            //        str += '   <div style="width:' + arrWidth[0] + '" class=" cell" tk_' + count + '>' + objs[count].TableName + '</div>';
            //        str += '   <div style="width:' + arrWidth[1] + '" class=" cell" bs_' + count + '>' + objs[count].UserName + '</div>';
            //        str += '   <div style="width:' + arrWidth[2] + '" class=" cell" bn_' + count + '>' + objs[count].TenBN + '</div>';
            //        str += '   <div style="width:' + arrWidth[3] +'" class=" cell" ns_' + count + '>' + objs[count].NamSinh + '</div>';
            //        str += '   <div style="width:' + arrWidth[4] +'" class=" cell" kg_' + count + '>' + objs[count].KhungGio + '</div>';
            //        str += '   <div style="width:' + arrWidth[5] + '" class=" cell" glp_' + count + '>' + objs[count].GioLaySo + '</div>';
            //        str += '   <div style="width:' + arrWidth[6] +'" class="col-md-' + (12 / (col * 2)) + ' col-sm-' + (12 / (col * 2)) + ' cell ' + (z == col ? 'cell-last' : '') + '" room_' + count + '>' + objs[count].Note + '</div>';
            //        count++;
            //        Global.Data.objs.push(obj1);
            //    }
            //    str += ' <div class="clearfix"></div></div>';
            //}
            //$('[content]').empty().html(str);


            var str = '<div class="col m12 rowcontent"> Không có dữ liệu </div>';
            if (objs.length > 0) {
                str = '';
                var count = 0;
                $.each(objs, function (i, item) {
                    if (count == 0)
                        str += '<div  class="slide-image" style=" height:100% ; width:100%" class="col m12">';

                    obj1 = ((count < objs.length) ? { tk: item.TicketNumber, room: item.TableCode, change: item.StartStr } : { tk: '', room: '' });
                    str += '<div id="r_' + item.TableId + '" class="' + (i % 2 == 0 ? 'row-color' : 'row') + '">   <div style="width:' + arrWidth[0] + '" class=" cell" tk_' + i + '>' + item.TableName + '</div>';
                    str += '   <div style="width:' + arrWidth[1] + '" class=" cell" bs_' + i + '>' + item.UserName + '</div>';
                    str += '   <div style="width:' + arrWidth[2] + '" class=" cell" bn_' + i + '>' + item.TenBN + '</div>';
                    str += '   <div style="width:' + arrWidth[3] + '" class=" cell" ns_' + i + '>' + item.NamSinh + '</div>';
                    str += '   <div style="width:' + arrWidth[4] + '" class=" cell" kg_' + i + '>' + item.KhungGio + '</div>';
                    str += '   <div style="width:' + arrWidth[5] + '" class=" cell" glp_' + i + '>' + item.GioLaySo + '</div>';
                    str += '   <div style="width:' + arrWidth[6] + '" class="col-md-' + (12 / (col * 2)) + ' col-sm-' + (12 / (col * 2)) + ' cell  " room_' + i + '>' + item.Note + '</div><div class="clearfix"></div></div>';

                    Global.Data.objs.push(obj1);

                    count++;
                    if (count == Global.Data.rows) {
                        count = 0;
                        str += '</div>';
                    }
                });
            }
            $('#vtemslideshow1').html(str);

            Global.Data.lastRows = 1;
            Global.Data.Drawed = true;

            if (objs.length > 0 && objs.length > Global.Data.rows)
                RunTicker();
        }
    }

    function BindTable(objs) {
        $.each(objs, function (i, item) {
            $('[bn_' + i + ']').html(getValue(objs[i].TenBN));
            $('[bs_' + i + ']').html(getValue(objs[i].UserName));
            $('[ns_' + i + ']').html((objs[i].NamSinh == 0 ? "---" : objs[i].NamSinh));
            $('[kg_' + i + ']').html(getValue(objs[i].KhungGio));
            $('[glp_' + i + ']').html(getValue(objs[i].GioLaySo));

            $('[tk_' + i + ']').html(objs[i].TableName);
            $('[room_' + i + ']').html(objs[i].room);
            if (Global.Data.objs[i] != null && Global.Data.objs[i].TicketNumber != objs[i].TicketNumber) {
                Global.Data.objs[i].TicketNumber = objs[i].TicketNumber;
                $('[tk_' + i + '],[room_' + i + '],[bn_' + i + '],[bs_' + i + '],[ns_' + i + '],[kg_' + i + '],[glp_' + i + ']').addClass('doi');
                var inter = setInterval(function () {
                    $('[tk_' + i + '],[room_' + i + '],[bn_' + i + '],[bs_' + i + '],[ns_' + i + '],[kg_' + i + '],[glp_' + i + ']').removeClass('doi');
                    clearInterval(inter);
                }, 5000);
            }
        });
    }

    function getValue(value) {
        return (value == null || value == 'null' ? "---" : value);
    }

    function RunTicker() {
        $('#vtemslideshow1').cycle({
            fx: 'blindY,blindY',
            timeout: parseInt($('#timer_lat').val()),
            speed: 1000,
            next: '#cycle_next',
            prev: '#cycle_prev',
            pager: '#vtemnav',
            pagerEvent: 'click',
            startingSlide: 0,
            fit: true,
            height: 550,
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.RHM_DT();
    home.Init();
});