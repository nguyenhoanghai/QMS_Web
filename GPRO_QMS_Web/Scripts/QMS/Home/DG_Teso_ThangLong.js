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
        Data:{
            IconUrl: $("#user").attr('imgfolder')
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        //  $('#currentNumber, #danhgia_box').css('height', $(window).height());
        GetEvaluate();
        RegisterEvent();
    }
    this.DanhGia = function (value) {
        danhgia(value);
    }


    var RegisterEvent = function () {
        setInterval(function () { Get(); }, 2000);
    }

    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data == '-1') {
                    window.location.href = "/DangNhap/Login";
                }
                else {
                    var arr = data.split(',');
                    $("#num").html(arr[0]);
                    //if (arr[1] == '1') {
                    //    $('#currentNumber').hide();
                    //    $('#danhgia_box').show();
                    //}
                    //else {
                    //    $('#currentNumber').show();
                    //    $('#danhgia_box').hide();
                    //}

                    // $('#currentNumber').show();
                    if ($('#camon').css('display') == 'none')
                        $('#danhgia_box').show();
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
                var bt = '';
                if (data.Data != null && data.Data.length > 0) {
                    str = '';                     
                        str += '<div class="child_title">' + data.Data[0].Name + '</div>';
                        if (data.Data[0].Childs != null && data.Data[0].Childs.length > 0) {
                           // bt += '<div class="child_ct">';
                            $.each(data.Data[0].Childs, function (ii, citem) { 
                                bt += '<button  class="btn col-md-4 image-' + ii + '" name="dg_' + data.Data[0].Id + '" onclick="DanhGia(\'' + data.Data[0].Id + '_' + citem.Id + '\')"  value="' + citem.Id + '"  style="background-image: url(' + (Global.Data.IconUrl + citem.Icon) + ') !important;"> <span class="button-text">' + citem.Name + '</span></button>';
                            });
                            bt += ' <div class="clearfix"></div>';
                        } 
                }
                $('.ct').html(str);
                $('#button').html(bt);
            }
        });
    }

    function danhgia(value) {
        $.ajax({
            url: '/Home/Evaluate',
            type: 'POST',
            data: JSON.stringify({ 'name': $('#user').val(), 'value': value, 'num': $('#num').html(), 'isUseQMS': $('#user').attr('isUseQMS') }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                $('#camon').removeClass("hide");
                $('#camon').addClass("show");



                $('#danhgia_box').hide();
                $('#camon').css("display", 'block !important');
                var inter = setInterval(function () {
                    $('#danhgia_box').show();
                    $('#camon').css("display", 'none !important');
                }, 10000);
            }
        });
    }

}
