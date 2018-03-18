<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_011.aspx.cs" Inherits="SLM.Application.SLM_SCR_011x" %>

<%@ Register Src="~/Shared/Lead_Share_Common.ascx" TagPrefix="uc1" TagName="Lead_Share_Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .style1 { width: 50px; }
    .style2 { width: 180px; text-align:left; font-weight:bold; }
    .style3 { width: 280px; text-align:left; }
    .style4 { font-family: Tahoma; font-size: 9pt; color: Red; }
    .style5 { width: 955px; }
    .style6 { font-family: Tahoma; font-size: 9pt; color: blue; }
    .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -20px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
        }
    .ui-autocomplete-input { width: 249px !important; margin-right: 0px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel runat="server" ID="updMain">
        <ContentTemplate>

            <uc1:Lead_Share_Common runat="server" id="ctlCommon" />
            <asp:PlaceHolder runat="server" ID="plcControl" EnableViewState="true" />
            &nbsp;
    <hr />
            <table class="style5" runat="server" id="tbSave" >
                <tr>
                    <td align="right">
                        <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button"
                            Width="90px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing()" />
                        &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button"
                        Width="90px" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่')"
                        OnClick="btnCancel_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
