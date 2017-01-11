//js default theme
function Identity() {
    var self = this;
    this.check;
    OzeBase.apply(this, arguments);
    this.load = function () {
        $('input').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            increaseArea: '20%' // optional
        });
    }
    this.init = function () {
        this.load();
    }
}
//forgot password ~/Accounts/Index href text = "Tiếp theo"
//function fn_continue() {
//    var fakedUri = "/Accounts/ForgotPassword?user=xxxx";
//    var uri = fakedUri.replace("xxxx", $("#username").val());
//    window.location.href = uri;
//}


var g_Identity;
//var g_continue;
$(document).ready(function () {
    g_Identity = new Identity();
    g_Identity.init();

    //g_continue = new fn_continue();
    //g_continue.init();
})