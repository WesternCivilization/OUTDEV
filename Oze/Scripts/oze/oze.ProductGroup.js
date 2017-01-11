function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}
function getParams() {
    return { s: $("#txtSearch").val(), offset: $table.bootstrapTable('getOptions').pageNumber, limit: $table.bootstrapTable('getOptions').pageSize }
}
function editDialog(id) {

    $("#myModal").off('show.bs.modal');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").html('<p>loading..</p>');
        $("#myModal .modal-body-content").load("/ProductGroup/Edit/" + id);
        $("#myModal button#btnSave").css("display", "inline");
        $("#btnSave").off("click");
        $("#btnSave").click(function () {
            if (!$("#dummyProductGroup").valid()) return;
            var pdata = getFormData($("#dummyProductGroup"));
            showDialogLoading();
                $.post("/ProductGroup/update", { obj: pdata }, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid();});
                    }
                    else {
                        alert("Có lỗi khi tạo nhóm dịch vụ");
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
        $("#myModal .modal-body-content").load("/ProductGroup/GetDetail/" + id);
        $("#myModal button#btnSave").css("display", "none");
    });
    $("#myModal").modal("show");
}