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
GPRO.namespace('ServeInfo');
GPRO.ServeInfo = function () {
    var Global = {
        UrlAction: {
            GetServices: '/DangKyOnline/GetServices',
            GetServiceInfo: '/DangKyOnline/GetServiceInfo',
            InsertServiceRequire: '/DangKyOnline/InsertServiceRequire',
            Find: '/DangKyOnline/Find',
        },
        Element: {
        },
        Data: {
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        //GetServices();
        GetServiceSelect_bv('dichvu');
        setInterval(function () { Get() }, 1000);
    }

    var RegisterEvent = function () {
        $('#dangky').click(function () {
            if (CheckValidate())
                InsertServiceRequire();
        });

        $('#sodienthoai,#sodienthoai_tracuu').on("keypress", function () {
            return isNumberKey(event);
        });
        $('#xemthongtinphucvu').click(function () {

            Find();
        });

        let date = new Date();
        date.setDate(date.getDate() + 1);

        $("#ngay-dk,#ngay-dk-tc").kendoDatePicker({
            format: "dd/MM/yyyy",
            value: date,
            min: date
        });
    }

    function GetServices() {
        $.ajax({
            url: Global.UrlAction.GetServices,
            type: 'GET',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                console.log(data);
                if (data.Result == "OK")
                    DrawTable(data.Data);
            }
        });
    }


    function Get() {
        $.ajax({
            url: Global.UrlAction.GetServiceInfo,
            type: 'POST',
            data: '',
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data.Result == "OK")
                    DrawTable(data.Data);
            }
        });
    }

    function Find() {
        if ($('#sodienthoai_tracuu').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Số Điện Thoại đăng ký khám.", function () { $('#sodienthoai_tracuu').focus() }, "Lỗi Nhập liệu");
            return false;
        }

        $.ajax({
            url: Global.UrlAction.Find,
            type: 'POST',
            data: JSON.stringify({ 'Phone': $('#sodienthoai_tracuu').val(), 'ngaydk': $('#ngay-dk-tc').val() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data.Result == "OK") {
                    $('#contentView-tracuu').html(data.Data);
                }
                else {
                    $('#contentView-tracuu').html('Không tìm thấy thông tin theo yêu cầu');
                }
            }
        });
    }

    function DrawTable(objs) {
        var tb = $('.tb-info tbody');
        tb.empty();
        var str = '<div class="col-md-12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            str = '';
            $.each(objs, function (i, item) {
                if (i % 2 == 0) {
                    str += '<div class="background-row-even rowcontent"><div class="col-md-4 col-sm-4 col-xs-4 font_28 margin-right bor_bottom border-right">' + item.Name + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt margin-left bor_bottom border-right">' + item.TicketNumberProcessing + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt bor_bottom border-right">' + item.TotalCarsWaiting + '</div><div class="clearfix"></div>';
                }
                else {
                    str += '<div class="background-row-odd rowcontent"><div class="col-md-4 col-sm-4 col-xs-4 font_28 margin-right bor_bottom border-right">' + item.Name + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt margin-left bor_bottom border-right">' + item.TicketNumberProcessing + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt bor_bottom border-right">' + item.TotalCarsWaiting + '</div><div class="clearfix"></div>';
                }

                tb.append(`<tr><td>${item.Name}</td><td>${item.TicketNumberProcessing}</td><td>${item.TotalCarsWaiting}</td></tr>`)
            });
        }
        else {
            tb.append('<tr><td colspan="3">Không có dữ liệu</td></tr>')
        }
        $('#thongtindichvuchitiet').empty().append(str);

    }

    function CheckValidate() {
        if ($('#sodienthoai').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Số Điện Thoại.", function () { $('#sodienthoai').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#dichvu').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn Dịch Vụ.", function () { $('#dichvu').focus() }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }

    function InsertServiceRequire() {
        var obj = {
            TicketNumber: 0,
            ServiceId: $('#dichvu').val(),
            PhoneNumber: $('#sodienthoai').val(),
            Name: $('#hoten').val(),
            RegisterDate: $("#ngay-dk").val()
        }
        $.ajax({
            url: Global.UrlAction.InsertServiceRequire,
            type: 'POST',
            data: JSON.stringify({ 'model': obj }),
            contentType: 'application/json charset=utf-8',
            success: function (result) {
                if (result.Result == "OK") {
                    // $('#sodienthoai').val('');
                    // $('#dichvu').val(1);

                    $('#contentView').html(result.Data);
                    //  GlobalCommon.ShowMessageDialog("Bạn đã đăng ký thành công!", function () { }, 'Thông Báo');
                }
                else
                    //GlobalCommon.ShowMessageDialog(result.ErrorMessages[0].Message, function () { }, 'Lỗi');
                    $('#contentView').html(result.ErrorMessages[0].Message);
            }
        });
    }

}

$(document).ready(function () {
    var serveinfo = new GPRO.ServeInfo();
    serveinfo.Init();
});