<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabInbound.ascx.cs" Inherits="SLM.Application.Shared.Obt.TabInbound" %>
<%@ Register src="../TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>

<script type="text/javascript">
    //On Page Load
    //Set Autocomplete dropdownlist

    Sys.Application.add_load(function () {

        $("#<%= cmbCampaign.ClientID %>").combobox({
            select: function (event, ui) {
                DoPostBack(this);
            },
            notfound: function (event) {
                <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
            },
            cleared: function () {
                <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
            }
        });

        $("#<%= cmbOwnerBranchSearch.ClientID %>").combobox({
            select: function (event, ui) {
                DoPostBack(this);
            },
            notfound: function (event) {
                <%= Page.ClientScript.GetPostBackEventReference(cmbOwnerBranchSearch, "-1") %>
            },
            cleared: function () {
                <%= Page.ClientScript.GetPostBackEventReference(cmbOwnerBranchSearch, "-1") %>
            }
        });

        $("#<%= cmbOwnerLeadSearch.ClientID %>").combobox();

        $("#<%= cmbDelegateBranchSearch.ClientID %>").combobox({
            select: function (event, ui) {
                DoPostBack(this);
            },
            notfound: function (event) {
                <%= Page.ClientScript.GetPostBackEventReference(cmbDelegateBranchSearch, "-1") %>
            },
            cleared: function () {
                <%= Page.ClientScript.GetPostBackEventReference(cmbDelegateBranchSearch, "-1") %>
            }
        });

        $("#<%= cmbDelegateLeadSearch.ClientID %>").combobox();

        $("#<%= cmbCreatebyBranchSearch.ClientID %>").combobox({
            select: function (event, ui) {
                DoPostBack(this);
            },
            notfound: function (event) {
                <%= Page.ClientScript.GetPostBackEventReference(cmbCreatebyBranchSearch, "-1") %>
            },
            cleared: function () {
                <%= Page.ClientScript.GetPostBackEventReference(cmbCreatebyBranchSearch, "-1") %>
            }
        });

        $("#<%= cmbCreatebySearch.ClientID %>").combobox();
    });

    function CheckAllWorkFlag() {
        var flag = document.getElementById('<%=cbWorkFlagAll.ClientID%>').checked;
        document.getElementById('<%=cbWaitCreditForm.ClientID%>').checked = flag;
        document.getElementById('<%=cbWait50Tawi.ClientID%>').checked = flag;
        document.getElementById('<%=cbWaitDriverLicense.ClientID%>').checked = flag;
        document.getElementById('<%=cbPolicyNo.ClientID%>').checked = flag;
        document.getElementById('<%=cbAct.ClientID%>').checked = flag;
        document.getElementById('<%=cbStopClaim.ClientID%>').checked = flag;
        document.getElementById('<%=cbStopClaim_Cancel.ClientID%>').checked = flag;
    }
    function CheckWorkFlagItem() {
        if (document.getElementById('<%=cbWaitCreditForm.ClientID%>').checked
            && document.getElementById('<%=cbWait50Tawi.ClientID%>').checked
            && document.getElementById('<%=cbWaitDriverLicense.ClientID%>').checked
            && document.getElementById('<%=cbPolicyNo.ClientID%>').checked
            && document.getElementById('<%=cbAct.ClientID%>').checked
            && document.getElementById('<%=cbStopClaim.ClientID%>').checked
            && document.getElementById('<%=cbStopClaim_Cancel.ClientID%>').checked)
            document.getElementById('<%=cbWorkFlagAll.ClientID%>').checked = true;
        else {
            document.getElementById('<%=cbWorkFlagAll.ClientID%>').checked = false;
        }
    }
</script>

<asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
        <table cellpadding="2" cellspacing="0" border="0">
            <tr><td colspan="4" style="height:2px;"></td></tr>
            <tr>
                <td class="ColInfo">
                    เลขที่สัญญา
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtContractNo" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    <asp:TextBox ID="txtEmpCode" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffTypeId" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffTypeDesc" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffId" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffBranchCode" runat="server" Visible="false"></asp:TextBox>
                </td>
                <td class="ColInfo">
                    Ticket ID
                </td>
                <td>
                     <asp:TextBox ID="txtTicketID" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ชื่อลูกค้า
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtFirstname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                </td>
                <td class="ColInfo">
                    นามสกุล
                </td>
                <td>
                    <asp:TextBox ID="txtLastname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ประเภทบุคคล
                </td>
                <td class="ColInput">
                    <asp:DropDownList ID="cmbCardType" runat="server" Width="203px" AutoPostBack="true"
                        CssClass="Dropdownlist" onselectedindexchanged="cmbCardType_SelectedIndexChanged" >
                    </asp:DropDownList>
                </td>
                <td class="ColInfo">
                    <asp:Label ID="lblCardNo" runat="server" Text="เลขที่บัตร"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ช่องทาง
                </td>
                <td class="ColInput">
                    <asp:DropDownList ID="cmbChannel" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="ColInfo">
                    แคมเปญ
                </td>
                <td>
                    <asp:DropDownList ID="cmbCampaign" runat="server" Width="203px" 
                    CssClass="Dropdownlist" 
                        onselectedindexchanged="cmbCampaign_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    
                    
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ทะเบียนรถยนต์
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtLicenseNo" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                </td>
                <td class="ColInfo">
                    Grade ลูกค้า
                </td>
                <td class="ColInput">
                    <asp:DropDownList ID="cmbGradeSearch" runat="server" Width="200px" CssClass="Dropdownlist" >
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    วันที่นัดโทรหาลูกค้าเริ่มต้น
                </td>
                <td>
                    <uc1:TextDateMask ID="tdmNextAppointmentFrom" runat="server" Width="182px" />
                </td>
                <td class="ColInfo">
                    วันที่นัดโทรหาลูกค้าสิ้นสุด
                </td>
                <td>
                    <uc1:TextDateMask ID="tdmNextAppointmentTo" runat="server" Width="182px" />
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    มีค่าเบี้ยปีต่อ
                </td>
                <td class="ColInput">
                    <asp:DropDownList ID="cmbHasNotifyPremium" runat="server" CssClass="Dropdownlist" Width="200px"/>
                </td>
                <td class="ColInfo">
                    &nbsp;
                </td>
                <td class="ColInput">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ค่าเบี้ยปีต่อเริ่มต้น
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtNotifyGrossPremiumMin" runat="server" CssClass="Textbox" Width="200px" MaxLength="13"/>
                    <asp:Label ID="vtxtNotifyGrossPremiumMin" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="ColInfo">
                    ค่าเบี้ยปีต่อสิ้นสุด
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtNotifyGrossPremiumMax" runat="server" CssClass="Textbox" Width="200px" MaxLength="13"/>
                    <asp:Label ID="vtxtNotifyGrossPremiumMax" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ปีที่คุ้มครอง
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtPolicyExpirationYear" runat="server" CssClass="Textbox" Width="200px" MaxLength="4"></asp:TextBox>
                </td>
                <td class="ColInfo">
                    เดือนที่คุ้มครอง
                </td>
                <td>
                    <asp:DropDownList ID="cmbPolicyExpirationMonth" runat="server" CssClass="Dropdownlist" Width="203px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ปีที่แจก Leads
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtPeriodYear" runat="server" CssClass="Textbox" Width="200px" MaxLength="4"></asp:TextBox>
                </td>
                <td class="ColInfo">
                    เดือนที่แจก Leads
                </td>
                <td>
                    <asp:DropDownList ID="cmbPeriodMonth" runat="server" CssClass="Dropdownlist" Width="203px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="height:8px; vertical-align:bottom;">
                </td>
                <td colspan="3"></td>
            </tr>
            <tr>
                <td class="ColInfo">
                    
                </td>
                <td colspan="3">
                    <asp:CheckBox ID="cbFollowUp" runat="server" Text="Follow Up" ForeColor="#f89797" Font-Bold="true" />&nbsp;&nbsp;
                    <asp:CheckBox ID="cbInbound" runat="server" Text="Inbound" ForeColor="Orange" Font-Bold="true" />&nbsp;&nbsp;
                    <asp:CheckBox ID="cbOutbound" runat="server" Text="Outbound" ForeColor="Green" Font-Bold="true" />&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td style="height:8px; vertical-align:bottom;">
                </td>
                <td colspan="3"></td>
            </tr>
        </table>
        <asp:LinkButton ID="lbAdvanceSearch" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()" 
            Text="[+] <b>Advance Search</b>"  onclick="lbAdvanceSearch_Click"></asp:LinkButton>
        <asp:TextBox ID="txtAdvanceSearch" runat="server" Text="N" Visible="false" ></asp:TextBox>
        <asp:Panel ID="pnAdvanceSearch" runat="server" style="display:none;" >
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="4" style="height:8px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        เลขที่สัญญาที่เคยมีกับธนาคาร
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtContractNoRefer" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        
                    </td>
                    <td>
                        
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        วันทีสร้าง Lead
                    </td>
                    <td class="ColInput">
                        <uc1:TextDateMask ID="tdmCreateDate" runat="server" Width="182px" />
                    </td>
                    <td class="ColInfo">
                        วันที่ได้รับมอบหมายล่าสุด
                    </td>
                    <td>
                        <uc1:TextDateMask ID="tdmAssignDate" runat="server" Width="182px" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Owner Branch
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbOwnerBranchSearch" runat="server" Width="203px"
                            CssClass="Dropdownlist" AutoPostBack="true"
                            onselectedindexchanged="cmbOwnerBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                            Owner Lead
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbOwnerLeadSearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Delegate Branch
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbDelegateBranchSearch" runat="server" Width="203px" 
                            CssClass="Dropdownlist" AutoPostBack="true"
                            onselectedindexchanged="cmbDelegateBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                            Delegate Lead
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbDelegateLeadSearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สาขาผู้สร้าง Lead
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCreatebyBranchSearch" runat="server" Width="203px" 
                            CssClass="Dropdownlist" AutoPostBack="true"
                            onselectedindexchanged="cmbCreatebyBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                            ผู้สร้าง Lead
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbCreatebySearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="height:3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Flag การทำงาน
                    </td>
                    <td colspan="3">
                        &nbsp;<asp:CheckBox ID="cbWorkFlagAll" runat="server" Text="ทั้งหมด" onchange="CheckAllWorkFlag()" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td colspan="3">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr style="height:32px;">
                                <td style="width:170px;">
                                    &nbsp;<asp:CheckBox id="cbWaitCreditForm" runat="server" Text="รอฟอร์มตัดบัตรเครดิต" onchange="CheckWorkFlagItem()" />
                                </td>
                                <td style="width:170px;">
                                    <asp:CheckBox id="cbWait50Tawi" runat="server" Text="รอฟอร์ม 50 ทวิ" onchange="CheckWorkFlagItem()" />
                                </td>
                                <td style="width:170px;">
                                    <asp:CheckBox id="cbWaitDriverLicense" runat="server" Text="รอสำเนาใบขับขี่" onchange="CheckWorkFlagItem()" />
                                </td>
                                <td style="width:170px;">
                                    &nbsp;<asp:CheckBox id="cbPolicyNo" runat="server" Text="ลูกค้าติดตามเลขกรมธรรม์" onchange="CheckWorkFlagItem()" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width:170px;">
                                    &nbsp;<asp:CheckBox id="cbAct" runat="server" Text="ลูกค้าติดตามเลข พรบ." onchange="CheckWorkFlagItem()" />
                                </td>
                                <td>
                                    <asp:CheckBox id="cbStopClaim" runat="server" Text="ระงับเคลม" onchange="CheckWorkFlagItem()" />
                                </td>
                                <td>
                                    <asp:CheckBox id="cbStopClaim_Cancel" runat="server" Text="ยกเลิกระงับเคลม" onchange="CheckWorkFlagItem()" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table><br />
            <asp:UpdatePanel ID="upRepeaterStatus" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellpadding="3" cellspacing="0" border="0" >
                        <tr>
                            <td valign="top" class="ColInfo">
                                    สถานะของ Lead
                            </td>
                            <td >
                                <asp:Repeater ID="rptStatus" runat="server" ClientIDMode="AutoID" >
                                    <HeaderTemplate>
                                        <table cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">
                                        <tr style="vertical-align:top; border-bottom:1px solid #f2f2f2; height:30px;" >
                                            <td >
                                                <asp:CheckBox ID="cbStatusAll" runat="server" Text="ทั้งหมด" AutoPostBack="true" OnCheckedChanged="cbStatusAll_CheckedChanged" />
                                            </td>
                                            <td></td>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="vertical-align:top; height:30px; border-bottom:1px solid #f2f2f2;">
                                            <td style="width:170px;">
                                                <asp:CheckBox id="cbStatus" runat="server" Text='<%#Eval("TextField") %>' AutoPostBack="true" OnCheckedChanged="cbStatus_CheckedChanged"  />
                                                <asp:HiddenField ID="hiddenStatusCode" runat="server" Value='<%#Eval("ValueField") %>' />
                                            </td>
                                            <td style="vertical-align:top;">
                                                <asp:CheckBoxList id="cblSubStatus" runat="server" RepeatLayout="Table"  RepeatDirection="Horizontal" RepeatColumns="5" ></asp:CheckBoxList>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel> 
    

