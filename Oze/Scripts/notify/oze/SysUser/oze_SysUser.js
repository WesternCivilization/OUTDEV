
var g_Group;
var g_RuleGroup;
var g_MenuGroup;
var idUserEdit;
var idUserDelete;
var erro = "callout callout-danger";
var suss = "callout callout-success";


function TableGroup() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.createTable = function () {
        var table = self.getById('GridGroup');
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": false,
            "bCustomHeader": {
                "add": true,
                "export": true
            }
        }
        self.SetDatatable($(table), object);

    }
    this.create = function () {
        $("#btnAddRow").click(function () {
            $("#btnCreateUser").css("display", "block");
            $("#btnSave").css("display", "none");
            $("#divCreateUser").css("display", "none");
            $("#showlistQuyen").css("display", "none");
            $("#divEditUser").css("display", "none");
            $("#txtusername").prop("disabled", false);
            $("#lblTitle").text('Thêm mới User');
            /*reset validate form*/
            var validator = $("#frmSysUser").validate();
            validator.resetForm();
        });
        $("#btnChangeEdit").click(function () {
            $("#btnSave").css("display", "block");
            $("#btnCreateUser").css("display", "none");
            $("#showlistQuyen").css("display", "block");
            $("#lblTitle").text('Chỉnh sửa User');
        });

        $("#btnSave").click(function () {
            $("#thongbao").empty();
            //var menuid = $('select#SysMenuID option:selected').val();
            //if (menuid === undefined || menuid === null)
            //    return false;

            var checkActive = 0;
            if ($('#cboactive').is(":checked")) {
                checkActive = 1;
            }
            
            if ($("#frmSysUser").valid()) {
                $.ajax({
                    url: '/SysUsers/Update_User',
                    type: "Post",
                    datatype: "json",
                    data: {
                        'id': idUserEdit,
                        'txtname': $("#txtname").val(),
                        'txtpass': $("#txtpass").val(),
                        'txtphone': $("#txtphone").val(),
                        'txtcmt': $("#txtcmt").val(),
                        'txtAddress': $("#txtAddress").val(),
                        'email': $("#txtEmail").val(),
                        'cboHotelname': $('#cboHotelname').find('option:selected').val(),
                        'cbxChonUser': $('#cbxChonUser').find('option:selected').val(),
                        'cboactive': checkActive
                    },
                    success: function (response) {
                        var mess = response["mess"];
                        var classdiv = erro;

                        if (response["rs"] == true) {
                            classdiv = suss;
                        }
                        //$("#thongbao").empty();
                        //$("#thongbao").css("height", "70px");
                        //$("#thongbao").append("<div class='box-body'><div class='" + classdiv + "'><strong>Thông báo  </strong>" + mess + "</div></div>");

                        $("#messthongbao").empty();
                        $("#messthongbao").text(response.mess);
                        $('#modalthongbao').modal('show');
                        $("#btnSave").css("display", "block");
                        setTimeout(function () {
                            window.location.reload(1);
                        }, 2000);
                    },
                    error: function (response) {
                        $("#messthongbao").empty();
                        $("#messthongbao").text("Error : Thêm menu vào nhóm thất bại!");
                        $("#modalloading").modal('close');
                        $('#modalthongbao').modal('show');
                    }
                });
            }

        });

    }

    this.addNew = function () {
        var btnAdd = self.getById(self.m_BTN_ID_ADD_ROW);
        btnAdd.addEventListener('click', function () {
            var $modal, title, btSave
            $modal = $('.' + self.m_MODAL_CLASS);
            // thay text
            $modal.modal(self.m_MODAL_SHOW);
            //title = $modal.find('.modal-header .modal-title')[0].innerHTML = self.m_BTN_TEXT_CREATE + ' ' + m_Title;
            //btSave = $modal.find('.' + self.m_BTN_CLASS_SAVE)[0].innerHTML = self.m_BTN_TEXT_CREATE;
        })
    }

    this.closeModal = function () {
        $('#mdModify').on('hidden.bs.modal', function (e) {
            $('#mdModify').find('input[type=text]').val('');
        })
    }
    
    this.init = function () {
        this.createTable();
        this.addNew();
        this.create();
        this.closeModal();
    }
}


function RuleGroup() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.createTable = function () {
        var table = self.getById('ListRule');
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": true
        }
        self.SetDatatable($(table), object);
    }
}
function MenuGroup() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.createTable = function () {
        var table = self.getById('ListGroupMenu');
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": true
        }
        self.SetDatatable($(table), object);
    }
}


//function validate() { $("#frmSysUser").validate(); }

