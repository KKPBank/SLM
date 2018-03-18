<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActRenewInsureContact.ascx.cs"
    Inherits="SLM.Application.Shared.Obt.ActRenewInsureContact" %>
<%@ Register Src="../TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc2" %>
<%@ Register Src="../TextDateMaskWithEvent.ascx" TagName="TextDateMaskWithEvent" TagPrefix="uc3" %>

<style type="text/css">
    .modalPopupActRenewInsureContact
    {
        background-color: #ffffff;
        border-width: 1px;
        border-style: solid;
        border-color: Gray;
        padding: 3px;
        width:890px;
        height:600px;
    }
</style>
<script language="javascript" type="text/javascript">
    function ConfirmContactDetailSave() {
        var detail = document.getElementById('<%= txtContactDetail.ClientID %>').value;
        if (detail.trim() != '') {
            return confirm('ต้องการบันทึกใช่หรือไม่');
        }
    }
    
    function TelNoSmsChange() {
        var textboxSms = document.getElementById('<%= txtTelNoSms.ClientID %>');
        var alertSms = document.getElementById('<%= vTelNoSms.ClientID %>');
        if (textboxSms.value.trim() == '') {
            textboxSms.value = document.getElementById('<%= txtTelNo1.ClientID %>').value;
            alertSms.innerHTML = '';
        }
    }

    //Modified By Pom 24/05/2016 - validate file uploaded for old browser IE < 10, Firefox 3
    function checkBeforeSave() {
        var hdf = document.getElementById('<%= hdfSubStatusType.ClientID %>');
        var i = 0;
        if ($("[id$=txtContactDetail]").val() == "") {
            i += 1;
            $("[id*=vtxtContactDetail]").text("กรุณากรอกข้อมูลรายละเอียดก่อนทำการบันทึก");
        }
        else {
            $("[id*=vtxtContactDetail]").text("");
        }

        if ($("[id$=cmbLeadStatus]").val() == "") {
            i += 1;
            $("[id*=vcmbLeadStatus]").text("กรุณาระบุสถานะของ Lead");
        }
        else {
            $("[id*=vcmbLeadStatus]").text("");
        }

        if ($("[id*=ucActRenewInsureContact_cmbCardType]").val() == ""){            
            $("[id*=vtxtCitizenId]").text("");
        }
        else {
            if ($("[id*=ucActRenewInsureContact_txtCitizenId]").val() == "")
            {
                i += 1;
                $("[id*=vtxtCitizenId]").text("กรุณาระบุเลขที่บัตร");
            }
            else
                $("[id*=vtxtCitizenId]").text("");
        }
        
        if ($("[id$=cmbLeadSubStatus]").val() == "") {
            i += 1;
            $("[id*=vcmbLeadSubStatus]").text("กรุณาระบุสถานะย่อยของ Lead");
        }
        else {
            $("[id*=vcmbLeadSubStatus]").text("");
        }

        if($("[id$=cmbLeadStatus]").val() != 08 )
        {
            if( $("[id$=cmbLeadStatus]").val() != 09 )
            {
                if (ispolicychecked() == false && isactchecked() == false){
                    alert('ไม่สามารถบันทึกได้ เนื่องจากไม่พบข้อมูลการซื้อประกันหรือพรบ.');
                    return false;
                }
            }
        }

        if ($("[id$=cmbOwnerBranch]").val() != "" && $("[id$=cmbOwner]").val() == "") {
            $("[id$=vcmbOwner]").text("กรุณาระบุ Owner Lead");
            i += 1;
        }
        else {
            $("[id$=vcmbOwner]").text("");
        }

        if ($("[id$=cmbDelegateBranch]").val() != "" && $("[id$=cmbDelegate]").val() == "") {
            $("[id$=vcmbDelegate]").text("กรุณาระบุ Delegate Lead");
            i += 1;
        }
        else {
            $("[id$=vcmbDelegate]").text("");
        }

        //Comment by nung 2017/05/04
        //if ($("[id$=cmbDelegateBranch]").val() != $("[id*=txtOldDelegateBranch]").val() || $("[id$=cmbDelegate]").val() != $("[id*=txtOldDelegate]").val()){
        
        //    if ($("[id$=cmbDelegateBranch]").val() == "") {
        //        $("[id*=vcmbDelegate]").text("กรุณาระบุ Delegate Lead");
        //        i += 1;
        //    }
        //    else {
        //        $("[id*=vcmbDelegate]").text("");
        //    }
        //}

        //if($("[id=cmbLeadSubStatus]").val())
        //if (ispolicychecked() == false && isactchecked() == false){
        //    alert('ไม่สามารถบันทึกได้ เนื่องจากไม่พบข้อมูลการซื้อประกันหรือพรบ.');
        //    return false;
        //}

        //validate files upload
        var fuCreditCard = document.getElementById('<%= fuCreditCard.ClientID %>');
        var alertCreditCard = document.getElementById('<%= vlblCreditCard.ClientID %>');
        var fu50tawi = document.getElementById('<%= fu50tawi.ClientID %>');
        var alert50tawi = document.getElementById('<%= vlbl50tawi.ClientID %>');
        var fuDriverlicense = document.getElementById('<%= fuDriverlicense.ClientID %>');
        var alertDriverlicense = document.getElementById('<%= vlblDriverlicense.ClientID %>');

        if (fuCreditCard != null) {
            if (fuCreditCard.files[0] != null) { 
                var validImageSize = <%= SLM.Application.Utilities.AppConstant.MaximumEcmFileUploadSize %>;
                var filePath = fuCreditCard.value;
                var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                if (ext != 'jpg' && ext != 'jpeg' && ext != 'png' && ext != 'pdf') {
                    alertCreditCard.innerHTML = 'กรุณาระบุไฟล์ให้ถูก format (jpg, jpeg, png, pdf)';
                    i += 1;
                }
                else if (fuCreditCard.files[0].size > validImageSize) {
                    var mb = validImageSize / 1048576;
                    alertCreditCard.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                    i += 1;
                }
                else
                    alertCreditCard.innerHTML = '';
            }
        }
        if (fu50tawi != null) {
            if (fu50tawi.files[0] != null) {
                var validImageSize = <%= SLM.Application.Utilities.AppConstant.MaximumEcmFileUploadSize %>;
                var filePath = fu50tawi.value;
                var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                if (ext != 'jpg' && ext != 'jpeg' && ext != 'png' && ext != 'pdf') {
                    alert50tawi.innerHTML = 'กรุณาระบุไฟล์ให้ถูก format (jpg, jpeg, png, pdf)';
                    i += 1;
                }
                else if (fu50tawi.files[0].size > validImageSize) {
                    var mb = validImageSize / 1048576;
                    alert50tawi.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                    i += 1;
                }
                else
                    alert50tawi.innerHTML = '';
            }
        }
        if (fuDriverlicense != null) {
            if (fuDriverlicense.files[0] != null) {
                var validImageSize = <%= SLM.Application.Utilities.AppConstant.MaximumEcmFileUploadSize %>;
                var filePath = fuDriverlicense.value;
                var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                if (ext != 'jpg' && ext != 'jpeg' && ext != 'png' && ext != 'pdf') {
                    alertDriverlicense.innerHTML = 'กรุณาระบุไฟล์ให้ถูก format (jpg, jpeg, png, pdf)';
                    i += 1;
                }
                else if (fuDriverlicense.files[0].size > validImageSize) {
                    var mb = validImageSize / 1048576;
                    alertDriverlicense.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                    i += 1;
                }
                else
                    alertDriverlicense.innerHTML = '';
            }
        }

        if (i > 0) {
            return false;
        }
        else {
            // client validate
            var st = $("#<%= cmbLeadStatus.ClientID %>").val();
            if ($("#<%= txtOldStatus.ClientID %>").val() != $("#<%= cmbLeadStatus.ClientID %>").val() && (st == "08" || st == "09" || st == "10") ) {
                if ($("#<%= hdfPaidPolicy.ClientID %>").val() == "N") {
                    alert('ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน');
                    return false;
                }

                if ($("#<%= hdfPaidAct.ClientID %>").val() == "N") {
                    alert('ไม่สามารถบันทึกข้อมูลได้เนื่องจากเงินที่ต้องชำระกับเงินที่รับชำระไม่เท่ากัน');
                    return false;
                }
            }

            var goon = true;

            // มีการซื้อประกันแต่ sub status เป็น พรบ
            if (ispolicychecked() && hdf.value == '2') goon = confirm('สถานะที่เลือกไม่ตรงกับข้อมูลการเลือกซื้อ ต้องการบันทึกข้อมูลใช่หรือไม่');

            // ไม่มีการซื้อประกัน แต่ sub status เป็น ประกัน
            if (ispolicychecked() == false && isactchecked() && hdf.value == '1') goon = confirm('สถานะที่เลือกไม่ตรงกับข้อมูลการเลือกซื้อ ต้องการบันทึกข้อมูลใช่หรือไม่');

            if (goon) { 
                DisplayProcessing();
                return true;
            }
            else return false;
        }
    }

    $(document).on("click", "[id*=clearCreditCard]", function (e) {
        if ($("[id*=fuCreditCard]") != null) {
            $("[id*=fuCreditCard]").attr("type", "input");
            $("[id*=fuCreditCard]").attr("type", "file");
        }
        //Added By Pom 24/05/2016 - Clear alert message
        if ($("[id*=vlblCreditCard]") != null) {
            $("[id*=vlblCreditCard]").text("");
        }
    });

    $(document).on("click", "[id*=clear50tawi]", function (e) {
        if ($("[id*=fu50tawi]") != null) {
            $("[id*=fu50tawi]").attr("type", "input");
            $("[id*=fu50tawi]").attr("type", "file");
        }
        //Added By Pom 24/05/2016 - Clear alert message
        if ($("[id*=vlbl50tawi]") != null) {
            $("[id*=vlbl50tawi]").text("");
        }
    });

    $(document).on("click", "[id*=clearDriverlicense]", function (e) {
        if ($("[id*=fuDriverlicense]") != null) {
            $("[id*=fuDriverlicense]").attr("type", "input");
            $("[id*=fuDriverlicense]").attr("type", "file");
        }
        //Added By Pom 24/05/2016 - Clear alert message
        if ($("[id*=vlblDriverlicense]") != null) {
            $("[id*=vlblDriverlicense]").text("");
        }
    });

    $(document).on("change", "[id*=fuCreditCard]", function (e) {
        check_extension($(this).val(), this);
    });

    $(document).on("change", "[id*=fu50tawi]", function (e) {
        check_extension($(this).val(), this);
    });

    $(document).on("change", "[id*=fuDriverlicense]", function (e) {
        check_extension($(this).val(), this);
    });

    //Added By Pom 24/05/2016
    $(document).on("change", "[id*=cmbLeadSubStatus]", function (e) {
        if ($("[id$=cmbLeadSubStatus]").val() != "") {
            $("[id*=vcmbLeadSubStatus]").text("");
        }
    });

    function check_extension(filename, submitId) {

        var ext = filename.toLowerCase();
        if (ext.indexOf('.jpg') >= 0) {
            //submitEl.disabled = false;
            return true;
        } else if (ext.indexOf('.jpeg') >= 0) {
            //submitEl.disabled = false;
            return true;
        } else if (ext.indexOf('.png') >= 0) {
            //submitEl.disabled = false;
            return true;
        } else if (ext.indexOf('.pdf') >= 0) {
            return true;
        } else {
            //alert("Invalid filename, please select another file");
            //submitEl.disabled = true;
            $(submitId).attr("type", "input");
            $(submitId).attr("type", "file");

            alert("กรุณาระบุไฟล์ให้ถูก format (jpeg, jpg, png, pdf)")
            return false;
        }
    }
