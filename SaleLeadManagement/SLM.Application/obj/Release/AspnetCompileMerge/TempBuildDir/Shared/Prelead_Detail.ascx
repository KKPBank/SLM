<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Prelead_Detail.ascx.cs" Inherits="SLM.Application.Shared.Prelead_Detail" %>
<%@ Register Src="~/Shared/TextDateMask.ascx" TagPrefix="uc1" TagName="TextDateMask" %>

<asp:HiddenField runat="server" ID="hdfPreleadId" />
<table cellpadding="2" cellspacing="0" border="0">
    <tr>
        <td colspan="5" style="height: 15px;"></td>
    </tr>
    <tr>
        <td colspan="5">
            <asp:Image ID="imgHeader1" runat="server" ImageUrl="~/Images/customer_detail.jpg" />
        </td>
    </tr>
    <tr>
        <td class="style2">เลขที่สัญญา<span class="style4">*</span></td>
        <td class="stylez3">
            <asp:TextBox ID="txtContractNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" AutoPostBack="True" OnTextChanged="txtContractNo_TextChanged" ReadOnly="True"></asp:TextBox>
            <asp:Label ID="vtxtName" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">สาขาที่สร้างสัญญา</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbBranch" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="style2">ปีที่ลูกค้าทำสัญญากับ KK</td>
        <td class="stylez3">
            <asp:TextBox ID="txtContractYear" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
            <asp:Label ID="Label1" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">สถานะของสัญญา</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbContractStatus" runat="server" Width="253px" CssClass="Dropdownlist" Enabled="False">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="01">เตือน 1</asp:ListItem>
                <asp:ListItem Value="02">เตือน 2</asp:ListItem>
                <asp:ListItem Value="00">ปกติ</asp:ListItem>
                <asp:ListItem Value="03">เตรียมยึด</asp:ListItem>
                <asp:ListItem Value="04">เตือนสุดท้าย</asp:ListItem>
                <asp:ListItem Value="05">แจ้งยึด</asp:ListItem>
                <asp:ListItem Value="06">ยึดได้</asp:ListItem>
                <asp:ListItem Value="0S">ปิดบัญชี</asp:ListItem>
                <asp:ListItem Value="07">รถเคลมประกัน</asp:ListItem>
                <asp:ListItem Value="08">เตรียมฟ้อง</asp:ListItem>
                <asp:ListItem Value="09">ลูกหนี้ดำเนินคดี</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="style2">ประเภทรถ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbCarCategory" runat="server" Width="253px" CssClass="Dropdownlist">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="U">รถเก่า</asp:ListItem>
                <asp:ListItem Value="N">รถใหม่</asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">รหัสลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCustomerKey" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ReadOnly="True"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ประเภทลูกค้า</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbCardTypeId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblCardTypeOrg" runat="server"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">รหัสบัตรประชาชนของลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">สถานภาพสมรส</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbMaritalStatus" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อลูกค้า</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbTitleId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblTitleNameOrg" runat="server" Text=""></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtLastName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">อาชีพลูกค้า</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbOccupation" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblCareerDescOrg" runat="server" Text=""></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">เกรดลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGrade" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ReadOnly="True"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">วันเกิดลูกค้า</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdBirthDate" runat="server" Width="230px" />
        </td>
        <td class="style1">&nbsp;</td>
        <td class="style2">&nbsp;</td>
        <td class="stylez3">
            &nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" style="height: 15px;"></td>
    </tr>
    <tr>
        <td colspan="5">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/car_detail.jpg" />
        </td>
    </tr>
    <tr>
        <td class="style2">ประเภทการใช้รถ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbCarByGovId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblCarByGovNameOrg" runat="server" Text=""></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">ยี่ห้อรถ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbBrandCode" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbBrandCode_SelectedIndexChanged"></asp:DropDownList>
            <asp:Label ID="lblBrandNameOrg" runat="server" Text=""></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">รุ่นรถ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbModel" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbModel_SelectedIndexChanged"></asp:DropDownList>
            <asp:Label ID="lblModelNameOrg" runat="server" Text=""></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">ปีรถ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbYearGroup" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblYearGroupOrg" runat="server" Text="" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">เลขเครื่องยนต์</td>
        <td class="stylez3">
            <asp:TextBox ID="txtEngineNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">เลขตัวถัง</td>
        <td class="stylez3">
            <asp:TextBox ID="txtChassisNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ทะเบียนรถ</td>
        <td class="stylez3">
            <asp:TextBox ID="txtLicenseNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">จังหวัดที่จดทะเบียน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbProvinceRegis" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblProvinceRegisOrg" runat="server" Text=""></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ซีซี รถ</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCC" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">วันครบกำหนดภาษี</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdExpireDate" runat="server" Width="230px" />
        </td>
    </tr>
    <tr>
        <td colspan="5" style="height: 15px;"></td>
    </tr>
    <tr>
        <td colspan="5">
            <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/voluntary_detail.jpg" />
        </td>
    </tr>
    <tr>
        <td class="style2">รหัสพนักงาน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolMktId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
            <asp:Label ID="lblVolMktIdOrg" runat="server"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อพนักงาน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbVolMktTitle" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblVolMktTitleOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อพนักงาน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolMktFName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลพนักงาน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolMktLName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อบริษัทประกัน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbVolCompanyCode" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">เลขที่กรมธรรม์</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolPolicyNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ประเภทประกันภัย</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbVolTypeKey" runat="server" Width="253px" CssClass="Dropdownlist">
                <asp:ListItem Value=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">ทุนประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolCarAmt" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">วันที่เริ่มต้นความคุ้มครอง</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdVolPolicyEFDate" runat="server" Width="230px" />
        </td>
        <td class="style1"></td>
        <td class="style2">วันที่สิ้นสุดความคุ้มครอง</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdVolPolicyEPDate" runat="server" Width="230px" />
        </td>
    </tr>
    <tr>
        <td class="style2">ปีของประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolPolicyYear" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">ยอดที่ต้องชำระของลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolGrossPreimum" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ปีที่หมดประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolPolicyExpYear" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">เดือนที่หมดประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtVolPolicyExpMonth" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ทำประกันภัยผ่าน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbVolChannelKey" runat="server" Width="253px" CssClass="Dropdownlist">
                <asp:ListItem Value=""></asp:ListItem>
                <asp:ListItem Value="P">ประกันภัยผ่านบริษัท</asp:ListItem>
                <asp:ListItem Value="F">ฟรีประกันภัย</asp:ListItem>
                <asp:ListItem Value="C">ผู้เช่าซื้อประกันภัยเอง</asp:ListItem>
                <asp:ListItem Value="D">ผู้จำหน่ายรถยนต์แจ้งประกันภัย</asp:ListItem>
                <asp:ListItem Value="O">ประกันภัยเดิม</asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อผู้เอาประกัน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbBenTitleId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblBenTitleNameOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้เอาประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtBenFName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลผู้เอาประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtBenLName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์โทรติดต่อผู้เอาประกัน</td>
        <td class="stylez3">
            <asp:TextBox ID="txtBenTelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td class="style2">คำนำหน้าชื่อผู้ขับขี่คนที่1</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbDrv1TitleId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblDrv1TitleNameOrg" runat="server"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้ขับขี่คนที่1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv1FName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลผู้ขับขี่คนที่1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv1LName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์โทรติดต่อผู้ขับขี่คนที่1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv1TelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">วันเกิดผู้ขับขี่คนที่1</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdDrv1Birthday" runat="server" Width="230px" />
        </td>
    </tr>
    <tr>
        <td class="style2">คำนำหน้าชื่อผู้ขับขี่คนที่2</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbDrv2TitleId" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblDrv2TitleNameOrg" runat="server"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้ขับขี่คนที่2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv2FName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>

        <td class="style1"></td>
        <td class="style2">นามสกุลผู้ขับขี่คนที่2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv2LName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์โทรติดต่อผู้ขับขี่คนที่2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtDrv2TelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">วันเกิดผู้ขับขี่คนที่2</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdDrv2Birthday" runat="server" Width="230px" />
        </td>
    </tr>
    <tr>
        <td colspan="5" style="height: 15px;"></td>
    </tr>
    <tr>
        <td colspan="5">
            <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/compulsory_detail.jpg" />
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อบริษัทประกัน</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbCompulCompCode" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">เลขที่ พรบ.</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCompulPolicyNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">ปี พรบ.</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCompulPolicyYear" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">ทุนประกัน พรบ.</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCompulCovAmt" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">วันที่เริ่ม พรบ.</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdCompulPolicyEFDate" runat="server" Width="230px" />
        </td>
        <td class="style1"></td>
        <td class="style2">วันหมดอายุ พรบ.</td>
        <td class="stylez3">
            <uc1:TextDateMask ID="tdCompulPolicyEPDate" runat="server" Width="230px" />
        </td>
    </tr>
    <tr>
        <td class="style2">ยอดที่ต้องชำระของลูกค้า</td>
        <td class="stylez3">
            <asp:TextBox ID="txtCompulGrossPremium" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td colspan="5" style="height: 15px;"></td>
    </tr>
    <tr>
        <td colspan="5">
            <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/guarantor_detail.jpg" />
        </td>
    </tr>
    <tr>
        <td class="style2">รหัสผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar1Code" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar1Title" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar1TitleNameOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar1FName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar1LName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เลขที่บัตรผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar1CitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">ความสัมพันธ์กับผู้เช่าซื้อ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar1Relation" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar1RelationOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์มือถือผู้ค้ำ1</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar1TelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td class="style2">รหัสผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar2Code" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar2Title" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar2TitleNameOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar2FName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar2LName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เลขที่บัตรผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar2CitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">ความสัมพันธ์กับผู้เช่าซื้อ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar2Relation" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar2RelationOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์มือถือผู้ค้ำ2</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar2TelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
    <tr>
        <td class="style2">รหัสผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar3Code" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">คำนำหน้าชื่อผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar3Title" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar3TitleNameOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">ชื่อผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar3FName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">นามสกุลผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar3LName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="style2">เลขที่บัตรผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar3CitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2">ความสัมพันธ์กับผู้เช่าซื้อ</td>
        <td class="stylez3">
            <asp:DropDownList ID="cmbGuar3Relation" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
            <asp:Label ID="lblGuar3RelationOrg" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">เบอร์มือถือผู้ค้ำ3</td>
        <td class="stylez3">
            <asp:TextBox ID="txtGuar3TelNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="stylez3"></td>
    </tr>
</table>
