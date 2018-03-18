<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabInsureSummary.ascx.cs" Inherits="SLM.Application.Shared.Obt.TabInsureSummary" %>

<style type="text/css">
    .tabInsureSummary_table
    {
        border:1px solid lightgray; 
        border-collapse:collapse;   
    }
    .tabInsureSummary_col1
    {
        font-weight:bold;
        background-color:lightblue;
        text-align:right;
        width:200px;
        border-bottom:0.5px dash lightblue;
    }
    .tabInsureSummary_col2
    {
        width:500px;
        border:0.5px dash lightblue;
    }
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
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height:15px;">
            <asp:UpdateProgress ID="upgTabInsureSummary" runat="server" AssociatedUpdatePanelID="upTabMain" >
                <ProgressTemplate>
                    <div style="vertical-align:middle; text-align:center;">
                        <asp:Image ID="imgLoadingInsureSummary" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <div style="float:right; height:18px;">
            <asp:LinkButton ID="lbReloadSummary" runat="server" Text="Refresh" Font-Names="Tahoma" Font-Underline="true" OnClick="lbReloadSummary_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;
        </div>
        <asp:Panel ID="pnTabInsureSummary" runat="server" Visible="false">
            <table cellpadding="2" cellspacing="0" border="0" >
                <tr>
                    <td class="style2">&nbsp;</td>
                    <td class="style3">
                        <asp:CheckBox ID="cbIsExportExpired" runat="server" Text="ออกรายงาน LeadsForTransfer" Font-Bold="true" OnClick="javascript: return false;"></asp:CheckBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">วันที่ออกรายงาน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtIsExportExpiredDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
            </table>
        <br />
            <table cellpadding="2" cellspacing="0" border="0" >
                <tr>
                    <td colspan="5">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hInsurance.gif" />
                    </td>
                </tr>
                <tr>
                    <td class="style2">Grade</td>
                    <td class="style3">
                        <asp:TextBox ID="txtGrade" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">รถเก่า/รถใหม่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtCarCategory" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">วิธีการต่ออายุประกันภัย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryChannelKey" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เลขที่กรมธรรม์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryPolicyNumber" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"  ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="style2">ทุนประกันรวม</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryCovAmt" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">บริษัทประกันภัย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryCompanyName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"  ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ประเภทประกันภัย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryType" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เบี้ยประกันภัย(รวมภาษีอากร)</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryGrossPremium" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"  ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="style2">วันที่สร้างสัญญา</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryFirstCreateDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2"></td>
                    <td class="style3">
                    </td>
                </tr>
                 <tr>
                    <td class="style2">วันเริ่มต้นกรมธรรม์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryPolicyEffDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">วันสิ้นสุดกรมธรรม์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtVoluntaryPolicyExpDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"  ></asp:TextBox>
                    </td>
                </tr>
            </table>
        <br />
            <table cellpadding="2" cellspacing="0" border="0" >
                <tr>
                    <td colspan="5">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/ActData.png" />
                    </td>
                </tr>
                 <tr>
                    <td class="style2">เลขที่ พ.ร.บ.</td>
                    <td class="style3">
                        <asp:TextBox ID="txtactno" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2"></td>
                    <td class="style3">
               
                    </td>
                </tr>
                <tr>
                    <td class="style2">บริษัท พ.ร.บ.</td>
                    <td class="style3">
                        <asp:TextBox ID="txtCompulsoryCompanyName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เบี้ย พ.ร.บ.(รวมภาษีอากร)</td>
                    <td class="style3">
                        <asp:TextBox ID="txtCompulsoryGrossPremium" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">วันที่เริ่มต้น พ.ร.บ.</td>
                    <td class="style3">
                        <asp:TextBox ID="txtCompulsoryPolicyEffDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">วันที่สิ้นสุด พ.ร.บ.</td>
                    <td class="style3">
                        <asp:TextBox ID="txtCompulsoryPolicyExpDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true"  ></asp:TextBox>
                    </td>
                </tr>

            </table>
        <asp:Label ID="lblInsureInfo" runat="server" Text="ข้อมูลประกันภัย" Font-Bold="true" ForeColor="Orange" CssClass="Hidden" ></asp:Label>
        <table cellpadding="2" cellspacing="0" border="1" class="Hidden"  >
            <tr>
                <td class="tabInsureSummary_col1">
                    Grade :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblGrade" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    รถเก่า/รถใหม่ :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblCarCategory" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วิธีการต่ออายุประกันภัย :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryChannelKey" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    เลขที่กรรมธรรม์ :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryPolicyNumber" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    ทุนประกันรวม :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryCovAmt" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>            
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    บริษัทประกันภัย :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryCompanyName" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    ประเภทประกันภัย :
                </td>
                <td class="tabInsureSummary_col2">    
                    <asp:Label ID="lblVoluntaryType" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    เบี้ยประกันภัย(รวมภาษีอากร) :
                </td>
                <td class="tabInsureSummary_col2">    
                    <asp:Label ID="lblVoluntaryGrossPremium" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วันที่สร้างสัญญา :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryFirstCreateDate" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วันเริ่มต้นกรมธรรม์ :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryPolicyEffDate" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วันสิ้นสุดกรมธรรม์ :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblVoluntaryPolicyExpDate" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="ข้อมูล พ.ร.บ." Font-Bold="true" ForeColor="DarkOrange" CssClass ="Hidden" ></asp:Label>
        <table cellpadding="2" cellspacing="0" border="1" class="Hidden" >
            <tr>
                <td class="tabInsureSummary_col1">
                    บริษัท พ.ร.บ. :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblCompulsoryCompanyName" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    เบี้ย พ.ร.บ.(รวมภาษีอากร) :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblCompulsoryGrossPremium" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>           
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วันที่เริ่มต้น พ.ร.บ. :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblCompulsoryPolicyEffDate" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tabInsureSummary_col1">
                    วันที่สิ้นสุด พ.ร.บ. :
                </td>
                <td class="tabInsureSummary_col2">
                    <asp:Label ID="lblCompulsoryPolicyExpDate" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>
            <%--<tr>
                <td class="tabInsureSummary_col1">
                    วันสิ้นสุดอายุภาษีรถยนต์ :
                </td>
                <td class="tabInsureSummary_col2">
                    05/07/2558
                    <asp:Label ID="lblCarExpire" runat="server" Text="" CssClass="tabHeaderText"></asp:Label>
                </td>
            </tr>--%>
        </table>
        </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    