</script>

<asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden" />
<asp:Panel runat="server" ID="pnPopup" Style="display: none;" CssClass="modalPopupActRenewInsureContact" ScrollBars="Auto">
    <br />
    <asp:UpdatePanel ID="upSection1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hFollow.gif" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblTab008Info" runat="server" ForeColor="Red"></asp:Label>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="5" style="height: 1px;"></td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">เลขที่สัญญา
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtContractNo" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                    </td>
                    <td class="ColInfo1">Ticket ID
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTicketID" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                        <asp:TextBox ID="txtAssignedFlag" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtDelegateFlag" runat="server" Width="40px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">ชื่อ
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                    </td>
                    <td class="ColInfo2">นามสกุล
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtLastname" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">ประเภทบุคคล
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCardType" runat="server" CssClass="Dropdownlist" Width="203px"
                            AutoPostBack="true" OnSelectedIndexChanged="cmbCardType_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="vtxtCardType" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                    <td class="ColInfo2">เลขที่บัตร<asp:Label ID="lblCitizenId" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="200px" Enabled="false"
                            AutoPostBack="true" OnTextChanged="txtCitizenId_TextChanged"></asp:TextBox>
                        <br />
                        <asp:Label ID="vtxtCitizenId" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">แคมเปญ<asp:TextBox ID="txtCampaignId" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtProductGroupId" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtProductId" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtChannelId" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtPreleadId" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtProductGroupName" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtProductName" runat="server" Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtLoginName" runat="server" Width="40px" Visible="false"></asp:TextBox>
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtCampaign" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                    </td>
                    <td class="ColInfo2">ประเทศ<asp:Label ID="lblCountryId" runat="server" ForeColor="Red"></asp:Label></td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCountry" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                        <asp:Label ID="vcmbCountry" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:HiddenField runat="server" ID="hdfPaidAct" />
            <asp:HiddenField runat="server" ID="hdfPaidPolicy" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upDropdownlist" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">Owner Branch<asp:TextBox ID="txtOldOwnerBranch" runat="server" Width="40px"></asp:TextBox>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="203px" CssClass="Dropdownlist"
                            AutoPostBack="True" OnSelectedIndexChanged="cmbOwnerBranch_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="vcmbOwnerBranch" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                    <td class="ColInfo2">Owner Lead<asp:TextBox ID="txtOldOwner" runat="server" Width="40px"></asp:TextBox>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbOwner" runat="server" CssClass="Dropdownlist" Width="203px"
                            AutoPostBack="true" OnSelectedIndexChanged="cmbOwner_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="vcmbOwner" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">Delegate Branch<asp:TextBox ID="txtOldDelegateBranch" runat="server" Width="40px"></asp:TextBox>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbDelegateBranch" runat="server" Width="203px" CssClass="Dropdownlist"
                            AutoPostBack="True" OnSelectedIndexChanged="cmbDelegateBranch_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="vcmbDelegateBranch" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                    <td class="ColInfo2">Delegate lead<asp:TextBox ID="txtOldDelegate" runat="server" Width="40px"></asp:TextBox>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbDelegate" runat="server" CssClass="Dropdownlist" Width="203px"
                            AutoPostBack="True" OnSelectedIndexChanged="cmbDelegateLead_SelectedIndexChanged">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="vcmbDelegate" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upStatusSection" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">หมายเลขโทรศัพท์
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTelNo1" runat="server" CssClass="TextboxView" ReadOnly="true"
                            Width="200px"></asp:TextBox>
                        <asp:TextBox ID="txtExt1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="46px"
                            Visible="false"></asp:TextBox>
                    </td>
                    <td class="ColInfo2">
                        หมายเลข SMS
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTelNoSms" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" onblur="TelNoSmsChange()"></asp:TextBox>
                        <br />
                        <asp:Label ID="vTelNoSms" runat="server" CssClass="MsgAlert" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">สถานะของ Lead<span style="color: Red">*</span>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbLeadStatus" runat="server" Width="203px" CssClass="Dropdownlist" Enabled="false">
                        </asp:DropDownList>
                        <asp:TextBox ID="txtOldStatus" runat="server" Width="20px"></asp:TextBox>
                        <asp:Label ID="vcmbLeadStatus" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                    <td class="ColInfo2">หมายเลขโทรศัพท์ที่ติดต่อลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtContactPhone" runat="server" CssClass="Textbox" Width="200px"
                            MaxLength="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1" style="vertical-align: top;">สถานะย่อยของ Lead<span style="color: Red">*</span>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbLeadSubStatus" runat="server" Width="203px" CssClass="Dropdownlist" onchange="DisplayProcessing();" AutoPostBack="true" OnSelectedIndexChanged="cmbLeadSubStatus_OnSelectedIndexChanged">
                        </asp:DropDownList><br />
                        <asp:TextBox ID="txtOldSubStatus" runat="server" Width="20px"></asp:TextBox>
                        <asp:Label ID="vcmbLeadSubStatus" runat="server" CssClass="MsgAlert"></asp:Label>
                        <asp:HiddenField runat="server" ID="hdfSubStatusType" />
                    </td>
                    <td class="ColInfo1" style="font-weight: normal;">
                        <asp:CheckBox ID="chkFollowpolicy" runat="server" Text="ลูกค้าติดตามเลขกรมธรรม์" />
                        <asp:CheckBox ID="chkFollowpolicyOld" Visible="false" runat="server" Text="" />
                    </td>
                    <td class="ColInput">
                        <asp:CheckBox ID="chkFollowAct" runat="server" Text="ลูกค้าติดตามเลข พรบ." />
                        <asp:CheckBox ID="chkFollowActOld" Visible="false" runat="server" Text="" />
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">วันที่นัดหมายครั้งต่อไป
                    </td>
                    <td class="ColInput">
                        <asp:UpdatePanel runat="server" ID="upAppointment" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc3:TextDateMaskWithEvent ID="txtAppointment" runat="server" Width="180px" CssClass="Dropdownlist"></uc3:TextDateMaskWithEvent>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td class="ColInfo1" style="font-weight: normal;">
                        <asp:CheckBox ID="chkBlacklist" runat="server" Text="Blacklist" ForeColor="Red" />
                        <asp:HiddenField ID="hidBlacklist" runat="server" />
                    </td>
                    <td class="ColInput"></td>
                </tr>
                <tr>
                    <td colspan="5" style="height: 2px;"></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upBrowseFile" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr id="divCreditCard" runat="server" style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">ฟอร์มตัดบัตรเครดิต
                    </td>
                    <td class="ColInput" colspan="3">
                        <asp:LinkButton ID="lblCreditCard" OnClick="lblCreditCard_OnClick" Enabled="false" runat="server" Style="vertical-align: top;"></asp:LinkButton>
                        <asp:ImageButton runat="server" Width="16px" ImageUrl="../../images/close_icon.png" Style="vertical-align: top;" ID="imgCreditCard" OnClick="imgCreditCard_OnClick" Visible="false" />
                        <asp:FileUpload ID="fuCreditCard" runat="server" accept="image/png, image/jpeg, image/jpg, application/pdf" Width="360px" />
                        <asp:Label ID="clearCreditCard" Style="cursor: pointer;" ForeColor="blue" runat="server">Clear</asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label3" runat="server" CssClass="MsgAlert" Text="Size ไม่เกิน 3 MB (jpg, jpeg, png, pdf)"></asp:Label>
                        <br />
                        <asp:Label ID="vlblCreditCard" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
                <tr id="div50tawi" runat="server" style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">ฟอร์มสำเนา 50 ทวิ
                    </td>
                    <td class="ColInput" colspan="3">
                        <asp:LinkButton ID="lbl50tawi" OnClick="lbl50tawi_OnClick" Enabled="false" runat="server" Style="vertical-align: top;"></asp:LinkButton>
                        <asp:ImageButton runat="server" ID="img50tawi" OnClick="img50tawi_OnClick" Width="16px" Style="vertical-align: top;" ImageUrl="../../images/close_icon.png" Visible="false" />
                        <asp:FileUpload ID="fu50tawi" runat="server" accept="image/png, image/jpeg, image/jpg, application/pdf" Width="360px" />
                        <asp:Label ID="clear50tawi" Style="cursor: pointer;" ForeColor="blue" runat="server">Clear</asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label2" runat="server" CssClass="MsgAlert" Text="Size ไม่เกิน 3 MB (jpg, jpeg, png, pdf)"></asp:Label>
                        <br />
                        <asp:Label ID="vlbl50tawi" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
                <tr id="divDriverlicense" runat="server" style="vertical-align: top;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1">สำเนาใบขับขี่
                    </td>
                    <td class="ColInput" colspan="3">
                        <asp:LinkButton ID="lblDriverlicense" OnClick="lblDriverlicense_OnClick" Enabled="false" runat="server" Style="vertical-align: top;"></asp:LinkButton>
                        <asp:ImageButton runat="server" OnClick="imgDriverlicense_OnClick" Width="16px" ImageUrl="../../images/close_icon.png" Style="vertical-align: top;" ID="imgDriverlicense" Visible="false" />
                        <asp:FileUpload ID="fuDriverlicense" runat="server" accept="image/png, image/jpeg, image/jpg, application/pdf" Width="360px" />
                        <asp:Label ID="clearDriverlicense" Style="cursor: pointer;" ForeColor="blue" runat="server">Clear</asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label1" runat="server" CssClass="MsgAlert" Text="Size ไม่เกิน 3 MB (jpg, jpeg, png, pdf)"></asp:Label>
                        <br />
                        <asp:Label ID="vlblDriverlicense" runat="server" CssClass="MsgAlert"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1" valign="top">รายละเอียดเพิ่มเติม<span style="color: Red">*</span>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtContactDetail" runat="server" CssClass="Textbox" TextMode="MultiLine"
                            Rows="5" Width="638px"></asp:TextBox>
                    </td>
                </tr>
                <tr style="height: 35px;">
                    <td class="ColIndent2"></td>
                    <td class="ColInfo1" valign="top"></td>
                    <td colspan="2" style="vertical-align: top; width: 425px;">
                        <asp:Label ID="vtxtContactDetail" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="98px" OnClick="btnSave_Click"
                            OnClientClick="return checkBeforeSave();" />&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" Width="98px" OnClick="btnCancel_Click"
                            OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่?')" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Panel>
