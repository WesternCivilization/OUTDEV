function Menu() {
    var self = this;
    this.check;
    OzeBase.apply(this, arguments);
    
    this.createTable = function () {
        var table = self.getById('tbMenu');
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": true,
            fixedHeader: true
        }
        self.SetDatatable($(table), object);
    }
    
    this.init = function () {
        this.createTable();
    }
}
var g_Menu;
$(document).ready(function () {
    g_Menu = new Menu();
    g_Menu.init();
})