<asp:UpdatePanel ID="upButton" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="3" cellspacing="0" border="0">
            <tr>
                <td colspan="2" style="height:3px"></td>
            </tr>
            <tr>
                <td class="ColInfo">
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                        Text="ค้นหา" onclick="btnSearch_Click" />&nbsp;
                    <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" 
                        Text="ล้างข้อมูล" onclick="btnClear_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<br />
<div class="Line"></div>
<br />
<asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
        <asp:Button ID="btnAddLead" runat="server" Text="เพิ่ม Lead" Width="120px" 
            CssClass="Button" Height="23px" onclick="btnAddLead_Click"  />
            <br /><br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="3040px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                AllowSorting="true" onsorting="gvResult_Sorting" Width="3040px" 
                onrowdatabound="gvResult_RowDataBound" ondatabound="gvResult_DataBound"  >
                <Columns>
                <asp:TemplateField HeaderText="SLA">
                    <ItemTemplate>
                        <asp:image ID="imgSla" runat="server" ImageUrl="~/Images/invalid.gif" Visible='<%# Eval("Counting") != null ? (Convert.ToInt32(Eval("Counting")) > 0 ? true : false) : false %>' />
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="30px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        &nbsp;<asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbView_Click" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง" OnClientClick="DisplayProcessing();" />
                        <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลผู้มุ่งหวัง" OnClientClick="DisplayProcessing();" />
                    </ItemTemplate>
                    <ItemStyle Width="50px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Notice">
                    <ItemTemplate>
                        <asp:image ID="imgNotify" runat="server" ImageUrl="~/Images/exclamation.jpg" Visible='<%# Eval("NoteFlag") != null ? (Eval("NoteFlag").ToString() == "1" ? true : false) : false %>' />
                    </ItemTemplate>
                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cal">
                    <ItemTemplate>
                        <asp:ImageButton ID="imbCal" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Calculator.png" ToolTip="Calculator" />
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="30px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Doc">
                    <ItemTemplate>
                        <asp:ImageButton ID="imbDoc" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Document.png" ToolTip="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("TicketId") %>' OnClientClick="DisplayProcessing()" />
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="30px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Others">
                    <ItemTemplate>
                        <asp:ImageButton ID="imbOthers" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Others.png" OnClick="lbAolSummaryReport_Click" CommandArgument='<%# Container.DisplayIndex %>' ToolTip="เรียกดูข้อมูลเพิ่มเติม" OnClientClick="DisplayProcessing()" />
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="30px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField SortExpression="NextContactDate">
                    <HeaderTemplate>
                        <asp:LinkButton ID="lbNextContactDateHeader" runat="server" Text="วันที่นัด<br/>โทรหาลูกค้า" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="NextContactDate"></asp:LinkButton>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# Eval("NextContactDate") != null ? Convert.ToDateTime(Eval("NextContactDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("NextContactDate")).Year.ToString() : "" %>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center" />
                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="ปีที่คุ้มครอง" >
                    <ItemTemplate>
                        <asp:Label ID="lblPolicyExpirationYear" runat="server" Text='<%# Eval("PolicyExpirationYear") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                    <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="เดือนที่คุ้มครอง">
                    <ItemTemplate>
                        <asp:Label ID="lblPolicyExpirationMonth" runat="server" Text='<%# Eval("PolicyExpirationMonth") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="เลขที่สัญญา">
                    <ItemTemplate>
                        <asp:Label ID="lblContractNo" runat="server" Text='<%# Eval("ContractNo") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center"  />
                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ticket ID">
                    <ItemTemplate>
                        <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("TicketId") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Grade">
                    <ItemTemplate>
                        <asp:Label ID="lblGrade" runat="server" Text='<%# Eval("Grade") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                    <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ชื่อ">
                    <ItemTemplate>
                        <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="นามสกุล">
                    <ItemTemplate>
                        <asp:Label ID="lblLastname" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField SortExpression="StatusDesc">
                    <HeaderTemplate>
                        <asp:LinkButton ID="lbStatusDescHeader" runat="server" Text="สถานะของ Lead" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="StatusDesc"></asp:LinkButton>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Eval("StatusDesc") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="สถานะย่อยของ Lead">
                    <ItemTemplate>
                        <asp:Label ID="lblSubStatusDesc" runat="server" ></asp:Label>

                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="วันที่หมดอายุประกัน">
                    <ItemTemplate>
                        <%# Eval("PolicyExpireDate") != null ? Convert.ToDateTime(Eval("PolicyExpireDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("PolicyExpireDate")).Year.ToString() : ""%>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="แจ้งเตือนครั้งที่">
                    <ItemTemplate>
                        <asp:Label ID="lblCounting" runat="server" Text='<%# Eval("Counting") != null ? Convert.ToDecimal(Eval("Counting")).ToString("#,##0") : "0" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="วันเวลา NextSLA">
                    <ItemTemplate>
                        <%# Eval("NextSLA") != null ? Convert.ToDateTime(Eval("NextSLA")).ToString("dd/MM/") + Convert.ToDateTime(Eval("NextSLA")).Year.ToString() + " " + Convert.ToDateTime(Eval("NextSLA")).ToString("HH:mm:ss") : "" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภทบุคคล">
                    <ItemTemplate>
                        <asp:Label ID="lblCardTypeName" runat="server" Text='<%# Eval("CardTypeName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="เลขที่บัตร">
                    <ItemTemplate>
                        <asp:Label ID="lblCitizenId" runat="server" Text='<%# Eval("CitizenId") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="110px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField SortExpression="CampaignName">
                    <HeaderTemplate>
                        <asp:LinkButton ID="lbCampaignNameHeader" runat="server" Text="แคมเปญ" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="CampaignName"></asp:LinkButton>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="DelegateName" HeaderText="Delegate Lead">
                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="CreaterName" HeaderText="ผู้สร้าง Lead">
                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="วันที่สร้าง Lead">
                    <ItemTemplate>
                        <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField SortExpression="AssignedDate">
                    <HeaderTemplate>
                        <asp:LinkButton ID="lbAssignedDateHeader" runat="server" Text="วันที่ได้รับมอบหมายล่าสุด" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="AssignedDate"></asp:LinkButton>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# Eval("AssignedDate") != null ? Convert.ToDateTime(Eval("AssignedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("AssignedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("AssignedDate")).ToString("HH:mm:ss") : "" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"/>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="DelegateDate">
                    <HeaderTemplate>
                        <asp:LinkButton ID="lbDelegateDateHeader" runat="server" Text="วันที่ได้รับ Delegate" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="DelegateDate"></asp:LinkButton>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# Eval("DelegateDate") != null ? Convert.ToDateTime(Eval("DelegateDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("DelegateDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("DelegateDate")).ToString("HH:mm:ss") : "" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ปีที่แจก<br/>Leads" >
                    <ItemTemplate>
                        <asp:Label ID="lblPeriodYear" runat="server" Text='<%# Eval("PeriodYear") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                    <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="เดือนที่แจก<br/>Leads">
                    <ItemTemplate>
                        <asp:Label ID="lblPeriodMonth" runat="server" Text='<%# Eval("PeriodMonth") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:BoundField DataField="OwnerBranchName" HeaderText="Owner Branch">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                    <asp:BoundField DataField="DelegateBranchName" HeaderText="Delegate Branch">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                    <asp:BoundField DataField="CreaterBranchName" HeaderText="สาขาผู้สร้าง Lead">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                 <asp:BoundField DataField="ContractNoRefer" HeaderText="เลขที่สัญญา<br/>ที่เคยมีกับธนาคาร" HtmlEncode="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center"  />
                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="CalculatorUrl">
                    <ItemTemplate>
                        <asp:Label ID="lblCalculatorUrl" runat="server" Text='<%# Eval("CalculatorUrl") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="HasAdamUrl">
                    <ItemTemplate>
                        <asp:Label ID="lblHasAdamUrl" runat="server" Text='<%# Convert.ToBoolean(Eval("HasAdamUrl")) ? "Y" : "N" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="AppNo">
                    <ItemTemplate>
                        <asp:Label ID="lblAppNo" runat="server" Text='<%# Eval("AppNo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ProductId">
                    <ItemTemplate>
                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="IS COC">
                    <ItemTemplate>
                        <asp:Label ID="lblIsCOC" runat="server" Text='<%# Eval("ISCOC") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="COCCurrentTeam">
                    <ItemTemplate>
                        <asp:Label ID="lblCOCCurrentTeam" runat="server" Text='<%# Eval("COCCurrentTeam") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="StatusCode">
                    <ItemTemplate>
                        <asp:Label ID="lbslmStatusCode" runat="server" Text='<%# Eval("StatusCode") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SubStatusCode">
                    <ItemTemplate>
                        <asp:Label ID="lblSubStatusCode" runat="server" Text='<%# Eval("SubStatusCode") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PreleadId">
                    <ItemTemplate>
                        <asp:Label ID="lblPreleadId" runat="server" Text='<%# Eval("PreleadId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CampaignId">
                    <ItemTemplate>
                        <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="LicenseNo">
                    <ItemTemplate>
                        <asp:Label ID="lblLicenseNo" runat="server" Text='<%# Eval("LicenseNo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ResultFlag">
                    <ItemTemplate>
                        <asp:Label ID="lblResultFlag" runat="server" Text='<%# Eval("ResultFlag") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>

                <%--<asp:TemplateField HeaderText="ProductName">
                    <ItemTemplate>
                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                
                
                <asp:TemplateField HeaderText="LicenseNo">
                    <ItemTemplate>
                        <asp:Label ID="lblLicenseNo" runat="server" Text='<%# Eval("LicenseNo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="TelNo1">
                    <ItemTemplate>
                        <asp:Label ID="lblTelNo1" runat="server" Text='<%# Eval("TelNo1") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ProvinceRegis">
                    <ItemTemplate>
                        <asp:Label ID="lblProvinceRegis" runat="server" Text='<%# Eval("ProvinceRegis") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="ProductGroupId">
                    <ItemTemplate>
                        <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>--%>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
