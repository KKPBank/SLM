<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabActRenewInsure.ascx.cs"
    Inherits="SLM.Application.Shared.Obt.TabActRenewInsure" %>
<%@ Register Src="../GridviewPageController.ascx" TagName="GridviewPageController"
    TagPrefix="uc1" %>
<%@ Register Src="../TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc2" %>
<%@ Register Src="../TextDateMaskWithEvent.ascx" TagName="TextDateMaskWithEvent"
    TagPrefix="uc5" %>
<%@ Register Src="ActRenewInsureSnap.ascx" TagName="ActRenewInsureSnap" TagPrefix="uc3" %>
<%@ Register Src="ActRenewInsureContact.ascx" TagName="ActRenewInsureContract" TagPrefix="uc4" %>
<style type="text/css">
    #ContentPlaceHolder1_tabMain_tab008_tabActRenewInsure_tabRenewInsuranceContainer_body {
        border-bottom: 0;
        border-left: 0;
        border-right: 0;
        display: none !important;
    }
    
    .divRenewInsurance {
        background-color: #ffffff;
        border-width: 1px;
        border-style: solid;
        border-color: Gray;
        padding: 3px;
        width: 1160px;
        border-top: 0;
    }
    
    .ColIndent {
        width: 15px;
    }
    
    .ColIndent2 {
        width: 36px;
    }
    
    .ColInfo1 {
        font-weight: bold;
        width: 150px;
    }
    
    .ColInfo2 {
        font-weight: bold;
        width: 190px;
    }
    
    .ColInput {
        width: 240px;
    }
    
    .MsgAlert {
        font-family: Tahoma;
        font-size: 9pt;
        color: Red;
    }
    
    td.solid_boder {
        border: 1px solid black;
    }
    
    .t_rowhead {
        font-weight: bold;
        color: #000000;
        background-color: #b3c8e7;
        width: 150px;
        border: 1px solid black;
    }
    
    .cancel {
        display: relative;
        cursor: pointer;
        margin: 0;
        float: right;
        height: 10px;
        width: 14px;
        padding: 0 0 5px 0;
        background-color: red;
        text-align: center;
        font-weight: bold;
        font-size: 11px;
        color: white;
        border-radius: 3px;
        z-index: 100000000000000000;
    }
    
        .cancel:hover {
        background: rgb(255,50,50);
    }
    
    .auto-style2 {
        height: 29px;
    }
    
    .tabStyle .ajax__tab_body {
        border-bottom: 0px;
    }

    .normal_row {
        min-height: 27px;
    }

    .normal_table {
        border: 1px solid gray;
        border-collapse: collapse;
    }
