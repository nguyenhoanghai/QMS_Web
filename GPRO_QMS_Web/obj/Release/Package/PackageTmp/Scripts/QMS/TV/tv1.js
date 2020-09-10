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
GPRO.namespace('TV1');
GPRO.TV1 = function () {
    var Global = {
        UrlAction: {
            GetDayInfo: '/TV/GetTV1Info',
            Save: '/HienThiQuay/SaveBVConfig',
            GetConfig: '/HienThiQuay/GetConfig'
        },
        Data: {
            slideInterval: 1000,
            counters: "1,2,3",
            Drawed: false,
            rows: 1,
            totalPage: 0,


            cols: 1,
            lastRows: 0,

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
        GetConfig();
        //   Get(); 
    }

    var RegisterEvent = function () {
        $('[btSave]').click(function () {
            SaveConfig();
        });
         
    }

    function SaveConfig() {
        var obj = {
            PageTitle: $('#title').val(),
            PageTitle_Css: $('#title_css').val(),
            SubTitle: $('#subText').val(),
            SubTitle_Css: $('#sub_css').val(),
            Row1_Css: $('#css_r1').val(),
            Row2_Css: $('#css_r2').val(),
            background: $('#background').val(),
            Row: $('#row').val(),
            Hight: $('#Hight').val(),
            SlideInterval: $('#SlideInterval').val(),
            css_box_video: $('#css_box_video').val(),
            counters: $('#counters').val(),
        }
        var str = JSON.stringify(obj);

        $.ajax({
            url: Global.UrlAction.Save,
            type: 'POST',
            data: JSON.stringify({ 'configStr': str, 'pageType': 'TV1' }),
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
            data: JSON.stringify({ 'counters': Global.Data.counters }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (!Global.Data.Drawed)
                    DrawTable(data);
                else
                    BindTable(data);
            }
        });
    }
    
    function GetConfig() {
        $.ajax({
            url: Global.UrlAction.GetConfig,
            type: 'POST',
            data: JSON.stringify({ 'pageType': 'TV1' }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var obj = JSON.parse(data);
                if (obj != null) {
                    var css = '.title-page {' + obj.PageTitle_Css + '  }';
                    css += '.sub-title{' + obj.SubTitle_Css + ' }';
                    css += '.cell {' + obj.Row1_Css + '  }';
                    css += '.row-color .cell {' + obj.Row2_Css + ' }';
                    css += '.video-box{' + obj.css_box_video + '  }';
                    css += '.page-setting{background:' + obj.background + ';height:' + obj.Hight + 'px }'

                    $("style").append(css);

                    $('#row').val(obj.Row);
                    Global.Data.rows = parseInt(obj.Row);
                    $('#column').val(obj.Col);
                    Global.Data.cols = parseInt(obj.Col);

                    $('.title-page').html(obj.PageTitle);
                    $('#title').val(obj.PageTitle);
                    $('#title_css').val(obj.PageTitle_Css);
                    $('#subText').val(obj.SubTitle);
                    $('#sub_css').val(obj.SubTitle_Css);
                    obj.SubTitle.split('|').map((item, i) => {
                        $('.t' + i).html(item);
                    });

                    $('#css_r1').val(obj.Row1_Css);
                    $('#css_r2').val(obj.Row2_Css);
                    $('#SlideInterval').val(obj.SlideInterval);
                    Global.Data.slideInterval = parseInt(obj.SlideInterval);
                    $('#background').val(obj.background);
                    $('#Hight').val(obj.Hight);
                    $('#css_box_video').val(obj.css_box_video);
                    $('#counters').val(obj.counters);
                    Global.Data.counters = obj.counters;

                    $('.materialize-textarea').trigger('autoresize');
                    M.updateTextFields();
                    setSlideInterval();
                    Get();
                }
            }
        });
    }

    function DrawTable(objs) {
        var container = $('.slide-container');
        if (objs == null || objs.length == 0)
            container.html(' <div class="mySlides "><div class="col m12 rowcontent"> Không có dữ liệu </div></div>');
        else {
            var str = '';
            var page = Math.ceil(objs.length / Global.Data.rows);
            Global.Data.totalPage = page;
            var index = 0;
            for (var i = 0; i < page; i++) {
                str += `<div class="mySlides ">
                            <div class="thead row">
                                <div class="col col m3 sub-title t0">Bàn Nâng</div>
                                <div class="col col m2 sub-title t1">Số thẻ</div>
                                <div class="col col m3 sub-title t2">số phiếu</div>
                                <div class="col col m4 sub-title t3">thời gian còn lại</div>
                                <div class="clearfix"></div>
                            </div>`;
                for (var ii = 0; ii < Global.Data.rows; ii++) {
                    var item = objs[index];
                    if (item != null) {
                        str += '<div class="tbody row ' + (ii % 2 == 0 ? " row-color" : "") + '" >';
                        str += '<div class="col col m3 cell">' + item.TenQuay + '</div>';
                        str += '<div class="col col m2 cell r' + item.CounterId + '_stt">' + (item.STT == '' || item.STT == '0' ? '-' : item.STT) + '</div>';
                        str += '<div class="col col m3 cell r' + item.CounterId + '_stt_3">' + (item.STT_3 == '' || item.STT_3 == '0' ? '-' : item.STT_3) + '</div>';
                        str += '<div class="col col m4 cell r' + item.CounterId + '_status">' + getStatus(item) + '</div>';
                        str += `<div class="clearfix"></div>
                            </div >`;
                    }
                    index++;
                }
                str += `  </div>`;
            }
            container.empty().html(str);
            Global.Data.Drawed = true;
            //chay slide
            currentSlide(1);
            plusSlides(0);
            //var intel = setInterval(() => {
            //    plusSlides(0);
            //    clearInterval(intel);
            //}, Global.Data.slideInterval);
            //chay get du lieu
             setInterval(function () {
                  if (Global.Data.Drawed)
                      Get();
             }, 1500);

        }
    }

    function BindTable(objs) {
        $.each(objs, function (i, item) {
            $('.r' + item.CounterId + '_stt').html((item.STT == '' || item.STT == '0' ? '-' : item.STT));
            $('.r' + item.CounterId + '_stt_3').html((item.STT_3 == '' || item.STT_3 == '0' ? '-' : item.STT_3));
            $('.r' + item.CounterId + '_status').html(getStatus(item));
            //$('[room_' + i + ']').html(objs[i].room);
            //if (Global.Data.objs[i] != null && Global.Data.objs[i].TicketNumber != objs[i].TicketNumber) {
            //    Global.Data.objs[i].TicketNumber = objs[i].TicketNumber;
            //    $('[tk_' + i + '],[room_' + i + ']').addClass('doi');
            //    var inter = setInterval(function () {
            //        $('[tk_' + i + '],[room_' + i + ']').removeClass('doi');
            //        clearInterval(inter);
            //    }, 5000);
            //}
        });
    }

    function getStatus(item) {
        var str = '';
        switch (item.TrangThai) {
            case 'Process': str = (item.TGConLai.Hours < 10 ? '0' + item.TGConLai.Hours : item.TGConLai.Hours) + ':' + (item.TGConLai.Minutes < 10 ? '0' + item.TGConLai.Minutes : item.TGConLai.Minutes); break;
            case 'Complete': str = 'Hoàn thành'; break;
            case 'Over': str = 'Phát sinh'; break;
            default: str = "..:.."; break;
        }
        return str;
    }
}
 