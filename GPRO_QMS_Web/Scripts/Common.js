﻿//**************************************************************************************//
//                          FUNCTION FORMAT JSONDATE TO DATE                            //
//                                                                                      //
//                  input( /Date(1297246301973)/) => out  14/2/2011                     //
//**************************************************************************************//
function parseJsonDateToDate(jsonDate) {
    if (jsonDate != null && jsonDate != '') {
        var dateString = jsonDate.substr(6);
        return new Date(parseInt(dateString));
    }
    else {
        return null;
    }
}

//function parseJsonTimeToTime(jsonTime) {
//    if (jsonTime != null && jsonTime != '') {
//        var timeString = jsonTime.substr(7);
//        return new Time(parseTime(timeString));
//    }
//    else {
//        return null;
//    }
//}

//**************************************************************************************//
//                    FUNCTION FORMAT NUMBER TO CURRENCY STYLE                          //
//                                                                                      //
//                            input(10000) => out 10.000                                //
//**************************************************************************************//
function ParseStringToCurrency(money) {
    money += '';
    var x = money.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function formatCurrency(total) {
    var neg = false;
    if (total < 0) {
        neg = true;
        total = Math.abs(total);
    }
    return (neg ? "-$" : '$') + parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
}

//**************************************************************************************//
//                              FUNCTION ADD DAY INTO A DATE                            //
//              parameter : date => the date which you wanto add day                    //
//              parameter : days => the number of day which you want add add            //
//**************************************************************************************//

function addDays(theDate, days) {
    return new Date(theDate.getTime() + days * 24 * 60 * 60 * 1000);
}

/**********************************************************************************************************************
 *                                                  CHECK KHÔNG CHO NHẬP CHỮ                                          *      
 **********************************************************************************************************************/
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode == 59 || charCode == 46)
        return true;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
    { GlobalCommon.ShowMessageDialog("Vui lòng nhập số.", function () { }, "Lỗi Nhập liệu"); }
}
 

function ParseDateToString(date) {
    var dd = date.getDate() < 10 ? ('0' + date.getDate()) : date.getDate();
    dd += '/';
    dd += (date.getMonth() + 1) < 10 ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    dd += '/' + date.getFullYear() + ' '; 
    return dd;
}

function ParseDateToStringWithoutTime(date) {
    var dd = date.getDate() < 10 ? ('0' + date.getDate()) : date.getDate();
    dd += '/';
    dd += (date.getMonth() + 1) < 10 ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    dd += '/' + date.getFullYear() + ' ';
    return dd;
}


//function ParseTimeToString(time) {
//    var dd = time.getTime() < 10 ? ('0' + time.getTime()) : time.getTime();
//    dd += '/';
//    dd += (time.getTime().Ho + 1) < 10 ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
//    dd += '/' + date.getFullYear() + ' ';
//    return dd;
//}