<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_052.aspx.cs" Inherits="SLM.Application.SLM_SCR_052" %>

<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #menuwrapper ul li a {
            width: inherit;
        }

        .ColInfo {
            font-weight: bold;
            width: 200px;
        }

        .ColInput {
            width: 230px;
        }

        .ColInfoPopup {
            font-weight: bold;
            width: 120px;
            padding-left: 30px;
        }

        .t_rowhead2 {
            font-weight: bold;
            color: #4b134a;
            background-color: #fbcbfa;
            height: 24px;
            border: 1px double #f995f8;
        }

        .t_row2 {
            background-color: #ffffff;
            height: 25px;
            border-top-style: none;
            border-top-width: 0px;
            border-left-style: none;
            border-left-width: 0px;
            border-right-style: none;
            border-right-width: 0px;
            border-bottom-style: Dashed;
            border-bottom-width: 1px;
            border-bottom-color: #fbcbfa;
            border-collapse: collapse;
            font-family: Tahoma;
            font-size: 13px;
        }

        .input {
            width: 180px;
        }

        .t_row td {
            text-align:center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportExcel" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height: 2px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo">Team Telesales 
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList runat="server" ID="cmbTelesalesTeam" AutoPostBack="true" DataTextField="TextField" DataValueField="ValueField" OnSelectedIndexChanged="cmbTelesalesTeam_SelectedIndexChanged" CssClass="input"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">ชื่อ Telesales
                    </td>
                    <td class="ColInput">
                        <asp:UpdatePanel runat="server" ID="upTelesales">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmbTelesalesTeam" EventName="SelectedIndexChanged" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:DropDownList runat="server" ID="cmbTelesales" DataTextField="TextField" DataValueField="ValueField" CssClass="input">
                                    <asp:ListItem Value="">ทั้งหมด</asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">บริษัทประกัน 
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList runat="server" ID="cmbInsuranceCom" DataTextField="TextField" DataValueField="ValueField" CssClass="input"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">ประเภทประกัน
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList runat="server" ID="cmbCoverageType" DataTextField="TextField" DataValueField="ValueField" CssClass="input"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">ชื่อลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:TextBox runat="server" ID="txtCustomerName" CssClass="input"></asp:TextBox>
                    </td>
                    <td class="ColInfo">นามสกุลลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:TextBox runat="server" ID="txtCustomerLastName" CssClass="input"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">Contract No
                    </td>
                    <td class="ColInput">
                        <asp:TextBox runat="server" ID="txtContractNo" CssClass="input"></asp:TextBox>
                    </td>
                    <td class="ColInfo"></td>
                    <td class="ColInput"></td>
                </tr>
                <tr>
                    <td class="ColInfo">วันที่กดรับเลขรับแจ้งเริ่มต้น 
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdStartDate" runat="server" Width="120px" />
                    </td>
                    <td class="ColInfo">วันที่กดรับเลขรับแจ้งสิ้นสุด
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdEndDate" runat="server" Width="120px" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">วันที่กดเบิก Incentive เริ่มต้น 
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdIncentiveStartDate" runat="server" Width="120px" />
                    </td>
                    <td class="ColInfo">วันที่กดเบิก Incentive สิ้นสุด
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdIncentiveEndDate" runat="server" Width="120px" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">วันที่เริ่มต้นของวันที่คุ้มครอง กธ 
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdProtectStartDate" runat="server" Width="120px" />
                    </td>
                    <td class="ColInfo">วันสิ้นสุดของวันคุ้มครอง กธ
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdProtectEndDate" runat="server" Width="120px" />
                    </td>
                </tr>

            </table>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height: 5px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ล้างข้อมูล" OnClick="btnClear_Click" Visible="false"/>
                        <asp:Button ID="btnExportExcel" runat="server" CssClass="Button" Width="100px"                             Text="Export Excel" OnClick="btnExportExcel_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;            
            <br />
            <br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1200px" Visible="false" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1800px">
                <Columns>
                    <asp:BoundField HeaderText="Team Code" DataField="slm_TeamTelesales_Code" />
                    <asp:BoundField HeaderText="ชื่อหัวหน้าทีม" DataField="slm_HeadStaffNameTH" />
                    <asp:BoundField HeaderText="Telesales" DataField="slm_StaffNameTH" />
                    <asp:BoundField HeaderText="สถานะย่อย" DataField="slm_SubStatusName" />
                    <asp:BoundField HeaderText="Contract" DataField="slm_ContractNo" />
                    <asp:BoundField HeaderText="ลูกค้า" DataField="slm_CustomerName" />
                    <asp:BoundField HeaderText="ค่าเบี้ยค้างชำระ" DataField="slm_RemainAmt" HtmlEncode="false" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="เงินที่รับชำระ" DataField="slm_ReceivedAmt" HtmlEncode="false" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="เบี้ยประกันหลังหักส่วนลด" DataField="slm_PolicyGrossPremium" HtmlEncode="false" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="เบี้ยประกันตามกรมธรรม์" DataField="slm_PolicyGrossPremiumTotal" HtmlEncode="false" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="บริษัทประกัน" DataField="slm_InsNameTh" />
                    <asp:TemplateField HeaderText="วันที่เริ่มคุ้มครอง">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCoverDate" Text='<%# Convert.ToDateTime(Eval("slm_PolicyStartCoverDate")) == DateTime.MinValue ? "" : String.Format("{0:d/M/yyyy}", Eval("slm_PolicyStartCoverDate")) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่รับเลขรับแจ้ง">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblReceiveDate" Text='<%# Convert.ToDateTime(Eval("slm_ReceiveDate")) == DateTime.MinValue ? "" : String.Format("{0:d/M/yyyy}", Eval("slm_ReceiveDate")) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="ส่วนลด (บาท)" DataField="slm_PolicyDiscountAmt" HtmlEncode="false" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="ประเภทประกัน" DataField="slm_ConverageTypeName" /> 
                    <asp:BoundField HeaderText="หมายเหตุประกันภัย" DataField="slm_RemarkPolicy"/>
                    <asp:TemplateField HeaderText="วันที่รับ Incentive">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCoverEndDate" Text='<%# Convert.ToDateTime(Eval("slm_IncentiveDate")) == DateTime.MinValue ? "" : String.Format("{0:d/M/yyyy}", Eval("slm_IncentiveDate")) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="สรุป" DataField="slm_Summary" />
                    <asp:BoundField HeaderText="หมายเหตุอืนๆ" DataField="slm_OtherRemark" />
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
