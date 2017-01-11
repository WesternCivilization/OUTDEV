var g_DetailBooking;
function DetailBooking() {
    var self = this;
    OzeBase.apply(this, arguments);
    
    this.init = function () {
    }
}
$(document).ready(function () {
    g_DetailBooking = new DetailBooking();
    g_DetailBooking.init();
})