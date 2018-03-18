<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_999.aspx.cs" Inherits="SLM.Application.SLM_SCR_999"%>
<%@ Register src="Shared/Obt/TabActRenewInsure.ascx" tagname="TabActRenewInsure" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="upTabMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <act:TabContainer ID="tabMain" runat="server" ActiveTabIndex="0" Width="1190px" >
                <act:TabPanel ID="tab008" runat="server" >
                    <HeaderTemplate>
                        <asp:Label ID="lblTabAct" runat="server" Text="&nbsp;Activity&nbsp;" CssClass="tabHeaderText"></asp:Label>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc1:TabActRenewInsure ID="tabActRenewInsure" runat="server" OnUpdatedDataChanged="UpdateStatusDesc" />
                    </ContentTemplate>
                </act:TabPanel>
            </act:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
