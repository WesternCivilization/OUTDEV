/*ThichPV-24/10/2016 Design Group Table*/
function AccessRight() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.createTable = function () {
        $('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });
        var Table = self.getById('ListRule');
        var object = {
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": false,
            "addRow": [true, 'disabled'],

        }
        self.SetDatatable($(Table), object);

    }
    /**
        get gia tri aria-checked ar tu string html
    */
    this.getAttrFromString = function (strHTML) {
        var myregexp = /class="(.*?)"/;
        var match = myregexp.exec(strHTML);
        if (match != null && match.length > 1) {
            var temp = match[1].split(' ');
            var result = '';
            for (var i = 0; i < temp.length; i++) {
                if (temp[i] === "checked") {
                    result = "true";
                }
            }
            return result;
        } else {
            return "";
        }
    }
    this.formatDataTable = function (data) {
        var result = '', ruleID = '', Read = '', Write = '', Create = '', Delete = '';
        for (var i = 0; i < data.length; i++) {
            var child = data[i];
            ruleID += child[1] + '$';
            var re, wr, cr, del
            re = this.getAttrFromString(child[3]);
            wr = this.getAttrFromString(child[4]);
            cr = this.getAttrFromString(child[5]);
            del = this.getAttrFromString(child[6]);
            Read += re === "" ? 'false' + '$' : re + '$';
            Write += wr === "" ? 'false' + '$' : wr + '$';
            Create += cr === "" ? 'false' + '$' : cr + '$';
            Delete += del === "" ? 'false' + '$' : del + '$';
        }
        result +=  ruleID + ',';
        result +=  Read + ',';
        result += Write + ',';
        result += Create + ',';
        result +=  Delete ;
        return result;
    }
    this.create = function () {
        $("#btnSave").click(function () {
            var action = $('form')[0].action.split('/')
            var menuid = action[action.length - 1];
            var table = self.SetDatatable($('#ListRule'), { destroy: true });
            var data = table.rows().data();
            var strsObj = self.formatDataTable(data);
            if (menuid === undefined || menuid === null)
                return false;

            $.ajax({
                url: '/Group/Edit',
                type: "Post",
                datatype: "json",
                data: {
                    'id': menuid,
                    'strObject': strsObj
                },
                success: function (response) {
                    
                   
                },
                error: function (response) {
                    var x = response;
                }
            });

        });
    }
    
    this.init = function () {
        this.createTable();
        this.create();
    }
}

var g_AccessRight;

/*Goi ham vao day*/
$(document).ready(function () {
    g_AccessRight = new AccessRight();
    g_AccessRight.init();
    //
})