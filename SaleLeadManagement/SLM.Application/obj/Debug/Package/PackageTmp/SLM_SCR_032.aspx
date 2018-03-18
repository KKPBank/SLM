<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_032.aspx.cs" Inherits="SLM.Application.SLM_SCR_032" %>

<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo {
            font-weight: bold;
            width: 190px;
        }

        .ColInput {
            width: 250px;
        }

        .ColCheckBox {
            width: 160px;
        }

        .modalPopupImportExcel {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 550px;
            /*             height: 150px; */
        }

        .modalPopupChangeOwner {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 550px;
            height: 180px;
        }
        .pnPopupConfirmReject {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 30px;
            width: 300px;
            height: 70px;
            text-align: center;
        }
        .style4 {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery-2.2.0.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnClear" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="4" style="height: 2px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo">ชื่อลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    </td>
                    <td class="ColInfo">นามสกุล
                    </td>
                    <td>
                        <asp:TextBox ID="txtLastname" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">ประเภทบุคคล
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCardType" runat="server" Width="203px" CssClass="Dropdownlist" DataTextField="TextField" DataValueField="ValueField">
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">เลขที่บัตร
                    </td>
                    <td>
                        <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">แคมเปญ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCampaign" runat="server" Width="203px" DataTextField="TextField" DataValueField="ValueField" CssClass="Dropdownlist">
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">Transfer Date
                    </td>
                    <td>
                        <uc1:TextDateMask ID="tdmTransferDate" runat="server" Width="182px" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">Telesales Team
                    </td>
                    <td class="ColInput">

                        <asp:UpdatePanel runat="server" ID="upTelesalesTeam" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmbTelesalesTeam" EventName="SelectedIndexChanged" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:DropDownList ID="cmbTelesalesTeam" runat="server" Width="203px"
                                    CssClass="Dropdownlist" DataTextField="TextField" DataValueField="ValueField" AutoPostBack="true" OnSelectedIndexChanged="cmbTelesalesTeam_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </td>
                    <td class="ColInfo">Owner
                    </td>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upOwnerSearch" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmbTelesalesTeam" EventName="SelectedIndexChanged" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:DropDownList ID="cmbOwnerSearch" runat="server" Width="203px"
                                    CssClass="Dropdownlist" DataTextField="TextField" DataValueField="ValueField">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">จำนวนรายการต่อหน้า
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbRecordPerPage" runat="server" Width="203px" CssClass="Dropdownlist">
                            <asp:ListItem Value="50">50</asp:ListItem>
                            <asp:ListItem Value="100">100</asp:ListItem>
                            <asp:ListItem Value="200">200</asp:ListItem>
                            <asp:ListItem Value="500">500</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo"></td>
                    <td></td>
                </tr>
                <tr>
                    <td style="height: 10px; vertical-align: bottom;"></td>
                    <td colspan="3"></td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height: 3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ล้างข้อมูล" OnClick="btnClear_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />

    <asp:Panel ID="pnlResult" runat="server" Visible="true">
        <asp:UpdatePanel ID="upTabMain" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
                <asp:Button ID="Button1" runat="server" Text="Import Excel" Width="120px" CssClass="Button" Height="23px" OnClick="btnImportExcel_Click" />&nbsp;
                <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" Width="120px" CssClass="Button" Height="23px" OnClick="btnExportExcel_Click" OnClientClick="DisplayProcessing()" />
                <br />
                <br />
                <asp:UpdatePanel ID="upNoLot" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <uc2:GridviewPageController ID="pcNoLot" runat="server" OnPageChange="pcNoLot_PageChange" Width="1160px" />
                        <asp:GridView ID="gvNoLot" runat="server" AutoGenerateColumns="False"
                            GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                            EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1160px">
                            <Columns>
                                <asp:BoundField DataField="No" HeaderText="No">
                                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_TeamTelesales_Code" HeaderText="ชื่อทีม">
                                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_StaffNameTH" HeaderText="ชื่อ Telesales">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_MonthNameTh" HtmlEncode="false" HeaderText="เดือนที่สิ้นสุด<br/>การคุ้มครอง">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_Contract_Number" HeaderText="Contract">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CustomerName" HeaderText="ชื่อ-สกุล ลูกค้า">
                                    <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                    <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_InsNameTh" HeaderText="บริษัทประกันภัย">
                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_ConverageTypeName" HeaderText="ประเภทประกัน">
                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_Voluntary_Gross_Premium" HeaderText="ค่าเบี้ยประกัน">
                                    <HeaderStyle Width="110px" HorizontalAlign="Center" />
                                    <ItemStyle Width="110px" HorizontalAlign="Right" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_CreatedDate" HeaderText="Transfer Date" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_Grade" HeaderText="Grade ลูกค้า">
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_Assign_Type_Name" HeaderText="ประเภทการจ่ายงาน">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_AssignDescription" HeaderText="หมายเหตุการจ่ายงาน">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:BoundField DataField="slm_FlagNotifyPremium" HeaderText="ค่าเบี้ยปีต่อ">
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:BoundField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                        </asp:GridView>
                        <br />
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td align="right" style="width: 1060px;">
                                    <asp:Button ID="Button4" runat="server" Text="Confirm" CssClass="Button" Width="100px" Visible="false" />
                                    <asp:Button ID="Button13" runat="server" Text="Reject" CssClass="Button" Width="100px" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <act:TabContainer ID="tabMain" runat="server" ActiveTabIndex="0" Width="1190px">
                    <act:TabPanel ID="tabObtReport" runat="server">
                        <HeaderTemplate>
                            <asp:Label ID="lblHeaderObtReport" runat="server" Text="&nbsp;Success&nbsp;" CssClass="tabHeaderText"></asp:Label>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upSuccess" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc2:GridviewPageController ID="pcSuccess" runat="server" OnPageChange="pcSuccess_PageChange" Width="1160px" />
                                    <asp:GridView ID="gvSuccess" runat="server" AutoGenerateColumns="False"
                                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1160px">
                                        <%--OnRowDataBound="gvResult_RowDataBound">--%>
                                        <Columns>
                                            <asp:TemplateField HeaderText="Action" Visible="false">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("slm_Prelead_Id") %>' OnClick="imbEdit_Click" ToolTip="แก้ไขเจ้าของงาน" />
                                                </ItemTemplate>
                                                <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="No" HeaderText="No">
                                                <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_TeamTelesales_Code" HeaderText="ชื่อทีม">
                                                <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_StaffNameTH" HeaderText="ชื่อ Telesales">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_MonthNameTh" HtmlEncode="false" HeaderText="เดือนที่สิ้นสุด<br/>การคุ้มครอง">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Contract_Number" HeaderText="Contract">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CustomerName" HeaderText="ชื่อ-สกุล ลูกค้า">
                                                <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                                <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_InsNameTh" HeaderText="บริษัทประกันภัย">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_ConverageTypeName" HeaderText="ประเภทประกัน">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Voluntary_Gross_Premium" DataFormatString ="{0:#,##0.00}" HtmlEncode="false"  HeaderText="ค่าเบี้ยประกัน">
                                                <HeaderStyle Width="110px" HorizontalAlign="Center" />
                                                <ItemStyle Width="110px" HorizontalAlign="Right" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Transfer Date">
                                                <ItemTemplate>
                                                    <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() : ""%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="slm_Grade" HeaderText="Grade ลูกค้า">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Assign_Type_Name" HeaderText="ประเภทการจ่ายงาน">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_AssignDescription" HeaderText="หมายเหตุการจ่ายงาน">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_FlagNotifyPremium" HeaderText="Flag ค่าเบี้ยปีต่อ">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="t_rowhead" />
                                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                    </asp:GridView>
                                    <br />
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="right" style="width: 1060px;">
                                                <asp:Button ID="Button7" runat="server" Text="Confirm" CssClass="Button" Width="100px" Visible="false" />
                                                <asp:Button ID="Button8" runat="server" Text="Reject" CssClass="Button" Width="100px" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </act:TabPanel>
                    <act:TabPanel ID="tabObtReport2" runat="server">
                        <HeaderTemplate>
                            <asp:Label ID="lblHeaderObtReport2" runat="server" Text="&nbsp;De-Dup&nbsp;" CssClass="tabHeaderText"></asp:Label>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upDedup" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc2:GridviewPageController ID="pcDedup" runat="server" OnPageChange="pcDedup_PageChange" Width="1160px" />
                                    <asp:GridView ID="gvDedup" runat="server" AutoGenerateColumns="False"
                                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" OnRowDataBound="gvDedup_RowDataBound"
                                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1160px">
                                        <Columns>
                                            <asp:TemplateField HeaderText="&nbsp">
                                                <ItemTemplate>
                                                    <asp:CheckBox runat="server" ID="checkDedup" />
                                                    <asp:HiddenField runat="server" ID="hiddenTempId" Value='<%# Eval("slm_TempId") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="No" HeaderText="No">
                                                <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_MonthNameTh" HtmlEncode="false" HeaderText="เดือนที่สิ้นสุด<br/>การคุ้มครอง">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Contract_Number" HeaderText="Contract">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CustomerName" HeaderText="ชื่อ-สกุล ลูกค้า">
                                                <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                                <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_InsNameTh" HeaderText="บริษัทประกันภัย">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_ConverageTypeName" HeaderText="ประเภทประกัน">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Voluntary_Gross_Premium" DataFormatString ="{0:#,##0.00}" HtmlEncode="false" HeaderText="ค่าเบี้ยประกัน">
                                                <HeaderStyle Width="110px" HorizontalAlign="Center" />
                                                <ItemStyle Width="110px" HorizontalAlign="Right" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Transfer Date">
                                                <ItemTemplate>
                                                    <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() : ""%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="slm_Grade" HeaderText="Grade ลูกค้า">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_FlagNotifyPremium" HeaderText="Flag ค่าเบี้ยปีต่อ">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="t_rowhead" />
                                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                    </asp:GridView>
                                    <br />
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="right" style="width: 1060px;">
                                                <asp:Button ID="Button5" runat="server" Text="Confirm" CssClass="Button" Width="100px" Visible="false" />
                                                <asp:Button ID="Button6" runat="server" Text="Reject" CssClass="Button" Width="100px" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </act:TabPanel>
                    <act:TabPanel ID="tabObtReport3" runat="server">
                        <HeaderTemplate>
                            <asp:Label ID="lblHeaderObtReport3" runat="server" Text="&nbsp;Blacklist&nbsp;" CssClass="tabHeaderText"></asp:Label>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upBlacklist" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc2:GridviewPageController ID="pcBlacklist" runat="server" OnPageChange="pcBlacklist_PageChange" Width="1160px" />
                                    <asp:GridView ID="gvBlacklist" runat="server" AutoGenerateColumns="False"
                                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" OnRowDataBound="gvBlacklist_RowDataBound"
                                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1160px">
                                        <Columns>
                                            <asp:TemplateField HeaderText="&nbsp">
                                                <ItemTemplate>
                                                    <asp:CheckBox runat="server" ID="checkBlacklist" />
                                                    <asp:HiddenField runat="server" ID="hiddenTempId" Value='<%# Eval("slm_TempId") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="No" HeaderText="No">
                                                <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_MonthNameTh" HtmlEncode="false" HeaderText="เดือนที่สิ้นสุด<br/>การคุ้มครอง">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Contract_Number" HeaderText="Contract">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CustomerName" HeaderText="ชื่อ-สกุล ลูกค้า">
                                                <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                                <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_InsNameTh" HeaderText="บริษัทประกันภัย">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_ConverageTypeName" HeaderText="ประเภทประกัน">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Voluntary_Gross_Premium"  DataFormatString ="{0:#,##0.00}" HtmlEncode="false"  HeaderText="ค่าเบี้ยประกัน">
                                                <HeaderStyle Width="110px" HorizontalAlign="Center" />
                                                <ItemStyle Width="110px" HorizontalAlign="Right" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Transfer Date">
                                                <ItemTemplate>
                                                    <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() : ""%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="slm_Grade" HeaderText="Grade ลูกค้า">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_FlagNotifyPremium" HeaderText="Flag ค่าเบี้ยปีต่อ">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="t_rowhead" />
                                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                    </asp:GridView>
                                    <br />
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="right" style="width: 1060px;">
                                                <asp:Button ID="Button9" runat="server" Text="Confirm" CssClass="Button" Width="100px" Visible="false" />
                                                <asp:Button ID="Button10" runat="server" Text="Reject" CssClass="Button" Width="100px" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </act:TabPanel>
                    <act:TabPanel ID="tabObtReport4" runat="server">
                        <HeaderTemplate>
                            <asp:Label ID="lblHeaderObtReport4" runat="server" Text="&nbsp;Exceptional&nbsp;" CssClass="tabHeaderText"></asp:Label>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upExceptional" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc2:GridviewPageController ID="pcExceptional" runat="server" OnPageChange="pcExceptional_PageChange" Width="1160px" />
                                    <asp:GridView ID="gvExceptional" runat="server" AutoGenerateColumns="False"
                                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1160px">
                                        <%--OnRowDataBound="gvResult_RowDataBound">--%>
                                        <Columns>
                                            <asp:TemplateField HeaderText="&nbsp" Visible="false">
                                                <ItemTemplate>
                                                    <asp:CheckBox runat="server" ID="checkExceptional" />
                                                </ItemTemplate>
                                                <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="No" HeaderText="No">
                                                <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_MonthNameTh" HtmlEncode="false" HeaderText="เดือนที่สิ้นสุด<br/>การคุ้มครอง">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Contract_Number" HeaderText="Contract">
                                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CustomerName" HeaderText="ชื่อ-สกุล ลูกค้า">
                                                <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                                <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_InsNameTh" HeaderText="บริษัทประกันภัย">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_ConverageTypeName" HeaderText="ประเภทประกัน">
                                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                                <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Voluntary_Gross_Premium"  DataFormatString ="{0:#,##0.00}" HtmlEncode="false" HeaderText="ค่าเบี้ยประกัน">
                                                <HeaderStyle Width="110px" HorizontalAlign="Center" />
                                                <ItemStyle Width="110px" HorizontalAlign="Right" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Transfer Date">
                                                <ItemTemplate>
                                                    <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() : ""%>
                                                </ItemTemplate>
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="slm_Grade" HeaderText="Grade ลูกค้า">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_Assign_Type_Name" HeaderText="ประเภทการจ่ายงาน">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_AssignDescription" HeaderText="หมายเหตุการจ่ายงาน">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="slm_FlagNotifyPremium" HeaderText="Flag ค่าเบี้ยปีต่อ">
                                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="t_rowhead" />
                                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                    </asp:GridView>
                                    <br />
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="right" style="width: 1060px;">
                                                <asp:Button ID="Button11" runat="server" Text="Confirm" CssClass="Button" Width="100px" Visible="false" />
                                                <asp:Button ID="Button12" runat="server" Text="Reject" CssClass="Button" Width="100px" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </act:TabPanel>
                </act:TabContainer>
                <br />
                <table>
                    <tr>
                        <td align="right" style="width: 1100px">
                            <asp:Button ID="btnConfirm" runat="server" CssClass="Button" Width="100px" Text="Confirm" OnClick="btnConfirm_Click" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnReject" runat="server" CssClass="Button" Width="100px" Text="Reject" OnClick="btnReject_Click"  />
                        </td>
                    </tr>
                </table>                
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdatePanel ID="upPopupImportExcel" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPopupDoImportExcel" />
        </Triggers>
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupImportExcel" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupImportExcel" Style="display: none" CssClass="modalPopupImportExcel">
                <br />
                <br />
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="width: 100px; text-align : right; padding-right: 5px; font-weight:bold;">แนบไฟล์</td>
                        <td>
                            <asp:FileUpload ID="fuData" runat="server" Width="380px" size="45" />
                            <br />
                            <asp:Label ID="lblUploadError" runat="server" CssClass="style4"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;"></td>
                        <td style="height: 1px;"></td>
                    </tr>
                    <tr>
                        <td style="width: 100px;"></td>
                        <td>
                            <asp:Button ID="btnPopupDoImportExcel" runat="server" OnClick="btnPopupDoImportExcel_Click" Text="Import" Width="100px" OnClientClick="DisplayProcessing()" />&nbsp;
                                <asp:Button ID="btnPopupCancel" runat="server" Text="Cancel" Width="100px" />
                        </td>
                    </tr>
                </table><br />
                <div style="max-height: 300px; overflow-y: auto; padding: 20px">
                    <asp:GridView ID="gvUploadError" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="500px">
                        <Columns>
                            <asp:TemplateField HeaderText="Row No">
                                <ItemTemplate>
                                    <asp:Label ID="lblRowNo" runat="server" Text='<%# Eval("RowNo") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="20px" HorizontalAlign="Center" />
                                <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemTemplate>
                                    <asp:Label ID="lblError" runat="server" Text='<%# Eval("ErrorMessage") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#dddddd" />
                    </asp:GridView>
                </div>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupImportExcel" runat="server" TargetControlID="btnPopupImportExcel" PopupControlID="pnPopupImportExcel" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopupChangeOwner" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupChangeOwner" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupChangeOwner" Style="display: none" CssClass="modalPopupChangeOwner">
                <br />
                <br />
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="font-weight: bold; width: 100px; padding-left: 20px;">Owner Branch
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbOwnerBranch" runat="server" CssClass="Dropdownlist" Width="250px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; width: 100px; padding-left: 20px;">Owner Lead
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbOwner" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="height: 5px;"></td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td>
                            <asp:Button ID="btnSavePopupChangeOwner" runat="server" Text="บันทึก" Width="100px" />&nbsp;
                                <asp:Button ID="Button3" runat="server" Text="ยกเลิก" Width="100px" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupChangeOwner" runat="server" TargetControlID="btnPopupChangeOwner" PopupControlID="pnPopupChangeOwner" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopupConfirmReject" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupConfirmReject" Width="0px" CssClass="Hidden" />
            <asp:Panel ID="pnPopupConfirmReject" runat="server" Style="display: none" CssClass="pnPopupConfirmReject">
                <asp:Label runat="server" ID="lblConfirm" Visible="true">ต้องการ Confirm ข้อมูลใช่หรือไม่</asp:Label>
                <asp:Label runat="server" ID="lblReject" Visible="false">ต้องการ Reject ข้อมูลใช่หรือไม่</asp:Label>
                <br /><br />
                <asp:Button runat="server" ID="btnConfirm2" Text="ใช่" OnClick="btnConfirm2_Click" OnClientClick="DisplayProcessing()" CssClass="Button" Width="70px"/>&nbsp;&nbsp;&nbsp;
                <asp:Button runat="server" ID="btnReject2" Text="ไม่" OnClick="btnReject2_Click" CssClass="Button" Width="70px"/>
            </asp:Panel>
            
            <act:ModalPopupExtender ID="mpePopupConfirmReject" runat="server" TargetControlID="btnPopupConfirmReject" PopupControlID="pnPopupConfirmReject" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        $("#<%=upTabMain.ClientID %>").on("change", "input[type='checkbox']", function (e) {
            if ($("input[type='checkbox']:checked").length > 0) {
                $("#ContentPlaceHolder1_btnConfirm")[0].disabled = true;
            }
            else {
                $("#ContentPlaceHolder1_btnConfirm")[0].disabled = false;
            }
        });
    </script>
</asp:Content>
