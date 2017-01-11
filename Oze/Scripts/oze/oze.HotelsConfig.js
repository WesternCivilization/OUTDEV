$(document).ready(function () {
    $("#btnSave").click(function() {
        var pdata = getFormData($("#formData"));
        showDialogLoading();
            $.post("/HotelsConfig/UpdateOrInsert", { obj: pdata }, function (data) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (data.result > 0) {
                    bootbox.alert("Lưu cấu hình thành công", function () { });
                }
                else {
                    alert("lỗi");
                }
            });
    });
});


