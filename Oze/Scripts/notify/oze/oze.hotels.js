/*NamLD 11/11/2016 04:58:06*/
var g_Hotels;
function Hotels() {
    var self = this;
    // check xem co phai la sua khong?
    // update = true
    var m_check_modify = false;
    // ma khach san
    var m_Code = '';
    // tr
    var m_tr_modify;
    // data row truoc khi update
    var m_data;
    // ke thua OzeBase
	OzeBase.apply(this, arguments);
    // tao bang
	this.renderHotelTable = function () {
		var table = self.getById('tbHotels');
	    var object = {
	        "paging": true,
	        "lengthChange": false,
	        "searching": true,
	        "ordering": true,
	        "info": true,
	        "autoWidth": true,
            // them button add, export
	        "bCustomHeader": {
	            "add": true,
	            "export": true
	        },
            // gan nhan, class cho column
			"columns": [
				{ "data": "STT", className: "all" },
				{ "data": "Code", className: "all" },
				{ "data": "Name", className: "all" },
				{ "data": "Address", className: "min-tablet" },
				{ "data": "Phone", className: "desktop" },
				{ "data": "Mobile", className: "desktop" },
				{ "data": "Email", className: "desktop" },
				{ "data": "Website", className: "none" },
				{ "data": "LogoUrl", className: "none" },
				{ "data": "RoomCount", className: "none" },
				{ "data": "Description", className: "none" },
				{ "data": "CreateByUser", className: "none" },
				{ "data": "CreateDate", className: "none" },
				{ "data": "ModifyByUser", className: "none" },
				{ "data": "ModifyDate", className: "none" },
				{ "data": "Status", className: "all" },
				{ "data": "Modify", className: "all" },
			],
            // sort default
			"order": [
                [2, 'asc']
			],
			// button chi tiet
			responsive: {
				details: {
					display: $.fn.dataTable.Responsive.display.modal({
						header: function (row) {
							var data = row.data();
							return 'Chi tiết KS ' + data.Name;
						}
					}),
					renderer: function (api, rowIdx, columns) {
					    var data = $.map(columns, function (col, i) {
					        return col.columnIndex !== 0 && col.columnIndex !== 16 ?// k show column thao tac
                                '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                                    '<td>' + col.title + ':' + '</td> ' +
                                    '<td>' + col.data + '</td>' +
                                '</tr>' :
                                '';
					    }).join('');

					    return data ?
                            $('<table class="table table-striped table-bordered nowrap table-hover"/>').append(data) :
                            false;
					}
				}
			}
		}
	    var tbLast = self.SetDatatable($(table), object);
	}
    // show modal insert
	this.callMdInsertHotel = function () {
	    var btnAdd = $('#' + self.m_BTN_ID_ADD_ROW);
	    btnAdd.on('click', function (e) {
	        e.preventDefault();
	        var $modal, title, btSave;
	        $modal = $('.' + self.m_MODAL_CLASS);
	        self.resetForm($modal[0]);
	        self.changeModal($modal, true);
	        m_check_modify = false;
	    })
	}

    // show modal edit
	this.callModalEditHotel = function () {
	    var btnEdit = $('.' + self.m_BTN_CLASS_EDIT_TABLE);
	    btnEdit.click(function (e) {
	        e.preventDefault();
            // reset cac bien
	        m_Code = '';
	        m_data = null;
	        m_tr_modify = null;
	        
	        var $modal, title, btSave, data, table, tr;
	        tr = $(this).parents('tr');
	        m_tr_modify = tr;
	        table = tr.parents('table');
	        data = self.getDataRowTable(table, tr);
	        m_data = data;
	        m_Code = m_data.Code;
	        $modal = $('#mdModify');
	        self.resetForm($modal[0]);
	        self.fillDataToModal(m_data, $modal);
	        // thay text title, button
	        self.changeModal($modal, false);
	        m_check_modify = true;
            
	    })
	}
    // add/edit hotel 
	this.modifyHotel = function () {
	    var btnSave = $('#' + self.m_BTN_ID_SAVE);
	    btnSave.click(function () {
	        var form, hotel, url;
	        form = self.getParentByTagName(this, 'form');
            // lay du lieu checkbox status
	        var status = $('.radio-group > label > div.checked input').parents('label').attr('data');
            // gan cac doi tuong vao object
	        hotel = {
	            'Name': self.getById('Name').value.trim(),
	            'Address': self.getById('Address').value.trim(),
	            'Phone': self.getById('Phone').value.trim(),
	            'Mobile': self.getById('Mobile').value.trim(),
	            'Email': self.getById('Email').value.trim(),
	            'Website': self.getById('Website').value.trim(),
	            'LogoUrl': self.getById('LogoUrl').value.trim(),
	            'RoomCount': self.getById('RoomCount').value.trim(),
	            'Description': self.getById('Description').value.trim(),
	            'Status': status
	        }
            // thay doi url theo them/sua
	        if (m_check_modify) {
	            url = m_UrlList.UrlUpdate;
	            hotel.Code = m_Code;
	        } else {
	            url = m_UrlList.UrlCreatre;
	        }
	        
	        if ($(form).valid()) {
	            $.ajax({
	                url: url,
	                type: "Post",
	                datatype: "json",
	                data: hotel,
	                success: function (response) {
	                    var mess, result;
	                    mess = response.mess;
	                    var table = $('#tbHotels').dataTable();
	                    
	                    if (!m_check_modify && mess[1] !== null) {
	                        var obj = mess[1][0];
	                        hotel.STT = '';
	                        hotel.Code = obj.Code;
	                        hotel.CreateByUser = obj.Createby;
	                        hotel.CreateDate = obj.CreateDate;
	                        hotel.ModifyByUser = obj.Modifyby;
	                        hotel.ModifyDate = obj.ModifyDate;
	                        hotel.Modify = '<div class="edit-delete-table">' +
                                                    '<div class="edit-table" data-toggle="modal" data-backdrop="static" data-target="#mdModify">' +
                                                        '<i class="fa fa-pencil" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                    '<div class="delete-table"  data-toggle="modal" data-backdrop="static" data-target="#mdDelete">' +
                                                        '<i class="fa fa-times" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                '</div>';
                                            
	                        self.addRowToDatatable(table, hotel);
	                        self.notify(mess[0][0], mess[0][1]);
	                    } else {
	                        // chinh lai trang thai status
	                        hotel.Status = hotel.Status === "Close" ? "Khóa" : "Hoạt động";
                            // gop data truoc khi update va sau khi update
	                        var data = self.mergeObject(m_data, hotel);
	                        self.updateRowToDatatable(table, data, m_tr_modify);
	                        m_data = null;
	                        m_tr_modify = false;
	                        m_Code = '';
	                        self.notify(mess[0], mess[1]);

	                    }
	                    m_check_modify = false;
	                    // an modal
	                    var md = $('#mdModify');
	                    md.modal('hide');
	                    // lam rong cac textbox
	                    self.resetForm(md[0]);
                        // reset lai event click button edit
	                    self.callModalEditHotel();
	                    // reset lai event click delete
	                    self.callModalDeleteHotels();
	                },
	                error: function (xhr, ajaxOptions, thrownError) {
	                    // thong bao loi k success
	                    self.notify(thrownError, -1);
	                }
	            })
	        }
	    })
	}
    // do data vao textbox modal
	this.fillDataToModal = function (data, modal) {
	    var texbox, grRadio, divHide, arrOverLook = ['STT', 'Modify'];
	    textbox = modal.find('.txt-content');
	    grRadio = modal.find('.radio-group');
	    divHide = modal.find('.form-group.hide');
        // xoa class hide
	    divHide.removeClass('hide');
	    for (var key in data) {
	        var value = '';
	        value = data[key];
            // bỏ qua các cột stt, thao tác
	        if (arrOverLook.indexOf(key) > -1) {
	            continue;
	        }
	        if (key === 'Status') {
	            var checkbox;
	            if (value === "Hoạt động") {
	                checkbox = $('.radio-group label[data="Open"]')[0];
	                self.simulate(checkbox, 'click');
	            } else {
	                checkbox = $('.radio-group label[data="Close"]')[0];
	                self.simulate(checkbox, 'click');
	            }
	        }
	        for (var i = 0; i < textbox.length; i++) {
	            var child;
	            child = textbox[i];
	            if (child.hasAttribute('data')) {
	                if (child.getAttribute('data') === key) {
	                    if (child.tagName.toLocaleLowerCase() === 'label') {
	                        child.innerHTML = value;
	                    } else {
	                        child.value = value;
	                    }
	                    break;
	                }
	            }

	        }
	    }
	}
    // reset modal
	this.resetForm = function (modal) {
	    var form, groupHide, validator;
	    form = self.getParentByTagName(modal, 'form');
	    groupHide = $(form).find('[data-insert]');
	    for (var i = 0; i < groupHide.length; i++) {
	        var child
	        child = groupHide[i];
	        if (child.getAttribute('data-insert') === 'hide') {
	            self.addClass(child, 'hide');
	        }
	    }
	    form.reset();
	    // reset validate
	    this.resetValid();
	}
    // thay doi button save, title cho modal khi them/xoa
    /**
        isAdd: true-> them moi, false: sua
    */
	this.changeModal = function (modal, isAdd) {
	    var title, btSave, textTitle = '', textBtn = '';
	    title = modal.find('.modal-header .modal-title')[0];
	    if (isAdd) {
	        textTitle = self.m_BTN_TEXT_CREATE + ' ' + m_Title;
	    } else {
	        textTitle = self.m_BTN_TEXT_EDIT + ' ' + m_Title;
	    }
	    // thay text
	    title.innerHTML = textTitle;
	}
    /*
    thay doi text trong modal delete
    */
	this.callModalDeleteHotels = function () {
	    var btnDelete = $('.delete-table');
	    btnDelete.click(function (e) {
	        e.preventDefault();
	        var $modal, title, data, table, tr;
	        tr = self.getParentByTagName(this, 'tr');
	        table = self.getParentByTagName(tr, 'table');
	        data = self.getDataRowTable($(table), $(tr));
	        $modal = $('#mdDelete');
	        // thay text titleton
	        content = 'Bạn có muốn xóa khách sạn ' + '<label class="name-bold">'+data.Name + ' (' + data.Code + ')</label> không?';
	        $modal.find('.modal-body-content > p').html(content);
	        self.delteHotels(tr, data.Code);
	    })
	}
    /*
        Xoa hotel
    */
	this.delteHotels = function (tr, code) {
	    $('#btnDelete').click(function (e) {
	        e.preventDefault();
	        $.ajax({
	            url: m_UrlList.UrlDelte,
	            type: "Post",
	            datatype: "json",
	            data: {'hotel': code},
	            success: function (response) {
	                var mess, result;
	                mess = response.mess;
	                var table = $('#tbHotels').dataTable();
	                self.deleteRow(table, tr);
	                self.notify(mess[0], mess[1]);
	                // an modal
	                var md = $('#mdDelete');
	                md.modal('hide');
	                // reset lai event click button edit
	                self.callModalEditHotel();
	            },
	            error: function (xhr, ajaxOptions, thrownError) {
	                // thong bao loi k success
	                self.notify(thrownError, -1);
	            }
	        })
	    })
	    
	}
	this.init = function () {
	    self.buildStyleRadio(self.m_RADIO_BTN_CLASS_MINIAL_RED);
	    this.renderHotelTable();
        //insert
	    this.callMdInsertHotel();
	    //this.insertHotel();
	    //update
	    this.callModalEditHotel();
	    this.modifyHotel();
	    // delete
	    this.callModalDeleteHotels();
	}
}
$(document).ready(function () {
	g_Hotels = new Hotels();
	g_Hotels.init();
})