
var $modalMail = $("#modalDetails");
//phụ trội quá giờ theo trả ngày
var counserviceteam = 0;
var counservice = 0;
$(document).ready(function () {

    $("#btnDetailPrint").click(function () {
        $("#modalDetails").off('show.bs.modal');
        $("#modalDetails")
            .on('show.bs.modal',
                function () {
                    var checkinID = parseInt($("#CheckInID").val());
                    var IsboolService = $('#ckService').is(":checked");
                    var IsboolRoom = $('#ckRoom').is(":checked");
                    var Leave_DateCheckOut = $("#Leave_DateCheckOut").val();
                    var khunggio = $("#dllKhungGio").val();

                    //RuleValidateSubmitToAdd();
                    $("#modalDetails .modal-body-content").html('<p>loading..</p>');
                    $.post("/RoomCheckOut/ViewPrintPay", { checkinID: checkinID, IsboolService: IsboolService, IsboolRoom: IsboolRoom, tDate: Leave_DateCheckOut, khunggio: khunggio },
                        function (rs) {
                            $("#modalDetails .modal-body-content").html(rs);
                            $("body").addClass("modal-open1");

                            //g_Utils.SetAmount();
                        });

                    $("#modalDetails button#btnUpdateDetail").css("display", "inline");
                    //$("#btnUpdateDetail").off("click");


                });
        //$("#modalDetails").css('width', '750px');
        //$("#modalDetails").css('margin', '100px auto 100px auto');
        $("#modalDetails").modal("show");
        //g_Utils.SetAmount();
    });


    $("#btnPay").click(function () {
        bootbox.confirm("Bạn có chắc chắn muốn thanh toán cho khách hàng này không?", function (result) {
            if (!result) return;
            //var r = confirm("Bạn có chắc chắn muốn thanh toán cho khách hàng này không?");
            //if (r === true) {
            var checkinID = parseInt($("#CheckInID").val());
            var Leave_DateCheckOut = $("#Leave_DateCheckOut").val();
            showDialogLoading();
            $.post("/RoomCheckOut/PaymentCheckOut",
                { checkInid: checkinID, tdate: Leave_DateCheckOut, khunggio: $("#dllKhungGio").val() },
                function (rs) {
                    if (rs.Status === "01") {
                        bootbox.alert(rs.Message,
                            function () {
                                $("body").addClass("modal-open1");
                                hideDialogLoading();
                            });
                    } else {
                        alert(rs.Message);
                        hideDialogLoading();
                    }
                    //g_Utils.SetAmount();
                });

            //} else {
            //    alert("adad");
            //}
            //g_Utils.SetAmount();
        });
    });
    $("#btnPayBill").click(function () {
        bootbox.confirm("Bạn có chắc chắn muốn thanh toán cho khách hàng này không?", function (result) {
            if (!result) return;
            var checkinID = parseInt($("#CheckInID").val());
            var Leave_DateCheckOut = $("#Leave_DateCheckOut").val();
            showDialogLoading();
            $.post("/RoomCheckOut/PayBilltCheckOut",
                { checkInid: checkinID, tdate: Leave_DateCheckOut, khunggio: $("#dllKhungGio").val() },
                function (rs) {
                    if (rs.Status === "01") {
                        bootbox.alert(rs.Message,
                            function () {
                                $("body").addClass("modal-open1");
                                hideDialogLoading();
                            });
                    } else {
                        alert(rs.Message);
                        hideDialogLoading();
                    }
                    //g_Utils.SetAmount();
                });
        });
        //} else {
        //    alert("adad");
        //}

        //g_Utils.SetAmount();
    });
    $("#btnExport").click(function () {
        $("#formExport").submit();
    });
    $("#btnPrint").click(function () {



        var checkinID = parseInt($("#CheckInID").val());
        var IsboolService = $('#ckService').is(":checked");
        var IsboolRoom = $('#ckRoom').is(":checked");
        var Leave_DateCheckOut = $("#Leave_DateCheckOut").val();
        var khunggio = $("#dllKhungGio").val();


        $.post("/RoomCheckOut/PrintPay", { checkinID: checkinID, tDate: Leave_DateCheckOut, khunggio: khunggio },
                      function (rs) {

                          var stt = 0;

                          var totalSale = 0;
                          var mywindow = window.open('', '', 'height=768,width=1024,scrollbars=yes');
                          mywindow.document.write('<html>');
                          mywindow.document.write('<head>');
                          mywindow.document.write('<link rel="stylesheet" href="/Content/bootstrap.min.css" type="text/css" />');
                          mywindow.document.write('<link rel="stylesheet" href="/Content/hungpvCustom.css" type="text/css" />');
                          mywindow.document.write('<link rel="stylesheet" href="/Content/hungpvprint.css" type="text/css" />');
                          mywindow.document.write('</head>');
                          mywindow.document.write('<body class="container-fluid">');
                          mywindow.document.write('<button class="button-print btn btn-primary noprint" onclick="window.print();"><i class="glyphicon glyphicon-print"></i></button>');
                          mywindow.document.write('<div class="row">');

                          mywindow.document.write('<div class="col-xs-7"> <div class="row">' +
                              ' <div class="col-xs-12">' +
                              ' <label class="col-xs-6 control-label ">Mã phiếu </label>' +
                              ' <div class="col-xs-6">  </div> </div>' +
                              ' <div class="col-xs-12"> ' +
                              '<label class="col-xs-6 control-label ">Họ tên khách </label>' +
                              ' <div class="col-xs-6"> ' + rs.InforCustomer.CustomerName + ' </div>' +
                              ' </div> <div class="col-xs-12">' +
                              ' <label class="col-xs-6 control-label ">Số điện thoại ' +
                              '</label> <div class="col-xs-6"> ' + (rs.InforCustomer.Phone === null ? "" : rs.InforCustomer.Phone) + ' </div> </div>' +
                              ' <div class="col-xs-12"> <label class="col-xs-6 control-label ">Ngày giờ thanh toán </label> ' +
                              '<div class="col-xs-6"> ' + moment(new Date()).format('DD/MM/YYYY HH:mm') +
                              ' </div> </div> </div> </div>' +
                              ' <div class="col-xs-5"> <div class="col-xs-12 add-textcenter-hungpv"><img src="/images/icon/icon-logoanhung.png" style="width: 50px;display: inline;">' +
                              ' <h4 style="color: goldenrod;display: inline;">' + rs.InforCustomer.HotelName + '</h4> </div> <div class="col-xs-12 add-textcenter-hungpv">' +
                              ' <p style="font-size:12px">Địa chỉ: Số 2, Ngõ 33 phố Phạm Tấn Tài, Q. Cầu Giấy, Tp. Hà Nội</p> ' +
                              '</div> <div class="col-xs-12 add-textcenter-hungpv"> ' +
                              '<p style="font-size:12px">ĐT:0437 939200/DĐ:0936 149 286 - 0901 790 116</p> </div> </div>');

                          mywindow.document.write('<div class="col-xs-12">' +
                              ' <table id="HistoryCustomer" class="table table-striped table-bordered" cellspacing="0" width="100%">' +
                              ' <thead>' +
                              ' <tr>' +
                              ' <th>STT</th> ' +
                              '<th>Nội dung</th>' +
                              ' <th>Đơn vị</th>' +
                              ' <th>Giá</th>' +
                              ' <th>Số lượng</th> ' +
                              '<th>Thành tiền</th> ' +
                              '<th>Ghi chú</th> ' +
                              '</tr>' +
                              ' </thead> ');

                          if (IsboolRoom) {
                              mywindow.document.write('<tr><th colspan="7" style="text-align: left">Tiền phòng</th></tr>');

                              for (var j = 0; j < rs.GetListPriceEstimate.length; j++) {
                                  stt = j + 1;
                                  totalSale = totalSale + parseFloat(rs.GetListPriceEstimate[j].price);
                                  mywindow.document.write(' <tr>' +
                                      '<td>' +
                                      stt +
                                      '</td>' +
                                      ' <td>' +
                                      moment(new Date(parseInt(rs.GetListPriceEstimate[j].dtFrom.slice(6, -2)))).format("DD/MM/YYYY HH:mm") + " - " + moment(new Date(parseInt(rs.GetListPriceEstimate[j].dtTo.slice(6, -2)))).format("DD/MM/YYYY HH:mm") +
                                      '</td> ' +
                                      ' <td>Tiền phòng</td> ' +
                                      '<td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListPriceEstimate[j].price.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListPriceEstimate[j].quantiy.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListPriceEstimate[j].price.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td></td> ' +
                                      ' </tr>');
                              }

                          }
                          if (IsboolService) {


                              mywindow.document.write('<tr><th colspan="7" style="text-align: left">Dịch vụ</th></tr>');
                              for (var i = 0; i < rs.GetListCustomerServices.length; i++) {
                                  stt = i + 1;
                                  totalSale = totalSale + parseFloat(rs.GetListCustomerServices[i].TotalSale);
                                  mywindow.document.write(' <tr>' +
                                      '<td>' +
                                      stt +
                                      '</td>' +
                                      ' <td>' +
                                      rs.GetListCustomerServices[i].Name +
                                      '</td> ' +
                                      ' <td>' +
                                      rs.GetListCustomerServices[i].UnitName +
                                      '</td> ' +
                                      '<td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListCustomerServices[i].SalePrice.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListCustomerServices[i].Quantity.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td  class="amount-double-mask dt-body-right">' +
                                      rs.GetListCustomerServices[i].TotalSale.toString()
                                      .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                                      '</td>' +
                                      ' <td></td> ' +
                                      ' </tr>');
                              }
                          }
                          mywindow.document.write(' </table> ' +
                          '</div> <div class="col-xs-12"> <div class="col-xs-6">' +
                          ' </div> <div class="col-xs-6">' +
                          ' <label class="col-xs-6 control-label ">Tổng tiền </label> ' +
                              '<div class="col-xs-4"> ' + totalSale.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + ' </div>' +
                               '<div class="col-xs-2"> VNĐ </div>' +
                          ' </div> <div class="col-xs-6"> </div> ' +
                              '<div class="col-xs-6">' +
                          ' <label class="col-xs-6 control-label ">Trả trước </label> ' +
                          '<div class="col-xs-4"> ' + rs.InforCustomer.Deposit.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + ' </div>' +
                               '<div class="col-xs-2"> VNĐ </div>' +
                              '</div> <div class="col-xs-6"> ' +
                          '</div> <div class="col-xs-6"> <label class="col-xs-6 control-label ">Giảm trừ </label> ' +
                         '<div class="col-xs-4"> ' + rs.InforCustomer.Deduction.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + ' </div>' +
                               '<div class="col-xs-2"> VNĐ </div>' +
                              '</div> <div class="col-xs-6"> ' +
                          '</div> <div class="col-xs-6"> ' +
                          '<label class="col-xs-6 control-label ">Tổng tiền thanh toán </label>' +
                          '<div class="col-xs-4"> ' + (parseFloat(totalSale) - parseFloat(rs.InforCustomer.Deduction) - parseFloat(rs.InforCustomer.Deposit)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + ' </div>' +
                               '<div class="col-xs-2"> VNĐ </div>' +
                              ' </div> </div> <div class="col-xs-9"> ' +
                          '</div> <div class="col-xs-3"> Ngày...tháng... năm... </div> <br/> <div class="col-xs-12 "> ' +
                          '<div class="row"> <div class="col-xs-4 add-textcenter-hungpv "> ' +
                          '<label class="col-xs-12 control-label ">Khách hàng </label> <i>(Ký ghi rõ họ tên)</i> ' +
                          '</div> <div class="col-xs-4 add-textcenter-hungpv">' +
                          ' <label class="col-xs-12 control-label ">Quản lý </label> <i>(Ký ghi rõ họ tên)</i> ' +
                          '</div> <div class="col-xs-4 add-textcenter-hungpv">' +
                          ' <label class="col-xs-12 control-label ">Người lập </label> <i>(Ký ghi rõ họ tên)</i> </div> </div> </div>');

                          mywindow.document.write("</div>");
                          mywindow.document.write('</body></html>');
                          mywindow.document.close();
                          mywindow.focus();
                      });
    });
    //thêm dịch vụ
    $("#AddService").click(function () {
        var quantity = parseInt($("#QuantityProduct").val());
        var checkinID = parseInt($("#CheckInID").val());
        var hotelID = parseInt($("#SysHotelID").val());
        var customerid = parseInt($("#CustomerId").val());
        var productId = parseInt($("#slProductID").val());

        //$("#addptqgtn").click(function () {
        if (productId === 0) {
            alert('Vui lòng chọn dịch vụ !');
            return;
        }
        if (quantity === 0) {
            alert('Chưa nhập số lượng !');
            return;
        }
        //return;
        showDialogLoading();

        var url = "/RoomCheckOut/InsertService";
        $.post(url,
            {
                productId: productId,
                checkinID: checkinID,
                hotelID: hotelID,
                customerid: customerid,
                Quantity: quantity
            },
            function (rs) {
                hideDialogLoading();
                //console.log(rs.Data)
                if (rs.Status === "01") {
                    $(".listService").append('<div class="itemService' + rs.Data.cussvID + '">' +
                        ' <div class="form-group">' +
                        ' <label class="col-xs-4 control-label" id="" Item="">' + rs.Data.Name + ' </label> ' +
                          ' <label class="col-xs-3 control-label" id="" Item="">' + (rs.Data.datecreated == null ? "" : moment(new Date(parseInt(rs.Data.datecreated.slice(6, -2)))).format("DD-MM-YYYY hh:mm")) + ' </label> ' +
                           ' <label class="col-xs-2 control-label" id="" Item="">' + rs.Data.SalePrice + ' x ' + rs.Data.Quantity + ' = </label> ' +
                        '<label class="col-xs-2 control-label amount-double-mask" id="IditemService' + rs.Data.cussvID + '" Item="' + rs.Data.cussvID + '" style="text-align: right;"> ' + rs.Data.TotalSale + ' </label>' +
                        ' <div class="col-xs-1">' +
                        ' <a href="#"><i class="fa fa-remove" onclick="removeptqgtn(' + rs.Data.cussvID + ')"></i></a> </div> </div> </div>');
                    $("#QuantityProduct").val(0);
                    $("#slProductID").val(0);
                } else {
                    alert("Thêm mới dịch vụ thất bại");
                }
                var total = $("#txtTotalPrice").val().replace('.', '');
                var totalpay = $("#txtTotalPay").val().replace('.', '');
                //total += 100000;
                $("#txtTotalPrice").val(parseFloat(total) + parseFloat(rs.Data.TotalSale));
                $("#txtTotalPay").val(parseFloat(totalpay) + parseFloat(rs.Data.TotalSale));
                g_Utils.SetAmount();
            });

    });
    //Theemdichj vụ khác
    $("#AddServiceOrther").click(function () {
        var name = $("#ServerOther").val();
        var PriceOther = parseFloat($("#PriceOther").val());
        var checkinID = parseInt($("#CheckInID").val());
        var hotelID = parseInt($("#SysHotelID").val());
        var customerid = parseInt($("#CustomerId").val());
        counserviceteam++;
        //$("#addptqgtn").click(function () {
        if (name === "") {
            alert('Vui lòng nhập dịch vụ !');
            return;
        }
        if (PriceOther === 0 || PriceOther === "") {
            alert('Chưa nhập giá dịch vụ !');
            return;
        }


        var url = "/RoomCheckOut/InsertNewOtherService";
        $.post(url,
            {
                name: name,
                checkinID: checkinID,
                hotelID: hotelID,
                customerid: customerid,
                price: PriceOther
            },
            function (rs) {
                hideDialogLoading();
                //console.log(rs.Data)
                if (rs.Status === "01") {
                    $(".listServiceOrther").append('<div class="itemServiceOrther' +
                        parseInt(rs.Data.Id) +
                        '"> ' +
                        '<div class="form-group">' +
                        '<label class="col-xs-4 control-label" id="" Item=""> ' +
                        name +
                        ' </label>' +
                        '<label class="col-xs-4 control-label" id="" Item=""> ' +
                        moment(new Date(parseInt(rs.Data.datecreated.slice(6, -2)))).format("DD-MM-YYYY hh:mm") +
                        ' </label>' +
                        '<label class="col-xs-3 control-label amount-double-mask" style="text-align: right;" id="IditemService' +
                        rs.Status +
                        '" Item=""> ' +
                        PriceOther.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') +
                        ' </label>' +
                        '<div class="col-xs-1"> <a href="#">' +
                        '<i class="fa fa-remove" onclick="removeOtherService(' +
                        parseInt(rs.Data.Id) +
                        ')"></i>' +
                        '</a> ' +
                        '</div> ' +
                        '</div> ' +
                        '</div>');

                    $("#ServerOther").val('');
                    $("#PriceOther").val(0);
                    var total = $("#txtTotalPrice").val().replace('.', '');
                    var totalpay = $("#txtTotalPay").val().replace('.', '');
                    $("#txtTotalPrice").val(parseFloat(total) + parseFloat(PriceOther));
                    $("#txtTotalPay").val(parseFloat(totalpay) + parseFloat(PriceOther));
                } else {
                    alert(rs.Message);
                    return;
                }

            });

    });
    $("#btnSendMail").click(function () {
        $.post("/RoomCheckOut/SendMail",
                         function (rs) {
                             alert(rs.Message);
                             $("body").addClass("modal-open1");

                         });
    });

    var dateoldl = "";
    $(".date-add-picker_tdate_full_notdefault").datetimepicker({
        //locale: 'vi',
        debug: false,
        format: 'DD/MM/YYYY HH:mm',
        showTodayButton: true,
        defaultDate: new Date(),
        showClose: false
    });
    //hungpv
    $("#btnSaveEdit").click(function () {
        var check = $("#btnSaveEdit").attr("class").includes('glyphicon-pencil');
        if (check) {
            $("#btnSaveEdit").removeClass("glyphicon-pencil");
            $("#btnSaveEdit").addClass("glyphicon-ok");
            $("#Leave_DateCheckOut").prop("disabled", false);
            $("#btnSaveCance").prop("disabled", false);
            $("#btnDetailPrint").prop("disabled", true);
            $("#btnPay").prop("disabled", true);
            $("#btnCancelCheckIn").prop("disabled", true);

        } else {

            $("#btnSaveEdit").addClass("glyphicon-pencil");
            $("#btnSaveEdit").removeClass("glyphicon-ok");
            $("#Leave_DateCheckOut").prop("disabled", true);
            $("#btnDetailPrint").prop("disabled", false);
            $("#btnPay").prop("disabled", false);
            $("#btnCancelCheckIn").prop("disabled", false);
            $("#btnSaveCance").prop("disabled", true);


            if ($("#Leave_DateCheckOut").val() === $("#Leave_DateOld").val()) {
                hideDialogLoading();
                return;

            }

            $("#Leave_DateOld").val($("#Leave_DateCheckOut").val());
            callToPrice();
        }
        //.removeClass("hide");
        //alert(111);
    });
    $("#btnSaveCance").click(function () {
        $("#btnSaveEdit").addClass("glyphicon-pencil");
        $("#btnSaveEdit").removeClass("glyphicon-ok");
        $("#Leave_DateCheckOut").prop("disabled", true);
        $("#btnDetailPrint").prop("disabled", false);
        $("#btnPay").prop("disabled", false);
        $("#btnCancelCheckIn").prop("disabled", false);
        $("#btnSaveCance").prop("disabled", true);
        $("#Leave_DateCheckOut").val($("#Leave_DateOld").val());

    });
    $("#btnSaveCance").prop("disabled", true);
    $(".date-add-picker_tdate_full_notdefault").on("dp.change", function (e) {
        if (e.date._d !== e.oldDate._d) {
            $("#Leave_DateOld").val(moment(e.oldDate._d).format("DD-MM-YYYY HH:mm"))

        }
    });
    $("#dllKhungGio").change(function () {
        callToPrice();
    });
    function callToPrice() {
        showDialogLoading("Đang tính giá");
        var checkinID = parseInt($("#CheckInID").val());
        var Leave_DateCheckOut = $("#Leave_DateCheckOut").val();
        var khunggio = $("#dllKhungGio").val();
        $.post("/RoomCheckOut/PayRoomPrice", { tDate: Leave_DateCheckOut, checkinID: checkinID, khunggio: khunggio },
          function (rs) {
              $("body").addClass("modal-open1");
              $("#PriceEstimate").html("");
              for (var i = 0; i < rs.listRoomPrice.length; i++) {
                  $("#PriceEstimate").append('<label class="col-xs-12 control-label"> ' + moment(new Date(parseInt(rs.listRoomPrice[i].dtFrom.slice(6, -2)))).format("DD-MM-YYYY HH:mm") + '- ' + moment(new Date(parseInt(rs.listRoomPrice[i].dtTo.slice(6, -2)))).format("DD-MM-YYYY HH:mm") + ' -' + rs.listRoomPrice[i].quantiy.toString()
                                     .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + ' ' + rs.listRoomPrice[i].titlePrice + ' = ' + rs.listRoomPrice[i].price.toString()
                                     .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') + '' + '-<span style="color:red">' + rs.listRoomPrice[i].pricePolicyName + '</span>');
              }

              $("#txtTotalPrice").val(rs.Total);
              $("#txtTotalPay").val(rs.totalpay);
              hideDialogLoading();
          });
    }
    //$("#Leave_Date").on("dp.change", function (e) {
    //    alert('hey');

    //});
    //$('#Leave_Date').change(function () {
    //    alert('hey');
    ////});
    //$('#date-Leave_DateCheckOut').change(function () {
    //    console.log($(this).val())
    //    console.log(99112222)
    //});
    //$('#Leave_DateCheckOut').blur(function () {
    //    console.log($(this).val())
    //   console.log(9911)
    //});
    //$('#Leave_Date').datepicker().on('changeDate', function (ev) {
    //    $('#date-daily').change();
    //});
    //$("#Leave_Date").change(function () {

    //    if ($(this).val() == "" || dateoldl === $(this).val()) {
    //        //    var date = new Date();
    //        //    var _fn = date.getDate();
    //        //    var _ft = date.getMonth();
    //        //    var _fna = date.getFullYear();
    //        //    var ft = (_ft + 1);
    //        //    if (ft < 10)
    //        //        ft = "0" + ft;
    //        //    var fdate = (_fn + "/" + ft + "/" + _fna);
    //        //    $(this).val(fdate);

    //    } else {
    //        dateoldl = $(this).val();
    //        $.post("/RoomCheckOut/PayRoomPrice", { tDate: $(this).val() },
    //       function (rs) {
    //           $("body").addClass("modal-open1");
    //           //$("#PriceEstimate").html("");
    //           $("#PriceEstimate").append('<label class="col-xs-12 control-label"> ' + '' + 0 + '' + '- ' + 0 + ' ' + '- ' + 0 + ' ' + '-' + 0 + '');
    //       });
    //    }
    //});
});


//
function removeptqgtn(id) {
    //var total = $("#QuantityProduct").val();
    //total == total-;
    ////total += 100000;
    //$("#QuantityProduct").val(total)
    showDialogLoading();
    $.post("/RoomCheckOut/LockCustomerService",
    {
        cussvID: id
    },
               function (rs) {
                   hideDialogLoading();
                   //console.log(rs.Data)
                   if (rs.Status === "01") {
                       var price = $("#IditemService" + id).html().replace('.', '');

                       $('.ItemService' + id).remove();
                       alert("Xóa dịch vụ thành công.");
                       var total = $("#txtTotalPrice").val().replace('.', '');
                       var totalpay = $("#txtTotalPay").val().replace('.', '');
                       //total = total - price;
                       $("#txtTotalPrice").val(parseFloat(total) - parseFloat(price));
                       $("#txtTotalPay").val(parseFloat(totalpay) - parseFloat(price));
                       g_Utils.SetAmount();
                   } else {
                       alert("Xóa dịch vụ thất bại!");
                   }

               });

}
//
function removeOtherService(id) {

    showDialogLoading();
    var price = $("#IditemService" + id).html().replace('.', '');
    var total = $("#txtTotalPrice").val().replace('.', '');
    var totalpay = $("#txtTotalPay").val().replace('.', '');
    $.post("/RoomCheckOut/LockCustomerService",
    {
        cussvID: id
    },
               function (rs) {
                   hideDialogLoading();
                   //console.log(rs.Data)
                   if (rs.Status === "01") {
                       $('.itemServiceOrther' + id).remove();
                       alert("Xóa dịch vụ thành công.");
                       $("#txtTotalPrice").val(parseFloat(total) - parseFloat(price));
                       $("#txtTotalPay").val(parseFloat(totalpay) - parseFloat(price));
                       //g_Utils.SetAmount();
                   } else {
                       alert("Xóa dịch vụ thất bại!");
                   }

               });

}
function dateChanged(ev) {
    alert(2);
}
function showRoomMateList(customerid) {
    showDialogLoading();
    $("#modalDetails1").off('show.bs.modal');
    $("#modalDetails1 .modal-body-content").html('<p>loading..</p>');
    $("#modalDetails1").on('show.bs.modal', function () {
        $("#TittleBox").html("Bạn cùng phòng");
        $.post("/CustomerManage/GetRoomMateDetail", { customerid: customerid }, function (rs) {
            hideDialogLoading();
            $("#modalDetails1 .modal-body-content").html(rs);
            $("#modalDetails1 button#btnUpdateDetail").css("display", "none");
        });
    });
    $("#modalDetails1 button#btnUpdateDetail").css("display", "none");
    $("#modalDetails1 button#btnUpdateDetail").css("disabled", "none");
    $("#modalDetails1 button#btnUndoRoom").css("disabled", "none");
    $("#modalDetails1 button#btnChangeRoom").css("disabled", "none");

    $("#modalDetails1 button#btnUndoRoom").css("display", "none");
    $("#modalDetails1 button#btnChangeRoom").css("display", "none");
    $("#modalDetails1").modal("show");
};
function AddUsingRoomDialog(checkinID)
{
    $("#modalDetails1").off('show.bs.modal');
    $("#modalDetails1").on('show.bs.modal', function () {
        $("#TittleBox").html("Thêm khách ở cùng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails1 .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerArriveManage/AddUsingCustomers", { checkinID: checkinID }, function (rs) {
            $("#modalDetails1 .modal-body-content").html(rs);
            SetDate(".birthday");
            g_Utils.SetDateDefaultAdd();
            g_Utils.ConfigAutocomplete('#Name', "/CustomerArriveManage/SelectCustomer", "Name", "Name",
                function (item) {
                    var data = (JSON.parse(item.object));
                    //"" CitizenshipCode : 238 Company : "" CountryId : null CreateBy : 1 CreateDate : "/Date(1481792366727)/" CustomerTypeID : 0 DOB : "/Date(1480525200000)/" Email : "" Fax : "" HotelCode : "OzeHotel0001" Id : 73 IdentifyNumber : "125309881" Mobile : "01658756994" ModifyBy : null ModifyDate : null Name : "Ngọc Lam Man" Phone : "" ReservationID : null Sex : 1 SourceID : 0 Status : true SysHotelID : null TaxCode : "" TeamMergeSTT : null TeamSTT : null
                    console.log(data);
                    $("#modalDetails1 #formDetail :input").attr("disabled", true);
                    $('#Name').prop('disabled', false);
                    $("#formDetail  :input[name='Name']").val(data.Name);
                    $("#formDetail  :input[name='CustomerId']").val(data.Id);
                    $("#formDetail  :input[name='Company']").val(data.Company);
                    $("#formDetail  :input[name='CountryId']").val(data.CountryId == null ? "" : data.CountryId);
                    $("#formDetail  :input[name='Sex']").val(data.Sex);
                    $("#formDetail  :input[name='Email']").val(data.Email);
                    $("#formDetail  :input[name='IdentifyNumber']").val(data.IdentifyNumber);
                    $("#formDetail  :input[name='TeamMergeSTT']").val(data.TeamMergeSTT == null ? "0" : data.TeamMergeSTT);
                    $("#formDetail  :input[name='TeamSTT']").val(data.TeamSTT == null ? "0" : data.TeamSTT);
                    $("#formDetail  :input[name='Phone']").val(data.Mobile);
                    $("#formDetail  :input[name='DOB']").val(moment(new Date(parseInt(data.DOB.slice(6, -2)))).format("DD-MM-YYYY"));
                    valueold = data.Name;
                },
          function (query) {
              return {
                  //storeId: $('#txtKhoXuat_Add').val(),
                  search: query,
                  customerold: $("#formDetail  :input[name='CustomerIdOld']").val()
              }
          },
          function (data) {
              //if (data.Status === false) {
              //    return false;
              //}
              return data;
          }
      );

        });


    });
    /*
    $("#modalDetails1 button#btnUpdateDetail").css("display", "inline");
    $("#modalDetails1 button#btnUpdateDetail").css("disabled", "false");
    $("#modalDetails1 button#btnUndoRoom").css("disabled", "true");
    $("#modalDetails1 button#btnChangeRoom").css("disabled", "true");

    $("#modalDetails1 button#btnUndoRoom").css("display", "none");
    $("#modalDetails1 button#btnChangeRoom").css("display", "none");
    */

    $("#btnUpdateDetail").off("click");

    $("#btnUpdateDetail").click(function () { addRoomMate(); });

    $("#modalDetails1").modal("show");
}
function EditCustomerDialog(checkinID)
{
    $("#modalDetails1").off('show.bs.modal');
    $("#modalDetails1").on('show.bs.modal', function () {
        $("#TittleBox").html("Thông tin khách hàng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails1 .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerArriveManage/AddUsingCustomers", { checkinID: checkinID }, function (rs) {
            $("#modalDetails1 .modal-body-content").html(rs);
            SetDate(".birthday");
            g_Utils.SetDateDefaultAdd();
            g_Utils.ConfigAutocomplete('#Name', "/CustomerArriveManage/SelectCustomer", "Name", "Name",
                function (item)
                {
                    var data = (JSON.parse(item.object));
                    //"" CitizenshipCode : 238 Company : "" CountryId : null CreateBy : 1 CreateDate : "/Date(1481792366727)/" CustomerTypeID : 0 DOB : "/Date(1480525200000)/" Email : "" Fax : "" HotelCode : "OzeHotel0001" Id : 73 IdentifyNumber : "125309881" Mobile : "01658756994" ModifyBy : null ModifyDate : null Name : "Ngọc Lam Man" Phone : "" ReservationID : null Sex : 1 SourceID : 0 Status : true SysHotelID : null TaxCode : "" TeamMergeSTT : null TeamSTT : null
                    console.log(data);
                    $("#modalDetails1 #formDetail :input").attr("disabled", true);
                    $('#Name').prop('disabled', false);
                    $("#formDetail  :input[name='Name']").val(data.Name);
                    $("#formDetail  :input[name='CustomerId']").val(data.Id);
                    $("#formDetail  :input[name='Company']").val(data.Company);
                    $("#formDetail  :input[name='CountryId']").val(data.CountryId == null ? "" : data.CountryId);
                    $("#formDetail  :input[name='Sex']").val(data.Sex);
                    $("#formDetail  :input[name='Email']").val(data.Email);
                    $("#formDetail  :input[name='IdentifyNumber']").val(data.IdentifyNumber);
                    $("#formDetail  :input[name='TeamMergeSTT']").val(data.TeamMergeSTT == null ? "0" : data.TeamMergeSTT);
                    $("#formDetail  :input[name='TeamSTT']").val(data.TeamSTT == null ? "0" : data.TeamSTT);
                    $("#formDetail  :input[name='Phone']").val(data.Mobile);
                    $("#formDetail  :input[name='DOB']").val(moment(new Date(parseInt(data.DOB.slice(6, -2)))).format("DD-MM-YYYY"));
                    valueold = data.Name;
                },
          function (query) {
              return {
                  //storeId: $('#txtKhoXuat_Add').val(),
                  search: query,
                  customerold: $("#formDetail  :input[name='CustomerIdOld']").val()
              }
          },
          function (data) {
              //if (data.Status === false) {
              //    return false;
              //}
              return data;
          }
      );

        });


    });
    $("#modalDetails1 button#btnUpdateDetail").css("display", "inline");
    $("#modalDetails1 button#btnUpdateDetail").css("disabled", "false");
    $("#modalDetails1 button#btnUndoRoom").css("disabled", "true");
    $("#modalDetails1 button#btnChangeRoom").css("disabled", "true");

    $("#modalDetails1 button#btnUndoRoom").css("display", "none");
    $("#modalDetails1 button#btnChangeRoom").css("display", "none");


    $("#modalDetails1").modal("show");
}
function addRoomMate() {
    // thêm khách ở cùng
    if (!$("#formDetail").valid()) return;
    var pdata = getFormData($("#formDetail"));
    var id = $("#formDetail  :input[name='CustomerId']").val();

    if (id > 0) {

        var model = {
            CustomerId: id,
            CheckInID: $("#formDetail  :input[name='CheckInID']").val(),
            Roomid: $("#formDetail  :input[name='Roomid']").val(),
            SysHotelID: $("#formDetail  :input[name='SysHotelID']").val(),
            CustomerIdOld: $("#formDetail  :input[name='CustomerIdOld']").val()
        }
        pdata = model;
    }


    showDialogLoading();
    $.post("/CustomerArriveManage/AddUsingRoom", { obj: pdata }, function (rs) {
        hideDialogLoading();
        //closeDlgLoadingData();
        if (rs.Status === "01") {
            bootbox.alert(rs.Message, function () {
                //searchGrid();
                $("#modalDetails1").modal("hide");
            });
        }
        else {
            alert(rs.Message);
        }
    });
}
//view cập nhật thông tin khách hàng
function viewEditDialog(id)
{
    $("#modalDetails1").off('show.bs.modal');
    $("#modalDetails1").on('show.bs.modal', function () {

        $("#TittleBox").html("Thông tin khách hàng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails1 .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerManage/GetEdit", { id: id }, function (rs)
        {
            $("#modalDetails1 .modal-body-content").html(rs);
            g_Utils.SetDate(".birthday");
        });

        $("#modalDetails1 button#btnUpdateDetail").css("display", "inline");
        $("#btnUpdateDetail").off("click");

        $("#btnUpdateDetail").click(function () {

            if (!$("#formDetail").valid()) return;
            var pdata = getFormData($("#formDetail"));
            showDialogLoading();
            $.post("/CustomerManage/update", { obj: pdata }, function (data) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (data.result > 0) {
                    $("#NameCheckout").val(pdata.Name);
                    $("#PhoneCheckout").val(pdata.Mobile);
                    $("#IdentifyNumberCheckout").val(pdata.IdentifyNumber);
                    bootbox.alert("Cập nhật thông tin khách hàng thành công.", function ()
                    {
                        //searchGrid();
                        $("#modalDetails1").modal("hide");
                    });
                }
                else {
                    alert("Cập nhật thống tin khách hàng thất bại!");
                }
            });
        });
    });
    $("#modalDetails1").modal("show");
}