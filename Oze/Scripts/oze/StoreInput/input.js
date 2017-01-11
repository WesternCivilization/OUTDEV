var Products = [];
var index = 0;
function removeA(arr) {
    var what, a = arguments, L = a.length, ax;
    while (L > 1 && arr.length) {
        what = a[--L];
        while ((ax = arr.indexOf(what)) !== -1) {
            arr.splice(ax, 1);
        }
    }
    return arr;
}

//Array.prototype.remove = function () {
//    var what, a = arguments, L = a.length, ax;
//    while (L && this.length) {
//        what = a[--L];
//        while ((ax = this.indexOf(what)) !== -1) {
//            this.splice(ax, 1);
//        }
//    }
//    return this;
//};



function RemoveItem(el) {
    Dialog.ConfirmCustom("Xác nhận",
        "Bạn có chắc xóa sản phẩm này?",
        function () {
            var elm = $(el).closest('tr');
            var id = elm.data('id');
            elm.remove();
            for (var i = 0; i < Products.length - 1; i++) {
                if (Products[i].ID === id)
                    removeA(Products, Products[i]);
            }
        });
}
var Input = function () {
    var base = this;
    this.$table = $("#table");

    //this.Validate = function () {
    //    var $form = $("#formAdd");
    //    $form.validate({
    //        rules: {
    //            Name: {
    //                required: true
    //            },
    //            Blog: {
    //                required: true
    //            },
    //            Source: {
    //                required: true
    //            },
    //            NumberPost: {
    //                required: true
    //            },
    //            Interval: {
    //                required: true
    //            },
    //            Time: {
    //                required: true
    //            },
    //            LinkNumber: {
    //                required: true
    //            }

    //        },
    //        messages: {
    //            Name: {
    //                required: "Vui lòng nhập tên"
    //            },
    //            Blog: {
    //                required: "Vui lòng chọn blog"
    //            },
    //            Source: {
    //                required: "Vui lòng chọn nguồn tin"
    //            },
    //            NumberPost: {
    //                required: "Vui lòng nhập số lượng bài viết"
    //            },
    //            Interval: {
    //                required: "Vui lòng nhập khoảng các các bài đăng"
    //            },
    //            Time: {
    //                required: "Vui lòng nhập Thời gian đăng bài"
    //            },
    //            LinkNumber: {
    //                required: "Vui lòng nhập số link chèn"
    //            }
    //        }
    //    });
    //};
    //this.Columns = function () {
    //    var obj = [
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "<input type='checkbox' id='check_all' />",
    //            field: "Index",
    //            width: "40px",
    //            align: "center",
    //            formatter: function (value, data, index) {
    //                return "<input type='checkbox' class='chk-delete'  data='" +
    //                    data.Id +
    //                    "' id='check_" +
    //                    data.Id +
    //                    "'  />";
    //            }
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "STT",
    //            field: "Index",
    //            width: "40px",
    //            align: "center",
    //            formatter: function (value, data, index) {
    //                return Sv.BootstrapTableSTT(base.$table, index);
    //            }
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Tên",
    //            field: "Name",
    //            width: "250px"
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Loại Blog",
    //            field: "BlogType",
    //            width: "200px",
    //            formatter: function (value, data) {
    //                if (data.BlogType == "1")
    //                    return "Blogs Post";
    //                return "WordPress";
    //            },
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Tên Blog",
    //            field: "BlogName",
    //            width: "170px"
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Nguồn tin",
    //            field: "SourceName",
    //            width: "170px"
    //        }),
    //        Sv.BootstrapTableColumn("DateTime",
    //        {
    //            title: "Ngày bắt đầu",
    //            field: "StartDate",
    //            width: "170px"
    //        }),
    //        Sv.BootstrapTableColumn("DateTime",
    //        {
    //            title: "Ngày bắt đầu",
    //            field: "EndDate",
    //            width: "170px"
    //        }),
    //        //  Sv.BootstrapTableColumn("String",
    //        //{
    //        //    title: "Giờ đăng",
    //        //    field: "PostDate",
    //        //    width: "170px"
    //        //}),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Trạng thái",
    //            field: "UserName",
    //            width: "100px",
    //            formatter: function (value, data) {
    //                var str = "<a  href='javascript:void(0)' class='Onoff' id='achange_" +
    //                    data.Id +
    //                    "' >" +
    //                    (data.IsEnabled ? "Kích hoạt" : "Tắt") +
    //                    "</a>";
    //                return str;
    //            },
    //            events: {
    //                'click .Onoff': function (e, value, row, index) {
    //                    Sv.AjaxPost({
    //                        Url: "/AutoPostBlog/ChangeStatus",
    //                        Data: { id: row.Id, status: (row.IsEnabled == false) }
    //                    },
    //                        function (rs) {
    //                            if (rs.ResponseCode == "01") {
    //                                row.IsEnabled = row.IsEnabled == false;
    //                                $("#achange_" + row.Id).html(row.IsEnabled ? "Kích hoạt" : "Tắt");
    //                                //base.LoadTableSearch();
    //                            } else {
    //                                Dialog.Alert(rs.Message, Dialog.Error);
    //                            }
    //                        });

    //                }
    //            }
    //        }),
    //        Sv.BootstrapTableColumn("String",
    //        {
    //            title: "Thao tác",
    //            width: "140px",
    //            formatter: function (value, data, index) {
    //                return '<i class="fa fa-pencil-square-o edit" aria-hidden="true"></i> <i class="fa fa-times delete" aria-hidden="true"></i>';

    //            },
    //            events: {
    //                'click .edit': function (e, value, row, index) {
    //                    base.ShowEditForm(row);
    //                },
    //                'click .delete': function (e, value, row, index) {
    //                    Dialog.ConfirmCustom("Confirm",
    //                        "Bạn có chắc chắn xóa bản ghi này",
    //                        function () {
    //                            Sv.AjaxPost({
    //                                Url: "/AutoPostBlog/DeletebyId",
    //                                Data: { id: row.Id }
    //                            },
    //                                function (rs) {
    //                                    if (rs.ResponseCode == "01") {
    //                                        base.LoadTableSearch();
    //                                    } else {
    //                                        Dialog.Alert(rs.Message, Dialog.Error);
    //                                    }
    //                                });
    //                        });
    //                }
    //            }
    //        })
    //    ];
    //    return obj;
    //};
    this.ResetForm = function (form) {

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

                    //   input.prop("disabled", false);
                    //input.val("");
                });
        }

        //$("#txtName").val("");
        //$("#txtKeyword").val("");
        //$("#cboSearchSubjectId").val("-1");
        //$("#cboStatus").val("-1");
    };
    //this.GetFormSearchData = function () {
    //    var obj = {};
    //    obj.Status = $("#cboStatus").val();
    //    obj.Keyword = $("#txtKeyword").val();
    //    obj.SubjectId = $("#cboSearchSubjectId").val();
    //    return obj;
    //};
    //this.ResetViewTable = function () {
    //    base.$table.bootstrapTable("resetView");
    //};
    //this.GetFormData = function () {
    //    var obj = {};
    //    obj.Id = $("#hdnId").val();
    //    obj.SourceId = $("#Source").val();
    //    obj.StartDate = $("[name=FromDate]").val();
    //    obj.EndDate = $("[name=ToDate]").val();
    //    obj.ParentSourceId = $("#SourceParent").val();
    //    obj.Name = $("#Name").val();
    //    obj.BlogId = $("#Blog").val();
    //    obj.PostNumber = $("#NumberPost").val();
    //    obj.Interval = $("#Interval").val();
    //    obj.AutoSpin = $("#Spin").val() == "1";
    //    obj.PostDate = $("#Time").val();
    //    obj.LinkNumber = $("#LinkNumber").val();
    //    obj.Groups = $("#KeywordList").val();
    //    obj.Keyword = $("#Keyword").val();
    //    obj.IsEnabled = $("#chkIsenabled").is(":checked");
    //    console.log(obj);
    //    return obj;
    //};
    //this.ShowformAdd = function () {
    //    var validator = $("#formAdd").validate();
    //    validator.resetForm();
    //    $(".modal-title").html("Thêm mới");
    //    $("#hdnId").val("0");
    //    $("#Source").val("");
    //    $("#Blog").val("");
    //    $("#SourceParent").val("");
    //    $("#NumberPost").val("");
    //    $("#Interval").val("");
    //    $("#Spin").val("0");
    //    $("#Time").val("");
    //    $("#LinkNumber").val("");
    //    $("#KeywordList").val("");
    //    $("#Keyword").val("");
    //    $("#FromDate").data("DateTimePicker").date(Sv.DefaultDate().MomentFromDate);
    //    $("#ToDate").data("DateTimePicker").date(Sv.DefaultDate().MomentToDate);

    //    $("#modalAdd").modal();
    //};
    //this.ShowEditForm = function (obj) {
    //    var validator = $("#formAdd").validate();
    //    validator.resetForm();
    //    $(".modal-title").html("Sửa");
    //    $("#hdnId").val(obj.Id);
    //    $("#Name").val(obj.Name);
    //    $("#SourceParent").val(obj.ParentSourceId);
    //    base.ChangeSource();
    //    setTimeout(function () { $("#Source").val(obj.SourceId); }, 200);
    //    $("#Blog").val(obj.BlogId);
    //    $("#NumberPost").val(obj.PostNumber);
    //    $("#Interval").val(obj.Interval);
    //    $("#Spin").val(obj.AutoSpin ? "1" : "0");
    //    $("#Time").val(obj.PostDate);
    //    $("#LinkNumber").val(obj.LinkNumber);
    //    // $("#KeywordList").val(obj.Groups);
    //    $("#Keyword").val(obj.Keyword);
    //    var group = obj.KeywordGroup.split(",");
    //    $("#KeywordList").val(group);
    //    $("#chkIsenabled").prop("checked", obj.IsEnabled);
    //    $("#FromDate").data("DateTimePicker").date(obj.StartDate);
    //    $("#ToDate").data("DateTimePicker").date(obj.EndDate);
    //    $("#modalAdd").modal();
    //};
    this.Save = function () {

        if ($("#FrmOrder").valid()) {
            if (Products.length == 0) {
                alert("Vui lòng nhập sản phẩm");
                return;
            }
            Dialog.ConfirmCustom("Xác nhận", "Bạn có chắc chắn nhập kho đơn hàng này?", function () {
                var order = {};
                order.SoPhieu = $("#SoPhieu").val();
                order.SoChungTu = $("#SoChungTu").val();
                order.NgayNhapHD = $("input[name=NgayNhapHD]").val();
                order.NgayChungTu = $("input[name=NgayChungTu]").val();
                order.StoreId = $("#StoreId").val();
                Sv.AjaxPost({
                    Url: "/StoreInput/Add",
                    Data: { order: order, orderdetails: Products }
                },
                   function (rs) {
                       console.log(rs);
                       if (rs.ResponseCode === "01") {
                           Dialog.Alert(rs.Message, Dialog.Success, function () {
                               location.href = "index";
                           });

                           //base.LoadTableSearch();
                       } else {
                           Dialog.Alert(rs.Message, Dialog.Error);
                       }
                   });
            })
        }
        return;
    };
  
   
    this.ChangeProduct = function () {
        $("#ProductCode").val("");
        $("#NhaCC").val("");
        $("#NhomDv").val("");
        $("#Unit").val("");
        $("#DonGia").val("");
        var id = $("#Product").val() != "" ? $("#Product").val() : "-1";
        Sv.AjaxPost({
            Url: "/StoreInput/GetProductInfo",
            Data: { ProductId: id }
        },
            function (rs) {
                if (rs) {
                    $("#ProductCode").val(rs.Code);
                    $("#NhaCC").val(rs.SupplierID);
                    $("#NhomDv").val(rs.ProductCateID);
                    $("#Unit").val(rs.UnitID);
                    $("#DonGia").val(rs.PriceOrder);
                } else {

                }
            });
    };
    this.AddProduct = function () {
        if ($("#formProduct").valid()) {
            index++;
            var obj = {};
            obj.ID = index;
            obj.ProductId = $("#Product").val();
            obj.ProductCode = $("#ProductCode").val();
            obj.CateId = $("#NhomDv").val();
            obj.Price = $("#DonGia").val();
            obj.SupplierId = $("#NhaCC").val();
            obj.Quantity = $("#SoLuongNhap").val();
            obj.UnitId = $("#Unit").val();
            obj.NgaySanXuat = $("input[name=NgaySanXuat]").val();
            obj.HanSuDung = $("input[name=HanSuDung]").val();
            obj.ProductName = $("#Product :selected").text();
            obj.CateName = $("#NhomDv :selected").text();

            Products.push(obj);
            var newRowContent =
                "<tr data-id='" + index + "'> <td> " +
                    $("#Product :selected").text() +
                    " </td> <td> " +
                    $("#ProductCode").val() +
                    "</td> <td> " +
                    $("#NhaCC :selected").text() +
                    " </td> <td> " +
                    $("#NhomDv :selected").text() +
                    " </td>  <td> " +
                    $("input[name=NgaySanXuat]").val() +
                    " </td> <td> " +
                    $("input[name=HanSuDung]").val() +
                    "  </td> <td> " +
                    $("#SoLuongNhap").val() +
                    " </td> <td>" +
                    $("#Unit :selected").text() +
                    " </td> <td> " +
                    $("#DonGia").val() +
                    "  </td>" +
                  //  "<td><i class='fa fa-times' onclick='RemoveItem(this)' aria-hidden='true'></i></td>" +
                    " </tr>";
            $("#product-table tbody").append(newRowContent);
            base.ResetForm($("#formProduct"));
        }

    };
};
$(document)
    .ready(function () {
        var unit = new Input();

     //   Sv.SetupDateTimeOneInput($("#NgayNhapHD"),"dd/MM/yyyy");
    //    Sv.SetupDateTimeOneInput($("#NgayChungTu"), "dd/MM/yyyy");
      //  Sv.SetupDateTimeOneInput($("#NgaySanXuat"), "dd/MM/yyyy");
      //  Sv.SetupDateTimeOneInput($("#HanSuDung"), "dd/MM/yyyy");
        Sv.AjaxPost({
            Url: "/StoreInput/GetOrderCode"
        },
         function (rs) {
             $("#SoPhieu").val(rs);
         });

        $("#Product")
            .change(function () {
                unit.ChangeProduct();
            });
        $("#btnOk")
            .click(function () {
                unit.AddProduct();
            });
        $("#btnSave").click(function () {
            unit.Save();
        });
    });