function GetValueInforEdit(id) {
    idUserEdit = id;
    $("#btnSave").css("display", "block");
    $("#btnCreateUser").css("display", "none");
    $("#txtusername").prop("disabled", true);
    $("#lblTitle").text('Chỉnh sửa User');
    $("#showlistQuyen").css("display", "block");

    /*Trường hợp khi validate createUser mà mở edit sẽ vẫn còn câu thông báo sẽ dùng đoạn code này để chặn*/
    var validator = $("#frmSysUser").validate();
    validator.resetForm();

        $.ajax({
            url: '/SysUsers/GetDetailSysUser',
            type: "Post",
            datatype: "json",
            data: {
                'id': id
            },
            success: function (response) {
                $("#btnCreateUser").css("display", "none");
                $("#txtname").val(response["obj"].FullName);
                $("#txtusername").val(response["obj"].UserName);

                $("#txtphone").val(response["obj"].Mobile);
                $("#txtcmt").val(response["obj"].IdentityNumber);
                $("#txtEmail").val(response["obj"].Email);
                $("#txtAddress").val(response["obj"].Address);

                //khi selected dropbox ko được để khoảng trống nếu ko sẽ lỗi
                //$("#cboHotelname option:selected").val(response["obj"].SysHotelID);
                //$("#cbxChonUser option:selected").val(response["obj"].ParentID);
                //$('#cbxChonUser option:contains(' + response["obj"].ParentID + ')');
                $("#cboHotelname option:eq(" + response['obj'].SysHotelID + ")").prop("selected", true);
                $("#cbxChonUser option:eq(" + response['obj'].ParentID + ")").prop("selected", true);


                $("#divCreateUser").css("display", "block");
                $("#txtUserCreat").val(response["obj"].NameCreateby);
                var ngaytao = "";
                if (Date.parse(response["obj"].CreateDate)) {
                    ngaytao = response["obj"].CreateDate;
                }
                $('#cboactive').prop('checked', false);
               
                if (response["obj"].IsActive == "1") {
                    $('#cboactive').prop('checked', true);
                }

                var currentTimeCreate = new Date(parseInt(response["obj"].CreateDate.substr(6)));
                var datecreate = currentTimeCreate.toLocaleDateString() + "  " + currentTimeCreate.getHours() + "h:" + currentTimeCreate.getMinutes();
                $("#txtCreatDate").val(datecreate);

                $("#divEditUser").css("display", "block");
                $("#txtUserEdit").val(response.NameModifyby);

                var currentTimeEdit = new Date(parseInt(response["obj"].ModifyDate.substr(6)));
                var dateEdit = currentTimeEdit.toLocaleDateString() + "  " + currentTimeEdit.getHours() + "h:" + currentTimeEdit.getMinutes();
                $("#txtEditDate").val(dateEdit);

                var dataGroup = response["listGroupUser"];

                $("#listGroup").empty();
                if (dataGroup.length == 0) {
                    $("#listGroup").append("<b>User này chưa được gán quyền</b></div>");
                } else {
                    $.each(dataGroup, function (i, item) {
                        $("#listGroup").append("<div class='col-sm-4'><input name='cboactive' type='checkbox' checked='checked' disabled /><b>" + dataGroup[i] + "</b></div>");
                    });
                }
                //$("#messthongbao").empty();
                //$("#messthongbao").text(response.mess);
                //$('#modalthongbao').modal('show');
                $('#mdModify').modal({ backdrop: 'static', keyboard: false });
                $("#mdModify").modal('show');

            },
            error: function (response) {
                $("#messthongbao").empty();
                $("#messthongbao").text("Error : Thêm menu vào nhóm thất bại!");
                $('#modalthongbao').modal('show');
            }
        });
    
}
/*TrungNd-7/11/2016*/
function GetValueInforDelete(id) {
    idUserDelete = id;
    $('#messthongbao').modal({ backdrop: 'static', keyboard: false })
    $("#messthongbao").empty();
    $("#messthongbao").text("Bạn chắc chắn muốn xóa ?");
    $('#modalthongbao').modal('show');
    $("#footerShowModal").append("<button type='button' data-dismiss='modal' class='btn btn-primary' id='btnOkXoa'>Đồng ý</button>");
    
    $("#btnOkXoa").click(function () {
        $("#btnOkXoa").remove();
        $(".modal-backdrop").remove();
        $.ajax({
            url: '/SysUsers/Delete_User',
            type: "Post",
            datatype: "json",
            data: {
                'id': id
            },
            success: function (response) {
                $("#btnOkXoa").remove();
                $("#messthongbao").empty();
                $("#messthongbao").text(response.mess);
                setTimeout(function () {
                    window.location.reload(1);
                }, 2000);
            },
            error: function (response) {
                $("#messthongbao").empty();
                $("#messthongbao").text("Error : Có lỗi từ phía hệ thống");
                $('#modalthongbao').modal('show');
            }
        });

    });
    $("#btnclose").click(function () {
        $(".modal-backdrop").remove();
        $("#btnOkXoa").remove();
    });
}

