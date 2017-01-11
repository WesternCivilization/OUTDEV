function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}

function editDialog(id) {

    $("#myModal").off('show.bs.modal');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").html('<p>loading..</p>');
        $("#myModal .modal-body-content").load("/Room1/Edit/" + id);
        $("#myModal button#btnSave").css("display", "inline");
        $("#btnSave").off("click");
        $("#btnSave").click(function () {
            if (!$("#dummyRoom").valid()) return;
            var pdata = getFormData($("#dummyRoom"));
            showDialogLoading();
           
            $.post("/Room1/update", { obj: pdata }, function (data)
            {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (data.result > 0)
                {
                    bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchRoom(); });
                }
                else {
                    alert("Có lỗi khi cập nhật phòng");
                }
            });
        });
    });
    $("#myModal").modal("show");
}
function deleteDialog(id) {

}
function viewDialog(id) {
    $("#myModal").off('show.bs.modal');
    $("#myModal .modal-body-content").html('<p>loading..</p>');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").load("/Room1/GetDetail/" + id);
        $("#myModal button#btnSave").css("display", "none");
    });
    $("#myModal").modal("show");
}