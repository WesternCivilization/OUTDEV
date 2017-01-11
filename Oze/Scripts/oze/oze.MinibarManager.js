function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}
function getParams() {
    return { s: $("#txtSearch").val(), offset: $table.bootstrapTable('getOptions').pageNumber, limit: $table.bootstrapTable('getOptions').pageSize }
}
function validateInput() {
    var $form = $("#frmDetail");
    $form.validate({
        rules: {

        },
        messages: {

        }
    });
    $('.LimitCheck').each(function () {
        $(this).rules('add', {
            number: true,
            messages: {
                number: 'Nhập sai định dạng',
            }
        });
    });
}
function editDialog(id) {
    $("#myModal").off('show.bs.modal');
    $("#myModal").on('show.bs.modal', function () {
        $("#myModal .modal-body-content").html('<p>loading..</p>');
        var ac = "Add";
        if (id == 0) {
            $(this).find('.modal-title').html("Thêm mới");
        } else {
            $(this).find('.modal-title').html("Cập nhật");
            ac = "Edit";
        }
        //var data = { id: id, action: "Add" };
        //var recursiveEncoded = $.param(data);
        //var recursiveDecoded = decodeURIComponent($.param(data));
        //$("#myModal .modal-body-content").load("/MinibarManager/ShowModal/14/");
        $.ajax({
            cache: false,
            type: 'POST',
            url: '/MinibarManager/ShowModal/',
            data: { id: id, action: ac },
            success: function (data) {
                $("#myModal .modal-body-content").html(data);
            }
        });
        //validateInput();
        $("#myModal button#btnSave").css("display", "inline");
        $("#btnSave").off("click");
        $("#btnSave").click(function () {
            if (!$("#frmDetail").valid()) return;
            var arr = [];
            $("#tblProduct tr").each(function (i) {
                var $fieldset = $(this);
                var productId = ($('input:checkbox:eq(0)', $fieldset).val());
                var limit = ($('input:text:eq(0)', $fieldset).val());
                var note = ($('input:text:eq(1)', $fieldset).val());
                if ($('input:checkbox:eq(0)', $fieldset).is(":checked")) {
                    if (productId != 0 && limit != 0 && productId != 'undefined' && limit != 'undefined') {
                        var item = {
                            ProductId: productId,
                            Limit: limit,
                            Note: note
                        };
                        //var queryStr = { item };
                        arr.push(item);
                    }
                }
            });
            var name = $("#frmDetail").find("#txtName").val();
            var storeId = $("#frmDetail").find("#txtStoreId").val();
            var roomId = $("#frmDetail").find("#cboSelectRoom").val();
            var pdata = { Name: name, RoomId: roomId, Item: arr, Id: storeId };
            showDialogLoading();
            setTimeout(function () {
                $.post("/MinibarManager/Update", { model: pdata, values: JSON.stringify(arr) }, function (data) {
                    hideDialogLoading();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid(); });
                    }
                    else {
                        if (data.result == -100) {
                            alert("Phòng này đã có Minibar. Hãy chọn phòng khác");
                        } else {
                            alert("Có lỗi khi tạo hạng phòng");
                        }
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
        $(this).find('#title_confirm').html("Xóa minibar");
        $(this).find('#bodyConfirm').html("Bạn có chắc chắn muốn xóa?");
        //--delete
        $("#btnDelete").off("click");
        $("#btnDelete").click(function () {
            showDialogLoading();
            var id = $("#identifierDelete").val();
            if (id != "") {
                setTimeout(function () {
                    $.post("/MinibarManager/Delete", { id: id },
                        function (data) {
                            hideDialogLoading();
                            //closeDlgLoadingData();
                            if (data.result > 0) {
                                bootbox.alert("Thao tác thành công", function () { $("#myModalDelete").modal("hide"); searchGrid(); });
                            } else {
                                alert("Có lỗi khi xóa Minibar");
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
        $.ajax({
            cache: false,
            type: 'POST',
            url: '/MinibarManager/ShowModal/',
            data: { id: id, action: "View" },
            success: function (data) {
                $("#myModal .modal-body-content").html(data);
                $("#myModal").find("#cboSelectRoom").prop('disabled', true);
            }
        });
        $("#myModal button#btnSave").css("display", "none");
    });
    $("#myModal").modal("show");
}
