<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeadInfo.ascx.cs" Inherits="SLM.Application.Shared.LeadInfo" %>
<%@ Register src="TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>

<%@ Register src="GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>

<style type="text/css">
    .style1
    {
        width: 50px;
    }
    .style2
    {
        width: 180px;
        text-align:left;
        font-weight:bold;
    }
    .style3
    {
        width: 280px;
        text-align:left;
    }
    .style4
    {
        font-family: Tahoma;
        font-size: 9pt;
        color: Red;
    }
    .style5
    {
        width: 955px;
    }
    .style6
    {
        font-family: Tahoma;
        font-size: 9pt;
        color: blue;
    }
</style>
<script language="javascript" type="text/javascript">
    function GetScreenResolution() {

        document.getElementById('<%= txtScreenWidth.ClientID %>').value = screen.width;
        document.getElementById('<%= txtScreenHeight.ClientID %>').value = screen.height;
    }
    </script>
<br />
<asp:UpdatePanel ID="upHeader1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0" >
            <tr>
                <td colspan="5">
                    <asp:Image ID="imgHeader1" runat="server" ImageUrl="~/Images/hGeneral.gif" />
                </td>
            </tr>
            <tr>
                <td class="style2"><asp:Label  id="lbTicketID" runat="server" Text="Ticket ID" Visible="false" ></asp:Label> </td>
                <td class="style3" colspan="4">
                    <asp:TextBox ID="txtslm_TicketId" runat="server" CssClass="Textbox" Width="250px" ReadOnly="true" Visible="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ชื่อ <span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                    <asp:Label ID="vtxtName" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">นามสกุล</td>
                <td class="style3">
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="Textbox" Width="250px" MaxLength="120" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">แคมเปญ <span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCampaignId" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbCampaignId_SelectedIndexChanged"  ></asp:DropDownList>
                    <asp:ImageButton ID="imbSearchCampaign" runat="server" ImageAlign="AbsMiddle" ToolTip="ค้นหา"  
                        ImageUrl="~/Images/iSearch.gif" onclick="imbSearchCampaign_Click" OnClientClick="GetScreenResolution()" />
                    <asp:Label ID="vcmbCampaignId" runat="server" CssClass="style4"></asp:Label>
                    <asp:TextBox ID="txtScreenHeight" runat="server" CssClass="Hidden"></asp:TextBox>
                    <asp:TextBox ID="txtScreenWidth" runat="server" CssClass="Hidden"></asp:TextBox>
                    <asp:TextBox ID="txtOwnerBranchUserLogin" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtOwnerBranchBefore" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtOwnerLeadBefore" runat="server" CssClass="Hidden" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ผลิตภัณฑ์/บริการ ที่สนใจ</td>
                <td class="style3">
                    <asp:TextBox ID="txtInterestedProd" runat="server" CssClass="Textbox" Width="250px" MaxLength="500" ></asp:TextBox>
                </td>
            </tr>
             <tr style="vertical-align:top;">
                <td class="style2">Owner Branch</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbOwnerBranch_SelectedIndexChanged" ></asp:DropDownList>
                </td>
                 <td class="style1"></td>
                <td class="style2">Owner lead</td>
                <td class="style3">
                     <asp:DropDownList ID="cmbOwner" runat="server" Width="253px" 
                         CssClass="Dropdownlist" Enabled="false" AutoPostBack="True" 
                         onselectedindexchanged="cmbOwner_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbOwner" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader2" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0" >
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hContactDetail.gif" />
                </td>
            </tr>
            <tr>
                <td class="style2">เรื่อง</td>
                <td class="style3">
                    <asp:TextBox ID="txtTopic" runat="server" CssClass="Textbox" Width="250px" MaxLength="50" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ช่องทาง <span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbChannelId" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="true" 
                        onselectedindexchanged="cmbChannelId_SelectedIndexChanged" ></asp:DropDownList>
                     <asp:Label ID="vcmbChannelId" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">สาขา</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbBranch" runat="server" Width="253px" CssClass="Dropdownlist" ></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">บริษัท</td>
                <td class="style3">
                    <asp:TextBox ID="txtCompany" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2" valign="top"> รายละเอียด</td>
                <td colspan="4">
                    <asp:TextBox ID="txtDetail" runat="server" CssClass="Textbox" Width="770px" Height="70px" TextMode ="MultiLine"  MaxLength="4000" ></asp:TextBox>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader3" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/hLeadDetail.gif" />
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbIsCustomer" runat="server" Width="253px" CssClass="Dropdownlist" >
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                        <asp:ListItem Value="0" Text="ไม่เคย"></asp:ListItem>
                        <asp:ListItem Value="1" Text="เคย"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">รหัสลูกค้า</td>
                <td class="style3">
                    <asp:TextBox ID="txtCusCode" runat="server" CssClass="Textbox" Width="250px" MaxLength="20" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">ประเภทบุคคล</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbCardType" runat="server" Width="253px" AutoPostBack="true"
                        CssClass="Dropdownlist" onselectedindexchanged="cmbCardType_SelectedIndexChanged" >
                    </asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">เลขที่สัญญาที่เคยมีกับธนาคาร</td>
                <td class="style3">
                    <asp:TextBox ID="txtContractNoRefer" runat="server" CssClass="Textbox" Width="250px" MaxLength="50" ></asp:TextBox>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">เลขที่บัตร<asp:Label ID="lblCitizenId" runat="server" ForeColor="Red"></asp:Label></td>
                <td class="style3" >
                    <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Enabled="false" Width="250px" MaxLength="13" ontextchanged="txtCitizenId_TextChanged" AutoPostBack="True" ></asp:TextBox>
                    <br />
                    <asp:Label ID="vtxtCitizenId" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">วันเกิด</td>
                <td class="style3">
                    <uc1:TextDateMask ID="tdBirthdate" runat="server" Width="230px" />
                </td>
            </tr>
            <tr>
                <td class="style2">อาชีพ</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbOccupation" runat="server" Width="253px" CssClass="Dropdownlist" ></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">ฐานเงินเดือน</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtBaseSalary" runat="server" CssClass="TextboxR" 
                        Width="250px" AutoPostBack="True" MaxLength="15"
                        ontextchanged="txtBaseSalary_TextChanged" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtBaseSalary" runat="server" CssClass="TextboxR" 
                        Width="250px" MaxLength="15" ></asp:TextBox>
                    <br />
                    <asp:Label ID="vtxtBaseSalary" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">หมายเลขโทรศัพท์ 1(มือถือ)<span class="style4">*</span></td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo_1" runat="server" CssClass="Textbox" MaxLength="10"  
                    Width="250px" AutoPostBack="True" ontextchanged="txtslm_TelNo_1_TextChanged" ></asp:TextBox>
                <asp:Label ID="vtxtTelNo_1" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">สาขาที่สะดวกให้ติดต่อกลับ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbContactBranch" runat="server" Width="253px" CssClass="Dropdownlist" ></asp:DropDownList>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">หมายเลขโทรศัพท์ 2</td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo2" runat="server" CssClass="Textbox" Width="170px" 
                        MaxLength="10" AutoPostBack="True" ontextchanged="txtTelNo2_TextChanged" ></asp:TextBox>
                    <asp:Label ID="label1" runat="server" Width="10px" CssClass="LabelC" Text="-"></asp:Label>
                    <asp:TextBox ID="txtExt2" runat="server" CssClass="Textbox" Width="57px" MaxLength="50" ></asp:TextBox>
                    <asp:Label ID="vtxtTelNo2" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">เวลาที่สะดวกให้ติดต่อกลับ</td>
                <td class="style3">
                    <asp:TextBox ID="txtAvailableTimeHour" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                    <asp:Label ID="label4" runat="server" CssClass="LabelC" Text=":" Width="5px" ></asp:Label>
                    <asp:TextBox ID="txtAvailableTimeMinute" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                    <asp:Label ID="label3" runat="server" CssClass="LabelC" Text=":" Width="5px" ></asp:Label>
                    <asp:TextBox ID="txtAvailableTimeSecond" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                    <br />
                    <asp:Label ID="vtxtAvailableTime" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">หมายเลขโทรศัพท์ 3</td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo3" runat="server" CssClass="Textbox" Width="170px" 
                        MaxLength="10" AutoPostBack="True" ontextchanged="txtTelNo3_TextChanged" ></asp:TextBox>
                    <asp:Label ID="label2" runat="server" Width="10px" CssClass="LabelC" Text="-"></asp:Label>
                    <asp:TextBox ID="txtExt3" runat="server" CssClass="Textbox" Width="57px" MaxLength="50" ></asp:TextBox>
                    <asp:Label ID="vtxtTelNo3" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">E-Mail</td>
                <td class="style3">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="Textbox" Width="250px" 
                        MaxLength="100" AutoPostBack="True" ontextchanged="txtEmail_TextChanged" ></asp:TextBox>
                    <asp:Label ID="vtxtEmail" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader4" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/hAddressDetail.gif" />
                </td>
            </tr>
            <tr>
                <td class="style2">เลขที่</td>
                <td class="style3">
                    <asp:TextBox ID="txtAddressNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ชื่ออาคาร/หมู่บ้าน</td>
                <td class="style3">
                    <asp:TextBox ID="txtBuildingName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ชั้น</td>
                <td class="style3">
                    <asp:TextBox ID="txtFloor" runat="server" CssClass="Textbox" Width="250px" MaxLength="10" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ซอย</td>
                <td class="style3">
                    <asp:TextBox ID="txtSoi" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ถนน</td>
                <td class="style3">
                    <asp:TextBox ID="txtStreet" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">จังหวัด</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbProvince" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbProvince_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">เขต/อำเภอ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbAmphur" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbAmphur_SelectedIndexChanged"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">แขวง/ตำบล</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbTambol" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">รหัสไปรษณีย์</td>
                <td class="style3" colspan="4">
                    <asp:TextBox ID="txtPostalCode" runat="server" CssClass="Textbox" Width="250px" MaxLength="5" ></asp:TextBox>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader5" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/hCalculateDetail.gif" />
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">ประเภทความสนใจ<br />(รถใหม่/รถเก่า)</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCarType" runat="server" Width="253px" CssClass="Dropdownlist">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                        <asp:ListItem Value="0" Text="รถใหม่"></asp:ListItem>
                        <asp:ListItem Value="1" Text="รถเก่า"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">ทะเบียนรถ</td>
                <td class="style3">
                    <asp:TextBox ID="txtLicenseNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">จังหวัดที่จดทะเบียน</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbProvinceRegis" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">ปีรถ</td>
                <td class="style3">
                    <asp:TextBox ID="txtYearOfCar" runat="server" CssClass="Textbox" Width="250px" MaxLength="4" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ปีที่จดทะเบียนรถยนต์</td>
                <td class="style3">
                    <asp:TextBox ID="txtYearOfCarRegis" runat="server" CssClass="Textbox" Width="250px" MaxLength="4" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ยี่ห้อรถ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbBrand" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbBrand_SelectedIndexChanged"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">รุ่นรถ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbModel" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbModel_SelectedIndexChanged"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">รุ่นย่อยรถ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbSubModel" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">ราคารถยนต์</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtCarPrice" runat="server" CssClass="TextboxR" Width="250px" 
                         AutoPostBack="True" ontextchanged="txtCarPrice_TextChanged" 
                        MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtCarPrice" runat="server" CssClass="TextboxR" Width="250px" MaxLength="13" ></asp:TextBox>
                    <asp:Label ID="vtxtCarPrice" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">เงินดาวน์</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtDownPayment" runat="server" CssClass="TextboxR" 
                        Width="250px" AutoPostBack="True" 
                        ontextchanged="txtDownPayment_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                     <asp:TextBox ID="txtDownPayment" runat="server" CssClass="TextboxR" Width="250px" MaxLength="13" ></asp:TextBox>
                     <asp:Label ID="vtxtDownPayment" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">เปอร์เซ็นต์เงินดาวน์</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtDownPercent" runat="server" CssClass="Textbox" 
                        Width="250px" AutoPostBack="True" 
                        ontextchanged="txtDownPercent_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtDownPercent" runat="server" CssClass="Textbox" Width="250px" MaxLength="6" ></asp:TextBox>
                    <asp:Label ID="vtxtDownPercent" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">ยอดจัด Finance</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtFinanceAmt" runat="server" CssClass="TextboxR" 
                        Width="250px" AutoPostBack="True" 
                        ontextchanged="txtFinanceAmt_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtFinanceAmt" runat="server" CssClass="TextboxR" Width="250px" MaxLength="13" ></asp:TextBox>
                    <asp:Label ID="vtxtFinanceAmt" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">ระยะเวลาผ่อนชำระ</td>
                <td class="style3">
                    <asp:TextBox ID="txtPaymentTerm" runat="server" CssClass="Textbox" Width="250px" MaxLength="4" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ประเภทการผ่อนชำระ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbPaymentType" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
            </tr>
             <tr>
                <td class="style2">Balloon Amount</td>
                <td class="style3">
                     <%--<asp:TextBox ID="txtBalloonAmt" runat="server" CssClass="TextboxR" 
                         Width="250px" AutoPostBack="True" 
                         ontextchanged="txtBalloonAmt_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtBalloonAmt" runat="server" CssClass="TextboxR" Width="250px" MaxLength="13" ></asp:TextBox>
                     <asp:Label ID="vtxtBalloonAmt" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">Balloon Percent</td>
                <td class="style3">
                    <%--<asp:TextBox ID="txtBalloonPercent" runat="server" CssClass="Textbox" 
                        Width="250px" AutoPostBack="True" 
                        ontextchanged="txtBalloonPercent_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtBalloonPercent" runat="server" CssClass="Textbox" Width="250px" MaxLength="6" ></asp:TextBox>
                    <asp:Label ID="vtxtBalloonPercent" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader6" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image5" runat="server" ImageUrl="~/Images/hInsurance.gif" />
                </td>
            </tr>
            <tr>
                <td class="style2">ประเภทกรมธรรม์</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbPlanType" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">วันที่เริ่มต้นคุ้มครอง</td>
                <td class="style3">
                    <uc1:TextDateMask ID="tdCoverageDate" runat="server" Width="230px" />
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader7" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="5">
                    <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/hProductOther.gif" />
                </td>
            </tr>
            <tr>
                <td class="style2">ประเภทเงินฝาก</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbAccType" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">โปรโมชั่นเงินฝากที่สนใจ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbAccPromotion" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">ระยะเวลาฝาก Term</td>
                <td class="style3">
                     <asp:TextBox ID="txtAccTerm" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">อัตราดอกเบี้ยที่สนใจ</td>
                <td class="style3">
                    <asp:TextBox ID="txtInterest" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">เงินฝาก/เงินลงทุน</td>
                <td class="style3">
                     <asp:TextBox ID="txtInvest" runat="server" CssClass="TextboxR" Width="250px" MaxLength="13" ></asp:TextBox>
                     <asp:Label ID="vtxtInvest" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">สินเชื่อ Over Draft</td>
                <td class="style3">
                    <asp:TextBox ID="txtLoanOd" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ระยะเวลา Over Draft</td>
                <td class="style3">
                     <asp:TextBox ID="txtLoanOdTerm" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">สนใจ E-Banking</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbEbank" runat="server" Width="253px" CssClass="Dropdownlist">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                            <asp:ListItem Value="0" Text="ไม่สนใจ"></asp:ListItem>
                            <asp:ListItem Value="1" Text="สนใจ"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">สนใจ ATM</td>
                <td class="style3">
                     <asp:DropDownList ID="cmbAtm" runat="server" Width="253px" CssClass="Dropdownlist">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                            <asp:ListItem Value="0" Text="ไม่สนใจ"></asp:ListItem>
                            <asp:ListItem Value="1" Text="สนใจ"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3">
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader8" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr>
                <td colspan="2">
                    <asp:Image ID="Image7" runat="server" ImageUrl="~/Images/hAttach.gif" />
                </td>
            </tr>
            <tr>
                <td style="width:180px; font-weight:bold;">Path Link</td>
                <td class="style3">
                    <asp:TextBox ID="txtPathLink" runat="server" CssClass="Textbox" Width="770px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel>
