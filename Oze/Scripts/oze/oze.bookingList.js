var g_BookingList;
function BookingList() {
    var self = this;
    var m_table_id = 'tbBookingList';
    var m_table;
    var m_rowspage = 5;
    var m_pageCount = 0;
    var m_startPage = 1;
    var data = {
        "pageCount": "10",
        "data": [{
            "CustomerName": "Name",
            "BookingCode": "Code",
            "ReservationType": "Type",
            "ArrivalDate": "CheckinDate",
            "LeaveDate": "CheckoutDate",
            "ReservationStatus": "Status"
        },
          {
              "CustomerName": "Name2",
              "BookingCode": "Code2",
              "ReservationType": "Type2",
              "ArrivalDate": "CheckinDate2",
              "LeaveDate": "CheckoutDate2",
              "ReservationStatus": "Status2"
          }]
    }
    var m_temp_modify = '<div class="modify-columns">' +
                            '<div class="btn-modify-detail"><i class="icon-modify icon-detail" data-toggle="tooltip" data-container="body" data-placement="top" title="Chi tiết"></i></div>' +
                            '<div class="btn-modify-set"><i class="icon-modify icon-set" data-toggle="tooltip" data-container="body" data-placement="top" title="Gán phòng"></i></div>' +
                            '<div class="btn-modify-change"><a class="icon-modify icon-change" data-toggle="tooltip" data-container="body" data-placement="top" title="Đổi phòng"></a></div>' +
                            '<div class="btn-modify-delete"><i class="icon-modify icon-delete" data-toggle="tooltip" data-container="body" data-placement="top" title="Xóa phòng"></i></div>' +
                        '</div>';
    // ke thua oze.base.js
    OzeBase.apply(this, arguments);

    this.clickDateIcon = function () {
        var icons = self.getByCl('input-group-addon');
        for (var i = 0; i < icons.length; i++) {
            icons[i].addEventListener('click', function () {
                var parent, input;
                parent = self.getParentByCl(this, 'input-group');
                input = self.getByTag('input',parent)[0];
                //self.simulate(input, 'click');
                input.focus();
            })
        }
    }
    this.callDate = function () {
        var $from, $to;
        $from = $('#dpkArriveDate');
        $to = $('#dpkLeaveDate');
        self.setDateRangePicker($from, $to, self.m_FORMAT_DATE);
        self.clickDateIcon();
    }

    
    
    this.renderTR = function (data, no) {
        var TR = '', numTD;
        TR += '<td>' + no + '</td>';
        
        //numTD = self.getSizeObject(data);
        //for (var i = 0; i < arr.length; i++) {
        //    TR += '<td>' + data[arr[i]] + '</td>';
        //}
        //TR += '<td>thao tac</td>';
        //return TR;
    }
    this.renderTable = function (tableID, _data) {
        var table, TBODY = '', pageCount, data;
        _data = JSON.parse(_data);
        pageCount = parseInt(_data.pageCount);
        table = $('#' + tableID).dataTable();
        if (pageCount !== 0) {
            data = _data.data;
            
            table.fnClearTable();
            for (var i = 0; i < data.length; i++) {
                data[i].STT = '<td></td>';
                data[i].Modify = '<td>' + m_temp_modify + '</td>';
                self.addRowToDatatable(table, data[i])
            }
        } else {
            table.fnClearTable();
            pagination = $('.pagination-table').remove();
        }
        
        return pageCount;
    }
    this.callDataTable = function (tableId) {
        var tb = $('#' + tableId);
        var object = {
            "paging": false,
            "lengthChange": false,
            "searching": false,
            "ordering": true,
            "info": false,
            "autoWidth": true,
            // them button add, export
            
            // gan nhan, class cho column
            "columns": [
				{ "data": "STT", className: "all" },
				{ "data": "CustomerName", className: "all" },
				{ "data": "BookingCode", className: "all" },
				{ "data": "ReservationType", className: "all" },
				{ "data": "ArrivalDate", className: "all" },
				{ "data": "LeaveDate", className: "all" },
				{ "data": "ReservationStatus", className: "all" },
				{ "data": "Modify", className: "all" },
            ],
            // sort default
            "order": [
                [1, 'asc']
            ],
            // nhung th nao co class=no-sort => k co sort
            "columnDefs": [{
                "targets": 'no-sort',
                "orderable": false,
            }, {
                "width": "165px",
                "targets": 7
            }
            
            ]
        }
        m_table = self.SetDatatable(tb, object)
    }
    this.clickSearch = function (searchID) {
        $('#' + searchID).click(function () {
            var currentPage = 1, pagination;
            self.sendServer(currentPage);
            
        })
    }
    this.sendServer = function (currentPage) {
        var object, ariveDate, leaveDate, toDate, checkDate;
        ariveDate = $('#dpkArriveDate').val();
        leaveDate = $('#dpkLeaveDate').val();
        checkDate = $('input[type="checkbox"]').parent().attr('aria-checked');
        if (checkDate) {
            ariveDate = '';
            leaveDate = '';
        }
        object = {
            "ReservationType": $('#ddlReservationType').val(),
            "ReservationStatus": $('#ddlReservationStatus').val(),
            "RoomID": $('#ddlRoom').val(),
            "RoomTypeID": $('#ddlRoomType').val(),
            "ArrivalDate": ariveDate,
            "LeaveDate": leaveDate,
            "BookingCode": $('#txtBookingCode').val(),
            "CustomerName": $('#txtCustomerName').val(),
        }
        $.ajax({
            url: '/Receptionist/SearchBooking',
            type: "Post",
            datatype: "json",
            data: {
                mdReservationRoom: object,
                CurrentPage: currentPage,
                NumInPage: m_rowspage
            },
            success: function (response) {
                var mess, pageCount;
                mess = response.mess[0];
                pageCount = self.renderTable(m_table_id, mess);
                if (pageCount!== 0) {
                    pagination = $('.pagination-table');
                    self.createPaginationTable(pagination, pageCount, currentPage, g_BookingList.sendServer);
                }
                
                self.removeClass(self.getByCl('group-grid')[0], 'hide');// xoa class hide cho grid
                self.detailBooking();
                self.setRoom();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                // thong bao loi k success
                self.notify(thrownError, -1);
            }
        })
        
    }
    // check chon search date
    this.clickCheckDate = function (checkboxID) {
        $('#' + checkboxID).siblings('ins').click(function () {
            var pickDate = self.getByCl('pick-date');
            for (var i = 0; i < pickDate.length; i++) {
                pickDate[i].disabled = pickDate[i].disabled? false: true;
            }
        })
    }
    this.createPaginationTable = function (element, pageCount, startPage, callback) {
        self.createPagination(element, pageCount, startPage, callback);
    }
    this.detailBooking = function () {
        $('.icon-detail').click(function (e) {
            var tr, table, data, bookingCode;
            tr = $(this).parents('tr');
            table = tr.parents('table');
            data = self.getDataRowTable(table, tr);
            bookingCode = data.BookingCode;
            window.open('/Receptionist/DetailBooking?BookingCode=' + bookingCode, '_parent')
        })
    }
    this.setRoom = function () {
        $('.icon-set').click(function (e) {
            var tr, table, data, bookingCode, modal;
            tr = $(this).parents('tr');
            table = tr.parents('table');
            data = self.getDataRowTable(table, tr);
            bookingCode = data.BookingCode;
            modal = $('#mdSetRoom');
            
            ///window.open('/Receptionist/GetRoomForReservationRoom?BookingCode=' + bookingCode, '_parent')
            $.ajax({
                url: '/Receptionist/GetRoomForReservationRoom',
                type: "Post",
                datatype: "json",
                data: {
                    BookingCode: bookingCode
                },
                success: function (response) {
                    var mess, oldRoomInfo;
                    mess = response.mess;
                    oldRoomInfo = self.fillDataToSetRoom(mess, modal);
                    modal.modal({ backdrop: 'static', keyboard: false })
                    self.saveSetRoom(modal, bookingCode, oldRoomInfo);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    // thong bao loi k success
                    self.notify(thrownError, -1);
                }
            })
            
        })
    }
    this.saveSetRoom = function (modal, bookingCode, oldRoomInfo) {
        var form = modal.parents('form');
        this.setVallid(form);
       
        modal.find('#btnSave').click(function () {
            if (form.valid()) {
                var newRoom, newroomType;
                newRoom = modal.find('#ddlRoom').val();
                newroomType = modal.find('#ddlRoomType').val();
                $.ajax({
                    url: '/Receptionist/SetRoomForReservationRoom',
                    type: "Post",
                    datatype: "json",
                    data: model = {
                        BookingCode: bookingCode,
                        New_Room_ID: newRoom,
                        Old_Room_ID: oldRoomInfo.oldRoom,
                        ID: oldRoomInfo.ID
                    },
                    success: function (response) {
                        var mess = response.mess;
                        self.notify(mess[0]);
                        modal.modal('hide');
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // thong bao loi k success
                        self.notify(thrownError, -1);
                    }
                })
            }
        })
        self.closeModal(modal);
    }
    this.closeModal = function (modal) {
        modal.find('.close').click(function(){
            self.resetValid();
        });
    }
    this.setVallid = function (form) {
        form.validate({
            rules: {
                RoomID: "required",
                Room_Type_ID: "required"
            },
            messages: {
                RoomID: "Bạn chưa chọn phòng",
                Room_Type_ID: "Bạn chưa chọn hạng phòng"
            }
        })
    }
    this.resetValid = function () {
        $('label.error').remove();
        $('.error')
           .removeClass('error');
    }
    this.resetModal = function () {
        text = modal.find('p[data]');
    }
    this.fillDataToSetRoom = function (data, modal) {
        var roomType, room, arrPrice;
        
        arrPrice = ['Deposit', 'Deduction', 'Price'];
        for (var i = 0; i < data.length; i++) {
            var child = JSON.parse(data[i])[0];
            for (var key in child) {
                var value = child[key];
                if (arrPrice.indexOf(key) > -1) {
                    value = self.formatNumber(value);
                    if (key === "Deduction") {
                        if (value === "0") value = "Không có";
                    }
                }
                var el = modal.find('p[data="'+key+'"]');
                if (el.length > 0) {
                    el[0].innerHTML = value;
                }
                if (key === "New_Room_ID") {
                    modal.find('#ddlRoom').val(value);
                }
                if (key === "Room_Type_ID") {
                    modal.find('#ddlRoomType').val(value);
                }
            }
        }
        return JSON.parse(data[1])[0];
    }
    this.init = function () {
        self.buildStyleRadio(); // style checkbox
        self.callDate(); //
        self.callDataTable(m_table_id);
        self.clickSearch('btnSearch');
        self.clickCheckDate('cbxDate');
    }
}
$(document).ready(function () {
    g_BookingList = new BookingList();
    g_BookingList.init();
})