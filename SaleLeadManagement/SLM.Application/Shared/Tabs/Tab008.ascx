<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab008.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab008" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="../TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>

<style type="text/css">
    .ColIndent
    {
        width:15px;
    }
    .ColIndent2
    {
        width:36px;
    }
    .ColInfo1
    {
        font-weight:bold;
        width:150px;
    }
    .ColInfo2
    {
        font-weight:bold;
        width:190px;
    }
    .ColInput
    {
        width:240px;
    }
    .MsgAlert
    {
        font-family: Tahoma;
        font-size: 9pt;
        color: Red;
    }
</style>
<div style="font-family:Tahoma; font-size:13px;">
    <script language="javascript" type="text/javascript">
        function ConfirmContactDetailSave() {
            var detail = document.getElementById('<%= txtContactDetail.ClientID %>').value;
            if (detail.trim() != '') {
                return confirm('ต้องการบันทึกใช่หรือไม่');
            }
        }
        function TelNoSmsChange() {
            var textboxSms = document.getElementById('<%= txtTelNoSms.ClientID %>');
            var alertSms = document.getElementById('<%= vTelNoSms.ClientID %>');
            if (textboxSms.value.trim() == '') {
                textboxSms.value = document.getElementById('<%= txtTelNo1.ClientID %>').value;
                alertSms.innerHTML = '';
            }
        }
    </script>
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="left" style="width:1100px;">
                          
                        <asp:Button ID="btnAddResultContact" runat="server" CssClass="Button" OnClientClick="DisplayProcessing();"
                            Text="บันทึกผลการติดต่อ" Width="140px" onclick="btnAddResultContact_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:CheckBox ID="cbThisLead" runat="server" Text="แสดงเฉพาะข้อมูลผู้มุ่งหวังนี้" Checked="true" AutoPostBack="true" onchange="DisplayProcessing()"   
                            oncheckedchanged="chkthis_CheckedChanged" /> 
                        <asp:TextBox ID="txtPreleadIdSearch" runat="server"  Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtTicketIdSearch" runat="server"  Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtCitizenIdSearch" runat="server"  Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtTelNo1Search" runat="server"  Width="40px"></asp:TextBox>
                        <asp:TextBox ID="txtLoginName" runat="server" Width="40px"></asp:TextBox>
                    </td>
                    <td>
                        <%--<asp:Button ID="btnAddContractPrelead" runat="server" CssClass="Button" OnClientClick="DisplayProcessing();"
                            Text="บันทึกผลการติดต่อ" Width="140px" onclick="btnAddContractPrelead_Click" />
                        <asp:TextBox ID="txtPreleadIdSearch" runat="server" Width="50px"></asp:TextBox>--%>

                        <asp:LinkButton ID="lbCasAllActivity" runat="server" Text="All Activity" Font-Underline="true" ></asp:LinkButton>
                    </td>
                </tr>
            </table>
            <br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1173px" />
            <asp:Panel id="pnlPhoneCallHistoty" runat="server" CssClass="PanelPhoneCallHistoty" ScrollBars="Auto">
                <asp:GridView ID="gvPhoneCallHistoty" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId" Width="1570px"
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" >
                    <Columns>
                    <asp:TemplateField HeaderText="วันที่บันทึกข้อมูล">
                        <ItemTemplate>
                            <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TicketId" HeaderText="Ticket ID"  >
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ"  >
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="Firstname" HeaderText="ชื่อ" >
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="Lastname" HeaderText="นามสกุล" >
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead">
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContactPhone" HeaderText="หมายเลขโทรศัพท์<br />ที่ติดต่อลูกค้า" HtmlEncode="false">
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContactDetail" HeaderText="รายละเอียดการติดต่อ">
                        <HeaderStyle Width="170px" HorizontalAlign="Center"/>
                        <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreatedName" HeaderText="ผู้บันทึก">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="ประเภทบุคคล">
                        <ItemTemplate>
                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="110px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <%--<asp:TemplateField HeaderText="50 ทวิ">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbDownload50Tawi" runat="server" Width="27px" CommandArgument='<%# Eval("Tawi50FilePath") %>'  OnClick="imbDownload50Tawi_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("Tawi50FilePath") != null ? (Eval("Tawi50FilePath").ToString() != "" ? true : false) : false %>' ToolTip="Download" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="บัตรเครดิต">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbDownloadCredit" runat="server" Width="27px" CommandArgument='<%# Eval("CreditFilePath") %>' OnClick="imbDownloadCredit_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("CreditFilePath") != null ? (Eval("CreditFilePath").ToString() != "" ? true : false) : false %>' ToolTip="Download" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ใบขับขี่">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbDownloadDriverLicense" runat="server" Width="27px" CommandArgument='<%# Eval("DriverLicenseFilePath") %>' OnClick="imbDownloadDriverLicense_Click" ImageUrl="~/Images/doc_pic.png" Visible='<%# Eval("DriverLicenseFilePath") != null ? (Eval("DriverLicenseFilePath").ToString() != "" ? true : false) : false %>' ToolTip="Download" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>--%>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView><br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupTab008">
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hFollow.gif" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblTab008Info" runat="server" ForeColor="Red" ></asp:Label>
                    <asp:UpdatePanel ID="upSection1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr><td colspan="5" style="height:1px;"></td></tr>
                                <tr>
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        Ticket ID
                                    </td>
                                    <td class="ColInput">
                                        <asp:TextBox ID="txtTicketID" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAssignedFlag" runat="server" Width="40px" ></asp:TextBox>
                                        <asp:TextBox ID="txtDelegateFlag" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        ชื่อ
                                    </td>
                                    <td class="ColInput">
                                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInfo2">
                                        นามสกุล
                                    </td>
                                    <td class="ColInput">
                                        <asp:TextBox ID="txtLastname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        ประเภทบุคคล
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbCardType" runat="server" CssClass="Dropdownlist" Width="203px" AutoPostBack="true" onselectedindexchanged="cmbCardType_SelectedIndexChanged"  >
                                        </asp:DropDownList>
                                        <br />
                                        <asp:Label ID="vtxtCardType" runat="server" CssClass="MsgAlert"></asp:Label>
                                    </td>
                                    <td class="ColInfo2">
                                        เลขที่บัตร<asp:Label ID="lblCitizenId" runat="server" ForeColor="Red"></asp:Label>
                                    </td>
                                    <td class="ColInput">
                                        <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="200px" Enabled="false" AutoPostBack="true" OnTextChanged="txtCitizenId_TextChanged" ></asp:TextBox>
                                        <br />
                                        <asp:Label ID="vtxtCitizenId" runat="server" CssClass="MsgAlert"></asp:Label>
                                    </td>
                                </tr>
                                    <tr>
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        แคมเปญ<asp:TextBox ID="txtCampaignId" runat="server" Width="40px" ></asp:TextBox>
                                        <asp:TextBox ID="txtProductGroupId" runat="server" Width="40px" ></asp:TextBox>
                                        <asp:TextBox ID="txtProductId" runat="server" Width="40px" ></asp:TextBox>
                                        <asp:TextBox ID="txtProductGroupName" runat="server" Width="40px" ></asp:TextBox>
                                        <asp:TextBox ID="txtProductName" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInput">
                                        <asp:TextBox ID="txtCampaign" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                        <asp:TextBox ID="txtChannelId" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInfo2">
                                        ประเทศ<asp:Label ID="lblCountryId" runat="server" ForeColor="Red"></asp:Label>
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbCountry" runat="server" Width="200px"  CssClass="Dropdownlist" >
                                        </asp:DropDownList>
                                        <br />
                                        <asp:Label ID="vcmbCountry" runat="server" CssClass="MsgAlert"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upDropdownlist" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr style="vertical-align:top;">
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        Owner Branch<asp:TextBox ID="txtOldOwnerBranch" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="203px"
                                            CssClass="Dropdownlist" AutoPostBack="True" 
                                            onselectedindexchanged="cmbOwnerBranch_SelectedIndexChanged" ></asp:DropDownList>
                                            <br />
                                        <asp:Label ID="vcmbOwnerBranch" runat="server" CssClass="MsgAlert" ></asp:Label>
                                    </td>
                                    <td class="ColInfo2">
                                        Owner Lead<asp:TextBox ID="txtOldOwner" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbOwner" runat="server" CssClass="Dropdownlist"  Width="203px" AutoPostBack ="true"  onselectedindexchanged="cmbOwner_SelectedIndexChanged" ></asp:DropDownList>
                                        <br />
                                        <asp:Label ID="vcmbOwner" runat="server" CssClass="MsgAlert" ></asp:Label>
                                    </td>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td class="ColIndent2"></td>
                                    <td class="ColInfo1">
                                        Delegate Branch<asp:TextBox ID="txtOldDelegateBranch" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInput">
                                            <asp:DropDownList ID="cmbDelegateBranch" runat="server" Width="203px" CssClass="Dropdownlist" AutoPostBack="True" 
                                                onselectedindexchanged="cmbDelegateBranch_SelectedIndexChanged"></asp:DropDownList>
                                            <br />
                                            <asp:Label ID="vcmbDelegateBranch" runat="server" CssClass="MsgAlert" ></asp:Label>
                                    </td>
                                    <td class="ColInfo2">
                                        Delegate lead<asp:TextBox ID="txtOldDelegate" runat="server" Width="40px" ></asp:TextBox>
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbDelegate" runat="server" CssClass="Dropdownlist"  Width="203px" AutoPostBack="True" 
                                            onselectedindexchanged="cmbDelegateLead_SelectedIndexChanged"  ></asp:DropDownList> 
                                        <br />
                                        <asp:Label ID="vcmbDelegate" runat="server" CssClass="MsgAlert" ></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr style="vertical-align:top;">
                            <td class="ColIndent2"></td>
                            <td class="ColInfo1">
                                หมายเลขโทรศัพท์
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtTelNo1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                <asp:TextBox ID="txtExt1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="46px" Visible="false" ></asp:TextBox>
                            </td>
                            <td class="ColInfo2">
                                หมายเลข SMS
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtTelNoSms" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" onblur="TelNoSmsChange()"></asp:TextBox>
                                <br />
                                <asp:Label ID="vTelNoSms" runat="server" CssClass="MsgAlert" ></asp:Label>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td class="ColIndent2"></td>
                            <td class="ColInfo1">
                                สถานะของ Lead<span style="color:Red">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbLeadStatus" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                                <asp:TextBox ID="txtOldStatus" runat="server" Width="20px" ></asp:TextBox>
                                <br />
                                <asp:Label ID="vcmbLeadStatus" runat="server" CssClass="MsgAlert" ></asp:Label>
                            </td>
                            <td class="ColInfo2">
                                หมายเลขโทรศัพท์ที่ติดต่อลูกค้า
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtContactPhone" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="ColIndent2"></td>
                            <td class="ColInfo1" valign="top">
                                รายละเอียดเพิ่มเติม<span style="color:Red">*</span>
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="txtContactDetail" runat="server" CssClass="Textbox" TextMode="MultiLine" Rows="5"  Width="638px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="height:35px;">
                            <td class="ColIndent2"></td>
                            <td class="ColInfo1" valign="top">
                            </td>
                            <td colspan="2" style="vertical-align:top;">
                                <asp:Label ID="vtxtContactDetail" runat="server" ForeColor="Red"></asp:Label>
                                <%--<asp:RequiredFieldValidator ID="ContactDetailRequired" runat="server" ForeColor="Red"
                                    ControlToValidate="txtContactDetail" ErrorMessage="กรุณากรอกข้อมูรายละเอียดก่อนทำการบันทึก" 
                                    ToolTip="กรุณากรอกข้อมูลรายละเอียดก่อนทำการบันทึก" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                <div id="lblMsg1" style="color:Red;"></div>--%>
                            </td>
                            <td class="ColInput">
                                <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="98px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" Width="98px" OnClick="btnCancel_Click" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่?')" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>
	            </asp:Panel>
	            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Prelead Section move to TabActRenewInsure.ascx -->
    <%--<asp:Button runat="server" ID="btnPreleadPopup" Width="0px" CssClass="Hidden"/>
	<asp:Panel runat="server" ID="pnPreleadPopup" style="display:none" CssClass="modalPopupTab008_Obt">
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hFollow.gif" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:UpdatePanel ID="upPreleadPopup" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="5" style="height:1px;"></td></tr>
                    <tr>
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            Contract No
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtContractNo_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo2">
                            Ticket ID
                        </td>
                        <td>
                            <asp:TextBox ID="txtTicketId_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            ชื่อ
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtFirstname_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo2">
                            นามสกุล
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtLastname_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            ประเภทบุคคล
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbCardType_PreleadPopup" runat="server" CssClass="Dropdownlist" Width="203px" AutoPostBack="true" onselectedindexchanged="cmbCardType_PreleadPopup_SelectedIndexChanged"  >
                            </asp:DropDownList>
                            <br />
                            <asp:Label ID="vtxtCardType_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                        </td>
                        <td class="ColInfo2">
                            เลขที่บัตร<asp:Label ID="lblCitizenId_PreleadPopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtCitizenId_PreleadPopup" runat="server" CssClass="Textbox" Width="200px" Enabled="false" AutoPostBack="true" OnTextChanged="txtCitizenId_PreleadPopup_TextChanged" ></asp:TextBox>
                            <br />
                            <asp:Label ID="vtxtCitizenId_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                        </td>
                    </tr>
                        <tr>
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            แคมเปญ
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtCampaignName_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo2">
                        </td>
                        <td class="ColInput">
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            Owner Branch
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtOwnerBranchName_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo2">
                            Owner
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtOwnerName_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            หมายเลขโทรศัพท์<span style="color:Red">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtTelNo1_PreleadPopup" runat="server" CssClass="Textbox" MaxLength="10" Width="200px" AutoPostBack="true" OnTextChanged="txtTelNo1_PreleadPopup_TextChanged" ></asp:TextBox>
                            <br />
                            <asp:Label ID="vtxtTelNo1_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                        </td>
                        <td class="ColInfo2">
                        </td>
                        <td class="ColInput">
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            สถานะหลัก
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtStatusDesc_PreleadPopup" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                            <asp:TextBox ID="txtStatusCode_PreleadPopup" runat="server" Width="40px"></asp:TextBox>
                        </td>
                        <td class="ColInfo2">
                            สถานะย่อย<span style="color:Red">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbSubStatus_PreleadPopup" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                            <asp:TextBox ID="txtSubStatusCodeCurrent_PreleadPopup" runat="server" Width="50px"></asp:TextBox>
                            <asp:TextBox ID="txtSubStatusNameCurrent_PreleadPopup" runat="server" Width="50px"></asp:TextBox>
                            <br />
                            <asp:Label ID="vcmbSubStatus_PreleadPopup" runat="server" CssClass="MsgAlert"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1">
                            วันที่นัดหมายครั้งถัดไป
                        </td>
                        <td class="ColInput">
                            <uc2:TextDateMask ID="tdmNextContactDate_PreleadPopup" runat="server" />
                        </td>
                        <td class="ColInfo2">
                            หมายเลขโทรศัพท์ที่ติดต่อลูกค้า
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtContactPhone_PreleadPopup" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1" valign="top">
                            รายละเอียดเพิ่มเติม<span style="color:Red">*</span>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtContactDetail_PreleadPopup" runat="server" CssClass="Textbox" TextMode="MultiLine" Rows="5"  Width="638px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="height:35px;">
                        <td class="ColIndent2"></td>
                        <td class="ColInfo1" valign="top">
                        </td>
                        <td colspan="2" style="vertical-align:top;">
                            <asp:Label ID="vtxtContactDetail_PreleadPopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="ColInput">
                            <asp:Button ID="btnSave_PreleadPopup" runat="server" Text="บันทึก" Width="98px" OnClick="btnSave_PreleadPopup_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                            <asp:Button ID="btnCancel_PreleadPopup" runat="server" Text="ยกเลิก" Width="98px" OnClick="btnCancel_PreleadPopup_Click" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่?')" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <act:ModalPopupExtender ID="mpePreleadPopup" runat="server" TargetControlID="btnPreleadPopup" PopupControlID="pnPreleadPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	</act:ModalPopupExtender>--%>
</div>