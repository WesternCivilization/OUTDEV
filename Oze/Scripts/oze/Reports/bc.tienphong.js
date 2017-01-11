
var $table;
function Detail(id) {
	Sv.AjaxPost({
		Url: "/StoreTransfer/GetDetail",
		Data: { Id: id }
	},
            function (obj) {
            	if (obj) {
            		$("#SoPhieu").val(obj.OrderCode);
            		$("#SoChungTu").val(obj.SupplierCode);
            		if (obj.InputDate != null) {
            			$("#NgayNhap").val(moment(new Date(parseInt(obj.InputDate.slice(6, -2)))).format("DD-MM-YYYY"));
            		} else {
            			$("#NgayNhap").val("");
            		}
            		if (obj.DatePayment != null) {
            			$("#NgayChungTu")
                            .val(moment(new Date(parseInt(obj.DatePayment.slice(6, -2)))).format("DD-MM-YYYY"));
            		} else {
            			$("#NgayChungTu").val("");
            		}
            		$("#DesStoreId").val(obj.StoreID);
            		$("#SrcStoreId").val(obj.FromStoreId);
            		$("#Product").val(obj.ProductId);
            		$("#ProductCode").val(obj.ProductCode);
            		$("#NhaCC").val(obj.SupplierID);
            		$("#NhomDv").val(obj.CateId);
            		if (obj.ManufactureDate != null) {
            			$("#NgaySx")
                            .val(moment(new Date(parseInt(obj.ManufactureDate.slice(6, -2)))).format("DD-MM-YYYY"));
            		} else {
            			$("#NgaySx").val('')
            		}
            		if (obj.ExpirationDate != null) {
            			$("#HSD").val(moment(new Date(parseInt(obj.ExpirationDate.slice(6, -2)))).format("DD-MM-YYYY"));
            		} else {
            			$("#HSD").val('')
            		}
            		// $("#NgaySx").val(obj.OrderCode);
            		// $("#HSD").val(obj.OrderCode);
            		$("#SoLuongNhap").val(obj.Quantity);
            		$("#Unit").val(obj.UnitId);
            		$("#DonGia").val(obj.Price);
            		$("#modalDetail").modal('show');
            	} else {

            	}
            });


}

$(document)
    .ready(function () {
    	Sv.SetupDateTime($("#FromDate"), $("#ToDate"));

    	function searchGrid() {
    		//$table.dataTable().fnDestroy();
    		///$table.ajax.reload();
    		$table.destroy();
    		$table = $("#example")
            .DataTable({
            	fixedHeader: {
            		header: true,
            		footer: true
            	},
            	language: {
            		sProcessing: "Đang xử lý...",
            		sLengthMenu: "Xem _MENU_ mục",
            		sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            		sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            		"sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
            		"sInfoFiltered": "(được lọc từ _MAX_ mục)",
            		"sInfoPostFix": "",
            		"sSearch": "Tìm:",
            		"sUrl": "",
            		"oPaginate": {
            			"sFirst": "Đầu",
            			"sPrevious": "Trước",
            			"sNext": "Tiếp",
            			"sLast": "Cuối"
            		}
            	},
            	"processing": true,
            	"serverSide": true,
            	"initComplete": function (settings, json) {
            		hideDialogLoading();
            		var api = this.api();
            		$(api.column(4).footer()).html("Tổng: " + json.totalAmount.toString().replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1."));
            	},
            	/*bFilter: false, bInfo: false,*/
            	"dom": '<"top">rt<"bottom" lip><"clear">',
            	"ajax": {
            		"url": "/report/ListTienPhong",
            		"data": function (d) {
            			d.search = "";
            			d.FromDate = $("input[name='FromDate']").val();
            			d.ToDate = $("[name='ToDate']").val();
            			d.Keyword = $("#keyword").val();
            			d.columns = "";
            			//d.search = "";
            			//d.FromDate = "";
            			//d.ToDate = "";
            			//d.Keyword = "";
            		}
            	},
            	"columns":
                [
                    {
                    	"data": null,
                    	render: function (data, type, row, infor) {

                    		return $table.page.info().page + infor.row + 1;
                    	}
                    },
                    { "data": "BookingCode", "orderable": "false" },
					{ "data": "Customername", "orderable": "false" },
                    {
                    	"data": null,
                    	render: function (data, type, row, infor) {
                    		if (row.DateCreated != null) {
                    			return moment(new Date(parseInt(row.DateCreated.slice(6, -2)))).format("DD-MM-YYYY");
                    		}
                    		return "";
                    	}
                    },
					 {
					 	"data": null,
					 	render: function (data, type, row, infor) {
					 		return row.TotalAmount.toString().replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
					 	},
					 	className: "dt-body-right"
					 },
					 { "data": "RoomNo", "orderable": "false" },
                    { "data": "RoomLevel", "orderable": "false" },

                ],
            	"footerCallback": function (row, data, start, end, display) {
            	}
            });

    	}
    	$("#btnSearch")
            .click(function () {
            	searchGrid();
            });
    	$("#keyword").keypress(function (e) {
    		if (e.which == 13) {
    			searchGrid();
    		}
    	});
    	showDialogLoading();
    	$table = $("#example")
            .DataTable({
            	fixedHeader: {
            		header: true,
            		footer: true
            	},
            	language: {
            		sProcessing: "Đang xử lý...",
            		sLengthMenu: "Xem _MENU_ mục",
            		sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            		sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            		"sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
            		"sInfoFiltered": "(được lọc từ _MAX_ mục)",
            		"sInfoPostFix": "",
            		"sSearch": "Tìm:",
            		"sUrl": "",
            		"oPaginate": {
            			"sFirst": "Đầu",
            			"sPrevious": "Trước",
            			"sNext": "Tiếp",
            			"sLast": "Cuối"
            		}
            	},
            	"processing": true,
            	"serverSide": true,
            	"initComplete": function (settings, json) {
            		hideDialogLoading();
            		var api = this.api();
            		$(api.column(4).footer()).html("Tổng: " + json.totalAmount.toString().replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1."));
            	},
            	/*bFilter: false, bInfo: false,*/
            	"dom": '<"top">rt<"bottom" lip><"clear">',
            	"ajax": {
            		"url": "/report/ListTienPhong",
            		"data": function (d) {
            			d.search = "";
            			d.FromDate = $("input[name='FromDate']").val();
            			d.ToDate = $("[name='ToDate']").val();
            			d.Keyword = $("#keyword").val();
            			d.columns = "";
            			//d.search = "";
            			//d.FromDate = "";
            			//d.ToDate = "";
            			//d.Keyword = "";
            		}
            	},
            	"columns":
                [
                    {
                    	"data": null,
                    	render: function (data, type, row, infor) {

                    		return $table.page.info().page + infor.row + 1;
                    	}
                    },
                    { "data": "BookingCode", "orderable": "false" },
					{ "data": "Customername", "orderable": "false" },
                    {
                    	"data": null,
                    	render: function (data, type, row, infor) {
                    		if (row.DateCreated != null) {
                    			return moment(new Date(parseInt(row.DateCreated.slice(6, -2)))).format("DD-MM-YYYY");
                    		}
                    		return "";
                    	}
                    },
					 {
					 	"data": null,
					 	render: function (data, type, row, infor) {
					 		return row.TotalAmount.toString().replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
					 	},
					 	className: "dt-body-right"
					 },
					 { "data": "RoomNo", "orderable": "false" },
                    { "data": "RoomLevel", "orderable": "false" },

                ],
            	"footerCallback": function (row, data, start, end, display) {
            	}
            });


    });