
var $table;
function Detail(id) {
    Sv.AjaxPost({
        Url: "/kho/GetDetail",
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
                    $("#StoreId").val(obj.StoreID);
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
                    $("#modalDetail").modal('show')
                } else {

                }
            });


}
$(document)
    .ready(function () {
        Sv.SetupDateTime($("#FromDate"), $("#ToDate"));

        function searchGrid() {
            //$table.bootstrapTable('refresh');
            $table.ajax.reload();
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
                "language": {
                    "sProcessing": "Đang xử lý...",
                    "sLengthMenu": "Xem _MENU_ mục",
                    "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
                    "sInfo": "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
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
                    //alert( 'DataTables has finished its initialisation.' );
                },
                /*bFilter: false, bInfo: false,*/
                "dom": '<"top">rt<"bottom" lip><"clear">',
                "ajax": {
                    "url": "/kho/DanhSachNhapKho",
                    "data": function (d) {
                        console.log(d)
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
                    { "data": "OrderCode", "orderable": "false" },
                    {
                        "data": null,
                        render: function (data, type, row, infor) {
                            return moment(new Date(parseInt(row.InputDate.slice(6, -2)))).format("DD-MM-YYYY");
                        }
                    },
                    { "data": "SupplierName", "orderable": "false" },
                    { "data": "ProductName", "orderable": "false" },
                    { "data": "ProductCode", "orderable": "false" },
                    { "data": "Unit", "orderable": "false" },


                    {
                        "data": null,
                        render: function (data, type, row, infor) {
                            return row.Price.toString().replace(".", ",").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
                        },
                        className: "dt-body-right"
                    },
                    { "data": "Quantity", "orderable": "false" },

                    {
                        "data": null,
                        render: function (data, type, row, infor) {
                            return "<div style='width:10px;display:inline'><a title='Thông tin giá phòng'  href='javascript:Detail(" +
                                row.Id +
                                ")'><i class='fa fa-search-plus'></i></a></div>"
                        }
                    }
                ]
            });


    });