</style>
<div style="font-family: Tahoma; font-size: 13px;">
    <%--<script language="javascript" type="text/javascript" src="../Scripts/moment.js"></script>--%>
    <script language="javascript" type="text/javascript">

        function ispolicychecked() {
            var chk_c = document.getElementById('<%= rbInsNameTh_cur.ClientID %>');
            var chk_p1 = document.getElementById('<%= rbInsNameTh_pro1.ClientID %>');
            var chk_p2 = document.getElementById('<%= rbInsNameTh_pro2.ClientID %>');
            var chk_p3 = document.getElementById('<%= rbInsNameTh_pro3.ClientID %>');

            if (chk_c != null) {
                return chk_c.checked || chk_p1.checked || chk_p2.checked || chk_p3.checked;
            }
            else {
                return false;
            }

        }

        function isactchecked() {
            var chk_p1 = document.getElementById('<%= rbAct_pro1.ClientID %>');
            var chk_p2 = document.getElementById('<%= rbAct_pro2.ClientID %>');
            var chk_p3 = document.getElementById('<%= rbAct_pro3.ClientID %>');

            return chk_p1.checked || chk_p2.checked || chk_p3.checked;
        }

        function blockButton() {
            //$("[id$=HiddenBtn]").click();
        }

        function blockActButton() {
            $("[id$=HiddenActBtn]").click();
            //document.getElementById('HiddenActBtn').click();
        }

        function blockPaymentButton() {
            $("[id$=HiddenPaymentBtn]").click();
            //document.getElementById('HiddenPaymentBtn').click();
        }

        function blockReceiptButton() {
            $("[id$=HiddenReceiptBtn]").click();
            //document.getElementById('HiddenReceiptBtn').click();
        }

        //        var lasttab = 0;
        //        function tabChanged(sender, args) {
        //            // do what ever i want with lastTab value
        //            lasttab = sender.get_activeTabIndex();
        //            $("[id*=hidLasttab]").val(lasttab);
        //        }
        function confirmCancelPolicy() {
            if (confirm("ต้องการยกเลิกประกัน?")) {
                DisplayProcessing(); return true;
            } else {
                return false;
            }
        }

        function confirmCancelAct() {
            if (confirm("ต้องการยกเลิก พรบ.?")) {
                DisplayProcessing(); return true;
            } else {
                return false;
            }
        }

        function dayDiff(date1, date2) {
            if (date1 == "") {
                return 0;
            }
            else {
                var getDate1 = date1.split("/");
                var setDate1 = moment(getDate1[2] + "-" + getDate1[1] + "-" + getDate1[0]);

                var getDate2 = date2.split("/");
                var setDate2 = moment(getDate2[2] + "-" + getDate2[1] + "-" + getDate2[0]);

                //alert(setDate1 + ' ' + setDate2);
                var diff = setDate2.diff(setDate1, 'days');
                return diff;
            }
        }

        function save004() {
            var warning = "";
            var policyWarning = "กรุณาตรวจสอบ วันที่เริ่มต้นการคุ้มครองของประกันน้อยกว่าวันที่ปัจจุบัน";
            var actWarning = "กรุณาตรวจสอบ วันที่เริ่มต้นการคุ้มครองของ พรบ. น้อยกว่าวันที่ปัจจุบัน";
            var bothWarning = "กรุณาตรวจสอบ วันที่เริ่มต้นการคุ้มครองของประกันหรือ พรบ.น้อยกว่าวันที่ปัจจุบัน";

            var policySelect = false, policyDateAlert = false, actSelect = false, actDateAlert = false;
            if ($("[id$=rbInsNameTh_cur]").attr('checked') == 'checked' && $("[id$=lblVoluntary_Policy_Eff_Date_cur_txtDate]").attr('class') == "Textbox")
            {
                policySelect = true;
                if (dayDiff($("[id$=lblVoluntary_Policy_Eff_Date_cur_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
                    policyDateAlert = true;
                }
            }
            else if ($("[id$=rbInsNameTh_pro1]").attr('checked') == 'checked' && $("[id$=lblVoluntary_Policy_Eff_Date_pro1_txtDate]").attr('class') == "Textbox") {
                policySelect = true;
                if (dayDiff($("[id$=lblVoluntary_Policy_Eff_Date_pro1_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
                    policyDateAlert = true;
                }
            }
            else if ($("[id$=rbInsNameTh_pro2]").attr('checked') == 'checked' && $("[id$=lblVoluntary_Policy_Eff_Date_pro2_txtDate]").attr('class') == "Textbox") {
                policySelect = true;
                if (dayDiff($("[id$=lblVoluntary_Policy_Eff_Date_pro2_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
                    policyDateAlert = true;
                }
            }
            else if ($("[id$=rbInsNameTh_pro3]").attr('checked') == 'checked' && $("[id$=lblVoluntary_Policy_Eff_Date_pro3_txtDate]").attr('class') == "Textbox") {
                //actSelect = true;
                policySelect = true;
                if (dayDiff($("[id$=lblVoluntary_Policy_Eff_Date_pro3_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
                    policyDateAlert = true;
                }
            }

            //if ($("[id$=rbAct_pro1]").attr('checked') == 'checked' && $("[id$=txtActStartCoverDateAct_pro1_txtDate]").attr('class') == "Textbox") {
            //    actSelect = true;
            //    if (dayDiff($("[id$=txtActStartCoverDateAct_pro1_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
            //        actDateAlert = true;
            //    }
            //}
            //else if ($("[id$=rbAct_pro2]").attr('checked') == 'checked' && $("[id$=txtActStartCoverDateAct_pro2_txtDate]").attr('class') == "Textbox") {
            //    actSelect = true;
            //    if (dayDiff($("[id$=txtActStartCoverDateAct_pro2_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
            //        actDateAlert = true;
            //    }
            //}
            //else if ($("[id$=rbAct_pro3]").attr('checked') == 'checked' && $("[id$=txtActStartCoverDateAct_pro3_txtDate]").attr('class') == "Textbox") {
            //    if (dayDiff($("[id$=txtActStartCoverDateAct_pro3_txtDate]").val(), moment().format("DD/MM/YYYY")) > 0) {
            //        actDateAlert = true;
            //    }
            //}

            if (policySelect && actSelect && (policyDateAlert || actDateAlert))
            {
                warning = bothWarning;
            }
            else if (policySelect && policyDateAlert)
            {
                warning = policyWarning;
            }
            else if (actSelect && actDateAlert)
            {
                warning = actWarning;
            }

            if (warning != "")
                warning += "\r\n";

            if (confirm(warning + "ต้องการบันทึกใช่หรือไม่?")) {
                if ($("[id$=hidTime]").val() == "") {
                    $("[id$=hidTime]").val('16:00');
                }
                var ActPurchaseTime = $("[id$=hidTime]").val();
                var chour = moment().hour();
                var cminute = moment().minute();

                var res = ActPurchaseTime.split(":");

                var ctime = moment().format("DD/MM/YYYY");

                if (chour > res[0] || (chour == res[0] && cminute > res[1])) {

                    DisplayProcessing(); return true;
                    //if ($("[id$=rbAct_pro1]").attr('checked') == 'checked') {
                    //    if ($("[id$=txtActStartCoverDateAct_pro1_txtDate]").val() == ctime) {
                    //        if (confirm('ต้องการซื้อ พรบ หลังเวลา ' + ActPurchaseTime + ' ใช่หรือไม่')) {
                    //            DisplayProcessing(); return true;
                    //        } else {
                    //            return false;
                    //        }
                    //    } else {
                    //        DisplayProcessing(); return true;
                    //    }
                    //} else if ($("[id$=rbAct_pro2]").attr('checked') == 'checked') {
                    //    if ($("[id$=txtActStartCoverDateAct_pro2_txtDate]").val() == ctime) {
                    //        if (confirm('ต้องการซื้อ พรบ หลังเวลา ' + ActPurchaseTime + ' ใช่หรือไม่')) {
                    //            DisplayProcessing(); return true;
                    //        } else {
                    //            return false;
                    //        }
                    //    } else {
                    //        DisplayProcessing(); return true;
                    //    }
                    //} else if ($("[id$=rbAct_pro3]").attr('checked') == 'checked') {
                    //    if ($("[id$=txtActStartCoverDateAct_pro3_txtDate]").val() == ctime) {
                    //        if (confirm('ต้องการซื้อ พรบ หลังเวลา ' + ActPurchaseTime + ' ใช่หรือไม่')) {
                    //            DisplayProcessing(); return true;
                    //        } else {
                    //            return false;
                    //        }
                    //    } else {
                    //        DisplayProcessing(); return true;
                    //    }
                    //} else {
                    //    DisplayProcessing(); return true;

                    //}
                } else {
                    DisplayProcessing(); return true;
                }
            } else {
                return false;
            }
        }


        function editReceipt(obj, recNo) {

            if ($(obj).closest("td").prev().find('select').val() == "") {
                if (confirm('ต้องการลบข้อมูลแก้ไขใบเสร็จใช่หรือไม่')) {
                    DisplayProcessing();
                    return true;
                }
            } else {
                if (confirm('ต้องการแก้ไขใบเสร็จ ' + $(obj).attr('class') + ' ใช่หรือไม่')) {
                    DisplayProcessing();
                    return true;
                }
            }

            return false;
        }

        function doToggle() {

            var pnAdvanceSearch = document.getElementById('<%=pnAdvanceSearch.ClientID%>');
            var lbAdvanceSearch = document.getElementById('<%=lbAdvanceSearch.ClientID%>');
            var txtAdvanceSearch = document.getElementById('<%=txtAdvanceSearch.ClientID%>');

            if (pnAdvanceSearch.style.display == '' || pnAdvanceSearch.style.display == 'none') {
                lbAdvanceSearch.innerHTML = "[-] <b>ข้อมูลประกันภัย</b>";
                pnAdvanceSearch.style.display = 'block';
                txtAdvanceSearch.value = "Y";
            }
            else {
                lbAdvanceSearch.innerHTML = "[-] <b>ข้อมูลประกันภัย</b>";
                pnAdvanceSearch.style.display = 'none';
                txtAdvanceSearch.value = "N";
            }

            var pnPromotion = document.getElementById('<%=pnPromotion.ClientID%>');
            var detailPromotion = document.getElementById('<%=detailPromotion.ClientID%>');
            var txtPromotion = document.getElementById('<%=txtPromotion.ClientID%>');

            if (pnPromotion.style.display == '' || pnPromotion.style.display == 'none') {
                detailPromotion.innerHTML = "[-] <b>ข้อมูลโปรโมชั่นประกันภัย</b>";
                pnPromotion.style.display = 'block';
                txtPromotion.value = "Y";
            }
            else {
                detailPromotion.innerHTML = "[-] <b>ข้อมูลโปรโมชั่นประกันภัย</b>";
                pnPromotion.style.display = 'none';
                txtPromotion.value = "N";
            }

            var pnSurety = document.getElementById('<%=pnSurety.ClientID%>');
            var surety = document.getElementById('<%=surety.ClientID%>');
            var txtSurety = document.getElementById('<%=txtSurety.ClientID%>');

            if (pnSurety.style.display == '' || pnSurety.style.display == 'none') {
                surety.innerHTML = "[-] <b>ข้อมูลผู้ค้ำ</b>";
                pnSurety.style.display = 'block';
                txtSurety.value = "Y";
            }
            else {
                surety.innerHTML = "[-] <b>ข้อมูลผู้ค้ำ</b>";
                pnSurety.style.display = 'none';
                txtSurety.value = "N";
            }

            var pnLegislation = document.getElementById('<%=pnLegislation.ClientID%>');
            var legislation = document.getElementById('<%=legislation.ClientID%>');
            var txtLegislation = document.getElementById('<%=txtLegislation.ClientID%>');

            if (pnLegislation.style.display == '' || pnLegislation.style.display == 'none') {
                legislation.innerHTML = "[-] <b>ข้อมูล พรบ.</b>";
                pnLegislation.style.display = 'block';
                txtLegislation.value = "Y";
            }
            else {
                legislation.innerHTML = "[-] <b>ข้อมูล พรบ.</b>";
                pnLegislation.style.display = 'none';
                txtLegislation.value = "N";
            }

            var pnSendDocument = document.getElementById('<%=pnSendDocument.ClientID%>');
            var sendDocument = document.getElementById('<%=sendDocument.ClientID%>');
            var txtSendDocument = document.getElementById('<%=txtSendDocument.ClientID%>');

            if (pnSendDocument.style.display == '' || pnSendDocument.style.display == 'none') {
                sendDocument.innerHTML = "[-] <b>ที่อยู่ในการจัดส่งเอกสาร</b>";
                pnSendDocument.style.display = 'block';
                txtSendDocument.value = "Y";
            }
            else {
                sendDocument.innerHTML = "[-] <b>ที่อยู่ในการจัดส่งเอกสาร</b>";
                pnSendDocument.style.display = 'none';
                txtSendDocument.value = "N";
            }

            var pnNoIncentive = document.getElementById('<%=pnNoIncentive.ClientID%>');
            var noIncentive = document.getElementById('<%=noIncentive.ClientID%>');
            var txtNoIncentive = document.getElementById('<%=txtNoIncentive.ClientID%>');

            if (pnNoIncentive.style.display == '' || pnNoIncentive.style.display == 'none') {
                noIncentive.innerHTML = "[-] <b>เลขที่รับแจ้งและ Incentive</b>";
                pnNoIncentive.style.display = 'block';
                txtNoIncentive.value = "Y";
            }
            else {
                noIncentive.innerHTML = "[-] <b>เลขที่รับแจ้งและ Incentive</b>";
                pnNoIncentive.style.display = 'none';
                txtNoIncentive.value = "N";
            }


        }

        function pageLoad() {
            $(document).ready(function () {

                //debugger;
                if ($("[id$=hidYear_pre]").val() == 'N') {
                    $("[id*=_pre]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=_pre]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                if ($("[id$=hidYear_cur]").val() == 'N') {
                    $("[id*=_cur]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=_cur]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }



                if ($("[id$=hidPromotionId1]").val() == '') {
                    $("[id*=divPolicy] [id*=pro1]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=pro1]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                if ($("[id$=hidPromotionId2]").val() == '') {
                    $("[id*=divPolicy] [id*=pro2]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=pro2]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                if ($("[id$=hidPromotionId3]").val() == '') {
                    $("[id*=divPolicy] [id*=pro3]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                }
                else {
                    //                    $("[id*=pro3]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }


                if ($("[id$=hidPromotionActId1]").val() == '') {
                    $("[id*=pnLegislation] [id*=pro1]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=pro1]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                if ($("[id$=hidPromotionActId2]").val() == '') {
                    $("[id*=pnLegislation] [id*=pro2]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                } else {
                    //                    $("[id*=pro2]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                if ($("[id$=hidPromotionActId3]").val() == '') {
                    $("[id*=pnLegislation] [id*=pro3]").each(function (i, o) {
                        $(o).parents('td:first').hide();
                    });
                }
                else {
                    //                    $("[id*=pro3]").each(function (i, o) {
                    //                        $(o).parents('td:first').show()
                    //                    });
                }
                //                if ($("[id$=hidPromotionId4]").val() == '') {
                //                    $("[id*=_bla]").each(function (i, o) {
                //                        $(o).parents('td:first').hide()
                //                    });
                //                } else {
                //                    //                    $("[id*=_bla]").each(function (i, o) {
                //                    //                        $(o).parents('td:first').show() 
                //                    //                    });
                //                }

                //                if ($("[id$=ModalPopupHistoryDetail_foregroundElement]").css( "display") != "none")
                //                {
                if ($("[id$=hidHistId1]").val() == '') {
                    $('#tbHistory td:nth-child(' + (2) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (2) + ')').show();
                }

                if ($("[id$=hidHistId2]").val() == '') {
                    $('#tbHistory td:nth-child(' + (3) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (3) + ')').show();
                }

                if ($("[id$=hidHistId3]").val() == '') {
                    $('#tbHistory td:nth-child(' + (4) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (4) + ')').show();
                }

                if ($("[id$=hidHistId4]").val() == '') {
                    $('#tbHistory td:nth-child(' + (5) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (5) + ')').show();
                }

                if ($("[id$=hidHistId5]").val() == '') {
                    $('#tbHistory td:nth-child(' + (6) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (6) + ')').show();
                }

                if ($("[id$=hidHistId6]").val() == '') {
                    $('#tbHistory td:nth-child(' + (7) + ')').hide();
                } else {
                    //                    $('#tbHistory td:nth-child(' + (7) + ')').show();
                }

                //======for Act History========
                if ($("[id$=hdActHist1]").val() == '') {
                    //                    $('#tbDtlAct td:nth-child(' + (2) + ')').hide();
                } else {
                    //                    $('#tbDtlAct td:nth-child(' + (2) + ')').show();
                }

                if ($("[id$=hdActHist2]").val() == '') {
                    $('#tbDtlAct td:nth-child(' + (3) + ')').hide();
                } else {
                    //                    $('#tbDtlAct td:nth-child(' + (3) + ')').show();
                }

                if ($("[id$=hdActHist3]").val() == '') {
                    $('#tbDtlAct td:nth-child(' + (4) + ')').hide();
                } else {
                    //                    $('#tbDtlAct td:nth-child(' + (4) + ')').show();
                }

                if ($("[id$=hdActHist4]").val() == '') {
                    $('#tbDtlAct td:nth-child(' + (5) + ')').hide();
                } else {
                    //                    $('#tbDtlAct td:nth-child(' + (5) + ')').show();
                }

                if ($("[id$=hdActHist5]").val() == '') {
                    $('#tbDtlAct td:nth-child(' + (6) + ')').hide();
                } else {
                    //                    $('#tbDtlAct td:nth-child(' + (6) + ')').show();
                }
                //}

                $(".int").keypress(function (e) {
                    return ChkIntMinus(event);
                });

                $(".money").keypress(function (e) {
                    return ChkDblMinus(e, e.target);
                });

                $(".money").focus(function (e) {
                    prepareNum(e.target);
                    return false;
                });

                $(".money").on("blur", function (e) {
                    $(e.target).val(valDblMinus($(e.target).val()));
                    return true;
                });

                $("#<%=txtBPPopup_PolicyAmountSpecify1.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify2.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify3.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify4.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify5.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify6.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify7.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify8.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify9.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_PolicyAmountSpecify10.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
                $("#<%=txtBPPopup_ActAmountSpecify.ClientID%>").on("blur", function () {
                    if ($(this).val() == "") {
                        $(this).val("0.00");
                    }
                });
            });
        }

        function TelNo1ChangePrelead() {
            var textboxSms = document.getElementById('<%= txtTelNoSms_PreleadPopup.ClientID %>');
            var textboxTelNo1 = document.getElementById('<%= txtTelNo1_PreleadPopup.ClientID %>');

            if (textboxSms.value.trim() == '') {
                textboxSms.value = textboxTelNo1.value.trim();
            }
        }

        function TelNoSmsChangePrelead() {
            var textboxSms = document.getElementById('<%= txtTelNoSms_PreleadPopup.ClientID %>');
            var alertSms = document.getElementById('<%= vTelNoSms_PreleadPopup.ClientID %>');
            if (textboxSms.value.trim() == '') {
                textboxSms.value = document.getElementById('<%= txtTelNo1_PreleadPopup.ClientID %>').value;
                alertSms.innerHTML = '';
            }
        }

        function SetPolicyAmount() {
            var cmbPaymentmethod = $('#<%= cmbPaymentmethod.ClientID %>');
            var txtBPPopup_PolicyAmountDue = $('#<%= txtBPPopup_PolicyAmountDue.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify1 = $('#<%= txtBPPopup_PolicyAmountSpecify1.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify2 = $('#<%= txtBPPopup_PolicyAmountSpecify2.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify3 = $('#<%= txtBPPopup_PolicyAmountSpecify3.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify4 = $('#<%= txtBPPopup_PolicyAmountSpecify4.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify5 = $('#<%= txtBPPopup_PolicyAmountSpecify5.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify6 = $('#<%= txtBPPopup_PolicyAmountSpecify6.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify7 = $('#<%= txtBPPopup_PolicyAmountSpecify7.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify8 = $('#<%= txtBPPopup_PolicyAmountSpecify8.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify9 = $('#<%= txtBPPopup_PolicyAmountSpecify9.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify10 = $('#<%= txtBPPopup_PolicyAmountSpecify10.ClientID %>');
            var txtPeriod1 = $('#<%= txtPeriod1.ClientID %>');
            var txtPeriod2 = $('#<%= txtPeriod2.ClientID %>');
            var txtPeriod3 = $('#<%= txtPeriod3.ClientID %>');
            var txtPeriod4 = $('#<%= txtPeriod4.ClientID %>');
            var txtPeriod5 = $('#<%= txtPeriod5.ClientID %>');
            var txtPeriod6 = $('#<%= txtPeriod6.ClientID %>');
            var txtPeriod7 = $('#<%= txtPeriod7.ClientID %>');
            var txtPeriod8 = $('#<%= txtPeriod8.ClientID %>');
            var txtPeriod9 = $('#<%= txtPeriod9.ClientID %>');
            var txtPeriod10 = $('#<%= txtPeriod10.ClientID %>');

            if (cmbPaymentmethod.val() == "1" || cmbPaymentmethod.val() == "") {
                //ชำระเต็ม
                txtBPPopup_PolicyAmountSpecify1.val(txtBPPopup_PolicyAmountDue.val());
                txtBPPopup_PolicyAmountSpecify1.prop("disabled", false);
            }
            else if (cmbPaymentmethod.val() == "2") {
                //ชำระแบบผ่อน
                if (txtPeriod1 != null && txtBPPopup_PolicyAmountSpecify1 != null) {
                    txtBPPopup_PolicyAmountSpecify1.val(txtPeriod1.val());
                    txtBPPopup_PolicyAmountSpecify1.prop("disabled", false);
                }
                if (txtPeriod2 != null && txtBPPopup_PolicyAmountSpecify2 != null) {
                    txtBPPopup_PolicyAmountSpecify2.val(txtPeriod2.val());
                    txtBPPopup_PolicyAmountSpecify2.prop("disabled", false);
                }
                if (txtPeriod3 != null && txtBPPopup_PolicyAmountSpecify3 != null) {
                    txtBPPopup_PolicyAmountSpecify3.val(txtPeriod3.val());
                    txtBPPopup_PolicyAmountSpecify3.prop("disabled", false);
                }
                if (txtPeriod4 != null && txtBPPopup_PolicyAmountSpecify4 != null) {
                    txtBPPopup_PolicyAmountSpecify4.val(txtPeriod4.val());
                    txtBPPopup_PolicyAmountSpecify4.prop("disabled", false);
                }
                if (txtPeriod5 != null && txtBPPopup_PolicyAmountSpecify5 != null) {
                    txtBPPopup_PolicyAmountSpecify5.val(txtPeriod5.val());
                    txtBPPopup_PolicyAmountSpecify5.prop("disabled", false);
                }
                if (txtPeriod6 != null && txtBPPopup_PolicyAmountSpecify6 != null) {
                    txtBPPopup_PolicyAmountSpecify6.val(txtPeriod6.val());
                    txtBPPopup_PolicyAmountSpecify6.prop("disabled", false);
                }
                if (txtPeriod7 != null && txtBPPopup_PolicyAmountSpecify7 != null) {
                    txtBPPopup_PolicyAmountSpecify7.val(txtPeriod7.val());
                    txtBPPopup_PolicyAmountSpecify7.prop("disabled", false);
                }
                if (txtPeriod8 != null && txtBPPopup_PolicyAmountSpecify8 != null) {
                    txtBPPopup_PolicyAmountSpecify8.val(txtPeriod8.val());
                    txtBPPopup_PolicyAmountSpecify8.prop("disabled", false);
                }
                if (txtPeriod9 != null && txtBPPopup_PolicyAmountSpecify9 != null) {
                    txtBPPopup_PolicyAmountSpecify9.val(txtPeriod9.val());
                    txtBPPopup_PolicyAmountSpecify9.prop("disabled", false);
                }
                if (txtPeriod10 != null && txtBPPopup_PolicyAmountSpecify10 != null) {
                    txtBPPopup_PolicyAmountSpecify10.val(txtPeriod10.val());
                    txtBPPopup_PolicyAmountSpecify10.prop("disabled", false);
                }
            }
        }

        function ClearPolicyAmount() {
            var cmbPaymentmethod = $('#<%= cmbPaymentmethod.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify1 = $('#<%= txtBPPopup_PolicyAmountSpecify1.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify2 = $('#<%= txtBPPopup_PolicyAmountSpecify2.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify3 = $('#<%= txtBPPopup_PolicyAmountSpecify3.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify4 = $('#<%= txtBPPopup_PolicyAmountSpecify4.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify5 = $('#<%= txtBPPopup_PolicyAmountSpecify5.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify6 = $('#<%= txtBPPopup_PolicyAmountSpecify6.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify7 = $('#<%= txtBPPopup_PolicyAmountSpecify7.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify8 = $('#<%= txtBPPopup_PolicyAmountSpecify8.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify9 = $('#<%= txtBPPopup_PolicyAmountSpecify9.ClientID %>');
            var txtBPPopup_PolicyAmountSpecify10 = $('#<%= txtBPPopup_PolicyAmountSpecify10.ClientID %>');
            var txtPeriod1 = $('#<%= txtPeriod1.ClientID %>');
            var txtPeriod2 = $('#<%= txtPeriod2.ClientID %>');
            var txtPeriod3 = $('#<%= txtPeriod3.ClientID %>');
            var txtPeriod4 = $('#<%= txtPeriod4.ClientID %>');
            var txtPeriod5 = $('#<%= txtPeriod5.ClientID %>');
            var txtPeriod6 = $('#<%= txtPeriod6.ClientID %>');
            var txtPeriod7 = $('#<%= txtPeriod7.ClientID %>');
            var txtPeriod8 = $('#<%= txtPeriod8.ClientID %>');
            var txtPeriod9 = $('#<%= txtPeriod9.ClientID %>');
            var txtPeriod10 = $('#<%= txtPeriod10.ClientID %>');

            if (txtBPPopup_PolicyAmountSpecify1 != null) {
                txtBPPopup_PolicyAmountSpecify1.val("");
                txtBPPopup_PolicyAmountSpecify1.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify2 != null) {
                txtBPPopup_PolicyAmountSpecify2.val("");
                txtBPPopup_PolicyAmountSpecify2.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify3 != null) {
                txtBPPopup_PolicyAmountSpecify3.val("");
                txtBPPopup_PolicyAmountSpecify3.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify4 != null) {
                txtBPPopup_PolicyAmountSpecify4.val("");
                txtBPPopup_PolicyAmountSpecify4.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify5 != null) {
                txtBPPopup_PolicyAmountSpecify5.val("");
                txtBPPopup_PolicyAmountSpecify5.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify6 != null) {
                txtBPPopup_PolicyAmountSpecify6.val("");
                txtBPPopup_PolicyAmountSpecify6.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify7 != null) {
                txtBPPopup_PolicyAmountSpecify7.val("");
                txtBPPopup_PolicyAmountSpecify7.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify8 != null) {
                txtBPPopup_PolicyAmountSpecify8.val("");
                txtBPPopup_PolicyAmountSpecify8.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify9 != null) {
                txtBPPopup_PolicyAmountSpecify9.val("");
                txtBPPopup_PolicyAmountSpecify9.prop("disabled", true);
            }
            if (txtBPPopup_PolicyAmountSpecify10 != null) {
                txtBPPopup_PolicyAmountSpecify10.val("");
                txtBPPopup_PolicyAmountSpecify10.prop("disabled", true);
            }
        }

        function BPReportCheckChange() {
            var chkBP = $('#<%= chkBP.ClientID %>');
            var txtBPPopup_PolicyAmountDue = $('#<%= txtBPPopup_PolicyAmountDue.ClientID %>');
            var txtBPPopup_ActAmountDue = $('#<%= txtBPPopup_ActAmountDue.ClientID %>');

            var cbBPPopup_PolicyAmountSpecify = $('#<%= cbBPPopup_PolicyAmountSpecify.ClientID %>');
            var cbBPPopup_ActAmountSpecify = $('#<%= cbBPPopup_ActAmountSpecify.ClientID %>');

            var txtBPPopup_ActAmountSpecify = $('#<%= txtBPPopup_ActAmountSpecify.ClientID %>');

            if (chkBP.is(":checked")) {
                if (txtBPPopup_PolicyAmountDue != null && txtBPPopup_PolicyAmountDue.val() != "") {
                    cbBPPopup_PolicyAmountSpecify.prop("checked", true);
                    cbBPPopup_PolicyAmountSpecify.prop("disabled", false);

                    SetPolicyAmount();
                }
                if (txtBPPopup_ActAmountDue != null && txtBPPopup_ActAmountDue.val() != "") {
                    cbBPPopup_ActAmountSpecify.prop("checked", true);
                    cbBPPopup_ActAmountSpecify.prop("disabled", false);
                    txtBPPopup_ActAmountSpecify.val(txtBPPopup_ActAmountDue.val());
                    txtBPPopup_ActAmountSpecify.prop("disabled", false);
                }
            }
            else {
                if (cbBPPopup_PolicyAmountSpecify != null) {
                    cbBPPopup_PolicyAmountSpecify.prop("checked", false);
                    cbBPPopup_PolicyAmountSpecify.prop("disabled", true);

                    ClearPolicyAmount();
                }
                if (cbBPPopup_ActAmountSpecify != null) {
                    cbBPPopup_ActAmountSpecify.prop("checked", false);
                    cbBPPopup_ActAmountSpecify.prop("disabled", true);
                    txtBPPopup_ActAmountSpecify.val("");
                    txtBPPopup_ActAmountSpecify.prop("disabled", true);
                }
            }
        }

        function CheckBoxBPSpecifyChange() {
            var chkBP = $('#<%= chkBP.ClientID %>');
            var txtBPPopup_PolicyAmountDue = document.getElementById('<%= txtBPPopup_PolicyAmountDue.ClientID %>');
            var txtBPPopup_ActAmountDue = $('#<%= txtBPPopup_ActAmountDue.ClientID %>');

            var cbBPPopup_PolicyAmountSpecify = $('#<%= cbBPPopup_PolicyAmountSpecify.ClientID %>');
            var cbBPPopup_ActAmountSpecify = $('#<%= cbBPPopup_ActAmountSpecify.ClientID %>');

            var txtBPPopup_ActAmountSpecify = $('#<%= txtBPPopup_ActAmountSpecify.ClientID %>');

            if (cbBPPopup_PolicyAmountSpecify != null) {
                if (cbBPPopup_PolicyAmountSpecify.is(":checked")) {
                    SetPolicyAmount();
                }
                else {
                    ClearPolicyAmount();
                }
            }

            if (cbBPPopup_ActAmountSpecify != null) {
                if (cbBPPopup_ActAmountSpecify.is(":checked")) {
                    txtBPPopup_ActAmountSpecify.val(txtBPPopup_ActAmountDue.val());
                    txtBPPopup_ActAmountSpecify.prop("disabled", false);
                }
                else {
                    txtBPPopup_ActAmountSpecify.val("");
                    txtBPPopup_ActAmountSpecify.prop("disabled", true);
                }
            }

            if (cbBPPopup_PolicyAmountSpecify != null && cbBPPopup_ActAmountSpecify != null) {
                if (cbBPPopup_PolicyAmountSpecify.is(":checked") == false && cbBPPopup_ActAmountSpecify.is(":checked") == false) {
                    cbBPPopup_PolicyAmountSpecify.prop("disabled", true);
                    cbBPPopup_ActAmountSpecify.prop("disabled", true);
                    chkBP.prop("checked", false);
                }
            }
            else if (cbBPPopup_PolicyAmountSpecify != null && cbBPPopup_PolicyAmountSpecify.is(":checked") == false) {
                cbBPPopup_PolicyAmountSpecify.prop("disabled", true);
                chkBP.prop("checked", false);
            }
            else if (cbBPPopup_ActAmountSpecify != null && cbBPPopup_ActAmountSpecify.checked == false) {
                cbBPPopup_ActAmountSpecify.disabled = true;
                chkBP.prop("checked", false);
            }
        }

    </script>
    <script type="text/javascript">
        function newBlankInsuranceForm() {

            document.getElementById('test').style.display = 'block';
            return true;
        }
        function closeBlankInsuranceForm() {
            document.getElementById('test').style.display = 'none';
        }
    </script>
    <!-- Prelead Popup Section -->
    <asp:UpdatePanel ID="upPreleadPopupMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPreleadPopup" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPreleadPopup" Style="display: none" CssClass="modalPopupTab008_Obt">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hFollow.gif" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:UpdatePanel ID="upPreleadPopup" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" border="0">
                            <tr>
                                <td colspan="5" style="height: 1px;"></td>
                            </tr>
                            <tr>
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">Contract No
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtContractNo_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                                <td class="ColInfo2">Ticket ID
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTicketId_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">ชื่อ
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtFirstname_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                                <td class="ColInfo2">นามสกุล
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtLastname_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">ประเภทบุคคล
                                </td>
                                <td class="ColInput">
                                    <asp:DropDownList ID="cmbCardType_PreleadPopup" runat="server" CssClass="Dropdownlist"
                                        Width="203px" AutoPostBack="true" OnSelectedIndexChanged="cmbCardType_PreleadPopup_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <br />
                                    <asp:Label ID="vtxtCardType_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                                </td>
                                <td class="ColInfo2">เลขที่บัตร<asp:Label ID="lblCitizenId_PreleadPopup" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtCitizenId_PreleadPopup" runat="server" CssClass="Textbox" Width="200px"
                                        Enabled="false" AutoPostBack="true" onblur="blockButton();" OnTextChanged="txtCitizenId_PreleadPopup_TextChanged"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="vtxtCitizenId_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">แคมเปญ
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtCampaignName_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                    <asp:TextBox ID="txtCampaignId_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtProductId_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtProductGroupId_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtProductName_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtProductGroupName_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtCarLicenseNo_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                </td>
                                <td class="ColInfo2"></td>
                                <td class="ColInput"></td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">Owner Branch
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtOwnerBranchName_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                                <td class="ColInfo2">Owner
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtOwnerName_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">หมายเลขโทรศัพท์<span style="color: Red">*</span>
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtTelNo1_PreleadPopup" runat="server" CssClass="Textbox" MaxLength="10" Width="200px" onblur="TelNo1ChangePrelead()" ></asp:TextBox>
                                    <br />
                                    <asp:Label ID="vtxtTelNo1_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                                </td>
                                <td class="ColInfo2">
                                    หมายเลข SMS
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtTelNoSms_PreleadPopup" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" onblur="TelNoSmsChangePrelead()"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="vTelNoSms_PreleadPopup" runat="server" CssClass="MsgAlert" ></asp:Label>
                                </td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">สถานะหลัก
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtStatusDesc_PreleadPopup" runat="server" CssClass="TextboxView"
                                        ReadOnly="true" Width="200px"></asp:TextBox>
                                    <asp:TextBox ID="txtStatusCode_PreleadPopup" runat="server" Width="40px" Visible="false"></asp:TextBox>
                                </td>
                                <td class="ColInfo2">สถานะย่อย<span style="color: Red">*</span>
                                </td>
                                <td class="ColInput">
                                    <asp:DropDownList ID="cmbSubStatus_PreleadPopup" runat="server" Width="203px" CssClass="Dropdownlist">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtSubStatusCodeCurrent_PreleadPopup" runat="server" Width="50px"
                                        Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtSubStatusNameCurrent_PreleadPopup" runat="server" Width="50px"
                                        Visible="false"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="vcmbSubStatus_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                                </td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1">วันที่นัดหมายครั้งถัดไป
                                </td>
                                <td class="ColInput">
                                    <uc2:TextDateMask ID="tdmNextContactDate_PreleadPopup" runat="server" />
                                </td>
                                <td class="ColInfo2">หมายเลขโทรศัพท์ที่ติดต่อลูกค้า
                                </td>
                                <td class="ColInput">
                                    <asp:TextBox ID="txtContactPhone_PreleadPopup" runat="server" CssClass="Textbox"
                                        Width="200px" MaxLength="10"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="vertical-align: top;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1"></td>
                                <td class="ColInput">
                                    <asp:CheckBox ID="cbBlacklist" runat="server" Text="Blacklist" ForeColor="Red" />
                                    <asp:HiddenField ID="hdBlacklistId" runat="server" />
                                </td>
                                <td class="ColInfo2"></td>
                                <td class="ColInput"></td>
                            </tr>
                            <tr>
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1" valign="top">รายละเอียดเพิ่มเติม<span style="color: Red">*</span>
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtContactDetail_PreleadPopup" runat="server" CssClass="Textbox"
                                        TextMode="MultiLine" Rows="5" Width="638px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="height: 35px;">
                                <td class="ColIndent2"></td>
                                <td class="ColInfo1" valign="top"></td>
                                <td colspan="2" style="vertical-align: top;">
                                    <asp:Label ID="vtxtContactDetail_PreleadPopup" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                                <td class="ColInput">
                                    <asp:Button ID="btnSavepop_PreleadPopup" runat="server" Text="บันทึก" Width="98px"
                                        OnClick="btnSavepop_PreleadPopup_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                                    <asp:Button ID="btnCancel_PreleadPopup" runat="server" Text="ยกเลิก" Width="98px"
                                        OnClick="btnCancel_PreleadPopup_Click" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่?')" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePreleadPopup" runat="server" TargetControlID="btnPreleadPopup"
                PopupControlID="pnPreleadPopup" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- End Section Prelead Popup -->
    <br />
    <!-- New Section For RenewInsurance -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <act:TabContainer ID="tabRenewInsuranceContainer" runat="server" ActiveTabIndex="0"
                CssClass="tabStyle" Width="1190px" AutoPostBack="True" OnClientActiveTabChanged="DisplayProcessing"
                OnActiveTabChanged="tabRenewInsuranceContainer_OnActiveTabChanged">
            </act:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="left" style="width: 1100px;">
                        <%--<asp:Button ID="btnAddResultContactTmp" runat="server" CssClass="Button" OnClientClick="DisplayProcessing();"
                            Text="บันทึกผลการติดต่อ" Width="140px" OnClick="btnAddResultContact_Click" />--%>
                        <div id="divLeadAddContact" runat="server">
                            <asp:Button ID="btnAddResultContact" runat="server" CssClass="Button" OnClientClick="DisplayProcessing();"
                                Text="บันทึกผลการติดต่อ" Width="140px" OnClick="btnAddResultContact_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="cbThisLead" runat="server" Text="แสดงเฉพาะข้อมูลผู้มุ่งหวังนี้"
                                Checked="true" AutoPostBack="true" onchange="DisplayProcessing()" OnCheckedChanged="chkthis_CheckedChanged" />
                            <asp:TextBox ID="txtTicketIdSearch" runat="server" Width="40px"></asp:TextBox>
                            <asp:TextBox ID="txtCitizenIdSearch" runat="server" Width="40px"></asp:TextBox>
                            <asp:TextBox ID="txtTelNo1Search" runat="server" Width="40px"></asp:TextBox>
                            <asp:TextBox ID="txtLoginName" runat="server" Width="40px" Visible="false"></asp:TextBox>
                        </div>
                        <div id="divPreleadAddContact" runat="server">
                            <asp:Button ID="btnAddContractPrelead" runat="server" CssClass="Button" OnClientClick="DisplayProcessing();"
                                Text="บันทึกผลการติดต่อ" Width="140px" OnClick="btnAddContractPrelead_Click" />
                            <asp:TextBox ID="txtPreleadIdSearch" runat="server" Width="50px"></asp:TextBox>
                        </div>
                    </td>
                    <td>
                        <asp:LinkButton ID="lbCasAllActivity" runat="server" Text="All Activity" Font-Underline="true"></asp:LinkButton>
                    </td>
                </tr>
            </table>
            <br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange"
                Width="1173px" />
            <asp:Panel ID="pnlPhoneCallHistoty" runat="server" CssClass="PanelPhoneCallHistoty"
                ScrollBars="Auto">
                <asp:GridView ID="gvPhoneCallHistoty" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="TicketId" Width="1570px" GridLines="Horizontal" BorderWidth="0px"
                    EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>">
                    <Columns>
                        <asp:TemplateField HeaderText="วันที่บันทึกข้อมูล">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="TicketId" HeaderText="Ticket ID">
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                            <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Firstname" HeaderText="ชื่อ">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Lastname" HeaderText="นามสกุล">
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ContactPhone" HeaderText="หมายเลขโทรศัพท์<br />ที่ติดต่อลูกค้า"
                            HtmlEncode="false">
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                            <HeaderStyle Width="130px" HorizontalAlign="Center" />
                            <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ContactDetail" HeaderText="รายละเอียดการติดต่อ">
                            <HeaderStyle Width="170px" HorizontalAlign="Center" />
                            <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreatedName" HeaderText="ผู้บันทึก">
                            <HeaderStyle Width="130px" HorizontalAlign="Center" />
                            <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="ประเภทบุคคล">
                            <ItemTemplate>
                                <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร">
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <%-- 2016-11-08 --%>
                        <asp:TemplateField HeaderText="ใบเสนอราคา">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDownloadQuotation" runat="server" Width="27px" CommandArgument='<%# Container.DisplayIndex %>'
                                    OnClick="imbDownloadQuotation_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("QuotationFilePath") != null ? (Eval("QuotationFilePath").ToString() != "" ? true : false) : false %>'
                                    ToolTip="Download" OnClientClick="DisplayProcessing();" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <%-- ---- --%>
                        <asp:TemplateField HeaderText="บัตรเครดิต">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDownloadCredit" runat="server" Width="27px" CommandArgument='<%# Container.DisplayIndex %>'
                                    OnClick="imbDownloadCredit_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("CreditFilePath") != null ? (Eval("CreditFilePath").ToString() != "" ? true : false) : false %>'
                                    ToolTip="Download" OnClientClick="DisplayProcessing();" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="50 ทวิ">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDownload50Tawi" runat="server" Width="27px" CommandArgument='<%# Container.DisplayIndex %>'
                                    OnClick="imbDownload50Tawi_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("Tawi50FilePath") != null ? (Eval("Tawi50FilePath").ToString() != "" ? true : false) : false %>'
                                    ToolTip="Download" OnClientClick="DisplayProcessing();" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ใบขับขี่">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbDownloadDriverLicense" runat="server" Width="27px" CommandArgument='<%# Container.DisplayIndex %>'
                                    OnClick="imbDownloadDriverLicense_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("DriverLicenseFilePath") != null ? (Eval("DriverLicenseFilePath").ToString() != "" ? true : false) : false %>'
                                    ToolTip="Download" OnClientClick="DisplayProcessing();" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Path">
                            <ItemTemplate>
                                <asp:Label ID="lblQuotationFilePath" runat="server" Text='<%# Eval("QuotationFilePath") %>'></asp:Label>
                                <asp:Label ID="lblQuotationFileName" runat="server" Text='<%# Eval("QuotationFileName") %>'></asp:Label>
                                <asp:Label ID="lblCreditFilePath" runat="server" Text='<%# Eval("CreditFilePath") %>'></asp:Label>
                                <asp:Label ID="lblCreditFileName" runat="server" Text='<%# Eval("CreditFileName") %>'></asp:Label>
                                <asp:Label ID="lblTawi50FilePath" runat="server" Text='<%# Eval("Tawi50FilePath") %>'></asp:Label>
                                <asp:Label ID="lblTawi50FileName" runat="server" Text='<%# Eval("Tawi50FileName") %>'></asp:Label>
                                <asp:Label ID="lblDriverLicenseFilePath" runat="server" Text='<%# Eval("DriverLicenseFilePath") %>'></asp:Label>
                                <asp:Label ID="lblDriverLicenseFileName" runat="server" Text='<%# Eval("DriverLicenseFileName") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="Hidden" />
                            <ControlStyle CssClass="Hidden" />
                            <HeaderStyle CssClass="Hidden" />
                            <FooterStyle CssClass="Hidden" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                </asp:GridView>
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
            <asp:PostBackTrigger ControlID="tabRenewInsuranceContainer" />
        </Triggers>--%>
        <ContentTemplate>
            <div id="divRenewInsurance" runat="server" class="divRenewInsurance">
                <asp:HiddenField ID="hidLasttab" runat="server" Value="0" />
                <asp:HiddenField ID="hidTime" runat="server" />
                <br />
                <asp:Panel ID="Panel2" runat="server" Style="padding-right: 5px">
                    <asp:Button ID="btnCancel_PreleadDat" Visible="false" runat="server" CssClass="Button"
                        Text="ยกเลิก" Width="140px" Style="float: right; padding-right: 5px" />&nbsp;&nbsp;
                    <asp:Button ID="btnSave_PreleadData" runat="server" CssClass="Button" Text="บันทึก"
                        Width="140px" OnClick="btnSave_PreleadData_Click" OnClientClick="return save004();"
                        Style="float: right; padding-right: 5px" />&nbsp;&nbsp;<asp:Button ID="btnHistoryMain"
                            runat="server" CssClass="Button" Text="ประวัติการเสนอขาย" Width="140px" Style="float: right; padding-right: 5px"
                            OnClick="btnHistoryMain_Click" OnClientClick="DisplayProcessing();" />
                    <p>
                    </p>
                </asp:Panel>
                <asp:UpdatePanel ID="upCopyPrint" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnCopy" runat="server" Visible="false">
                            คัดลอกข้อมูลจาก Ticket ID
                            <asp:Label ID="lblMainContractNo" runat="server"></asp:Label>
                            <asp:Button ID="btnCopyContract" runat="server" CssClass="Button" Text="Apply" Width="100px"
                                OnClick="btnCopyContract_Click" OnClientClick=" DisplayProcessing(); return true;" />
                            <p>
                            </p>
                        </asp:Panel>
                        <asp:Panel ID="pnPrint" runat="server">
                            <asp:CheckBox ID="chkPrint" runat="server" AutoPostBack="true" Text="พิมพ์ใบเสนอราคา"
                                OnCheckedChanged="chkPrint_OnCheckedChanged" onclick="DisplayProcessing();" />
                            <p>
                            </p>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Panel ID="Panel1" runat="server">
                    <asp:UpdatePanel ID="pnCar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table>
                                <tr style="vertical-align:top;">
                                    <td>ยี่ห้อรถยนต์<span style="color:red">*</span>
                                        <asp:HiddenField ID="slm_Contract_Number" runat="server" />
                                        <!--add by nung 20170505-->
                                        <asp:HiddenField ID="slm_BranchId" runat="server" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbCarBrand" AutoPostBack="true" runat="server" Width="170px" CssClass="Dropdownlist"
                                            OnSelectedIndexChanged="cmbCarBrand_SelectedIndexChanged" EnableViewState="true"
                                            onchange="DisplayProcessing()">
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width:120px;">
                                        <asp:Label ID="lblSlm_Brand_Name_Org" runat="server"></asp:Label>
                                    </td>
                                     <td>
                                         ประเภทรถ<span style="color:red">*</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbInsuranceCarType" runat="server" CssClass="Dropdownlist" Width="270px"></asp:DropDownList>
                                        <asp:Label ID="lblInsuranceCarTypeNameOrg" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td>รุ่นรถ<span style="color:red">*</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbCarName" AutoPostBack="true" runat="server" Width="170px" CssClass="Dropdownlist"
                                            OnSelectedIndexChanged="cmbCarName_SelectedIndexChanged" EnableViewState="true"
                                            onchange="DisplayProcessing()">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblSlm_Model_Sub_Name_Org" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td >
                                        ปีรถ
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbModelYear" runat="server" CssClass="Dropdownlist" Width="80px"></asp:DropDownList>
                                        <asp:Label ID="lblModelYearOrg" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr style="display: none">
                                    <td>รุ่นรถย่อย
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbCarSubName" runat="server" Width="170px" CssClass="Dropdownlist">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblSlm_Model_Name_Org" runat="server" Text=""></asp:Label>
                                    </td>
                                     <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td>ซีซีรถ
                                    </td>
                                    <td>
                                        <asp:TextBox ID="CCCar" runat="server" Width="100px" MaxLength="5"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblSlm_Cc" runat="server" Text=""></asp:Label>
                                    </td>
                                     <td>
                                         จังหวัดที่จดทะเบียน<span style="color:red">*</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbProvinceRegis" runat="server" CssClass="Dropdownlist" Width="170px"></asp:DropDownList>
                                        <asp:Label ID="lblProvinceRegisOrg" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <asp:HiddenField ID="hidPromotionId1" runat="server" />
                <asp:HiddenField ID="hidPromotionId2" runat="server" />
                <asp:HiddenField ID="hidPromotionId3" runat="server" />
                <asp:HiddenField ID="hidPromotionId4" runat="server" />
                <asp:HiddenField ID="hidPromotionActId1" runat="server" />
                <asp:HiddenField ID="hidPromotionActId2" runat="server" />
                <asp:HiddenField ID="hidPromotionActId3" runat="server" />
                <asp:HiddenField ID="hidPromotionActId4" runat="server" />
                <asp:HiddenField ID="hidSeq1" runat="server" />
                <asp:HiddenField ID="hidSeq2" runat="server" />
                <asp:HiddenField ID="hidSeq3" runat="server" />
                <asp:HiddenField ID="hidSeq4" runat="server" />
                <asp:HiddenField ID="hidSeqAct1" runat="server" />
                <asp:HiddenField ID="hidSeqAct2" runat="server" />
                <asp:HiddenField ID="hidSeqAct3" runat="server" />
                <asp:HiddenField ID="hidSeqAct4" runat="server" />
                <asp:HiddenField ID="hdClientFullname" runat="server" />
                <br />
                <div id="divPolicy" runat="server">
                    <asp:LinkButton ID="lbAdvanceSearch" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                        Text="[-] <b>ข้อมูลประกันภัย</b>" OnClick="lbAdvanceSearch_Click"></asp:LinkButton>
                    <asp:TextBox ID="txtAdvanceSearch" runat="server" Text="N" Visible="false"></asp:TextBox>
                    <asp:Panel ID="pnAdvanceSearch" runat="server" Style="display: initial;">
                        <asp:UpdatePanel ID="pnPolicy" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <p>
                                    <asp:Button ID="btnCancelPolicy" OnClientClick="DisplayProcessing();" runat="server"
                                        CssClass="Button" Text="ยกเลิกประกัน" Width="140px" OnClick="btnCancelPolicy_OnClick" />
                                    &nbsp; &nbsp;<%--OnClientClick='return confirmCancelPolicy()'--%><asp:Button ID="btnAddBlankPolicy"
                                        runat="server" CssClass="Button" Text="ซื้อบริษัทประกันภัยอื่นๆ" Width="160px"
                                        OnClientClick='DisplayProcessing()' OnClick="btnAddBlankPolicy_OnClick" />&nbsp;&nbsp;&nbsp;&nbsp;
                                    <span id="divCancelPolicy" runat="server">วันที่ยกเลิกประกันภัย&nbsp;&nbsp;<asp:TextBox
                                        ID="txtPolicyCancelDate" runat="server" Enabled="false"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                        เหตุผลการยกเลิก&nbsp;&nbsp;<asp:DropDownList ID="cmbPolicyCancelReason" runat="server"
                                            Enabled="false">
                                        </asp:DropDownList>
                                    </span>
                                </p>
                                <table cellpadding="2" cellspacing="0" border="1" class="normal_table">
                                    <tr class="normal_row">
                                        <td class="t_rowhead" align="center" style="width: 200px">ข้อมูลประกัน
                                        </td>
                                        <td class="t_rowhead" align="center">ปีเดิม(<asp:Label ID="lblYear_pre" runat="server" Text=""></asp:Label>)
                                            <asp:HiddenField ID="hidYear_pre" runat="server" />
                                        </td>
                                        <td class="t_rowhead" align="center">ปีนี้(<asp:Label ID="lblExpireDate" runat="server" Text=""></asp:Label>)
                                            <asp:HiddenField ID="hidYear_cur" runat="server" />
                                            <asp:HiddenField ID="hidNotifyPremiumId" runat="server"/>
                                            <asp:HiddenField ID="hidHasCurrentYearPolicy" runat="server" Value="true"/>
                                        </td>
                                        <td class="t_rowhead" align="center">
                                            <asp:Label ID="lblInsnameth_name_pro1" runat="server" Text=""></asp:Label>
                                            <asp:Button ID="imgDel_pro1" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDel_pro1_Click" Style="height: 24px; cursor: pointer;" />
                                            <asp:HiddenField ID="hidInjuryDeath_pro1" runat="server" />
                                            <asp:HiddenField ID="hidTPPD_pro1" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccident_pro1" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentPassenger_pro1" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentDriver_pro1" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFee_pro1" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeePassenger_pro1" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeeDriver_pro1" runat="server" />
                                            <asp:HiddenField ID="hidInsuranceDriver_pro1" runat="server" />
                                        </td>
                                        <td class="t_rowhead" align="center">
                                            <asp:Label ID="lblInsnameth_name_pro2" runat="server" Text=""></asp:Label>
                                            <asp:Button ID="imgDel_pro2" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDel_pro2_Click" Style="height: 24px; cursor: pointer;" />
                                            <asp:HiddenField ID="hidInjuryDeath_pro2" runat="server" />
                                            <asp:HiddenField ID="hidTPPD_pro2" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccident_pro2" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentPassenger_pro2" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentDriver_pro2" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFee_pro2" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeePassenger_pro2" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeeDriver_pro2" runat="server" />
                                            <asp:HiddenField ID="hidInsuranceDriver_pro2" runat="server" />
                                        </td>
                                        <td class="t_rowhead" align="center">
                                            <asp:Label ID="lblInsnameth_name_pro3" runat="server" Text=""></asp:Label>
                                            <asp:Button ID="imgDel_pro3" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDel_pro3_Click" Style="height: 24px; cursor: pointer;" />
                                            <asp:HiddenField ID="hidInjuryDeath_pro3" runat="server" />
                                            <asp:HiddenField ID="hidTPPD_pro3" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccident_pro3" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentPassenger_pro3" runat="server" />
                                            <asp:HiddenField ID="hidPersonalAccidentDriver_pro3" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFee_pro3" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeePassenger_pro3" runat="server" />
                                            <asp:HiddenField ID="hidMedicalFeeDriver_pro3" runat="server" />
                                            <asp:HiddenField ID="hidInsuranceDriver_pro3" runat="server" />
                                        </td>
                                        <%-- <td class="t_rowhead" align="center">
                                           บริษัทประกันอื่นๆ
                                            <asp:ImageButton ID="imgDel_bla" OnClientClick="DisplayProcessing();" OnClick="imgDel_bla_Click" ImageUrl="~/Images/delete.gif"
                                                runat="server" />
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ประกัน
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="txtInsNameTh_pre" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:RadioButton ID="rbInsNameTh_cur" GroupName="InsNameTh" Text="ซื้อบริษัทเดิม" runat="server" OnCheckedChanged="rbInsNameTh_CheckedChanged"  AutoPostBack="true"/>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:RadioButton ID="rbInsNameTh_pro1" GroupName="InsNameTh" Text="ซื้อบริษัทนี้" runat="server"  OnCheckedChanged="rbInsNameTh_CheckedChanged"  AutoPostBack="true" />
                                        </td>
                                        <td style="text-align: right">
                                            <asp:RadioButton ID="rbInsNameTh_pro2" GroupName="InsNameTh" Text="ซื้อบริษัทนี้" runat="server"  OnCheckedChanged="rbInsNameTh_CheckedChanged"  AutoPostBack="true"/>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:RadioButton ID="rbInsNameTh_pro3" GroupName="InsNameTh" Text="ซื้อบริษัทนี้" runat="server" OnCheckedChanged="rbInsNameTh_CheckedChanged"  AutoPostBack="true" />
                                        </td>
                                        <%--<td style="text-align: right">
                                           <asp:RadioButton ID="rbInsNameTh_bla" GroupName="InsNameTh" Text="ซื้อบริษัทนี้"
                                                runat="server" />
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ประเภทประกัน
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCoverageType_pre" runat="server" Text=""></asp:Label><asp:HiddenField
                                                ID="hidCoverageType_pre" runat="server" />
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType_cur" Enabled="false" Width="160px" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblCoverageType_cur" runat="server" Text=""></asp:Label><asp:HiddenField
                                                ID="hidCoverageType_cur" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType_pro1" Width="160px" runat="server">
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCoverageType_pro1" runat="server" Text=""></asp:Label><asp:HiddenField
                                                ID="hidCoverageType_pro1" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType_pro2" Width="160px" runat="server">
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCoverageType_pro2" runat="server" Text=""></asp:Label><asp:HiddenField
                                                ID="hidCoverageType_pro2" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType_pro3" Width="160px" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblCoverageType_pro3" runat="server" Text=""></asp:Label><asp:HiddenField
                                                ID="hidCoverageType_pro3" runat="server" />--%>
                                        </td>
                                        <%-- <td style="text-align: right"><asp:DropDownList ID="cmbCoverageType_bla" Width="170px" runat="server">
                                            </asp:DropDownList></td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>บริษัทประกันภัย
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInsNameTh_pre" runat="server" Text=""></asp:Label><asp:Label ID="lblInsCom_Id_pre"
                                                runat="server" Style="display: none;"></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblInsNameTh_cur" Enabled="false" runat="server" Width="160px">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblInsNameTh_cur" runat="server" Text=""></asp:Label><asp:Label ID="lblInsCom_Id_cur"
                                                runat="server" Style="display: none;"></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblInsNameTh_pro1" runat="server" Width="160px">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblInsNameTh_pro1" runat="server" Text=""></asp:Label><asp:Label ID="lblInsCom_Id_pro1"
                                                runat="server" Style="display: none;"></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblInsNameTh_pro2" runat="server" Width="160px">
                                            </asp:DropDownList>
                                            <%--  <asp:Label ID="lblInsNameTh_pro2" runat="server" Text=""></asp:Label><asp:Label ID="lblInsCom_Id_pro2"
                                                runat="server" Style="display: none;"></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblInsNameTh_pro3" runat="server" Width="160px">
                                            </asp:DropDownList>
                                            <%--  <asp:Label ID="lblInsNameTh_pro3" runat="server" Text=""></asp:Label><asp:Label ID="lblInsCom_Id_pro3"
                                                runat="server" Style="display: none;"></asp:Label>--%>
                                        </td>
                                        <%--<td style="text-align: right"><asp:DropDownList ID="cmbInsNameTh_bla" runat="server" Width="170px">
                                            </asp:DropDownList></td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>เลขที่กรมธรรม์
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_cur" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_pro1" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_pro2" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_pro3" runat="server" Text=""></asp:Label>
                                        </td>
                                        <%--<td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Policy_Number_bla" runat="server" Text=""></asp:Label>
                                        </td>--%>
                                    </tr>
                                    <asp:UpdatePanel ID="pnDriver" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <tr class="normal_row">
                                                <td>ชื่อผู้ขับขี่
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_Flag_pre" runat="server"></asp:Label><asp:HiddenField ID="hidDriver_Flag_pre"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:RadioButton ID="rbDriver_Flag_cur1" runat="server" GroupName="rbDriverFlagCur" Text="ระบุ" onchange="DisplayProcessing()" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" />&nbsp;&nbsp;&nbsp;
                                                    <asp:RadioButton ID="rbDriver_Flag_cur2" GroupName="rbDriverFlagCur" runat="server"
                                                        Text="ไม่ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" onchange="DisplayProcessing()" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:RadioButton ID="rbDriver_Flag_pro11" GroupName="rbDriver_Flag_pro1" runat="server"
                                                        Text="ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" onchange="DisplayProcessing()" />&nbsp;&nbsp;&nbsp;
                                                    <asp:RadioButton ID="rbDriver_Flag_pro12" GroupName="rbDriver_Flag_pro1" runat="server"
                                                        Text="ไม่ระบุ" OnCheckedChanged="rbDriver_Flag_Click" Checked="true" AutoPostBack="true"
                                                        onchange="DisplayProcessing()" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:RadioButton ID="rbDriver_Flag_pro21" GroupName="rbDriver_Flag_pro2" runat="server"
                                                        Text="ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" onchange="DisplayProcessing()" />&nbsp;&nbsp;&nbsp;
                                                    <asp:RadioButton ID="rbDriver_Flag_pro22" GroupName="rbDriver_Flag_pro2" runat="server"
                                                        Text="ไม่ระบุ" OnCheckedChanged="rbDriver_Flag_Click" Checked="true" AutoPostBack="true"
                                                        onchange="DisplayProcessing()" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:RadioButton ID="rbDriver_Flag_pro31" GroupName="rbDriver_Flag_pro3" runat="server"
                                                        Text="ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" onchange="DisplayProcessing()" />&nbsp;&nbsp;&nbsp;
                                                    <asp:RadioButton ID="rbDriver_Flag_pro32" GroupName="rbDriver_Flag_pro3" runat="server"
                                                        Text="ไม่ระบุ" OnCheckedChanged="rbDriver_Flag_Click" Checked="true" AutoPostBack="true"
                                                        onchange="DisplayProcessing()" />
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:RadioButton ID="rbDriver_Flag_bla1" GroupName="rbDriver_Flag_bla" runat="server"
                                                        Text="ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" onclick="DisplayProcessing()" />&nbsp;&nbsp;&nbsp;
                                                    <asp:RadioButton ID="rbDriver_Flag_bla2" GroupName="rbDriver_Flag_bla" runat="server"
                                                        Text="ไม่ระบุ" OnCheckedChanged="rbDriver_Flag_Click" AutoPostBack="true" Checked="true"
                                                        onclick="DisplayProcessing()" />
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>คำนำหน้าผู้ขับขี่คนที่ 1
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblTitleName1_pre" runat="server"></asp:Label>
                                                    <asp:HiddenField ID="hidTitleName1_pre" runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName1_cur" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName1_pro1" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName1_pro2" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName1_pro3" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <%--<td style="text-align: right">
                                                   <asp:DropDownList ID="cmbTitleName1_bla" runat="server" Width="170px">
                                                    </asp:DropDownList>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ชื่อผู้ขับขี่คนที่ 1
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_First_Name1_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name1_cur" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name1_pro1" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name1_pro2" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name1_pro3" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <%-- <td style="text-align: right"><asp:TextBox ID="txtDriver_First_Name1_bla" MaxLength="250" runat="server"></asp:TextBox></td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>นามสกุลผู้ขับขี่คนที่ 1
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_Last_Name1_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name1_cur" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name1_pro1" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name1_pro2" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name1_pro3" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name1_bla" MaxLength="250" runat="server"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>วันเกิดผู้ขับขี่คนที่ 1 ระบุเป็น ค.ศ.
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_Birthdate1_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate1_cur" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate1_pro1" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate1_pro2" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate1_pro3" runat="server" Width="102px" />
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate1_bla" runat="server" Width="130px" />
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>คำนำหน้าผู้ขับขี่คนที่ 2
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblTitleName2_pre" runat="server"></asp:Label>
                                                    <asp:HiddenField ID="hidTitleName2_pre" runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName2_cur" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName2_pro1" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName2_pro2" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName2_pro3" runat="server" Width="160px">
                                                    </asp:DropDownList>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:DropDownList ID="cmbTitleName2_bla" runat="server" Width="170px">
                                                    </asp:DropDownList>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ชื่อผู้ขับขี่คนที่ 2
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_First_Name2_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name2_cur" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name2_pro1" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name2_pro2" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name2_pro3" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <%-- <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_First_Name2_bla" MaxLength="250" runat="server"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>นามสกุลผู้ขับขี่คนที่ 2
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_Last_Name2_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name2_cur" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name2_pro1" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name2_pro2" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name2_pro3" MaxLength="250" runat="server" Width="154px"></asp:TextBox>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtDriver_Last_Name2_bla" MaxLength="250" runat="server"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>วันเกิดผู้ขับขี่คนที่ 2 ระบุเป็น ค.ศ.
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDriver_Birthdate2_pre" runat="server"></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate2_cur" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate2_pro1" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate2_pro2" runat="server" Width="102px" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate2_pro3" runat="server" Width="102px" />
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <uc2:TextDateMask ID="tdmDriver_Birthdate2_bla" runat="server" Width="130px" />
                                                </td>--%>
                                            </tr>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <tr class="normal_row">
                                        <td>เลขที่รับแจ้ง
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInformed_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInformed_cur" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInformed_pro1" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInformed_pro2" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblInformed_pro3" runat="server" Text=""></asp:Label>
                                        </td>
                                        <%-- <td style="text-align: right">
                                            <asp:Label ID="lblInformed_bla" runat="server" Text=""></asp:Label>
                                        </td>--%>
                                    </tr>
                                    <asp:UpdatePanel ID="pnPolicyDate" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <tr class="normal_row">
                                                <td>วันเริ่มคุ้มครอง
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Eff_Date_pre" style="width: 130px;" Enabled="false"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc5:TextDateMaskWithEvent ID="lblVoluntary_Policy_Eff_Date_cur" style="width: 130px;"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc5:TextDateMaskWithEvent ID="lblVoluntary_Policy_Eff_Date_pro1" style="width: 130px;"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc5:TextDateMaskWithEvent ID="lblVoluntary_Policy_Eff_Date_pro2" style="width: 130px;"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc5:TextDateMaskWithEvent ID="lblVoluntary_Policy_Eff_Date_pro3" style="width: 130px;"
                                                        runat="server" />
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Eff_Date_bla" style="width: 130px;" runat="server" />
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>วันหมดอายุประกัน
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_pre" style="width: 130px;" Enabled="false"
                                                        runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_cur" style="width: 130px;" runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_pro1" style="width: 130px;" runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_pro2" style="width: 130px;" runat="server" />
                                                </td>
                                                <td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_pro3" style="width: 130px;" runat="server" />
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <uc2:TextDateMask ID="lblVoluntary_Policy_Exp_Date_bla" style="width: 130px;" runat="server" />
                                                </td>--%>
                                            </tr>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <tr style="display: none">
                                        <td>ประเภทประกัน
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCoverageType2_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType2_cur" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblCoverageType2_cur" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType2_pro1" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblCoverageType2_pro1" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType2_pro2" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%--  <asp:Label ID="lblCoverageType2_pro2" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblCoverageType2_pro3" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCoverageType2_pro3" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <%--<td style="text-align: right">
                                            <asp:DropDownList ID="cmbCoverageType2_bla" Style="width: 170px;" runat="server">
                                            </asp:DropDownList>
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ประเภทการซ่อม
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblMaintanance_pre" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hidMaintanance_pre" runat="server" />
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblMaintanance_cur" Enabled="false" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblMaintanance_cur" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hidMaintanance_cur" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblMaintanance_pro1" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblMaintanance_pro1" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hidMaintanance_pro1" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblMaintanance_pro2" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%-- <asp:Label ID="lblMaintanance_pro2" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hidMaintanance_pro2" runat="server" />--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:DropDownList ID="lblMaintanance_pro3" Style="width: 160px;" runat="server">
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblMaintanance_pro3" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hidMaintanance_pro3" runat="server" />--%>
                                        </td>
                                        <%-- <td style="text-align: right">
                                         <asp:DropDownList ID="cmbMaintanance_bla" Style="width: 170px;" runat="server">
                                            </asp:DropDownList>
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ค่าเสียหายส่วนแรก
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCost_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCost_cur" Enabled="false" Visible="false" CssClass="money" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <asp:DropDownList ID="cmbDeDuctFlag_cur" runat="server" Enabled="false" Style="width: 160px;">
                                                <asp:ListItem Text="N" Value="N"></asp:ListItem>
                                                <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCost_cur" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCost_pro1" CssClass="money" Visible="false" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <asp:DropDownList ID="cmbDeDuctFlag_pro1" runat="server" Style="width: 160px;">
                                                <asp:ListItem Text="N" Value="N"></asp:ListItem>
                                                <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCost_pro1" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCost_pro2" CssClass="money" Visible="false" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <asp:DropDownList ID="cmbDeDuctFlag_pro2" runat="server" Style="width: 160px;">
                                                <asp:ListItem Text="N" Value="N"></asp:ListItem>
                                                <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCost_pro2" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCost_pro3" CssClass="money" Visible="false" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <asp:DropDownList ID="cmbDeDuctFlag_pro3" runat="server" Style="width: 160px;">
                                                <asp:ListItem Text="N" Value="N"></asp:ListItem>
                                                <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="lblCost_pro3" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <%-- <td style="text-align: right">
                                            <asp:TextBox ID="txtCost_bla" CssClass="money" MaxLength="15" Style="text-align: right"
                                                runat="server"></asp:TextBox>
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ทุนประกันความเสียหายต่อตัวรถยนต์ (OD)
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVoluntary_Cov_Amt_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVoluntary_Cov_Amt_cur" Enabled="false" MaxLength="15" CssClass="money" Width="154px"
                                                Style="text-align: right" runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblVoluntary_Cov_Amt_cur" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVoluntary_Cov_Amt_pro1" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblVoluntary_Cov_Amt_pro1" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVoluntary_Cov_Amt_pro2" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblVoluntary_Cov_Amt_pro2" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVoluntary_Cov_Amt_pro3" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblVoluntary_Cov_Amt_pro3" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <%-- <td style="text-align: right">
                                            <asp:TextBox ID="txtVoluntary_Cov_Amt_bla" MaxLength="15" CssClass="money" Style="text-align: right"
                                                runat="server"></asp:TextBox>
                                        </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ทุนประกันกรณีรถยนต์สูญหาย/ไฟไหม้ (F&amp;T)
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblCostFT_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCostFT_cur" Enabled="false" CssClass="money" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblCostFT_cur" runat="server" Text=""></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCostFT_pro1" CssClass="money" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblCostFT_pro1" runat="server"></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCostFT_pro2" CssClass="money" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%-- <asp:Label ID="lblCostFT_pro2" runat="server"></asp:Label>--%>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblCostFT_pro3" CssClass="money" MaxLength="15" Style="text-align: right" Width="154px"
                                                runat="server"></asp:TextBox>
                                            <%--<asp:Label ID="lblCostFT_pro3" runat="server"></asp:Label>--%>
                                        </td>
                                        <%--<td style="text-align: right">
                                            <asp:TextBox ID="txtCostFT_bla" CssClass="money" MaxLength="15" Style="text-align: right"
                                                runat="server"></asp:TextBox>
                                        </td>--%>
                                    </tr>
                                    <asp:UpdatePanel ID="pnCalPolicy" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <tr class="normal_row">
                                                <td>เบี้ยสุทธิ
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblNetpremium_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblNetpremium_cur" Enabled="false" MaxLength="15" CssClass="money" Width="154px"
                                                        Style="text-align: right" onblur="blockButton();" OnTextChanged="txtNetpremium_cur_OnTextChanged"
                                                        onchange="DisplayProcessing()" AutoPostBack="true" runat="server" Text=""></asp:TextBox>
                                                    <%-- <asp:Label ID="lblNetpremium_cur" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <%----%>
                                                    <asp:TextBox ID="lblNetpremium_pro1" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                        onblur="blockButton();" onchange="DisplayProcessing()" OnTextChanged="txtNetpremium_pro1_OnTextChanged"
                                                        AutoPostBack="true" runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblNetpremium_pro1" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblNetpremium_pro2" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                        onblur="blockButton();" OnTextChanged="txtNetpremium_pro2_OnTextChanged" onchange="DisplayProcessing()"
                                                        AutoPostBack="true" runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblNetpremium_pro2" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblNetpremium_pro3" MaxLength="15" CssClass="money" Style="text-align: right" Width="154px"
                                                        onblur="blockButton();" OnTextChanged="txtNetpremium_pro3_OnTextChanged" onchange="DisplayProcessing()"
                                                        AutoPostBack="true" runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblNetpremium_pro3" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtNetpremium_bla" MaxLength="15" CssClass="money" Style="text-align: right"
                                                        onblur="blockButton();" OnTextChanged="txtNetpremium_bla_OnTextChanged" onchange="DisplayProcessing()"
                                                        AutoPostBack="true" runat="server" Text=""></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>อากร
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDuty_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblDuty_cur" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%--  <asp:Label ID="lblDuty_cur" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblDuty_pro1" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%-- <asp:Label ID="lblDuty_pro1" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblDuty_pro2" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblDuty_pro2" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblDuty_pro3" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblDuty_pro3" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtDuty_bla" MaxLength="15" Style="text-align: right" Enabled="false"
                                                        runat="server" Text=""></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ภาษี
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblVat_amount_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVat_amount_cur" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVat_amount_cur" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVat_amount_pro1" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%-- <asp:Label ID="lblVat_amount_pro1" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVat_amount_pro2" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%-- <asp:Label ID="lblVat_amount_pro2" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVat_amount_pro3" MaxLength="15" Style="text-align: right" Enabled="false" Width="154px"
                                                        runat="server" Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVat_amount_pro3" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtVat_amount_bla" MaxLength="15" Style="text-align: right" Enabled="false"
                                                        runat="server" Text=""></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>เบี้ยประกันรวมภาษีอากร
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblVoluntary_Gross_Premium_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVoluntary_Gross_Premium_cur" Enabled="false" MaxLength="15" CssClass="money" Width="154px"
                                                        Style="text-align: right" OnTextChanged="txtVoluntary_Gross_Premium_cur_OnTextChanged"
                                                        onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"
                                                        Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVoluntary_Gross_Premium_cur" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <%----%>
                                                    <asp:TextBox ID="lblVoluntary_Gross_Premium_pro1" MaxLength="15" CssClass="money" Width="154px"
                                                        Style="text-align: right" OnTextChanged="txtVoluntary_Gross_Premium_pro1_OnTextChanged"
                                                        onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"
                                                        Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVoluntary_Gross_Premium_pro1" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVoluntary_Gross_Premium_pro2" MaxLength="15" CssClass="money" Width="154px"
                                                        Style="text-align: right" OnTextChanged="txtVoluntary_Gross_Premium_pro2_OnTextChanged"
                                                        onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"
                                                        Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVoluntary_Gross_Premium_pro2" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="lblVoluntary_Gross_Premium_pro3" MaxLength="15" CssClass="money" Width="154px"
                                                        Style="text-align: right" OnTextChanged="txtVoluntary_Gross_Premium_pro3_OnTextChanged"
                                                        onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"
                                                        Text=""></asp:TextBox>
                                                    <%--<asp:Label ID="lblVoluntary_Gross_Premium_pro3" runat="server" Text=""></asp:Label>--%>
                                                </td>
                                                <%-- <td style="text-align: right">
                                                    <asp:TextBox ID="txtVoluntary_Gross_Premium_bla" MaxLength="15" CssClass="money"
                                                        Style="text-align: right" OnTextChanged="txtVoluntary_Gross_Premium_bla_OnTextChanged"
                                                        onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"
                                                        Text=""></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ประเภทบุคคล
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_cur" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_pro1" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_pro2" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_pro3" runat="server" Text=""></asp:Label>
                                                </td>
                                                <%-- <td style="text-align: right">
                                                    <asp:Label ID="lblCardTypeId_bla" runat="server" Text=""></asp:Label>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>
                                                    <asp:CheckBox ID="chkCardType" runat="server" OnCheckedChanged="chkCardType_OnCheckedChanged"
                                                        AutoPostBack="true" onclick="DisplayProcessing()" />
                                                    หัก 1% (กรณีนิติบุคคล)
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_cur" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_pro1" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_pro2" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_pro3" runat="server" Text=""></asp:Label>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:Label ID="lblPersonType_bla" runat="server" Text=""></asp:Label>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ส่วนลด(%)<asp:Button ID="HiddenBtn" runat="server" Style="display: none;" />
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDiscountPercent_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountPercent_cur" CssClass="int" runat="server" AutoPostBack="true" Width="154px"
                                                        Style="text-align: right" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountPercent_OnTextChanged" MaxLength="2"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountPercent_pro1" CssClass="int" runat="server" AutoPostBack="true" Width="154px"
                                                        Style="text-align: right" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountPercent_OnTextChanged" MaxLength="2"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountPercent_pro2" CssClass="int" runat="server" AutoPostBack="true" Width="154px"
                                                        Style="text-align: right" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountPercent_OnTextChanged" MaxLength="2"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountPercent_pro3" CssClass="int" runat="server" AutoPostBack="true" Width="154px"
                                                        Style="text-align: right" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountPercent_OnTextChanged" MaxLength="2"></asp:TextBox>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountPercent_bla" CssClass="int" runat="server" AutoPostBack="true"
                                                        Style="text-align: right" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountPercent_OnTextChanged" MaxLength="2"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>ส่วนลด(บาท)
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblDiscountBath_pre" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountBath_cur" CssClass="money" Style="text-align: right" Width="154px"
                                                        runat="server" AutoPostBack="true" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountBath_OnTextChanged" MaxLength="15"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountBath_pro1" CssClass="money" Style="text-align: right" Width="154px"
                                                        runat="server" AutoPostBack="true" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountBath_OnTextChanged" MaxLength="15"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountBath_pro2" CssClass="money" Style="text-align: right" Width="154px"
                                                        runat="server" AutoPostBack="true" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountBath_OnTextChanged" MaxLength="15"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountBath_pro3" CssClass="money" Style="text-align: right" Width="154px"
                                                        runat="server" AutoPostBack="true" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountBath_OnTextChanged" MaxLength="15"></asp:TextBox>
                                                </td>
                                                <%--<td style="text-align: right">
                                                    <asp:TextBox ID="txtDiscountBath_bla" CssClass="money" Style="text-align: right"
                                                        runat="server" AutoPostBack="true" onchange="DisplayProcessing()" onblur="blockButton();"
                                                        OnTextChanged="txtDiscountBath_OnTextChanged" MaxLength="15"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td>เบี้ยประกันที่ต้องชำระ
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:Label ID="lblTotal_Voluntary_Gross_Premium_pre" runat="server" Enabled="false"
                                                        Text=""></asp:Label>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtTotal_Voluntary_Gross_Premium_cur" Style="text-align: right" Width="154px"
                                                        runat="server" Enabled="false"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtTotal_Voluntary_Gross_Premium_pro1" Style="text-align: right" Width="154px"
                                                        runat="server" Enabled="false"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtTotal_Voluntary_Gross_Premium_pro2" Style="text-align: right" Width="154px"
                                                        runat="server" Enabled="false"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtTotal_Voluntary_Gross_Premium_pro3" Style="text-align: right" Width="154px"
                                                        runat="server" Enabled="false"></asp:TextBox>
                                                </td>
                                                <%-- <td style="text-align: right">
                                                    <asp:TextBox ID="txtTotal_Voluntary_Gross_Premium_bla" Style="text-align: right"
                                                        runat="server" Enabled="false"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                            <tr class="normal_row">
                                                <td></td>
                                                <td align="right">ลูกค้าประหยัดเงิน
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtSafe_cur" Style="text-align: right" runat="server" Enabled="false" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtSafe_pro1" Style="text-align: right" runat="server" Enabled="false" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtSafe_pro2" Style="text-align: right" runat="server" Enabled="false" Width="154px"></asp:TextBox>
                                                </td>
                                                <td style="text-align: right">
                                                    <asp:TextBox ID="txtSafe_pro3" Style="text-align: right" runat="server" Enabled="false" Width="154px"></asp:TextBox>
                                                </td>
                                                <%-- <td style="text-align: right">
                                                    <asp:TextBox ID="txtSafe_bla" Style="text-align: right" runat="server" Enabled="false"></asp:TextBox>
                                                </td>--%>
                                            </tr>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </table>
                                <br />
                                <div id="divBeneficiary" runat="server">
                                     ข้อมูลผู้รับผลประโยชน์&nbsp;
                                    <asp:DropDownList ID="cmbBeneficiary" runat="server" CssClass="Dropdownlist" Width="400px" >
                                    </asp:DropDownList>
                                </div>
                                <div id="divRemarkPolicy" runat="server">
                                    <p>
                                        <asp:Image alt="" ID="ContentPlaceHolder1_imgResult" runat="server" ImageUrl="~/Images/PolicyRemark.png"
                                            ImageAlign="top" />
                                    </p>
                                    <asp:TextBox ID="txtInsDesc" TextMode="MultiLine" MaxLength="4000" Rows="5" Width="800"
                                        runat="server"></asp:TextBox>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="pnPolicyPayment" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="tabPayment" runat="server">
                                    <p>
                                        <asp:Image alt="" ID="Image2" runat="server" ImageUrl="~/Images/PolicyPaymentType.png"
                                            ImageAlign="top" />
                                    </p>
                                    <table>
                                        <tr>
                                            <td>ประเภทการชำระเงิน
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPaymentmethod" OnSelectedIndexChanged="cmbPaymentmethod_OnSelectedIndexChanged"
                                                    AutoPostBack="true" Width="100" runat="server" onchange="DisplayProcessing()">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod" Text="จำนวนงวด" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="textPeriod" class="int" onchange="DisplayProcessing();" OnTextChanged="textPeriod_TextChanged"
                                                    onblur="blockButton();" AutoPostBack="true" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPaymentType" Text="รูปแบบการชำระ" runat="server" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPaymentType" Width="100" runat="server" OnSelectedIndexChanged="cmbPaymentType_OnSelectedIndexChanged"
                                                    AutoPostBack="true" onchange="DisplayProcessing()">
                                                </asp:DropDownList>
                                            </td>
                                            <td>สาขา
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPolicyPayBranchCode" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trPday1" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate1" Text="วันที่ชำระงวดที่ 1" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate1" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod1" Text="จำนวนเงินที่ต้องชำระงวดที่ 1" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod1" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday2" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate2" Text="วันที่ชำระงวดที่ 2" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate2" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod2" Text="จำนวนเงินที่ต้องชำระงวดที่ 2" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod2" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday3" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate3" Text="วันที่ชำระงวดที่ 3" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate3" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod3" Text="จำนวนเงินที่ต้องชำระงวดที่ 3" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod3" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday4" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate4" Text="วันที่ชำระงวดที่ 4" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate4" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod4" Text="จำนวนเงินที่ต้องชำระงวดที่ 4" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod4" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday5" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate5" Text="วันที่ชำระงวดที่ 5" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate5" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod5" Text="จำนวนเงินที่ต้องชำระงวดที่ 5" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod5" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday6" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate6" Text="วันที่ชำระงวดที่ 6" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate6" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod6" Text="จำนวนเงินที่ต้องชำระงวดที่ 6" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod6" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday7" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate7" Text="วันที่ชำระงวดที่ 7" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate7" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod7" Text="จำนวนเงินที่ต้องชำระงวดที่ 7" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod7" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday8" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate8" Text="วันที่ชำระงวดที่ 8" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate8" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod8" Text="จำนวนเงินที่ต้องชำระงวดที่ 8" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod8" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday9" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate9" Text="วันที่ชำระงวดที่ 9" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate9" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod9" Text="จำนวนเงินที่ต้องชำระงวดที่ 9" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod9" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPday10" runat="server">
                                            <td>
                                                <asp:Label ID="lblPaymentDate10" Text="วันที่ชำระงวดที่ 10" runat="server" />
                                            </td>
                                            <td>
                                                <uc2:TextDateMask ID="tdmPaymentDate10" runat="server" Width="130px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPeriod10" Text="จำนวนเงินที่ต้องชำระงวดที่ 10" runat="server" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPeriod10" Style="text-align: right" MaxLength="15" CssClass="money"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <p>
                                    </p>
                                    <asp:Image alt="" ID="Img1" ImageUrl="~/Images/RemarkInstallment.png" runat="server"
                                        ImageAlign="top" /><p>
                                        </p>
                                    <asp:TextBox ID="txtPaymentDesc" TextMode="MultiLine" MaxLength="4000" Rows="5" Width="600"
                                        runat="server"></asp:TextBox>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
                <div id="divPromotion" runat="server">
                    <br />
                    <asp:UpdatePanel ID="pnPromotionAll" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:LinkButton ID="detailPromotion" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                                Text="[-] <b>ข้อมูลโปรโมชั่นประกันภัย</b>" OnClick="detailPromotion_Click"></asp:LinkButton>
                            <asp:TextBox ID="txtPromotion" runat="server" Text="N" Visible="false"></asp:TextBox>
                            <asp:Panel ID="pnPromotion" runat="server" Style="display: initial;">
                                <asp:UpdatePanel ID="pnCarCondition" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td>บริษัทประกัน
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtInsNameSearch" runat="server" Width="146px"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>ยี่ห้อรถยนต์
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmbCarBrand2" runat="server" EnableViewState="true" AutoPostBack="true" 
                                                        Width="150px" onchange="DisplayProcessing();" OnSelectedIndexChanged="cmbCarBrand2_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>รุ่นรถ
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmbCarName2" EnableViewState="true" runat="server" Width="100px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>ประเภทประกัน
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmbInsuranceType" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>ทุนประกันภัยต่อตัวรถยนต์
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtOD" CssClass="money" onchange="DisplayProcessing();" AutoPostBack="true"
                                                        runat="server" Width="80px" MaxLength="15" OnTextChanged="txtOD_TextChanged"></asp:TextBox>
                                                </td>
                                                <td>Ranking
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtODRanking" onchange="DisplayProcessing();" AutoPostBack="true"
                                                        runat="server" MaxLength="3" class="int" OnTextChanged="txtFTRanking_TextChanged"
                                                        Enabled="false" Width="50px"></asp:TextBox>&nbsp;&nbsp;&nbsp;%
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td class="auto-style2">ทุนประกันภัยสูญหาย/ไฟไหม้(F&T)
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtFT" CssClass="money" onchange="DisplayProcessing();" AutoPostBack="true"
                                                        runat="server" Width="80px" MaxLength="15" OnTextChanged="txtFT_TextChanged"></asp:TextBox>
                                                </td>
                                                <td class="auto-style2">Ranking
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtFTRanking" onchange="DisplayProcessing();" AutoPostBack="true"
                                                        runat="server" Enabled="false" MaxLength="2" OnTextChanged="txtFTRanking_TextChanged"
                                                        Width="50px"></asp:TextBox>&nbsp;&nbsp;&nbsp;%
                                                </td>
                                                <td class="auto-style2">
                                                    <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="Search" OnClick="btnSearch_Click"
                                                        OnClientClick="DisplayProcessing()" />
                                                </td>
                                                <td class="auto-style2"></td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <br />
                                <asp:UpdatePanel ID="upPromotion" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <uc1:GridviewPageController ID="pcTopPromotion" runat="server" OnPageChange="PromotionPageSearchChange"
                                            Width="1100px" />
                                        <asp:GridView ID="gvResultPromotion" runat="server" AutoGenerateColumns="False" Width="1100px"
                                            GridLines="Horizontal" BorderWidth="0px" OnRowCommand="lblPositionNameAbb_Click"
                                            EnableModelValidation="True" DataKeyNames="slm_PromotionId" OnRowDataBound="gvResultPromotion_OnRowDataBound"
                                            EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                                            <Columns>
                                                <asp:TemplateField HeaderText="เลือก">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkSelect" runat="server" onchange="DisplayProcessing();" AutoPostBack="true"
                                                            onclick="DisplayProcessing()" OnCheckedChanged="chkSelect_CheckedChanged" TabIndex="<%# Container.DisplayIndex %>" />
                                                        <asp:ImageButton ImageUrl="../../Images/add.gif" ID="lnkSelect" runat="server" onchange="DisplayProcessing();"
                                                            OnClientClick="DisplayProcessing()" OnClick="lnkSelect_OnClick" TabIndex="<%# Container.DisplayIndex %>" Visible="false" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="slm_PromotionId" HeaderText="รหัสโปร" Visible="false">
                                                    <HeaderStyle Width="130px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="130px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="ชื่อบริษัทประกันภัย">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lblPositionNameAbb" runat="server" CommandName="popup" CommandArgument='<%# Eval("slm_PromotionId") %>'
                                                            Text='<%# Eval("insname") %>'></asp:LinkButton>
                                                        <asp:Label ID="txtPositionNameAbb" runat="server" Text='<%# Eval("insname") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="330px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="330px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="แคมเปญ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCamname" runat="server" Text='<%# Eval("camname")!= null  ?  Eval("camname"):"" %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ความคุ้มครอง">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblConveragetypename" runat="server" Text='<%# Eval("converagetypename") != null ? Eval("converagetypename") :"" %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="200px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ประเภทการซ่อม">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRepairname" runat="server" Text='<%# Eval("repairname") != null ? Eval("repairname") :"" %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataFormatString="{0:N}" DataField="slm_OD" HeaderText="ทุนประกันภัยคุ้มครองต่อตัวรถยนต์">
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="ทุนประกันภัยสูญหายไฟไหม้" DataFormatString="{0:N}" DataField="slm_FT">
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="เบี้ยรวมภาษีและอากร" DataFormatString="{0:N}" DataField="slm_NetGrossPremium">
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:TemplateField Visible="false" HeaderText="เบี้ยรวมหักภาษี ณ ที่จ่าย 1% แล้ว(กรณีนิติบุคคล)">
                                                    <ItemTemplate>
                                                        <%--<asp:Label ID="lblEffectiveDateFrom" runat="server" Text='<%# Eval("EffectiveDateFrom") != null ? Convert.ToDateTime(Eval("EffectiveDateFrom")).ToString("dd/MM/yyyy") + Convert.ToDateTime(Eval("EffectiveDateFrom")).Year.ToString() : ""%>'></asp:Label>--%>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="ประเภทการซ่อม" DataField="repairname">
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:BoundField DataFormatString="{0:N}" DataField="slm_Act" HeaderText="พรบ.">
                                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="100px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:TemplateField Visible="false" HeaderText="เบี้ยประกันภัยภาคสมัครใจ + พรบ.">
                                                    <ItemTemplate>
                                                        <%--<asp:Label ID="lbIinsname" runat="server" Text='<%# Eval("insname") %>'></asp:Label>--%>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="อายุรถ(ปี)" DataField="slm_AgeCarYear">
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="right" VerticalAlign="Top" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="วันเริ่มการขาย">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEffectiveDateFrom" runat="server" Text='<%# Eval("EffectiveDateFrom") != null ? Convert.ToDateTime(Eval("EffectiveDateFrom")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EffectiveDateFrom")).Year.ToString() : ""%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="วันสิ้นสุดการขาย">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEffectiveDateTo" runat="server" Text='<%# Eval("EffectiveDateTo") != null ? Convert.ToDateTime(Eval("EffectiveDateTo")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EffectiveDateTo")).Year.ToString() : ""%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="t_rowhead" />
                                            <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <br />
                <asp:LinkButton ID="surety" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                    Text="[-] <b>ข้อมูลผู้ค้ำ</b>" OnClick="surety_Click"></asp:LinkButton>
                <asp:TextBox ID="txtSurety" runat="server" Text="N" Visible="false"></asp:TextBox>
                <br />
                <asp:Panel ID="pnSurety" runat="server" Style="display: initial;">
                    <asp:GridView ID="gvSurety" runat="server" AutoGenerateColumns="False" Width="900px"
                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                        <Columns>
                            <asp:TemplateField HeaderText="No">
                                <ItemTemplate>
                                    <%--<asp:Label ID="lblPositionNameAbb" runat="server" Text='<%# Eval("TITLENAME") %>'></asp:Label>--%>
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="คำนำหน้า">
                                <ItemTemplate>
                                    <asp:Label ID="lblGuaTitleName" runat="server" Text='<%# Eval("TITLENAME") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ชื่อผู้ค้ำ">
                                <ItemTemplate>
                                    <asp:Label ID="lblGuaPositionName" runat="server" Text='<%# Eval("FIRSTNAME") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="นามสกุลผู้ค้ำ">
                                <ItemTemplate>
                                    <asp:Label ID="lblPositionSurname" runat="server" Text='<%# Eval("LASTNAME") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ความสัมพันธ์">
                                <ItemTemplate>
                                    <asp:Label ID="lblRelationAbb" runat="server" Text='<%# Eval("RELATION") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="เลขที่บัตร">
                                <ItemTemplate>
                                    <asp:Label ID="lblGuaCardId" runat="server" Text='<%# Eval("CARDID") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="เบอร์มือถือ">
                                <ItemTemplate>
                                    <asp:Label ID="lblGuaTelNo" runat="server" Text='<%# Eval("TELNO") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="250px" HorizontalAlign="Center" />
                                <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="t_rowhead" />
                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                    </asp:GridView>
                </asp:Panel>
                <br />
                <asp:LinkButton ID="legislation" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                    Text="[-] <b>ข้อมูล พรบ.</b>" OnClick="legislation_Click"></asp:LinkButton>
                <asp:TextBox ID="txtLegislation" runat="server" Text="N" Visible="false"></asp:TextBox>
                <asp:Panel ID="pnLegislation" runat="server" Style="display: initial;">
                    <p>
                        <asp:Button ID="btnCancelAct" runat="server" CssClass="Button" Text="ยกเลิก พรบ."
                            Width="140px" OnClientClick="DisplayProcessing();" OnClick="btnCancelAct_OnClick" />&nbsp;
                        &nbsp;<%--OnClientClick="return confirmCancelAct()" --%><asp:Button ID="btnAddBlankAct"
                            runat="server" CssClass="Button" Text="ซื้อพรบ. อื่นๆ" Width="160px" OnClientClick='DisplayProcessing()'
                            OnClick="btnAddBlankAct_OnClick" />&nbsp;&nbsp;&nbsp;&nbsp; <span id="divCancelAct"
                                runat="server">วันที่ยกเลิกพรบ.&nbsp;&nbsp;<asp:TextBox ID="txtActCancelDate" runat="server"
                                    Enabled="false"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp; เหตุผลการยกเลิก&nbsp;&nbsp;<asp:DropDownList
                                        ID="cmbActCancelReason" runat="server" Enabled="false">
                                    </asp:DropDownList>
                            </span>
                        <p>
                        </p>
                        <table border="1" cellpadding="2" cellspacing="0" class="normal_table">
                            <tr>
                                <td align="center" class="t_rowhead" style="width: 200px">ข้อมูล พรบ.
                                </td>
                                <td align="center" class="t_rowhead">ปีเดิม (<asp:Label ID="lblPromoAct_name_pre" runat="server" Text=""></asp:Label>
                                    )
                                </td>
                                <td align="center" class="t_rowhead" style="width: 120px">
                                    <asp:Label ID="lblPromoAct_name_pro1" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lblPromoId_pro1" runat="server" Style="display: none;" Text=""></asp:Label>
                                    <asp:Button ID="imgDelAct_pro1" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDelAct_pro1_Click" Style="height: 24px; cursor: pointer;" />
                                </td>
                                <td align="center" class="t_rowhead" style="width: 120px">
                                    <asp:Label ID="lblPromoAct_name_pro2" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lblPromoId_pro2" runat="server" Style="display: none;" Text=""></asp:Label>
                                    <asp:Button ID="imgDelAct_pro2" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDelAct_pro2_Click" Style="height: 24px; cursor: pointer;" />
                                </td>
                                <td align="center" class="t_rowhead" style="width: 120px">
                                    <asp:Label ID="lblPromoAct_name_pro3" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lblPromoId_pro3" runat="server" Style="display: none;" Text=""></asp:Label>
                                    <asp:Button ID="imgDelAct_pro3" runat="server" Text="ลบ" Width="40px" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" OnClick="imgDelAct_pro3_Click" Style="height: 24px; cursor: pointer;" />
                                </td>
                                <%-- <td class="t_rowhead" align="center" style="width: 120px">
                                <asp:Label ID="lblAct_bla" runat="server" Text="บริษัทประกันอื่นๆ"></asp:Label>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>พรบ.
                                </td>
                                <td style="text-align: right">&nbsp;
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbAct_pro1" runat="server" GroupName="rbAct" Text="ซื้อ พรบ. บริษัทนี้" OnCheckedChanged="rbAct_CheckedChanged" AutoPostBack="true" />
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbAct_pro2" runat="server" GroupName="rbAct" Text="ซื้อ พรบ. บริษัทนี้" OnCheckedChanged="rbAct_CheckedChanged" AutoPostBack="true" />
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbAct_pro3" runat="server" GroupName="rbAct" Text="ซื้อ พรบ. บริษัทนี้" OnCheckedChanged="rbAct_CheckedChanged" AutoPostBack="true" />
                                </td>
                                <%--<td style="text-align: right">
                                <asp:RadioButton ID="rbAct_bla" runat="server" GroupName="rbAct" Text="ซื้อ พรบ. บริษัทนี้" />
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>ออกที่
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActIssuePlace_pre" runat="server" Text=""></asp:Label>
                                    <asp:HiddenField ID="hidActIssuePlace_pre" runat="server" />
                                    <asp:HiddenField ID="hidActIssueBranch_pre" runat="server" />
                                </td>
                                <td align="right">
                                    <asp:DropDownList ID="cmbActIssuePlace_pro1" runat="server" AutoPostBack="true" onchange="DisplayProcessing();"
                                        OnSelectedIndexChanged="cmbActIssuePlace_OnSelectedIndexChanged" Width="160px">
                                        <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="ธนาคาร" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="บริษัทประกัน" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:DropDownList ID="cmbActIssueBranch_pro1" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <asp:DropDownList ID="cmbActIssuePlace_pro2" runat="server" AutoPostBack="true" onchange="DisplayProcessing();"
                                        OnSelectedIndexChanged="cmbActIssuePlace_OnSelectedIndexChanged" Width="160px">
                                        <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="ธนาคาร" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="บริษัทประกัน" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:DropDownList ID="cmbActIssueBranch_pro2" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <asp:DropDownList ID="cmbActIssuePlace_pro3" runat="server" AutoPostBack="true" onchange="DisplayProcessing()"
                                        OnSelectedIndexChanged="cmbActIssuePlace_OnSelectedIndexChanged" Width="160px">
                                        <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="ธนาคาร" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="บริษัทประกัน" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:DropDownList ID="cmbActIssueBranch_pro3" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <%--<td align="right">
                                <asp:DropDownList ID="cmbActIssuePlace_bla" runat="server" Width="150px" onchange="DisplayProcessing();"
                                    OnSelectedIndexChanged="cmbActIssuePlace_OnSelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="ธนาคาร" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="บริษัทประกัน" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="สาขาออก" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <asp:DropDownList ID="cmbActIssueBranch_bla" runat="server" Width="150px">
                                </asp:DropDownList>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>เอกสารลงทะเบียน
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblRegisterAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbRegisterAct_pro1" runat="server" GroupName="promo1" Text="ลงทะเบียน" />
                                    <asp:RadioButton ID="rbNormalAct_pro1" runat="server" GroupName="promo1" Text="ธรรมดา" />
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbRegisterAct_pro2" runat="server" GroupName="promo2" Text="ลงทะเบียน" />
                                    <asp:RadioButton ID="rbNormalAct_pro2" runat="server" GroupName="promo2" Text="ธรรมดา" />
                                </td>
                                <td style="text-align: right">
                                    <asp:RadioButton ID="rbRegisterAct_pro3" runat="server" GroupName="promo3" Text="ลงทะเบียน" />
                                    <asp:RadioButton ID="rbNormalAct_pro3" runat="server" GroupName="promo3" Text="ธรรมดา" />
                                </td>
                                <%--<td style="text-align: right">
                                <asp:RadioButton ID="rbRegisterAct_bla" GroupName="promo3" runat="server" Text="ลงทะเบียน" /><asp:RadioButton
                                    ID="rbNormalAct_bla" GroupName="promo3" runat="server" Text="ธรรมดา" />
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>บริษัท พรบ.
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblCompanyInsuranceAct_pre" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lblActInsCom_Id_pre" runat="server" Style="display: none;" Text=""></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <%-- <asp:Label ID="lblCompanyInsuranceAct_pro1" runat="server" Text=""></asp:Label><asp:Label
                                    ID="lblActInsCom_Id_pro1" runat="server" Text="" Style="display: none;"></asp:Label>--%>
                                    <asp:DropDownList ID="lblCompanyInsuranceAct_pro1" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <td style="text-align: right">
                                    <%--<asp:Label ID="lblCompanyInsuranceAct_pro2" runat="server" Text=""></asp:Label><asp:Label
                                    ID="lblActInsCom_Id_pro2" runat="server" Text="" Style="display: none;"></asp:Label>--%>
                                    <asp:DropDownList ID="lblCompanyInsuranceAct_pro2" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <td style="text-align: right">
                                    <%--<asp:Label ID="lblCompanyInsuranceAct_pro3" runat="server" Text=""></asp:Label><asp:Label
                                    ID="lblActInsCom_Id_pro3" runat="server" Text="" Style="display: none;"></asp:Label>--%>
                                    <asp:DropDownList ID="lblCompanyInsuranceAct_pro3" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                                <%-- <td align="right">
                                <asp:DropDownList ID="cmbCompanyInsuranceAct_bla" runat="server" Width="150px">
                                </asp:DropDownList>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>เลขเครื่องหมายตาม พรบ.
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lbltxtSignNoAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:TextBox ID="txtSignNoAct_pro1" runat="server" MaxLength="250" Width="154px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <asp:TextBox ID="txtSignNoAct_pro2" runat="server" MaxLength="250" Width="154px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <asp:TextBox ID="txtSignNoAct_pro3" runat="server" MaxLength="250" Width="154px"></asp:TextBox>
                                </td>
                                <%--<td align="right">
                                <asp:TextBox ID="txtSignNoAct_bla" runat="server" MaxLength="250" Width="150px"></asp:TextBox>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>วันเริ่มต้น พรบ.
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActStartCoverDateAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActStartCoverDateAct_pro1" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActStartCoverDateAct_pro2" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActStartCoverDateAct_pro3" runat="server" Width="102px" />
                                </td>
                                <%--<td align="right">
                                <uc5:TextDateMaskWithEvent ID="txtActStartCoverDateAct_bla" runat="server" Width="130px" />
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>วันสิ้นสุด พรบ.
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActEndCoverDateAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActEndCoverDateAct_pro1" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActEndCoverDateAct_pro2" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc5:TextDateMaskWithEvent ID="txtActEndCoverDateAct_pro3" runat="server" Width="102px" />
                                </td>
                                <%--<td align="right">
                                <uc5:TextDateMaskWithEvent ID="txtActEndCoverDateAct_bla" runat="server" Width="130px" />
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>วันสิ้นสุดอายุภาษีรถยนต์
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblCarTaxExpiredDateAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <uc2:TextDateMask ID="txtCarTaxExpiredDateAct_pro1" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc2:TextDateMask ID="txtCarTaxExpiredDateAct_pro2" runat="server" Width="102px" />
                                </td>
                                <td align="right">
                                    <uc2:TextDateMask ID="txtCarTaxExpiredDateAct_pro3" runat="server" Width="102px" />
                                </td>
                                <%--<td align="right">
                                <uc2:TextDateMask ID="txtCarTaxExpiredDateAct_bla" runat="server" Width="130px" />
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>เบี้ยสุทธิ (เต็มปี)
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActNetGrossPremiumFullAct_pre" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:TextBox ID="lblActNetGrossPremiumFullAct_pro1" runat="server" AutoPostBack="true"
                                        CssClass="money" MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing()"
                                        OnTextChanged="txtActNetGrossPremiumFullAct_pro1_OnTextChanged" Style="text-align: right"
                                        Width="154px"></asp:TextBox>
                                </td>
                                <td style="text-align: right">
                                    <asp:TextBox ID="lblActNetGrossPremiumFullAct_pro2" runat="server" AutoPostBack="true"
                                        CssClass="money" MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing()"
                                        OnTextChanged="txtActNetGrossPremiumFullAct_pro2_OnTextChanged" Style="text-align: right"
                                        Width="154px"></asp:TextBox>
                                </td>
                                <td style="text-align: right">
                                    <asp:TextBox ID="lblActNetGrossPremiumFullAct_pro3" runat="server" AutoPostBack="true"
                                        CssClass="money" MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing()"
                                        OnTextChanged="txtActNetGrossPremiumFullAct_pro3_OnTextChanged" Style="text-align: right"
                                        Width="154px"></asp:TextBox>
                                </td>
                                <%--<td style="text-align: right">
                               <asp:TextBox ID="txtActNetGrossPremiumFullAct_bla" MaxLength="15" CssClass="money"
                                    Style="text-align: right" Width="150px" OnTextChanged="txtActNetGrossPremiumFullAct_bla_OnTextChanged"
                                    onblur="blockButton();" AutoPostBack="true" onchange="DisplayProcessing()" runat="server"></asp:TextBox>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>เบี้ยสุทธิ
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossPremiumAct_pre" runat="server" Text=""></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossPremiumAct_pro1" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossPremiumAct_pro2" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossPremiumAct_pro3" runat="server"></asp:Label>
                                </td>
                                <%--<td style="text-align: right">
                                <asp:Label ID="lblActGrossPremiumAct_bla" runat="server"></asp:Label>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>อากร
                                    <asp:Button ID="HiddenActBtn" runat="server" Style="display: none;" />
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossStampAct_pre" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossStampAct_pro1" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossStampAct_pro2" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossStampAct_pro3" runat="server"></asp:Label>
                                </td>
                                <%--<td style="text-align: right">
                                <asp:Label ID="lblActGrossStampAct_bla" runat="server"></asp:Label>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>ภาษี
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossVatAct_pre" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossVatAct_pro1" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossVatAct_pro2" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActGrossVatAct_pro3" runat="server"></asp:Label>
                                </td>
                                <%--<td style="text-align: right">
                                <asp:Label ID="lblActGrossVatAct_bla" runat="server"></asp:Label>
                            </td>--%>
                            </tr>
                            <tr class="normal_row">
                                <td>เบี้ย พรบ.รวมภาษีอากร
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActNetGrossPremiumAct_pre" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActNetGrossPremiumAct_pro1" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActNetGrossPremiumAct_pro2" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="lblActNetGrossPremiumAct_pro3" runat="server"></asp:Label>
                                </td>
                                <%--<td style="text-align: right">
                                <asp:Label ID="lblActNetGrossPremiumAct_bla" runat="server"></asp:Label>
                            </td>--%>
                            </tr>
                            <asp:UpdatePanel ID="pnCalAct" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <tr class="normal_row">
                                        <td>
                                            <asp:CheckBox ID="chkCardType2" runat="server" AutoPostBack="true" OnCheckedChanged="chkCardType2_OnCheckedChanged"
                                                onclick="DisplayProcessing()" />
                                            หัก 1% (กรณีนิติบุคคล)
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblActPersonType_pre" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblActPersonType_pro1" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblActPersonType_pro2" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblActPersonType_pro3" runat="server" Text=""></asp:Label>
                                        </td>
                                        <%--<td style="text-align: right">
                                        <asp:Label ID="lblActPersonType_bla" runat="server" Text=""></asp:Label>
                                    </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ส่วนลด (%)
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblVat1PercentBathAct_pre" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVat1PercentBathAct_pro1" runat="server" AutoPostBack="true" CssClass="int"
                                                MaxLength="3" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActPercent"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVat1PercentBathAct_pro2" runat="server" AutoPostBack="true" CssClass="int"
                                                MaxLength="3" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActPercent"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblVat1PercentBathAct_pro3" runat="server" AutoPostBack="true" CssClass="int"
                                                MaxLength="3" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActPercent"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <%--<td style="text-align: right">
                                        <asp:TextBox ID="lblVat1PercentBathAct_bla" runat="server" onchange="DisplayProcessing();"
                                            AutoPostBack="true" OnTextChanged="calActPercent" onblur="blockButton();" CssClass="int"
                                            Style="text-align: right" MaxLength="3" Width="150px"></asp:TextBox>
                                    </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>ส่วนลด (บาท)
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblDiscountPercentAct_pre" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblDiscountPercentAct_pro1" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActBath"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblDiscountPercentAct_pro2" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActBath"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="lblDiscountPercentAct_pro3" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="calActBath"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <%--<td style="text-align: right">
                                        <asp:TextBox ID="lblDiscountPercentAct_bla" runat="server" AutoPostBack="true" OnTextChanged="calActBath"
                                            onchange="DisplayProcessing();" MaxLength="15" onblur="blockButton();" CssClass="money"
                                            Style="text-align: right" Width="150px"></asp:TextBox>
                                    </td>--%>
                                    </tr>
                                    <tr class="normal_row">
                                        <td>เบี้ย พรบ. ที่ต้องชำระ
                                        </td>
                                        <td style="text-align: right">
                                            <asp:Label ID="lblDiscountBathAct_pre" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="txtDiscountBathAct_pro1" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="txtDiscountBathAct_OnTextChanged"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="txtDiscountBathAct_pro2" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="txtDiscountBathAct_OnTextChanged"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: right">
                                            <asp:TextBox ID="txtDiscountBathAct_pro3" runat="server" AutoPostBack="true" CssClass="money"
                                                MaxLength="15" onblur="blockButton();" onchange="DisplayProcessing();" OnTextChanged="txtDiscountBathAct_OnTextChanged"
                                                Style="text-align: right" Width="154px"></asp:TextBox>
                                        </td>
                                        <%--<td style="text-align: right">
                                        <asp:TextBox ID="txtDiscountBathAct_bla" CssClass="money" Style="text-align: right"
                                            Width="150px" OnTextChanged="txtDiscountBathAct_OnTextChanged" AutoPostBack="true"
                                            onblur="blockButton();" MaxLength="15" onchange="DisplayProcessing" runat="server"></asp:TextBox>
                                        <asp:Button ID="btnCalAct" runat="server" Visible="false" Text="คำนวณ" OnClick="btnCalAct_Click" />
                                    </td>--%>
                                    </tr>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </table>
                        <div id="divRemarkAct" runat="server">
                            <p>
                                <asp:Image ID="Image3" runat="server" alt="" ImageAlign="top" ImageUrl="~/Images/ActRemark.png" />
                            </p>
                            <asp:TextBox ID="txtRemarkAct" runat="server" MaxLength="4000" Rows="5" TextMode="MultiLine"
                                Width="800"></asp:TextBox>
                        </div>
                        <div id="divPaymentAct" runat="server">
                            <p>
                                <asp:Image ID="Image12" runat="server" alt="" ImageAlign="top" ImageUrl="~/Images/PolicyPaymentType.png" />
                            </p>
                            <asp:UpdatePanel ID="pnActPayment" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td>ประเภทการชำระเงิน
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPaymentmethodAct" runat="server" AutoPostBack="true" onchange="DisplayProcessing()"
                                                    OnSelectedIndexChanged="cmbPaymentmethodAct_OnSelectedIndexChanged" Width="100">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text="รูปแบบการชำระ" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPaymentTypeAct" runat="server" AutoPostBack="true" onchange="DisplayProcessing()"
                                                    OnSelectedIndexChanged="cmbPaymentTypeAct_OnSelectedIndexChanged" Width="100">
                                                </asp:DropDownList>
                                            </td>
                                            <td>สาขา
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPayBranchCodeAct" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                    </p>
                </asp:Panel>
                <br />
                <br />
                <asp:UpdatePanel ID="pnDocument" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:LinkButton ID="sendDocument" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                            Text="[-] <b>ที่อยู่ในการจัดส่งเอกสาร</b>" OnClick="sendDocument_Click"></asp:LinkButton>
                        <asp:TextBox ID="txtSendDocument" runat="server" Text="N" Visible="false"></asp:TextBox>
                        <asp:Panel ID="pnSendDocument" runat="server" Style="display: initial;">
                            <table>
                                <tr>
                                    <td>ที่อยู่ในการจัดส่งเอกสาร
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rdoAddressOld" AutoPostBack="true" GroupName="rdoAddress" OnCheckedChanged="rdoAddress_OnCheckedChanged"
                                            runat="server" Checked="true" onchange="DisplayProcessing()" />ที่อยู่เดิม&nbsp;&nbsp;
                                        <asp:RadioButton ID="rdoAddressChange" AutoPostBack="true" GroupName="rdoAddress"
                                            OnCheckedChanged="rdoAddress_OnCheckedChanged" runat="server" onchange="DisplayProcessing()" />เปลี่ยนแปลงที่อยู่&nbsp;&nbsp;
                                        <asp:RadioButton ID="rdoAddressBranch" AutoPostBack="true" GroupName="rdoAddress"
                                            OnCheckedChanged="rdoAddress_OnCheckedChanged" runat="server" onchange="DisplayProcessing()" />สาขา&nbsp;&nbsp;
                                        <asp:DropDownList ID="cmbBranchCodeDoc" AutoPostBack="true" runat="server" Width="300px"
                                            OnSelectedIndexChanged="cmbBranchCodeDoc_OnSelectedIndexChanged" onchange="DisplayProcessing();">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="2" style="height: 5px;"></td>
                                </tr>
                            </table>
                            <table cellpadding="2" cellspacing="0" border="1" style="border-collapse: collapse;">
                                <tr>
                                    <td style="width: 100px">เลขที่
                                    </td>
                                    <td style="width: 165px">
                                        <asp:TextBox ID="txtHouseNo" MaxLength="50" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                    <td style="width: 56px">หมู่
                                    </td>
                                    <td style="width: 165px">
                                        <asp:TextBox ID="txtMoo" MaxLength="50" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                    <td style="width: 56px">
                                        อาคาร 
                                    </td>
                                    <td style="width: 165px">
                                        <asp:TextBox ID="txtBuilding" MaxLength="500" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>ชื่ออาคาร/หมู่บ้าน
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtHouseName" MaxLength="500" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                    <td>ซอย
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSoi" MaxLength="100" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                    <td>ถนน
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStreet" MaxLength="100" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>จังหวัด
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbProvince" runat="server" Width="160px" AutoPostBack="true"
                                            OnSelectedIndexChanged="cmbProvince_SelectedIndexChanged" onchange="DisplayProcessing()">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        เขต/อำเภอ
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbDistinct" runat="server" Width="160px" AutoPostBack="true"
                                            OnSelectedIndexChanged="cmbDistinct_SelectedIndexChanged" onchange="DisplayProcessing()">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        แขวง/ตำบล
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbThambol" runat="server" Width="160px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        รหัสไปรษณีย์
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtZipCode" MaxLength="5" runat="server" Width="154px"></asp:TextBox>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                            <hr />
                            <table>
                                <tr>
                                    <td>ชื่อผู้รับเอกสาร
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtReceiver" MaxLength="500" Width="400px" runat="server"></asp:TextBox>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
                <asp:Panel ID="pnIncentive" runat="server">
                    <asp:LinkButton ID="noIncentive" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                        Text="[-] <b>เลขที่รับแจ้งและ Incentive</b>" OnClick="noIncentive_Click"></asp:LinkButton>
                    <asp:TextBox ID="txtNoIncentive" runat="server" Text="N" Visible="false"></asp:TextBox>
                    <asp:Panel ID="pnNoIncentive" runat="server" Style="display: initial;">
                        <asp:UpdatePanel ID="upIncentive" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>เลขที่รับแจ้ง
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtReceiveNo" runat="server" Width="130px" Enabled="false"></asp:TextBox>&nbsp;&nbsp;
                                            <asp:Button ID="btnReceive" runat="server" Text="รับเลขรับแจ้ง" OnClientClick="DisplayProcessing();"
                                                OnClick="btnReceive_Click" />
                                        </td>
                                        <td>วันที่กดเลขรับแจ้ง
                                        </td>
                                        <td>
                                            <%--<uc2:TextDateMask ID="tdmReceiveDate" runat="server" Enabled="false" />--%>
                                            <asp:TextBox ID="txtReceiveDate" runat="server" Width="130px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>วันที่รับ Incentive
                                        </td>
                                        <td>
                                            <%--<uc2:TextDateMask ID="tdmIncentiveDate" runat="server" Enabled="false"></uc2:TextDateMask>--%>
                                            <asp:TextBox ID="txtIncentiveDate" runat="server" Width="130px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                                            &nbsp;&nbsp;
                                            <asp:Button ID="btnIncentive" OnClick="btnIncentive_Click" OnClientClick="DisplayProcessing();"
                                                runat="server" Text="รับ Incentive" />&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>วันที่ส่งแจ้ง พรบ.
                                        </td>
                                        <td>
                                            <%--<uc2:TextDateMask ID="tdmActSendDate" runat="server" Enabled="false"></uc2:TextDateMask>--%>
                                            <asp:TextBox ID="txtActSendDate" runat="server" Width="130px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>&nbsp;&nbsp;
                                            <asp:Button ID="btnReceiveAct" runat="server" Text="ส่งแจ้งพรบ." OnClientClick="DisplayProcessing();"
                                                OnClick="btnReceiveAct_Click" />
                                        </td>
                                        <td>วันที่รับ Incentive พรบ.
                                        </td>
                                        <td>
                                            <%--<uc2:TextDateMask ID="tdmActIncentiveDate" runat="server" Enabled="false" />--%>
                                            <asp:TextBox ID="txtActIncentiveDate" runat="server" Width="130px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                                            &nbsp;&nbsp;
                                            <asp:Button ID="btnIncentiveAct" runat="server" Text="รับ Incentive" OnClientClick="DisplayProcessing();"
                                                OnClick="btnIncentiveAct_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </asp:Panel>
                <br />
                <asp:Panel ID="pnClaim" runat="server">
                    <asp:LinkButton ID="noClaim" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                        Text="[-] <b>ระงับเคลมและยกเลิกระงับเคลม</b>" OnClick="noClaim_Click"></asp:LinkButton>                    
                    <asp:Panel ID="pnNoClaim" runat="server" Style="display: initial;">
                        <asp:UpdatePanel ID="upClaim" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:RadioButtonList ID="radioClaim" runat="server">
                                    <asp:ListItem Text="ไม่ระงับเคลม" Value="-1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="ระงับเคลม" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="ยกเลิกระงับเคลม" Value="0" Enabled="false"></asp:ListItem>
                                </asp:RadioButtonList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </asp:Panel>
                <br />
                <div id="divPayment" runat="server">
                    <asp:LinkButton ID="payment" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                        Text="[-] <b>ข้อมูลการชำระเงิน</b>" OnClick="payment_Click"></asp:LinkButton>
                    <asp:TextBox ID="txtPayment" runat="server" Text="N" Visible="false"></asp:TextBox>
                    <asp:Panel ID="pnPayment" runat="server" Style="display: initial;">
                        <p>
                            <asp:Image alt="" ID="Image4" ImageUrl="~/Images/PolicyData.png" runat="server" ImageAlign="top" />
                        </p>
                        <asp:UpdatePanel ID="pnPamentPMain" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>จำนวนเงินที่ต้องชำระก่อนหักส่วนลด<asp:Button ID="HiddenPaymentBtn" runat="server"
                                                Style="display: none;" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPolicyGrossPremiumTotal" Enabled="false" CssClass="money" runat="server"
                                                Style="text-align: right">
                                            </asp:TextBox>
                                        </td>
                                        <td>จำนวนเงินที่ต้องชำระ
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPolicyGrossPremium" runat="server" Style="text-align: right">
                                            </asp:TextBox>
                                        </td>
                                        <td>จำนวนเงินที่รับชำระทั้งสิ้น
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPolicyRecAmount" runat="server" Style="text-align: right"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            ส่วนลด (%) <span style='color:red'>(คำนวนจากเบี้ยสุทธิ)</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDiscountPercent" MaxLength="3" AutoPostBack="true" OnTextChanged="DiscountPer_OnTextChanged"
                                                onblur="blockPaymentButton();" onchange="DisplayProcessing();" CssClass="int"
                                                runat="server" Style="text-align: right" Enabled="true"></asp:TextBox>
                                        </td>
                                        <td>ส่วนลด (บาท)
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPolicyDiscountAmt" MaxLength="15" AutoPostBack="true" OnTextChanged="DiscountBath_OnTextChanged"
                                                onblur="blockPaymentButton();" onchange="DisplayProcessing();" CssClass="money"
                                                runat="server" Style="text-align: right" Enabled="true"></asp:TextBox>
                                        </td>
                                        <td>ส่วนต่าง
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPolicyDiffAmt" runat="server" Style="text-align: right"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:Panel ID="pnPaymentMainPolicy" runat="server">
                            <p>
                                <asp:Image alt="" ID="Image5" ImageUrl="~/Images/PolicyPaymentTypeReceipt.png" runat="server"
                                    ImageAlign="top" />
                            </p>
                            <asp:UpdatePanel ID="pnPaymentDetail" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td>ประเภทการชำระเงิน
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPolicyPayMethod" runat="server" OnSelectedIndexChanged="cmbPolicyPayMethod_OnSelectedIndexChanged"
                                                    onchange="DisplayProcessing()" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>จำนวนงวด
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPolicyAmountPeriod" onchange="DisplayProcessing();" onblur="blockPaymentButton();"
                                                    OnTextChanged="txtPolicyAmountPeriod_OnTextChanged" AutoPostBack="true" class="int"
                                                    Style="text-align: right" runat="server"></asp:TextBox>
                                            </td>
                                            <td>รูปแบบการชำระ
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPayOption" onchange="DisplayProcessing();" OnTextChanged="cmbPayOption_OnTextChanged"
                                                    AutoPostBack="true" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>สาขา
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbPayBranchCode" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                    <div id="paymentDetail" runat="server">
                                        <table>
                                            <tr id="trPay1" runat="server">
                                                <td>วันที่ชำระงวดที่ 1
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate1" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 1
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount1" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay2" runat="server">
                                                <td>วันที่ชำระงวดที่ 2
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate2" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 2
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount2" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay3" runat="server">
                                                <td>วันที่ชำระงวดที่ 3
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate3" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 3
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount3" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay4" runat="server">
                                                <td>วันที่ชำระงวดที่ 4
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate4" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 4
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount4" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay5" runat="server">
                                                <td>วันที่ชำระงวดที่ 5
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate5" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 5
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount5" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay6" runat="server">
                                                <td>วันที่ชำระงวดที่ 6
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate6" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 6
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount6" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay7" runat="server">
                                                <td>วันที่ชำระงวดที่ 7
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate7" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 7
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount7" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay8" runat="server">
                                                <td>วันที่ชำระงวดที่ 8
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate8" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 8
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount8" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay9" runat="server">
                                                <td>วันที่ชำระงวดที่ 9
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate9" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 9
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount9" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr id="trPay10" runat="server">
                                                <td>วันที่ชำระงวดที่ 10
                                                </td>
                                                <td>
                                                    <uc2:TextDateMask ID="tdmPolicyPaymentDate10" runat="server" />
                                                </td>
                                                <td>จำนวนเงินที่ต้องชำระงวดที่ 10
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPolicyPaymentAmount10" MaxLength="15" runat="server" Style="text-align: right"></asp:TextBox>
                                                </td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                        <p>
                            <asp:Image alt="" ID="Image6" ImageUrl="~/Images/ActData.png" runat="server" ImageAlign="top" />
                        </p>
                        <table>
                            <tr>
                                <td>จำนวนเงินที่ต้องชำระก่อนหักส่วนลด
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActGrossPremiumTotal" Enabled="false" runat="server" Style="text-align: right"></asp:TextBox>
                                </td>
                                <td>จำนวนเงินที่ต้องชำระ
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActGrossPremium" runat="server" Style="text-align: right"></asp:TextBox>
                                </td>
                                <td>จำนวนเงินที่รับชำระทั้งสิ้น
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActRecAmount" runat="server" Style="text-align: right"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    ส่วนลด (%) <span style='color:red'>(คำนวนจากเบี้ยสุทธิ)</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDiscountPercentAct" MaxLength="3" AutoPostBack="true" OnTextChanged="DiscountPerAct_OnTextChanged"
                                        onblur="blockPaymentButton();" onchange="DisplayProcessing();" CssClass="int"
                                        runat="server" Style="text-align: right" Enabled="true"></asp:TextBox>
                                </td>
                                <td>ส่วนลด (บาท)
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActDiscountAmt" MaxLength="15" AutoPostBack="true" OnTextChanged="DiscountBathAct_OnTextChanged"
                                        onblur="blockPaymentButton();" onchange="DisplayProcessing();" CssClass="money"
                                        runat="server" Style="text-align: right" Enabled="true"></asp:TextBox>
                                </td>
                                <td>ส่วนต่าง
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActDiffAmt" runat="server" Style="text-align: right"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <asp:Panel ID="pnPaymentMainAct" runat="server">
                            <p>
                                <asp:Image alt="" ID="Image7" ImageUrl="~/Images/ActPaymentTypeReceipt.png" runat="server"
                                    ImageAlign="top" />
                            </p>
                            <table>
                                <tr>
                                    <td>ประเภทการชำระเงิน
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbActPayOption" runat="server" AutoPostBack="true" onchange="DisplayProcessing();">
                                        </asp:DropDownList>
                                    </td>
                                    <td>รูปแบบการชำระ
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbActPayMethod" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>สาขา
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbActPayBranchCode" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <p>
                            <asp:Image alt="" ID="Image8" ImageUrl="~/Images/ReciptList.png" runat="server" ImageAlign="top" />
                        </p>
                        <asp:UpdatePanel ID="pnReceipt" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView ID="gvReceipt" runat="server" AutoGenerateColumns="False" Width="883px"
                                    OnRowDataBound="gvReceipt_OnRowDataBound" OnRowCommand="gvReceipt_OnRowCommand"
                                    GridLines="Horizontal" OnPreRender="gvReceipt_OnPreRender" BorderWidth="0px"
                                    EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                                    <Columns>
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hidcountExport" runat="server" Value='<%# Eval("countExport") %>' />
                                                <asp:HiddenField ID="hidRenewInsuranceReceiptId" runat="server" Value='<%# Eval("slm_RenewInsuranceReceiptId") %>' />
                                                <asp:ImageButton ImageUrl="../../images/view.gif" ID="imgReceipt" runat="server"
                                                    CommandName="view" CommandArgument='<%# Eval("slm_RenewInsuranceReceiptId") %>' />
                                                <%--<asp:Label ID="lblPositionNameAbb" runat="server" Text='<%# Eval("TITLENAME") %>'></asp:Label>--%>
                                            </ItemTemplate>
                                            <HeaderStyle Width="20px" HorizontalAlign="Center" />
                                            <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="เลขที่ใบเสร็จ">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRecNo" runat="server" Text='<%# Eval("slm_RecNo") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                            <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="จำนวนเงิน" DataFormatString="{0:###,##0.00}" DataField="total">
                                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                            <ItemStyle Width="120px" HorizontalAlign="Right" VerticalAlign="Top" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="สถานะใบเสร็จ">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hidStatus" runat="server" Value='<%# Eval("slm_Status") %>' />
                                                <asp:DropDownList ID="cmbStatus" Width="180px" runat="server" AutoPostBack="true"
                                                    OnSelectedIndexChanged="cmbStatus_OnSelectedIndexChanged" onchange="DisplayProcessing();">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="01" Text="แก้ไขใบเสร็จแบบปรับปรุง"></asp:ListItem>
                                                    <asp:ListItem Value="02" Text="แก้ไขใบเสร็จแบบยกเลิก"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                            <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                            <ItemStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:Button ID="btnReceipt" runat="server" Text="Edit" Width="100px" class='<%# Eval("slm_RecNo") %>'
                                                    OnClientClick='return editReceipt(this);' CommandName="show" CommandArgument='<%# Eval("slm_RenewInsuranceReceiptId") %>' />
                                            </ItemTemplate>
                                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                </asp:GridView>
                                <div runat="server" id="divimport">
                                    <p>
                                        <asp:Image alt="" ID="Image9" ImageUrl="~/Images/PaymentList.png" runat="server"
                                            ImageAlign="top" />
                                    </p>
                                    <asp:GridView ID="gvImport" runat="server" AutoGenerateColumns="False" Width="900px"
                                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                                        <Columns>
                                            <asp:TemplateField HeaderText="No" Visible="false">
                                                <ItemTemplate>
                                                    <%--<asp:Label ID="lblPositionNameAbb" runat="server" Text='<%# Eval("TITLENAME") %>'></asp:Label>--%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="วันที่นำเข้าข้อมูล">
                                                <ItemTemplate>
                                                    <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("HH:mm:ss") : ""%>
                                                    <%--<asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("slm_CreatedDate") %>'></asp:Label>--%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="วันที่รับชำระ">
                                                <%--DataField="slm_TransDate" DataFormatString="{0:dd/MM/yyyy}"--%>
                                                <ItemTemplate>
                                                    <%# Eval("slm_TransDate") != null ? Convert.ToDateTime(Eval("slm_TransDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_TransDate")).Year.ToString()  : ""%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ชำระค่า">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPaymentCode" runat="server" Text='<%# Eval("slm_paymentdesc") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ประเภทการจ่ายเงิน">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRecBy" runat="server" Text='<%# Eval("slm_RecBy") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="จำนวนเงิน" DataField="slm_RecAmount" DataFormatString="{0:N}">
                                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                                <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="t_rowhead" />
                                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                    </asp:GridView>
                                </div>
                                <asp:HiddenField ID="hidRenewInsuranceReceiptId" runat="server" />
                                <asp:HiddenField ID="hidRenewInsuranceStatus" runat="server" />
                                <asp:HiddenField ID="hidRenewInsuranceReceiptAmount" runat="server" />
                                <div runat="server" id="divEditReceipt">
                                    <p>
                                        <asp:Image alt="" ID="Image10" ImageUrl="~/Images/RevisionReceipt.png" runat="server"
                                            ImageAlign="top" />
                                    </p>
                                    <table id="tabEditReceipt" cellpadding="2" style="border-width: 0px; width: 480px; border-collapse: collapse;">
                                        <tr class="t_rowhead">
                                            <td style="width: 10px"></td>
                                            <td style="width: 60%">รายการแก้ไขใบเสร็จ
                                            </td>
                                            <td style="width: 40%">จำนวนเงิน<asp:Button ID="HiddenReceiptBtn" runat="server" Style="display: none;" />
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc1" onchange="DisplayProcessing();" AutoPostBack="true"
                                                    OnCheckedChanged="chkPaymentDesc1_OnCheckedChange" runat="server" />
                                            </td>
                                            <td style="width: 49%">ค่าเบี้ยประกันภัยปีต่ออายุ
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc1" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" AutoPostBack="true" runat="server"
                                                    onchange="DisplayProcessing();" Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc2" AutoPostBack="true" OnCheckedChanged="chkPaymentDesc2_OnCheckedChange"
                                                    onchange="DisplayProcessing();" runat="server" />
                                            </td>
                                            <td style="width: 49%">ค่าเบี้ยพรบ. ปีต่ออายุ
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc2" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" AutoPostBack="true" runat="server"
                                                    onchange="DisplayProcessing();" Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc3" AutoPostBack="true" OnCheckedChanged="chkPaymentDesc3_OnCheckedChange"
                                                    onchange="DisplayProcessing();" runat="server" />
                                            </td>
                                            <td style="width: 49%">ค่างวดรถยนต์
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc3" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" AutoPostBack="true" runat="server"
                                                    onchange="DisplayProcessing();" Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc4" AutoPostBack="true" OnCheckedChanged="chkPaymentDesc4_OnCheckedChange"
                                                    onchange="DisplayProcessing();" runat="server" />
                                            </td>
                                            <td style="width: 49%">ค่าต่อภาษีรถยนต์
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc4" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" AutoPostBack="true" runat="server"
                                                    onchange="DisplayProcessing();" Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc5" AutoPostBack="true" OnCheckedChanged="chkPaymentDesc5_OnCheckedChange"
                                                    onchange="DisplayProcessing();" runat="server" />
                                            </td>
                                            <td style="width: 49%">เจ้าหนี้อื่นๆ (ประกัน)
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc5" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" AutoPostBack="true" runat="server"
                                                    onchange="DisplayProcessing();" Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td style="width: 20px">
                                                <asp:CheckBox ID="chkPaymentDesc6" AutoPostBack="true" OnCheckedChanged="chkPaymentDesc6_OnCheckedChange"
                                                    onchange="DisplayProcessing();" runat="server" />
                                            </td>
                                            <td style="width: 49%">อื่นๆระบุ&nbsp;&nbsp;
                                                <asp:TextBox ID="txtPaymentOther" onblur="blockReceiptButton();" OnTextChanged="txtPaymentDesc_OnTextChanged"
                                                    onchange="DisplayProcessing();" AutoPostBack="true" runat="server" Width="115"></asp:TextBox>
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDesc6" MaxLength="15" CssClass="money" onblur="blockReceiptButton();"
                                                    OnTextChanged="txtPaymentDesc_OnTextChanged" onchange="DisplayProcessing();" AutoPostBack="true" runat="server"
                                                    Style="text-align: right"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr class="t_row" style="border-style: Dashed;">
                                            <td colspan="2" align="center">รวมทั้งสิ้น
                                            </td>
                                            <td style="width: 49%">
                                                <asp:TextBox ID="txtPaymentDescTotal" runat="server" Style="text-align: right" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnSaveReceipt" runat="server" Text="Apply" Width="80px" OnClientClick="DisplayProcessing();"
                                                    OnClick="btnSaveReceipt_OnClick" />&nbsp;&nbsp;
                                                <asp:Button ID="btnCancelReceipt" runat="server" Text="Cancel" Width="80px" OnClientClick="DisplayProcessing();"
                                                    OnClick="btnCancelReceipt_OnClick" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
                <br />
                <div id="divProblem" runat="server">
                    <asp:LinkButton ID="problem" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()"
                        Text="[-] <b>งานติดปัญหา</b>" OnClick="problem_Click"></asp:LinkButton>
                    <asp:TextBox ID="txtproblem" runat="server" Text="N" Visible="false"></asp:TextBox>
                    <asp:Panel ID="pnProblem" runat="server" Style="display: initial;">
                        <p>
                            <asp:Image alt="" ID="Image11" ImageUrl="~/Images/ProblemList.png" runat="server"
                                ImageAlign="top" />
                        </p>
                        <asp:GridView ID="gvProblem" runat="server" AutoGenerateColumns="False" Width="1100px"
                            OnRowDataBound="gvProblem_OnRowDataBound" GridLines="Horizontal" BorderWidth="0px"
                            EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                            <Columns>
                                <asp:TemplateField HeaderText="No" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="slm_ProblemDetailId" runat="server" Text='<%# Eval("slm_ProblemDetailId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="วันที่แจ้งปัญหา" DataField="slm_ProblemDate" DataFormatString="{0:dd/MM/yyyy}"
                                    HtmlEncode="false">
                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="ชื่อบริษัทประกัน">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInsNameTh" runat="server" Text='<%# Eval("slm_InsNameTh") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                                    <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ประเภทงานติดปัญหา">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPositionNameAbb" runat="server" Text='<%# Eval("slm_InsType") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="เลขที่สัญญา">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContract_Number" runat="server" Text='<%# Eval("slm_Contract_Number") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ชื่อ-นามสกุลลูกค้า">
                                    <ItemTemplate>
                                        <asp:Label ID="lbName" runat="server" Text='<%# Eval("slm_Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                                    <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ปัญหา">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProblemDetail" runat="server" Text='<%# Eval("slm_ProblemDetail") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="สาเหตุ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCauseDetail" runat="server" Text='<%# Eval("slm_CauseDetail") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="350px" HorizontalAlign="Center" />
                                    <ItemStyle Width="350px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="สถานะงานติดปัญหา">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hidFixTypeFlag" Value='<%# Eval("slm_FixTypeFlag") %>' runat="server" />
                                        <asp:DropDownList ID="cmbFixTypeFlag" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cmbFixTypeFlag_OnSelectedIndexChanged"
                                            onchange="DisplayProcessing();" Enabled='<%# !(bool)Eval("slm_Export_Flag") %>'>
                                            <asp:ListItem Value="1" Text="อยู่ระหว่างดำเนินการ"> </asp:ListItem>
                                            <asp:ListItem Value="2" Text="แก้ไขเสร็จเรียบร้อย"> </asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                    <ItemStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="แจ้งผลกลับ">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtResponseDetail" MaxLength="4000" runat="server" Text='<%# Eval("slm_ResponseDetail") %>' Enabled='<%# !(bool)Eval("slm_Export_Flag") %>'
                                            Width="180px"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                    <ItemStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="หมายเหตุ">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtRemark" runat="server" MaxLength="4000" Text='<%# Eval("slm_Remark") %>' Enabled='<%# !(bool)Eval("slm_Export_Flag") %>'
                                            Width="180px"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                    <ItemStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                        </asp:GridView>
                    </asp:Panel>
                    <br />
                </div>
                <asp:Panel ID="Panel3" runat="server" Style="padding-right: 5px">
                    <asp:Button ID="Button1" runat="server" CssClass="Button" Text="ยกเลิก" Width="140px"
                        Style="float: right; padding-right: 5px" OnClientClick="DisplayProcessing();"
                        Visible="false" />&nbsp;&nbsp;
                    <asp:Button ID="Button2" runat="server" CssClass="Button" Text="บันทึก" Width="140px"
                        OnClick="btnSave_PreleadData_Click" OnClientClick="return save004();" Style="float: right; padding-right: 5px" />&nbsp;&nbsp;<asp:Button ID="Button3" runat="server" CssClass="Button"
                            Text="ประวัติการเสนอขาย" Width="140px" Style="float: right; padding-right: 5px"
                            OnClick="btnHistoryMain_Click" OnClientClick="DisplayProcessing();" />
                </asp:Panel>
                <br />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Begin Promotion Popup Section -->
    <asp:UpdatePanel ID="upPopupPromotion" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupPromotion" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupPromotion" Style="display: none" CssClass="modalPopupPromotion">
                <div style="height: 600px; overflow: auto">
                    <table cellpadding="2" style="border-width: 0px; width: 480px; border-collapse: collapse;">
                        <tr>
                            <td colspan="2" style="height: 5px;"></td>
                        </tr>
                        <tr class="t_rowhead">
                            <td style="width: 49%">ข้อมูลประกัน
                            </td>
                            <td style="width: 49%">รายละเอียด
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td style="width: 49%">ชื่อ บจ.ประกันภัย
                            </td>
                            <td style="width: 49%">
                                <asp:Label ID="lblinsname" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>Campaign Name
                            </td>
                            <td>
                                <asp:Label ID="lblcamname" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ระยะเวลาคุ้มครอง
                            </td>
                            <td>
                                <asp:Label ID="lblslm_DurationYear" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>Effective Date From
                            </td>
                            <td>
                                <asp:Label ID="lblEffectiveDateFrom" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>Effective Date To
                            </td>
                            <td>
                                <asp:Label ID="lblEffectiveDateTo" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ยี่ห้อ
                            </td>
                            <td>
                                <asp:Label ID="lblbrandname" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>รุ่น
                            </td>
                            <td>
                                <asp:Label ID="lblmodelname" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ประเภทรถยนต์/ลักษณะการใช้งาน
                            </td>
                            <td>
                                <asp:Label ID="lblUseCarType" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ประเภทความคุ้มครอง
                            </td>
                            <td>
                                <asp:Label ID="lblconveragetypename" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>อายุผู้ขับขี่
                            </td>
                            <td>
                                <asp:Label ID="lblAgeDrivenFlag" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ประเภทการซ่อม
                            </td>
                            <td>
                                <asp:Label ID="lblrepairname" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>อายุรถ(ปี)
                            </td>
                            <td>
                                <asp:Label ID="lblAgeCarYear" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ขนาดเครื่องยนต์/น้ำหนักบรรทุก
                            </td>
                            <td>
                                <asp:Label ID="lblEngineSize" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ความเสียหายต่อตัวรถยนต์ (OD)
                            </td>
                            <td>
                                <asp:Label ID="lblslm_OD" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>กรณีรถยนต์สูญหาย/ไฟไหม้ (F&T)
                            </td>
                            <td>
                                <asp:Label ID="lblslm_FT" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ความรับผิดชอบส่วนแรก
                            </td>
                            <td>
                                <asp:Label ID="lblDeDuctible" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>เบี้ยประกันภัยสุทธิ
                            </td>
                            <td>
                                <asp:Label ID="lblGrossPremium" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>อากร
                            </td>
                            <td>
                                <asp:Label ID="lblStamp" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ภาษี
                            </td>
                            <td>
                                <asp:Label ID="lblslm_Vat" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>เบี้ยรวมภาษีและอากร
                            </td>
                            <td>
                                <asp:Label ID="lblNetGrossPremium" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>พรบ.
                            </td>
                            <td>
                                <asp:Label ID="lblAct" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ความรับผิดชอบต่อความบาดเจ็บ/เสียชีวิต
                            </td>
                            <td>
                                <asp:Label ID="lblInjuryDeath" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ความรับผิดชอบต่อทรัพย์สินคู่กรณี (TPPD)
                            </td>
                            <td>
                                <asp:Label ID="lblTPPD" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ประกันภัยอุบัติเหตุส่วนบุคคล (ร.ย. 01) ผู้ขับขี่และผู้โดยสาร
                            </td>
                            <td>
                                <asp:Label ID="lblPersonalAccident" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ผู้ขับขี่
                            </td>
                            <td>
                                <asp:Label ID="lblPersonalAccidentDriver" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ผู้โดยสาร
                            </td>
                            <td>
                                <asp:Label ID="lblPersonalAccidentPassenger" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ค่ารักษาพยาบาล (ร.ย.02)
                            </td>
                            <td>
                                <asp:Label ID="lblMedicalFee" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ผู้ขับขี่
                            </td>
                            <td>
                                <asp:Label ID="lblMedicalFeeDriver" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>ผู้โดยสาร
                            </td>
                            <td>
                                <asp:Label ID="lblMedicalFeePassenger" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>การประกันตัวผู้ขับขี่ (ร.ย.03)
                            </td>
                            <td>
                                <asp:Label ID="lblInsuranceDriver" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr class="t_row" style="border-style: Dashed;">
                            <td>เงื่อนไขพิเศษอื่นๆ
                            </td>
                            <td>
                                <asp:Label ID="lblRemark" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <asp:Button ID="btnCancelPopupPromotion" OnClientClick="DisplayProcessing();" runat="server"
                                    Text="ปิด" Width="90px" CssClass="Button" OnClick="btnCancelPopupPromotion_Click" />
                                <td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupPromotion" runat="server" TargetControlID="btnPopupPromotion"
                PopupControlID="pnPopupPromotion" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopupCancel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupCancel" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupCancel" Style="display: none" CssClass="modalPopupCancel">
                <table cellpadding="2" style="border-width: 0px; width: 400px; border-collapse: collapse;">
                    <tr>
                        <td colspan="2" style="height: 5px;"></td>
                    </tr>
                    <tr>
                        <td style="width: 15%; padding-left: 15px;">
                            <asp:Image alt="" ID="Image13" ImageUrl="~/Images/CancelReason.png" runat="server"
                                ImageAlign="top" />
                        </td>
                        <td style="width: 85%">
                            <asp:HiddenField ID="hidCancelType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height: 5px;"></td>
                    </tr>
                    <tr>
                        <td style="width: 15%; padding-left: 15px;">เหตุผลการยกเลิก
                        </td>
                        <td style="width: 85%">
                            <asp:DropDownList ID="cmbCancelReason" runat="server" CssClass="Dropdownlist" Width="230px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%; padding-left:15px;">
                            วันที่ยกเลิก
                        </td>
                        <td style="width: 85%">
                            <asp:Label ID="lblCancelDate" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height: 15px;"></td>
                    </tr>
                    <tr>
                        <td style="width: 15%; padding-left: 15px;"></td>
                        <td>
                            <asp:Button ID="btnCancel" OnClick="btnCancel_OnClick" OnClientClick="DisplayProcessing();"
                                Text="บันทึก" runat="server" Width="80" />&nbsp;
                            <asp:Button ID="btnCloseCancel" OnClick="btnCloseCancel_OnClick" OnClientClick="DisplayProcessing();"
                                Text="ยกเลิก" runat="server" Width="80" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupCancel" runat="server" TargetControlID="btnPopupCancel"
                PopupControlID="pnPopupCancel" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPopupReport" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupReport" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupReport" Style="display: none" CssClass="modalPopupBPReport">
                <br />
                &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkBP" runat="server" Text="ต้องการออกรายงานแจ้งออก BP" onchange="BPReportCheckChange()" />
                <div style="padding-left:30px;">
                    <table cellpadding="2" style="border-width: 0px;border-collapse: collapse;">
                        <tr>
                            <td style="height: 5px;"></td><td></td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountDue" runat="server">
                            <td style="width:273px;">
                                จำนวนเงินประกันที่ต้องชำระ
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountDue" runat="server" CssClass="TextboxViewR" ReadOnly="true" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_ActAmountDue" runat="server">
                            <td style="width:273px;">
                                จำนวนเงินพรบ.ที่ต้องชำระ
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_ActAmountDue" runat="server" CssClass="TextboxViewR" ReadOnly="true" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="2" style="border-width: 0px;border-collapse: collapse;">
                        <tr>
                            <td style="height: 5px;" colspan="3"></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                จำนวนเงินในรายงานแจ้งออก BP
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 5px;" colspan="3"></td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify1" runat="server">
                            <td style="width:150px;">
                                <asp:CheckBox ID="cbBPPopup_PolicyAmountSpecify" runat="server" Enabled="false" Text="ประกัน" onchange="CheckBoxBPSpecifyChange()" />
                            </td>
                            <td style="text-align:right; width:120px;">
                                <asp:Label ID="lblBPPopup_PolicyAmountSpecify1" runat="server" Text="จำนวนเงิน"></asp:Label>&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify1" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify2" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 2&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify2" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify3" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 3&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify3" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify4" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 4&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify4" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify5" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 5&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify5" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify6" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 6&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify6" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify7" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 7&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify7" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify8" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 8&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify8" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify9" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 9&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify9" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_PolicyAmountSpecify10" runat="server" visible="false">
                            <td style="width:150px;">
                            </td>
                            <td style="text-align:right;">
                                จำนวนเงินงวดที่ 10&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_PolicyAmountSpecify10" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBPPopup_ActAmountSpecify" runat="server">
                            <td style="width:150px;">
                                <asp:CheckBox ID="cbBPPopup_ActAmountSpecify" runat="server" Enabled="false" Text="พรบ." onchange="CheckBoxBPSpecifyChange()" />
                            </td>
                            <td style="text-align:right; width:120px;">
                                จำนวนเงิน&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtBPPopup_ActAmountSpecify" runat="server" Enabled="false" CssClass="TextboxR" Width="100px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:15px;" colspan="3"></td>
                        </tr>
                        <tr>
                            <td colspan="3" style="text-align:right;">
                                <asp:Button ID="btnSaveAll" OnClick="btnSaveAll_Click" OnClientClick="DisplayProcessing();"
                                    Text="บันทึก" runat="server" Width="90px" />&nbsp;
                                <asp:Button ID="btnCloseReport" OnClick="btnCloseReport_OnClick" OnClientClick="DisplayProcessing();"
                                    Text="ยกเลิก" runat="server" Width="90px" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupReport" runat="server" TargetControlID="btnPopupReport"
                PopupControlID="pnPopupReport" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPopupReceive" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupReceive" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupReceive" Style="display: none" CssClass="modalPopupReceive">
                <table cellpadding="2" style="border-width: 0px; width: 200px; border-collapse: collapse;">
                    <tr>
                        <td style="height: 5px;"></td>
                    </tr>
                    <tr>
                        <td>เลือกรายการที่จะข้อมูลไปยังรายงาน
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 5px;"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkClaim" runat="server" Text="รายงานแจ้งระงับเคลม" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 15px;"></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnSaveReceive" OnClick="btnSaveReceive_Click" OnClientClick="DisplayProcessing();"
                                Text="บันทึก" runat="server" Width="80" />&nbsp;
                            <asp:Button ID="btnCloseReceive" OnClick="btnCloseReceive_Click" OnClientClick="DisplayProcessing();"
                                Text="ยกเลิก" runat="server" Width="80" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupReceive" runat="server" TargetControlID="btnPopupReceive"
                PopupControlID="pnPopupReceive" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc3:ActRenewInsureSnap ID="ucActRenewInsureSnap" runat="server" />
    <uc4:ActRenewInsureContract ID="ucActRenewInsureContact" runat="server" OnUpdatedDataChanged="UpdateStatusDesc" />
</div>
