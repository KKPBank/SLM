<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Lead_Detail_ContinueInsure.ascx.cs" Inherits="SLM.Application.Shared.Lead_Detail_ContinueInsure" %>
<%@ Register Src="~/Shared/Lead_Share_Ins.ascx" TagPrefix="uc1" TagName="Lead_Share_Ins" %>


&nbsp;
<hr />
&nbsp;
<table>
    <tr>
        <td class="style2" style="vertical-align:top;">รายละเอียด</td>
        <td colspan="4">
            <asp:TextBox runat="server" ID="txtDetails" TextMode="MultiLine" Height="70px" Width="770px" CssClass="Textbox" MaxLength="4000" AutoPostBack="true" OnTextChanged="txtDetails_TextChanged"> </asp:TextBox>
            <asp:Label runat="server" ID="vtxtDetail" CssClass="style4"></asp:Label>
        </td>
    </tr>
</table>
&nbsp;
<uc1:Lead_Share_Ins runat="server" id="ctlLeadIns" /> 
