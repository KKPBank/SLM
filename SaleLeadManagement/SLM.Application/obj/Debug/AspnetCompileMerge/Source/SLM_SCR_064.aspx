<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_064.aspx.cs" Inherits="SLM.Application.SLM_SCR_064" %>

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
            </table>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height: 5px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td>
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
</asp:Content>
