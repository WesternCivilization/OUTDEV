
jQuery.validator.setDefaults({
    ignore: '',
    errorElement: 'span',
    errorPlacement: function (error, element) {
        var parent = element.parent();
        if (parent.hasClass('input-group')) {
            parent.parent().append(error);
        } else {
            parent.append(error);
        }

    },
    highlight: function (element, error, valid) {
        $(element).css("border", "1px solid red");
        //$(element).focus();
    },
    unhighlight: function (element) {
        $(element).css("border", "1px solid #d2d6de");
    },
    success: function (label) { // khi hết lỗi thì làm gì
        label.closest('.form-group').removeClass('has-error');
        label.remove();
    },
});

//==================================================================
//	Description:  		chỉ validate số điện thoại 
//                      nhập vào 09 hoặc 01 và với 09 thì phải nhập 10 ký tự và 01 là 11 ký tự		
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("phone", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[0](9[0-9]{8}|1[0-9]{9}|4[0-9]{8})$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  Kiểm tra số điện thoại đơn giản
//                  chỉ bắt là số và 10-11 ký tự 
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("phonenumber", function (value, element) {
    if (value.length == 0)
        return true;
    var x = value.substring(0, 1);
    if (x == '+') {
        value = value.substring(1, value.length);

    }
    var reg = "^[0-9]{9,12}$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, gồm 9 hoặc 12 ký tự!");

jQuery.validator.addMethod("phonebase", function (value, element) {
    if (value.length == 0)
        return true;
    var x = value.substring(0, 1);
    if (x == '+') {
        value = value.substring(1, value.length);

    }
    var reg = "^[0-9]{1,20}$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, gồm 10 hoặc 11 ký tự!");


//==================================================================
//	Description:  Validate số					
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("number", function (value, element) {
    var reg = "^[0-9]+$";
    reg = new RegExp(reg);
    if (value.length == 0)
        return true;
    else if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Trường dữ liệu không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  validate email đúng định dạng					
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("email", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z][a-zA-Z0-9\.\_]+@[a-zA-Z][a-zA-Z0-9\.\_]+[a-zA-Z]{2,4}$";

    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Email không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  kiểm tra tên đăng nhập đúng định dạng		
//               không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, phải có ít nhất 1 ký tự chữ.
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("username", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z0-9]([a-zA-Z0-9\_]){4,24}$";
    reg = new RegExp(reg);
    var reg2 = "[a-zA-Z]";
    reg2 = new RegExp(reg2);
    if (reg.test(value) && reg2.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Tên đăng nhập không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  kiểm tra string UTF8 - Latin
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("utf8string", function (value, element) {
    if (value.length == 0)
        return true;
    var latin = value.latinize().replace(/\s/g, "");
    var regex = "^[a-zA-Z0-9]+$";
    var reg = new RegExp(regex);
    return reg.test(latin);
}, "String không đúng định dạng");

//==================================================================
//	Description:  		validate chứng minh thư nhân dân của người dùng 
//                      Chỉ bao gồm ký tự số và gồm 9 hoặc 10,12 ký tự		
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("idcard", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^([0-9]{9,10}|[0-9]{12})$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Chứng minh thư nhân dân không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  		validate với select option default = -1
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("selectrequired", function (value, element) {
    if (value == '-1' || value.length == 0) {
        return false;
    } else {
        return true;
    };
}, "Vui lòng chọn!");

//==================================================================
//	Description:  kiểm tra mã nhà cung cấp đúng định dạng		
//               không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, .
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("ProviderCode", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z0-9]([a-zA-Z0-9\_]){3,24}$";
    reg = new RegExp(reg);

    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Mã nhà cung cấp không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  kiểm tra Unit không được trùng nhau		
//  kiểm tra Unit không được trùng nhau		
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("notEqual", function (value, element, param) {

    return this.optional(element) || value != $(param).val();
}, "Không được trùng với đơn vị tính nhỏ nhất. vui lòng kiểm tra lại!");

//==================================================================
//	Description:  		
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("EqualListAnstring", function (value, element, param) {
    var chek = true;
    $(param).find("input").each(function () {
        if (value.toLowerCase() == $(this)[0].name.toLowerCase()) {
            chek = false;
            return false;
        }
        return chek;
    });

    return chek;
}, "Đơn vị tính cần thêm mới đã tồn tại trong danh sách vui lòng chọn. Vui lòng kiểm tra lại!");

//==================================================================
//	Description:  		 checkListBacode
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("checkListBacode", function (value, element, param) {
    var chek = true;
    if (value == "") {
        return true;
    }
        
    $(param).find("input").each(function () {
        var type = $(this)[0].name.substring(0, 1);
        if (type == "B" && element.name.trim() != $(this)[0].name) {
            var fireObject = $("input[name='" + $(this)[0].name + "']");
            if (fireObject.val() == value) {
                chek = false;
                return false;
            }

        }
        return chek;
    });

    return chek;
}, "BarCode trùng.Vui lòng kiểm tra lại!");

//==================================================================
//	Description:  		
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("ValueCode", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z0-9]([a-zA-Z0-9\-_.]){5,50}$";
    reg = new RegExp(reg);

    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Mã không  đúng định dạng. Vui lòng kiểm tra lại!");
//==================================================================
//	Description:  		
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("inputCodeEqual", function (value, element, param) {
    if (value == "" || value == null) {
        return true;
    }
    if (value == $(param).val()) {
       return false; 
    } else {
       return true;
    };
}, "Bacode không được trùng nhau. Vui lòng kiểm tra lại!");
//==================================================================
//	Description:  		
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("notnumber0", function (value, element) {
    if (value == 0)
        return false;
    return true;

}, "Giá trị không được nhép nhập giá trị bằng 0. Vui lòng kiểm tra lại!");

//==================================================================
//	Description:  	 số lớn nhất khi nhập số lượng	
//  
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("maxquantity", function (value, element) {

    if (value > 9999 || value == 0)
        return false;
    return true;

}, "Số lượng không được nhép nhập giá trị bằng 0 và lớn hơn 9.999. Vui lòng kiểm tra lại!");

//==================================================================
//	Description:  bỏ khoảng trắng	của câu chuyển vào
//              
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("namenotspace", function (value, element) {
    var _value = value.replace(/\s/g, '');

    return _value.length >= 6;


}, "Yêu cầu nhập thông tin ít nhất là 6 đến 250 ký tự !");

//==================================================================
//	Description:  kiểm tra mã All page		
//               không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, .
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("CodePage", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z0-9]([a-zA-Z0-9\_]){2,24}$";
    reg = new RegExp(reg);

    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Mã không đúng định dạng, vui lòng kiểm tra lại!");
//==================================================================
//	Description:  kiểm tra mã All page		
//       idinsystem        không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, .
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("idinsystem", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z0-9]([a-zA-Z0-9\_]){2,200}$";
    reg = new RegExp(reg);

    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Mã không đúng định dạng, vui lòng kiểm tra lại!");
//==================================================================
//	Description:  kiểm tra mã All page		
//               không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, .
//	Author: hungpv
//==================================================================
jQuery.validator.addMethod("Chosenselection", function (value, element) {
    console.log(typeof value);
    var checkValue = value && Object.keys(value).length > 1;
    //checkValue == obj ko loi
    if (checkValue || (Object.keys(value).length == 1 && value[0].length > 0)) {
        return true;
    } else {
        return false;
    }
}, "Bạn chưa hoàn thiện thông tin, vui lòng kiểm tra lại!");
//==================================================================
//	Description:  từ ngày lớn hơn tới ngày					
//	Author: LongLD
//==================================================================
jQuery.validator.addMethod("comparedate", function (value, element, param) {
    try {
        var f = param.format ? param.format : "DD/MM/YYYY";
        var d1 = moment(value, f),
        d2 = moment(param.element.val(), f);
        if (param.type == 1) {
            return d1.valueOf() <= d2.valueOf();
        } else if (param.type == 2) {
            return d1.valueOf() >= d2.valueOf();
        } else {
            return false;
        }
    } catch (e) {
        console.log(e);
        return false;
    }
}, "");