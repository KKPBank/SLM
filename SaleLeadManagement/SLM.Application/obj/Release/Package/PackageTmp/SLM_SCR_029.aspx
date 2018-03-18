<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_029.aspx.cs" Inherits="SLM.Application.SLM_SCR_029" %>
<%@ Register src="Shared/Obt/TabInbound.ascx" tagname="TabInbound" tagprefix="uc3" %>
<%@ Register src="Shared/Obt/TabFollowUp.ascx" tagname="TabFollowUp" tagprefix="uc1" %>
<%--<%@ Register src="Shared/Obt/TabOutbound.ascx" tagname="TabOutbound" tagprefix="uc2" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:190px;
        }
        .ColInput
        {
            width:250px;
        }
        .ColCheckBox
        {
            width:160px;
        }

        .style4 { font-family: Tahoma; font-size: 9pt; color: Red; }
        
        /* เรียกใช้จาก SlmScript.js */
        .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -1px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function doMainSearch() {
            var ctrl = $find('<%=tabMain.ClientID%>');
            var i = ctrl.get_activeTabIndex();

            if (i == 0) {
                document.getElementById('ContentPlaceHolder1_tabMain_tabFollowUp_tabFollowUpCtl_btnSearch').click();
            }
            else if (i == 1) {
                var bt = document.getElementById('ContentPlaceHolder1_tabMain_tabInbound_tabInboundCtl_btnSearch');
                bt.click();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <%--<asp:Button ID="btnMainSearch" runat="server" Text="Main Search" onclick="btnMainSearch_Click" OnClientClick="DisplayProcessing()" CssClass="Hidden" />--%>
    <asp:UpdatePanel ID="upMainSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="btnMainSearch" runat="server" CssClass="Hidden" OnClick="btnMainSearch_Click" OnClientClick="DisplayProcessing();" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <table cellpadding="2" cellspacing="0">
        <tr>
            <td class="ColInfo">งานในมือ Outbound : </td>
            <td class="ColInput" align="right">
                <asp:Label ID="lblOutbound" runat="server" Width="120px" Height="16px" ForeColor="Blue" Font-Bold="true" BorderColor="ActiveBorder" BorderStyle="Solid" BorderWidth="1px" ></asp:Label>
                &nbsp;
                <asp:Label ID="lb1" runat="server" Text="งาน" ForeColor="Blue" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr>
            <td class="ColInfo">งานในมือ Inbound : </td>
            <td class="ColInput" align="right">
                <asp:Label ID="lblInbound" runat="server" Width="120px" Height="16px" ForeColor="Blue" Font-Bold="true" BorderColor="ActiveBorder" BorderStyle="Solid" BorderWidth="1px" ></asp:Label>
                &nbsp;
                <asp:Label ID="Label2" runat="server" Text="งาน" ForeColor="Blue" Font-Bold="true"></asp:Label></td>
        </tr>
    </table>
        <br />
        <asp:UpdatePanel ID="upTabMain" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <act:TabContainer ID="tabMain" runat="server" ActiveTabIndex="0" Width="3060px" >
                <act:TabPanel ID="tabFollowUp" runat="server">
                    <HeaderTemplate>
                        <asp:Label ID="lblTabFollowUp" runat="server" Text="&nbsp;Follow Up&nbsp;" CssClass="tabHeaderText"></asp:Label>                 
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc1:TabFollowUp ID="tabFollowUpCtl" runat="server" OnExtendTab="ExtendTab" />
                    </ContentTemplate>
                </act:TabPanel>
                <act:TabPanel ID="tabInbound" runat="server"  >
                    <HeaderTemplate>
                        <asp:Label ID="lblTabInbound" runat="server" Text="&nbsp;All Task&nbsp;" CssClass="tabHeaderText"></asp:Label>                 
                    </HeaderTemplate>
                    <ContentTemplate>
                            <uc3:TabInbound ID="tabInboundCtl" runat="server" OnExtendTab="ExtendTab" />
                    </ContentTemplate>
                </act:TabPanel>
                </act:TabContainer>
            </ContentTemplate>
        </asp:UpdatePanel>
        
</asp:Content>
