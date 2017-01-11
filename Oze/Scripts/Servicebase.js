



var Dialog = {
    Success: 'success',
    Warning: 'warning',
    Error: 'error',
    CONFIRM: 'primary',
    /*  Description: Dialog thông báo
                     - message: Thông tin thông báo cần hiển thị.
                     - status: Trạng thái dialog (Success: Thành công, Warning: Cảnh báo, Error: Thất bại).
                     - callbackFuntion: Function Callback thực hiện sau khi ấn nút xác nhận form thông báo.
        Author: nhannv  */
    Alert: function (message, status, callbackFuntion, hideModalFuntion) {
        var typeDialog = this._getTypeDialog(status);
        bootbox.dialog({
            message: message,
            title: typeDialog.title,
            closeButton: false,
            className: typeDialog.className,
            buttons: {
                success: {
                    label: "<i class='fa fa-check'></i>" + "Đóng",
                    className: typeDialog.buttonClass,
                    callback: callbackFuntion
                }
            }
        }).on('shown.bs.modal', function () {
            $('.bootbox').find('button:first').focus();
        }).on('hidden.bs.modal', function () {
            var p = $("body").css('padding-right');
            var p1 = parseInt(p) - 17;
            if (p1 >= 0)
                $("body").css('padding-right', p1);
            hideModalFuntion == undefined ? function () { } : hideModalFuntion();
        });
    },
    /*  Description: Dialog Config custom
                    - message: Thông tin thông báo cần hiển thị.
                    - callbackFuntion: Function Callback thực hiện sau khi ấn nút xác nhận form thông báo.
        Author: nhannv  */
    ConfirmCustom: function (title, message, callbackFuntion, showModalFuntion) {
        var typeDialog = this._getTypeDialog(this.CONFIRM);
        bootbox.dialog({
            message: message,
            title: title ? title : typeDialog.title,// title ? typeDialog.title : title,
            closeButton: false,
            className: typeDialog.className,
            buttons: {
                success: {
                    label: "<i class='fa fa-check'></i>" + "Xác nhận",
                    className: typeDialog.buttonClass,
                    callback: callbackFuntion
                },
                cancel: {
                    label: "<i class='fa fa-reply'></i>" + "Đóng",
                    className: "btn btn-df"
                }
            }
        }).on('shown.bs.modal', showModalFuntion == undefined ? function () {
            //$('.bootbox').find('button:first').focus();
        } : showModalFuntion);
    },
    /*  Description: Hàm xác định kiểu của Dialog
        Author: nhannv  */
    _getTypeDialog: function (status) {
        var type = {};
        switch (status) {
            case 'success':
                type = {
                    title: "Thành công",
                    className: 'my-modal-success',
                    buttonClass: 'btn btn-primary'
                };
                break;
            case 'warning':
                type = {
                    title: "Thông báo",
                    className: 'my-modal-warning',
                    buttonClass: 'btn btn-df'
                };
                break;
            case 'error':
                type = {
                    title: "Lỗi",
                    className: 'my-modal-error',
                    buttonClass: 'btn btn-df'
                };
                break;
            case 'primary':
                type = {
                    title: "Xác nhận",
                    className: 'my-modal-primary',
                    buttonClass: 'btn btn-primary'
                };
                break;
        }
        return type;
    }
}

