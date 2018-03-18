<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_053.aspx.cs" Inherits="SLM.Application.SLM_SCR_053" %>

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
        .require { color: red; }
        
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
                    <td class="ColInfo">ปีที่แจกงาน  <span class="require">*</span>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList runat="server" ID="cmbYear" DataTextField="TextField" DataValueField="ValueField" Width="180px"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">เดือนที่หมดอายุ กธ <span class="require">*</span></td>
                    <td class="ColInput">
                        <asp:DropDownList runat="server" ID="cmbMonth" DataTextField="TextField" DataValueField="ValueField" Width="180px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">Team Telesales</td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbTelesalesTeam" runat="server" AutoPostBack="true" DataTextField="TextField" DataValueField="ValueField" OnSelectedIndexChanged="cmbTelesalesTeam_SelectedIndexChanged" Width="180px">
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">ชื่อ Telesales
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbTelesales" runat="server" DataTextField="TextField" DataValueField="ValueField" Width="180px">
                            <asp:ListItem Value="">ทั้งหมด</asp:ListItem>
                        </asp:DropDownList>
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
                            Text="ล้างข้อมูล" OnClick="btnClear_Click" Visible="false" />
                        <asp:Button ID="btnExportExcel" runat="server" CssClass="Button" Width="100px" 
                            Text="Export Excel" OnClick="btnExportExcel_Click" />
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
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1200px" Visible="false"/>
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" OnDataBound="gvResult_DataBound"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1200px">
                <Columns>                    
                    <asp:BoundField HeaderText="Team Code" DataField="slm_TeamTelesales_Code">
                        <ItemStyle Width="60px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Telesales" DataField="slm_StaffNameTH">
                        <ItemStyle Width="60px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Port ตั้งต้น" DataField="startPort">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Port คงค้าง" DataField="pendingPort">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Port ที่ไม่ต่อประกัน" DataField="notContinue">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev0Count" DataField="CountMonthB5">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev1Count" DataField="CountMonthB4">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev2Count" DataField="CountMonthB3">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev3Count" DataField="CountMonthB2">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev4Count" DataField="CountMonthB1">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev5Count" DataField="CountMonthCur">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev6Count" DataField="CountMonthA1">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="prev6Count" DataField="CountMonthA2">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="grand Total" DataField="GrandTotal">
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="% Success">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPercentSuccess" Text='<%# String.Format("{0:0.00}%", Eval("PercentSuccess")) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>                    
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