/*TrungNd-31/10/2016*/
$(document).ready(function () {
    $.validator.addMethod('check_chonks', function (value, element) {
        var chonKs = $("#cboHotelname").find('option:selected').val()
        if (chonKs == 0) {
            return false;
        } else {
            return true;
        }
    });

    $.validator.addMethod('check_chonUser', function (value, element) {
        var chonUser = $("#cbxChonUser").find('option:selected').val();
        if (chonUser == 0) {
            return false;
        }
        else {
            return true;
        }
    });

    $("#frmSysUser").validate({
        rules: {
            txtname: {
                required: true
            },
            txtusername: {
                required: true
            },
            txtphone: {
                number: true,
                minlength: 10
            },
            cbxChonUser: {
                check_chonUser: true
            },
            SysHotelID: {
                check_chonks: true
            },
            txtcmt: {
                required: true
            },
            txtEmail: {
                required: true,
                email: true
            }
        },
        messages: {
            txtname: {
                required: "Vui lòng nhập họ tên"
            },
            txtusername: {
                required: "Vui lòng nhập tên đăng nhập"
            },
            txtphone: {
                number: "Vui lòng nhập đúng số điện thoại",
                minlength: "Số điện thoại phải lớn hơn 10 số"
            },
            cbxChonUser: {
                check_chonUser: "Vui lòng chọn user"
            },
            SysHotelID: {
                check_chonks: "Vui lòng chọn khách sạn"
            },
            txtcmt: {
                required: "vui lòng nhập số chứng mình thư hộ chiếu"
            },
            txtEmail: {
                required: "Vui lòng chọn nhập email",
                email: "Chưa nhập đúng định dạng email"
            }
        }
    });
    /*Thêm mới user*/
    $('#btnCreateUser').on('click', function () {
        if ($("#frmSysUser").valid()) {
            $("#txtusername").prop("disabled", false);
            $("#showlistQuyen").css("display", "none");
            $("#thongbao").empty();
            var checkActive = 0;
            if ($('#cboactive').is(":checked")) {
                checkActive = 1;
            }
            $('#mdModify').modal({ backdrop: 'static', keyboard: false });

            $.ajax({
                url: '/SysUsers/CreateUser',
                type: "Post",
                datatype: "json",
                data: {
                    'txtname': $("#txtname").val(),
                    'txtusername': $("#txtusername").val(),
                    'txtpass': $("#txtpass").val(),
                    'txtphone': $("#txtphone").val(),
                    'txtcmt': $("#txtcmt").val(),
                    'txtAddress': $("#txtAddress").val(),
                    'email': $("#txtEmail").val(),
                    'cboHotelname': $('#cboHotelname').find('option:selected').val(),
                    'cbxChonUser': $('#cbxChonUser').find('option:selected').val(),
                    'cboactive': checkActive
                },
                success: function (response) {
                    var mess = response["mess"];
                    var classdiv = erro;

                    if (response["rs"] == true) {
                        classdiv = suss;
                    }
                    $("#messthongbao").empty();
                    $("#messthongbao").text(response.mess);
                    $('#modalthongbao').modal('show');
                    //$("#btnSave").css("display", "block");
                    setTimeout(function () {
                        window.location.reload(1);
                    }, 2000);

                },
                error: function (response) {
                    $("#messthongbao").empty();
                    $("#messthongbao").text("Error : Thêm menu vào nhóm thất bại!");
                    $('#modalthongbao').modal('show');
                }
            });
        }
    });

    g_Group = new TableGroup();
    g_Group.init();
    //
    g_RuleGroup = new RuleGroup();
    g_RuleGroup.createTable();
    //
    g_MenuGroup = new MenuGroup();
    g_MenuGroup.createTable();
    //
    
    //this.GetValueInforEdit = function GetValueInforEdit(id) {
    $("#btnShowGroupQuyen").click(function () {
        if ($('#listGroup').css('display') == 'none') {
            $('#listGroup').css('display', 'block') //to show
        } else {
            $('#listGroup').css('display', 'none') //to hide
        }
    });
    
});
