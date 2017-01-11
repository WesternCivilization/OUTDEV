/*ThichPV 05/12/2016 14:58:06
Nhóm tài khoản chính là bộ phận
*/
var g_GroupType;
function GroupType() {
    var self = this;
    // check xem co phai la sua khong?
    // update = true
    var m_check_modify = false;
    // ma khach san
    var m_ID = '';
    // tr
    var m_tr_modify;
    // data row truoc khi update
    var m_data;
    // ke thua OzeBase
	OzeBase.apply(this, arguments);
    // tao bang
	this.renderGroupTypeTable = function () {
	    var table = self.getById('ListGroupType');
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
                { "data": "ID", className: "none" },
				{ "data": "NameVN", className: "all" },
				{ "data": "NameEN", className: "all" },
				{ "data": "Order", className: "all" },
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
							return 'Chi tiết bộ phận ' + data.NameVN;
						}
					}),
					renderer: function (api, rowIdx, columns) {
					    var data = $.map(columns, function (col, i) {
					        return col.columnIndex !== 0 && col.columnIndex !== 1 && col.columnIndex !== 3 ?// k show column thao tac
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
	this.callMdInsertGroupType = function () {
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
	this.callModalEditGroupType = function () {
	    var btnEdit = $('.' + self.m_BTN_CLASS_EDIT_TABLE);
	    btnEdit.click(function (e) {
	        e.preventDefault();
            // reset cac bien
	        m_ID = '';
	        m_data = null;
	        m_tr_modify = null;
	        
	        var $modal, title, btSave, data, table, tr;
	        tr = $(this).parents('tr');
	        m_tr_modify = tr;
	        table = tr.parents('table');
	        data = self.getDataRowTable(table, tr);
	        m_data = data;
	        m_ID = m_data.ID;
	        $modal = $('#mdModify');
	        self.resetForm($modal[0]);
	        self.fillDataToModal(m_data, $modal);
	        // thay text title, button
	        self.changeModal($modal, false);
	        m_check_modify = true;
            
	    })
	}
    // add/edit GroupType 
	this.modifyGroupType = function () {
	    var btnSave = $('#' + self.m_BTN_ID_SAVE);
	    btnSave.click(function () {
	        var form, grouptype, url;
	        form = self.getParentByTagName(this, 'form');
            // lay du lieu checkbox status
	        var status = $('.radio-group > label > div.checked input').parents('label').attr('data');
            // gan cac doi tuong vao object
	        grouptype = {
	            'ID': self.getById('ID').value.trim(),
	            'NameVN': self.getById('NameVN').value.trim(),
	            'NameEN': self.getById('NameEN').value.trim(),
	            'Order': self.getById('Order').value.trim()
	        }
            // thay doi url theo them/sua
	        if (m_check_modify) {
	            url = m_UrlList.UrlUpdate;
	            grouptype.ID = m_ID;
	        } else {
	            url = m_UrlList.UrlCreatre;
	        }
	        
	        if ($(form).valid()) {
	            $.ajax({
	                url: url,
	                type: "Post",
	                datatype: "json",
	                data: grouptype,
	                success: function (response) {
	                    var mess, result;
	                    mess = response.mess;
	                    var table = $('#ListGroupType').dataTable();
	                    
	                    if (!m_check_modify && mess[1] !== null) {
	                        var obj = mess[1][0];
	                        grouptype.STT = '';
	                        grouptype.ID = obj.ID;
	                        grouptype.Modify = '<div class="edit-delete-table">' +
                                                    '<div class="edit-table" data-toggle="modal" data-backdrop="static" data-target="#mdModify">' +
                                                        '<i class="fa fa-pencil" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                    '<div class="delete-table"  data-toggle="modal" data-backdrop="static" data-target="#mdDelete">' +
                                                        '<i class="fa fa-times" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                '</div>';
                                            
	                        self.addRowToDatatable(table, grouptype);
	                        self.notify(mess[0][0], mess[0][1]);
	                    } else {
	                        // chinh lai trang thai status
	                        //grouptype.Status = grouptype.Status === "Close" ? "Khóa" : "Hoạt động";
                            // gop data truoc khi update va sau khi update
	                        var data = self.mergeObject(m_data, grouptype);
	                        self.updateRowToDatatable(table, data, m_tr_modify);
	                        m_data = null;
	                        m_tr_modify = false;
	                        m_ID = '';
	                        self.notify(mess[0], mess[1]);

	                    }
	                    m_check_modify = false;
	                    // an modal
	                    var md = $('#mdModify');
	                    md.modal('hide');
	                    // lam rong cac textbox
	                    self.resetForm(md[0]);
                        // reset lai event click button edit
	                    self.callModalEditGroupType();
	                    // reset lai event click delete
	                    self.callModalDeleteGroupType();
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
	this.callModalDeleteGroupType = function () {
	    var btnDelete = $('.delete-table');
	    btnDelete.click(function (e) {
	        e.preventDefault();
	        var $modal, title, data, table, tr;
	        tr = self.getParentByTagName(this, 'tr');
	        table = self.getParentByTagName(tr, 'table');
	        data = self.getDataRowTable($(table), $(tr));
	        $modal = $('#mdDelete');
	        // thay text titleton
	        content = 'Bạn có muốn xóa bộ phận ' + '<label class="name-bold">'+data.Name + ' (' + data.Code + ')</label> không?';
	        $modal.find('.modal-body-content > p').html(content);
	        self.delteGroupType(tr, data.ID);
	    })
	}
    /*
        Xoa bộ phận
    */
	this.delteGroupType = function (tr, ID) {
	    $('#btnDelete').click(function (e) {
	        e.preventDefault();
	        $.ajax({
	            url: m_UrlList.UrlDelte,
	            type: "Post",
	            datatype: "json",
	            data: {'ID': ID},
	            success: function (response) {
	                var mess, result;
	                mess = response.mess;
	                var table = $('#ListGroupType').dataTable();
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
	    this.renderGroupTypeTable();
        //insert
	    this.callMdInsertGroupType();
	    //update
	    this.callModalEditGroupType();
	    this.modifyGroupType();
	    // delete
	    this.callModalDeleteGroupType();
	}
}
$(document).ready(function () {
    g_GroupType = new GroupType();
    g_GroupType.init();
})