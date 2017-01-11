var g_UserDetail;
function UserDetail() {
    var self = this;
    // update = true, insert = false
    var m_check_modify = true;
    // Inheritance OzeBase.js
    OzeBase.apply(this, arguments);

    // Update Info
    this.Modify = function () {
        var btnSave = $('#' + self.m_BTN_ID_SAVE);
        btnSave.click(function () {
            var form, obj, url;
            form = self.getParentByTagName(this, 'form');
            // gan cac doi tuong vao object
            obj = {
                'Mobile': self.getById('txtphone').value.trim(),
                'Email': self.getById('txtEmail').value.trim(),
                'Address': self.getById('txtAddress').value.trim()
            }            
            // Get url 
            if (m_check_modify) {
                url = m_UrlList.UrlUpdate;
            } else {
                url = m_UrlList.UrlCreate;
            }
            // Pass to Controller
            if ($(form).valid()) {
                $.ajax({
                    url: url,
                    type: "Post",
                    datatype: "json",
                    data: obj,
                    success: function (response) {
                        var mess, result;
                        mess = response.mess;
                        // success message
                        self.notify(mess[0], mess[1]);

                        m_check_modify = true;
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // error message
                        self.notify(thrownError, -1);
                    }
                })
            }
        })
    }

    this.init = function () {
        //update
        this.Modify();
    }
}

$(document).ready(function () {
    g_UserDetail = new UserDetail();
    g_UserDetail.init();
})