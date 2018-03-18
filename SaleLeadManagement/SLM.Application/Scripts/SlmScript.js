function ChkNotThaiCharacter(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (charCode == 8 || charCode == 99 || charCode == 118 || charCode == 120)
        return true;
    else if ((charCode >= 3585 && charCode <= 3675))                         //UTF8 ก=3585 
        return false;
    else
        return true;
}
function ChkNotEnglishCharacter(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (charCode == 8 || charCode == 99 || charCode == 118 || charCode == 120)
        return true;
    else if ((charCode >= 97 && charCode <= 122) || (charCode >= 65 && charCode <= 90))                         //a-z : 97-122 / A-Z : 65-90 / 8 : backspace
        return false;
    else
        return true;
}

function ChkNumeric(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if ((charCode >= 48 && charCode <= 57) || charCode == 8 || charCode == 99 || charCode == 118 || charCode == 120)
        return true;
    else {
        if (charCode == 46) {
            if (e.target.value.indexOf(".", 0) >= 0)
                return false;
            else
                return true;
        }
        return false;
    }
}

function ChkInt(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (e.ctrlKey == true) {
        if (charCode == 99 || charCode == 118 || charCode == 120)   //99=c, 118=v, 120=x
            return true;
        else
            return false;
    }
    else {
        if (charCode != 8 && (charCode < 48 || charCode > 57)) {  //8=backspace, 48-57=0-9
            return false;
        }
        else {
            return true;           
        }
    }
}

function ChkIntMinus(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (e.ctrlKey == true) {
        if (charCode == 99 || charCode == 118 || charCode == 120)   //99=c, 118=v, 120=x
            return true;
        else
            return false;
    }
    else {
        if (charCode != 8 && charCode != 45 && (charCode < 48 || charCode > 57)) {  //8=backspace, 48-57=0-9
            return false;
        }
        else {
            if (charCode == 45) {
                if (e.target.value.indexOf("-", 0) >= 0)
                    return false;
                else
                    return true;
            } else {
                return true;
            }
        }
    }
}

function ChkIntOnBlur(textbox, labelId, errmsg) {
    if (isNaN(textbox.value)) {
        document.getElementById(labelId).innerHTML = errmsg;
    }
}
function ChkIntOnBlurClear(textbox) {
    if (isNaN(textbox.value)) {
        textbox.value = '';
        textbox.focus();
    }
}

function ChkDbl(e, ctl) {
    var charCode = (e.which) ? e.which : event.keyCode
    if (charCode != 8 && (charCode < 48 || charCode > 57)) {
        if (charCode == 46) {
            if (ctl.value.indexOf(".", 0) >= 0)
                return false;
            else
                return true;
        }
        else
            return false;
    }
    else {
        return true;
    }
}

function ChkDblMinus(e, ctl) {
    var charCode = (e.which) ? e.which : event.keyCode
    if (charCode != 8 && charCode == 45 && (charCode < 48 || charCode > 57)) {
        if (charCode == 46) {
            if (ctl.value.indexOf(".", 0) >= 0)
                return false;
            else
                return true;
        }
        else if (charCode == 45) {
            //alert(ctl.value.indexOf("-", 1));
            if (ctl.value.indexOf("-", 0) >= 0)
                return false;
            else
                return true;
        }
        else
            return false;
    }
    else {
        return true;
    }
}

function valDbl(ctlz) {
    var temp = formatDbl(ctlz.value);
    temp = ClearMinus(temp);
    ctlz.value = AddComma(temp, ctlz.value.length - 3);
}

