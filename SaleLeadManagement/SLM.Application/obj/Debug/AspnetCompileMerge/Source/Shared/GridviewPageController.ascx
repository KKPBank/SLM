<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridviewPageController.ascx.cs" Inherits="SLM.Application.Shared.GridviewPageController" %>

<asp:Panel id="pnPageControl" runat="server">
    <table cellpadding="3" cellspacing="0" width="100%">
        <tr>
            <td>
                <asp:LinkButton ID="lnbFirst" runat="server" OnClientClick="DisplayProcessing()" OnClick="lnbFirst_Click">[<<]</asp:LinkButton>
                <asp:LinkButton ID="lnbBack" runat="server" OnClientClick="DisplayProcessing()" OnClick="lnbBack_Click">[<]</asp:LinkButton>
                หน้าที่
                <asp:DropDownList ID="cmbPage" runat="server" onchange="DisplayProcessing()" CssClass="Dropdownlist"  Width="70px" AutoPostBack="True" OnSelectedIndexChanged="cmbPage_SelectedIndexChanged">
                </asp:DropDownList>
                จาก&nbsp;<asp:Label ID="lblTotalPage" runat="server"></asp:Label>&nbsp;หน้า 
                <asp:LinkButton ID="lnbNext" runat="server" OnClientClick="DisplayProcessing()" OnClick="lnbNext_Click">[>]</asp:LinkButton>
                <asp:LinkButton ID="lnbLast" runat="server" OnClientClick="DisplayProcessing()" OnClick="lnbLast_Click">[>>]</asp:LinkButton>
            </td>
            <td style="text-align:right;">
                <asp:Label ID="lblSummary" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>