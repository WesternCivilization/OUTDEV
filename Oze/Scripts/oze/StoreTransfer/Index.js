var StoreInput = function () {
    var base = this;
    this.$table = $("#table");

    this.Validate = function () {
        var $form = $("#formAdd");
        $form.validate({
            rules: {
                Name: {
                    required: true
                },
                Blog: {
                    required: true
                },
                Source: {
                    required: true
                },
                NumberPost: {
                    required: true
                },
                Interval: {
                    required: true
                },
                Time: {
                    required: true
                },
                LinkNumber: {
                    required: true
                }

            },
            messages: {
                Name: {
                    required: "Vui lòng nhập tên"
                },
                Blog: {
                    required: "Vui lòng chọn blog"
                },
                Source: {
                    required: "Vui lòng chọn nguồn tin"
                },
                NumberPost: {
                    required: "Vui lòng nhập số lượng bài viết"
                },
                Interval: {
                    required: "Vui lòng nhập khoảng các các bài đăng"
                },
                Time: {
                    required: "Vui lòng nhập Thời gian đăng bài"
                },
                LinkNumber: {
                    required: "Vui lòng nhập số link chèn"
                }
            }
        });
    };
    this.Columns = function () {
        var obj = [
            Sv.BootstrapTableColumn("String",
            {
                title: "<input type='checkbox' id='check_all' />",
                field: "Index",
                width: "40px",
                align: "center",
                formatter: function (value, data, index) {
                    return "<input type='checkbox' class='chk-delete'  data='" +
                        data.Id +
                        "' id='check_" +
                        data.Id +
                        "'  />";
                }
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "STT",
                field: "Index",
                width: "40px",
                align: "center",
                formatter: function (value, data, index) {
                    return Sv.BootstrapTableSTT(base.$table, index);
                }
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "Tên",
                field: "Name",
                width: "250px"
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "Loại Blog",
                field: "BlogType",
                width: "200px",
                formatter: function (value, data) {
                    if (data.BlogType == "1")
                        return "Blogs Post";
                    return "WordPress";
                },
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "Tên Blog",
                field: "BlogName",
                width: "170px"
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "Nguồn tin",
                field: "SourceName",
                width: "170px"
            }),
            Sv.BootstrapTableColumn("DateTime",
            {
                title: "Ngày bắt đầu",
                field: "StartDate",
                width: "170px"
            }),
            Sv.BootstrapTableColumn("DateTime",
            {
                title: "Ngày bắt đầu",
                field: "EndDate",
                width: "170px"
            }),
            //  Sv.BootstrapTableColumn("String",
            //{
            //    title: "Giờ đăng",
            //    field: "PostDate",
            //    width: "170px"
            //}),
            Sv.BootstrapTableColumn("String",
            {
                title: "Trạng thái",
                field: "UserName",
                width: "100px",
                formatter: function (value, data) {
                    var str = "<a  href='javascript:void(0)' class='Onoff' id='achange_" +
                        data.Id +
                        "' >" +
                        (data.IsEnabled ? "Kích hoạt" : "Tắt") +
                        "</a>";
                    return str;
                },
                events: {
                    'click .Onoff': function (e, value, row, index) {
                        Sv.AjaxPost({
                            Url: "/AutoPostBlog/ChangeStatus",
                            Data: { id: row.Id, status: (row.IsEnabled == false) }
                        },
                            function (rs) {
                                if (rs.ResponseCode == "01") {
                                    row.IsEnabled = row.IsEnabled == false;
                                    $("#achange_" + row.Id).html(row.IsEnabled ? "Kích hoạt" : "Tắt");
                                    //base.LoadTableSearch();
                                } else {
                                    Dialog.Alert(rs.Message, Dialog.Error);
                                }
                            });

                    }
                }
            }),
            Sv.BootstrapTableColumn("String",
            {
                title: "Thao tác",
                width: "140px",
                formatter: function (value, data, index) {
                    return '<i class="fa fa-pencil-square-o edit" aria-hidden="true"></i> <i class="fa fa-times delete" aria-hidden="true"></i>';

                },
                events: {
                    'click .edit': function (e, value, row, index) {
                        base.ShowEditForm(row);
                    },
                    'click .delete': function (e, value, row, index) {
                        Dialog.ConfirmCustom("Confirm",
                            "Bạn có chắc chắn xóa bản ghi này",
                            function () {
                                Sv.AjaxPost({
                                    Url: "/AutoPostBlog/DeletebyId",
                                    Data: { id: row.Id }
                                },
                                    function (rs) {
                                        if (rs.ResponseCode == "01") {
                                            base.LoadTableSearch();
                                        } else {
                                            Dialog.Alert(rs.Message, Dialog.Error);
                                        }
                                    });
                            });
                    }
                }
            })
        ];
        return obj;
    };
    this.FormSearchReset = function () {
        var form = $("#formSearch");
        if (form.length) {
            form.find("input, textarea, select")
                .each(function (index) {
                    var input = $(this);
                    if (input.is(":radio, :checkbox")) {
                        input.prop("checked", this.defaultChecked);
                    } else if (input.is("select")) {
                        input.val("-1");
                    } else {
                        input.val("");
                    }

                    input.prop("disabled", false);
                    //input.val("");
                });
        }

        //$("#txtName").val("");
        //$("#txtKeyword").val("");
        //$("#cboSearchSubjectId").val("-1");
        //$("#cboStatus").val("-1");
    };
    this.GetFormSearchData = function () {
        var obj = {};
        obj.Status = $("#cboStatus").val();
        obj.Keyword = $("#txtKeyword").val();
        obj.SubjectId = $("#cboSearchSubjectId").val();
        return obj;
    };
    this.ResetViewTable = function () {
        base.$table.bootstrapTable("resetView");
    };
    this.GetFormData = function () {
        var obj = {};
        obj.Id = $("#hdnId").val();
        obj.SourceId = $("#Source").val();
        obj.StartDate = $("[name=FromDate]").val();
        obj.EndDate = $("[name=ToDate]").val();
        obj.ParentSourceId = $("#SourceParent").val();
        obj.Name = $("#Name").val();
        obj.BlogId = $("#Blog").val();
        obj.PostNumber = $("#NumberPost").val();
        obj.Interval = $("#Interval").val();
        obj.AutoSpin = $("#Spin").val() == "1";
        obj.PostDate = $("#Time").val();
        obj.LinkNumber = $("#LinkNumber").val();
        obj.Groups = $("#KeywordList").val();
        obj.Keyword = $("#Keyword").val();
        obj.IsEnabled = $("#chkIsenabled").is(":checked");
        console.log(obj);
        return obj;
    };
    this.ShowformAdd = function () {
        var validator = $("#formAdd").validate();
        validator.resetForm();
        $(".modal-title").html("Thêm mới");
        $("#hdnId").val("0");
        $("#Source").val("");
        $("#Blog").val("");
        $("#SourceParent").val("");
        $("#NumberPost").val("");
        $("#Interval").val("");
        $("#Spin").val("0");
        $("#Time").val("");
        $("#LinkNumber").val("");
        $("#KeywordList").val("");
        $("#Keyword").val("");
        $("#FromDate").data("DateTimePicker").date(Sv.DefaultDate().MomentFromDate);
        $("#ToDate").data("DateTimePicker").date(Sv.DefaultDate().MomentToDate);

        $("#modalAdd").modal();
    };
    this.ShowEditForm = function (obj) {
        var validator = $("#formAdd").validate();
        validator.resetForm();
        $(".modal-title").html("Sửa");
        $("#hdnId").val(obj.Id);
        $("#Name").val(obj.Name);
        $("#SourceParent").val(obj.ParentSourceId);
        base.ChangeSource();
        setTimeout(function () { $("#Source").val(obj.SourceId); }, 200);
        $("#Blog").val(obj.BlogId);
        $("#NumberPost").val(obj.PostNumber);
        $("#Interval").val(obj.Interval);
        $("#Spin").val(obj.AutoSpin ? "1" : "0");
        $("#Time").val(obj.PostDate);
        $("#LinkNumber").val(obj.LinkNumber);
        // $("#KeywordList").val(obj.Groups);
        $("#Keyword").val(obj.Keyword);
        var group = obj.KeywordGroup.split(",");
        $("#KeywordList").val(group);
        $("#chkIsenabled").prop("checked", obj.IsEnabled);
        $("#FromDate").data("DateTimePicker").date(obj.StartDate);
        $("#ToDate").data("DateTimePicker").date(obj.EndDate);
        $("#modalAdd").modal();
    };
    this.Save = function () {
        var $form = $("#formAdd").on();
        if (!$form.valid()) {
            return;
        }

        if ($("#hdnId").val() == "0") {
            //Check Validate
            Sv.AjaxPost({
                Url: "/AutoPostBlog/Add",
                Data: { autopost: base.GetFormData() }
            },
                function (rs) {
                    if (rs.ResponseCode == "01") {
                        Dialog.Alert(rs.Message, Dialog.Success);
                        $("#modalAdd").modal("hide");
                        base.LoadTableSearch();
                    } else {
                        Dialog.Alert(rs.Message, Dialog.Error);
                    }
                });
        } else {
            Sv.AjaxPost({
                Url: "/AutoPostBlog/Edit",
                Data: { autopost: base.GetFormData() }
            },
                function (rs) {
                    if (rs.ResponseCode == "01") {
                        Dialog.Alert(rs.Message, Dialog.Success);
                        $("#modalAdd").modal("hide");
                        base.LoadTableSearch();
                    } else {
                        Dialog.Alert(rs.Message, Dialog.Error);
                    }
                });

        }


    };
    this.DeleteList = function () {
        //Get list selected
        var obj = [];
        $("input:checkbox.chk-delete")
            .each(function () {
                if (this.checked) {
                    var id = this.getAttribute("data");
                    obj.push(id);
                }
            });
        if (obj.length == 0) {
            alert("Chọn bản ghi để xóa");
            return;
        }
        Dialog.ConfirmCustom("Confirm",
            "Bạn có chắc chắn xóa các bản ghi này",
            function () {
                Sv.AjaxPost({
                    Url: "/AutoPostBlog/Delete",
                    Data: { ids: obj }
                },
                    function (rs) {
                        if (rs.ResponseCode == "01") {
                            base.LoadTableSearch();
                        } else {
                            Dialog.Alert(rs.Message, Dialog.Error);
                        }
                    });
            });
    };
    this.LoadTableSearch = function () {
        base.$table.bootstrapTable("refreshOptions",
        {
            responseHandler: function (res) {
                return Sv.ResponseHandlerSearch(res, base.$searchModal, base.$table);
            }
        });
    };
    this.ChangeSource = function () {
        var id = $("#SourceParent").val() != "" ? $("#SourceParent").val() : "-1";
        Sv.AjaxPost({
            Url: "/Crawl/GetSource",
            Data: { parentId: id }
        },
            function (rs) {
                $("#Source").empty();
                $("#Source").append('<option value="">--Chọn mục--</option>');
                rs.item.forEach(function (i) {
                    $("#Source").append("<option value='" + i.Id + "'>" + i.Name + "</option>");
                });
            });
    };
};
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
                    "url": "/StoreTransfer/List",
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
                            return "<div style='width:10px;display:inline'><a title='Chi tiết'  href='javascript:Detail(" +
                                row.Id +
                                ")'><i class='fa fa-search-plus'></i></a></div>"
                        }
                    }
                ]
            });


    });