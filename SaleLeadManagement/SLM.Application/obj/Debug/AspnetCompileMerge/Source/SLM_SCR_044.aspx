<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_044.aspx.cs" Inherits="SLM.Application.SLM_SCR_044" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .slm_menu ul { list-style : none; }
        .slm_menu ul li { padding: 3px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="slm_menu">
        <ul>
            <asp:Repeater runat="server" ID="rptMain">
                <ItemTemplate>
                    <li>
                        <a href="#" onclick="window.open('<%# ResolveUrl( Eval("slm_ReportMenuUrl").ToString()) %>', 'TelesaleReport', 'status=yes, toolbar=no, scrollbars=yes, menubar=no, width=1100, height=600, resizable=yes'); return false;"><%# Eval("slm_ReportMenuName") %></a>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</asp:Content>
