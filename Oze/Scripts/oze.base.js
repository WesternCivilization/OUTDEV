/* Cac ham co ban dung chung, nhieu lan khai bao o day
NamLD 10/01/2016 09:09:12
function Base la function cha, cac cho khac muon ke thua lai thi dung:
  - Base.apply(this, arguments) // voi arguments la bien so truyen vao
  chang han co file js abc.js chua cac phan:

  function Abc(bien 1, bien 2, bien 3){
    Basic.apply(this, arguments)
  }
  var m_Abc
  $(document).ready(function(){
     m_Abc = new Abc(bien 1, bien 2, bien 3); // khi do cac bien nay tu dong theo arguments vao, khi truyen bien thi ben Base cung phai co cac bien
     // neu k co bien truyen thi mac dinh bo di: function Abc(){};
  })
*/
function OzeBase() {
    
    /********************************************/
    var self = this;
    this.m_Action;
    this.m_MODAL_CLASS = 'modal-modify';
    this.m_MODAL_SHOW = "show";
    this.m_BTN_TEXT_CREATE = "Thêm";
    this.m_BTN_TEXT_EDIT = "Sửa";
    this.m_BTN_TEXT_DELETE = "Xóa";
    this.m_BTN_ID_ADD_ROW = "btnAddRow";
    this.m_BTN_ID_SAVE = "btnSave";
    this.m_BTN_ID_CANCEL = "btnCancel";

    this.m_BTN_CLASS_SAVE = "btn-save";
    this.m_BTN_CLASS_ADD = "btn-add";
    this.m_BTN_CLASS_EDIT = "btn-edit";
    this.m_CLASS_SUCCESS = 'success';
    this.m_CLASS_FAIL = 'error';
    this.m_DIV_CLASS_NOTIFY = 'notify-popup';

    this.m_BTN_CLASS_EDIT_TABLE = 'edit-table';
    this.m_BTN_CLASS_DELETE_TABLE = 'delete-table';

    this.m_RADIO_BTN_CLASS_MINIAL = 'minimal';
    this.m_RADIO_BTN_CLASS_MINIAL_RED = 'minimal-red';
    this.m_RADIO_BTN_CLASS_FLAT_RED = 'flat-red';
    this.m_FORMAT_DATE = 'DD/MM/YYYY';
    this.m_FORMAT_DATE_TIME = 'DD/MM/YYYY HH:mm';
    
    /********************************************/
    /**
        tim doi tuong boi id, co the truyen element(parent) hoac khong
        id: id cua doi tuong can tim,
        el: doi tuong parent cua doi tuong can tim
    */
    this.getById = function (id, el) {
        var result;
        if (el) {
            result = el.getElementById(id);
        } else {
            result = document.getElementById(id);
        }
        return result;
    }

    /**
        tim doi tuong boi class, co the truyen element(parent) hoac khong
        cl: cl cua doi tuong can tim,
        el: doi tuong parent cua doi tuong can tim
    */
    this.getByCl = function (cl, el) {
        var result;
        if (el) {
            result = el.getElementsByClassName(cl);
        } else {
            result = document.getElementsByClassName(cl);
        }
        return result;
    }

    /**
        tim doi tuong boi tagName, co the truyen element(parent) hoac khong
        tag: tagName cua doi tuong can tim,
        el: doi tuong parent cua doi tuong can tim
    */
    this.getByTag = function (tag, el) {
        var result;
        if (el) {
            result = el.getElementsByTagName(tag);
        } else {
            result = document.getElementsByTagName(tag);
        }
        return result;
    }
    /**
        tim attribute
        attr: ten attr cua doi tuong,
        el: doi tuong can tim attr
    */
    this.getAttr = function (attr, el) {
        var result;
        // neu k ton tai thi tao
        if (!el.hasAttribute(attr)) {
            el.createAttribute(attr);
        }
        result = el.getAttribute(attr);
        return result;
    }
    /**
        tim parent khi biet child va class cua parent
        child: doi tuong con
        parentClass: class doi tuong cha can tim
    */
    this.getParentByCl = function (child, parentClass) {
        var parent = child.parentElement;
        while (!parent.classList.contains(parentClass)) {
            parent = parent.parentElement;
            if (parent === null) {
                parent = false;
            }
        }
        return parent;
    }
    /**
       tim parent khi biet child va tag name cua parent
       child: doi tuong con
       parentTagName: TagName doi tuong cha can tim
   */
    this.getParentByTagName = function (child, parentTagName) {
        var parent = child.parentNode;
        var name = parent.tagName;
        while (name.toLocaleLowerCase() !== parentTagName.toLocaleLowerCase()) {
            parent = parent.parentNode;
            name = parent.tagName;
            if (parent === null) {
                parent = false;
            }
        }
        return parent;
    }

    /**
       tim parent khi biet child va id cua parent
       child: doi tuong con
       parentId: id doi tuong cha can tim
   */
    this.getParentById = function (child, parentId) {
        var parent = child.parentElement;
        while (parent.id !== parentId) {
            parent = parent.parentElement;
            if (parent === null) {
                parent = false;
            }
        }
        return parent;
    }

    /**
        tim style: width, height,... cua doi tuong
        el: doi tuong can get style
        styleProp: style can get
    */
    this.getStyle = function (el, styleProp) {
        var value, defaultView = (el.ownerDocument || document).defaultView;
        // W3C standard way:
        if (defaultView && defaultView.getComputedStyle) {
            // sanitize property name to css notation
            // (hypen separated words eg. font-Size)
            styleProp = styleProp.replace(/([A-Z])/g, "-$1").toLowerCase();
            return defaultView.getComputedStyle(el, null).getPropertyValue(styleProp);
        } else if (el.currentStyle) { // IE
            // sanitize property name to camelCase
            styleProp = styleProp.replace(/\-(\w)/g, function (str, letter) {
                return letter.toUpperCase();
            });
            value = el.currentStyle[styleProp];
            // convert other units to pixels on IE
            return value;
        }
    }

    /** 
        format so, co the truyen so chu so sau dau phay length
        number: so can format
        length: doi dai sau dau phay. Default la 0
    */
    this.formatNumber = function (number, length) {
        var x, x1, x2, rgx, type;
        if (number === '') {
            number = '0';
        }

        if (!isNaN(number)) {
            number = parseFloat(number);
            length = typeof length === 'undefined' ? 0 : length;
        }

        number = number.toFixed(length) + '';
        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';

        rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }

    /**
        add class vao mot doi tuong, co the truyen mot chuoi cac class: "class1, class2/* 
        element: doi tuong can add class
        _class: class can add
    */
    this.addClass = function (element, _class) {
        if (_class.indexOf(',') > -1) {
            var split = _class.split(',');
            for (var i = 0; i < split.length; i++) {
                var vClass = split[i];
                if (vClass.trim() === '') {
                    continue;
                }
                if (!element.classList.contains(vClass)) {
                    element.classList.add(vClass);
                }
            }
        } else {
            if (!element.classList.contains(_class)) {
                element.classList.add(_class);
            }
        }
    }

    /**
        remove class vao mot doi tuong, co the truyen mot chuoi cac class: "class1, class2"
        element: doi tuong can remove class
        _class: class can remove
    */
    this.removeClass = function (element, _class) {
        if (_class.indexOf(',') > -1) {
            var split = _class.split(',');
            for (var i = 0; i < split.length; i++) {
                var vClass = split[i];
                if (vClass.trim() === '') {
                    continue;
                }
                if (element.classList.contains(vClass)) {
                    element.classList.remove(vClass);
                }
            }
        } else {
            if (element.classList.contains(_class)) {
                element.classList.remove(_class);
            }
        }
    }
    /**
        2016-10-14 15:40:47 NamLD fire event auto click button, link, ... khi ma k co click bao doi tuong do
        element: doi tuong can fire event
        eventName: ten event
    */
    this.simulate = function (element, eventName) {
        var eventMatchers = {
            'HTMLEvents': /^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,
            'MouseEvents': /^(?:click|dblclick|mouse(?:down|up|over|move|out))$/
        }
        var defaultOptions = {
            pointerX: 0,
            pointerY: 0,
            button: 0,
            ctrlKey: false,
            altKey: false,
            shiftKey: false,
            metaKey: false,
            bubbles: true,
            cancelable: true
        }
        function extend(destination, source) {
            for (var property in source)
                destination[property] = source[property];
            return destination;
        }

        var options = extend(defaultOptions, arguments[2] || {});
        var oEvent, eventType = null;

        for (var name in eventMatchers) {
            if (eventMatchers[name].test(eventName)) { eventType = name; break; }
        }

        if (!eventType)
            throw new SyntaxError('Only HTMLEvents and MouseEvents interfaces are supported');

        if (document.createEvent) {
            oEvent = document.createEvent(eventType);
            if (eventType == 'HTMLEvents') {
                oEvent.initEvent(eventName, options.bubbles, options.cancelable);
            }
            else {
                oEvent.initMouseEvent(eventName, options.bubbles, options.cancelable, document.defaultView,
                options.button, options.pointerX, options.pointerY, options.pointerX, options.pointerY,
                options.ctrlKey, options.altKey, options.shiftKey, options.metaKey, options.button, element);
            }
            element.dispatchEvent(oEvent);
            oEvent.stopImmediatePropagation();// chan truong hop loi maximum call stack size exceeded
        }
        else {
            options.clientX = options.pointerX;
            options.clientY = options.pointerY;
            var evt = document.createEventObject();
            oEvent = extend(evt, options);
            element.fireEvent('on' + eventName, oEvent);
        }
        return element;
    }

    /*element dạng jquery, object là json
       object ={
                "paging": true,
                "lengthChange": false,
                "searching": false,
                "ordering": true,
                "info": true,
                "autoWidth": false
            }
    */
    this.SetDatatable = function (element, object) {
        var table;
        if (object) {
            object.language = {
                "lengthMenu": "Hiển thị _MENU_ dòng/1 trang",
                "zeroRecords": "Không tìm thấy",
                "info": "_START_ - _END_ / _TOTAL_ dòng",
                "infoEmpty": "0 - 0 / 0 dòng",
                "search": "",
                "searchPlaceholder": "Nhập từ khóa"
            }
            table = element.DataTable(object);
        }
        else
        {
            var obj = {"language" : {
                "lengthMenu": "Hiển thị _MENU_ dòng/1 trang",
                "zeroRecords": "Không tìm thấy",
                "info": "_START_ - _END_ / _TOTAL_ dòng",
                "infoEmpty": "0 - 0 / 0 dòng",
                "search": "",
                "searchPlaceholder": "Nhập từ khóa"
            }}
            table = element.DataTable(obj);
        }
        //search 
        table.on('order.dt search.dt', function () {
            table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
        return table;
    }
    /* table: doi tuong table da su dung lib Datatable
    rows là array, co 2 truong hop:
     +1) table da duoc add data cho columns
     columns:[ { 'data': 'data1' },
                { 'data': 'data2' },
                { 'data': 'data3' },
                { 'data': 'data4' }
             ]
      rows =[{
                "data1": '1',
                "data2": '2',
                "data3": '3',
                "data4": '4'
            },
            {
                "data1": '1',
                "data2": '2',
                "data3": '3',
                "data4": '4'
            }]
       +2) table chua duoc add data:
           _) ta co the add data bang cach:
           table.DataTable({
            destroy: true,
            columns:[
                { 'data': 'data1' },
                { 'data': 'data2' },
                { 'data': 'data3' },
                { 'data': 'data4' }
             ]
          });
          _) hoac add:
          rows=[['1', '2', '3', '4'],['1', '2', '3', '4']]
          tr la node tr can update
   */
    this.updateRowToDatatable = function (table, rows, tr) {
        table.fnUpdate(rows, tr);
    }
   
    /* table: doi tuong table da su dung lib Datatable
    rows là array, co 2 truong hop:
     +1) table da duoc add data cho columns
     columns:[ { 'data': 'data1' },
                { 'data': 'data2' },
                { 'data': 'data3' },
                { 'data': 'data4' }
             ]
      rows =[{
                "data1": '1',
                "data2": '2',
                "data3": '3',
                "data4": '4'
            },
            {
                "data1": '1',
                "data2": '2',
                "data3": '3',
                "data4": '4'
            }]
       +2) table chua duoc add data:
           _) ta co the add data bang cach:
           table.DataTable({
            destroy: true,
            columns:[
                { 'data': 'data1' },
                { 'data': 'data2' },
                { 'data': 'data3' },
                { 'data': 'data4' }
             ]
          });
          _) hoac add:
          rows=[['1', '2', '3', '4'],['1', '2', '3', '4']]
   */

    this.addRowToDatatable = function (table, rows) {
        table.fnAddData(rows);
    }
    /* table: doi tuong table da su dung lib Datatable
    row: doi tuong can xoa la doi tuong dang DOM
   */
    this.deleteRow = function(table, row){
        table.fnDeleteRow(row);
    }
    /*
    NamLD 11/16/2016 08:06:35
    get data row tu table
    // element la button edit, delete
    element la dang jQuery
    return object
    */
    this.getDataRowTable = function (table, tr) {
        var lastTable, data;
        lastTable = table.DataTable();
        data = lastTable.row(tr).data();
        return data;
    }
    /**
        2016-11-15 11:25:20 NamLD notify 
        element: đối tượng truyền thông báo
        mess: thông báo
        type: 1 == success, -1 == error
    */
    Number.prototype.padLeft = function (base, chr) {
        var len = (String(base || 10).length - String(this).length) + 1;
        return len > 0 ? new Array(len).join(chr || '0') + this : this;
    }
    this.notify = function (mess, type, autoHidden, element) {
        var vClass, el, clickHide, autoHide;
        autoHide = !autoHidden ? autoHidden : true;
        clickHide = autoHidden ? false : true;
        el = element ? element : $('.' + this.m_DIV_CLASS_NOTIFY);
        vClass = type === -1 ? this.m_CLASS_FAIL : this.m_CLASS_SUCCESS;
        el.notify(mess, {
            clickToHide: clickHide,
            // whether to auto-hide the notification
            autoHide: autoHide,
            // if autoHide, hide after milliseconds
            autoHideDelay: 5000,
            style: 'bootstrap',
            // default class (string or [string])
            className: vClass,
            // show animation
            showAnimation: 'slideDown',
            // show animation duration
            showDuration: 400,
            // show the arrow pointing at the element
            arrowShow: false,
            elementPosition: "bottom right",
            globalPosition: 'top right',
            // hide animation
            hideAnimation: 'slideUp',
            // hide animation duration
            hideDuration: 200,
        })
    }
    /**
    NamLD 11/15/2016 11:29:59
    format datetime to dd/MM/yyyy hh:mm:ss
    */
    this.formatDatetime = function (strDateTime) {
        if (strDateTime === "" || strDateTime === null) {
            return "";
        }
        var d = new Date(strDateTime),
        dformat = [(d.getMonth() + 1).padLeft(),
               d.getDate().padLeft(),
               d.getFullYear()].join('/') + ' ' +
              [d.getHours().padLeft(),
               d.getMinutes().padLeft(),
               d.getSeconds().padLeft()].join(':');
        return dfomat;
    }
    /**
    2016-11-16 15:05:29 NamLD
    tao slyle cho radiobutton
    vClass: ten class radiobutton
     + minimal
     + minimal-red
     + flat-red
    */
    this.buildStyleRadio = function () {
        //iCheck for checkbox and radio inputs
        $('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });
        //Red color scheme for iCheck
        $('input[type="checkbox"].minimal-red, input[type="radio"].minimal-red').iCheck({
            checkboxClass: 'icheckbox_minimal-red',
            radioClass: 'iradio_minimal-red'
        });
        //Flat red color scheme for iCheck
        $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
        });
    }
    /**
        2016-11-22 16:51:42 NamLD
        su dung cho merge object khi update table
        objOld: object truoc khi merge
        objNew: object khi update thay doi
    */
    this.mergeObject = function (objOld, objNew) {
        for (var keyOld in objOld) {
            
            for (var keyNew in objNew) {
                if (keyNew === keyOld) {
                    objOld[keyOld] = objNew[keyNew];
                    continue;
                }
            }
        }
        return objOld;
    }
    /**
    reset validate
    */
    this.resetValid = function () {
        $('.field-validation-error')
           .removeClass('field-validation-error')
           .addClass('field-validation-valid');
        $('.field-validation-valid span').remove();
        $('.input-validation-error')
            .removeClass('input-validation-error')
            .addClass('valid');
    }

    /**
        2016-11-28 16:13:35 NamLD
        set cookies; co the dung cho them/xoa cookie
        name: ten cookie
        value: gia tri
        time: thoi gian song (mac dinh 30 ngay)_ type: number, unit: minisecond
        name = value; name1 = value1;...
    */
    this.setCookies = function (name, value, time) {
        // Phai co expire de luu thanh persistent cookie (loai close trinh duyet k bi mat coookie giong nhu session cookie)
        var timeDefault, timeout, expDate;
        timeDefault = 30 * 24 * 60 * 60 * 1000;
        timeout = time ? time : timeDefault;
        expDate = new Date();
        expDate.setTime(expDate.getTime() + timeout); // one month ahead
        document.cookie = name + '=' + value + ';expires=' + expDate.toGMTString() + 'path=/';
    }

    /**
        2016-11-28 16:13:35 NamLD 
        get cookies; get value cookie
        name: ten cookie
    */
    this.getCookies = function (name) {
        var value = '', cookies, arrCookie;
        cookies = document.cookie;
        if (cookies.indexOf(';') === -1) {
            return value;
        }
        arrCookie = cookies.split(';');
        for (var i = 0; i < arrCookie.length; i++) {
            var cookie = arrCookie[i];
            if (cookie.indexOf('=') === -1) { // kiem tra neu k chua dau = thi next for
                continue;
            }
            var strs = cookie.split('=');
            if (strs[0].trim().toLocaleUpperCase() === name.toLocaleUpperCase() && strs[1] != '') { // neu chua cookie menu va co du lieu
                value = strs[1];
            }
        }
        return value;
    }
    /**
        NamLD 12/10/2016 02:40:40
        tao datetimepicker from to
        $fromDate,$toDate: cac doi tuong dang JQuery
        formatDate: dinh dang format datetime
        dateFrom: thoi gian bat dau. Neu k co thi mac dinh khoang cach la 1 thang
    */
    this.setDateRangePicker = function ($fromDate, $toDate, formatDate, dateFrom) {
        var defaultFrom, defaultTo;
        defaultTo = new Date();
        if (dateFrom) {
            defaultFrom = dateFrom;
        } else {
            defaultFrom = new Date(defaultTo);
            //defaultFrom.setDate(defaultTo.getDate() - 1);
            defaultFrom.setMonth(defaultTo.getMonth() - 1);
        }
        // khoi tao 2 date picker
        $fromDate.datetimepicker({
            format: formatDate,
            defaultDate: defaultFrom
        });
        $toDate.datetimepicker({
            format: formatDate,
            useCurrent: false, //Important! See issue #1075
            defaultDate: defaultTo
        });
        // lien ket 2 picker
        $fromDate.on("dp.change", function (e) {
            $toDate.data("DateTimePicker").minDate(e.date);
        });
        $toDate.on("dp.change", function (e) {
            $fromDate.data("DateTimePicker").maxDate(e.date);
        });
    }
    /**
        NamLD 12/10/2016 02:40:40
        tao datetimepicker from to
        $date: cac doi tuong dang JQuery
        formatDate: dinh dang format datetime
        defaultDate: ngay mac dinh
    */
    this.setDateTimePicker = function ($date, formatDate, defaultDate) {
        var _default = defaultDate ? defaultDate : new Date();
        $date.datetimepicker({
            format: formatDate,
            defaultDate: _default
        });
    }
    /**
        NamLD 12/10/2016 02:40:40
        get size object
    */
    this.getSizeObject = function (object) {
        Object.size = function(obj) {
            var size = 0, key;
            for (key in obj) {
                if (obj.hasOwnProperty(key)) size++;
            }
            return size;
        };
        return Object.size(object);
    }
    /**
        NamLD 12/10/2016 02:40:40
        tao phan trang
        element: doi tuong chua phan trang(dang jQuery)
        pageCount: so luong trang
        startPage: trang hien tai
        callback: func callback xu ly khi nhan phan trang, vd: click vao trang -> send request server
    */
    this.createPagination = function (element, pageCount, startPage, callback) {
        element.twbsPagination({
            pageCount: pageCount,
            startPage: startPage,
            funcCallback: callback,
            onPageClick: function (event, page) {
                $('#page-content').text('Page ' + page);
            }
        });
    }
}

