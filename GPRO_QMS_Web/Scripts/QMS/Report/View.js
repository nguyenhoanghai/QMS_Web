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
            //  Get: '/Report/Get ', 
            Get: '/Report/GetReport_NotUseQMS',  //ko dung qms
            GetEvaluate: '/Admin/Evaluate/GetList',
            Report: '/Report/Excel'
        },
        Data: {
            firstLoad: true
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        //$('#currentNumber, #danhgia_box').css('height', $(window).height());
        //GetEvaluate();
        RegisterEvent();
        GetUserSelect('user');
    }

    var RegisterEvent = function () {
        $("#from,#to").kendoDatePicker({
            format: "dd/MM/yyyy",
        });

        $('#view').click(function () {
            Get();
        });

        // setInterval(function () { $('#view').click(); }, 1000)

        $('#export').click(function () {
            window.location.href = Global.UrlAction.Report + "?userId=0&from=" + $("#from").val() + "&to=" + $("#to").val() + "&useQMS=false";
        });
    }

    //function Get() { 
    //    $.ajax({
    //        url: Global.UrlAction.Get,
    //        type: 'POST',
    //        data: JSON.stringify({'userId': $('#user').val(), 'from': $("#from").data("kendoDatePicker").value() , 'to' : $("#to").data("kendoDatePicker").value() }),
    //        contentType: 'application/json charset=utf-8',
    //        success: function (data) {
    //            var str = '<tr><td colspan="4">Không có dữ liệu</td></tr>';
    //            if (data.length > 0) {
    //                str = '';
    //                $.each(data, function (i, item) {
    //                    str += '<tr>';
    //                    str += ' <td  >' + item.Name + '</td> ';
    //                    str += ' <td  >' + item.tc1 + '</td> ';
    //                    str += ' <td  >' + item.tc2 + '</td> ';
    //                    str += ' <td  >' + item.tc3 + '</td> ';
    //                    str += '<tr>'; 
    //                });
    //            }
    //            $('#tb_export tbody').empty().html(str);
    //        }
    //    });
    //}


    function Get() {
        $.ajax({
            url: Global.UrlAction.Get,
            type: 'POST',
            data: JSON.stringify({ 'userId': 0, 'from': $("#from").data("kendoDatePicker").value(), 'to': $("#to").data("kendoDatePicker").value() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                var str = '<tr><td colspan="5">Không có dữ liệu</td></tr>',
                    str1 = '<tr><td colspan="5">Không có dữ liệu</td></tr>';
                if (data.length > 0) {
                    str = '', str1 = '';
                    var i = 0, y = Math.ceil(data.length / 2);
                    for (i, y; i < data.length; i++ , y++) {
                        if (i == 0) {
                            var tr = $('<tr></tr>');
                            tr.append('<td>STT</td>');
                            tr.append('<td>HỌ TÊN</td>');
                            $.each(data[i].Details, function (ii, child) {
                                tr.append('<td>' + child.Name + '</td>');
                            })
                            $('#tb_export thead,#tb_export2 thead').empty().append(tr);
                        }
                        if (i < Math.ceil(data.length / 2)) {

                            str += '<tr><td  >' + (i + 1) + '</td>';
                            str += '<td>' + data[i].Name + '</td>';
                            $.each(data[i].Details, function (ii, child) {
                                str += '<td>' + child.Id + '</td>';
                            });
                            str += '</tr>';

                            if (data[y] != null) {
                                str1 += '<tr><td  >' + (y + 1) + '</td>';
                                str1 += '<td class="text" >' + data[y].Name + '</td>';
                                $.each(data[y].Details, function (ii, child) {
                                    str1 += '<td>' + child.Id + '</td>';
                                });
                                str1 += '</tr>';
                            }
                        }
                    }
                    $('#tb_export tbody').empty().html(str);
                    $('#tb_export2 tbody').empty().html(str1);
                    //$.each(data, function (i, item) {
                    //    str += '<tr>';
                    //    str += ' <td  >' + (i + 1) + '</td> ';
                    //    str += ' <td  >' + item.Name + '</td> ';
                    //    str += ' <td  >' + item.tc1 + '</td> ';
                    //    str += ' <td  >' + item.tc2 + '</td> ';
                    //    str += ' <td  >' + item.tc3 + '</td> ';
                    //    str += '<tr>';   
                    //});
                    //$('#tb_export tbody').empty().html(str);
                }

            }
        });
    }

}

$(document).ready(function () {
    var home = new GPRO.Home();
    home.Init();
});