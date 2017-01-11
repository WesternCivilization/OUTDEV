function searchGrid() {
    //$table.bootstrapTable('refresh');
    $table.ajax.reload();
}
function getParams() {
    return { s: $("#txtSearch").val(), offset: $table.bootstrapTable('getOptions').pageNumber, limit: $table.bootstrapTable('getOptions').pageSize }
}
var $modalMail = $("#modalDetails");
var valueold = "";
//view cập nhật thông tin khách hàng

function AddUsingRoomDialog(checkinID) {

    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {
        $("#TittleBox").html("Thêm khách ở cùng");
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerArriveManage/AddUsingCustomers", { checkinID: checkinID }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            SetDate(".birthday");
            g_Utils.SetDateDefaultAdd();
            g_Utils.ConfigAutocomplete('#Name', "/CustomerArriveManage/SelectCustomer", "Name", "Name",
                function (item) {
                    var data = (JSON.parse(item.object));
                    //"" CitizenshipCode : 238 Company : "" CountryId : null CreateBy : 1 CreateDate : "/Date(1481792366727)/" CustomerTypeID : 0 DOB : "/Date(1480525200000)/" Email : "" Fax : "" HotelCode : "OzeHotel0001" Id : 73 IdentifyNumber : "125309881" Mobile : "01658756994" ModifyBy : null ModifyDate : null Name : "Ngọc Lam Man" Phone : "" ReservationID : null Sex : 1 SourceID : 0 Status : true SysHotelID : null TaxCode : "" TeamMergeSTT : null TeamSTT : null
                    console.log(data);
                    $("#modalDetails #formDetail :input").attr("disabled", true);
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
    $("#modalDetails button#btnUpdateDetail").css("display", "inline");
    $("#modalDetails button#btnUpdateDetail").css("disabled", "false");
    $("#modalDetails button#btnUndoRoom").css("disabled", "true");
    $("#modalDetails button#btnChangeRoom").css("disabled", "true");

    $("#modalDetails button#btnUndoRoom").css("display", "none");
    $("#modalDetails button#btnChangeRoom").css("display", "none");
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



}
function viewInfoDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function () {
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerArriveManage/GetInfomationRoomDetail", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("#modalDetails #formInforDetailCustomer :input").attr("disabled", true);
            $("#modalDetails #formInforDetailRoom :input").attr("disabled", true);
            SetDate(".birthday");
            $("#TittleBox").html("Thông tin phòng");
            $('#divDescription').prop('display', 'none');
        });
        $("#modalDetails button#btnUpdateDetail").css("disabled", "true");
        $("#modalDetails button#btnUndoRoom").css("disabled", "true");
        $("#modalDetails button#btnChangeRoom").css("disabled", "true");
        $("#modalDetails button#btnUpdateDetail").css("display", "none");
        $("#modalDetails button#btnUndoRoom").css("display", "none");
        $("#modalDetails button#btnChangeRoom").css("display", "none");
    });
    $("#modalDetails").modal("show");
}
function UndoRoomDialog(id) {
    window.location.href = "/RoomCheckOut/Index?CheckinID=" + id;
    //$("#modalDetails").off('show.bs.modal');
    //$("#modalDetails").on('show.bs.modal', function () {
    //    //RuleValidateSubmitToAdd();
    //    $("#modalDetails .modal-body-content").html('<p>loading..</p>');
    //    $.post("/CustomerArriveManage/GetInfomationRoomDetail", { id: id }, function (rs) {
    //        $("#modalDetails .modal-body-content").html(rs);
    //        $("#modalDetails #formInforDetailCustomer :input").attr("disabled", true);
    //        $("#modalDetails #formInforDetailRoom :input").attr("disabled", true);
    //        SetDate(".birthday");
    //        $("#TittleBox").html("Trả phòng");
    //    });
    //    $("#modalDetails button#btnUpdateDetail").css("disabled", "true");
    //    $("#modalDetails button#btnUndoRoom").css("disabled", "false");
    //    $("#modalDetails button#btnChangeRoom").css("disabled", "true");
    //    $("#modalDetails button#btnUpdateDetail").css("display", "none");
    //    $("#modalDetails button#btnUndoRoom").css("display", "inline");
    //    $("#modalDetails button#btnChangeRoom").css("display", "none");
    //});
    //$("#modalDetails").modal("show");
}
function ExChangeDialog(id) {
    $("#modalDetails").off('show.bs.modal');
    $("#modalDetails").on('show.bs.modal', function ()
    {
        //RuleValidateSubmitToAdd();
        $("#modalDetails .modal-body-content").html('<p>loading..</p>');
        $.post("/CustomerArriveManage/GetInfomationExChange", { id: id }, function (rs) {
            $("#modalDetails .modal-body-content").html(rs);
            $("#modalDetails #formInforDetailCustomer :input").attr("disabled", true);
            $("#modalDetails #formInforDetailRoom :input").attr("disabled", true);
            $("#TittleBox").html("Đổi phòng");
            $('#RoomNewID').prop('disabled', false);
            $('#Leave_Date').prop('disabled', false);
            $('#Description').prop('disabled', false);
            $('#divDescription').prop('display', 'block');
            $('#CheckRoomNewID').prop('display', 'block');
            SetDate(".birthday");
            g_Utils.SetDateDefaultAdd();
            $("#btnCheckKiemTraPhong").prop('disabled', false);
            $("#dllRoomTypeID").prop('disabled', false);
            $("#dllRoom").prop('disabled', false);

            
            $("#modalDetails").off('show.bs.modal');
        });
        $("#modalDetails button#btnUpdateDetail").css("disabled", "true");
        $("#modalDetails button#btnUndoRoom").css("disabled", "true");
        $("#modalDetails button#btnChangeRoom").css("disabled", "false");
        $("#modalDetails button#btnUpdateDetail").css("display", "none");
        $("#modalDetails button#btnUndoRoom").css("display", "none");
        $("#modalDetails button#btnChangeRoom").css("display", "inline");
     

    });
    $("#modalDetails").modal("show");
}
$(document).ready(function () {
    $modalMail.on('blur', 'input#Name', function (e) {
        if ($(this).val() !== valueold && valueold !== "") {
            var date = new Date();
            var _fn = date.getDate();
            var _ft = date.getMonth();
            var _fna = date.getFullYear();
            var ft = (_ft + 1);
            if (ft < 10)
                ft = "0" + ft;
            var fdate = (_fn + "/" + ft + "/" + _fna);
            $("#modalDetails #formDetail :input").attr("disabled", false);
            //$("#formDetail  :input[name='Name']").val('');
            $("#formDetail  :input[name='CustomerId']").val('0');
            $("#formDetail  :input[name='Company']").val('');
            $("#formDetail  :input[name='CountryId']").val('');
            $("#formDetail  :input[name='Sex']").val('');
            $("#formDetail  :input[name='Email']").val('');
            $("#formDetail  :input[name='IdentifyNumber']").val('');
            $("#formDetail  :input[name='TeamMergeSTT']").val(0);
            $("#formDetail  :input[name='TeamSTT']").val(0);
            $("#formDetail  :input[name='Phone']").val('');
            $("#formDetail  :input[name='DOB']").val(fdate);
            valueold = "";
        }

    });
    // Trả phòng
    $("#btnUndoRoom").click(function () {

        if (!$("#formInforDetailRoom").valid()) return;
        var pdata = getFormData($("#formInforDetailRoom"));

        showDialogLoading();
            $.post("/CustomerArriveManage/UndoRoom", { obj: pdata }, function (rs) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (rs.Status === "01") {
                    bootbox.alert(rs.Message, function () {
                        searchGrid();
                        $("#modalDetails").modal("hide");
                    });
                }
                else {
                    alert(rs.Message);
                }
            });
        
    });


    // su kien chon loai phong
    $("#RoomType").change(function () {
        showDialogLoading();
        $.post("/CustomerArriveManage/SelectRoomByType", { id: $(this).val() }, function (rs) {
            hideDialogLoading();
            $("#Room").empty();
            $("#Room").append("<option value='0'>--Chọn phòng--</option>");
            for (var i = 0; i < rs.length; i++) {
                $("#Room").append("<option value=" + rs[i].Id + ">" + rs[i].Name + "</option>");
            }
        });

    });
    // theemkhachs ở cùng
    $("#btnUpdateDetail").click(function () {

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
                        searchGrid();
                        $("#modalDetails").modal("hide");
                    });
                }
                else {
                    alert(rs.Message);
                }
            });
    });
    // Trả phòng
    $("#btnChangeRoom").click(function () {

        if (!$("#formInforDetailRoom").valid()) return;


        //var roomID = $("#formInforDetailRoom  :input[name='RoomNewID']").val();
        var roomID = $("#formInforDetailRoom  :input[name='dllRoom']").val();
        var CheckInID = $("#formInforDetailCustomer  :input[name='CheckInID']").val();
        var tdate = $("#formInforDetailRoom  :input[name='Leave_Date']").val();
        var Note = $("#formInforDetailRoom  :input[name='Description']").val();
    
      showDialogLoading();


            $.post("/CustomerArriveManage/ChangeRoom", { id: roomID, CheckInID: CheckInID,Note:Note, tdate: tdate }, function (rs) {
                hideDialogLoading();
                //closeDlgLoadingData();
                if (rs.Status === "01") {
                    bootbox.alert(rs.Message, function () {
                        searchGrid();
                        $("#modalDetails").modal("hide");
                    });
                }
                else {
                    alert(rs.Message);
                }
            });
    });
});