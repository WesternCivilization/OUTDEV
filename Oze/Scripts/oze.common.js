function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}
function alert(msg,callback) {
    bootbox.alert(msg, function () 
    {
        if ( callback) callback.call();
        //console.log('This was logged in the callback: ' + result);
    });
}
function confirm(msg, callback)
{
    bootbox.confirm(msg,
        function (result)
        {
            if (result && callback) callback.call();
            //console.log('This was logged in the callback: ' + result);
        });
}
var dialogLoading;
function showDialogLoading(msg) {
    if (!msg) msg = "Đang cập nhật dữ liệu...<img src='/images/load.gif' />";
    dialogLoading = bootbox.dialog({
        message: '<p class="text-center">' + msg + '</p>',
        closeButton: false
    });
    // do something in the background

}
function hideDialogLoading() {
    if (dialogLoading) dialogLoading.modal('hide');
}
function genMenu(id, suffixDialog)
{
    return
    '<div class="edit-delete-table">'+
        '<div class="edit-table" data-toggle="modal" data-backdrop="static"  onclick="javascript:edit'+jsFunction+'"('+id+')">'
            '<img src="/images/icon/icon-edit.png" style=" border: none;" alt="Chỉnh sửa">'+
        '</div>'+
        '<div class="delete-table" data-toggle="modal" data-backdrop="static" onclick="javascript:view' + jsFunction + '"(' + id + ')">' +
            '<img src="/images/icon/icon-view.png" style=" border: none;">' +
        '</div>'+
        '<div class="delete-table" data-toggle="modal" data-backdrop="static" onclick="javascript:view' + jsFunction + '"(' + id + ')">' +
                   '<img src="/images/icon/icon-view.png" style=" border: none;">' +
        '</div>'
    '</div>';
}
function toggleMenu(idmenu)
{
    $("#" + idmenu).addClass("active");
    $("#" + idmenu+" ul:first").addClass("menu-open");
    $("#" + idmenu+" ul:first").css("display", "block");
}
function loadRoomByLevelRoom(idType, idRoom, idRoomSelect)
{
    $.get("/Common/GetAllRoomByRoomTypeID?roomtypeid=" + idType, function (data)
    {
        $("#" + idRoom).html('');
        $("#" + idRoom).append('<option value="0">--Chọn phòng--</option>');
        if (data.result)
        {
            $.each(data.result, function (i, obj)
            {
                var sselect = '';
                if (obj.Id == idRoomSelect) sselect = 'selected';
                $("#" + idRoom).append('<option value="' + obj.Id + '" ' + sselect + '>' + obj.Name + '</option>');
            });                   
        }        
    });
}
var idTimeOut;
function startKeepSessionAlive()
{
    if (idTimeOut) clearInterval(idTimeOut);
    idTimeOut=setInterval("KeepSessionAlive()", 60000)
}

function KeepSessionAlive()
{
    var url = "/KeepSessionAlive.ashx?";
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", url, true);
    xmlHttp.send();
}