//(new ReservationRoom()).AssignRoom(id)
function ReservationRoom()
{
    var self = this;
    var currentMainPrice = {};
    var currentMainPriceHour = {};
    this.changeLabelGiaPhong = function (val)
    {
        if (val == 0) $("#lbl_GiaPhong").html("Giá phòng/Giờ");
        if (val == 1) $("#lbl_GiaPhong").html("Giá phòng/Ngày");
        if (val == 2) $("#lbl_GiaPhong").html("Giá phòng/Đêm");
        self.showPrice();
    }
    this.clearPrice=function()
    {
        currentMainPrice = {};
        currentMainPriceHour = {};
    }
    this.chooseRoom = function(roomid)
    {
        $('#dllRoom').val(roomid);
        $.get("/Common/GetRoomTypeByRoomID?roomid=" + $('#dllRoom').val(), function (data)
        {
            if (data.result) {
                if (data.result.Id) {
                    $("#dllRoomTypeID").val(data.result.Id);
                    self.loadPrice();
                    
                    //document.activeElement.blur()
                    //$(this).find("#modalDetails .modal-body :input:visible:first").focus();
                    //$("#dllRoomTypeID").focus();
                }
            }
        });
        $("#myModalTemp").modal("hide");
    }
    this.viewDetailDatPhong = function (id)
    {
        window.location.href = "/ReservationRoom/DatPhongDetail?id=" + id;
    }
    this.editRoomMate = function (id)
    {
        $("#myModal").off('show.bs.modal');
        $("#myModal").on('show.bs.modal', function () {
            $("#myModal .modal-body-content").html('<p>loading..</p>');
            $("#myModal .modal-body-content").load("/ReservationRoom/EditRoomMate/" + id);
            $("#myModal button#btnSave").css("display", "inline");
            $("#btnSave").off("click");
            $("#btnSave").click(function () {
                if (!$("#dummyRoomMate").valid()) return;
                var pdata = self.getFormDataMate();
                showDialogLoading();                
                $.post("/ReservationRoom/AddRoomMate", pdata, function (data)
                {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function () { $("#myModal").modal("hide"); searchGrid(); });
                    }
                    else {
                        alert("Có lỗi khi thêm bạn cùng phòng");
                    }
                });
            });
        });
        $("#myModal").modal("show");
    }
    this.AssignRoom = function (id)
    {
        $("#myModal").off('show.bs.modal');
        $("#myModal").on('show.bs.modal', function () {
            $("#myModal .modal-body-content").html('<p>loading..</p>');
            $("#myModal .modal-body-content").load("/ReservationRoom/AssignRoom/" + id);
            $("#myModal button#btnSave").css("display", "inline");
            $("#btnSave").off("click");
            $("#btnSave").click(function () 
            {                
                var pdata = { reservationid: $("#checkinid").val(), roomid: $("#dllRoom").val() };
                showDialogLoading();
                $.post("/ReservationRoom/GanPhong", pdata, function (data) {
                    hideDialogLoading();
                    //closeDlgLoadingData();
                    if (data.result > 0) {
                        bootbox.alert("Thao tác thành công", function ()
                        {
                            $("#myModal").modal("hide");
                            window.location.href = "/ReservationRoom/DatPhongDetail?id=" + $("#checkinid").val();
                        });
                    }
                    else {
                        alert("Có lỗi khi gán phòng:" + data.mess);
                    }
                });
            });
        });
        $("#myModal").modal("show");
    }
    this.getFormDataMate = function ()
    {
        var cbLeader = $('#cbLeader input[name=chkLeaderf]:checked').val();
        var cbPayer = $('#cbPayer input[name=chkPayerf]:checked').val();
        var rbSex = $('#rbSex input[name=r2f]:checked').val();

        // gan cac doi tuong vao object
        var cust = {
            //THÔNG TIN KHÁCH HÀNG    
            'ID': 0,
            'FullName': $('#txtnamef').val(),//Tên đầy đủ
            'Sex': rbSex,//$('#txtname').val(),//giới tính
            'DOB': $('#txtDOBf').val(),//ngày tháng năm sinh dd/mm/yyyy
            'IdentityNumber': $('#txtIdentifyf').val(),// CMND/Hộ chiếu ....
            'CitizenshipCode': $('#dllQuocTichf').val(),// Quốc tịch
            'Address': $('#txtaddressf').val(),//địa chỉ
            'Email': $('#txtEmailf').val(),//email
            'Mobile': $('#txtMobilef').val(),//mobile
            'Company': $('#txtCompanyf').val(),//Company
            'RoomID': $('#dllRoom').val(),//ID phòng
            'GroupID': $('#dllOrgf').val(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
            'GroupJoinID': $('#dllOrgJoinf').val(),//ID phòng
            'Leader': cbLeader,//là Trưởng đoàn
            'Payer': cbPayer,//ID phòng
            'ReserCode': $('#txtResCode').val(),//mã đặt phòng
            'CountryId': $('#dllQuocTich').val()//mã đặt phòng

        }
        return { obj: cust, checkinid: $("#checkinid").val() };

    }
    this.loadPrice = function ()
    {
        $.get("/Common/GetPriceByDateInAndRoomType?roomtypeid=" + $('#dllRoomTypeID').val() + "&datetime=" + $('#txtFromDate').val(), function (data) {
            if (data.result) {
                currentMainPrice = data.result;
                currentMainPriceHour = data.hours;
                self.showPrice();
            }
            else self.clearPrice();
        });
    }
    this.showPrice = function ()
    {
        //hiển thị label
        if (currentMainPrice.title) $("#lblPolicyPrice").html(currentMainPrice.title);

        //tracking id chinh sach gia
        if (currentMainPrice.Id) $("#PriceLevelID").val(currentMainPrice.Id);

        //theo giờ
        if ($("#dllPriceKhungGio").val() == 0)
        {
            if (currentMainPriceHour && currentMainPriceHour.price) $("#txtPrice").val(currentMainPriceHour.price);
        }
        else
        {
            if ($("#dllPriceKhungGio").val() == 1 && currentMainPrice.PriceDay) $("#txtPrice").val(currentMainPrice.PriceDay); //theo đêm
            if ($("#dllPriceKhungGio").val() == 2 && currentMainPrice.PriceDay) $("#txtPrice").val(currentMainPrice.PriceNight); //theo ngày
        }
    }
    this.edit = function (idRes)
    {
        window.location.href = "/ReservationRoom/DatPhong?id=" + idRes;
    }
    this.huyPhong=function(idRes)
    {
            bootbox.prompt(
            {
                title: "Nhập lí do hủy:",
                inputType: 'textarea',
                callback: function (result) {
                    if (!result) return;
                    $.ajax({
                        url: '/ReservationRoom/HuyDatPhong',
                        type: "Post",
                        datatype: "json",
                        data: { id: idRes, "note": result },
                        success: function (response) {
                            if (response.result == 1) {
                                alert("Thao tác thành công");
                                window.location.href = "/ReservationRoom/DatPhongDetail?id=" + idRes;
                            }
                            else {
                                alert("Thao tác không thành công:" + response.mess);
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("Thao tác không thành công:" + thrownError);
                        }
                    });
                }
            });
    }
    this.cancelCheckin = function (idRes) {
        bootbox.prompt(
        {
            title: "Nhập lí do hủy:",
            inputType: 'textarea',
            callback: function (result) {
                if (!result) return;
                $.ajax({
                    url: '/ReservationRoom/CancelCheckIn',
                    type: "Post",
                    datatype: "json",
                    data: { id: idRes, "note": result },
                    success: function (response) {
                        if (response.result == 1) {
                            alert("Thao tác thành công");
                            window.location.href = "/Sodophong/Index";
                        }
                        else {
                            alert("Thao tác không thành công:" + response.mess);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("Thao tác không thành công:" + thrownError);
                    }
                });
            }
        });
    }
    this.checkRoom = function (dtFrom, dtTo, typeRoomId)
    {
        
        var objData = { dtFrom: dtFrom, dtTo: dtTo, typeRoomId: typeRoomId };
        $("#myModalTemp").off('show.bs.modal');
        $("#myModalTemp .modal-title").html("Kiểm tra phòng");

        $("#myModalTemp").on('show.bs.modal', function ()
        {
            $("#myModalTemp .modal-body").html('<p>loading..</p>');
            $.post("/ReservationRoom/checkRoom", objData, function (data)
            {
                hideDialogLoading();
                $("#myModalTemp .modal-body").html(data);
            });
        });
        $("#myModalTemp").modal("show");

    }
    this.setupClick=function()
    {    
        $('#dllRoomTypeID').change(function ()
        {
            if ($('#dllRoomTypeID').val() == 0) return;
            self.loadPrice();
            loadRoomByLevelRoom($('#dllRoomTypeID').val(), "dllRoom");
        });
        $('#dllRoom').change(function ()
        {
            if ($('#dllRoom').val() == 0) return;
            $.get("/Common/GetRoomTypeByRoomID?roomid=" + $('#dllRoom').val(), function (data) {
                if (data.result) {
                    if (data.result.Id)
                    {
                        $("#dllRoomTypeID").val(data.result.Id);
                        self.loadPrice();
                    }
                }
            });
        });
        //fn_Nhận phòng   --> DONE add booking, not done edit booking
        var btnCheckKiemTraPhong = $('#btnCheckKiemTraPhong');
        btnCheckKiemTraPhong.click(function ()
        {
            var objData = { dtFrom: $("#txtFromDate").val(), dtTo: $("#txtToDate").val() };
            self.checkRoom($("#txtFromDate").val(), $("#txtToDate").val(), $("#dllRoomTypeID").val());
            /*
           // alert($("#txtToDate").val());
            $.ajax({
                url: '/ReservationRoom/checkRoom',
                type: "Post",
                datatype: "json",
                data: objData,
                success: function (response)
                {
                    BootstrapDialog.show(
                        {
                        message: response
                    });

                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    // error message
                    alert("Thao tác không thành công:" + thrownError);
                }
            })
            */
        });
        var btnBook = $('#btnBook');
        btnBook.click(function ()
        {
            
               
                // form = self.getParentByTagName(this, 'form');

                cbLeader = $('#cbLeader input[name=chkLeader]:checked').val();
                cbPayer = $('#cbPayer input[name=chkPayer]:checked').val();
                rbSex = $('#rbSex input[name=r2]:checked').val();
                rbResType = $('input[name=r1]:checked', '#rbResType').val();

                // gan cac doi tuong vao object:THÔNG TIN KHÁCH HÀNG
                cust =
                {
                    //THÔNG TIN KHÁCH HÀNG
                    'ID': $("#hdCusID").val(),
                    'FullName': $('#txtname').val(),//Tên đầy đủ
                    'Sex': rbSex,//$('#txtname').val(),//giới tính
                    'DOB': $('#txtDOB').val(),//ngày tháng năm sinh dd/mm/yyyy
                    'IdentityNumber': $('#txtIdentify').val(),// CMND/Hộ chiếu ....
                    'CitizenshipCode': $('#dllQuocTich').val(),// Quốc tịch
                    'Address': $('#txtaddress').val(),//địa chỉ
                    'Email': $('#txtEmail').val(),//email
                    'Mobile': $('#txtMobile').val(),//mobile
                    'Company': $('#txtCompany').val(),//Company
                    'RoomID': $('#dllRoom').val(),//ID phòng
                    'GroupID': $('#dllOrg').val(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
                    'GroupJoinID': $('#dllOrgJoin').val(),//ID phòng
                    'Leader': cbLeader,//là Trưởng đoàn
                    'Payer': cbPayer,    //ID phòng
                    'CountryId': $("#dllQuocTich").val()
                }
                resroom = {
                    //THÔNG TIN ĐẶT PHÒNG
                    'ID': $("#hdReservationID").val(),
                    'CustomerName': $('#txtname').val(),//Ten khach dat phong
                    'ReservationCode': $('#txtResCode').val(),//mã đặt phòng
                    'ReservationType': rbResType,//loại đặt phòng (theo người, theo phòng)
                    'Payment_Type_ID': $('#dllPaymentType').val(),//ID phương thức thanh toán
                    'ArrivalDate': $('#txtFromDate').val(),//thời gian đến (booking)
                    'LeaveDate': $('#txtToDate').val(),//thời gian đi (booking)
                    'Adult': $('#txtAdult').val(),//số người lớn
                    'Children': $('#txtChildren').val(),//số trẻ nhỏ
                    'Holiday': $('#dllPriceDipLe').val(),//giá phòng theo dịp (tính theo ngày thường, ngày lễ)
                    'KhungGio': $('#dllPriceKhungGio').val(),//giá phòng theo khung (tính theo giờ, giá cả ngày, giá đêm)
                    'Price': $('#txtPrice').val().replace(',', ''),//giá phòng (tính theo loại đặt phòng)
                    'Deposit': $('#txtDeposit').val().replace(',', ''),//tiền cọc
                    'Deduction': $('#txtDiscount').val().replace(',', ''),//giảm trừ
                    'Discount': $('#txtDiscount').val().replace(',', ''),//giảm trừ
                    'Note': $('#txtNote').val(),//ghi chú 
                    'RoomTypeID': $('#dllRoomTypeID').val(),//loại phòng
                    'RoomID': $('#dllRoom').val(),//loại phòng
                    'RoomLevelPriceID': $('#RoomLevelPriceID').val(),//chính sách giá
                    customer:cust,
                }
                  
                if ($("#formReservation").valid())
                {
                    bootbox.confirm("Bạn có chắc chắn muốn đặt trước?", function (result) {
                        if (!result) return;
                        $.ajax({
                            url: '/ReservationRoom/DatTruoc',
                            type: "Post",
                            datatype: "json",
                            data: { reservation: resroom },
                            success: function (response) {
                                if (response.result >= 1) {
                                    alert("Thao tác thành công", function () {
                                        //self.viewDetailDatPhong(response.result);
                                        window.location.href = "/Sodophong/Index";
                                    });
                                }
                                else {
                                    alert("Thao tác không thành công:" + response.mess);
                                }

                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                // error message
                                alert("Thao tác không thành công:" + thrownError);
                            }
                        });
                    });
                }
        });
        var btnNhanPhongTheoBooking = $('#btnResSaveByBooking');
        btnNhanPhongTheoBooking.click(function ()
        {
            bootbox.confirm("Bạn có chắc chắn muốn nhận phòng?", function (result)
            {
                if(!result) return;

                $.ajax({
                    url: '/ReservationRoom/CheckInByReservationID',
                    type: "Post",
                    datatype: "json",
                    data: { reservationid: $("#checkinid").val() },
                    success: function (response)
                    {
                        if (response.result == 1)
                        {
                            alert("Thao tác thành công");
                            window.location.href = "/Sodophong/Index";
                        }
                        else {
                            alert("Thao tác không thành công:" + response.mess);
                        }

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        alert("Thao tác không thành công:" + thrownError);
                    }
                });
            });
        });
        var btnAssignRoom = $('#btnAssignRoom');
        btnAssignRoom.click(function () {
            self.AssignRoom($("#checkinid").val());
        });
        var btnNhanPhong = $('#btnResSave');
        btnNhanPhong.click(function ()
        {
            // form = self.getParentByTagName(this, 'form');
            cbLeader = $('#cbLeader input[name=chkLeader]:checked').val();
            cbPayer = $('#cbPayer input[name=chkPayer]:checked').val();
            rbSex = $('#rbSex input[name=r2]:checked').val();
            rbResType = $('input[name=r1]:checked', '#rbResType').val();

            // gan cac doi tuong vao object:THÔNG TIN KHÁCH HÀNG
            cust =
            {
                //THÔNG TIN KHÁCH HÀNG
                'ID': $("#hdCusID").val(),
                'FullName': $('#txtname').val(),//Tên đầy đủ
                'Sex': rbSex,//$('#txtname').val(),//giới tính
                'DOB': $('#txtDOB').val(),//ngày tháng năm sinh dd/mm/yyyy
                'IdentityNumber': $('#txtIdentify').val(),// CMND/Hộ chiếu ....
                'CitizenshipCode': $('#dllQuocTich').val(),// Quốc tịch
                'Address': $('#txtaddress').val(),//địa chỉ
                'Email': $('#txtEmail').val(),//email
                'Mobile': $('#txtMobile').val(),//mobile
                'Company': $('#txtCompany').val(),//Company
                'RoomID': $('#dllRoom').val(),//ID phòng
                'GroupID': $('#dllOrg').val(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
                'GroupJoinID': $('#dllOrgJoin').val(),//ID phòng
                'Leader': cbLeader,//là Trưởng đoàn
                'Payer': cbPayer,    //ID phòng
                'CountryId': $("#dllQuocTich").val()
            }
            resroom =
                {
                //THÔNG TIN ĐẶT PHÒNG
                'ID': $("#hdReservationID").val(),
                'CustomerName': $('#txtname').val(),//Ten khach dat phong
                'ReservationCode': $('#txtResCode').val(),//mã đặt phòng
                'ReservationType': rbResType,//loại đặt phòng (theo người, theo phòng)
                'Payment_Type_ID': $('#dllPaymentType').val(),//ID phương thức thanh toán
                'ArrivalDate': $('#txtFromDate').val(),//thời gian đến (booking)
                'LeaveDate': $('#txtToDate').val(),//thời gian đi (booking)
                'Adult': $('#txtAdult').val(),//số người lớn
                'Children': $('#txtChildren').val(),//số trẻ nhỏ
                'Holiday': $('#dllPriceDipLe').val(),//giá phòng theo dịp (tính theo ngày thường, ngày lễ)
                'KhungGio': $('#dllPriceKhungGio').val(),//giá phòng theo khung (tính theo giờ, giá cả ngày, giá đêm)
                'Price': $('#txtPrice').val().replace(',', ''),//giá phòng (tính theo loại đặt phòng)
                'Deposit': $('#txtDeposit').val().replace(',', ''),//tiền cọc
                'Deduction': $('#txtDiscount').val().replace(',', ''),//giảm trừ
                'Discount': $('#txtDiscount').val().replace(',', ''),//giảm trừ
                'Note': $('#txtNote').val(),//ghi chú 
                'RoomTypeID': $('#dllRoomTypeID').val(),//loại phòng
                'RoomLevelPriceID': $('#RoomLevelPriceID').val(),//chính sách giá
                'RoomID':$('#dllRoom').val(),
                customer: cust,
            }

            if ($("#formReservation").valid())
            {
                bootbox.confirm("Bạn có chắc chắn muốn nhận phòng?", function (result) {
                    if (!result) return;
                    $.ajax({
                        url: '/ReservationRoom/CheckIn',
                        type: "Post",
                        datatype: "json",
                        data: { reservation: resroom },
                        success: function (response) {
                            if (response.result == 1) {
                                alert("Thao tác thành công");
                                window.location.href = "/Sodophong/Index";
                            }
                            else {
                                alert("Thao tác không thành công:" + response.mess);
                            }

                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            // error message
                            alert("Thao tác không thành công:" + thrownError);
                        }
                    });
                });
            }
        });
        var btnCancel = $('#btnCancel');
        btnCancel.click(function ()
        {
            
            self.huyPhong($("#checkinid").val());
        });
        
        // fn_Thêm/Sửa thông tin bạn cùng phòng  ---> DONE add friend, not done edit friend
        $('#btnAdd').click(function () 
        {
            //form = self.getParentByTagName(this, 'form');

            cbLeader = $('#cbLeader input[name=chkLeaderf]:checked').val();
            cbPayer = $('#cbPayer input[name=chkPayerf]:checked').val();
            rbSex = $('#rbSex input[name=r2f]:checked').val();

            // gan cac doi tuong vao object
            cust = {
                //THÔNG TIN KHÁCH HÀNG    
                'ID' : 0,
                'FullName': $('#txtnamef').val(),//Tên đầy đủ
                'Sex': rbSex,//$('#txtname').val(),//giới tính
                'DOB': $('#txtDOBf').val(),//ngày tháng năm sinh dd/mm/yyyy
                'IdentityNumber': $('#txtIdentifyf').val(),// CMND/Hộ chiếu ....
                'CitizenshipCode': $('#dllQuocTichf').val(),// Quốc tịch
                'Address': $('#txtaddressf').val(),//địa chỉ
                'Email': $('#txtEmailf').val(),//email
                'Mobile': $('#txtMobilef').val(),//mobile
                'Company': $('#txtCompanyf').val(),//Company
                'RoomID': $('#dllRoom').val(),//ID phòng
                'GroupID': $('#dllOrgf').val(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
                'GroupJoinID': $('#dllOrgJoinf').val(),//ID phòng
                'Leader': cbLeader,//là Trưởng đoàn
                'Payer': cbPayer,//ID phòng
                'ReserCode': $('#txtResCode').val(),//mã đặt phòng
            }

            // thay doi url theo them/sua
            if (m_check_modify) {
                url = '/ReservationRoom/ReservationEditFriend';
                cust.ID = $(this).data("id");                
            } else {
                url = '/ReservationRoom/ReservationAddFriend';                
            }

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: { customer: cust },
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;

                        if (!m_check_modify) { //add
                            if (mess[1] == 1) { //success                                

                                //lay lai data vừa insert bin nguoc lai vao cac control
                                if (mess[2] != null) {
                                    var obj = mess[2];
                                    //append 1 row thông tin ban cung phòng moi
                                    $("#dllFriend").append('<a href="#myModal" data-toggle="modal" class="edit-friend" data-id="' + obj[0].ID + '"> <i class="fa fa-check-square-o"></i>&nbsp;&nbsp;'
                                                            + obj[0].FullName + '</a><br/>');
                                    self.callModalEdit();

                                    //disable nút add
                                    $("#btnAdd").prop('disabled', true); //disable
                                }
                            }
                        }
                        else{//edit
                            if (mess[1] == 1) { //success   
                                //lay lai data fill vao cac control
                                if (mess[2] != null) {
                                    var obj = mess[2];
                                    $('#txtnamef').val(obj[0].FullName);
                                    $('#rbSex input[name=r2f]:checked').val(obj[0].Sex);
                                    $('#txtDOBf').val(obj[0].DOB);
                                    //self.setDateTimePicker($('#dpDOBf'), 'DD/MM/YYYY')
                                    $('#txtIdentifyf').val(obj[0].IdentityNumber);
                                    $('#dllQuocTichf').val(obj[0].CitizenshipCode);
                                    $('#txtaddressf').val(obj[0].Address);
                                    $('#txtEmailf').val(obj[0].Email);
                                    $('#txtMobilef').val(obj[0].Mobile);
                                    $('#txtCompanyf').val(obj[0].Company);
                                    $('#dllRoom').val(obj[0].RoomID);
                                    $('#dllOrgf').val(obj[0].GroupID);
                                    $('#dllOrgJoinf').val(obj[0].GroupJoinID);
                                    $('#cbLeader input[name=chkLeaderf]:checked').val(obj[0].Leader);
                                    $('#cbPayer input[name=chkPayerf]:checked').val(obj[0].Payer);
                                }
                            }
                        }

                        // success message
                        //self.notify(mess[0], mess[1], false);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        //self.notify(thrownError, -1, false);
                    }
                })
            }
        });
    
        
    }
    this.setupValidate = function ()
    {
        //validate Nhan Phong
        $("form").validate({
            rules: {
                txtname: {
                    required: true
                },
                //txtIdentify: {
                //    required: true
                //},
                //txtDOB: {
                //    required: true,
                //    length: 10
                //},
                txtFromDate: {
                    required: true
                },
                txtToDate: {
                    required: true
                },
                txtAdult: {
                    required: true
                },
                txtChildren: {
                    required: true
                },
                txtDeposit: {
                    required: true,
                },
                txtDiscount: {
                    required: true
                },
                txtPrice: {
                    required: true
                }
            },
            messages: {
                txtname: {
                    required: "Vui lòng nhập Họ và tên"
                },
                //txtIdentify: {
                //    required: "Vui lòng nhập CMND/GPLX"
                //},
                //txtDOB: {
                //    required: "Vui lòng nhập Ngày tháng năm sinh",
                //    length: "Độ dài là 10 ký tự"
                //},
                txtFromDate: {
                    required: "Vui lòng nhập Ngày giờ đến"
                },
                txtToDate: {
                    required: "Vui lòng nhập Ngày giờ đi"
                },
                txtAdult: {
                    required: "Vui lòng nhập Số người lớn"
                },
                txtChildren: {
                    required: "Để là 0 nếu không có"
                },
                txtDeposit: {
                    required: "Để là 0 nếu không có Trả trước"
                },
                txtDiscount: {
                    required: "Để là 0 nếu không có Giảm trừ"
                },
                txtPrice: {
                    required: "Để là 0 nếu không có Giá phòng"
                }
            },
            highlight: function (element) {
                $(element).closest('.form-group').addClass('has-error');
            },
            unhighlight: function (element) {
                $(element).closest('.form-group').removeClass('has-error');
            },
            errorElement: 'span',
            errorClass: 'help-block',
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            }
        });
    }
    
    //fnReady    
    this.fnReady = function () 
    {

        $('#dpDOB').datetimepicker({
            format: 'DD/MM/YYYY'
        });

        $('#dpDOBf').datetimepicker({
            format: 'DD/MM/YYYY'
        });

        $('#dpFromDate').datetimepicker({
            format: 'DD/MM/YYYY HH:mm'
        });

        $('#dpToDate').datetimepicker({
            format: 'DD/MM/YYYY HH:mm'
        });

        var ResCode = $('#txtResCode').val();
        //lấy title
        $('#dllRoom').on('change', function () {
            var lblTitle = 'Mã '+ ResCode +' / Phòng ' + $(this).find("option:selected").text();
            //alert(lblTitle);
            $('#lblTitleH').html(lblTitle);
        });

        //format number    
        var total = 0, price = 0, deposit = 0, discount = 0;
        $("#txtPrice").blur(function () {
            //tinh tong
            price = $("#txtPrice").val().replace(/\,/g, '');
            deposit = $("#txtDeposit").val().replace(/\,/g, '');
            discount = $("#txtDiscount").val().replace(/\,/g, '');
            total = 0;
            /*
            NOTE:
                if( value ) {
                }
                will evaluate to true if value is not:
                null
                undefined
                NaN
                empty string ("")
                0
                false
            */
            //tinh tong
            if ($.isNumeric(price)) {
                total = parseFloat(total) + parseFloat(price);
                $("#txtPrice").val((new OzeBase()).formatNumber(price));
                $("div").remove(".notifyjs-container");
            }
            else {
                self.notify('Vui lòng nhập Giá phòng dạng số.', -1, false);
            }
            if ($.isNumeric(deposit)) {
                total = parseFloat(total) - parseFloat(deposit);
                $("#txtDeposit").val((new OzeBase()).formatNumber(deposit));
                //alert(total);
            }
            if ($.isNumeric(discount)) {
                price = $("#txtPrice").val().replace(/\,/g, '');
                if ($.isNumeric(price)) {
                    total = parseFloat(total) - (parseFloat(price) * (parseFloat(discount) / 100));
                }
                $("#txtDiscount").val((new OzeBase()).formatNumber(discount));
            }
            total = (new OzeBase()).formatNumber(total);
            $('#Total').html(total);
        });
        $("#txtDeposit").blur(function () {
            //tinh tong
            price = $("#txtPrice").val().replace(/\,/g, '');
            deposit = $("#txtDeposit").val().replace(/\,/g, '');
            discount = $("#txtDiscount").val().replace(/\,/g, '');
            total = 0;

            if ($.isNumeric(price)) {
                total = parseFloat(total) + parseFloat(price);
                $("#txtPrice").val(self.formatNumber(price));
                //alert(total);
            }
            if ($.isNumeric(deposit)) {
                total = parseFloat(total) - parseFloat(deposit);
                $("#txtDeposit").val(self.formatNumber(deposit));
                $("div").remove(".notifyjs-container");
            }
            else {
                self.notify('Vui lòng nhập Trả trước dạng số.', -1, false);
            }
            if ($.isNumeric(discount)) {
                price = $("#txtPrice").val().replace(/\,/g, '');
                if ($.isNumeric(price)) {
                    total = parseFloat(total) - (parseFloat(price) * (parseFloat(discount) / 100));
                }
                $("#txtDiscount").val(self.formatNumber(discount));
            }
            total = self.formatNumber(total);
            $('#Total').html(total);
        });
        $("#txtDiscount").blur(function () {
            //tinh tong
            price = $("#txtPrice").val().replace(/\,/g, '');
            deposit = $("#txtDeposit").val().replace(/\,/g, '');
            discount = $("#txtDiscount").val().replace(/\,/g, '');
            total = 0;
            
            if ($.isNumeric(price)) {
                total = parseFloat(total) + parseFloat(price);
                $("#txtPrice").val(self.formatNumber(price));
                //alert(total);
            }
            if ($.isNumeric(deposit)) {
                total = parseFloat(total) - parseFloat(deposit);
                $("#txtDeposit").val(self.formatNumber(deposit));
                //alert(total);
            }
            if ($.isNumeric(discount)) {
                price = $("#txtPrice").val().replace(/\,/g, '');
                if ($.isNumeric(price)) {
                    total = parseFloat(total) - (parseFloat(price) * (parseFloat(discount) / 100));
                }
                $("#txtDiscount").val(self.formatNumber(discount));
                $("div").remove(".notifyjs-container");
            }
            else {
                self.notify('Vui lòng nhập Giảm trừ dạng số.', -1, false);
            }
            total = self.formatNumber(total);
            $('#Total').html(total);
        });
    }

    this.init = function ()
    {
        this.setupClick();
        this.setupValidate();       
        this.fnReady();
    }
}
