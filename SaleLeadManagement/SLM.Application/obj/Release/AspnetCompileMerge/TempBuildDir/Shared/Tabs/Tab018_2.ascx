<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab018_2.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab018_2" %>

<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <br />
        <asp:Button ID="btnTransfer" runat="server" Text="โอนงาน" CssClass="Button" 
            Width="80px" onclick="btnTransfer_Click" />
        <asp:TextBox ID="txtusername" runat="server"  CssClass="Hidden" ></asp:TextBox>
        <br />
        <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1210px" />
        <asp:Panel ID="pnlResult" runat="server"  ScrollBars ="Auto"  Width="1210px" BorderStyle="Solid" BorderWidth="1px">
            <asp:GridView ID="gvDelegate" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" >
                <Columns>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbSelect" runat="server" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" HorizontalAlign="Center"/>
                        <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Ticket ID">
                        <ItemTemplate>
                            <asp:Label ID="lbTicketId" runat="server" Text ='<%# Bind("TicketId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                   
                    <asp:BoundField DataField="Firstname" HeaderText="ชื่อ" >
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Lastname" HeaderText="นามสกุล" >
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead">
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง">
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerBranchName" HeaderText="Owner Branch">
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelegateName" HeaderText="Delegate Lead">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelegateBranchName" HeaderText="Delegate Branch">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="วันที่สร้าง Lead">
                        <ItemTemplate>
                            <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่ได้รับมอบหมายล่าสุด">
                        <ItemTemplate>
                            <%# Eval("AssignedDate") != null ? Convert.ToDateTime(Eval("AssignedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("AssignedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("AssignedDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="ประเภทบุคคล">
                        <ItemTemplate>
                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="90px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="90px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="CampaignId">
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupTab018_1">
                <table cellpadding="2" cellspacing="0" border="0" >
                     <tr>
                        <td colspan="3" style="height:10px"></td>
                    </tr>
                    <tr>
                        <td colspan="3">&nbsp;&nbsp;<asp:Image ID="Image3" runat="server" ImageUrl="~/Images/Transfer.gif" /></td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">รหัสพนักงานธนาคาร</td>
                        <td class="style3">
                            <asp:TextBox ID="txtEmpCodePopup" runat="server" CssClass="Textbox"  Width="150px" MaxLength="6" ></asp:TextBox>&nbsp;
                            <asp:Button ID="btnSearch" runat="server"  Text="ค้นหา" Width="80px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing()" />
                            
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">Windows Username</td>
                        <td class="style3">
                            <asp:TextBox ID="txtUsernamePopup" runat="server" CssClass="TextboxView" Width="150px" ReadOnly="true" MaxLength="100" ></asp:TextBox>
                            <asp:TextBox ID="txtStaffId" runat="server" CssClass="Hidden"  ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">รหัสเจ้าหน้าที่การตลาด</td>
                        <td class="style3">
                            <asp:TextBox ID="txtMarketingCode" runat="server" CssClass="TextboxView" ReadOnly="true" Width="150px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">ชื่อ-นามสกุลพนักงาน </td>
                        <td class="style3">
                            <asp:TextBox ID="txtStaffNameTH" runat="server" CssClass="TextboxView" ReadOnly="true" Width="260px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">เบอร์โทรศัพท์ </td>
                        <td class="style3">
                            <asp:TextBox ID="txtTellNo" runat="server" CssClass="TextboxView" ReadOnly="true" Width="100px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">E-mail </td>
                        <td class="style3">
                            <asp:TextBox ID="txtStaffEmail" runat="server" CssClass="TextboxView" ReadOnly="true" Width="260px" ></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="style1"></td>
                        <td class="style2">ตำแหน่ง </td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbPosition" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                            <asp:Label ID="Label1" runat="server" CssClass="style4"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                        <td class="style1"></td>
                        <td class="style2">Role </td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbStaffType" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">ทีมการตลาด</td>
                        <td class="style3">
                            <asp:TextBox ID="txtTeam" runat="server" CssClass="TextboxView" ReadOnly="true" Width="260px" ></asp:TextBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">สาขาพนักงาน </td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbBranchCode" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">สาขาหัวหน้างาน </td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbHeadBranchCode" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1"></td>
                        <td class="style2">หัวหน้างาน</td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbHeadStaffId" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                        </td>
                    </tr>
                     <tr>
                        <td class="style1"></td>
                        <td class="style2">สถานะ </td>
                        <td class="style3">
                            <asp:RadioButton ID="rdNormal" runat="server" GroupName="EmpStatus" Text="ปกติ" Enabled="false" />
                            <asp:RadioButton ID="rdRetire" runat="server" GroupName="EmpStatus" Text="ลาออก" Enabled="false" />&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="width:10px"></td>
                        <td class="style2">สาย </td>
                        <td class="style3">
                            <asp:DropDownList ID="cmbDepartment" runat="server" CssClass="Dropdownlist" Enabled="false" Width="262px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height:10px;">
                        </td>
                    </tr>
                    <tr style="height:35px;">
                        <td class="style1"></td>
                        <td class="style2"></td>
                        <td  class="style3" >
                            <asp:Button ID="btnSavePopup" runat="server" Text="บันทึก" Width="100px" onclick="btnSavePopup_Click"  
                            OnClientClick="DisplayProcessing()" />&nbsp;
                            <asp:Button ID="btnClose" runat="server" Text="ยกเลิก" Width="100px" onclick="btnClose_Click" />
                        </td>
                    </tr>
                </table>
	        </asp:Panel>
	        <act:ModalPopupExtender ID="mpePopupTransfer" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>