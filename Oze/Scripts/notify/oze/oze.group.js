/*ThichPV-24/10/2016 Design Group Table*/
var g_Group;
function Group() {
    var self = this;
    // check xem co phai la sua khong?
    // update = true
    var m_check_modify = false;
    var m_ID = 0;
    // tr
    var m_tr_modify;
    // data row truoc khi update
    var m_data;
    // ke thua OzeBase
    OzeBase.apply(this, arguments);
    // Tao bang Group
    this.renderGroupTable = function () {
        //var table = self.getById('GridGroup');
        var idList = self.getById('hdIDList').value;
        var arr = idList.split(',');
        for (var i = 0; i < arr.length; i++) {
            this.Rendertable(arr[i]);
        }
    }
    this.Rendertable = function (GridID) {
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": false,
            // them button add, export
            "bCustomHeader": {
                "add": true,
                "export": true
            },
            // gan nhan, class cho column
            "columns": [
				{ "data": "STT", className: "all" },
                { "data": "ID", className: "none" },
				{ "data": "GroupName", className: "all" },
                { "data": "GroupType", className: "none" },
                { "data": "GroupTypeName", className: "all" },
                { "data": "SysHotelID", className: "none" },
                { "data": "SysHotelName", className: "none" },
				{ "data": "Description", className: "all" },
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
                            return 'Chi tiết nhóm quyền ' + data.GroupName;
                        }
                    }),
                    renderer: function (api, rowIdx, columns) {
                        var _array = [0, 3, 5, 9];
                        var data = $.map(columns, function (col, i) {
                            return _array.indexOf(col.columnIndex) === -1 ?// k show column thao tac
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
        var tbLast = self.SetDatatable($('#'+GridID), object);
    }

    // show modal insert
    this.callMdInsertGroup = function () {
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
    this.callModalEditGroup = function () {
        var btnEdit = $('.' + self.m_BTN_CLASS_EDIT_TABLE);
        btnEdit.click(function (e) {
            e.preventDefault();
            // reset cac bien
            m_ID = 0;
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

    // add/edit Group 
    this.modifyGroup = function () {
        var btnSave = $('#' + self.m_BTN_ID_SAVE);
        btnSave.click(function () {
            var form, group, url;
            form = self.getParentByTagName(this, 'form');
            // lay du lieu checkbox status
            var status = $('.radio-group > label > div.checked input').parents('label').attr('data');
            // gan cac doi tuong vao object
            group = {
                'ID': self.getById('ID').value.trim(),
                'GroupName': self.getById('GroupName').value.trim(),
                'GroupType': self.getById('GroupType').value.trim(),
                'SysHotelID': self.getById('SysHotelID').value.trim(),
                'Description': self.getById('Description').value.trim(),
                'Status': status
            }
            // thay doi url theo them/sua
            if (m_check_modify) {
                url = m_UrlList.UrlUpdate;
                group.ID = m_ID;
            } else {
                url = m_UrlList.UrlCreatre;
            }

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: group,
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;
                        var table = $('#GridGroup').dataTable();

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

                            self.addRowToDatatable(table, group);
                            self.notify(mess[0][0], mess[0][1]);
                        } else {
                            // chinh lai trang thai status
                            group.Status = group.Status === "Close" ? "Khóa" : "Hoạt động";
                            // gop data truoc khi update va sau khi update
                            var data = self.mergeObject(m_data, group);
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
                        self.callModalEditGroup();
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
        var texbox,dropdown, grRadio, divHide, arrOverLook = ['STT', 'Modify'];
        textbox = modal.find('.txt-content');
        dropdown = modal.find('.dll-content');
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
            // textbox
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
            for (var i = 0; i < dropdown.length; i++) {
                var child;
                child = dropdown[i];
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
            // dropdownlist
            for (var i = 0; i < dropdown.length; i++) {
                var child = dropdown[i];
                if (child.hasAttribute('data')) {
                    if (child.getAttribute('data') === key) {
                        var opts = child.options, flag = false;;
                        for (var j = 0; j < opts.length; j++) {
                            if (opts[j].innerHTML.toLocaleLowerCase() === value.toLocaleLowerCase()) {
                                child.options[j].selected = true;
                                flag = true;
                                break;
                            }
                        }
                        if (flag) {
                            break;
                        }
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
    this.callModalDeleteGroup = function () {
        var btnDelete = $('.delete-table');
        btnDelete.click(function (e) {
            e.preventDefault();
            var $modal, title, data, table, tr;
            tr = self.getParentByTagName(this, 'tr');
            table = self.getParentByTagName(tr, 'table');
            data = self.getDataRowTable($(table), $(tr));
            $modal = $('#mdDelete');
            // thay text titleton
            content = 'Bạn có muốn xóa nhóm ' + '<label class="name-bold">' + data.GroupName + '</label> không?';
            $modal.find('.modal-body-content > p').html(content);
            self.delteGroup(tr, data.ID);
        })
    }
    /*
        Xoa hotel
    */
    this.delteGroup = function (tr, id) {
        $('#btnDelete').click(function (e) {
            e.preventDefault();
            $.ajax({
                url: m_UrlList.UrlDelte,
                type: "Post",
                datatype: "json",
                data: { 'id': id },
                success: function (response) {
                    var mess, result;
                    mess = response.mess;
                    var table = $('#GridGroup').dataTable();
                    self.deleteRow(table, tr);
                    self.notify(mess[0], mess[1]);
                    // an modal
                    var md = $('#mdDelete');
                    md.modal('hide');
                    // reset lai event click button edit
                    self.callModalEditGroup();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    // thong bao loi k success
                    self.notify(thrownError, -1);
                }
            })
        })

    }
    // click phan trang can reset event button edit, del
    this.clickPagination = function () {
        $('.dataTables_paginate > .pagination > li > a').click(function (e) {
            e.preventDefault();
            self.callModalEditGroup();
            self.callModalDeleteGroup();
        })
        
    }
    this.init = function () { 
        self.buildStyleRadio(self.m_RADIO_BTN_CLASS_MINIAL_RED);
        this.renderGroupTable();
        //Insert
        this.callMdInsertGroup();
        //Update
        this.callModalEditGroup();
        this.modifyGroup();
        //Delete
        this.callModalDeleteGroup();
        this.clickPagination();
    }
}
/*Goi ham vao day*/
$(document).ready(function () {
    g_Group = new Group();
    g_Group.init();   
})