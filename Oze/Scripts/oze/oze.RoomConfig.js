$("#btnaddnew").click(function () {
	var validator = $("#frmRoom").validate();
	validator.resetForm();
	$("#lblTitle").text("Thêm mới phòng");
	$("#lblName").text("Tên phòng");
	$("#mdModify").modal("show");
	$("#btnAddRoom").css("display", "block");
	$("#btnEditRoom").css("display", "none");

	$("#divlblName").css("display", "block");
	$("#txtRoomName").css("display", "block");
	$("#txtRoomName").val("");

	$("#lblRoomNameDelete").css("display", "none");
	$("#btnDeleteRoom").css("display", "none");
	$("#alertDelRoom").css("display", "none");
});

$("#btnShowDSPhong").click(function () {
    var validator = $("#frmRoomtheogia").validate();
    validator.resetForm();
    $("#lblTitle").text("Thêm phòng");
    $("#lblName").text("Tên phòng");
    $("#mdModify").modal("show");
});

$("#frmRoomtheogia").validate({
    rules: {
        txtHangRoom: {
            required: true
        }
    },
    messages: {
        txttendangnhap: {
            required: "Vui lòng nhập tên đăng nhập"
        }
    }
});

function EditRoom(id, name) {
	$("#lblName").text("Đổi tên phòng");
	$("#lblTitle").text("");
	$("#txtRoomName").val(name);
	$("#btnAddRoom").css("display", "none");
	$("#btnEditRoom").css("display", "block");

	$("#divlblName").css("display", "block");
	$("#txtRoomName").css("display", "block");
	$("#lblRoomNameDelete").css("display", "none");
	$("#btnDeleteRoom").css("display", "none");
	$("#alertDelRoom").css("display", "none");

	var validator = $("#frmRoom").validate({
	    rules: {
	        txtRoomName: {
	            required: true
	        }
	    },
	    messages: {
	        txtRoomName: {
	            required: "Vui lòng nhập hạng phòng"
	        }
	    }
	});
	validator.resetForm();

	$("#mdModify").modal("show");

	$('#btnEditRoom').click(function () {
		// This is not working and is not validating the form
		if ($("#frmRoom").validate()) {
			$.ajax({
				url: '/Room/UpdateRooms',
				type: "Post",
				datatype: "json",
				data: {
				    'roomName': $("#txtRoomName").val(),
					'id': id
				},
				success: function (response) {
				    if (response["kq"] == 1) {
						$("#kq").text(response["mess"]);
						$("#kq").css("display", "block");
						setTimeout(function () {
						    window.location.reload(1);
						}, 1000);
					} else {
						$("#kq").text(response["mess"]);
						$("#kq").css("display", "block");
						
					}
				},
				error: function (response) {
					$("#kq").empty();
					$("#kq").text("Error : Chỉnh sửa thất bại!");
				}
			});
		}

	});
}

function DeleteRoom(id, name) {
	$("#lblRoomNameDelete").text(name);
	$("#btnAddRoom").css("display", "none");
	$("#btnEditRoom").css("display", "none");
	$("#kq").css("display", "none");
	$("#divlblName").css("display", "none");
	$("#txtRoomName").css("display", "none");
	$("#btnDeleteRoom").css("display", "block");
	$("#lblTitle").text("");

	$("#alertDelRoom").css("display", "block");

	$("#mdModify").modal("show");

	$('#btnDeleteRoom').click(function () {
		// This is not working and is not validating the form
		if ($("#frmRoom").validate()) {
			$.ajax({
				url: '/Room/DeleteRooms',
				type: "Post",
				datatype: "json",
				data: {
					'id': id
				},
				success: function (response) {
					$("#kq").text(response["mess"]);
					$("#kq").css("display", "block");
					setTimeout(function () {
						window.location.reload(1);
					}, 2500);
				},
				error: function (response) {
					$("#kq").empty();
					$("#kq").text("Error : Chỉnh sửa thất bại!");
				}
			});
		}

	});
}

$(document).ready(function () {
	$("#frmRoom").validate({
		rules: {
			txtRoomName: "required"
		},
		messages: {
			txtRoomName: "Không được để trống tên phòng"
		}
	});


	$('#btnaddnew').click(function () {
		$('#btnAddRoom').css("display", "block");
	});

	$('#btnAddRoom').click(function () {
		// This is not working and is not validating the form
		if ($("#frmRoom").validate()) {
			$.ajax({
				url: '/Room/CreateRoom',
				type: "Post",
				datatype: "json",
				data: {
					'roomName': $("#txtRoomName").val()
				},
				success: function (response) {
					if (response["kq"] != "1") {
						$("#kq").text(response["mess"]);
						$("#kq").css("display", "block");
					} else {
						$("#kq").text(response["mess"]);
						$("#kq").css("display", "block");
						setTimeout(function () {
							window.location.reload(1);
						}, 2500);
					}
				},
				error: function (response) {
					$("#messthongbao").empty();
					$("#messthongbao").text("Error : Thêm mới thất bại!");
					$('#modalthongbao').modal('show');
				}
			});
		}

	});
});