<br />
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table  class="style5">
            <tr>
                <td align="right">
                    <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" 
                        Width="90px" onclick="btnSave_Click"  OnClientClick="DisplayProcessing()" /> &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                        Width="90px" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่')" 
                        onclick="btnCancel_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 

<asp:UpdatePanel ID="upPopupSearchCampaign" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSearchCampaign" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSearchCampaign" style="display:none" CssClass="modalBoxlSearchCampaign" ScrollBars="Auto">
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/SearchCampaign.jpg" />
		    <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="width:20px;"></td>
                    <td>
                    </td>
                    <td style="width:220px; font-weight:bold;">กลุ่มผลิตภัณฑ์/บริการ</td>
                    <td style="width:220px; font-weight:bold;">ผลิตภัณฑ์/บริการ</td>
                    <td style="font-weight:bold; ">แคมเปญ</td>
                </tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td>
                        <asp:RadioButton ID="rbSearchByCombo" runat="server" GroupName="Campaign" AutoPostBack="true"
                            Checked="true" oncheckedchanged="rbSearchByCombo_CheckedChanged"  />
                    </td>
                    <td style="width:220px;">
                        <asp:DropDownList ID="cmbProductGroup" runat="server" AutoPostBack="true" 
                            CssClass="Dropdownlist" Width="200px" 
                            onselectedindexchanged="cmbProductGroup_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td style="width:220px;">
                        <asp:DropDownList ID="cmbProduct" runat="server" AutoPostBack="true" 
                            CssClass="Dropdownlist" Width="200px" 
                            onselectedindexchanged="cmbProduct_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="200px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td>
                    </td>
                    <td colspan="3" style="font-weight:bold;" >คำที่ต้องการค้นหา</td>
                </tr>
                <tr >
                    <td style="width:20px;"></td>
                    <td>
                        <asp:RadioButton ID="rbSearchByText" runat="server" GroupName="Campaign" AutoPostBack="true" 
                            oncheckedchanged="rbSearchByText_CheckedChanged"  />
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txtFullSearchCampaign" runat="server" CssClass="Textbox" Width="420px"></asp:TextBox>
                    </td>
                    <td>
                        
                    </td>
                </tr>
                <tr><td colspan="5" style="height:3px;"></td></tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td colspan="4">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSearchCampaign" runat="server" Text="ค้นหา" Width="80px" 
                            CssClass="Button" OnClientClick="DisplayProcessing()" onclick="btnSearchCampaign_Click" />
                    </td>
                </tr>
                <tr><td colspan="5" style="height:3px;"></td></tr>
            </table>
            <hr style="border-top:1px solid gray; border-bottom-style:none; border-left-style:none; border-right-style:none;" />
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="width:20px;"></td>
                    <td style="height:340px; vertical-align:top; ">
                        <asp:Label ID="label400" Text="* เลือกแคมเปญได้ 1 รายการเท่านั้น" runat="server" ForeColor="Red" ></asp:Label><br /><br />
                        <uc2:GridviewPageController ID="pcGridCampaign" runat="server" OnPageChange="PageSearchChange" Width="910px" />
                        <asp:GridView ID="gvCampaign" runat="server" AutoGenerateColumns="False" Width="910px" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"
                            GridLines="Horizontal" BorderWidth="1px" EnableModelValidation="True" 
                            onrowdatabound="gvCampaign_RowDataBound"  >
                            <Columns>
                                <asp:TemplateField HeaderText="เลือก">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server" AutoPostBack="true" OnCheckedChanged="cbSelect_CheckedChanged" TabIndex="<%# Container.DisplayIndex %>" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="35px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="35px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductGroupName" HeaderText="กลุ่มผลิตภัณฑ์/บริการ"  >
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ"  >
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="รายละเอียดแคมเปญ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Eval("CampaignDesc") %>'></asp:Label>
                                        <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="205px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="205px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                                    <ItemTemplate>
                                        <%# Eval("StartDate") != null ? Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                    <ItemTemplate>
                                        <%# Eval("EndDate") != null ? Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td >
                        <asp:Button id="btnClose" runat="server" Text="ปิดหน้าต่าง" Width="100px" 
                            onclick="btnClose_Click" />
                    </td>
                </tr>
            </table>
            <br />
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSearchCampaign" runat="server" TargetControlID="btnPopupSearchCampaign" PopupControlID="pnPopupSearchCampaign" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="upPopupSaveResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSaveResult" Width="0px" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSaveResult" style="display:none" CssClass="modalPopupCreateLeadResult">
            <br />
		    <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:1px;"></td></tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo" style="font-size:14px; width:380px;">
                        <b>บันทึกข้อมูลผู้มุ่งหวังสำเร็จ</b>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:5px;"></td></tr>
                 <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>Ticket Id:</b>&nbsp;<asp:Label ID="lblResultTicketId" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>แคมเปญ:</b>&nbsp;<asp:Label ID="lblResultCampaign" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>ช่องทาง:</b>&nbsp;<asp:Label ID="lblResultChannel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>Owner Lead:</b>&nbsp;<asp:Label ID="lblResultOwnerLead" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:8px;"></td></tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <asp:Label ID="lblResultMessage" runat="server"></asp:Label>
                        <asp:CheckBox ID="cbResultHasAdamsUrl" runat="server" Text="CallAdams" Enabled="false" Visible="false" />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td align="center" colspan="2" class="ColInfo">
                        <asp:Button ID="btnAttachDocYes" runat="server" Text="ใช่" CssClass="Button" 
                            Width="100px" OnClick="btnAttachDocYes_Click" />&nbsp;
                        <asp:Button ID="btnAttachDocNo" runat="server" Text="ไม่ใช่" CssClass="Button" 
                            Width="100px" OnClick="btnAttachDocNo_Click"  />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSaveResult" runat="server" TargetControlID="btnPopupSaveResult" PopupControlID="pnPopupSaveResult" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>