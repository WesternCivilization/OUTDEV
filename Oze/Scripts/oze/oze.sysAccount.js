function SysAccount() {
    var self = this;
    Base.apply(this, arguments);
    this.SetTable = function () {
        var element = $("#GridGroup");
        var obj = {
            "paging": true,
            "lengthChange": false,
            "searching": false,
            "ordering": true,
            "info": true,
            "autoWidth": false
        }
        self.SetDatatable(element, obj);   
    }
    this.init = function(){
        this.SetTable();
    }
}

var g_system;

$(document).ready(function () {
    g_system = new SysAccount();
    g_system.init();
});