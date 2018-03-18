<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_011_old.aspx.cs" Inherits="SLM.Application.SLM_SCR_011" %>

<%@ Register src="Shared/LeadInfoEdit.ascx" tagname="LeadInfoEdit" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <uc1:LeadInfoEdit ID="LeadInfoEdit1" runat="server" />
    
</asp:Content>
