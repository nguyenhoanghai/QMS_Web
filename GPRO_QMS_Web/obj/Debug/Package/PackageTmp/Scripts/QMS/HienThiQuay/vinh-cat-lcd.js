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
GPRO.namespace('HienThiQuay');
GPRO.HienThiQuay = function () {
    var Global = {
        UrlAction: {
            GetDayInfo: '/HienThiQuay/GetDayInfo',
            GetDateTime: '/Home/GetDateTime',
            GetTime: '/Home/GetDateTime'
        },
        Data: {
            rows: 0,
            tableIds: [],
            hub: $.connection.chatHub
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        InitHub();
        RegisterEvent();
        Get();
        setInterval(function () { GetTime(); }, 100);
    }

    var RegisterEvent = function () {
        setInterval(function () { Get(); }, 1000);
    }

    function InitHub() {
        Global.Data.hub.client.sendDateTimeToPage = function (data) {
            var dateTime = data.split('|');
            $('#date').html(dateTime[0]);
            $('#time').html(dateTime[1]);
        };

        Global.Data.hub.client.sendDayInfoToPage = function (obj) {
            $('#totalcar').html(obj.TotalCar);
            $('#done').html(obj.TotalCarServed);
            $('#waiting').html(obj.TotalCarWaiting);
            $('#process').html(obj.TotalCarProcessing);

            if ($('#table2').attr('config') == 'ticker') {
                $('#ticker').show();
                if (Global.Data.rows != obj.Details.length)
                    DrawTicker(obj.Details);
                else
                    BindTicker(obj.Details);
            }
            else {
                $('#table2').show();
                if (Global.Data.rows != obj.Details.length)
                    DrawTable(obj.Details);
                else
                    BindTable(obj.Details);
            }
        };

        $.connection.hub.start().done(function () {

        });
    }

    function GetTime() {
        $.ajax({
            url: Global.UrlAction.GetTime,
            type: 'POST',
            success: function () { }
        });
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.GetDayInfo,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                // $('#date').html(obj.Date);
                //  $('#time').html(obj.Time);
                $('#totalcar').html(obj.TotalCar);
                $('#done').html(obj.TotalCarServed);
                $('#waiting').html(obj.TotalCarWaiting);
                $('#procesing').html(obj.TotalCarProcessing);
                if ($('#table2').attr('config') == 'ticker') {
                    $('#ticker').show();
                    if (Global.Data.rows != obj.Details.length)
                        DrawTicker(obj.Details);
                    else
                        BindTicker(obj.Details);
                }
                else {
                    $('#table2').show();
                    if (Global.Data.rows != obj.Details.length)
                        DrawTable(obj.Details);
                    else
                        BindTable(obj.Details);
                }
            }
        });
    }

    function DrawTable(objs) {
        var str = '<div class="col-md-12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            str = '';
            var count = 0;
            $.each(objs, function (i, item) {
                if (count == 0)
                    str += '<div  class="slide-image" style=" height:100% ; width:100%" class="col-md-12">';

                str += '<div id="r_' + item.TableId + '"><div class="col-md-2 rowcontent r-text ">' + item.TableName + '</div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.TicketNumber + '</div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.GioGiaoDK + '</div>';
                str += '<div class="col-md-2 rowcontent   ">Sửa chữa</div>';
                str += '<div class="col-md-2 rowcontent font-dt "> <div class="progress"><div class="progress-bar progress-bar-striped progress-bar-animated bg-info" role="progressbar" aria-valuenow="' + 70 + '"  aria-valuemin="0" aria-valuemax="100" style="width:' + 70 + '%">100%</div></div></div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.strTimeCL + '</div> <div class="clearfix"></div></div>';

                count++;
                if (count == parseInt($('#table2').attr('rows'))) {
                    count = 0;
                    str += '</div>';
                }
            });
        }
        $('#vtemslideshow1').html(str);
        if (objs.length > 0 && objs.length > parseInt($('#table2').attr('rows')))
            RunTicker();
        Global.Data.rows = objs.length;

        //$('#ticker li').css('height', $('#ticker').height() / 4);
        $('#table2 .font-dt').css('cssText', $('#table2 .font-dt').attr('style') + ' ; font-size : ' + $('#table2').attr('size-dt') + 'px !important; line-height : ' + $('#table2').attr('size-dt') + 'px !important');
        $('#table2 .r-text').css('cssText', $('#table2 .r-text').attr('style') + ' ; font-size : ' + $('#table2').attr('size-text') + 'px !important; line-height : ' + $('#table2').attr('size-dt') + 'px !important');

    }

    function BindTable(objs) {
        $.each(objs, function (ii, item) {
            if ($('#r_' + item.TableId + ' div').length > 0)
                $.each($('#r_' + item.TableId + ' div'), function (iii, div) {
                    switch (iii) {
                        case 0: $(div).html(item.TableName); break;
                        case 1: $(div).html(item.TicketNumber); break;
                        case 2: $(div).html(item.GioGiaoDK); break;
                        case 3:
                            if (item.TicketNumber != '0')
                                $(div).html('Sữa chữa');
                            else
                                $(div).html('');
                            break;
                        case 4:
                            if (item.TicketNumber != '0')
                                $(div).html('<div class="progress"><div class="progress-bar progress-bar-striped progress-bar-animated bg-info" role="progressbar" aria-valuenow="' + item.TienDoTH + '"  aria-valuemin="0" aria-valuemax="100" style="width:' + item.TienDoTH + '%">' + item.TienDoTH + '%</div></div>');
                            else
                                $(div).html('');
                            break;
                        case 7: $(div).html(item.strTimeCL); break;
                    }
                });
        });
    }




    function DrawTicker(objs) {
        var str = '<li col-span="12" class=" rowcontent"> Không có dữ liệu </li>';
        if (objs.length > 0) {
            str = '';
            $.each(objs, function (i, item) {

                str += '<li ><ul id="r_' + item.TableId + '">';

                str += '<li class="col-md-4 rowcontent r-text ">' + item.TableName + '</li>';
                str += '<li class="col-md-2 rowcontent font-dt ">' + item.TicketNumber + '</li>';
                str += '<li class="col-md-3 rowcontent font-dt ">' + item.StartStr + '</li>';
                str += '<li class="col-md-3 rowcontent font-dt ">' + item.TimeProcess + '</li> <div style="clear:left"></div>';

                str += '</ul> <div class="clearfix"></div></li>';

            });
        }
        $('#ticker').html(str);
        if (objs.length > 0 && objs.length > parseInt($('#table2').attr('rows')))
            RunTicker();

        Global.Data.rows = objs.length;
        $('#ticker li').css('height', $('#ticker').height() / parseInt($('#table2').attr('rows')));
        $('#ticker .font-dt').css('cssText', $('#ticker .font-dt').attr('style') + ' ; font-size : ' + $('#table2').attr('size-dt') + 'px !important; line-height : ' + $('#ticker').height() / parseInt($('#table2').attr('rows')) + 'px !important');
        $('#ticker .r-text').css('cssText', $('#ticker .r-text').attr('style') + ' ; font-size : ' + $('#table2').attr('size-text') + 'px !important; line-height : ' + $('#table2').attr('size-dt') + 'px !important');
    }

    function BindTicker(objs) {
        $.each(objs, function (ii, item) {
            if ($('#r_' + item.TableId + ' li').length > 0)
                $.each($('#r_' + item.TableId + ' li'), function (iii, div) {
                    switch (iii) {
                        case 0: $(div).html(item.TableName); break;
                        case 1: $(div).html(item.TicketNumber); break;
                        case 2: $(div).html(item.StartStr); break;
                        case 3: $(div).html(item.TimeProcess); break;
                    }
                });
        });
    }
}

$(document).ready(function () {
    var home = new GPRO.HienThiQuay();
    home.Init();
});