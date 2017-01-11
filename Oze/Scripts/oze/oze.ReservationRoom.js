/*NamLD 11/11/2016 04:58:06*/
var g_Reservation;
function ReservationRoom() {
    var self = this;
    // ke thua OzeBase
    OzeBase.apply(this, arguments);
    // check xem co phai la sua khong?
    // update = true, insert = false
    var m_check_modify = false;
    var m_check_booking = false;

    var form, resroom, cust, url;
    var rbResType, rbSex;
    var cbLeader, cbPayer;

    //fn_Nhận phòng   --> DONE add booking, not done edit booking
    this.ReservationSave = function () {
        var btnSave = $('#btnResSave');
        btnSave.click(function () {
            
            form = self.getParentByTagName(this, 'form');

            cbLeader = $('#cbLeader input[name=chkLeader]:checked').val();
            cbPayer = $('#cbPayer input[name=chkPayer]:checked').val();
            rbSex = $('#rbSex input[name=r2]:checked').val();
            rbResType = $('input[name=r1]:checked', '#rbResType').val();

            // gan cac doi tuong vao object
            cust = {
                //THÔNG TIN KHÁCH HÀNG
                'ID': 0,
                'FullName': self.getById('txtname').value.trim(),//Tên đầy đủ
                'Sex': rbSex,//self.getById('txtname').value.trim(),//giới tính
                'DOB': self.getById('txtDOB').value.trim(),//ngày tháng năm sinh dd/mm/yyyy
                'IdentityNumber': self.getById('txtIdentify').value.trim(),// CMND/Hộ chiếu ....
                'CitizenshipCode': self.getById('dllQuocTich').value.trim(),// Quốc tịch
                'Address': self.getById('txtaddress').value.trim(),//địa chỉ
                'Email': self.getById('txtEmail').value.trim(),//email
                'Mobile': self.getById('txtMobile').value.trim(),//mobile
                'Company': self.getById('txtCompany').value.trim(),//Company
                'RoomID': self.getById('dllRoom').value.trim(),//ID phòng
                'GroupID': self.getById('dllOrg').value.trim(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
                'GroupJoinID': self.getById('dllOrgJoin').value.trim(),//ID phòng
                'Leader': cbLeader,//là Trưởng đoàn
                'Payer': cbPayer    //ID phòng
            }
            resroom = {
                //THÔNG TIN ĐẶT PHÒNG
                'ID': 0,
                'CustomerName': self.getById('txtname').value.trim(),//Ten khach dat phong
                'ReservationCode': self.getById('txtResCode').value.trim(),//mã đặt phòng
                'ReservationType': rbResType,//loại đặt phòng (theo người, theo phòng)
                'Payment_Type_ID': self.getById('dllPaymentType').value.trim(),//ID phương thức thanh toán
                'ArrivalDate': self.getById('txtFromDate').value.trim(),//thời gian đến (booking)
                'LeaveDate': self.getById('txtToDate').value.trim(),//thời gian đi (booking)
                'Adult': self.getById('txtAdult').value.trim(),//số người lớn
                'Children': self.getById('txtChildren').value.trim(),//số trẻ nhỏ
                'Holiday': self.getById('dllPriceDipLe').value.trim(),//giá phòng theo dịp (tính theo ngày thường, ngày lễ)
                'KhungGio': self.getById('dllPriceKhungGio').value.trim(),//giá phòng theo khung (tính theo giờ, giá cả ngày, giá đêm)
                'Price': self.getById('txtPrice').value.trim().replace(',',''),//giá phòng (tính theo loại đặt phòng)
                'Deposit': self.getById('txtDeposit').value.trim().replace(',', ''),//tiền cọc
                'Discount': self.getById('txtDiscount').value.trim().replace(',', ''),//giảm trừ
                'Note': self.getById('txtNote').value.trim()//ghi chú 
            }

            // thay doi url theo action    
            if (m_check_booking) {
                url = '/ReservationRoom/ReservationEdit';
                cust.ID = self.getById('hdCusID').value.trim();
                resroom.ID = self.getById('hdResID').value.trim();

            } else {
                url = '/ReservationRoom/ReservationSave';
            }

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: { customer: cust, reservation: resroom },
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;

                        if (mess[1] == 1) {

                            if (!m_check_booking) {
                                //insert success

                                //active link add friend 
                                //$('#addFriend').removeClass('not-active');
                                $('#addFriend').attr('data-toggle', 'modal');
                                //disable nút nhận  phòng
                                //$('#btnResSave').prop('disabled', true);
                                //Change text Nhận phòng -> Lưu
                                $("#btnResSave").html('Lưu');
                                //enable nút Hủy
                                $('#btnCancel').prop('disabled', false);

                                m_check_booking = true; //edit
                            }

                            //Fill data vừa insert
                            if(mess[2] != null && mess[3] != null)
                            {
                                var objcus = mess[2];
                                var objres = mess[3];
                                //THÔNG TIN KHÁCH HÀNG
                                $('#hdCusID').val(objcus[0].ID);
                                $('#txtname').val(objcus[0].FullName); //Tên đầy đủ
                                $('#rbSex').val(objcus[0].Sex);//giới tính
                                $('#txtDOB').val(objcus[0].DOB);//ngày tháng năm sinh dd/mm/yyyy
                                //self.setDateTimePicker($('#dpDOB'), 'DD/MM/YYYY')
                                $('#txtIdentify').val(objcus[0].IdentityNumber);// CMND/Hộ chiếu ....
                                $('#dllQuocTich').val(objcus[0].CitizenshipCode);// Quốc tịch
                                $('#txtaddress').val(objcus[0].Address);//địa chỉ
                                $('#txtEmail').val(objcus[0].Email);//email
                                $('#txtMobile').val(objcus[0].Mobile);//mobile
                                $('#txtCompany').val(objcus[0].Company);//Company
                                $('#dllRoom').val(objcus[0].RoomID);//ID phòng
                                $('#dllOrg').val(objcus[0].GroupID);//Code đoàn (nếu là đặt phòng đi theo đoàn)
                                $('#dllOrgJoin').val(objcus[0].GroupJoinID);//ID phòng
                                $('#chkLeader').val(objcus[0].Leader); //là Trưởng đoàn
                                $('#chkPayer').val(objcus[0].Payer);  //ID phòng

                                //THÔNG TIN ĐẶT PHÒNG
                                $('#hdResID').val(objres[0].ID);
                                $('#txtname').val(objres[0].CustomerName);//Ten khach dat phong
                                $('#txtResCode').val(objres[0].ReservationCode);//mã đặt phòng
                                $('#rbResType').val(objcus[0].ReservationType);//loại đặt phòng (theo người, theo phòng)
                                $('#dllPaymentType').val(objres[0].Payment_Type_ID);//ID phương thức thanh toán
                                $('#txtFromDate').val(objres[0].ArrivalDate);//thời gian đến (booking)
                                $('#txtToDate').val(objres[0].LeaveDate);//thời gian đi (booking)
                                //self.setDateTimePicker($('#dpFromDate'), 'DD/MM/YYYY HH:mm')
                                //self.setDateTimePicker($('#dpToDate'), 'DD/MM/YYYY HH:mm')
                                $('#txtAdult').val(objres[0].Adult);//số người lớn
                                $('#txtChildren').val(objres[0].Children);//số trẻ nhỏ
                                $('#dllPriceDipLe').val(objres[0].Holiday);//giá phòng theo dịp (tính theo ngày thường, ngày lễ)
                                $('#dllPriceKhungGio').val(objres[0].KhungGio);//giá phòng theo khung (tính theo giờ, giá cả ngày, giá đêm)
                                $('#txtPrice').val(objres[0].Price);//giá phòng (tính theo loại đặt phòng)
                                $('#txtDeposit').val(objres[0].Deposit);//tiền cọc
                                $('#txtDiscount').val(objres[0].Discount);//giảm trừ
                                $('#txtNote').val(objres[0].Note); //ghi chú 
                            }
                                
                        }
                        
                        // message
                        self.notify(mess[0], mess[1], false);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        self.notify(thrownError, -1, false);
                    }
                })
            }
        })
    }
    
    // fn_Thêm/Sửa thông tin bạn cùng phòng  ---> DONE add friend, not done edit friend
    this.ReservationAddEditFriend = function () {
        $('#btnAdd').click(function () {
            form = self.getParentByTagName(this, 'form');

            cbLeader = $('#cbLeader input[name=chkLeaderf]:checked').val();
            cbPayer = $('#cbPayer input[name=chkPayerf]:checked').val();
            rbSex = $('#rbSex input[name=r2f]:checked').val();

            // gan cac doi tuong vao object
            cust = {
                //THÔNG TIN KHÁCH HÀNG    
                'ID' : 0,
                'FullName': self.getById('txtnamef').value.trim(),//Tên đầy đủ
                'Sex': rbSex,//self.getById('txtname').value.trim(),//giới tính
                'DOB': self.getById('txtDOBf').value.trim(),//ngày tháng năm sinh dd/mm/yyyy
                'IdentityNumber': self.getById('txtIdentifyf').value.trim(),// CMND/Hộ chiếu ....
                'CitizenshipCode': self.getById('dllQuocTichf').value.trim(),// Quốc tịch
                'Address': self.getById('txtaddressf').value.trim(),//địa chỉ
                'Email': self.getById('txtEmailf').value.trim(),//email
                'Mobile': self.getById('txtMobilef').value.trim(),//mobile
                'Company': self.getById('txtCompanyf').value.trim(),//Company
                'RoomID': self.getById('dllRoom').value.trim(),//ID phòng
                'GroupID': self.getById('dllOrgf').value.trim(),////Code đoàn (nếu là đặt phòng đi theo đoàn)
                'GroupJoinID': self.getById('dllOrgJoinf').value.trim(),//ID phòng
                'Leader': cbLeader,//là Trưởng đoàn
                'Payer': cbPayer,//ID phòng
                'ReserCode': self.getById('txtResCode').value.trim(),//mã đặt phòng
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
                        self.notify(mess[0], mess[1], false);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        self.notify(thrownError, -1, false);
                    }
                })
            }
        })

    }

    // thay doi button save, title cho modal khi them/xoa
    /**
        isAdd: true-> them moi, false: sua
    */
    this.changeModal = function (isAdd) {
        var title, btSave, textTitle = '', textBtn = '';
        title = $('#divFriend').find('.modal-header .modal-title')[0];
        if (isAdd) {
            textTitle = 'Thêm khách ở cùng';
        } else {
            textTitle = 'Sửa khách ở cùng';
        }
        // thay text
        title.innerHTML = textTitle;
    }

    // show modal insert Friend-->DONE
    this.callMdInsert = function () {
        var btnAdd = $('.add-friend');
        btnAdd.on('click', function (e) {
            e.preventDefault();
            //Clear control
            self.ClearControls("#myModal");
            self.changeModal(true); //insert
            m_check_modify = false;//insert

            //change text Lưu -> thêm
            $("#btnAdd").html('Thêm');
            //enable nút add
            $("#btnAdd").prop('disabled', false); 
        })
    }

    // show modal edit Friend
    this.callModalEdit = function () {
        var btnEdit = $('.edit-friend');
        btnEdit.on('click', function (e) {
            e.preventDefault();
            //alert('edit-friend');
            //if (!$(this)[0].hasAttribute("data-toggle")) {
            //    self.notify('Vui lòng nhận phòng trước khi thêm bạn.',-1, false);
            //}
            //Clear control
            self.ClearControls("#myModal");
            self.changeModal(false); //edit
            m_check_modify = true; //edit  --> dùng để check lúc lưu thông tin edit

            //change text thêm -> lưu
            $("#btnAdd").html('Lưu');
            //enable nút add
            $("#btnAdd").prop('disabled', false);

            //Fill data 
            form = self.getParentByTagName(this, 'form');
            // gan cac doi tuong vao object
            cust = {  
                'ID': $(this).data("id")
            }

            url = '/ReservationRoom/ReservationGetFriend';

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: { customer: cust },
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;

                        if (mess[1] == 1) { //success                            
                            //lay lai data fill vao cac control
                            if (mess[2] != null) {
                                var obj = mess[2];
                                //$('#hdFCusID').val(obj[0].ID);
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
                        // success message
                        self.notify(mess[0], mess[1], false);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        self.notify(thrownError, -1, false);
                    }
                })
            }
        })
    }

    // fn_Huy thong tin dat phong --> DONE
    this.ReservationCancel = function () {
        $('#btnCancel').click(function () {
            form = self.getParentByTagName(this, 'form');
            // gan cac doi tuong vao object
            resroom = {
                'ReservationCode': self.getById('txtResCode').value.trim()//mã đặt phòng
            }

            url = '/ReservationRoom/ReservationCancel';

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: { resroomcancel: resroom },
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;

                        if (mess[1] == 1) { //success    
                            //Clear control
                            self.ClearControls("#frmresroom");

                            //change text Lưu -> thêm
                            $("#btnAdd").html('Thêm');
                            //enable nút add
                            $("#btnAdd").prop('disabled', false);

                            //dis active link add friend 
                            $('#addFriend').attr('data-toggle', 'modal');
                            //enable nút nhận  phòng
                            $('#btnResSave').prop('disabled', false);
                            //disable nút Hủy
                            $('#btnCancel').prop('disabled', true);
                        }

                        m_check_modify = false;
                        // success message
                        self.notify(mess[0], mess[1], false);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        self.notify(thrownError, -1, false);
                    }
                })
            }
        })

    }
    
    //Reset all controls
    this.ClearControls = function(ParentControl) {
        var control = $(ParentControl).get(0);
        var list = control.getElementsByTagName("input");
        var textAreaList = control.getElementsByTagName("textarea");
        var selectList = control.getElementsByTagName("select");
        //input
        for (var i = 0; i < list.length; i++) {
            var attr = list[i].getAttribute('type');
            var control = $('#' + list[i].id);
            var controlclass = $('.minimal-red');
            switch (attr) {
                case "text":
                case "hidden":
                    $(control).val('');
                    break;

                case "checkbox":
                case "radio":
                    //control.checked = false;
                    controlclass.removeClass("checked"); //do đang dùng thư viện icheck
                    controlclass.attr("aria-checked", false)
                    break;

                case "range":
                    control.setAttribute('value', '0');
                    break;

                case "file":
                    control.setAttribute('value', '');
                    break;
            }
        }
        //textArea
        for (var i = 0; i < textAreaList.length; i++) {
            var control = document.getElementById(textAreaList[i].id);
            $(control).val('');
        }
        //select
        for (var i = 0; i < selectList.length; i++) {
            if (selectList[i].id === 'dllQuocTichf' || selectList[i].id === 'dllQuocTich') {
                $(selectList[i]).val('238')
            }
            else {
                $(selectList[i]).val('0');
            }
        }
    }

    //validate Nhan Phong
    $("form").validate({
        rules: {
            txtname: {
                required: true
            },
            txtIdentify: {
                required: true
            },
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
            txtIdentify: {
                required: "Vui lòng nhập CMND/GPLX"
            },
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

    //validate Ban cung phong
    //$("#form").validate({
    //    rules: {
    //        txtnamef: {
    //            required: true
    //        },
    //        txtIdentifyf: {
    //            required: true
    //        },
    //        txtDOBf: {
    //            required: true,
    //            length: 10
    //        }
    //    },
    //    messages: {
    //        txtnamef: {
    //            required: "Vui lòng nhập Họ và tên"
    //        },
    //        txtIdentifyf: {
    //            required: "Vui lòng nhập CMND/GPLX"
    //        },
    //        txtDOBf: {
    //            required: "Vui lòng nhập Ngày tháng năm sinh",
    //            length: "Độ dài là 10 ký tự"
    //        }
    //    },
    //    highlight: function (element) {
    //        $(element).closest('.form-group').addClass('has-error');
    //    },
    //    unhighlight: function (element) {
    //        $(element).closest('.form-group').removeClass('has-error');
    //    },
    //    errorElement: 'span',
    //    errorClass: 'help-block',
    //    errorPlacement: function (error, element) {
    //        if (element.parent('.input-group').length) {
    //            error.insertAfter(element.parent());
    //        } else {
    //            error.insertAfter(element);
    //        }
    //    }
    //});    

    //fnReady    
    this.fnReady = function () {

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
                $("#txtPrice").val(self.formatNumber(price));
                $("div").remove(".notifyjs-container");
            }
            else {
                self.notify('Vui lòng nhập Giá phòng dạng số.', -1, false);
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
            }
            total = self.formatNumber(total);
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

    
    this.init = function () {
        this.ReservationSave();
        this.callMdInsert();
        this.callModalEdit();
        this.ReservationAddEditFriend();
        this.ReservationCancel();
        this.fnReady();
    }
}
$(document).ready(function () {
    g_Reservation = new ReservationRoom();
    g_Reservation.init();    

})