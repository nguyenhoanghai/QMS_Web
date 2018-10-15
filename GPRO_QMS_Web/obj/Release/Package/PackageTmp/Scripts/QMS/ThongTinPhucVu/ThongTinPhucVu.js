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
        GetServiceSelect('dichvu');
        setInterval(function () { Get() }, 1000); 
    }

    var RegisterEvent = function () {
        $('#dangky').click(function () {
            if (CheckValidate())
                InsertServiceRequire();
        });

        $('#sodienthoai').on("keypress", function () {
            return isNumberKey(event);
        }); 
        $('#xemthongtinphucvu').click(function () {
            if (CheckValidate())
                Find();
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
        $.ajax({
            url: Global.UrlAction.Find,
            type: 'POST',
            data: JSON.stringify({ 'Phone': $('#sodienthoai').val(), 'Service': $('#dichvu').val() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                if (data.Result == "OK") {
                    $('#contentView').html(data.Data);
                }
                else {
                    alert('Không tìm thấy thông tin theo yêu cầu');
                    $('#contentView').html('');
                }
            }
        });
    }
    
    function DrawTable(objs) {
        var str = '<div class="col-md-12 rowcontent"> Không có dữ liệu </div>';
        if (objs.length > 0) {
            str = '';
            $.each(objs, function (i, item) {
                if (i % 2 == 0) {
                    str += '<div class="background-row-even rowcontent"><div class="col-md-4 col-sm-4 col-xs-4 font_28 margin-right bor_bottom border-right">' + item.ServiceName + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt margin-left bor_bottom border-right">' + item.TicketNumberProcessing + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt bor_bottom border-right">' + item.TotalCarsWaiting + '</div><div class="clearfix"></div>';
                }
                else
                {
                    str += '<div class="background-row-odd rowcontent"><div class="col-md-4 col-sm-4 col-xs-4 font_28 margin-right bor_bottom border-right">' + item.ServiceName + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt margin-left bor_bottom border-right">' + item.TicketNumberProcessing + '</div><div class="col-md-4 col-sm-4 col-xs-4 font-dt bor_bottom border-right">' + item.TotalCarsWaiting + '</div><div class="clearfix"></div>';
                }
            });
        }
        $('#thongtindichvuchitiet').empty().append(str);

    }

    function CheckValidate() {
        if ($('#sodienthoai').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng nhập Số Điện Thoại.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        else if ($('#dichvu').val().trim() == "") {
            GlobalCommon.ShowMessageDialog("Vui lòng chọn Dịch Vụ.", function () { }, "Lỗi Nhập liệu");
            return false;
        }
        return true;
    }

    function InsertServiceRequire() {
        var obj = {
            MAPHIEU: 0,
            MADV: $('#dichvu').val(),
            GIOCAP: '',
            MATT: '',
            MADN: null,
            MANVDAU: null,
            SOXE: '',
            PHONE: $('#sodienthoai').val(),
            TGPHUCVU_DK: '',
        }
        $.ajax({
            url: Global.UrlAction.InsertServiceRequire,
            type: 'POST',
            data: JSON.stringify({ 'yc': obj }),
            contentType: 'application/json charset=utf-8',
            success: function (result) {
                if (result.Result == "OK") {
                    $('#sodienthoai').val('');
                    $('#dichvu').val(1);

                    $('#contentView').html(result.Data);
                    GlobalCommon.ShowMessageDialog("Bạn đã đăng ký thành công!", function () { }, 'Thông Báo');
                }
                else
                    GlobalCommon.ShowMessageDialog(result.ErrorMessages[0].Message, function () { }, 'Lỗi');
            }
        });
    }

 }

$(document).ready(function () {
    var serveinfo = new GPRO.ServeInfo();
    serveinfo.Init();
});