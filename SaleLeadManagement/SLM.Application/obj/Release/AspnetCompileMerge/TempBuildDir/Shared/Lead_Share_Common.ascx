<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Lead_Share_Common.ascx.cs" Inherits="SLM.Application.Shared.Lead_Share_Common" %>
<%@ Register src="TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<%@ Register src="GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>
&nbsp;
<asp:UpdatePanel runat="server" ID="updCommon" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField runat="server" ID="hdfTicketID" />
        <asp:Label runat="server" ID="lblAlert" ForeColor="Red" ></asp:Label>
       <table cellpadding="2" cellspacing="0" border="0" >
            <tr>
                <td colspan="5">
                    <asp:Image ID="imgHeader1" runat="server" ImageUrl="~/Images/hGeneral.gif" />
                </td>
            </tr>
            <tr runat="server" id="trInfo1">
                <td class="style2"><asp:Label  id="lbTicketID" runat="server" Text="Ticket ID"></asp:Label> </td>
                <td class="style3">
                    <asp:TextBox ID="txtslm_TicketId" runat="server" CssClass="Textbox" Width="250px" ReadOnly="true"></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">สถานะของ Lead</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbStatus" runat="server" CssClass="Textbox" Width="253px" MaxLength="120" ></asp:DropDownList>
                    <asp:TextBox ID="txtOldStatus" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtAssignFlag" runat="server" CssClass="Hidden" ></asp:TextBox>
                    <asp:TextBox ID="txtDelegateFlag" runat="server" CssClass="Hidden" ></asp:TextBox>
                </td>
            </tr>
           <tr>
                <td class="style2">คำนำหน้าชื่อ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbTitle" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3">
                </td>
            </tr>
            <tr>
                <td class="style2">ชื่อ <span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                    <asp:Label ID="vtxtName" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">นามสกุล</td>
                <td class="style3">
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="Textbox" Width="250px" MaxLength="120" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ช่องทาง <span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbChannelId" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="true" 
                        OnSelectedIndexChanged="cmbChannelId_SelectedIndexChanged" ></asp:DropDownList>
                     <asp:Label ID="vcmbChannelId" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1">&nbsp;</td>
                <td class="style2">หมายเลขโทรศัพท์ 1(มือถือ)<span class="style4">*</span></td>
                <td class="style3" >
                    <asp:TextBox ID="txtTelNo_1" runat="server" CssClass="Textbox" MaxLength="10"  
                    Width="250px" ></asp:TextBox>
                <asp:Label ID="vtxtTelNo_1" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">แคมเปญ <span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCampaignId" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        OnSelectedIndexChanged="cmbCampaignId_SelectedIndexChanged"  ></asp:DropDownList>
                    <asp:ImageButton ID="imbSearchCampaign" runat="server" ImageAlign="AbsMiddle" ToolTip="ค้นหา"  
                        ImageUrl="~/Images/iSearch.gif" onclick="imbSearchCampaign_Click" OnClientClick="GetScreenResolution()" />
                    <asp:Label ID="vcmbCampaignId" runat="server" CssClass="style4"></asp:Label>
                    <asp:TextBox ID="txtScreenHeight" runat="server" CssClass="Hidden" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtScreenWidth" runat="server" CssClass="Hidden" Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtOwnerBranchUserLogin" runat="server" CssClass="Hidden"  Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtOwnerBranchBefore" runat="server" CssClass="Hidden"  Width="1px"></asp:TextBox>
                    <asp:TextBox ID="txtOwnerLeadBefore" runat="server" CssClass="Hidden"  Width="1px"></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">หมายเลขโทรศัพท์ 2</td>
                <td class="style3">
                    <asp:TextBox ID="txtTelNo2" runat="server" CssClass="Textbox" Width="250px" 
                        MaxLength="10" ></asp:TextBox>
                <asp:Label ID="vtxtTelNo2" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
             <tr style="vertical-align:top;">
                <td class="style2">Owner Branch</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        OnSelectedIndexChanged="cmbOwnerBranch_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbOwnerBranch" runat="server" CssClass="style4" ></asp:Label>
                </td>
                 <td class="style1"></td>
                <td class="style2">Owner lead</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbOwner" runat="server" Width="253px" 
                        CssClass="Dropdownlist" Enabled="false" AutoPostBack="True" 
                        OnSelectedIndexChanged="cmbOwner_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbOwner" runat="server" CssClass="style4"></asp:Label>
                    <asp:TextBox ID="txtOldOwner" runat="server" CssClass="Hidden" ></asp:TextBox>
                </td>
            </tr>
            <tr style="vertical-align:top;" runat="server" id="trInfo2">
                <td class="style2">Delegate Branch</td>
                <td class="style3" >
                    <asp:DropDownList ID="cmbDelegateBranch" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbDelegateBranch_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:Label ID="vcmbDelegateBranch" runat="server" CssClass="style4" ></asp:Label>
                </td>
                 <td class="style1"></td>
                <td class="style2">Delegate lead</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbDelegateLead" runat="server" Width="253px" 
                        CssClass="Dropdownlist" AutoPostBack="True" 
                        onselectedindexchanged="cmbDelegateLead_SelectedIndexChanged"  ></asp:DropDownList>
                    <asp:Label ID="vcmbDelegateLead" runat="server" CssClass="style4"></asp:Label>
                    <asp:TextBox ID="txtoldDelegate" runat="server" CssClass="Hidden" ></asp:TextBox>
                </td>
            </tr>
        </table>
        &nbsp;
        <table runat="server" id="tbLeadInfo">
            <tr>
                <td class="style2">วันเวลาที่ติดต่อ Lead ล่าสุด</td>
                <td class="style3">
                    <asp:TextBox ID="txtContactLatestDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">วันเวลาที่ได้รับมอบหมายล่าสุด</td>
                <td class="style3">
                    <asp:TextBox ID="txtAssignDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">วันเวลาที่ติดต่อ Lead ครั้งแรก</td>
                <td class="style3" >
                    <asp:TextBox ID="txtContactFirstDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                 <td class="style1"></td>
                <td class="style2">ผู้สร้าง Lead</td>
                <td class="style3" >
                    <asp:TextBox ID="txtCreateBy" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="style2">วันที่สร้าง Lead</td>
                <td class="style3">
                   <asp:TextBox ID="txtCreateDate" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
                <td class="style1"></td>
                <td class="style2">เวลาที่สร้าง Lead</td>
                <td class="style3">
                    <asp:TextBox ID="txtCreateTime" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
            <tr style="vertical-align:top;" runat="server" id="trTicketIdRefer">
                <td class="style2">Ticket ID Refer</td>
                <td class="style3" >
                    <asp:TextBox ID="txtTicketIdRefer" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox><br />
                    <asp:Label ID="vtxtTicketIdRefer" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3"></td>
            </tr>
            <tr>
                <td class="style2"></td>
                <td class="style3">
                    
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="upPopupSearchCampaign" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSearchCampaign" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSearchCampaign" style="display:none" CssClass="modalBoxlSearchCampaign" ScrollBars="Auto" DefaultButton="btnSearchCampaign">
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/SearchCampaign.jpg" />
		    <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="width:20px;"></td>
                    <td>
                    </td>
                    <td style="width:220px; font-weight:bold;">กลุ่มผลิตภัณฑ์/บริการ</td>
                    <td style="width:220px; font-weight:bold;">ผลิตภัณฑ์/บริการ</td>
                    <td style="font-weight:bold; ">แคมเปญ</td>
                </tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td>
                        <asp:RadioButton ID="rbSearchByCombo" runat="server" GroupName="Campaign" AutoPostBack="true"
                            Checked="true" oncheckedchanged="rbSearchByCombo_CheckedChanged"  />
                    </td>
                    <td style="width:220px;">
                        <asp:DropDownList ID="cmbProductGroup" runat="server" AutoPostBack="true" 
                            CssClass="Dropdownlist" Width="200px" 
                            onselectedindexchanged="cmbProductGroup_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td style="width:220px;">
                        <asp:DropDownList ID="cmbProduct" runat="server" AutoPostBack="true" 
                            CssClass="Dropdownlist" Width="200px" 
                            onselectedindexchanged="cmbProduct_SelectedIndexChanged"></asp:DropDownList>
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
                <tr><td colspan="5" style="height:3px;"></td></tr>
                <tr>
                    <td style="width:20px;"></td>
                    <td colspan="4">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSearchCampaign" runat="server" Text="ค้นหา" Width="80px" 
                            CssClass="Button" OnClientClick="DisplayProcessing()" onclick="btnSearchCampaign_Click" />
                    </td>
                </tr>
                <tr><td colspan="5" style="height:3px;"></td></tr>
            </table>
            <hr style="border-top:1px solid gray; border-bottom-style:none; border-left-style:none; border-right-style:none;" />
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="width:20px;"></td>
                    <td style="height:340px; vertical-align:top; ">
                        <asp:Label ID="label400" Text="* เลือกแคมเปญได้ 1 รายการเท่านั้น" runat="server" ForeColor="Red" ></asp:Label><br /><br />
                        <uc2:GridviewPageController ID="pcGridCampaign" runat="server" OnPageChange="PageSearchChange" Width="910px" />
                        <asp:GridView ID="gvCampaign" runat="server" AutoGenerateColumns="False" Width="910px" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"
                            GridLines="Horizontal" BorderWidth="1px" EnableModelValidation="True" 
                            onrowdatabound="gvCampaign_RowDataBound"  >
                            <Columns>
                                <asp:TemplateField HeaderText="เลือก">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server" AutoPostBack="true" OnCheckedChanged="cbSelect_CheckedChanged" TabIndex="<%# Container.DisplayIndex %>" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="35px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="35px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductGroupName" HeaderText="กลุ่มผลิตภัณฑ์/บริการ"  >
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ"  >
                                    <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="รายละเอียดแคมเปญ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Eval("CampaignDesc") %>'></asp:Label>
                                        <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="205px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="205px" HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                                    <ItemTemplate>
                                        <%# Eval("StartDate") != null ? Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                    <ItemTemplate>
                                        <%# Eval("EndDate") != null ? Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() : ""%>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
                                    <FooterStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle CssClass="Hidden" />
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden" />
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
                        <asp:Button id="btnClose" runat="server" Text="ปิดหน้าต่าง" Width="100px" 
                            onclick="btnClose_Click" />
                    </td>
                </tr>
            </table>
            <br />
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSearchCampaign" runat="server" TargetControlID="btnPopupSearchCampaign" PopupControlID="pnPopupSearchCampaign" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    function GetScreenResolution() {

        document.getElementById('<%= txtScreenWidth.ClientID %>').value = screen.width;
        document.getElementById('<%= txtScreenHeight.ClientID %>').value = screen.height;
    }
</script>
