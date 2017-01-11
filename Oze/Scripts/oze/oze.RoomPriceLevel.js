
var $modalMail = $("#modalDetails");
//phụ trội quá giờ theo trả ngày
var counlate_dayteam = 0;
var counlate_day = 0;
//giá theo  giờ
var counlate_pricemainteam = 0;
var counlate_pricemain = 0;
//phụ trội quá giờ trả theo đêm
var counlate_nihgtteam = 0;
var counlate_nihgt = 0;
//Phụ trội nhận phòng sớm theo ngày
var counearly_dayteam = 0;
var counearly_day = 0;
//Phụ trội nhận phòng sớm theo đêm
var counearly_nihgtteam = 0;
var counearly_nihgt = 0;
//quá số người
var counelimit_Personteam = 0;
var counelimit_Person = 0;
//phụ trội trẻ em
var counelimit_Personteam_child = 0;
var counelimit_Person_child = 0;
$(document).ready(function () {

    $("#btnAdd").click(function () {
        $("#modalDetails").off('show.bs.modal');
        $("#modalDetails")
            .on('show.bs.modal',
                function () {
                    //RuleValidateSubmitToAdd();
                    $("#modalDetails .modal-body-content").html('<p>loading..</p>');
                    $.post("/RoomPriceLevel/ViewInsert",
                        function (rs) {
                            $("#modalDetails .modal-body-content").html(rs);
                            $("body").addClass("modal-open1");
                            SetDate(".birthday");
                            g_Utils.SetAmount();
                            $("body").addClass("modal-open1");
                            g_Utils.SetDateDefaultAdd();
                            //RuleValidateToPrintBacode();
                            $("#TittleBox").html("Thêm mới giá theo phòng");
                            counlate_dayteam = 0;
                            counlate_day = 0;
                            //giá theo  giờ
                            counlate_pricemainteam = 0;
                            counlate_pricemain = 0;
                            //phụ trội quá giờ trả theo đêm
                            counlate_nihgtteam = 0;
                            counlate_nihgt = 0;
                            //Phụ trội nhận phòng sớm theo ngày
                            counearly_dayteam = 0;
                            counearly_day = 0;
                            //Phụ trội nhận phòng sớm theo đêm
                            counearly_nihgtteam = 0;
                            counearly_nihgt = 0;
                            //quá số người
                            counelimit_Personteam = 0;
                            counelimit_Person = 0;
                            //phụ trội trẻ em
                            counelimit_Personteam_child = 0;
                            counelimit_Person_child = 0;
                        });

                    $("#modalDetails button#btnUpdateDetail").css("display", "inline");
                    //$("#btnUpdateDetail").off("click");


                });
        //$("#modalDetails").css('width', '750px');
        //$("#modalDetails").css('margin', '100px auto 100px auto');
        $("#modalDetails").modal("show");
    });
    $("#btnAddHotel").click(function () {
        $(".RowEndSelect").html('');
        $(".Rome_steam").find("input").each(function () {
            var id = "#" + $(this)[0].id;

            if ($(id).is(":checked")) {

                $(".RowEndSelect").append('<div class="col-lg-1 col-xs-1"> ' +
                    '<!-- small box --> <div class="small-box bg-gray">' +
                    ' <div class="inner" style="text-align: center;"> ' +
                    '<h5> ' + $(this)[0].name + ' </h5> <input type="hidden" id="' + $(this)[0].id + '" name="' + $(this)[0].name + '"/> </div> </div> </div>');
            } else {

            }

        })
        $("#modaHotel").modal("hide");
    });
    // them hangj phong
    $modalMail.on('click', 'button#btnUAddDetail', function (e) {
        var $form = $("#formDetail_f1");
        if ($form.valid()) {
            showDialogLoading();
            var element = {},
                listXtraDay = [],
                listPriceDay = [],
                listXtraNight = [],
                listEarlyDay = [],
                listEarlyNight = [],
                listLimitPerson = [];
            listLimitPerson_Child = [];
            //Phuj trội theo ngày
            $(".phutroigiotheongay")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = $(this)[0].id;
                        element.typeExtra = 1;
                        listXtraDay.push(element);
                        element = {};
                    }

                });
            // phụ trội theo đêm
            $(".phutroigiotheodem")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = 0;
                        element.typeExtra = 2;
                        listXtraNight.push(element);
                        element = {};
                    }

                });
            // phụ trộilistPriceDay
            $(".pricebytime")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberHours = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.price = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                        element.Id = $(this)[0].accept;
                        //element.typeExtra = 2;
                        listPriceDay.push(element);
                        element = {};
                    }

                });
            //consolelog()
            //return;
            // trả phòng sớm theo ngày
            $(".EarlyDay")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = 0;
                        element.typeExtra = 3;
                        listEarlyDay.push(element);
                        element = {};
                    }

                });
            // trả phòng sớm theo đêm
            $(".EarlyNight")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = 0;
                        element.typeExtra = 4;
                        listEarlyNight.push(element);
                        element = {};
                    }

                });
            // trả phòng sớm theo đêm
            $(".LimitPerson")
                .find("input")
                .each(function () {
                    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = 0;
                        element.typeExtra = 5;
                        listLimitPerson.push(element);
                        element = {};
                    }

                });
            // Phu trội trẻ em
            $(".LimitPerson_Child")
                .find("input")
                .each(function () {
                    var check = $(this)[0].className.includes("amount-number-mask");
                    if (check) {
                        element.numberExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();
                    } else {
                        element.priceExtra = $("#formDetail_f1 :input[name=" + $(this)[0].name + "]").val();;
                        element.Id = 0;
                        element.typeExtra = 6;
                        listLimitPerson_Child.push(element);
                        element = {};
                    }

                });
            var elementroom = {}, listRoom = [];
            $(".RowEndSelect")
                .find("input")
                .each(function () {
                    //console.log($(this)[0]);
                    elementroom = {};
                    elementroom.Id = $(this)[0].id;
                    elementroom.Name = $(this)[0].name;
                    listRoom.push(elementroom);


                });
            var id = $("#formDetail_f1  :input[name='Id']").val();
            var hotel = $('select[name="ListHotel"]').val();
            var typeRoom = $('select[name="ListRoomType"]').val();
            var twin = $("#formDetail_f1  :input[name='twin']").val();
            //var roomTypeName = $("#formDetail_f1  :input[name='txtRoomType']").val();
            var single = $("#formDetail_f1  :input[name='single']").val();
            var pricenight = $("#formDetail_f1  :input[name='txtGiaquadem']").val();
            var priceday = $("#formDetail_f1  :input[name='txtGiatheongay']").val();
            var pricemonth = $("#formDetail_f1  :input[name='txtGiatheothang']").val();
            var standperson = $("#formDetail_f1  :input[name='txtSonguoilon']").val();
            var note = $("#formDetail_f1  :input[name='txtLuuy']").val();
            var number = $("#formDetail_f1  :input[name='txtNumber_Adult']").val();
            var title = $("#formDetail_f1  :input[name='txtTitle']").val();

            var t2 = $("#formDetail_f1  :input[name='T2']")[0].checked;
            var t3 = $("#formDetail_f1  :input[name='T3']")[0].checked;
            var t4 = $("#formDetail_f1  :input[name='T4']")[0].checked;
            var t5 = $("#formDetail_f1  :input[name='T5']")[0].checked;
            var t6 = $("#formDetail_f1  :input[name='T6']")[0].checked;
            var t7 = $("#formDetail_f1  :input[name='T7']")[0].checked;
            var cn = $("#formDetail_f1  :input[name='CN']")[0].checked;
            var fDate = $("#formDetail_f1  :input[name='Formdate']").val();
            var tDate = $("#formDetail_f1  :input[name='Todate']").val();
            var dayOfWeeks = "";
            //dkm concat ko chay phai dung +=
            if (t2) {
                dayOfWeeks = "2";
            }

            if (t3) { if (dayOfWeeks === "") dayOfWeeks += "3"; else { dayOfWeeks += "-3"; } }
            if (t4) { if (dayOfWeeks === "") dayOfWeeks += "4"; else { dayOfWeeks += "-4"; } }
            if (t5) { if (dayOfWeeks === "") dayOfWeeks += "5"; else { dayOfWeeks += "-5"; } }
            if (t6) { if (dayOfWeeks === "") dayOfWeeks += "6"; else { dayOfWeeks += "-6"; } }
            if (t7) { if (dayOfWeeks === "") dayOfWeeks += "7"; else { dayOfWeeks += "-7"; } }
            if (cn) { if (dayOfWeeks === "") dayOfWeeks += "8"; else { dayOfWeeks += "-8"; } }
            var model = {
                Id: id,
                SysHotelID: hotel,
                RoomTypeID: typeRoom,
                Twin: twin,
                title: title,
                _Single: single,
                Note: note,
                Number_Adult: number,
                PriceNight: pricenight,
                PriceDay: priceday,
                PriceMonth: pricemonth,
                CreateID: standperson,
                dayOfWeeks: dayOfWeeks

            }
            ////return;
          
            //setTimeout(function () {
                var url = "/RoomPriceLevel/InsertOrUpdate";
                $.post(url,
                    {
                        listRoom: listRoom,
                        fDate: fDate,
                        tDate: tDate,
                        listXtraDay: listXtraDay,
                        listPriceDay: listPriceDay,
                        listXtraNight: listXtraNight,
                        listEarlyDay: listEarlyDay,
                        listEarlyNight: listEarlyNight,
                        listLimitPerson: listLimitPerson,
                        listLimitPerson_Child: listLimitPerson_Child,
                        model: model
                    },
                    function (data) {
                        hideDialogLoading();
                        //closeDlgLoadingData();
                        if (data.result) {
                            if (parseInt(id) > 0) {
                                bootbox.alert(data.Message,
                                    function () {
                                        searchGrid();
                                        $("#modalDetails").modal("hide");
                                    });
                            } else {
                                bootbox.alert(data.Message,
                                    function () {
                                        searchGrid();
                                        $("#modalDetails").modal("hide");
                                    });
                            }

                        } else {
                            alert(data.Message);

                        }
                    });
            //},
            //    5000);
        }
    });

    //phụ trội quá giờ theo trả ngày
    $modalMail.on('click', 'i#addptqgtn', function (e) {
        //$("#addptqgtn").click(function () {
        counlate_day++;
        if ($(".phutroigiotheongay").find("input").length >= 14)
            return;
        if (counlate_day === 0)
            counlate_day = (parseInt($(".phutroigiotheongay").find("input").length) / 2);
        $(".phutroigiotheongay").append('<div class="itemptgtn' + counlate_day + '">' +
            ' <div class="form-group "> <div class="giatheogio col-sm-5" style="display: flex;">' +
            ' <div class="col-sm-12 mp"> <p class="control-label col-sm-4 mp">Quá:</p> ' +
            '<div class="input-group col-sm-8 mp">' +
            ' <input class="form-control amount-number-mask" required="1" name="txtGioquatheongay' + counlate_day + '" id="txtGioquatheongay' + counlate_day + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">H</span> </div>' +
            ' </div> </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> ' +
            '<div class="col-sm-12 mp"> <div class="col-sm-10 mp" >  <div class="input-group" >' +
            '<input class="form-control amount-double-mask" required="1"' +
            ' name="txtGiaquatheongay' + counlate_day + '" id="txtGiaquatheongay' + counlate_day + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">VNĐ</span> </div></div> ' +
            ' <div class="input-group col-sm-2">' +
            ' <i class="fa fa-remove"onclick="removeptqgtn(' + counlate_day + ')"></i> ' +
            '</div> </div> </div> </div> </div>');

        g_Utils.SetAmount();
    });
    //giá theo  giờ
    $modalMail.on('click', 'i#addpricemain', function (e) {
        //$("#addptqgtn").click(function () {

        if ($(".pricebytime").find("input").length >= 14)
            return;
        if (counlate_pricemain === 0)
            counlate_pricemain = (parseInt($(".pricebytime").find("input").length)/2);

        counlate_pricemain++;
        //if (counlate_pricemainteam >= 5)
        //    return;
        //counlate_pricemainteam++;
        $(".pricebytime").append('<div class="itemptgtnpricebytime' + counlate_pricemain + '"> <div class="form-group"> ' +
            '<div class="giatheogio col-sm-5" style="display: flex;"> <div class="col-sm-12 mp">' +
            ' <p class="control-label col-sm-4 mp">Giá:</p> <div class="input-group col-sm-8 mp">' +
            ' <input class="form-control amount-number-mask" required accept="0" id="txtGio' + counlate_pricemain + '" name="txtGio' + counlate_pricemain + '" placeholder="0" min="0"> ' +
            '<span class="input-group-addon">H</span> </div> </div>' +
            ' </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> ' +
            '<div class="col-sm-12 mp"> <div class="col-sm-10 mp"> <div class="input-group">' +
            ' <input class="form-control amount-double-mask" accept="0" required id="txtGiatheogio' + counlate_pricemain + '"  name="txtGiatheogio' + counlate_pricemain + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">VNĐ</span> </div> </div> <div class="col-sm-2 mp">' +
            ' <i class="fa fa-remove" onclick="removeMainPrice(' + counlate_pricemain + ')"></i> </div> </div> </div> </div> </div>');

        //$(".pricebytime").append('<div class="itemptgtnpricebytime' + counlate_pricemain + '"> <div class="form-group"> <div class="giatheogio col-sm-5" style="display: flex;">' +
        //  ' <p class="control-label">Giá:</p> <div class="input-group col-sm-3" style="width: 70%; margin-left: 14px;"> <input class="form-control amount-number-mask" ' +
        //  ' name="txtGio' + counlate_pricemain + '" id="0"  required="1"  placeholder="0" min="0"> ' +
        //  '<span class="input-group-addon">H</span>' +
        //  ' </div> </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> ' +
        //  '<div class="input-group col-sm-10" style="padding-right: 10px;"> <input class="form-control amount-double-mask" ' +
        //  ' name="txtGiatheogio' + counlate_pricemain + '" placeholder="0" min="0" required="1" > <span class="input-group-addon">VNĐ</span> </div> <div class="input-group col-sm-2">' +
        //  ' <i class="fa fa-remove" onclick="removeMainPrice(' + counlate_pricemain + ')"></i>' +
        //  '</div> </div> </div> </div>');
        //countitemptgtn++;
        g_Utils.SetAmount();
    });
    //pPhụ trội quá giờ trả theo đêm
    $modalMail.on('click', 'i#addlatenight', function (e) {
        //$("#addptqgtn").click(function () {
        counlate_nihgt++;
        if ($(".phutroigiotheodem").find("input").length >= 14)
            return;
        if (counlate_nihgt === 0)
            counlate_nihgt =(parseInt($(".phutroigiotheodem").find("input").length)/2);
        //if (counlate_nihgtteam >= 5)
        //    return;
        //counlate_nihgtteam++;
        $(".phutroigiotheodem").append('<div class="itemptgtnnight' + counlate_nihgt + '"> <div class="form-group">' +
            ' <div class="giatheogio col-sm-5" style="display: flex;"> ' +
            '<div class="col-sm-12 mp">' +
            ' <p class="control-label col-sm-4 mp">Quá:</p> <div class="input-group col-sm-8 mp"> ' +
            '<input class="form-control amount-number-mask" required="1" id="txtGioquatheodem' + counlate_nihgt + '" name="txtGioquatheodem' + counlate_nihgt + '" placeholder="0" min="0"> ' +
            '<span class="input-group-addon">H</span> </div> </div> </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> <div class="col-sm-12 mp">' +
            ' <div class="col-sm-10 mp" >  <div class="input-group" >' +
            '<input class="form-control amount-double-mask" required="1" id="txtGiaquatheodem' + counlate_nihgt + '" name="txtGiaquatheodem' + counlate_nihgt + '" placeholder="0" min="0"> <span class="input-group-addon">VNĐ</span>' +
            ' </div></div> <div class="input-group col-sm-2">' +
            ' <i class="fa fa-remove" onclick="removeearly_night(' + counlate_nihgt + ')"></i> </div> </div> </div> </div> </div>');



        g_Utils.SetAmount();
    });
    //Phụ trội nhận phòng sớm theo ngày
    $modalMail.on('click', 'i#addEarlyDay', function (e) {
        //$("#addptqgtn").click(function () {
        counearly_day++;
        if ($(".EarlyDay").find("input").length >= 14)
            return;
        if (counearly_day === 0)
            counearly_day = (parseInt($(".EarlyDay").find("input").length) / 2);
        //if (counearly_dayteam >= 5)
        //    return;
        //counearly_dayteam++;
        $(".EarlyDay").append('<div class="EarlyDayItem' + counearly_day + '"> <div class="form-group">' +
            ' <div class="giatheogio col-sm-5" style="display: flex;"> <div class="col-sm-12 mp">' +
            ' <p class="control-label col-sm-4 mp">Sớm:</p>' +
            ' <div class="input-group col-sm-8 mp">' +
            ' <input class="form-control amount-number-mask" required="1" id="txtGiosomtheongay' + counearly_day + '" name="txtGiosomtheongay' + counearly_day + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">H</span> </div> </div> </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> ' +
            '<div class="col-sm-12 mp"> <div class="col-sm-10 mp"> <div class="input-group "> ' +
            '<input class="form-control amount-double-mask" required="1" id="txtGiasomtheongay' + counearly_day + '" name="txtGiasomtheongay' + counearly_day + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">VNĐ</span> </div> </div> <div class="input-group col-sm-2"> ' +
            ' <i class="fa fa-remove" onclick="removeearly_day(' + counearly_day + ')"></i> </div> </div> </div> </div> </div>');


        ;
        g_Utils.SetAmount();
    });
    //Phụ trội nhận phòng sớm theođêm
    $modalMail.on('click', 'i#addEarlyNight', function (e) {
        //$("#addptqgtn").click(function () {
        counearly_nihgt++;
        if ($(".EarlyNight").find("input").length >=  14)
            return;
        if (counearly_nihgt === 0)
            counearly_nihgt = (parseInt($(".EarlyNight").find("input").length) / 2);
        //if (counearly_nihgtteam >= 5)
        //    return;
        //counearly_nihgtteam++;
        $(".EarlyNight").append('<div class="EarlyNightItem' + counearly_nihgt + '"> <div class="form-group"> ' +
            '<div class="giatheogio col-sm-5" style="display: flex;"> <div class="col-sm-12 mp">' +
            ' <p class="control-label col-sm-4 mp">Sớm:</p> <div class="input-group col-sm-8 mp">' +
            ' <input class="form-control amount-number-mask" required="1" id="txtGiosomquadem' + counearly_nihgt + '" name="txtGiosomquadem' + counearly_nihgt + '" placeholder="0" min="0"> ' +
            '<span class="input-group-addon">H</span> ' +
            '</div> </div> </div> <div class="col-sm-7" style="display: flex; padding-left: 0px; padding-right: 0px;"> ' +
            '<div class="col-sm-12 mp"> <div class=" col-sm-10 mp">' +
            ' <div class="input-group">' +
            ' <input class="form-control amount-double-mask" required="1" id="txtGiasomquadem' + counearly_nihgt + '" name="txtGiasomquadem' + counearly_nihgt + '" placeholder="0" min="0"> ' +
            '<span class="input-group-addon">VNĐ</span> </div> </div> <div class="input-group col-sm-2">' +
            ' <i class="fa fa-remove" onclick="removeearly_nihgt(' + counearly_nihgt + ')"></i> </div> </div>' +
            ' </div> </div> </div>');



        g_Utils.SetAmount();
    });
    //Quá số người
    $modalMail.on('click', 'i#AddLimitPerson', function (e) {
        //$("#addptqgtn").click(function () {
        counelimit_Person++;

        //if (counelimit_Personteam >= 5)
        //    return;
        if ($(".LimitPerson").find("input").length >=  14)
            return;
        if (counelimit_Person === 0)
            counelimit_Person = (parseInt($(".LimitPerson").find("input").length) / 2);
        //counelimit_Personteam++;

        $(".LimitPerson").append('<div class="LimitPersonItem' + counelimit_Person + '">' +
            ' <div class="form-group"> <div class=" col-sm-5" style="display: flex;"> ' +
            '<div class="col-sm-12 mp"> <p class="control-label col-sm-4 mp">Quá:</p> ' +
            '<div class="input-group col-sm-8 mp"> <input class="form-control amount-number-mask"' +
            ' required="1" id="txtsonguoi' + counelimit_Person + '" name="txtsonguoi' + counelimit_Person + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">N</span> </div> </div> </div> <div class="col-sm-7"' +
            ' style="display: flex; padding-left: 0px; padding-right: 0px;"> <div class="col-sm-12 mp">' +
            ' <div class="col-sm-10 mp"> <div class="input-group"> <input class="form-control amount-double-mask"' +
            ' required="1" id="txtGiasonguoi' + counelimit_Person + '" name="txtGiasonguoi' + counelimit_Person + '" placeholder="0" min="0"> <span class="input-group-addon">VNĐ</span>' +
            ' </div> </div> <div class="input-group col-sm-2">  <i class="fa fa-remove" onclick="removeLimitPerson(' + counelimit_Person + ')"></i> </div> </div> </div> </div> </div>');
        //countitemptgtn++;
        //      ' <i class="fa fa-remove" onclick="removeearly_day(' + counearly_day + ')"></i>' +
        //'</div> </div> </div> </div>');
        g_Utils.SetAmount();
    });
    //phị trội trẻ em
    $modalMail.on('click', 'i#AddLimitPerson_Child', function (e) {
        //$("#addptqgtn").click(function () {
        counelimit_Person_child++;
        if ($(".LimitPerson_Child").find("input").length >=  14)
            return;
        if (counelimit_Person_child === 0)
            counelimit_Person_child = (parseInt($(".LimitPerson_Child").find("input").length) / 2);
        //if (counelimit_Personteam_child >= 5)
        //    return;
        //counelimit_Personteam_child++;

        $(".LimitPerson_Child").append('<div class="LimitPersonItem_Child' + counelimit_Person_child + '">' +
            ' <div class="form-group"> <div class=" col-sm-5" style="display: flex;"> ' +
            '<div class="col-sm-12 mp"> <p class="control-label col-sm-4 mp">Quá:</p> ' +
            '<div class="input-group col-sm-8 mp"> <input class="form-control amount-number-mask"' +
            ' required="1" id="txt_NumberChild' + counelimit_Person_child + '" name="txt_NumberChild' + counelimit_Person_child + '" placeholder="0" min="0">' +
            ' <span class="input-group-addon">N</span> </div> </div> </div> <div class="col-sm-7"' +
            ' style="display: flex; padding-left: 0px; padding-right: 0px;"> <div class="col-sm-12 mp">' +
            ' <div class="col-sm-10 mp"> <div class="input-group"> <input class="form-control amount-double-mask"' +
            ' required="1" id="txt_Child' + counelimit_Person_child + '" name="txt_Child' + counelimit_Person_child + '" placeholder="0" min="0"> <span class="input-group-addon">VNĐ</span>' +
            ' </div> </div> <div class="input-group col-sm-2">  <i class="fa fa-remove" onclick="removeLimitPerson_Child(' + counelimit_Person_child + ')"></i> </div> </div> </div> </div> </div>');
        //countitemptgtn++;
        //      ' <i class="fa fa-remove" onclick="removeearly_day(' + counearly_day + ')"></i>' +
        //'</div> </div> </div> </div>');
        g_Utils.SetAmount();
    });
    $modalMail.on('click', 'i#btnShowDSPhong', function (e) {
        var typeRoom = $('select[name="ListRoomType"]').val();
        if (typeRoom === "") {
            alert("Chưa xác định hạng phòng, quý khách vui lòng chọn hạng phòng trước !");
            return;
        }
        $("#modaHotel").off('show.bs.modal');
        $("#modaHotel")
            .on('show.bs.modal',
                function () {
                    var id = $("#formDetail_f1  :input[name='Id']").val();
                    var hotel = $('select[name="ListHotel"]').val();

                    //RuleValidateSubmitToAdd();
                    $("#modaHotel .modal-body-content").html('<p>loading..</p>');
                    $.post("/RoomPriceLevel/GetRoom", { id: id, hotelid: hotel, typeRoom: typeRoom },
                        function (rs) {
                            $("#modaHotel .modal-body-content").html(rs);
                            $("body").addClass("modal-open1");
                        });
                });
        $("#modaHotel").modal("show");
    });
    $modalMail.on('change', '#ListRoomType', function (e) {
        var typeRoom = $('select[name="ListRoomType"]').val();
        var hotel = $('select[name="ListHotel"]').val();


        showDialogLoading();
        $("#modaHotel .modal-body-content").html('<p>loading..</p>');
        $.post("/RoomPriceLevel/GetRoomActives", { id: typeRoom, hotelid: hotel },
            function (rs) {
                hideDialogLoading();
                if (rs.status) {
                    for (var i = 0; i < rs.Data.length; i++) {
                        $(".RowEndSelect").append('<div class="col-lg-1 col-xs-1"> ' +
                    '<!-- small box --> <div class="small-box bg-gray">' +
                    ' <div class="inner" style="text-align: center;"> ' +
                    '<h5> ' + rs.Data[i].Name + ' </h5> <input type="hidden" id="' + rs.Data[i].ID + '" name="' + rs.Data[i].Name + '"/> </div> </div> </div>');

                    }

                }

            });
    });
});


