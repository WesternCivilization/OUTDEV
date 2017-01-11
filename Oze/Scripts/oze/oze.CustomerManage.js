function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}
function getParams() {
    return { s: $("#txtSearch").val(), offset: $table.bootstrapTable('getOptions').pageNumber, limit: $table.bootstrapTable('getOptions').pageSize }
}
//view cập nhật thông tin khách hàng
function viewDetailDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {

        $("#TittleBox").html("Thông tin khách hàng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerManage/GetDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            g_Utils.SetDate(".birthday");
        });

        $("#modalDetails button#btnUpdateDetail").css("display", "inline");
        $("#btnUpdateDetail").off("click");

        $("#btnUpdateDetail").click(function () {

            if (!$("#formDetail").valid()) return;
            var pdata = getFormData($("#formDetail"));
            showDialogLoading();
                $.post("/CustomerManage/update", { obj: pdata }, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Cập nhật thống tin khách hàng thành công.", function () {
                            searchGrid();
                            $("#modalDetails").modal("hide");
                        });
                    }
                    else {
                        alert("Cập nhật thống tin khách hàng thất bại!");
                    }
                });
        });
    });
    $("#modalDetails").modal("show");
}
//view cập nhật thông tin khách hàng
function viewEditDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {

        $("#TittleBox").html("Thông tin khách hàng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerManage/GetEdit", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            g_Utils.SetDate(".birthday");
        });

        $("#modalDetails button#btnUpdateDetail").css("display", "inline");
        $("#btnUpdateDetail").off("click");

        $("#btnUpdateDetail").click(function () {

            if (!$("#formDetail").valid()) return;
            var pdata = getFormData($("#formDetail"));
            showDialogLoading();
                $.post("/CustomerManage/update", { obj: pdata }, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Cập nhật thông tin khách hàng thành công.", function () {
                            searchGrid();
                            $("#modalDetails").modal("hide");
                        });
                    }
                    else {
                        alert("Cập nhật thống tin khách hàng thất bại!");
                    }
                });
        });
    });
    $("#modalDetails").modal("show");
}

function SetDate(parameters) {
    $(parameters).datetimepicker({
        //debug:true,
        //locale: 'vi',
        format: 'DD/MM/YYYY',
        showTodayButton: true,
        //maxDate: new Date(),
        //defaultDate: new Date(),
        showClose: false

    });



}
function viewDialogHistory(id) {
    showDialogLoading();
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails .modal-body-content").html('<p>loading..</p>');
    $("#modalDetails").on('show.bs.modal', function () {
        $("#TittleBox").html("Lịch sử khách hàng");

        $.post("/CustomerManage/GetHistoryDetail", { id: id }, function (rs) {
            hideDialogLoading();
            $("#modalDetails .modal-body-content").html(rs);
            $("#modalDetails button#btnUpdateDetail").css("display", "none");

        });
    });
    $("#modalDetails").modal("show");
}

function viewInfoDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {
     
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerManage/GetInfomationRoomDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("#modalDetails #formInforDetailCustomer :input").attr("disabled", true);
            $("#modalDetails #formInforDetailRoom :input").attr("disabled", true);
            g_Utils.SetDate(".birthday");
            $("#TittleBox").html("Thông tin đặt phòng");
        });
        $("#modalDetails button#btnUpdateDetail").css("display", "none");
    });
    $("#modalDetails").modal("show");
}
function DeleteDialog(id) {
    var r = confirm("Bạn có chắc muốn xóa thông tin khách?");
    if (r) {
        showDialogLoading();
            $.post("/CustomerManage/Delete", { id: id }, function (data) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (data.result > 0) {
                    bootbox.alert("Xóa thông tin khách hàng thành công.", function () {
                        searchGrid();
                    });
                }
                else {
                    alert("Xóa thông tin khách hàng thất bại!");
                }
            });
    }
}