
function pageerror() {
    var self = this;
    this.check;
    OzeBase.apply(this, arguments);
    this.load = function () {
        $("#error").css({ "margin-left": "0px" });
    }
    this.init = function () {
        this.load();
    }
}
var g_pageerror;
$(document).ready(function () {
    g_pageerror = new pageerror();
    g_pageerror.init();
})