//phụ trội quá giờ theo trả ngày
function removeptqgtn(id) {
    //counlate_dayteam = counlate_dayteam - 1;
    $('.itemptgtn' + id).remove();
}
//giá theo  giờ
function removeMainPrice(id) {
    //counlate_pricemainteam = counlate_pricemainteam - 1;
    $('.itemptgtnpricebytime' + id).remove();
}
//Phụ trội quá giờ trả theo đêm

function removeearly_night(id) {

    //counlate_nihgtteam = counlate_nihgtteam - 1;
    $('.itemptgtnnight' + id).remove();
}
//Phụ trội nhận phòng sớm theo ngày
function removeearly_day(id) {
    //counearly_dayteam = counearly_dayteam - 1;
    $('.EarlyDayItem' + id).remove();
}
//Phụ trội nhận phòng sớm theo đêm
function removeearly_nihgt(id) {
    //counearly_nihgtteam= counearly_nihgtteam - 1;
    $('.EarlyNightItem' + id).remove();
}//quá số người
function removeLimitPerson(id) {
    //counelimit_Personteam= counelimit_Personteam - 1;
    $('.LimitPersonItem' + id).remove();
}
function removeLimitPerson_Child(id) {
    //counelimit_Personteam_child = counelimit_Personteam_child - 1;
    $('.LimitPersonItem_Child' + id).remove();
}
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
        $("#TittleBox").html("Thông tin giá và hạng phòng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/RoomPriceLevel/GetDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("body").addClass("modal-open1");
            $("#modalDetails button#btnUAddDetail").css("display", "none");
            $("#modalDetails #formDetail_f1 :input").attr("disabled", true);
            //SetDate(".birthday");
            g_Utils.SetAmount();
        });

    });

    $("#modalDetails").modal("show");
}
function viewEditlDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {
        $("#TittleBox").html("Sửa giá theo phòng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/RoomPriceLevel/GetEditDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("body").addClass("modal-open1");
            $("#modalDetails button#btnUAddDetail").css("display", "inline");
            g_Utils.SetAmount();
            counlate_dayteam = 0;
            counlate_day = 0;
            //giá theo  giờ
            counlate_pricemainteam = 0;
            counlate_pricemain = 0;
            //phụ trội quá giờ trả theo đêm
            counlate_nihgtteam = 0;
            counlate_nihgt = 0;
            //Phụ trội nhận phòng sớm theo ngày
            counearly_dayteam = 0;
            counearly_day = 0;
            //Phụ trội nhận phòng sớm theo đêm
            counearly_nihgtteam = 0;
            counearly_nihgt = 0;
            //quá số người
            counelimit_Personteam = 0;
            counelimit_Person = 0;
            //phụ trội trẻ em
            counelimit_Personteam_child = 0;
            counelimit_Person_child = 0;
        });
     
        $("#btnUpdateDetail").off("click");
         
        //$("#btnUpdateDetail").click(function () {

        //    if (!$("#formDetail").valid()) return;
        //    var pdata = getFormData($("#formDetail"));
        //    showDialogLoading();
        //    setTimeout(function () {
        //        $.post("/RoomPriceLevel/update", { obj: pdata }, function (data) {
        //            hideDialogLoading();
        //            //closeDlgLoadingData();
        //            if (data.result > 0) {
        //                bootbox.alert("Cập nhật thống tin khách hàng thành công.", function () {
        //                    searchGrid();
        //                    $("#modalDetails").modal("hide");
        //                });
        //            }
        //            else {
        //                alert("Cập nhật thống tin khách hàng thất bại!");
        //            }
        //        });
        //    }, 5000);
        //});
    });
    //$("#modalDetails").css('width', '750px');
    //$("#modalDetails").css('margin', '100px auto 100px auto');
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
    $(".date-picker").blur(function () {
        if ($(this).val() == "") {

            var date = new Date();
            var _fn = date.getDate();
            var _ft = date.getMonth();
            var _fna = date.getFullYear();
            var ft = (_ft + 1);
            if (ft < 10)
                ft = "0" + ft;
            var fdate = (_fn + "/" + ft + "/" + _fna);
            $(this).val(fdate);


        }
    });


}
function viewInfoDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {
        $("#TittleBox").text = "Thông tin đặt phòng";
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/RoomPriceLevel/GetInfomationRoomDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("body").addClass("modal-open1");
            $("#modalDetails #formInforDetailCustomer :input").attr("disabled", true);
            $("#modalDetails #formInforDetailRoom :input").attr("disabled", true);
            SetDate(".birthday");
        });
        $("#modalDetails button#btnUpdateDetail").css("display", "none");
    });
    $("#modalDetails").modal("show");
}
function DeleteDialog(id) {
    var r = confirm("Bạn có chắc muốn xóa giá này?");
    if (r) {
        showDialogLoading();
            $.post("/RoomPriceLevel/Delete", { id: id }, function (data) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (data.result > 0) {
                    bootbox.alert("Xóa giá thành công.", function () {
                        searchGrid();
                    });
                }
                else {
                    alert("Lỗi trong quá trình xử lý!");
                }
            });
    }
}


function RuleValidateToPrintBacode() {
    var $form = $("#formDetail_f1");
    $form.validate({
        rules:
        {

            txtGio: {
                required: true,
            },
            txtGiatheogio: {
                required: true,
            }
        },
        messages: {

            txtGio: {
                required: "Bạn chưa hoàn thiện thông tin, vui lòng kiểm tra lại",
            },
            txtGiatheogio: {
                required: "Bạn chưa hoàn thiện thông tin, vui lòng kiểm tra lại",
            }

        }
    });
}

//$(".phutroigiotheongay").find("input").each(function () {
//    //console.log($(this)[0].value, $(this)[0].name, $(this)[0].className);
//    var check = $(this)[0].className.includes("amount-number-mask");

//    if (check)
//        console.log(check);

//    console.log(check);
//});