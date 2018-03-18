<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Lead_Detail_KKMotorBox.ascx.cs" Inherits="SLM.Application.Shared.Lead_Detail_KKMotorBox" %>
<%@ Register Src="~/Shared/Lead_Share_Ins.ascx" TagPrefix="uc1" TagName="Lead_Share_Ins" %>
<%@ Register Src="TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>

&nbsp;
<hr />
&nbsp;
<table>
    <tr>
        <td class="style2" style="vertical-align:top;">รายละเอียด</td>
        <td colspan="4">
            <asp:TextBox runat="server" ID="txtDetails" TextMode="MultiLine" Height="70px" Width="770px" CssClass="Textbox" MaxLength="4000" AutoPostBack="true" OnTextChanged="txtDetails_TextChanged"></asp:TextBox>
            <asp:Label runat="server" ID="vtxtDetail" CssClass="style4"></asp:Label>
        </td>
    </tr>
</table>
<table>
    <tr id="trKKBox1" runat="server" style="vertical-align:top">
        <td class="style2">เลขกรมธรรม์<span class="style4">*</span></td>
        <td class="style3">
            <asp:TextBox ID="txtPolicyNo" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
            <asp:Label ID="vtxtPolicyNo" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">ประเภทการขาย</td>
        <td class="style3">
            <asp:RadioButtonList runat="server" ID="rdoPolicySaleType" RepeatDirection="Horizontal">
                <asp:ListItem Value="W">Walk in</asp:ListItem>
                <asp:ListItem Value="C">Center</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr id="trKKBox2" runat="server" style="vertical-align:top">
        <td class="style2">วันที่เริ่มต้นความคุ้มครอง<span class="style4">*</span></td>
        <td class="style3">
            <uc1:TextDateMask ID="tdPolicyStart" runat="server" OnOnTextChanged="tdPolicyStart_OnTextChanged" />
            <asp:Label ID="vtxtPolicyStart" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">วันที่สิ้นสุดความคุ้มครอง<span class="style4">*</span></td>
        <td class="style3">
            <uc1:TextDateMask ID="tdPolicyEnd" runat="server" OnOnTextChanged="tdPolicyEnd_OnTextChanged" />
            <asp:Label ID="vtxtPolicyEnd" runat="server" CssClass="style4"></asp:Label>
        </td>
    </tr>
    <tr id="trKKBox3" runat="server" style="vertical-align:top">
        <td class="style2">ออกที่<span class="style4">*</span></td>
        <td class="style3">
            <asp:DropDownList ID="cmbPolicyIssue" runat="server" Width="253px" CssClass="Dropdownlist" OnSelectedIndexChanged="cmbPolicyIssue_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="1" Text="ธนาคาร"></asp:ListItem>
                <asp:ListItem Value="2" Text="บริษัทประกัน"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">สาขาที่ออก<span class="style4" runat="server" id="reqPolicyBranch">*</span></td>
        <td class="style3">
            <asp:DropDownList ID="cmbPolicyBranch" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbPolicyBranch_SelectedIndexChanged">
                <asp:ListItem Value="1" Text=""></asp:ListItem>
                <asp:ListItem Value="2" Text="สาขาอโศก"></asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="vtxtPolicyBranch" runat="server" CssClass="style4"></asp:Label>
        </td>
    </tr>
    <tr id="tr1" runat="server">
        <td colspan="5">
            <br />
            <asp:CheckBox ID="cheACT" runat="server" Text="ซื้อ พรบ." Font-Bold="true" OnCheckedChanged="cheACT_CheckedChanged" AutoPostBack="true" /></td>
    </tr>
    <tr id="trPRB1" runat="server" visible="false" style="vertical-align:top">
        <td class="style2">เลขที่ พ.ร.บ.<span class="style4"></span></td>
        <td class="style3">
            <asp:TextBox ID="txtActNo" runat="server" CssClass="Textbox-View" Width="250px" MaxLength="100" ></asp:TextBox>
            <asp:Label ID="vtxtActNo" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2"></td>
        <td class="style3"></td>
    </tr>
    <tr id="trPRB2" runat="server" visible="false" style="vertical-align:top">
        <td class="style2">วันที่เริ่มต้น พ.ร.บ.<span class="style4">*</span></td>
        <td class="style3">
            <uc1:TextDateMask ID="tdActStart" runat="server" OnOnTextChanged="tdActStart_OnTextChanged" />
            <asp:Label ID="vtxtActStart" runat="server" CssClass="style4"></asp:Label>
        </td>
        <td class="style1"></td>
        <td class="style2">วันที่สิ้นสุด พ.ร.บ.<span class="style4">*</span></td>
        <td class="style3">
            <uc1:TextDateMask ID="tdActEnd" runat="server" OnOnTextChanged="tdActEnd_OnTextChanged" />
            <asp:Label ID="vtxtActEnd" runat="server" CssClass="style4"></asp:Label>
        </td>
    </tr>
    <tr id="trPRB3" runat="server" visible="false" style="vertical-align:top">
        <td class="style2">ออกที่<span class="style4">*</span></td>
        <td class="style3">
            <asp:DropDownList ID="cmbActIssue" runat="server" Width="253px" CssClass="Dropdownlist" OnSelectedIndexChanged="cmbActIssue_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="1" Text="ธนาคาร"></asp:ListItem>
                <asp:ListItem Value="2" Text="บริษัทประกัน"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="style1"></td>
        <td class="style2">สาขาที่ออก<span class="style4" runat="server" id="reqActBranch">*</span></td>
        <td class="style3">
            <asp:DropDownList ID="cmbActIssueBranch" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="cmbActIssueBranch_SelectedIndexChanged">
                <asp:ListItem Value="0" Text=""></asp:ListItem>
                <asp:ListItem Value="1" Text="สาขาอโศก"></asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="vtxtActIssueBranch" runat="server" CssClass="style4"></asp:Label>
        </td>
    </tr>

</table>
&nbsp;
<uc1:Lead_Share_Ins runat="server" ID="ctlLeadIns" />
