<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_010_old.aspx.cs" Inherits="SLM.Application.SLM_SCR_010" %>
<%@ Register src="Shared/LeadInfo.ascx" tagname="LeadInfo" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .style1 { width: 50px; }
    .style2 { width: 180px; text-align:left; font-weight:bold; }
    .style3 { width: 280px; text-align:left; }
    .style4 { font-family: Tahoma; font-size: 9pt; color: Red; }
    .style5 { width: 955px; }
    .style6 { font-family: Tahoma; font-size: 9pt; color: blue; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:LeadInfo ID="LeadInfo1" runat="server" />
</asp:Content>
