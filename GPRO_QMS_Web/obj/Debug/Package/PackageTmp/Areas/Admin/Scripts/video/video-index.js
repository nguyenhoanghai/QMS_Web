
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
GPRO.namespace('Video');
GPRO.Major = function () {
    var Global = {
        UrlAction: {
            Gets: '/Admin/Video/Gets',
            Delete: '/Admin/Video/Delete',
            Save: '/Admin/Video/Save'
        },
        Element: {
            Jtable: 'table',
            Popup: 'popup'
        },
        Data: {
            Id: 0
        }
    }
    this.GetGlobal = function () {
        return Global;
    }

    this.Init = function () {
        RegisterEvent();
        InitList();
        ReloadList();
        InitPopup();
    }

    var RegisterEvent = function () {
        $('#p-file-upload').change(function () {
            readURL(this);
        });

        $('#p-btn-file-upload').click(function () {
            $('#p-file-upload').click();
        });


        $('#p-file-upload').change(function () {
            if ($('#p-file-upload').val() != '')
                UploadVideo("p-file-upload", 'p-file-upload');
        })
        // Register event after upload file done the value of [filelist] will be change => call function save your Data 
        $('#p-file-upload').select(function () {
            ReloadList();
        });
    }

    readURL = (input) => {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                // $('.img-avatar').attr('src', e.target.result);

                var video = document.getElementsByTagName('video')[0];
                var sources = video.getElementsByTagName('source');

                sources[0].src = e.target.result;
                sources[1].src = e.target.result;
                video.load();
            }
            reader.readAsDataURL(input.files[0]); // convert to base64 string
        }
    }

    function InitList() {
        $('#' + Global.Element.Jtable).jtable({
            title: 'Danh sách tệp video',
            paging: true,
            pageSize: 50,
            pageSizeChange: true,
            sorting: true,
            selectShow: false,
            actions: {
                listAction: Global.UrlAction.Gets,
            },
            messages: {
            },
            searchInput: {
                id: 'search-key',
                className: 'search-input',
                placeHolder: 'Nhập từ khóa ...',
                keyup: function (evt) {
                    if (evt.keyCode == 13)
                        ReloadList();
                }
            },
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                FileName: {
                    title: "Tệp",
                    width: "70%",
                },
                Duration: {
                    title: "thời lượng",
                    width: "20%",
                    sorting: false,
                    display: function (data) {
                        return `<span>${data.record.Duration.Hours}:${data.record.Duration.Minutes}:${data.record.Duration.Seconds}</span>`;
                    }
                },
                actions: {
                    title: '',
                    width: '10%',
                    columnClass:'text-center',
                    sorting: false,
                    display: function (data) {
                        var div = $('<div></div>');
                        var btnPlay = $('<i data-toggle="modal" data-target="#' + Global.Element.Popup + '" title="Phát video" class="fa fa-play-circle-o clickable red"  ></i>');
                        btnPlay.click(function () {
                            var video = document.getElementsByTagName('video')[0];
                            var sources = video.getElementsByTagName('source');
                            sources[0].src = `/Videos/${data.record.FakeName}`;
                            sources[1].src = `/Videos/${data.record.FakeName}`;
                            video.load();
                        });
                        div.append(btnPlay);

                        var btnDelete = $('<i title="Xóa" class="fa fa-trash-o red clickable i-delete"></i>');
                        btnDelete.click(function () {
                            GlobalCommon.ShowConfirmDialog('Bạn có chắc chắn muốn xóa?', function () {
                                Delete(data.record.Id, data.record.FakeName);

                            }, function () { }, 'Đồng ý', 'Hủy bỏ', 'Thông báo');
                        });
                        div.append(btnDelete); 

                        return div;
                    }
                } 
            }
        });
    }

    function ReloadList() {
        $('#' + Global.Element.Jtable).jtable('load', { 'keyword': $('#search-key').val() });
    }

    function Delete(Id, fileName) {
        $.ajax({
            url: Global.UrlAction.Delete,
            type: 'POST',
            data: JSON.stringify({ 'Id': Id }),
            contentType: 'application/json charset=utf-8',
            beforeSend: function () { $('#loading').show(); },
            success: function (data) {
                GlobalCommon.CallbackProcess(data, function () {
                    if (data.Result == "OK") {
                        DeleteFile(fileName, 'video');
                        ReloadList();
                        $('#loading').hide();
                    }
                }, false, Global.Element.PopupNhanVien, true, true, function () {

                    var msg = GlobalCommon.GetErrorMessage(data);
                    GlobalCommon.ShowMessageDialog(msg, function () { }, "Đã có lỗi xảy ra.");
                });
            }
        });
    }

    function InitPopup() {
        $("#" + Global.Element.Popup).modal({
            keyboard: false,
            show: false
        });

        $("#" + Global.Element.Popup + ' button[cancel]').click(function () {
            $("#" + Global.Element.Popup).modal("hide");
            var video = document.getElementsByTagName('video')[0];
            var sources = video.getElementsByTagName('source');
            sources[0].src = '';
            sources[1].src = '';
            video.load();
        });
    }

}
$(document).ready(function () {
    var obj = new GPRO.Major();
    obj.Init();
})