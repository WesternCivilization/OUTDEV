/*NamLD 11/11/2016 04:58:06*/
var g_Territories;
function Territories() {
    var self = this;
    // check xem co phai la sua khong?
    // update = true
    var m_check_modify = false;
    // tr
    var m_tr_modify;
    // data row truoc khi update
    var m_data;
    // ke thua OzeBase
    OzeBase.apply(this, arguments);
    // tao bang
    this.renderTerritoriesTable = function () {
        var table = self.getById('tbTerritories');
        var object = {
            "paging": true,
            "lengthChange": false,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": true,
            // them button add, export
            "bCustomHeader": {
                //"add": true,
                "export": true
            },
            "columns": [
				{ "data": "STT", className: "all" },
                { "data": "ProvinceId", className: "all" },
				{ "data": "ProvinceName", className: "all" },
				{ "data": "DistrictId", className: "all" },
				{ "data": "DistrictName", className: "all" },
				{ "data": "WardsId", className: "all" },
				{ "data": "WardsName", className: "all" },
                //{ "data": "Modify", className: "all" }, //ko cho hiển thị edit (del-modify)
            ],
            
            // button chi tiet
            //responsive: {
            //    details: {
            //        display: $.fn.dataTable.Responsive.display.modal({
            //            header: function (row) {
            //                var data = row.data();
            //                return 'Chi tiết ' + data.WardsName;
            //            }
            //        }),
            //        renderer: function (api, rowIdx, columns) {
            //            var data = $.map(columns, function (col, i) {
            //                return col.columnIndex !== 0 && col.columnIndex !== 7 ?
            //                    '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
            //                        '<td>' + col.title + ':' + '</td> ' +
            //                        '<td>' + col.data + '</td>' +
            //                    '</tr>' :
            //                    '';
            //            }).join('');

            //            return data ?
            //                $('<table class="table table-striped table-bordered nowrap table-hover"/>').append(data) :
            //                false;
            //        }
            //    }
            //}
        }
        self.SetDatatable($(table), object);
    }
    // show form insert
    this.callMdInsertTerri = function () {
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
    // show form edit
    this.callModalEditTerri = function () {
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
            m_Code = m_data.WardsId;
            $modal = $('#mdModify');
            self.fillDataToModal(m_data, $modal);
            // thay text title, button
            self.changeModal($modal, false);
            m_check_modify = true;

        })
    }

    // add/edit  
    this.ModifyTerri = function () {
        var btnSave = $('#' + self.m_BTN_ID_SAVE);
        btnSave.click(function () {
            var form, terr, url;
            form = self.getParentByTagName(this, 'form');
            // lay du lieu checkbox status
            // var status = $('.radio-group > label > div.checked input').parents('label').attr('data');
            // gan cac doi tuong vao object
            terr = {
                'ProvinceId': self.getById('ProvinceId').value.trim(),
                'ProvinceName': self.getById('ProvinceName').value.trim(),
                'DistrictId': self.getById('DistrictId').value.trim(),
                'DistrictName': self.getById('DistrictName').value.trim(),
                'WardsId': self.getById('WardsId').value.trim(),
                'WardsName': self.getById('WardsName').value.trim()                
            }
            // thay doi url theo them/sua
            if (m_check_modify) {
                url = m_UrlList.UrlUpdate;
                //terr.WardsId = m_WardsCode;
                //terr.DistrictId = m_DistrictCode;
                //terr.ProvinceId = m_ProvinceCode;
            } else {
                url = m_UrlList.UrlCreate;
            }

            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: terr,
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;
                        var table = $('#tbTerritories').dataTable();

                        if (!m_check_modify && mess[1] == 1) {
                            //var obj = mess[2][0];
                            terr.STT = '';
                            //terr.ProvinceId = obj.ProvinceId;
                            //terr.ProvinceName = obj.ProvinceName;
                            //terr.DistrictId = obj.DistrictId;
                            //terr.DistrictName = obj.DistrictName;
                            //terr.WardsId = obj.WardsId;
                            //terr.WardsName = obj.WardsName;
                            terr.Modify = '<div class="edit-delete-table">' +
                                                    '<div class="edit-table">' +
                                                        '<i class="fa fa-pencil" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                    '<div class="delete-table">' +
                                                        '<i class="fa fa-times" aria-hidden="true"></i>' +
                                                    '</div>' +
                                                '</div>';

                            self.addRowToDatatable(table, terr);
                            self.notify(mess[0], mess[1]);
                        } else {
                            // chinh lai trang thai status
                            //hotel.Status = hotel.Status === "Close" ? "Khóa" : "Hoạt động";
                            // gop data truoc khi update va sau khi update
                            var data = self.mergeObject(m_data, terr);
                            self.updateRowToDatatable(table, data, m_tr_modify);
                            m_data = null;
                            m_tr_modify = false;
                            //m_WardsCode = '';
                            //m_DistrictCode = '';
                            //m_ProvinceCode = '';
                            self.notify(mess[0], mess[1]);
                        }
                        m_check_modify = false;
                        // an modal
                        var md = $('#mdModify');
                        md.modal('hide');
                        // lam rong cac textbox
                        self.resetForm(md[0]);
                        // reset lai event click button edit
                        self.callModalEditTerri();
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
        var form, groupHide;
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
    this.callModalDeleteTerri = function () {
        var btnDelete = $('.delete-table');
        btnDelete.click(function (e) {
            e.preventDefault();
            var $modal, title, data, table, tr;
            tr = self.getParentByTagName(this, 'tr');
            table = self.getParentByTagName(tr, 'table');
            data = self.getDataRowTable($(table), $(tr));
            $modal = $('#mdDelete');
            // thay text titleton
            content = 'Bạn có muốn xóa ' + '<label class="name-bold">' + data.WardsName + ' - ' + data.DistrictName + ' - ' + data.ProvinceName + '</label> ?';
            $modal.find('.modal-body-content > p').html(content);
            self.delteTerri(tr, data);
        })
    }
    /*
        Xoa 
    */
    this.delteTerri = function (tr, data) {
        $('#tbTerritories').click(function (e) {
            var terrdel = {
                'ProvinceId': data.ProvinceId.trim(),
                'ProvinceName': data.ProvinceName.trim(),
                'DistrictId': data.DistrictId.trim(),
                'DistrictName': data.DistrictName.trim(),
                'WardsId': data.WardsId.trim(),
                'WardsName': data.WardsName.trim(),
            }
            e.preventDefault();
            $.ajax({
                url: m_UrlList.UrlDelete,
                type: "Post",
                datatype: "json",
                data: terrdel,
                success: function (response) {
                    var mess, result;
                    mess = response.mess;
                    var table = $('#tbTerritories').dataTable();
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
        this.renderTerritoriesTable();
        //insert
        this.callMdInsertTerri();
        //this.insertHotel();
        //update
        this.callModalEditTerri();
        this.ModifyTerri();
        // delete
        this.callModalDeleteTerri();
    }
}
$(document).ready(function () {
    g_Territories = new Territories();
    g_Territories.init();
})