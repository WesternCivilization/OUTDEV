function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}

function editDialog1(id)
{
    BootstrapDialog.show({
        title: 'Chỉnh sửa đơn vị',
        message: $('<div></div>').load('/Unit/Edit/' + id),
        buttons: [{
            label: 'Lưu',
            action: function (dialog) {
                if (!$("#dummyUnit").valid()) return;
                var pdata = getFormData($("#dummyUnit"));
                showDialogLoading();
                    $.post("/Unit/update", { obj: pdata }, function (data) {
                        hideDialogLoading();
                        //closeDlgLoadingData();
                        if (data.result > 0) {
                            bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid(); });
                        }
                        else {
                            alert("Có lỗi khi tạo sản phẩm");
                        }
                    });
            }
        }, {
            label: 'Close',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}
function editDialog(id) {

    $("#myModal").off('show.bs.modal');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").html('<p>loading..</p>');
        $("#myModal .modal-body-content").load("/Unit/Edit/" + id);
        $("#myModal button#btnSave").css("display", "inline");
        $("#btnSave").off("click");
        $("#btnSave").click(function () {
            if (!$("#dummyUnit").valid()) return;
            var pdata = getFormData($("#dummyUnit"));
            showDialogLoading();
                $.post("/Unit/update", { obj: pdata }, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid(); });
                    }
                    else {
                        alert("Có lỗi khi tạo sản phẩm");
                    }
                });
        });
    });
    $("#myModal").modal("show");
}
function deleteDialog(id) {

}
function viewDialog(id)
{
    window.location.href = "/ReservationRoom/DatPhongDetail/" + id;
    /*
    $("#myModal").off('show.bs.modal');
    $("#myModal .modal-body-content").html('<p>loading..</p>');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").load("/Unit/GetDetail/" + id);
        $("#myModal button#btnSave").css("display", "none");
    });
    $("#myModal").modal("show");
    */
}