function valDblMinus(val) {
    var tempDblMinus = formatDbl(val);
    //temp = ClearMinus(temp);
    return AddComma(tempDblMinus, null);    
}
function ClearMinus(valIn) {
    temp = valIn;
    while (temp.indexOf("-", 0) != -1)
        temp = temp.replace("-", "");

    return temp;
}
function AddComma(x, posStart) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}
function prepareNum(ctlz) {
    ctlz.value = ClearComma(ctlz.value);
    ctlz.select();
}
function ClearComma(valIn) {
    temp = valIn;
    while (temp.indexOf(",", 0) != -1)
        temp = temp.replace(",", "");

    return temp;

}
function formatDbl(valIn) {
    var temp = valIn;
    if (temp.replace(" ", "") == '')
        return ''
    else {
        //alert(temp);
        temp = temp.replace(/,/g, "")
        if (isNaN(parseFloat(temp))) {
            temp = 0;
        }
        var temp = "" + Math.round(parseFloat(temp) * 100);
        if (temp == 0)
            return '0.00';
        else {
            if (parseFloat(temp) < 0) {
                temp = temp.substring(1, temp.length);
                var i = temp.length;
                while (i < 3) {
                    temp = "0" + temp;
                    i = i + 1;
                }
                i = i - 2;
                temp = "-" + temp.substring(0, i) + "." + temp.substring(i, temp.length);

            }
            else {
                var i = temp.length;
                while (i < 3) {
                    temp = "0" + temp;
                    i = i + 1;
                }
                i = i - 2;
                temp = temp.substring(0, i) + "." + temp.substring(i, temp.length);
            }
            return temp;
        }
    }
}
//Check Max Length for multiline textbox
function validateLimit(obj, divID, maxchar) {

    objDiv = document.getElementById(divID);

    if (this.id) obj = this;

    var remaningChar = maxchar - trimEnter(obj.value.trim()).length;

    if (remaningChar <= 0) {
        obj.value = obj.value.substring(maxchar, 0);
        if (objDiv.id) {
            objDiv.innerHTML = "Input reaches limit of " + maxchar + " characters";
        }
        return false;
    }
    else {
        objDiv.innerHTML = '';
        return true;
    }
}

function trimEnter(dataStr) {
    return dataStr.replace(/(\r\n|\r|\n)/g, "");
}
// --------------------------------------------

function ChkMultilineMaxLength(e, ctl, maxlength) {
    if (ctl.value.trim().length >= maxlength) {
        var charCode = (e.which) ? e.which : event.keyCode
        if (charCode != 8)
            return false;
        else
            return true;
    }
    else {
        return true;
    }
}

//Check Max Length for decimal textbox
//if pass, show number with format in textbox
//if fail, show error message in label
function valDbl2(textbox, labelId, errmsg, maxlength) {
    var val = textbox.value;

    if (val.indexOf(".") >= 0) {
        var vals = val.split('.');
        if (vals[0].trim().length == 0 && vals[1].trim().length == 0)
            textbox.value = '0.00';
        else if (vals[0].trim().length > maxlength)
            document.getElementById(labelId).innerHTML = errmsg;
        else {
            document.getElementById(labelId).innerHTML = '';
            textbox.value = formatDbl(textbox.value);
            textbox.value = ClearMinus(textbox.value);
            textbox.value = AddComma(textbox.value, textbox.value.length - 3);
        }
    }
    else {
        if (val.trim() == '') {
            textbox.value = '';
            document.getElementById(labelId).innerHTML = '';
        }
        else {
            if (val.length > maxlength)
                document.getElementById(labelId).innerHTML = errmsg;
            else {
                document.getElementById(labelId).innerHTML = '';
                textbox.value = formatDbl(textbox.value);
                textbox.value = ClearMinus(textbox.value);
                textbox.value = AddComma(textbox.value, textbox.value.length - 3);
            }
        }
    }
}

//Percent value for decimal textbox
//if pass, show number with format in textbox
//if fail, show error message in label
function valPercent(textbox, labelId, errmsg) {
    var val = textbox.value;

    if (val.indexOf(".") >= 0) {
        var vals = val.split('.');
        if (vals[0].trim().length == 0 && vals[1].trim().length == 0)
            textbox.value = '0.00';
        else if (parseFloat(val) > 100)
            document.getElementById(labelId).innerHTML = errmsg;
        else {
            document.getElementById(labelId).innerHTML = '';
            textbox.value = formatDbl(textbox.value);
            textbox.value = ClearMinus(textbox.value);
            textbox.value = AddComma(textbox.value, textbox.value.length - 3);
        }
    }
    else {
        if (val.trim() == '') {
            textbox.value = '';
            document.getElementById(labelId).innerHTML = '';
        }
        else {
            if (parseFloat(val) > 100)
                document.getElementById(labelId).innerHTML = errmsg;
            else {
                document.getElementById(labelId).innerHTML = '';
                textbox.value = formatDbl(textbox.value);
                textbox.value = ClearMinus(textbox.value);
                textbox.value = AddComma(textbox.value, textbox.value.length - 3);
            }
        }
    }
}

//(function ($) {
//    $.widget("custom.combobox", {
//        _create: function () {
//            this.wrapper = $("<span>")
//              .insertAfter(this.element);
//            this.element.hide();
//            this._createAutocomplete();
//            this._createShowAllButton();
//        },

