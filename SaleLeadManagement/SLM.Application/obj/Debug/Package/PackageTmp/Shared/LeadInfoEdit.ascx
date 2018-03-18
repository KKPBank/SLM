<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeadInfoEdit.ascx.cs" Inherits="SLM.Application.Shared.LeadInfoEdit" %>
<%@ Register src="TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>

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
        width: 200px;
        text-align:left;
        font-weight:bold;
    }
       .style7
    {
        width: 220px;
        text-align:left;
        font-weight:bold;
    }
      .style8
    {
        font-family: Tahoma;
        font-size: 9pt;
        color: blue;
    }
</style>
<br />
<asp:UpdatePanel ID="upHeader1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0" >
            <tr>
                <td>
                    <asp:Image ID="imgHeader1" runat="server" ImageUrl="~/Images/hGeneral.gif" />
                </td>
                <td colspan="4">
                    <asp:Label ID="lblAlert" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2"><asp:Label  id="lbTicketID" runat="server" Text="Ticket ID"  ></asp:Label> </td>
                <td class="style3" >
                    <asp:TextBox ID="txtTicketId" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">สถานะของ Lead</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbStatus" runat="server" Width="253px" 
                        CssClass="Dropdownlist" Enabled="false"  ></asp:DropDownList>
                    <asp:TextBox ID="txtOldStatus" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtAssignFlag" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtDelegateFlag" runat="server" CssClass="Hidden" ></asp:TextBox>
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
                        CssClass="Dropdownlist" Enabled="false"  ></asp:DropDownList>
                    <asp:Label ID="vcmbCampaignId" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">ผลิตภัณฑ์/บริการ ที่สนใจ</td>
                <td class="style3">
                    <asp:TextBox ID="txtInterestedProd" runat="server" CssClass="Textbox" Width="250px" MaxLength="500" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">วันเวลาที่ติดต่อ Lead ล่าสุด</td>
                <td class="style3">
                    <asp:TextBox ID="txtContactLatestDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">วันเวลาที่ได้รับมอบหมายล่าสุด</td>
                <td class="style3">
                    <asp:TextBox ID="txtAssignDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">วันเวลาที่ติดต่อ Lead ครั้งแรก</td>
                <td class="style3" colspan="4">
                    <asp:TextBox ID="txtContactFirstDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
             <tr style="vertical-align:top;">
                <td class="style2">Owner Branch</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbOwnerBranch_SelectedIndexChanged"  ></asp:DropDownList>
                    <asp:Label ID="vcmbOwnerBranch" runat="server" CssClass="style4" ></asp:Label>
                </td>
                 <td class="style1"></td>
                <td class="style2">Owner lead</td>
                <td class="style3">
                     <asp:DropDownList ID="cmbOwner" runat="server" Width="253px" 
                         CssClass="Dropdownlist" AutoPostBack="True" 
                         onselectedindexchanged="cmbOwner_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbOwner" runat="server" CssClass="style4" ></asp:Label>
                    <asp:TextBox ID="txtOldOwner" runat="server" CssClass="Hidden" ></asp:TextBox>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">Delegate Branch</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbDelegateBranch" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbDelegateBranch_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbDelegateBranch" runat="server" CssClass="style4" ></asp:Label>
                </td>
                 <td class="style1"></td>
                <td class="style2">Delegate lead</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbDelegateLead" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbDelegateLead_SelectedIndexChanged"  ></asp:DropDownList>
                    <asp:Label ID="vcmbDelegateLead" runat="server" CssClass="style4"></asp:Label>
                    <asp:TextBox ID="txtoldDelegate" runat="server" CssClass="Hidden" ></asp:TextBox>
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
                <td class="style2">ช่องทาง</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbChannelId" runat="server" Width="253px" CssClass="Dropdownlist" Enabled="false" ></asp:DropDownList>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">สาขา</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbBranch" runat="server" Width="253px" CssClass="Dropdownlist" ></asp:DropDownList>
                    <asp:Label ID="vcmbBranch" runat="server" CssClass="style4" ></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">บริษัท</td>
                <td class="style3">
                    <asp:TextBox ID="txtCompany" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">รหัสคู่ค้า/เต๊นท์</td>
                <td class="style3">
                    <asp:TextBox ID="txtDealerCode" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">ชื่อคู่ค้า/เต๊นท์</td>
                <td class="style3">
                    <asp:TextBox ID="txtDealerName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">วันที่สร้าง Lead</td>
                <td class="style3">
                   <asp:TextBox ID="txtCreateDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">เวลาที่สร้าง Lead</td>
                <td class="style3">
                    <asp:TextBox ID="txtCreateTime" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ผู้สร้าง Lead</td>
                <td class="style3" >
                    <asp:TextBox ID="txtCreateBy" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                 <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3">
                    
                </td>
            </tr>
            <tr>
               <td class="style2" valign="top">รายละเอียด</td>
                <td colspan ="4">
                    <asp:TextBox ID="txtDetail" runat="server" CssClass="Textbox" Width="770px" Height="70px" TextMode ="MultiLine"  MaxLength="4000" ></asp:TextBox>
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 
<br />
<asp:UpdatePanel ID="upHeader3" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0" >
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
             <tr style="vertical-align:top;">
                <td class="style2">ประเภทบุคคล</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbCardType" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="true"
                        onselectedindexchanged="cmbCardType_SelectedIndexChanged" >
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="vtxtCardType" runat="server" CssClass="style4"></asp:Label>
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
                    <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="13" Enabled="false" ontextchanged="txtCitizenId_TextChanged" AutoPostBack="True" ></asp:TextBox>
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
                        Width="250px" AutoPostBack="True" 
                        ontextchanged="txtBaseSalary_TextChanged" MaxLength="15" ></asp:TextBox>--%>
                    <asp:TextBox ID="txtBaseSalary" runat="server" CssClass="TextboxR" Width="250px" MaxLength="15" ></asp:TextBox>
                    <br />
                    <asp:Label ID="vtxtBaseSalary" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">หมายเลขโทรศัพท์ 1(มือถือ)<span class="style4">*</span></td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo_1" runat="server" CssClass="TextboxView"  ReadOnly="true" Width="250px"  ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">สาขาที่สะดวกให้ติดต่อกลับ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbContactBranch" runat="server" Width="253px" CssClass="Dropdownlist" ></asp:DropDownList>
                    <asp:Label ID="vcmbContactBranch" runat="server" CssClass="style4" ></asp:Label>
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
                    <asp:Label ID="vtxtAvailableTime" runat="server" CssClass="style4"> </asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">หมายเลขโทรศัพท์ 3</td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo3" runat="server" CssClass="Textbox" Width="170px" 
                        MaxLength="10" AutoPostBack="True" ontextchanged="txtTelNo3_TextChanged" ></asp:TextBox>
                    <asp:Label ID="label2" runat="server" Width="10px" CssClass="LabelC" Text="-"></asp:Label>
                    <asp:TextBox ID="txtExt3" runat="server" CssClass="Textbox" Width="57px" MaxLength="30" ></asp:TextBox>
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
        <table cellpadding="2" cellspacing="0" border="0" >
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
        <table cellpadding="2" cellspacing="0" border="0" >
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
        <table cellpadding="2" cellspacing="0" border="0" >
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
        <table cellpadding="2" cellspacing="0" border="0" >
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
        <table cellpadding="2" cellspacing="0" border="0" >
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
        <table class="style5">
            <tr>
                <td align="right">
                    <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" 
                        Width="90px" onclick="btnSave_Click"  OnClientClick="if (confirm('ต้องการบันทึกใช่หรือไม่')){ DisplayProcessing(); return true; } else { return false; }" /> &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                        Width="90px" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่')" 
                        onclick="btnCancel_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate> 
</asp:UpdatePanel> 

