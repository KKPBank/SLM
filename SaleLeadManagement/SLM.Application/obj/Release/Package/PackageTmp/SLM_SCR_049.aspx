<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_049.aspx.cs" Inherits="SLM.Application.SLM_SCR_049" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #menuwrapper ul li a { width: inherit; }
         .style1
         {
             width: 180px;
             font-weight:bold;
         }
         .style2
         {
             width: 180px;
         }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="2" cellspacing="0" border="0">
            <tr><td colspan="5" style="height:20px;"></td></tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1">วันที่กดเบิก Incentive เริ่มต้น</td>
                <td class="style2"><uc1:TextDateMask ID="tdmIncFrom" runat="server" /></td>
                <td class="style1">วันที่กดเบิก Incentive สิ้นสุด</td>
                <td><uc1:TextDateMask ID="tdmIncTo" runat="server" /></td>
            </tr>
             <tr style="display:none;">
                <td style="width:50px"></td>
                <td class="style1">วันที่ Call เริ่มต้น</td>
                <td class="style2"><uc1:TextDateMask ID="tdmCallFrom" runat="server" /></td>
                <td class="style1">วันที่ Call สิ้นสุด</td>
                <td><uc1:TextDateMask ID="tdmCallTo" runat="server" /></td>
            </tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1">Team Telesales</td>
                <td class="style2"><asp:DropDownList ID="cmbTelesale" runat="server" Width="152px" CssClass="Dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="cmbTelesale_SelectedIndexChanged">
                    <asp:ListItem Text="ทั้งหมด"></asp:ListItem>
                    <asp:ListItem Text="TAA3"></asp:ListItem>
                    <asp:ListItem Text="TAA4"></asp:ListItem>
                    <asp:ListItem Text="NC"></asp:ListItem>
                    <asp:ListItem Text="TAA2"></asp:ListItem>
                
                </asp:DropDownList></td>
                <td class="style1">ชื่อ Telesales</td>
                <td><asp:DropDownList ID="cmbTelesaleName" runat="server" Width="152px" CssClass="Dropdownlist">
                        <asp:ListItem Value="" Text="ทั้งหมด"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr><td colspan="5" style="height:15px;"></td></tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1"></td>
                <td valign="bottom" colspan="3">
                    <asp:Button ID="btnExcel1" runat="server" Text="Export Sheet สมัครใจ + Sheet พรบ" CssClass="Button" OnClick="btnExcel1_Click" Width="230px" />&nbsp;
                    <asp:Button ID="btnExcel2" runat="server" Text="Export Sheet สมัครใจ + พรบ" CssClass="Button" OnClick="btnExcel2_Click" Visible="False" Width="190px" />
                </td>
            </tr>
        </table>
        <br />
</asp:Content>
