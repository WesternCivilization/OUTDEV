function MenuGroupCreate() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.create = function () {
        $("#btnCreate").click(function () {
            
            var menuid = $('select#SysMenuID option:selected').val();
            if (menuid === undefined || menuid === null)
                return false;

            $.ajax({
                url: '/Group/CreateGroupMenu',
                type: "Post",
                datatype: "json",
                data: {
                    'SysMenuID': menuid,
                    'SysGroupID': $("#GroupID").val()
                },
                success: function (response) {
                    var table = $('#ListGroupMenu');
                    var dataTb = self.SetDatatable(table, {
                        destroy: true,
                        columns: [
                            { 'data': 'stt' },
                            { 'data': 'Action' },
                            { 'data': 'SysGroupID' },
                            { 'data': 'Name' },
                            { 'data': 'Level' }
                        ]
                    })
                    if (response.data != null) {
                        dataTb.row.add({
                            "stt": response.Count,
                            "Action": '<a href="/Group/DeleteGroupMenu/' + response.data[0].ID + '" id="liDeleteGroupMenu">Delete</a>',
                            "SysGroupID": response.data[0].SysGroupID,
                            "Name": response.data[0].Name,
                            "Level": response.data[0].Level
                        }).draw();
                        //table.row.add().draw();
                    }

                    $("#messthongbao").empty();
                    $("#messthongbao").text(response.mess);
                    $('#modalthongbao').modal('show');
                },
                error: function (response) {
                    $("#messthongbao").empty();
                    $("#messthongbao").text("Error : Thêm menu vào nhóm thất bại!");
                    $("#modalloading").modal('close');
                    $('#modalthongbao').modal('show');
                }
            });

        });
    }
    
    this.init = function () {
        //this.addNew();
        self.buildStyleRadio(self.m_RADIO_BTN_CLASS_MINIAL_RED);
        this.create();
    }
    this.init();
}
function MenuGroupDelete()
{
    var self = this;
    this.Delete = function () {
        $("#liDeleteGroupMenu").click(function () {
            $.ajax({
                url: '/Group/DeleteGroupMenu',
                type: "Post",
                datatype: "json",
                data: {
                    'ID': ID,
                },
                success: function (response) {
                    $("#messthongbao").empty();
                    $("#messthongbao").text(response.mess);
                    $('#modalthongbao').modal('show');
                },
                error: function (response) {
                    $("#messthongbao").empty();
                    $("#messthongbao").text("Error : Xóa quyền menu cho nhóm thất bại!");
                    $("#modalloading").modal('close');
                    $('#modalthongbao').modal('show');
                }
            });
        });
    }

    this.Delete();
}


/*========================================================*/
var g_MenuGroupCreate;
var g_MenuGroupDelete;

$(document).ready(function () {
    g_MenuGroupCreate = new MenuGroupCreate();
    g_MenuGroupDelete = new MenuGroupDelete();
})