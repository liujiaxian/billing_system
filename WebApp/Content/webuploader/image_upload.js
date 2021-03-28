Jquery: $(function () {
    uploader = WebUploader.create({
        auto: true,
        swf: '/Content/webuploader/Uploader.swf',
        server: '/Upload/UploadImage',//后台接收图片的url  （我用的是C#）
        fileVal: 'upload',
        pick: '#filePicker',//html  class
        resize: false,
        duplicate: true,//可以重复上传 但是筛选不了重复图片
        // 只允许选择图片文件。
        accept: {
            title: '图片上传',
            extensions: 'gif,jpg,jpeg,bmp,png',
            mimeTypes: 'image/*'
        }
    });
    //当文件被加入队列之前触发
    uploader.on('beforeFileQueued', function (file, data) {
        //如果是单文件上传，把旧的文件地址传过去
        //if ($("#txttypeid").val() == '0') {
        //    alertMsgError('请先选择分类再上传图片！');
        //    return false;
        //}
    });
    uploader.on('uploadBeforeSend', function (obj, data, headers) {
        data = $.extend(data, {
            //action: $("#txtAction").val()
        });
    });
    // 文件上传过程中创建进度条实时显示。 
    uploader.on('uploadProgress', function (file, percentage) {
        Loading("上传中：" + parseInt(percentage * 100) + '%');
    });
    uploader.on('uploadSuccess', function (file, jsondata) {
        layer.closeAll();
        if (jsondata.status == 0) {
            alertSuccessMsg('上传成功');
            $(".imageSrc").attr("src", jsondata.data);
            $("#imageUrl").val(jsondata.data);
        } else {
            alertErrorMsg(jsondata.msg);
        }
    });
    uploader.on('uploadError', function (file) {
        alertErrorMsg('上传失败!!!');
    });
});