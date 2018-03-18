<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_046.aspx.cs" Inherits="SLM.Application.SLM_SCR_046" %>

<%@ Register Src="~/Shared/Prelead_Detail.ascx" TagPrefix="uc1" TagName="Prelead_Detail" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1 {
            width: 20px;
        }

        .style2 {
            width: 180px;
            text-align: left;
            font-weight: bold;
        }

        .style3 {
            width: 380px;
            text-align: left;
        }
        .stylez3 span { display: inline-block; }

        .style4 {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }

        .style5 {
            width: 955px;
        }

        .style6 {
            font-family: Tahoma;
            font-size: 9pt;
            color: blue;
        }
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
    <asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
        <ContentTemplate>
            <uc1:Prelead_Detail runat="server" id="ctlPrelead" />
            <table width="100%">
                <tr>
                    <td class="style2">&nbsp;<span class="style4">*</span></td>
                    <td class="style3">&nbsp;</td>
                    <td class="style1">&nbsp;</td>
                    <td class="style2">&nbsp;</td>
                    <td class="style3">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="5">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="5">
                        <asp:Button ID="btnSave" runat="server" CssClass="Button" Text="บันทึก" Width="90px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing()" />
                        <asp:Button ID="Button1" runat="server" CssClass="Button" Text="ยกเลิก" Width="90px" OnClick="Button1_Click" OnClientClick="DisplayProcessing()" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
