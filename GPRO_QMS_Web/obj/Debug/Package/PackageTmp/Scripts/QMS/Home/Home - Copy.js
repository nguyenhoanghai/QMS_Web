﻿
<script src="~/Scripts/QMS/Home/Home_N.js"></script>
<input id="user" type="hidden" value="@ViewData["user"] " />



<style>
    body {
    background: url(../../Content/U__.png) no-repeat;
        background-size: cover;
        font-weight:bold;
    }

.b_title {
    background: black;
    padding:  0px 5px;
    text-align: center;
    font-size: 54px;
}

.i_title {
    font-size: 45px;
    font-weight: bold;
    border-bottom: 10px solid #bbb;
    margin: 50px 20px;
}

.i_title img {
    height: 100px;
}

ul, ul li {
    margin: 0;
    padding: 0;
}

ul li {
    list-style: none;
    padding:20px  ;
    text-align: right;
    cursor:pointer
}

.nut {
    background: none;
    border: none;
    text-transform: uppercase;
    font-size: 54px;
    font-weight: bold;
    color: red;
    padding:10px 0
}

.nut div{
    float:right;
    margin-top:-15px
}

.nut img{
    width:90px
}


       #currentNumber  {
           background-color: red;
           width: 100%;
           text-align: center;
           padding: 20px 0 0 0;
           margin: 0px 0 -81px 0;
           line-height: 331px;
       }

/*    #num  {
        position: relative;
        top: 50%;
        transform: translateY(-50%);
    } */
         
</style>




<div id="fullscreen">
    <div class="full-box" style="    position: absolute;">
        <a id="request-full" class="btn btn-primary">
            <i class="fa fa-arrows-alt "></i>
        </a>
        <a class="btn btn-primary" onclick="fullScreenApi.cancelFullScreen(document.documentElement)" style="display:none">
            <i class="fa fa-compress "> </i>
        </a>
    </div>




<div   id="currentNumber" class="font-dt"  >
    <span id="num" style="color:white ; font-size:500px; padding :  0px 0 ">2032</span>
</div>


<div id="danhgia_box" style="color: red; text-transform:uppercase; ">
    <div class="b_title">
<div class="col-md-1"  style="background:url(../../Content/logo_tienthu.png) no-repeat;     height: 80px; background-size: 81px;"> </div>
<div class="col-md-11">Khách hàng vui lòng đánh giá</div>
<div class="clearfix"></div>
</div>
    <div>
        <div id="noidung"> </div>
    </div>
</div>

</div>






<script>
    function danhgia(value) {
        $.ajax({
            url: '/Home/Evaluate',
            type: 'POST',
            data: JSON.stringify({ 'value': value, 'num': $('#num').html() }),
            contentType: 'application/json charset=utf-8',
            success: function (data) {
                //  alert(data.Result);
            }
        });
    }
</script>


<script>
    (function () {
        var
            fullScreenApi = {
                supportsFullScreen: false,
                nonNativeSupportsFullScreen: false,
                isFullScreen: function () { return false; },
                requestFullScreen: function () { },
                cancelFullScreen: function () { },
                fullScreenEventName: '',
                prefix: ''
            },
            browserPrefixes = 'webkit moz o ms khtml'.split(' ');

        // check for native support
        if (typeof document.cancelFullScreen != 'undefined') {
            fullScreenApi.supportsFullScreen = true;
        } else {
            // check for fullscreen support by vendor prefix
            for (var i = 0, il = browserPrefixes.length; i < il; i++) {
                fullScreenApi.prefix = browserPrefixes[i];

                if (typeof document[fullScreenApi.prefix + 'CancelFullScreen'] != 'undefined') {
                    fullScreenApi.supportsFullScreen = true;
                    break;
                }
            }
        }

        // update methods to do something useful
        if (fullScreenApi.supportsFullScreen) {
            fullScreenApi.fullScreenEventName = fullScreenApi.prefix + 'fullscreenchange';

            fullScreenApi.isFullScreen = function () {
                switch (this.prefix) {
                    case '':
                        return document.fullScreen;
                    case 'webkit':
                        return document.webkitIsFullScreen;
                    default:
                        return document[this.prefix + 'FullScreen'];
                }
            }
            fullScreenApi.requestFullScreen = function (el) {
                return (this.prefix === '') ? el.requestFullScreen() : el[this.prefix + 'RequestFullScreen']();
            }
            fullScreenApi.cancelFullScreen = function (el) {
                return (this.prefix === '') ? document.cancelFullScreen() : document[this.prefix + 'CancelFullScreen']();
            }
        }
        else if (typeof window.ActiveXObject !== "undefined") { // IE.
            fullScreenApi.nonNativeSupportsFullScreen = true;
            fullScreenApi.requestFullScreen = fullScreenApi.requestFullScreen = function (el) {
                var wscript = new ActiveXObject("WScript.Shell");
                if (wscript !== null) {
                    wscript.SendKeys("{F11}");
                }
            }
            fullScreenApi.isFullScreen = function () {
                return document.body.clientHeight == screen.height && document.body.clientWidth == screen.width;
            }
        }

        // jQuery plugin
        if (typeof jQuery != 'undefined') {
            jQuery.fn.requestFullScreen = function () {

                return this.each(function () {
                    if (fullScreenApi.supportsFullScreen) {
                        fullScreenApi.requestFullScreen(this);
                    }
                });
            };
        }

        // export api
        window.fullScreenApi = fullScreenApi;
    })();

function check() {
    if (!fullScreenApi.isFullScreen())
        fullScreenApi.requestFullScreen(document.documentElement);
    else
        fullScreenApi.cancelFullScreen(document.documentElement);
}

$('#request-full').click(function () {
    check();
});

</script>

