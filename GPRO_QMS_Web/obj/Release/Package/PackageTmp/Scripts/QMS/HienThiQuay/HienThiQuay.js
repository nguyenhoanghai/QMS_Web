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
        },
        Data: {
            rows: 0,

        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
      Get();
     //  setInterval(function () { Get() }, 1000);
        setInterval(function () {
            var date = new Date();
            $('#date').html(date.getDay() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
            $('#time').html(date.getHours() + ":" + (date.getMinutes()) + ":" + date.getSeconds());
        }, 10000);
    }

    var RegisterEvent = function () {

    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.GetDayInfo,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                $('#date').html(obj.Date);
                $('#time').html(obj.Time);
                $('#totalcar').html(obj.TotalCar);
                $('#done').html(obj.TotalCarServed);
                $('#waiting').html(obj.TotalCarWaiting);  
                $('#procesing').html(obj.TotalCarProcessing);

                if (Global.Data.rows != obj.Details.length)
                    DrawTable(obj.Details);
                else
                    BindTable(obj.Details);
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

                str += '<div id="r_' + item.TableId + '"><div class="col-md-2 rowcontent  ">' + item.TableName + '</div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.TicketNumber + '</div>';
                str += '<div class="col-md-4 rowcontent font-dt ">' + item.CarNumber + '</div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.StartStr + '</div>';
                str += '<div class="col-md-2 rowcontent font-dt ">' + item.TimeProcess + '</div> <div class="clearfix"></div></div>';

                count++;
                if (count == 5) {
                    count = 0;
                    str += '</div>';
                }
            });
        }
        $('#vtemslideshow1').html(str);
         if (objs.length > 0)
           RunTicker();
        Global.Data.rows = objs.length;
    }

    function BindTable(objs) { 
        $.each(objs, function (ii, item) {
            if ($('#r_' + item.TableId + ' div').length > 0)
                $.each($('#r_' + item.TableId + ' div'), function (iii, div) {
                    switch (iii) {
                        case 0: $(div).html(item.TableName); break;
                        case 1: $(div).html(item.TicketNumber); break;
                        case 2: $(div).html(item.CarNumber); break;
                        case 3: $(div).html(item.StartStr); break;
                        case 4: $(div).html(item.TimeProcess); break;
                    }
                });
        }); 
    }

}

$(document).ready(function () {
    var home = new GPRO.HienThiQuay();
    home.Init();
});