<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_004.aspx.cs" Inherits="SLM.Application.SLM_SCR_004" EnableEventValidation="false" %>
<%@ Register src="Shared/Tabs/Tab005.ascx" tagname="Tab005" tagprefix="uc1" %>
<%@ Register src="Shared/Tabs/Tab008.ascx" tagname="Tab008" tagprefix="uc2" %>
<%@ Register src="Shared/Tabs/Tab004.ascx" tagname="Tab004" tagprefix="uc3" %>
<%@ Register src="Shared/Tabs/Tab009.ascx" tagname="Tab009" tagprefix="uc4" %>
<%@ Register src="Shared/Tabs/Tab007.ascx" tagname="Tab007" tagprefix="uc5" %>
<%@ Register src="Shared/Tabs/Tab006.ascx" tagname="Tab006" tagprefix="uc6" %>
<%@ Register src="Shared/Obt/TabInsureSummary.ascx" tagname="TabInsureSummary" tagprefix="uc8" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc7" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:220px;
        }
        .ColInputView
        {
            width:150px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        
        /* Right Side Menu */
        .TableRightSideMenu
        {
            position:fixed; top:180px; right:0px; display:none;
        }
        
        /* Menu Tab Control */
        .TableTab
        {
            border-collapse:collapse;
        }
        .TableCellIcon
        {
            border-bottom:1px solid #e5edf5;
            width:50px;
            height:35px;
            vertical-align:middle;
            text-align:center;
        }
        .TableCellText
        {
            border-bottom:1px solid #e5edf5;
            width:150px;
            height:35px;
            vertical-align:middle;
        }
        
        /* Premium */
        table.t_tablestyle_premium th {
          border: 1px;
          border-bottom:1px solid #f2f2f2;
          background-color:#cee1f5;
          height:30px;
        }
        .t_row_premium
        {
            background-color: #ffffff;
            height: 32px;
            border:0xp;
        }
        .t_alternative_row_premium
        {
            background-color: #f6f6f6;
            height: 32px;
            border:0xp;
        }
        .textbox_premium
        {
            height:20px;
            text-align:right;
            border:1px solid gray;
        }
        
        /* EditReceiptList */
        .modalPopupEditReceiptList
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:480px;
            height:400px;
        }
         .stylez3 {
            width: 380px;
            text-align: left;
        }
        .stylez3 span { display: inline-block; }
        /* เรียกใช้จาก SlmScript.js */
        .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -1px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
            z-index: 100005;
            
        }
        .ui-autocomplete  
        {
            height: 220px; width:400px; overflow-y: scroll; overflow-x: hidden;
            /* add padding to account for vertical scrollbar */
            padding-right: 5px;
            z-index: 100005;
        }
   </style>
    <script language="javascript" type="text/javascript">
        function ConfirmSave() {

            return confirm('ต้องการบันทึกใช่หรือไม่');
        }
        function GetScreenHeight() {

            document.getElementById('<%= txtScreenWidth.ClientID %>').value = screen.width;
            document.getElementById('<%= txtScreenHeight.ClientID %>').value = screen.height;
            DisplayProcessing();
        }
        function doToggleInbound() {
            var pnInboundInfo = document.getElementById('<%=pnInboundInfo.ClientID%>');
            var lbInboundInfo = document.getElementById('<%=lbInboundInfo.ClientID%>');

            if (pnInboundInfo.style.display == '' || pnInboundInfo.style.display == 'none') {
                lbInboundInfo.innerHTML = "[-] <b>ข้อมูลเพิ่มเติม</b>";
                pnInboundInfo.style.display = 'block';
            }
            else {
                lbInboundInfo.innerHTML = "[+] <b>ข้อมูลเพิ่มเติม</b>";
                pnInboundInfo.style.display = 'none';
            }
        }
        function doToggleTicketIdReferSection() {
            var pnTicketIdReferSection = document.getElementById('<%=pnTicketIdReferSection.ClientID%>');
            var lbTicketIdReferSection = document.getElementById('<%=lbTicketIdReferSection.ClientID%>');

            if (pnTicketIdReferSection.style.display == '' || pnTicketIdReferSection.style.display == 'none') {
                lbTicketIdReferSection.innerHTML = "[-] <b>Ticket ID Refer/Relate</b>";
                pnTicketIdReferSection.style.display = 'block';
            }
            else {
                lbTicketIdReferSection.innerHTML = "[+] <b>Ticket ID Refer/Relate</b>";
                pnTicketIdReferSection.style.display = 'none';
            }
        }

        //Right Menu
        function SetActiveTab(tabControl, tabNumber) {
            var ctrl = $find(tabControl);
            ctrl.set_activeTab(ctrl.get_tabs()[tabNumber]);
        }

        function SideMenuMouseOver(obj) {
            obj.style.background = '#dceaf8'
        }
        function SideMenuMouseOut(obj) {
            obj.style.background = '#ffffff'
        }

        function TabClick(tabCode) {
            document.getElementById('<%=txtTabCode.ClientID %>').value = tabCode;
        }

        function clientActiveTabChanged(sender, args) {
            //var header = sender.get_activeTab().get_headerText().replace('<span>', '').replace('<span class="tabHeaderText">', '').replace('</span>', '').replace('</span>', '');
            //alert("เข้าแล้ว");

            //var tab = sender.get_activeTab();
            //alert(tab._header.id);
            //var index = sender.get_activeTabIndex();
            var tabCode = document.getElementById('<%=txtTabCode.ClientID %>').value;
            if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.LeadInfo%>') {
                var cbLeadInfo = document.getElementById('<%=cbLoadLeadInfo.ClientID %>');
                if (cbLeadInfo.checked == false) {
                    //cbLeadInfo.checked = true;
                    document.getElementById('<%=btnLoadLeadInfo.ClientID %>').click();
                }
            }
            else if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.ExistingLead%>') {
                var cbExistingLead = document.getElementById('<%=cbLoadExistingLead.ClientID %>');
                if (cbExistingLead.checked == false) {
                    cbExistingLead.checked = true;
                    document.getElementById('<%=btnLoadExistingLead.ClientID %>').click();
                }
            }
            else if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.ExistingProduct%>') {
                var cbExistingProduct = document.getElementById('<%=cbLoadExistingProduct.ClientID %>');
                if (cbExistingProduct.checked == false) {
                    cbExistingProduct.checked = true;
                    document.getElementById('<%=btnLoadExistingProduct.ClientID %>').click();
                }
            }
            else if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.OwnerLogging%>') {
                var cbOwnerLogging = document.getElementById('<%=cbLoadOwnerLogging.ClientID %>');
                if (cbOwnerLogging.checked == false) {
                    cbOwnerLogging.checked = true;
                    document.getElementById('<%=btnLoadOwnerLogging.ClientID %>').click();
                }
            }
            else if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.Note%>') {
                var cbNote = document.getElementById('<%=cbLoadNote.ClientID %>');
                if (cbNote.checked == false) {
                    cbNote.checked = true;
                    document.getElementById('<%=btnLoadNote.ClientID %>').click();
                }
            }
            else if (tabCode == '<%=SLM.Resource.SLMConstant.TabCode.InsuranceSummary%>') {
                var cbLoadInsureSummary = document.getElementById('<%=cbLoadInsureSummary.ClientID %>');
                if (cbLoadInsureSummary.checked == false) {
                    cbLoadInsureSummary.checked = true;
                    document.getElementById('<%=btnLoadInsureSummary.ClientID %>').click();
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hGeneral.gif" />
    <asp:UpdatePanel ID="upMainData" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td class="ColInfo">
                        เลขที่สัญญา
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtContractNo" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        Team Code
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtTeamCode" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                    </td>
                    <td class="ColInputView">
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อ
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        นามสกุล
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtLastname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        หมายเลขโทรศัพท์ 1(มือถือ)
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtTelNo_1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Ticket ID
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtTicketID" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                        <asp:TextBox ID="txtPreleadId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtCardTypeId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtCitizenId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtCountryId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtTelNo1" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtStatusCode" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtSubStatusCode" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtCampaignId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtChannelId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtOwnerBranchCode" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtOwnerUsername" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtOwnerEmpCode" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtDelegateBranchCode" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtDelegateUsername" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtProductGroupId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtProductGroupName" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtProductId" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtProductName" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtNextContactDate" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtCarLicenseNo" runat="server" Visible="false" Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtUserLoginChannelId" runat="server" Visible="false"  Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtUserLoginChannelDesc" runat="server" Visible="false"  Width="10px" ></asp:TextBox>
                        <asp:TextBox ID="txtLoginEmpCode" runat="server" Visible="false" Width="10px"></asp:TextBox>
                        <asp:TextBox ID="txtLoginNameTH" runat="server" Visible="false" Width="10px"></asp:TextBox>
                        <asp:TextBox ID="txtLoginStaffTypeId" runat="server" Visible="false" Width="10px"></asp:TextBox>
                        <asp:TextBox ID="txtLoginStaffTypeDesc" runat="server" Visible="false" Width="10px"></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        สถานะของ lead
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtstatus" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        สถานะย่อยของ Lead
                    </td>
                    <td class="ColInputView">
                         <asp:TextBox ID="txtExternalSubStatusDesc" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        แคมเปญ
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtCampaignName" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        ผลิตภัณฑ์/บริการ ที่สนใจ
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtInterestedProd" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        หมายเลขโทรศัพท์ 2
                    </td>
                    <td class="ColInputView">
                       <asp:TextBox ID="txtTelNo2" runat="server" CssClass="TextboxView" ReadOnly="true" Width="72px" ></asp:TextBox>
                        <asp:Label ID="label1" runat="server" Width="10px" CssClass="LabelC" Text="-"></asp:Label>
                        <asp:TextBox ID="txtExt2" runat="server" CssClass="TextboxView" Width="38px" ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Ticket ID Refer
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtTicketIDRefer" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px"></asp:TextBox>
                    </td>
                     <td></td>
                     <td></td>
                     <td></td>
                     <td></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <asp:LinkButton ID="lbInboundInfo" runat="server" ForeColor="Green" Text="[+] <b>ข้อมูลเพิ่มเติม</b>" OnClientClick="doToggleInbound(); return false;" ></asp:LinkButton>
    <asp:Panel ID="pnInboundInfo" runat="server" style="display:none;" >
    <asp:UpdatePanel ID="upMainData2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height:5px;"></td>
                </tr>
                <tr style="vertical-align:top;">
                    <td class="ColInfo" style="font-size:12px;">
                        วันเวลาที่ได้ติดต่อ Prelead ล่าสุด (OBT)
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtContactLatestDatePrelead" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        วันเวลาที่ได้รับมอบหมายล่าสุด (OBT)
                    </td>
                    <td class="ColInputView"> 
                        <asp:TextBox ID="txtAssignDatePrelead" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">หมายเลขโทรศัพท์ 3</td>
                    <td class="ColInputView">
                            <asp:TextBox ID="txtTelNo3" runat="server" CssClass="TextboxView" ReadOnly="true" Width="72px" ></asp:TextBox>
                        <asp:Label ID="label2" runat="server" Width="10px" CssClass="LabelC" Text="-"></asp:Label>
                        <asp:TextBox ID="txtExt3" runat="server" CssClass="TextboxView" Width="38px" ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        วันเวลาที่ได้ติดต่อ Lead ล่าสุด (SLM)
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtContactLatestDate" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        วันเวลาที่ได้รับมอบหมายล่าสุด (SLM)
                    </td>
                    <td class="ColInputView"> 
                        <asp:TextBox ID="txtAssignDate" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo"></td>
                    <td class="ColInputView">
                    </td>
                </tr>
                    <tr>
                    <td class="ColInfo">
                        วันเวลาที่ติดต่อ Lead ครั้งแรก (SLM)
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtContactFirstDate" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        วันเวลาที่ได้รับมอบหมายล่าสุด (COC)
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtCOCAssignDate" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo"></td>
                    <td class="ColInputView"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Owner Branch
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtOwnerBranch" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        Owner Lead
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtOwnerLead" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo"></td>
                    <td class="ColInputView"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Delegate Branch
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtDelegateBranch" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                        
                    </td>
                    <td class="ColInfo">
                        Delegate Lead
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtDelegateLead" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInputView" >
                        
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Marketing Owner
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtMarketingOwner" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px"  Text=""></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        สถานะของ COC
                    </td>
                    <td colspan="3" class="ColInputView">
                        <asp:TextBox ID="txtCocStatus" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        LastOwner
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtLastOwner" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px"  Text=""></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        Team
                    </td>
                    <td class="ColInputView">
                        <asp:TextBox ID="txtCocTeam" runat="server" CssClass="TextboxView" ReadOnly="true" Width="130px"  Text=""></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                            
                    </td>
                    <td class="ColInputView" >
                        
                    </td>
                </tr>
                    <tr>
                    <td valign="top" class="style2">รายละเอียด</td>
                    <td colspan="5">
                            <asp:TextBox ID="txtDetail" runat="server" CssClass="TextboxView" Width="770px" Height="70px" TextMode ="MultiLine"  ReadOnly="true" ></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Panel>
    <br /><br />
    <%--<asp:LinkButton ID="lbTicketIdReferSection" runat="server" ForeColor="Green" Text="[+] <b>Ticket ID Refer/Relate</b>" OnClientClick="doToggleTicketIdReferSection(); return false;" ></asp:LinkButton>--%>
    <%--<asp:Panel ID="pnTicketIdReferSection" runat="server" style="display:none;" >--%>
        
        <asp:UpdatePanel ID="upTicketReferRelate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:LinkButton ID="lbTicketIdReferSection" runat="server" ForeColor="Green" Text="[+] <b>Ticket ID Refer/Relate</b>" OnClick="lbTicketIdReferSection_Click" OnClientClick="DisplayProcessing()" ></asp:LinkButton>
                <asp:CheckBox ID="cbTicketReferSectionLoad" runat="server" Visible="false" />
                <asp:Panel ID="pnTicketIdReferSection" runat="server" Visible="false" >
                <table width="1190px" border="0" cellpadding="0" cellspacing="0" runat="server">
                    <tr>
                        <td style="height:5px;"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gvTicketRefer" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" ShowHeaderWhenEmpty="true"  
                                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1150px" >
                                <Columns>
                                    <asp:BoundField DataField="TicketId" HeaderText="Ticket ID Refer">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardTypeDesc" HeaderText="ประเภทบุคคล">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Name" HeaderText="ชื่อ">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LastName" HeaderText="นามสกุล">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status" HeaderText="สถานะของ Lead">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ExternalSubStatusDesc" HeaderText="สถานะย่อยของ Lead">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead" />
                                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td><br /></td>
                    </tr>
                    <tr>
                        <td>
                            <div id="divTicketRelate" runat="server">                        
                                <asp:GridView ID="gvTicketRelate" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"  ShowHeaderWhenEmpty="true" 
                                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1150px">
                                <Columns>
                                    <asp:BoundField DataField="TicketId" HeaderText="Ticket ID Relate">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardTypeDesc" HeaderText="ประเภทบุคคล">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Name" HeaderText="ชื่อ">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LastName" HeaderText="นามสกุล">
                                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status" HeaderText="สถานะของ Lead">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ExternalSubStatusDesc" HeaderText="สถานะย่อยของ Lead">
                                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead" />
                                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                            </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><br /></td>
                    </tr>
                </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    
    
    <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height:3px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td >
                        <asp:Button ID="btnBack" runat="server" Text="< กลับหน้าค้นหา" CssClass="Button" Width="110px" onclick="btnBack_Click" OnClientClick="DisplayProcessing();" style="vertical-align:top;" />&nbsp;&nbsp;
                        <asp:Button ID="btnNextLead" runat="server" Text="Next Lead >" CssClass="Button" Width="110px" onclick="btnNextLead_Click" OnClientClick="DisplayProcessing();" style="vertical-align:top;"  />&nbsp;&nbsp;
                        <asp:ImageButton ID="imbCal" runat="server" Width="25px" ImageUrl="~/Images/Calculator.png" ImageAlign="AbsMiddle" ToolTip="Calculator"  />&nbsp;&nbsp;
                        <asp:ImageButton id="imbDoc" runat="server" ImageUrl="~/Images/Document.png" Width="25px"  ImageAlign="AbsMiddle" ToolTip="แนบเอกสาร"  />&nbsp;&nbsp;
                        <asp:ImageButton ID="imbOthers" runat="server" Width="25px"  ImageUrl="~/Images/Others.png" ImageAlign="AbsMiddle" ToolTip="เรียกดูข้อมูลเพิ่มเติม" />&nbsp;&nbsp;
                        <asp:ImageButton id="imbCopyLead" runat="server" 
                            ImageUrl="~/Images/page_copy.png" Width="25px"  
                            ImageAlign="AbsMiddle" ToolTip="คัดลอกข้อมูลผู้มุ่งหวัง" 
                            onclick="imbCopyLead_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br /><br />
    <asp:UpdatePanel ID="upHistory" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="1190px" >
                <tr>
                    <td>
                        <asp:Image ID="imgHistory" runat="server" ImageUrl="~/Images/CampaignFinal.gif" ImageAlign="Top" />&nbsp;&nbsp;
                        <%--<asp:Button ID="btnAdd" runat="server" Visible="false" Text ="แคมเปญทั้งหมด" Width="120px" CssClass="Button" onclick="btnAdd_Click" />--%>
                        &nbsp;&nbsp;
                        <%--<asp:Button ID="btnOfferCampaign" runat="server" Visible="false" Text ="แนะนำแคมเปญ" Width="120px" CssClass="Button" onclick="btnOfferCampaign_Click" /> --%>
                        <asp:Button ID="btnAllCampaign" runat="server" Text ="แคมเปญทั้งหมด" Width="120px" CssClass="Button" OnClientClick="GetScreenHeight()" onclick="btnAllCampaign_Click" />
                        <asp:TextBox ID="txtScreenHeight" runat="server" CssClass="Hidden"></asp:TextBox>
                        <asp:TextBox ID="txtScreenWidth" runat="server" CssClass="Hidden"></asp:TextBox>
                    </td>
                    <td align="right">
                        รวมทั้งสิ้น
                        <asp:Label ID="lbSum" runat="server"></asp:Label>
                        รายการ
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlHistory" runat="server" ScrollBars ="Auto" Height="170px" Width="1190px" BorderStyle="Solid" BorderWidth="1px" >
                <asp:GridView ID="gvCampaign" runat="server" AutoGenerateColumns="False" 
                    GridLines="Horizontal" BorderWidth="0px"  Width="1160px"
                    EnableModelValidation="True" 
                    EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" 
                    onrowdatabound="gvCampaign_RowDataBound"  > 
                    <Columns>
                        <asp:TemplateField HeaderText="CampaignFinalId">
                            <ItemTemplate>
                                <asp:Label ID="lbCampaignFinalId"  runat="server" Text='<%#Bind("CampaignFinalId") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lbCampaignFinalIdEdit" runat="server" Text='<%#Bind("CampaignFinalId") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle CssClass="Hidden" />
                            <HeaderStyle CssClass="Hidden" />
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="No.">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top"/>
                            <HeaderStyle Width="40px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อ Product/Campaign">
                            <ItemTemplate>
                                <asp:Label ID="lbCampaign"  runat="server" Text='<%#Bind("CampaignName") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="220px" HorizontalAlign="Left" VerticalAlign="Top"/>
                            <HeaderStyle Width="220px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รายละเอียด">
                            <ItemTemplate>
                                <asp:Label ID="lbCampaignDetail" runat="server" Text='<%#Bind("CampaignDetail") %>'></asp:Label>
                                <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Width="600px" HorizontalAlign="Left" VerticalAlign="Top"/>
                            <HeaderStyle Width="600px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="วันที่ดำเนินการ">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <ItemStyle Width="140px" HorizontalAlign="Center" VerticalAlign="Top"/>
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="ผู้ดำเนินการ">
                            <ItemTemplate>
                                <asp:Label ID="lbCreatedByName"  runat="server" Text='<%#Bind("CreatedByName") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top"/>
                            <HeaderStyle Width="160px" HorizontalAlign="Center"/>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate> 
    </asp:UpdatePanel> 
    <br />
    <asp:UpdatePanel ID="upTabMain" runat="server" UpdateMode="Conditional" >
        <ContentTemplate>
            <asp:TextBox ID="txtTabCode" runat="server" CssClass="Hidden" Width="50px"></asp:TextBox>
            <asp:Button ID="btnLoadLeadInfo" runat="server" Text="LoadExistingLead" OnClick="btnLoadLeadInfo_Click" CssClass="Hidden" />
            <asp:CheckBox ID="cbLoadLeadInfo" runat="server" CssClass="Hidden"/>
            <asp:Button ID="btnLoadExistingLead" runat="server" Text="LoadExistingLead" OnClick="btnLoadExistingLead_Click" CssClass="Hidden"/>
            <asp:CheckBox ID="cbLoadExistingLead" runat="server" CssClass="Hidden"/>
            <asp:Button ID="btnLoadExistingProduct" runat="server" Text="LoadExistingProduct" OnClick="LoadExistingProduct_Click" CssClass="Hidden"/>
            <asp:CheckBox ID="cbLoadExistingProduct" runat="server" CssClass="Hidden"/>
            <asp:Button ID="btnLoadOwnerLogging" runat="server" Text="LoadOwnerLogging" OnClick="btnLoadOwnerLogging_Click" CssClass="Hidden"/>
            <asp:CheckBox ID="cbLoadOwnerLogging" runat="server" CssClass="Hidden"/>
            <asp:Button ID="btnLoadNote" runat="server" Text="LoadNote" OnClick="btnLoadNote_Click" CssClass="Hidden"/>
            <asp:CheckBox ID="cbLoadNote" runat="server" CssClass="Hidden"/>
            <asp:Button ID="btnLoadInsureSummary" runat="server" Text="LoadInsureSummary" OnClick="btnLoadInsureSummary_Click" CssClass="Hidden"/>
            <asp:CheckBox ID="cbLoadInsureSummary" runat="server" CssClass="Hidden"/>
            <act:TabContainer ID="tabMain" runat="server" ActiveTabIndex="0" Width="1190px" OnClientActiveTabChanged="clientActiveTabChanged" >
            </act:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Right Menu Section -->

    <script type="text/javascript">
        $(document).ready(function () {
            setTimeout(function () {
                $("#<%=tabRightSideMenu.ClientID%>").show(1000);
            }, 100);

            $("#<%=imbMenuTab.ClientID%>").click(function () {
                var main_pos = $("#<%=tabRightSideMenu.ClientID%>").position();
                var menu_pos = $(this).position();

                var new_top = (main_pos.top + menu_pos.top) - 14;
                var new_left = (main_pos.left + menu_pos.left) - 237;
                var divMenuTab = $("#<%=divMenuTab.ClientID%>");

                divMenuTab.css("position", "fixed");
                divMenuTab.css("top", new_top + "px");
                divMenuTab.css("left", new_left + "px");
                divMenuTab.toggle("slide", { direction: "right" }, 500);

                if ($("#<%=divMenuButton.ClientID%>") != null)
                    $("#<%=divMenuButton.ClientID%>").hide();

                if ($("#<%=divMenuPremium.ClientID%>") != null)
                    $("#<%=divMenuPremium.ClientID%>").hide();

                return false;
            });

            $("#<%=imbMenuButton.ClientID%>").click(function () {
                var main_pos = $("#<%=tabRightSideMenu.ClientID%>").position();
                var menu_pos = $(this).position();

                var offset = $(this).offset();
                var new_top = (main_pos.top + menu_pos.top) - 17;
                var new_left = (main_pos.left + menu_pos.left) - 242;
                var divMenuButton = $("#<%=divMenuButton.ClientID%>");

                divMenuButton.css("position", "fixed");
                divMenuButton.css("top", new_top + "px");
                divMenuButton.css("left", new_left + "px");
                divMenuButton.toggle("slide", { direction: "right" }, 500);

                if ($("#<%=divMenuTab.ClientID%>") != null)
                    $("#<%=divMenuTab.ClientID%>").hide();

                if ($("#<%=divMenuPremium.ClientID%>") != null)
                    $("#<%=divMenuPremium.ClientID%>").hide();

                return false;
            });

            $("#<%=imbMenuPremium.ClientID%>").click(function () {
                var main_pos = $("#<%=tabRightSideMenu.ClientID%>").position();
                var menu_pos = $(this).position();

                var offset = $(this).offset();
                var new_top = (main_pos.top + menu_pos.top) - 18;
                var new_left = (main_pos.left + menu_pos.left) - 467;
                var divMenuPremium = $("#<%=divMenuPremium.ClientID%>");

                divMenuPremium.css("position", "fixed");
                divMenuPremium.css("top", new_top + "px");
                divMenuPremium.css("left", new_left + "px");
                divMenuPremium.toggle("slide", { direction: "right" }, 500);

                if ($("#<%=divMenuTab.ClientID%>") != null)
                    $("#<%=divMenuTab.ClientID%>").hide();

                if ($("#<%=divMenuButton.ClientID%>") != null)
                    $("#<%=divMenuButton.ClientID%>").hide();

                return false;
            });

            $("#<%=divMenuInfo.ClientID%>").css({ "opacity": "1.0" });
            $("#<%=imbMenuInfo.ClientID%>").click(function () {
                var divMenuInfo = $("#<%=divMenuInfo.ClientID%>");
                var divMenuScript = $("#<%=divMenuSaleScript.ClientID%>");

                if (divMenuInfo.is(":visible")) {
                    divMenuScript.css("top", "0px");
                }
                else {
                    var height = divMenuInfo.height() + 1;
                    divMenuScript.css("top", height + "px");
                }

                divMenuInfo.slideToggle();
                return false;
            });

            $('#<%=imbMenuSaleScript.ClientID%>').click(function () {
                var divMenuInfo = $('#<%=divMenuInfo.ClientID%>');
                var divMenuScript = $('#<%=divMenuSaleScript.ClientID%>');

                if (divMenuInfo.is(":visible")) {
                    var height = divMenuInfo.height() + 1;
                    divMenuScript.css("top", height + "px");
                }
                else {
                    divMenuScript.css("top", "0px");
                }

                divMenuScript.slideToggle();
                return false;
            });
        });

        function HideDivMenuButton() {
            document.getElementById('<%=divMenuButton.ClientID%>').style.display = 'none';
        }
    </script>

    <asp:Table ID="tabRightSideMenu" runat="server" CssClass="TableRightSideMenu">
        <asp:TableRow ID="trMenuTab" runat="server" Visible="false">
            <asp:TableCell>
                <asp:ImageButton ID="imbMenuTab" runat="server"  Width="36px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trMenuButton" runat="server" Visible="false">
            <asp:TableCell>
                <asp:ImageButton ID="imbMenuButton" runat="server"   Width="36px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trMenuPremium" runat="server" Visible="false">
            <asp:TableCell>
                <asp:ImageButton ID="imbMenuPremium" runat="server" Width="36px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trMenuInfo" runat="server" Visible="false">
            <asp:TableCell>
                <asp:ImageButton ID="imbMenuInfo" runat="server"  Width="36px" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trMenuSaleScript" runat="server" Visible="false">
            <asp:TableCell>
                <asp:ImageButton ID="imbMenuSaleScript" runat="server"  Width="36px" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <!-- Menu Tab Control -->
    <div id="divMenuTab" runat="server" style="border:1px solid #d7d9dc; border-radius:8px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); height:308px; width:220px; background-color:White; display:none;">
        <table cellspacing="0" cellpadding="0" border="0" width="100%";>
            <tr>
                <td colspan="2" style="width:3px; height:5px; font-weight:bold; padding:3px; border-bottom: 1px solid #d7d9dc; ">
                    &nbsp;&nbsp;Tabs
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnTableTab" runat="server" Height="270px" Width="100%" ScrollBars="Auto">
                        <asp:UpdatePanel ID="upGenerateTabMenu" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Table ID="tableTabMenu" runat="server" CssClass="TableTab"></asp:Table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <%--<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/triangle_right.jpg" style="top:-270px; left:220px; position:relative;" />--%>
    </div>

    <!-- Menu Button Control -->
    <div id="divMenuButton" runat="server" style="border:1px solid #d7d9dc; border-radius:8px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); height:440px; width:225px; background-color:White; display:none;">
        <table cellspacing="0" cellpadding="0" border="0" width="100%";>
            <tr>
                <td colspan="2" style="width:3px; height:5px; font-weight:bold; padding:3px; border-bottom: 1px solid #d7d9dc">
                    &nbsp;&nbsp;Functions
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlMenuButton" runat="server" Height="398px" Width="100%" ScrollBars="Auto">
                        <asp:UpdatePanel ID="upMenuButton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="0" border="0" class="TableTab">
                                    <tr id="trButtonCalculator" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imgCalculator" runat="server" ImageUrl="~/Images/Calculator.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbCalculator" runat="server" Text="Calculator" ForeColor="Black"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trButtonDocument" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imgDocument" runat="server" ImageUrl="~/Images/Document.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbDocument" runat="server" Text="แนบเอกสาร" ForeColor="Black"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trButtonOthers" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="ImgOthers" runat="server" ImageUrl="~/Images/Others.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbOthers" runat="server" Text="เรียกดูข้อมูลเพิ่มเติม" ForeColor="Black"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trButtonCopyLead" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imgCopyLead" runat="server" ImageUrl="~/Images/page_copy.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbCopyLead" runat="server" Text="คัดลอกข้อมูลผู้มุ่งหวัง" ForeColor="Black" OnClick="lbCopyLead_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();" ></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trPO" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imgPO" runat="server" ImageUrl="~/Images/excel_file_small.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbPO" runat="server" Text="พิมพ์ใบเสนอขาย" ForeColor="Black" OnClick="lbPO_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();" ></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="tr50Tawi" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="img50Tawi" runat="server" ImageUrl="~/Images/excel_file_small.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lb50Tawi" runat="server" Text="พิมพ์ใบ 50 ทวิ" ForeColor="Black" OnClick="lb50Tawi_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trCreditForm" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imbCreditForm" runat="server" ImageUrl="~/Images/excel_file_small.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbCreditForm" runat="server" Text="พิมพ์ฟอร์มตัดบัตรเครดิต" ForeColor="Black" OnClick="lbCreditForm_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();" ></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trEditReceipt" runat="server" onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imbEditReceipt" runat="server" ImageUrl="~/Images/excel_file_small.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbEditReceipt" runat="server" Text="พิมพ์ฟอร์มแก้ไขใบเสร็จ" ForeColor="Black" OnClick="lbEditReceipt_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();" ></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="imgQuestionaire" runat="server" ImageUrl="~/Images/doc_small_icon.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbQuestionaire" runat="server" Text="แบบสอบถาม" ForeColor="Black"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="Image1" runat="server" ImageUrl="~/Images/arrow_left_icon.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbBackToSearch" runat="server" Text="กลับหน้าค้นหา" ForeColor="Black" OnClick="lbBackToSearch_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();"></asp:LinkButton>
                                        </td>
                                    </tr>   
                                    <tr onmouseover="SideMenuMouseOver(this)" onmouseout="SideMenuMouseOut(this)">
                                        <td class="TableCellIcon">
                                            <asp:Image id="Image4" runat="server" ImageUrl="~/Images/arrow_right_icon.png" Width="25px" />
                                        </td>
                                        <td class="TableCellText">
                                            <asp:LinkButton ID="lbNextLead" runat="server" Text="Next Lead" ForeColor="Black" OnClick="lbNextLead_Click" OnClientClick="HideDivMenuButton(); DisplayProcessing();"></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <%--<asp:Image ID="Image4" runat="server" ImageUrl="~/Images/triangle_right.jpg" style="top:-270px; left:220px; position:relative;" />--%>
    </div>

    <!-- Menu Premium -->
    <div id="divMenuPremium" runat="server" style="border:1px solid #d7d9dc; border-radius:8px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); height:294px; width:450px; background-color:White; display:none;" >
        <asp:UpdatePanel id="upPremium" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table cellspacing="0" cellpadding="0" border="0" width="100%";>
                    <tr>
                        <td colspan="2" style="width:3px; height:5px; font-weight:bold; padding:3px; border-bottom: 1px solid #d7d9dc">
                            &nbsp;&nbsp;Premium&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="width:10px;"></td>
                        <td>
                            <asp:Panel ID="pnlMenuPremium" runat="server" Height="250px" Width="100%" ScrollBars="Auto">
                                <asp:GridView ID="gvPremium" runat="server" AutoGenerateColumns="False" CssClass="t_tablestyle_premium" 
                                    GridLines="None" EnableModelValidation="True"   
                                    EmptyDataText="<span style='color:Gray; text-align:center;'>ไม่พบข้อมูล</span>"  
                                    Width="415px" onrowdatabound="gvPremium_RowDataBound" >
                                    <Columns>
                                        <asp:TemplateField HeaderText="" >
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSelected" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ชื่อ Premium">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPremiumName" runat="server" Text='<%# Eval("PremiumName") %>' ></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="155px" HorizontalAlign="Left" VerticalAlign="Top" />
                                            <HeaderStyle Width="155px" HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="คงเหลือ" >
                                            <ItemTemplate>
                                                <asp:Label ID="lblTotalRemain" runat="server" Text='<%# Eval("TotalRemain") != null ? ( Convert.ToInt32(Eval("TotalRemain")).ToString("#,##0") ) : "" %>' ></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="จำนวนที่ให้" >
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtTotalGive" runat="server" Width="50px" Enabled="false" CssClass="textbox_premium" onblur="ChkIntOnBlurClear(this)" onkeypress="return ChkInt(event)" Text='<%# Eval("TotalGive") %>'></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" >
                                            <ItemTemplate>
                                                <asp:Button ID="btnReturnPremium" runat="server" Text="คืนของ" Width="60px" CssClass="Button" CommandArgument='<%# Container.DisplayIndex %>' OnClick="btnReturnPremium_Click" OnClientClick="if (confirm('ต้องการคืนของใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
                                            </ItemTemplate>
                                            <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" >
                                            <ItemTemplate>
                                                <asp:Label ID="lblPremiumId" runat="server" Text='<%# Eval("PremiumId") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="Hidden" />
                                            <HeaderStyle CssClass="Hidden"  />
                                            <FooterStyle CssClass="Hidden" />
                                            <ControlStyle CssClass="Hidden" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" >
                                            <ItemTemplate>
                                                <asp:Label ID="lblRaId" runat="server" Text='<%# Eval("RaId") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="Hidden" />
                                            <HeaderStyle CssClass="Hidden"  />
                                            <FooterStyle CssClass="Hidden" />
                                            <ControlStyle CssClass="Hidden" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <RowStyle CssClass="t_row_premium" />
                                    <AlternatingRowStyle CssClass="t_alternative_row_premium" />
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Menu CustomerInfo -->
    <div id="divMenuInfo" runat="server" style="position:fixed; top:0px; left:0px; border:0px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); background-color:#f1f7fc; display:none;">
        <table cellpadding="2" cellspacing="0" border="0">
            <tr style="background-color:#f1f7fc;">
                <td style="width:850px; height:25px; vertical-align:middle; color:White; background-color:#800080;">
                    <b>ชื่อพนักงาน: </b><asp:Label ID="lblInfoStaffName" runat="server"></asp:Label>&nbsp;&nbsp;
                    <b>ตำแหน่ง: </b><asp:Label ID="lblInfoStaffPositionName" runat="server"></asp:Label>&nbsp;&nbsp;
                    <b>Team: </b><asp:Label ID="lblInfoStaffTeam" runat="server"></asp:Label>&nbsp;&nbsp;
                </td>
            </tr>
            <tr><td style="height:1px;"></td></tr>
            <tr id="trLicenseNotFound" runat="server" visible="false">
                <td>
                    <asp:Label ID="lblLicenseNotFound" runat="server" Text="ไม่พบข้อมูล License" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Repeater ID="rptUserLicense" runat="server" ClientIDMode="AutoID">
                        <HeaderTemplate>
                            <table cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width:220px;">
                                    <%# Eval("LicenseTypeDesc") %>
                                </td>
                                <td style="width:200px;">
                                    License No:&nbsp;<%# Eval("LicenseNo") %></td>
                                <td style="width:200px;">
                                    วันหมดอายุ License:&nbsp;<%# Eval("LicenseExpireDate") != null ? Convert.ToDateTime(Eval("LicenseExpireDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("LicenseExpireDate")).Year.ToString() : "" %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr>
                <td style="height:23px; vertical-align:bottom; color:Black;">
                    <b>เลขที่สัญญา: </b><asp:Label ID="lblInfoContractNo" runat="server"></asp:Label>&nbsp;&nbsp;
                    <b>ชื่อลูกค้า: </b><asp:Label ID="lblInfoCustomerName" runat="server"></asp:Label>&nbsp;&nbsp;
                    <b>Ticket Id: </b><asp:Label ID="lblInfoTicketId" runat="server"></asp:Label>&nbsp;&nbsp;
                    <b>แคมเปญ: </b><asp:Label ID="lblInfoCampaignName" runat="server"></asp:Label>&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td style="height:16px; vertical-align:bottom; color:Black;">
                     <b>ยี่ห้อรถ: </b><asp:Label ID="lblInfoCarBrandName" runat="server"></asp:Label>&nbsp;&nbsp;
                     <b>รุ่นรถ: </b><asp:Label ID="lblInfoCarModelName" runat="server"></asp:Label>&nbsp;&nbsp;
                     <b>ทะเบียน: </b><asp:Label ID="lblInfoCarLicenseNo" runat="server"></asp:Label>&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td style="height:8px;"></td>
            </tr>
        </table>
    </div>

    <!-- MenuSaleScript -->
    <div id="divMenuSaleScript" runat="server" style="position:fixed; top:86px; left:0px; height:238px; width:854px; border:0px solid Gray; border-left:0px; border-top:1px solid #d8d8d8; border-buttom:1px solid #d8d8d8; border-bottom-left-radius:5px; border-bottom-right-radius:5px;  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); background-color:#f1f7fc; display:none; ">
        <asp:UpdatePanel ID="upSaleScript" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height:10px;"></div>
                <act:TabContainer ID="tabContainerSaleScript" runat="server" ActiveTabIndex="0" Height="183px" Width="853px" >
                    <act:TabPanel ID="tabPanelSaleScript" runat="server" ScrollBars="Auto"  >
                        <HeaderTemplate>
                            <asp:Label ID="Label5" runat="server" Text="&nbsp;สคริปการขาย&nbsp;" CssClass="tabHeaderText"></asp:Label>                 
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:Panel ID="pnlSaleScirpt" runat="server" Height="188px" Width="840px" ScrollBars="Auto" >
                                <asp:Repeater ID="rptSaleScript" runat="server" ClientIDMode="AutoID">
                                    <HeaderTemplate>
                                        <table cellpadding="0" cellspacing="0" border="0">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="font-size:14px; font-weight:bold; text-decoration:underline; height:25px;">
                                                <asp:Label ID="lblScriptSubject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left:20px;">
                                                <asp:Label ID="ltScriptDetail" runat="server" Text='<%# Eval("Detail") %>' ></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height:10px;"></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>         
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </ContentTemplate>
                    </act:TabPanel>
                    <act:TabPanel ID="tabPanelQandA" runat="server">
                            <HeaderTemplate>
                            <asp:Label ID="Label6" runat="server" Text="&nbsp;Q&A&nbsp;" CssClass="tabHeaderText"></asp:Label>                 
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:Panel ID="pnlQandA" runat="server" Height="188px" Width="840px" ScrollBars="Auto">
                                <asp:Repeater ID="rptQandA" runat="server" ClientIDMode="AutoID">
                                    <HeaderTemplate>
                                        <table cellpadding="0" cellspacing="0" border="0">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="font-size:14px; font-weight:bold; text-decoration:underline; height:25px;">
                                                <asp:Label ID="lblQandASubject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left:20px;">
                                                <asp:Label ID="ltQandADetail" runat="server" Text='<%# Eval("Detail") %>'></asp:Label>
                                                <%--<asp:Literal ID="ltQandADetail" runat="server" Text='<%# Eval("Detail") %>'></asp:Literal>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height:10px;"></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>         
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </ContentTemplate>
                    </act:TabPanel>
                </act:TabContainer>
                <div style="position:relative; width:340px; top:-226px; left:528px; border:0px">
                    <asp:TextBox ID="txtSearchScript" runat="server" style="border:1px solid Gray; font-size:13px; font-family:Tahoma;" Width="228px" onfocus="this.select();" ></asp:TextBox>&nbsp;
                    <asp:Button ID="btnSearchScript" runat="server" Text="ค้นหา" Width="80px" OnClick="btnSearchScript_Click"  />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>  
    
    <!-- End Right Menu Section -->

    <asp:UpdatePanel ID="upPopupSearchCampaign" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSearchCampaign" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSearchCampaign" style="display:none" CssClass="modalBoxlSearchBundleCampaign" ScrollBars="Auto">
            <table>
                <tr>
                    <td style="color:Red; padding-left:15px; height:20px; vertical-align:bottom">* เลือกแคมเปญได้มากกว่า 1 รายการ แล้วกดปุ่ม "เลือก"
                    </td>
                </tr>
            </table>
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image2" runat="server" ImageUrl="~/Images/SearchBundle.jpg" />
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="width:20px; height:212px;"></td>
                    <td style="height:212px; vertical-align:top;">
                        <asp:TextBox ID="txtBundleCampaignIdList" runat="server" Width="30px" Visible="false"></asp:TextBox>
                        <uc7:GridviewPageController ID="pcGridBundleCampaign" runat="server" OnPageChange="PageSearchChangeBundleCampaign" Width="910px" />
                        <asp:GridView ID="gvBundleCampaign" runat="server" AutoGenerateColumns="False" Width="910px"
                            GridLines="Horizontal" BorderWidth="1px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                            onrowdatabound="gvBundleCampaign_RowDataBound"  >
                            <Columns>
                                <asp:TemplateField HeaderText="เลือก">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelectCampaign" runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="40px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductGroupName" HeaderText="กลุ่มผลิตภัณฑ์/บริการ"  >
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="แคมเปญ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="รายละเอียดแคมเปญ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Eval("CampaignDesc") %>'></asp:Label>
                                        <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="180px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                                    <ItemTemplate>
                                        <%# Eval("StartDate") != null ? Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                    <ItemTemplate>
                                        <%# Eval("EndDate") != null ? Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="แนะนำ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCmt" runat="server" ForeColor="Red" Font-Bold="true" Text='<%# Eval("Recommend") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CampaignId">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ProductGroupId">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ProductId">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            
            <hr style="border-top:1px solid gray; border-bottom-style:none; border-left-style:none; border-right-style:none;" />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image3" runat="server" ImageUrl="~/Images/SearchCampaignAll.jpg" ImageAlign="AbsMiddle" />
            <asp:Label ID="lblPopupInfo" runat="server" ForeColor="Red"></asp:Label>
            <asp:UpdatePanel ID="upPopupSearchCampaignInner" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr>
                            <td style="width:20px; "></td>
                            <td>
                            </td>
                            <td style="width:220px; font-weight:bold;">กลุ่มผลิตภัณฑ์/บริการ</td>
                            <td style="width:220px; font-weight:bold;">ผลิตภัณฑ์/บริการ</td>
                            <td style="font-weight:bold;">แคมเปญ</td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td>
                                <asp:RadioButton ID="rbSearchByCombo" runat="server" GroupName="Campaign" AutoPostBack="true"
                                    Checked="true" oncheckedchanged="rbSearchByCombo_CheckedChanged"  />
                            </td>
                            <td style="width:220px;">
                                <asp:DropDownList ID="cmbProductGroup" runat="server" AutoPostBack="true" CssClass="Dropdownlist" Width="200px"
                                onselectedindexchanged="cmbProductGroup_SelectedIndexChanged" ></asp:DropDownList>
                            </td>
                            <td style="width:220px;">
                                <asp:DropDownList ID="cmbProduct" runat="server" AutoPostBack="true" CssClass="Dropdownlist" Width="200px"
                                onselectedindexchanged="cmbProduct_SelectedIndexChanged" ></asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="200px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td>
                            </td>
                            <td colspan="3" style="font-weight:bold;" >คำที่ต้องการค้นหา</td>
                        </tr>
                        <tr >
                            <td style="width:20px;"></td>
                            <td>
                                <asp:RadioButton ID="rbSearchByText" runat="server" GroupName="Campaign" AutoPostBack="true" 
                                    oncheckedchanged="rbSearchByText_CheckedChanged"  />
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtFullSearchCampaign" runat="server" CssClass="Textbox" Width="420px"></asp:TextBox>
                            </td>
                            <td>
                        
                            </td>
                        </tr>
                        <tr><td colspan="5" style="height:1px;"></td></tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td colspan="4">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSearchCampaign" runat="server" Text="ค้นหา" Width="80px" 
                                    CssClass="Button" onclick="btnSearchCampaign_Click" OnClientClick="DisplayProcessing()" />
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr>
                            <td style="width:20px;"></td>
                            <td style="height:200px; vertical-align:top; "><br />
                                <uc7:GridviewPageController ID="pcGridCampaign" runat="server" OnPageChange="PageSearchChangeCampaign" Width="910px" />
                                <asp:GridView ID="gvAllCampaign" runat="server" AutoGenerateColumns="False" Width="910px"
                                    GridLines="Horizontal" BorderWidth="1px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                                    onrowdatabound="gvAllCampaign_RowDataBound"  >
                                    <Columns>
                                        <asp:TemplateField HeaderText="เลือก">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSelectCampaign" runat="server" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="40px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ProductGroupName" HeaderText="กลุ่มผลิตภัณฑ์/บริการ"  >
                                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="150px" HorizontalAlign="Left"  VerticalAlign="Top"/>
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="แคมเปญ">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="รายละเอียดแคมเปญ">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Eval("CampaignDesc") %>'></asp:Label>
                                                <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                                            </ItemTemplate>
                                            <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="180px" HorizontalAlign="Left" VerticalAlign="Top"  />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                                            <ItemTemplate>
                                                <%# Eval("StartDate") != null ? Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() : ""%>
                                            </ItemTemplate>
                                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                            <ItemTemplate>
                                                <%# Eval("EndDate") != null ? Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() : ""%>
                                            </ItemTemplate>
                                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="แนะนำ">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCmt" runat="server" ForeColor="Red" Font-Bold="true" Text='<%# Eval("Recommend") %>' ></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CampaignId">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle CssClass="Hidden" />
                                            <HeaderStyle CssClass="Hidden" />
                                            <ItemStyle CssClass="Hidden" />
                                            <FooterStyle CssClass="Hidden" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ProductGroupId">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle CssClass="Hidden" />
                                            <HeaderStyle CssClass="Hidden" />
                                            <ItemStyle CssClass="Hidden" />
                                            <FooterStyle CssClass="Hidden" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ProductId">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle CssClass="Hidden" />
                                            <HeaderStyle CssClass="Hidden" />
                                            <ItemStyle CssClass="Hidden" />
                                            <FooterStyle CssClass="Hidden" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td > 
                                <asp:Button id="btnSelectCampaign" runat="server" Text="เลือก" Width="100px" OnClientClick="DisplayProcessing()"
                                    onclick="btnSelectCampaign_Click"  />
                                <asp:Button id="btnClose" runat="server" Text="ปิดหน้าต่าง" Width="100px" 
                                    onclick="btnClose_Click" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSearchCampaign" runat="server" TargetControlID="btnPopupSearchCampaign" PopupControlID="pnPopupSearchCampaign" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="upPopupSaveResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSaveResult" Width="0px" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSaveResult" style="display:none" CssClass="modalPopupCreateLeadResultList" ScrollBars="Auto">
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <b style="font-size:14px;">บันทึกข้อมูลผู้มุ่งหวังสำเร็จ</b>
            <br /><br />
            <asp:Panel ID="pnInner" runat="server" CssClass="modalPopupCreateLeadResultListInner" ScrollBars="Auto" BorderStyle="None">
                <asp:Repeater ID="rptPopupSaveResult" runat="server" >
                    <HeaderTemplate>
                        <table cellpadding="2" cellspacing="0" border="0">
                    </HeaderTemplate>
                    <ItemTemplate>
                         <tr>
                            <td style="width:40px;"></td>
                            <td style="width:380px;">
                                <b>Ticket Id:</b>&nbsp;<asp:Label ID="lblResultTicketId" runat="server" Text='<%# Eval("TicketId") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:40px;"></td>
                            <td>
                                <b>แคมเปญ:</b>&nbsp;<asp:Label ID="lblResultCampaign" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:40px;"></td>
                            <td>
                                <b>ช่องทาง:</b>&nbsp;<asp:Label ID="lblResultChannel" runat="server" Text='<%# Eval("ChannelDesc") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:40px;"></td>
                            <td>
                                <b>Owner Lead:</b>&nbsp;<asp:Label ID="lblResultOwnerLead" runat="server" Text='<%# Eval("OwnerName") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr><td colspan="2" style="height:15px;"></td></tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
            <center><asp:Button ID="btnPopupSaveResultOK" runat="server" Text="OK" CssClass="Button"  Width="100px" OnClick="btnPopupSaveResultOK_Click" OnClientClick="DisplayProcessing();" /></center>
            <br style="height:10px;" />
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSaveResult" runat="server" TargetControlID="btnPopupSaveResult" PopupControlID="pnPopupSaveResult" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:Button runat="server" ID="btnEditReceiptList" Width="0px" CssClass="Hidden"/>
<asp:Panel runat="server" ID="pnEditReceiptList" style="display:none" CssClass="modalPopupEditReceiptList" ScrollBars="Auto">
    <asp:UpdatePanel ID="upEditReceiptList" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <br />
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td style="width:15px; height:20px;"></td>
                    <td>
                        <b>Ticket Id:</b> <asp:Label ID="lblTicketIdPopupReceipt" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:15px;"></td>
                    <td>
                        <asp:GridView ID="gvEditReceiptList" runat="server" AutoGenerateColumns="False" Width="430px" 
                            GridLines="Horizontal" BorderWidth="1px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                            <Columns>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="40px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="เลขที่ใบเสร็จ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRecNo" runat="server" Text='<%#Eval("RecNo") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="จำนวนเงิน">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRecAmount" runat="server" Text='<%# Eval("RecAmount") != null ? (Convert.ToDecimal(Eval("RecAmount")).ToString("#,##0.00")) : "" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="130px" HorizontalAlign="Right" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่รับชำระ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTransDate" runat="server" Text='<%# Eval("TransDate") != null ? (Convert.ToDateTime(Eval("TransDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("TransDate")).Year.ToString()) : "" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="TicketId">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("TicketId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                        </asp:GridView>
                    </td>
                </tr>
                <tr style="height:10px;">
                    <td></td>
                    <td align="right">
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnPrintReceipt" runat="server" Text="พิมพ์" Width="80px" CssClass="Button" OnClick="btnPrintReceipt_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                        <asp:Button ID="btnClosePopupReceipt" runat="server" Text="ปิด" Width="80px" CssClass="Button" OnClick="btnClosePopupReceipt_Click" />
                    </td>
                </tr>
            </table>         
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
<act:ModalPopupExtender ID="mpeEditReceiptList" runat="server" TargetControlID="btnEditReceiptList" PopupControlID="pnEditReceiptList" BackgroundCssClass="modalBackground" DropShadow="True">
</act:ModalPopupExtender>

    
    <!-- ตั้งแต่บรรทัดนี้ ซ่อนไว้ ไม่ได้ใช้งาน -->
    <%--<asp:UpdatePanel  ID="upCampaignPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate >
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupCampaignSCR004">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/ProductAll.gif" />
                <table cellpadding="2" cellspacing="0" border="0">
                     <tr>
                        <td style="padding-left:12px;">
                            <asp:Panel ID="Panel2" runat="server" ScrollBars="Auto" ><br />
                                <uc7:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="800px" />
                                <asp:GridView ID="gvSearchCampaign" runat="server" AutoGenerateColumns ="false" 
                                    GridLines="Horizontal" BorderWidth="1px" 
                                    onrowdatabound="gvSearchCampaign_RowDataBound"  >
                                        <Columns >
                                            <asp:TemplateField >
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                                </ItemTemplate>
                                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                                                <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                                            </asp:TemplateField>
                                           <asp:BoundField DataField="CampaignId" HeaderText="รหัส Product/Campaign" >
                                               <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="150px" HorizontalAlign="Left"  />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="CampaignName" HeaderText="ชื่อ Product/Campaign"  >
                                                <HeaderStyle Width="250px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="250px" HorizontalAlign="Left"  />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="รายละเอียด Product/Campaign" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCampaignDetail" runat="server" ToolTip='<%# Bind("CampaignDetail") %>' Text='<%# Bind("CampaignDetail") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="350px" HorizontalAlign="Left" />
                                                <HeaderStyle Width="350px" HorizontalAlign="Center"/>
                                            </asp:TemplateField>

                                        </Columns>
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:12px; height:20px">
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:12px;">
                            <asp:Button ID="btnOK" runat="server" Text="เลือก" CssClass="Button" Width="90px" OnClick="btnOK_Click" OnClientClick="if(confirm('ต้องการบักทึกแนะนำ Product/Campaign ใช่หรือไม่?')){DisplayProcessing(); return true;} else{return false;}" />&nbsp;&nbsp;
                            <asp:Button ID="btnCancelCampaign" runat="server" Text="ยกเลิก" CssClass="Button" Width="90px"  />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>--%>

    <%--<asp:UpdatePanel  ID="upOfferCampaign" runat="server" UpdateMode="Conditional">
        <ContentTemplate >
            <asp:Button runat="server" ID="btnofferCampaigntest" Width="0px" CssClass="Hidden"/>
            <asp:Panel runat="server" ID="pnlOfferCampaign" style="display:none" CssClass="modalPopupOfferCampaignSCR004">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/ProductOffer.gif" />
                <table cellpadding="2" cellspacing="0" border="0">
                     <tr>
                        <td style="padding-left:12px;">
                            <asp:Panel ID="Panel3" runat="server" ScrollBars="Auto" ><br />
                                <asp:GridView ID="gvOfferCampaign" runat="server" AutoGenerateColumns="False" 
                                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                                    onrowdatabound="gvOfferCampaign_RowDataBound"  >
                                    <Columns>
                                        <asp:TemplateField HeaderText="สนใจ">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CampaignCode" HeaderText="รหัส Product/Campaign"  >
                                            <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="180px" HorizontalAlign="Center"  />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CampaignName" HeaderText="ชื่อ Product/Campaign"  >
                                            <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="200px" HorizontalAlign="Left"  />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="รายละเอียด Product/Campaign">
                                            <ItemTemplate>
                                                <asp:Label ID="lbDetail" runat="server" ToolTip='<%#Bind("CampaignDetail") %>'  Text='<%#Bind("CampaignDetail")  %>' Width="240px" ></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="250px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="250px" HorizontalAlign="Left"  />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                            <ItemTemplate>
                                                <%# Eval("ExpireDate") != null ? Convert.ToDateTime(Eval("ExpireDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("ExpireDate")).Year.ToString() : ""%>
                                            </ItemTemplate>
                                            <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                                            <ItemStyle Width="180px" HorizontalAlign="Center"  />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ChannelName" HeaderText="ช่องทาง" >
                                            <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                            <ItemStyle Width="150px" HorizontalAlign="Left"  />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:12px; height:20px">
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left:12px;">
                            <asp:Button ID="btnSelectOfferCampaign" runat="server" Text="เลือก" CssClass="Button" Width="90px" OnClick="btnSelectOfferCampaign_Click" OnClientClick="if(confirm('ต้องการบักทึกแนะนำ Product/Campaign ใช่หรือไม่?')){DisplayProcessing(); return true;} else{return false;}" />&nbsp;&nbsp;
                            <asp:Button ID="btnCancelOfferCampaign" runat="server" Text="ยกเลิก" CssClass="Button" Width="90px"  />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpeOfferCampaignPopup" runat="server" TargetControlID="btnofferCampaigntest" PopupControlID="pnlOfferCampaign" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
