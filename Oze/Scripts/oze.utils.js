var g_Utils;
function Utils() {
    var self = this;
    OzeBase.apply(this, arguments);
    this.showTooltip = function () {
        $('[data-toggle="tooltip"]').tooltip({ trigger: "hover" });
    }

    this.init = function () {
        self.buildStyleRadio();
        this.showTooltip();
    }
    //hungpv
    this.SetDate = function (parameters) {
        $(parameters).datetimepicker({
            //debug:true,
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            //maxDate: new Date(),
            //defaultDate: new Date(),
            showClose: false

        });
    };
    //hungpv
    this.SetAmount = function () {
        $('.amount-double-mask')
            .on()
            .inputmask({
                alias: 'decimal',
                placeholder: '',
                groupSeparator: '.',
                radixPoint: ',',
                autoGroup: true,
                digits: 2,
                allowPlus: false,
                allowMinus: false,
                autoUnmask: true,
                integerDigits: 11,
                min: 1,
                max: 999999999
            });

        $('.amount-double-mask_0')
           .on()
           .inputmask({
               alias: 'decimal',
               placeholder: '',
               groupSeparator: '.',
               radixPoint: ',',
               autoGroup: true,
               digits: 2,
               allowPlus: false,
               allowMinus: false,
               autoUnmask: true,
               integerDigits: 11,
               min: 0,
               max: 999999999
           });

        $('.amount-double-maskpay_0')
        .on()
        .inputmask({
            alias: 'decimal',
            placeholder: '',
            groupSeparator: '.',
            radixPoint: ',',
            autoGroup: true,
            pattern: /-/,
            digits: 0,
            allowPlus: true,
            allowMinus: true,
            autoUnmask: true,
            integerDigits: 0,
            min: -999999999,
            max: 999999999,
            reverse: true,
            translation: {
                'S': {
                    pattern: /-/,
                    optional: true
                }
            }
        });

        $('.amount-number-mask')
            .on()
            .inputmask({
                alias: 'decimal',
                placeholder: '',
                groupSeparator: '.',
                radixPoint: ',',
                autoGroup: true,
                digits: 0,
                allowPlus: false,
                allowMinus: false,
                autoUnmask: true,
                integerDigits: 11,
                min: 1,
                max: 360

            });
    };
    //hungpv
    this.addDays = function (day) {
        var dat = new Date();
        dat.setDate(dat.getDate() + parseInt(day));

        return new Date(dat);
    }
    this.SetDateDefault = function () {
        //Author: hungpv  
        $(".date-search-picker_fdate").datetimepicker({
            //debug:true,
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            /*maxDate: new Date(),*/
            defaultDate: g_Utils.addDays(-30),
            showClose: false

        });

        $(".date-search-picker_tdate").datetimepicker({
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            /*maxDate: new Date(),*/
            defaultDate: new Date(),
            showClose: false
        });
        $(".date-picker").blur(function () {
            if ($(this).val() == "") {
                var date = new Date();
                var _fn = date.getDate();
                var _ft = date.getMonth();
                var _fna = date.getFullYear();
                var ft = (_ft + 1);
                if (ft < 10)
                    ft = "0" + ft;
                var fdate = (_fn + "/" + ft + "/" + _fna);
                $(this).val(fdate);

            }
        });

    };


    //hungpv
    this.SetDateDefaultAdd = function () {
        //Author: hungpv  
        $(".date-add-picker_fdate").datetimepicker({
            //debug:true,
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            /*minDate: new Date(),*/
            defaultDate: new Date(),
            showClose: false

        });

        $(".date-add-picker_tdate").datetimepicker({
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            /* minDate: new Date(),*/
            defaultDate: new Date(),
            showClose: false
        });
        $(".date-add-picker").datetimepicker({
            //locale: 'vi',
            format: 'DD/MM/YYYY',
            showTodayButton: true,
            defaultDate: new Date(),
            showClose: false
        });
        $(".date-add-picker_fdate_full").datetimepicker({
            //debug:true,
            //locale: 'vi',
            format: 'DD/MM/YYYY HH:mm',
            showTodayButton: true,
            /*minDate: new Date(),*/
            defaultDate: new Date(),
            showClose: false

        });

        $(".date-add-picker_tdate_full").datetimepicker({
            //locale: 'vi',
            format: 'DD/MM/YYYY HH:mm',
            showTodayButton: true,
            /*minDate: new Date(),*/
            defaultDate: g_Utils.addDays(1),
            showClose: false
        });
        //$(".date-add-picker_tdate_full_notdefault").datetimepicker({
        //    //locale: 'vi',
        //    format: 'DD/MM/YYYY HH:mm',
        //    showTodayButton: true,
        //   defaultDate: new Date(),
        //    showClose: false
        //});
        $(".date-picker").blur(function () {
            if ($(this).val() == "") {
                var date = new Date();
                var _fn = date.getDate();
                var _ft = date.getMonth();
                var _fna = date.getFullYear();
                var ft = (_ft + 1);
                if (ft < 10)
                    ft = "0" + ft;
                var fdate = (_fn + "/" + ft + "/" + _fna);
                $(this).val(fdate);

            }
        });
    };
    /**
  * Cấu hình Control AutoComplete
  * @param {Id Control cần cấu hình} idControl 
  * @param {Những cấu hình thêm hoặc cần override cấu hình mặc định} option 
  * @param {Trường cần hiển thị} displayField 
  * @param {Giá trị lấy ra của trường đó (thường là Id)} valueField 
  * @param {Funtion xử lý sau khi select item} fnSelect 
  * @param {Funtion xử lý Query truyền lên} fnQuery 
  * @param {Funtion xử lý dữ liệu trả về từ Server} fnProcess 
  * @param {Cấu hình thêm cho AutoComplete} option 
  * @returns {} 
  * Author: hungpv
  */
    this.ConfigAutocomplete = function (idControl, url, displayField, valueField, fnSelect, fnQuery, fnProcess, option) {
        var optionDefault = {
            onSelect: function (item) {
                $(idControl).data("seletectedValue", item.value);
                $(idControl).data("seletectedText", item.text);
                if (item.object !== undefined)
                    $(idControl).data("seletectedObject", JSON.parse(item.object));
                //$(idControl).valid();
                if (typeof (fnSelect) == "function")
                    fnSelect(item);
            },
            cache: false,
            ajax: {
                url: url,
                timeout: 500,
                displayField: displayField,
                valueField: valueField,
                cache: false,
                triggerLength: 3,
                loadingClass: "loading-circle",
                preDispatch: (fnQuery == undefined ? function (query) {
                    return {
                        search: query
                    }
                } : fnQuery),
                preProcess: (fnProcess == undefined ? function (data) {
                    if (data.success === false) {
                        return false;
                    }
                    return data;
                } : fnProcess)
            }
        }
        $.extend(optionDefault, option);
        $(idControl).typeahead(optionDefault);
    }

    this.ConfigAutocompleteFromControl = function (control, url, displayField, valueField, fnSelect, fnQuery, fnProcess, option) {
        var optionDefault = {
            onSelect: function (item) {
                control.data("seletectedValue", item.value);
                control.data("seletectedText", item.text);
                if (item.object !== undefined)
                    control.data("seletectedObject", JSON.parse(item.object));
                if (typeof (fnSelect) == "function")
                    fnSelect(item);
            },
            cache: false,
            ajax: {
                url: url,
                timeout: 500,
                displayField: displayField,
                valueField: valueField,
                cache: false,
                triggerLength: 3,
                loadingClass: "loading-circle",
                preDispatch: (fnQuery == undefined ? function (query) {
                    return {
                        search: query
                    }
                } : fnQuery),
                preProcess: (fnProcess == undefined ? function (data) {
                    if (data.success === false) {
                        return false;
                    }
                    return data;
                } : fnProcess)
            }
        }
        $.extend(optionDefault, option);
        control.typeahead(optionDefault);
    }


    /**
     * Reset control AutoComplete
     * @param {ID control cần reset} idControlReset 
     * @returns {} 
     */
    this.resetTypeHead = function (idControlReset) {
        $(idControlReset).val('');
        $(idControlReset).data("seletectedValue", null);
        $(idControlReset).data("seletectedText", null);
        $(idControlReset).parent().find('.typeahead > li').remove();
    }
}
$(document).ready(function () {
    g_Utils = new Utils();
    g_Utils.init();
    g_Utils.SetAmount();
    g_Utils.SetDateDefault();
    g_Utils.SetDateDefaultAdd();

})