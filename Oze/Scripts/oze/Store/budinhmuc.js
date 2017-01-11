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
var Unit = function () {
    var base = this;
    this.$table = $("#table");
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


                });
        }


    };



    this.Save = function () {
        Products = [];

        if (!$("#frmOrder").valid()) {
            return;
        }
        $(".productitem").each(function (index) {
            var elm = $(".productitem")[index];
            // console.log(elm);
            if ($(elm).find("#chkauto").is(':checked')) {
                if ($(elm).data('quantity') > 0) {
                    var item = {};
                    item.StoreId = $(elm).data('storeid');
                    item.ProductId = $(elm).data('productid');
                    item.Quantity = $(elm).data('quantity');
               
                    Products.push(item);
                }
            } else {

                if ($(elm).find('#quantity').val() != "" && $(elm).find('#quantity').val() > 0) {
                    var item = {};
                    item.StoreId = $(elm).data('storeid');
                    item.ProductId = $(elm).data('productid');
                    item.Quantity = $(elm).find('#quantity').val();
                   // console.log(item);
                    Products.push(item);
                }
            }

           
        
        });

        if (Products.length == 0) {
            alert("Vui lòng chọn sản phần bù định mức");
            return;
        }

        Dialog.ConfirmCustom("Xác nhận",
            "Bạn có chắc chắn chuyển kho đơn hàng này?",
            function() {
                Sv.AjaxPost({
                        Url: "/kho/ThemPhieuBuDinhMuc",
                        Data: { storeid: $("#SrcStoreId").val(), products: Products }
                    },
                    function(rs) {
                        console.log(rs);
                        if (rs.ResponseCode === "01") {
                            Dialog.Alert(rs.Message,
                                Dialog.Success,
                                function() {
                                    location.href = "../storetransfer/index";
                                });

                        } else {
                            Dialog.Alert(rs.Message, Dialog.Error);
                        }
                    });

            });
        //if ($("#FrmOrder").valid()) {
        //    if (Products.length == 0) {
        //        alert("Vui lòng nhập sản phẩm");
        //        return;
        //    }
        //    if ($("#SrcStoreId").val() == $("#DesStoreId").val()) {
        //        alert("Kho nguồn phải khác kho đích");
        //        return;
        //    }
        //    //Check số lượng tồn


        //    Dialog.ConfirmCustom("Xác nhận", "Bạn có chắc chắn chuyển kho đơn hàng này?", function () {
        //        var order = {};
        //        order.SoPhieu = $("#SoPhieu").val();
        //        order.NgayNhapHD = $("input[name=NgayNhapHD]").val();
        //        order.SrcStoreId = $("#SrcStoreId").val();
        //        order.StoreId = $("#DesStoreId").val();


        //  })
        //  }
        //return;

    };

    //   this.Save

    this.ChangeStore = function () {
        Products = [];
        $("#product-table tbody").empty();

        Sv.AjaxPost({
            Url: "/Kho/LayTonKho",
            Data: { storeId: $("#DesStoreId").val(), productId: $("#Product").val() }
        },
            function (rs) {
                var index = 0;
                rs.forEach(function (itemList) {
                    index++;
                    var store = itemList.Store;
                    var listItem = itemList.TonKho;
                    var row =
                        "<tr> <td>" + index + "</td> <td> </td> <td><b> " + store.title + "</b> </td> <td>  </td> <td>  </td> <td>  </td> <td> </td> </tr>";
                    $("#product-table tbody").append(row);
                    listItem.forEach(function (item) {

                        row =
                            "<tr class='productitem' data-storeId='" + store.Id + "' data-productid='" + item.ProductId + "' data-quantity='" + item.Thieu + "' > <td></td> <td> <input type='number' id='quantity' name='total'/> </td> <td> " + item.ProductName + " </td> <td>  " + item.Quantity + "</td> <td> " + item.Thieu + " </td> <td> " + store.minQuality + "</td> <td> <input id='chkauto' type='checkbox'/> </td> </tr>";
                        $("#product-table tbody").append(row);
                    });

                    //  alert(store.title);


                });
            });
    };

};
$(document)
    .ready(function () {
        var unit = new Unit();
        unit.ChangeStore();

       

        $("#cboAuto")
     .change(function () {
         $("input:checkbox").not(this).prop("checked", this.checked);
     });
        //Sv.AjaxPost({
        //    Url: "/kho/GetTransferCode",
        //    data
        //},
        //function (rs) {
        //   // alert(rs);
        //     $("#SoPhieu").val(rs);
        //});

        $("#DesStoreId")
            .change(function () {
                unit.ChangeStore();
            });
        $("#Product")
           .change(function () {
               unit.ChangeStore();
           });
        $("#btnOk")
            .click(function () {
                unit.AddProduct();
            });
        $("#btnSave").click(function () {
            unit.Save();
        });
    });