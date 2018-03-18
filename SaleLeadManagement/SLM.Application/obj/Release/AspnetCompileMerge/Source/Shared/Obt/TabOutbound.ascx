<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabOutbound.ascx.cs" Inherits="SLM.Application.Shared.Obt.TabOutbound" %>

<%@ Register src="../TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>

<asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
<asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">
            <tr><td colspan="4" style="height:2px;"></td></tr>
            <tr>
                <td class="ColInfo">
                    Ticket ID
                </td>
                <td class="ColInput">
                    <asp:TextBox ID="txtTicketID" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    <asp:TextBox ID="txtEmpCode" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffTypeId" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffTypeDesc" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffId" runat="server" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="txtStaffBranchCode" runat="server" Visible="false"></asp:TextBox>
                </td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td class="ColInfo">
                    ชื่อ
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
                    <asp:DropDownList ID="cmbCardType" runat="server" Width="203px" CssClass="Dropdownlist" >
                    </asp:DropDownList>
                </td>
                <td class="ColInfo">
                    เลขที่บัตร
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
                    CssClass="Dropdownlist" ></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="height:10px; vertical-align:bottom;">
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
            </table><br />
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td valign="top" class="ColInfo">
                            สถานะ Outbound
                    </td>
                    <td class="ColInput">
                        <asp:CheckBoxList  ID="cbOptionList" runat="server" Visible="false"></asp:CheckBoxList>
                          <asp:CheckBox  ID="cbOptionAll" runat="server" Visible="false"></asp:CheckBox>
                        <asp:DropDownList ID="cmbSLMStatus" runat="server" CssClass="DropdownList" Width="200px">
                            <asp:ListItem id= "b001" Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem id= "b002" Text="งานติดปัญหา"></asp:ListItem>
                            <asp:ListItem id= "b003" Text="รอผลการพิจารณา "></asp:ListItem>
                            <asp:ListItem id= "b004" Text="อนุมัติ - ตามเสนอ"></asp:ListItem>
                            <asp:ListItem id= "b005" Text="ปฎิเสธลูกค้า (Reject)"></asp:ListItem>
                            <asp:ListItem id= "b006" Text="ลูกค้ายกเลิก (Cancel)"></asp:ListItem>
                            <asp:ListItem id= "b007" Text="สำเร็จ (Success)"></asp:ListItem>
                            <asp:ListItem id= "b008" Text="อยู่ระหว่างดำเนินการ"></asp:ListItem>
                            <asp:ListItem id= "b009" Text="รอติดต่อลูกค้า"></asp:ListItem>
                            <asp:ListItem id= "b010" Text="Follow Up"></asp:ListItem>
                        </asp:DropDownList>
                       
                         
                    </td>
                    <td class="ColInfo">
                            สถานะย่อย Outbound
                    </td>
                    <td>
                       <asp:DropDownList ID="DropDownList1" runat="server" CssClass="DropdownList" Width="200px">
                            <asp:ListItem id= "c001" Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem id= "c002" Text="เคลมได้"></asp:ListItem>
                            <asp:ListItem id= "c003" Text=""></asp:ListItem>
                            <asp:ListItem id= "c004" Text=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" style="height:15px;">
                    </td>
                </tr>
            </table>
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
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="2585px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                AllowSorting="true" onsorting="gvResult_Sorting" Width="2585px" 
                onrowdatabound="gvResult_RowDataBound" ondatabound="gvResult_DataBound" >
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
                        &nbsp;<asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Eval("TicketId") %>' OnClick="imbView_Click" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง"  />
                        <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("TicketId") %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลผู้มุ่งหวัง"  />
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
                <asp:TemplateField HeaderText="Ticket ID">
                    <ItemTemplate>
                        <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("TicketId") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภทบุคคล">
                    <ItemTemplate>
                        <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
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
                <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead" SortExpression="StatusDesc">
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="ExternalSubStatusDesc" HeaderText="สถานะย่อยของ Lead">
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ" SortExpression="CampaignName">
                    <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                    <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
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
                <asp:BoundField DataField="CreateName" HeaderText="ผู้สร้าง Lead">
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
                <asp:TemplateField HeaderText="วันที่ได้รับมอบหมายล่าสุด">
                    <ItemTemplate>
                        <%# Eval("AssignedDate") != null ? Convert.ToDateTime(Eval("AssignedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("AssignedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("AssignedDate")).ToString("HH:mm:ss") : ""%>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Calculator">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbCalculator" runat="server" Text="Calculator" ></asp:LinkButton>
                        <asp:LinkButton ID="lbSaleTool" runat="server" Text="Calculator" Visible="false" OnClientClick='<%# string.Format("javascript:return callsaletool(\"{0}\")", Eval("TicketId")) %>' ></asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="85px" CssClass="Hidden" />
                    <ItemStyle Width="85px"  CssClass="Hidden"  />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Document">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbDocument" runat="server" Text="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("TicketId") %>' OnClientClick="DisplayProcessing()" ></asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="85px" CssClass="Hidden"/>
                    <ItemStyle Width="85px" CssClass="Hidden"  />
                </asp:TemplateField>
                <asp:BoundField DataField="OwnerBranchName" HeaderText="Owner Branch">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                    <asp:BoundField DataField="DelegateBranchName" HeaderText="Delegate Branch">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                    <asp:BoundField DataField="BranchCreateBranchName" HeaderText="สาขาผู้สร้าง Lead">
                    <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                    <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:BoundField DataField="ContractNoRefer" HeaderText="เลขที่สัญญา<br/>ที่เคยมีกับธนาคาร" HtmlEncode="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center"  />
                    <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="ProductName">
                    <ItemTemplate>
                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
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
                <asp:TemplateField HeaderText="CalculatorUrl">
                    <ItemTemplate>
                        <asp:Label ID="lblCalculatorUrl" runat="server" Text='<%# Eval("CalculatorUrl") %>'></asp:Label>
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
                <asp:TemplateField HeaderText="AppNo">
                    <ItemTemplate>
                        <asp:Label ID="lblAppNo" runat="server" Text='<%# Eval("AppNo") %>'></asp:Label>
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
                <asp:TemplateField HeaderText="slmStatusCode">
                    <ItemTemplate>
                        <asp:Label ID="lbslmStatusCode" runat="server" Text='<%# Eval("slmStatusCode") %>'></asp:Label>
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
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>

