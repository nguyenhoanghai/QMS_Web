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
GPRO.namespace('GoiSo1');
GPRO.GoiSo1 = function () {
    var Global = {
        UrlAction: { 
        },
        Data: {
            rows: 1,
            cols: 1,
            lastRows: 0,
            Drawed: false,
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
    }
    var RegisterEvent = function () { }

     
};
$(document).ready(function () {
    var home = new GPRO.GoiSo1();
    home.Init();
});