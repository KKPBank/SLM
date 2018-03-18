<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab004.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab004" %>
<%@ Register src="../TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<%@ Register Src="~/Shared/Lead_Share_Common.ascx" TagPrefix="uc1" TagName="Lead_Share_Common" %>
<%@ Register Src="~/Shared/Prelead_Detail.ascx" TagPrefix="uc1" TagName="Prelead_Detail" %>



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
        width: 950px;
    }
</style>

<div style="height:10px;">
    <asp:UpdateProgress ID="upgTab004" runat="server" AssociatedUpdatePanelID="upTabMain" >
        <ProgressTemplate>
            <div style="vertical-align:middle; text-align:center;">
                <asp:Image ID="imgLoadingTab004" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</div>
<div style="float:right;">
    <asp:LinkButton ID="lbReloadInfo" runat="server" Text="Refresh" Font-Names="Tahoma" Font-Underline="true" OnClick="lbReloadInfo_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
</div>
<div style="float:right;">
    <asp:LinkButton ID="lbReloadPreleadInfo" runat="server" Text="Refresh" Font-Names="Tahoma" Font-Underline="true" OnClick="lbReloadPreleadInfo_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
</div>
<asp:Panel runat="server" ID="pnlLead" Visible ="false">
<uc1:Lead_Share_Common runat="server" ID="ctlCommon" />
<asp:PlaceHolder runat="server" ID="plcControl" EnableViewState="true" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlPrelead" Visible="false">
    <uc1:Prelead_Detail runat="server" ID="ctlPrelead" />