var Service = function () {
    var base = this;

    var dialogLoading;
    function showDialogLoading(msg) {
        if (!msg) msg = "Đang tải...<img src='/images/load.gif' />";
        dialogLoading = bootbox.dialog({
            message: '<p class="text-center">' + msg + '</p>',
            closeButton: false
        });
        // do something in the background

    }
    function hideDialogLoading() {
        if (dialogLoading) dialogLoading.modal('hide');
    }


    this.RequestStart = function () {
        showDialogLoading();
        //if ($("body > div.ajaxInProgress").length <= 0) {
        //    var str = '<div class="ajaxInProgress"><div class="loading-ct" >' +
        //        //'<i class="fa fa-spinner fa-pulse"></i>' +
        //        '<img src="/Assets/Admin/images/ajax-loader.gif">' +
        //        '<div>' + "Đang tải" + '</div>' +
        //        ' </div> </div>';
        //    $("body").append(str);
        //}
        //$("body > div.ajaxInProgress").show();
    }

    this.RequestEnd = function () {
        hideDialogLoading();
        //if ($("body > div.ajaxInProgress").length > 0)
        //    $("body > div.ajaxInProgress").hide();
    }

    this.CheckAuthen = function (option, fnSuccess, fnError) {
        //bỏ check đăng nhập
        var obj = {};
        obj.IsAuthen = true;
        fnSuccess(obj);

        //$.ajax({
        //    url: option.Url,
        //    type: 'Post',
        //    data: option.Data,
        //    beforeSend: function () {
        //        base.RequestStart();
        //    },
        //    async: (option.async == undefined ? true : option.async),
        //    complete: function () {
        //        // base.RequestEnd();
        //    },
        //    success: function (rs) {
        //        if (!rs.IsAuthen)
        //            base.RequestEnd();
        //        if (typeof fnSuccess === "function")
        //            fnSuccess(rs);
        //    },
        //    error: function (e) {
        //        if (!fnError)
        //            Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
        //        if (typeof fnError === "function")
        //            fnError(e);
        //        base.RequestEnd()
        //    }
        //});
    }

    this.Post = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();

        }
        else {
            option.Data = { __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }
        }
        $.ajax({
            url: option.Url,
            type: 'Post',
            data: option.Data,
            beforeSend: function () {
                base.RequestStart();
            },
            async: (option.async == undefined ? true : option.async),
            complete: function () {
                base.RequestEnd();
            },
            success: function (rs) {
                if (typeof fnSuccess === "function")
                    fnSuccess(rs);
            },
            error: function (e) {
                if (!fnError)
                    Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
                if (typeof fnError === "function")
                    fnError(e);
            }
        });
    }

    this.AjaxPost = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();

        }
        else {
            option.Data = { __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() }
        }
        if (option.CheckAuthen != false) {
            base.CheckAuthen({
                Url: "/Index/CheckAuthen",
            }, function (rs) {
                if (!rs.IsAuthen) {
                    Dialog.Alert("Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!", Dialog.Error, "Session Timedout", function () {
                        window.location = "/login?ReturnUrl=" + window.location.href;
                    });
                } else {
                    base.Post(option, fnSuccess, fnError);
                }
            }, function (e) {
            });
        } else {
            base.Post(option, fnSuccess, fnError);
        }
    }

    //this.AjaxPost = function (option, fnSuccess, fnError) {
    //    $.ajax({
    //        url: option.Url,
    //        type: 'Post',
    //        data: option.Data,

    //        beforeSend: function () {
    //            base.RequestStart();
    //        },
    //        async: (option.async == undefined ? true : option.async),
    //        complete: function () {
    //            base.RequestEnd();
    //        },
    //        success: function (rs) {
    //            if (typeof fnSuccess === "function")
    //                fnSuccess(rs);
    //        },
    //        error: function (e) {
    //            if (!fnError)
    //                Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
    //            if (typeof fnError === "function")
    //                fnError(e);
    //        }
    //    });
    //}

    this.DateTimeNow = function () {
        var dfFormDate = new Date();
        var dfToDate = new Date();
        dfFormDate.setHours(0);
        dfFormDate.setMinutes(0);
        dfFormDate.setSeconds(0);
        dfToDate.setHours(23);
        dfToDate.setMinutes(59);
        dfToDate.setSeconds(59);
        return {
            FormDate: dfFormDate,
            ToDate: dfToDate,
            MomentFromDate: moment(dfFormDate),
            MomentToDate: moment(dfToDate)
        }
    }
    ///PostFormCollection

    this.AjaxPostDemo = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.append("__RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
        }
        else {
            var data = new FormData();
            data.append("__RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
            option.Data = data;
        }
        if (option.CheckAuthen != false) {
            base.CheckAuthen({
                Url: "/Index/CheckAuthen",
            }, function (rs) {
                if (!rs.IsAuthen) {
                    Dialog.Alert("Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!", Dialog.Error, "Session Timedout", function () {
                        window.location = "/login?ReturnUrl=" + window.location.href;
                    });
                } else {
                    $.ajax({
                        url: option.Url,
                        type: 'Post',
                        //contentType: false,
                        //mimeType: false,
                        ////contentType: 'multipart/form-data',
                        ////mimeType: 'multipart/form-data',
                        //processData: false,
                        enctype: 'multipart/form-data',
                        cache: false,
                        contentType: false,
                        processData: false,
                        data: option.Data,
                        dataType: 'json',

                        beforeSend: function () {
                            base.RequestStart();
                        },
                        async: (option.async == undefined ? true : option.async),
                        complete: function () {
                            base.RequestEnd();
                        },
                        success: function (rs) {
                            if (typeof fnSuccess === "function")
                                fnSuccess(rs);
                        },
                        error: function (e) {
                            if (!fnError)
                                Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
                            if (typeof fnError === "function")
                                fnError(e);
                        }
                    });
                }
            }, function (e) {
            });
        } else {
            $.ajax({
                url: option.Url,
                type: 'Post',
                //contentType: false,
                //mimeType: false,
                ////contentType: 'multipart/form-data',
                ////mimeType: 'multipart/form-data',
                //processData: false,
                enctype: 'multipart/form-data',
                cache: false,
                contentType: false,
                processData: false,
                data: option.Data,
                dataType: 'json',

                beforeSend: function () {
                    base.RequestStart();
                },
                async: (option.async == undefined ? true : option.async),
                complete: function () {
                    base.RequestEnd();
                },
                success: function (rs) {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                },
                error: function (e) {
                    if (!fnError)
                        Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
                    if (typeof fnError === "function")
                        fnError(e);
                }
            });
        }
    }
    this.AjaxPostList = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() };
        }
        if (option.CheckAuthen != false) {
            base.CheckAuthen({
                Url: "/Index/CheckAuthen",
            }, function (rs) {
                if (!rs.IsAuthen) {
                    Dialog.Alert("Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!", Dialog.Error, "Session Timedout", function () {
                        window.location = "/login?ReturnUrl=" + window.location.href;
                    });
                } else {
                    $.ajax({
                        url: option.Url,
                        type: 'Post',
                        data: option.Data,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        beforeSend: function () {
                            base.RequestStart();
                        },
                        complete: function () {
                            base.RequestEnd();
                        },
                        success: function (rs) {
                            if (typeof fnSuccess === "function")
                                fnSuccess(rs);
                        },
                        error: function (e) {
                            if (typeof fnError === "function")
                                fnError(e);
                        }
                    });
                }
            }, function (e) {
            });
        } else {
            $.ajax({
                url: option.Url,
                type: 'Post',
                data: option.Data,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    base.RequestStart();
                },
                complete: function () {
                    base.RequestEnd();
                },
                success: function (rs) {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                },
                error: function (e) {
                    if (typeof fnError === "function")
                        fnError(e);
                }
            });
        }
    }

    this.AjaxGet = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val() };
        }
        if (option.CheckAuthen != false) {
            base.CheckAuthen({
                Url: "/Index/CheckAuthen",
            }, function (rs) {
                if (!rs.IsAuthen) {
                    Dialog.Alert("Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!", Dialog.Error, "Session Timedout", function () {
                        window.location = "/login?ReturnUrl=" + window.location.href;
                    });
                } else {
                    $.ajax({
                        url: option.Url,
                        type: 'Get',
                        data: option.Data,
                        beforeSend: function () {
                            base.RequestStart();
                        },
                        complete: function () {
                            base.RequestEnd();
                        },
                        success: function (rs) {
                            if (typeof fnSuccess === "function")
                                fnSuccess(rs);
                        },
                        error: function (e) {
                            if (typeof fnError === "function")
                                fnError(e);
                        }
                    });
                }
            }, function (e) {
            });
        } else {
            $.ajax({
                url: option.Url,
                type: 'Get',
                data: option.Data,
                beforeSend: function () {
                    base.RequestStart();
                },
                complete: function () {
                    base.RequestEnd();
                },
                success: function (rs) {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                },
                error: function (e) {
                    if (typeof fnError === "function")
                        fnError(e);
                }
            });
        }
    }

    this.JoinObject = function (oldObj, newObj) {
        if (typeof oldObj === "object" && oldObj != null
           && typeof newObj === "object" && newObj != null) {
            for (var key in newObj) {
                if (newObj.hasOwnProperty(key)) {
                    oldObj[key] = newObj[key];
                }
            }
        }
        return oldObj;
    }

    this.NumberToString = function (value) {
        return value != null ? value.toString().replace('.', ',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') : 0;
    };

    this.DateToString = function (value, fomart) {
        return moment(new Date(parseInt(value.slice(6, -2)))).format(fomart);
    };

    //==================================================================
    //	Description:  Bootstrap Table					 Option, function
    //	Author: Nhannv
    //==================================================================
    this.Sprintf = function (str) {
        var args = arguments,
            flag = true,
            i = 1;

        str = str.replace(/%s/g, function () {
            var arg = args[i++];

            if (typeof arg === 'undefined') {
                flag = false;
                return '';
            }
            return arg;
        });
        return flag ? str : '';
    };

    this.BootstrapTableOption = function (option) {
        var obj = {
            locale: 'en-US',
            classes: 'table table table-striped table-bordered dataTable no-footer', // table-hover
            cache: false,
            pagination: true,
            pageSize: 15,
            pageList: [15, 20, 30, 50, 100],
            formatLoadingMessage: function () {
                return '<div class="ajaxInProgress"> <div class="loading-ct" >' +
                    '<img src="/Assets/Admin/images/ajax-loader.gif">' +
                '<div>' + "Đang tải" + '</div>' +
                    '</div> </div>';
            },
            method: 'post',
            sidePagination: 'server',
            queryParams: function (params) {
                return params;
            },
            responseHandler: function (res) {
                return {
                    total: res.total,
                    rows: res.data
                };
            },
        };
        var xx = base.JoinObject(obj, option);
        return xx;
    }

    this.DateTimeToString = function (value) {

        return value ? moment(new Date(parseInt(value.slice(6, -2)))).format("DD/MM/YYYY HH:mm:ss") : "";
    }

    this.BootstrapTableColumn = function (type, option) {
        var align = "";
        var formatFn;
        var className = "";
        if (typeof type === "function")
            type = type();
        switch (type) {
            case "Number":
                align = "right";
                className = "row-number";
                formatFn = function (value) {
                    return value ? value.toString().replace('.', ',').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') : 0;
                }
                break;
                /* Nhannv - Dự án này áp dụng luôn 1 kiêu date format */
                //case "Date0":
                //    align = "center";
                //    className = "row-date";
                //    formatFn = function (value) {
                //        return value ? moment(new Date(parseInt(value.slice(6, -2)))).format('DD/MM/YYYY') : "";
                //    }
                //    break;
                //case "DateTime0":
                //    align = "center";
                //    className = "row-date";
                //    formatFn = function (value) {
                //        return value ? moment(new Date(parseInt(value.slice(6, -2)))).format('DD/MM/YYYY HH:mm') : "";
                //    }
                //    break;
            case "DateTimeFull":
            case "Date":
            case "DateTime":
                align = "center";
                className = "row-date";
                formatFn = function (value) {
                    return value ? moment(new Date(parseInt(value.slice(6, -2)))).format("DD/MM/YYYY HH:mm:ss") : "";
                }
                break;
            default:
                align = "left";
                className = "row-string";
                formatFn = function (value) {
                    return value ? value : "";
                }
                break;
        }
        var obj = {
            align: align,
            valign: "middle",
            width: "100px",
            'class': className,
            formatter: formatFn,
        };
        return base.JoinObject(obj, option);
    }

    this.BootstrapTableSTT = function ($table, index) {
        var option = $table.bootstrapTable('getOptions');
        if (option.sidePagination == "server") {
            var i = (option.pageNumber - 1) * option.pageSize;
            return i + index + 1;
        } else {
            return index + 1;
        }
    }

    this.ResponseHandlerFooter = function (res) {
        var obj = {
            total: res.total,
            rows: res.data != null ? res.data : [],
        };
        obj.rows.Footer = res.footer;
        return obj;
    }

    this.ResponseHandlerSearch = function (res, $modalSearch, $table) {
        //$modalSearch.modal("hide");
        if (res.total == 0) {
            $("body").css('padding-right', 0);
            $table.bootstrapTable('removeAll');
            Dialog.Alert("Không tìm thấy dữ liệu theo điều kiện tìm kiếm!", Dialog.Warning);
        }
        return {
            total: res.total,
            rows: res.data
        };
    },
    this.LoadTableSearch = function ($table, url, showDialog) {

        $table.bootstrapTable('refreshOptions', {
            url: url,
            responseHandler: function (res) {
                if (res.Status) {
                    if (res.Status == "URL") {
                        window.location.assign(res.Message);

                        return false;
                    }
                    if (res.Status == "Login") {
                        window.location = "/login?ReturnUrl=" + window.location.href;
                        return false;
                    }
                    return base.ResponseHandler($table, showDialog, res.Data);
                } else {
                    return base.ResponseHandler($table, showDialog, res);
                }
            },
            sidePagination: 'server',
        });
    }

    //this.CheckAuthen = function (callback) {
    //    Sv.AjaxPost({
    //        Url: "/Index/CheckAuthen",
    //    }, function (rs) {
    //        if (!rs.IsAuthen) {
    //            Dialog.Alert( "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!", Dialog.Error,  "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại!"Title, function () {
    //                window.location = "/Admin/Login?ReturnUrl=" + window.location.href;
    //            });
    //            return false;
    //        } else {
    //            if (typeof callback === "function")
    //                return callback();
    //            else
    //                return true;
    //        }

    //    }, function (e) {
    //        Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
    //        return false;
    //    });
    //}

    this.ResponseHandler = function ($table, showDialog, res) {
        if (res.total == 0) {
            $table.bootstrapTable('removeAll');
            if (res.total == 0 && showDialog)
                Dialog.Alert("Không tìm thấy dữ liệu theo điều kiện tìm kiếm!", Dialog.Error);
        }
        return { total: res.total, rows: res.data };
    }

    //Nhannv - ResetView Table
    //Nhannv - ResetView Table
    this.ResetViewTable = function () {
        try {
            for (var i = 0; i < arguments.length; i++) {
                arguments[i].bootstrapTable('resetView');
            }
        } catch (e) { }
        return;
    };

    ////Nhannv -  setup amount-mask
    //this.SetupAmountMask = function () {
    //    //Mask_groupSeparator: '.',
    //    //Mask_radixPoint: ',',
    //    //Mask_integerDigits: 11,
    //    //Mask_digits: 0,
    //    $('.amount-mask').on().inputmask({
    //        alias: 'decimal',
    //        placeholder: '',
    //        groupSeparator: language.Mask_groupSeparator,
    //        radixPoint: language.Mask_radixPoint,
    //        autoGroup: true,
    //        digits: language.Mask_digits,
    //        allowPlus: false,
    //        allowMinus: false,
    //        autoUnmask: true,
    //        integerDigits: language.Mask_integerDigits
    //    });
    //    $('.amount-mask2').on().inputmask({
    //        alias: 'decimal',
    //        placeholder: '',
    //        groupSeparator: language.Mask_groupSeparator,
    //        radixPoint: language.Mask_radixPoint,
    //        autoGroup: true,
    //        digits: language.Mask_digits,
    //        allowPlus: true,
    //        allowMinus: true,
    //        autoUnmask: true,
    //        integerDigits: language.Mask_integerDigits
    //    });
    //    $('.discount-mask').inputmask({
    //        alias: "percentage",
    //        placeholder: '',
    //        radixPoint: language.Mask_radixPoint,
    //        autoUnmask: true,
    //    });
    //}

    //Nhannv -  set date
    this.DefaultDate = function () {
        var dfFormDate = new Date();
        var dfToDate = new Date();
        var dfMax = new Date();
        dfFormDate.setHours(0);
        dfFormDate.setMinutes(0);
        dfFormDate.setSeconds(0);
        dfFormDate.setMilliseconds(000);
        dfToDate.setHours(23);
        dfToDate.setMinutes(59);
        dfToDate.setSeconds(59);
        dfToDate.setMilliseconds(000);
        dfMax.setHours(23);
        dfMax.setMinutes(59);
        dfMax.setSeconds(59);
        dfMax.setMilliseconds(999);
        return {
            FormDate: dfFormDate,
            ToDate: dfToDate,
            //Maxdate dfMax,
            MomentFromDate: moment(dfFormDate),
            MomentToDate: moment(dfToDate),
            MomentMaxDate: moment(dfMax)
        }
    }

    //Nhannv -  set date
    this.SetupDateTime = function (input1, input2) {
        input1.datetimepicker({
            format: "DD/MM/YYYY HH:mm:ss",
            showTodayButton: true,
            //Maxdate: base.DefaultDate().MomentMaxDate,
            defaultDate: base.DefaultDate().MomentFromDate,
            showClose: true,
        });
        input2.datetimepicker({
            format: "DD/MM/YYYY HH:mm:ss",
            showTodayButton: true,
            //Maxdate: base.DefaultDate().MomentMaxDate,
            defaultDate: base.DefaultDate().MomentToDate,
            showClose: true,
        });
        $('.date-time-mask').inputmask({
            alias: "99/99/9999 99:99:99",
            placeholder: ""
        });
    }

    this.SetupDateTimeOneInput = function (input1,format) {
        input1.datetimepicker({
            format: format,
            showTodayButton: true,
            //Maxdate: base.DefaultDate().MomentMaxDate,
            defaultDate: base.DefaultDate().MomentFromDate,
            showClose: true,
        });
        $('.date-time-mask').inputmask({
            alias: "99/99/9999 99:99:99",
            placeholder: ""
        });
    }


    this.SetupDate = function () {
        for (i = 0; i < arguments.length; i++) {
            var input = arguments[i];
            input.datetimepicker({
                format: 'MM/DD/YYYY',
                showTodayButton: true,
                //Maxdate: base.DefaultDate().MomentMaxDate,
                defaultDate: base.DefaultDate().MomentFromDate,
                showClose: true,
            });
        }
        $('.datemask').inputmask({
            alias: "99/99/9999 99:99:99",
            placeholder: ""
        });
    }

    this.BindPopup = function (url, model, callback) {
        base.AjaxPost({
            Url: url,
            Data: model
        }, function (rs) {
            if (rs.Status && rs.Status == "Login") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else if (rs.Status && rs.Status == "URL") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else if (rs.Status && rs.Status === "00") {
                Dialog.Alert(rs.Message, Dialog.Error);
            } else {
                if (typeof callback == "function")
                    callback(rs);
            }
        }, function () {
            Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
        });
    }
    //hungpv
    this.BindPopupOTP = function (callback) {
        base.AjaxPost({
            Url: "/Admin/Common/BindPopupOTP",
            Data: {}
        }, function (rs) {
            if (rs.Status && rs.Status == "Login") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else if (rs.Status && rs.Status == "URL") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else if (rs.Status && rs.Status === "00") {
                Dialog.Alert(rs.Message, Dialog.Error);
            } else {
                if (typeof callback == "function")
                    callback(rs);
            }
        }, function () {
            Dialog.Alert("Có lỗi trong quá trình xử lý. Vui lòng thử lại", Dialog.Error);
        });
    }



    var s = null; // Giây
    var timeout = null; // Timeout

    this.start = function ($time, callback) {
        /*BƯỚC 1: LẤY GIÁ TRỊ BAN ĐẦU*/
        if (s === null) {
            s = parseInt($time.html());
        }



        // Nếu số giờ = -1 tức là đã hết giờ, lúc này:
        //  - Dừng chương trình
        if (s == 0) {
            clearTimeout(timeout);
            $time.text("0");

            return false;
        }

        /*BƯỚC 1: GIẢM PHÚT XUỐNG 1 GIÂY VÀ GỌI LẠI SAU 1 GIÂY */
        timeout = setTimeout(function () {
            s--;
            base.start($time);
            $time.text("" + s + "");
        }, 1000);

        return true;
    }


    //this.DocSo = function ($e, val) {
    //    if (val > 0) {
    //        $e.html(language.MoneyToString(val));
    //    } else {
    //        $e.html("");
    //    }
    //}

    //==================================================================
    //	Description:  tạo ra các button thêm sửa xóa
    //	Author: Nhannv
    //==================================================================
    this.BindBtnCtrl = function ($e) {
        var name = $e.data("name");
        var type = $e.data("type");
        $e.attr("type", "button");
        $e.attr("title", name);
        $e.addClass("btn btn-primary btn-page-action");
        if (type.toUpperCase() == "SEARCH") {
            $e.html('<i class="fa fa-search"></i>' + name);
            return;
        }
        if (type.toUpperCase() == "ADD") {
            $e.html('<i class="fa fa-plus-square-o"></i>' + name);
            return;
        }
        if (type.toUpperCase() == "EXPORT") {
            $e.html('<i class="fa fa-print"></i>' + name);
            return;
        }
        if (type.toUpperCase() == "RESET") {
            $e.html('<i class="fa fa-refresh"></i>' + name);
            return;
        }
        if (type.toUpperCase() == "CLOSE") {
            $e.removeClass().addClass("btn btn-df");
            $e.html('<i class="fa fa-reply"></i>' + name);
            return;
        }
        return;
    }

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
    this.ConfigAutocomplete = function (idControl, url, displayField, valueField, triggerLength, fnSelect, fnQuery, fnProcess, option) {
        var optionDefault = {
            onSelect: function (item) {
                $(idControl).data("seletectedValue", item.value);
                $(idControl).data("seletectedText", item.text);
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
                triggerLength: triggerLength,
                loadingClass: "ax",
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

    //==================================================================
    //	Description:  config 	Typeahead
    //	Author: Nhannv
    //==================================================================
    this.ConfigTypeahead = function ($e, option) {
        var optionDefault = {
            onSelect: function (item) {
                $e.data("object", JSON.parse(item.object));
                $e.data("seletectedValue", item.value);
                $e.data("seletectedText", item.text);
                if (typeof (option.onSelect) == "function")
                    option.onSelect(item);
            },
            cache: false,
            fillValueOld: false,
            ajax: {
                url: option.url,
                timeout: option.timeout ? option.timeout : 500,
                displayField: option.displayField,
                valueField: option.valueField,
                cache: false,
                triggerLength: option.triggerLength ? option.triggerLength : 4,
                loadingClass: option.loadingClass ? option.loadingClass : "",
                preDispatch: function (query) {
                    if (option.preDispatch == undefined)
                        return {
                            search: query
                        }
                    else
                        return option.preDispatch(query);
                },
                preProcess: function (data) {
                    if (option.preProcess == undefined) {
                        if (data.success === false) {
                            return false;
                        }
                        return data;
                    } else {
                        return option.preProcess(data);
                    }
                },
                loading: function (check) {
                    check ? base.RequestStart() : base.RequestEnd();
                }
            }
        }
        $.extend(optionDefault, option);
        $e.typeahead(optionDefault);
    }
    this.SetTypeaheadValue = function ($e, item) {
        if (item != null) {
            $e.data("seletectedValue", item.SrcCustomerID);
            $e.data("seletectedText", item.SrcCustomerView);
        }
    }

    this.AlertJsonRs = function (rs, callback) {
        Dialog.Alert(rs.Message, Dialog.Error, function () {
            if (rs.Status && rs.Status == "Login") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else if (rs.Status && rs.Status == "URL") {
                window.location = "/login?ReturnUrl=" + window.location.href;
            } else {
                if (typeof callback == "function")
                    callback();
            }
        });
    }

    this.ReturnJsonRs = function (rs, successFn, callback) {
        if (rs.Status && rs.Status == "01") {
            if (typeof successFn == "function")
                successFn();
            else
                Dialog.Alert(rs.Message, Dialog.Success);
        } else {
            base.AlertJsonRs(rs, callback);
        }
    }

    //this.ValidateNote = function ($e, validate, mgs1, mgs2) {
    //    $e.rules("remove");
    //    if (validate) {
    //        $("#myinput").rules("add", {
    //            required: true,
    //            maxlength: 500,
    //            messages: {
    //                required: mgs1 ? mgs1 : language.Validate_Description_Required,
    //                minlength: mgs2 ? mgs2 : language.Validate_Description_Maxlength
    //            }
    //        });
    //    }
    //}
    //hoanglt add validate file
    //this.AddValidateFile = function () {
    //    $(".required-file").rules("add", {
    //        required: true,
    //        messages: {
    //            required: language.AllocatingFinancialTransactions.Validate_File_Required
    //        }
    //    });
    //}
    //--ResetForm
    this.ResetForm = function ($form, $fdate, $todate) {
        var validator = $form.validate();
        validator.resetForm();
        $form.each(function () {
            this.reset();
        });
        $form.find($fdate).data("DateTimePicker").date(Sv.DefaultDate().MomentFromDate);
        $form.find($todate).data("DateTimePicker").date(Sv.DefaultDate().MomentToDate);
    }
    this.ResetFormOnly = function ($form) {
        var validator = $form.validate();
        validator.resetForm();
        $form.each(function () {
            this.reset();
        });
    }
};

var Sv = new Service();

$(document).ready(function () {
    /*  Description: Cấu hình datetimepicker
                Author: Nhannv  */
    $('.input-date').datetimepicker({
        //locale: "en",
        format: 'DD/MM/YYYY',
        showTodayButton: true,
        showClose: true
    });

    /*  Description: Cấu hình datetimepicker (hiển thị cả giờ phút)
        Author: Nhannv  */
    $('.input-datetime').datetimepicker({
        //  locale: "en",
        format: 'DD/MM/YYYY HH:mm:ss',
        showTodayButton: true,
        showClose: true,
        //   debug:true
    });

    /*  Description: Cấu hình bootsrtap select
        Author: Nhannv  */
    //$('.selectpicker').selectpicker({
    //    liveSearch: true,
    //    style: 'btn-default',
    //    size: 10,
    //    noneSelectedText: ''
    //});

    /*  Description: Cấu hình Mask cho các control nhập số tiền (ko có phần thập phân)
        Author: Nhannv  */
    $('.price-mask').inputmask({
        alias: 'decimal',
        groupSeparator: '.', /* Ký tự phân cách giữa phần trăm, nghìn... */
        radixPoint: ",", /* Ký tự phân cách với phần thập phân */
        autoGroup: true,
        digits: 0, /* Lấy bao nhiêu số phần thập phân, mặc định của inputmask là 2 */
        autoUnmask: true, /* Tự động bỏ mask khi lấy giá trị */
        allowMinus: false, /* Không cho nhập dấu trừ */
        allowPlus: false, /* Không cho nhập dấu cộng */
        integerDigits: 9
    });

    /*  Description: Cấu hình Mask cho các control nhập số tiền
        Author: Nhannv  */
    $('.price-mask-digits').inputmask({
        alias: 'decimal',
        groupSeparator: '.', /* Ký tự phân cách giữa phần trăm, nghìn... */
        radixPoint: ",", /* Ký tự phân cách với phần thập phân */
        autoGroup: true,
        digits: 2, /* Lấy bao nhiêu số phần thập phân, mặc định của inputmask là 2 */
        autoUnmask: true, /* Tự động bỏ mask khi lấy giá trị */
        allowMinus: false, /* Không cho nhập dấu trừ */
        allowPlus: false, /* Không cho nhập dấu cộng */
        integerDigits: 9
    });

    /*  Description: Cấu hình Mask cho các control nhập chiết khấu
        Author: Nhannv  */
    $('.discounted-mask').inputmask("percentage", {
        placeholder: '',
        radixPoint: ",",
        autoUnmask: true
    });

    /*  Description: Cấu hình Validation các control
        Author: Nhannv  */
    $('form.formValid').each(function () {
        $(this).validate({
            ignore: '',
            errorPlacement: function (error, element) {
                var tagParent = element.parent();
                /* Nhannv: Đoạn kiểm tra nếu như thẻ div bọc chưa button và input
                            thì add class lỗi lên thẻ div cha cao hơn */
                if (tagParent.hasClass('input-group')) {
                    tagParent.parent().append(error);
                } else {
                    tagParent.append(error);
                }
                error.addClass('css-error');
            },
            onfocusout: false,
            invalidHandler: function (form, validator) {
                var errors = validator.numberOfInvalids();
                if (errors) {
                    validator.errorList[0].element.focus();
                }
            }
        });
    });

    /*  Description: Cấu hình Validation định dạng ngày tháng
        Author: Nhannv  */
    $.validator.addMethod("dateFormat", function (value, element, params) {
        var isDate = function (valueDate) {
            var currVal = valueDate;
            if (currVal === "") return false;
            var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
            var dtArray = currVal.match(rxDatePattern);
            if (dtArray == null) return false;
            var dtDay = dtArray[1];
            var dtMonth = dtArray[3];
            var dtYear = dtArray[5];
            if (dtMonth < 1 || dtMonth > 12)
                return false;
            else if (dtDay < 1 || dtDay > 31)
                return false;
            else if ((dtMonth === 4 || dtMonth === 6 || dtMonth === 9 || dtMonth === 11) && dtDay === 31)
                return false;
            else if (dtMonth === 2) {
                var isleap = (dtYear % 4 === 0 && (dtYear % 100 !== 0 || dtYear % 400 === 0));
                if (dtDay > 29 || (dtDay === 29 && !isleap))
                    return false;
            }
            return true;
        };
        return this.optional(element) || isDate(value);
    }, 'Message Null'); //Nhannv - ko show buộc phải customer đa ngôn ngữ (hoặc phải customer câu thông báo này) -- Định dạng ngày không hợp lệ, vui lòng kiểm tra lại

    /*  Description: Cấu hình Validation báo lỗi khi mật khấu chứa các ký tự unicode như â, ă...
        Author: Nhannv  */
    $.validator.addMethod("password-regex", function (value, element) {
        return this.optional(element) || /^[A-Za-z0-9\s`~!@#$%^&*()+={}|;:'",.<>\/?\\-]+$/.test(value);
    }, 'Message Null'); //Nhannv - Mật khẩu chứa ký tự không hợp lệ (ă, â, đ, ...), vui lòng kiểm tra lại

    /*  Description: Cấu hình Validation báo lỗi khi chứa ký tự khoảng trắng.
        Author: Nhannv  */
    $.validator.addMethod("nospace", function (value, element) {
        return value.indexOf(" ") < 0 && value != "";
    }, "Message Null"); //Nhannv - Không chưa ký tự khoảng trắng

    /*  Description: Cấu hình Validation chỉ nhập chữ và báo lỗi khi nhập số hoặc ký tự đặc biệt như !@#$%^&*()_+-=...
        Author: Nhannv  */
    $.validator.addMethod("spacecharacters", function (value, element) {
        return this.optional(element) || !/[~`!@#$%\^&*()_+=\-\[\]\\';,./{}|\\":<>\?0-9]/g.test(value);
    }, 'Message Null'); //Nhannv -Không chứa ký tự đặc biệt, vui lòng kiểm tra lại

    $(".date-picker").datetimepicker({
        // locale: "en",
        format: 'MM/DD/YYYY HH:mm:ss',
        showTodayButton: true,
        //Maxdate: new Date(),
        defaultDate: new Date(),
        showClose: true,
    });

    $('.date-mask').inputmask({
        alias: 'MM/DD/YYYY',
        placeholder: ""
    });

    /*  Description: Cấu hình Mask datetime
        Author: Nhannv  */
    $('.datetime-mask').inputmask("datetime", { "placeholder": "" });

    // config modal close
    $(".modal,.bootbox.modal").on('hidden.bs.modal', function () {
        //var p = $("body").css('padding-right');
    });

    //==================================================================
    //	Description:  bind button
    //	Author: Nhannv
    //==================================================================
    if ($("button[binding]").length > 0) {
        for (var i = 0; i < $("button[binding]").length ; i++) {
            Sv.BindBtnCtrl($($("button[binding]")[i]));
        }
    }
});