//        _createAutocomplete: function () {
//            var selected = this.element.children(":selected"),
//              value = selected.val() ? selected.text() : "";
//            this.input = $("<input>")
//              .appendTo(this.wrapper)
//              .val(value)
//              .attr("title", "")
//              .addClass("Dropdownlist")
//              .width(180)
//              .autocomplete({
//                  delay: 0,
//                  minLength: 0,
//                  source: $.proxy(this, "_source")
//              })

//              .tooltip({
//                  tooltipClass: "ui-state-highlight"
//              });

//            this._on(this.input, {
//                autocompleteselect: function (event, ui) {
//                    ui.item.option.selected = true;
//                    this._trigger("select", event, {
//                        item: ui.item.option
//                    });
//                },

//                autocompletechange: "_removeIfInvalid"
//            });
//        },

//        _createShowAllButton: function () {
//            var input = this.input,
//              wasOpen = false;

//            $("<a>")
//              .attr("tabIndex", -1)
//              //.attr("title", "Show All Items")    //show tooltip
//              .tooltip()
//              .appendTo(this.wrapper)
//              .button({
//                  icons: {
//                      //primary: "ui-icon-triangle-1-s"
//                  },
//                  text: false
//              })
//              .height(18)
//              .width(18)
//              .removeClass("ui-corner-all")
//              .addClass("AutoDropdownlist-toggle")
//              .mousedown(function () {
//                  wasOpen = input.autocomplete("widget").is(":visible");
//              })
//              .click(function () {
//                  input.focus();

//                  // Close if already visible
//                  if (wasOpen) {
//                      return;
//                  }

//                  // Pass empty string as value to search for, displaying all results
//                  input.autocomplete("search", "");
//              });
//        },

//        _source: function (request, response) {
//            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
//            response(this.element.children("option").map(function () {
//                var text = $(this).text();
//                if (this.value && (!request.term || matcher.test(text)))
//                    return {
//                        label: text,
//                        value: text,
//                        option: this
//                    };
//            }));
//        },

//        _removeIfInvalid: function (event, ui) {

//            // Selected an item, nothing to do
//            if (ui.item) {
//                return;
//            }

//            // Search for a match (case-insensitive)
//            var value = this.input.val(),
//              valueLowerCase = value.toLowerCase(),
//              valid = false;
//            this.element.children("option").each(function () {
//                if ($(this).text().toLowerCase() === valueLowerCase) {
//                    this.selected = valid = true;
//                    return false;
//                }
//            });

//            // Found a match, nothing to do
//            if (valid) {
//                return;
//            }

//            // Remove invalid value
//            this.input
//              .val("")
//              .attr("title", value + " didn't match any item")
//              .tooltip("open");
//            this.element.val("");
//            this._delay(function () {
//                this.input.tooltip("close").attr("title", "");
//            }, 2500);
//            this.input.autocomplete("instance").term = "";
//        },

//        _destroy: function () {
//            this.wrapper.remove();
//            this.element.show();
//        }
//    });
//})(jQuery);

(function ($) {
    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
              .prop('id', this.element.attr('id') + '_span')
              .insertAfter(this.element);
            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";
            this.input = $("<input>")
              .prop('id', this.element.attr('id') + '_input')
              .prop('disabled', this.element.attr('disabled'))
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .addClass("Dropdownlist")
              .width(180)
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })

              .tooltip({
                  tooltipClass: "ui-state-highlight"
              });
            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
              wasOpen = false;

            $("<a>")
              .prop('id', this.element.attr('id') + '_selector')
              .prop('disabled', this.element.attr('disabled'))
              .attr("tabIndex", -1)
            //.attr("title", "Show All Items")    //show tooltip
              .tooltip()
              .appendTo(this.wrapper)
              .button({
                  icons: {
                      //primary: "ui-icon-triangle-1-s"
                  },
                  text: false
              })
              .height(18)
              .width(18)
              .removeClass("ui-corner-all")
              .addClass("AutoDropdownlist-toggle")
              .mousedown(function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .click(function () {
                  input.focus();

                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }

                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");
              });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
            if (this.element.val() != "" && this.input.val() == "") {
                this.element.children("option").each(function () { this.selected = false; });
                this.input.blur();
                this._trigger("cleared");
            }

        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                } else this.selected = false;
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
              .val("")
              .attr("title", value + " didn't match any item")
              .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
            this._trigger("notfound", event);
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);

function DoPostBack(obj) {
    __doPostBack(obj.id, 'OtherInformation');
}