</asp:Panel>
<div style="font-family:Tahoma; font-size:13px;" runat="server" visible="false" id="oldcontrol">
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="imgHeader2" runat="server" ImageUrl="~/Images/hContactDetail.gif" />
            </td>
        </tr>
        <tr>
            <td class="style2">เรื่อง</td>
            <td class="style3">
                 <asp:TextBox ID="txtTopic" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ช่องทาง</td>
            <td class="style3">
                <asp:TextBox ID="txtChannelName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="style2">วันที่สร้าง Lead</td>
            <td class="style3">
                <asp:TextBox ID="txtCreateDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">สาขา</td>
            <td class="style3">
                <asp:TextBox ID="txtBranchprod" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">เวลาที่สร้าง Lead</td>
            <td class="style3">
                 <asp:TextBox ID="txtCreateTime" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">บริษัท</td>
            <td class="style3">
                <asp:TextBox ID="txtCompany" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="style2">ผู้สร้าง Lead</td>
            <td class="style3">
                 <asp:TextBox ID="txtCreateBy" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2"></td>
            <td class="style3">
            </td>
        </tr>
        <tr>
            <td class="style2">รหัสคู่ค้า/เต๊นท์</td>
            <td class="style3">
                 <asp:TextBox ID="txtDealerCode" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ชื่อคู่ค้า/เต๊นท์</td>
            <td class="style3">
                <asp:TextBox ID="txtDealerName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr class="Hidden">
            <td valign="top" class="style2">รายละเอียด</td>
            <td colspan="4">
                 <asp:TextBox ID="txtDetail" runat="server" CssClass="TextboxView" Width="770px" Height="70px" TextMode ="MultiLine"  ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
        <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/hLeadDetail.gif" />
            </td>
        </tr>
        <tr style="vertical-align:top;">
            <td class="style2">เป็นลูกค้าหรือเคยเป็นลูกค้า<br />ของธนาคารหรือไม่</td>
            <td class="style3">
                 <asp:TextBox ID="txtIsCustomer" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">รหัสลูกค้า</td>
            <td class="style3">
                <asp:TextBox ID="txtCusCode" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ประเภทบุคคล</td>
            <td class="style3">
                 <asp:TextBox ID="txtCardType" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">เลขที่สัญญาที่เคยมีกับธนาคาร</td>
            <td class="style3">
                <asp:TextBox ID="txtContractNoRefer" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">เลขที่บัตร</td>
            <td class="style3">
                 <asp:TextBox ID="txtCitizenId" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">วันเกิด</td>
            <td class="style3">
                <asp:TextBox ID="txtBitrhDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">อาชีพ</td>
            <td class="style3">
                 <asp:TextBox ID="txtOccupation" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ฐานเงินเดือน</td>
            <td class="style3">
                <asp:TextBox ID="txtBaseSalary" runat="server" CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">สาขาที่สะดวกให้ติดต่อกลับ</td>
            <td class="style3">
                 <asp:TextBox ID="txtBranchName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">เวลาที่สะดวกให้ติดต่อกลับ</td>
            <td class="style3">
                <asp:TextBox ID="txtAvailableTime" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">E-Mail</td>
            <td class="style3">
                 <asp:TextBox ID="txtEmail" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2"></td>
            <td class="style3">
                
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hAddressDetail.gif" />
            </td>
        </tr>
        <tr>
            <td class="style2">เลขที่</td>
            <td class="style3">
                <asp:TextBox ID="txtAddressNo" runat="server"  CssClass="TextboxView" Width="250px"  ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ชื่ออาคาร/หมู่บ้าน</td>
            <td class="style3">
                 <asp:TextBox ID="txtBuildingName" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ชั้น</td>
            <td class="style3">
                <asp:TextBox ID="txtFloor" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ซอย</td>
            <td class="style3">
                <asp:TextBox ID="txtSoi" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ถนน</td>
            <td class="style3">
                <asp:TextBox ID="txtStreet" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">แขวง/ตำบล</td>
            <td class="style3">
                <asp:TextBox ID="txtTambon" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">เขต/อำเภอ</td>
            <td class="style3">
                <asp:TextBox ID="txtAmphur" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">จังหวัด</td>
            <td class="style3">
                <asp:TextBox ID="txtProvince" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">รหัสไปรษณีย์</td>
            <td class="style3" colspan="4">
                <asp:TextBox ID="txtPostalCode" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/hCalculateDetail.gif" />
            </td>
        </tr>
        <tr style="vertical-align:top;">
            <td class="style2">ประเภทความสนใจ<br />(รถใหม่/รถเก่า)</td>
            <td class="style3">
                <asp:TextBox ID="txtInterestedProd" runat="server"  CssClass="TextboxView" Width="250px"  ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ทะเบียนรถ</td>
            <td class="style3">
                 <asp:TextBox ID="txtLicenseNo" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">จังหวัดที่จดทะเบียน</td>
            <td class="style3">
                <asp:TextBox ID="txtProvinceRegis" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ปีรถ</td>
            <td class="style3">
                 <asp:TextBox ID="txtYearOfCar" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ปีที่จดทะเบียนรถยนต์</td>
            <td class="style3">
                 <asp:TextBox ID="txtYearOfCarRegis" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ยี่ห้อรถ</td>
            <td class="style3">
                <asp:TextBox ID="txtBrand" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">รุ่นรถ</td>
            <td class="style3">
                 <asp:TextBox ID="txtModel" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">รุ่นย่อยรถ</td>
            <td class="style3">
                <asp:TextBox ID="txtSubmodel" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ราคารถยนต์</td>
            <td class="style3">
                 <asp:TextBox ID="txtCarPrice" runat="server"  CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">เงินดาวน์</td>
            <td class="style3">
                <asp:TextBox ID="txtDownPayment" runat="server"  CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">เปอร์เซ็นต์เงินดาวน์</td>
            <td class="style3">
                 <asp:TextBox ID="txtDownPercent" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ยอดจัด Finance</td>
            <td class="style3">
                <asp:TextBox ID="txtFinanceAmt" runat="server"  CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ระยะเวลาที่ผ่อนชำระ</td>
            <td class="style3">
                 <asp:TextBox ID="txtPaymentTerm" runat="server"  CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">ประเภทการผ่อนชำระ</td>
            <td class="style3">
                <asp:TextBox ID="txtPaymentType" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">Balloon Amount</td>
            <td class="style3">
                 <asp:TextBox ID="txtBalloonAmt" runat="server" CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">Balloon Percent</td>
            <td class="style3">
                <asp:TextBox ID="txtBalloonPercent" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/hInsurance.gif" />
            </td>
        </tr>
         <tr>
            <td class="style2">ประเภทกรมธรรม์</td>
            <td class="style3">
                 <asp:TextBox ID="txtPlanType" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">วันที่เริ่มต้นคุ้มครอง</td>
            <td class="style3">
                 <asp:TextBox ID="txtCoverageDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
    </table>
     <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/hProductOther.gif" />
            </td>
        </tr>
         <tr>
            <td class="style2">ประเภทเงินฝาก</td>
            <td class="style3">
                 <asp:TextBox ID="txtAccType" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">โปรโมชั่นเงินฝากที่สนใจ</td>
            <td class="style3">
                <asp:TextBox ID="txtAccPromotion" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ระยะเวลาฝาก Term</td>
            <td class="style3">
                 <asp:TextBox ID="txtAccTerm" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">อัตราดอกเบี้ยที่สนใจ</td>
            <td class="style3">
                <asp:TextBox ID="txtInterest" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">เงินฝาก/เงินลงทุน</td>
            <td class="style3">
                 <asp:TextBox ID="txtInvest" runat="server" CssClass="TextboxViewR" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">สินเชื่อ Over Draft</td>
            <td class="style3">
                <asp:TextBox ID="txtLoanOd" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">ระยะเวลา Over Draft</td>
            <td class="style3">
                 <asp:TextBox ID="txtLoanOdTerm" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2">สนใจ E-Banking</td>
            <td class="style3">
                <asp:TextBox ID="txtEbank" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">สนใจ ATM</td>
            <td class="style3">
                 <asp:TextBox ID="txtAtm" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"></asp:TextBox>
            </td>
            <td class="style1"></td>
            <td class="style2"></td>
            <td class="style3">
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr>
            <td colspan="5">
                <asp:Image ID="Image5" runat="server" ImageUrl="~/Images/hAttach.gif" />
            </td>
        </tr>
         <tr>
            <td style="width:178px; font-weight:bold;">Path Link</td>
            <td class="style3" colspan="4">
                 <%--<asp:HyperLink ID="hlPathLink" runat="server" Width="750px" Target="_blank" ></asp:HyperLink>--%>
                 <asp:LinkButton ID="lbPathLink" runat="server" Width="750px" ></asp:LinkButton>
            </td>
        </tr>
    </table>
    <br />
</div>
&nbsp;
<%--<hr />--%>
    <table class="style5">
        <tr>
            <td align="right">
                <asp:Button ID="btnSave" runat="server" Text="ย้อนกลับ" CssClass="Button" Visible="false"  
                    Width="90px" onclick="btnSave_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>