<act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup"
    BackgroundCssClass="modalBackground" DropShadow="True">
</act:ModalPopupExtender>
<%--
<act:ModalPopupExtender ID="mpeValidateConfirm" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup"
    BackgroundCssClass="modalBackground" DropShadow="True">
</act:ModalPopupExtender>
<asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:PostBackTrigger ControlID="btnSave" />
    </Triggers>
    <ContentTemplate>
        
    </ContentTemplate>
    
</asp:UpdatePanel>--%>
<asp:LinkButton runat="server" ID="lnbTmpWarning"></asp:LinkButton>
<act:ModalPopupExtender runat="server" ID="zPopWarning" TargetControlID="lnbTmpWarning" PopupControlID="pnlWarning" BackgroundCssClass="modalBackground" DropShadow="true"></act:ModalPopupExtender>
<asp:Panel runat="server" ID="pnlWarning" CssClass="modalPopupReceive" style="width:400px; height: 130px; display:none;" >
    <asp:UpdatePanel runat="server" ID="updWarning">
        <ContentTemplate>
            <div style="margin: 5px; padding: 10px 20px; text-align: center;">
                <b><asp:Label runat="server" ID="lblWarningLabel"></asp:Label></b>
                <hr style="margin-top: 35px; margin-bottom: 15px;" />
                <asp:Button runat="server" ID="btnWarningOK" Text="OK"  OnClick="btnWarningOK_Click" OnClientClick="DisplayProcessing()"/>
                <asp:Button runat="server" ID="btnWarningCancel" Text="Cancel" OnClick="btnWarningCancel_Click" OnClientClick="DisplayProcessing()" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

