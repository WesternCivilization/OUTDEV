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
        if (id == 0) {
            $(this).find('.modal-title').html("Thêm mới");
            //$("#frmDetail").find("#Code").prop('disabled', true);
        } else {
            $(this).find('.modal-title').html("Cập nhật");
            //$("#frmDetail").find("#Code").prop('disabled', true);
        }
        $("#myModal .modal-body-content").load("/RoomLevel/Edit/" + id);
        $("#myModal button#btnSave").css("display", "inline");
        $("#btnSave").off("click");
        $("#btnSave").click(function () {
            if (!$("#frmDetail").valid()) return;
            var pdata = getFormData($("#frmDetail"));
            showDialogLoading();
            setTimeout(function () {
                $.post("/RoomLevel/Update", { obj: pdata }, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid(); });
                    }
                    else {
                        alert("Có lỗi khi tạo hạng phòng");
                    }
                });
            }, 50);
        });
    });
    $("#myModal").modal("show");
}
function deleteDialog(id) {
    $("#myModalDelete").off('show.bs.modal');
    $('#myModalDelete').on('show.bs.modal', function (event) {
        $("#identifierDelete").val(id);
        //TO DO: init content for popup
        $(this).find('#title_confirm').html("Xóa hạng phòng");
        $(this).find('#bodyConfirm').html("Bạn có chắc chắn muốn xóa?");
        //--delete
        $("#btnDelete").off("click");
        $("#btnDelete").click(function () {
            showDialogLoading();
            var id = $("#identifierDelete").val();
            if (id != "") {
                setTimeout(function () {
                    $.post("/RoomLevel/Delete", { id: id },
                        function (data) {
                            hideDialogLoading();
                            //closeDlgLoadingData();
                            if (data.result > 0) {
                                bootbox.alert("Thao tác thành công", function () { $("#myModalDelete").modal("hide"); searchGrid(); });
                            } else {
                                alert("Có lỗi khi xóa hạng phòng");
                            }
                        });
                }, 50);
            }
        });
    });
    $("#myModalDelete").modal("show");
}
function viewDialog(id) {
    $("#myModal").off('show.bs.modal');
    $("#myModal .modal-body-content").html('<p>loading..</p>');
    $(this).find('.modal-title').html("Xem chi tiết");
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").load("/RoomLevel/GetDetail/" + id);
        $("#myModal button#btnSave").css("display", "none");
    });
    $("#myModal").modal("show");
}
function viewRoom(id) {
    $("#modalRoom").off('show.bs.modal');
    $("#modalRoom .modal-body-content").html('<p>loading..</p>');
    $("#modalRoom").on('show.bs.modal', function () {
        $("#modalRoom .modal-body-content").load("/RoomLevel/GetRoom/" + id);
    });
    $("#modalRoom").modal("show");
}