<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_016.aspx.cs" Inherits="SLM.Application.SLM_SCR_016" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:190px;
        }
        .ColInput
        {
            width:350px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        .style1
        {
            width: 5px;
        }
        .style2
        {
            width: 180px;
            text-align:left;
            font-weight:bold;
        }
        .style3
        {
            width: 250px;
            text-align:left;
        }
        .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
        .style5
        {
            width: 180px;
            text-align:left;
            font-weight:bold;
        }  
        .FixedHeader {
            position: absolute;
            font-weight: bold;
        }     
    </style>
    <script language="javascript" type="text/javascript">
        function DisplayLoading() {
            var cb = document.getElementById('<%= gvHistory.ClientID %>');

            if (cb != null) {
                cb.style.display = "none";
            }
            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
           <table cellpadding="2" cellspacing="0" border="0" width="1050px">
                <tr  >
                    <td class="ColInfo">
                        รหัสบัตรประชาชน
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSearchCitizenId" runat="server" CssClass="Textbox" Width="200px" MaxLength="13" ></asp:TextBox>
                    </td>
                    <td class="ColInfo" >ช่องทาง</td>
                    <td class="ColInput">
                        <asp:Label ID="lblChannel" runat="server" ForeColor="Red" Font-Bold="true"  CssClass="LabelC"
                            BorderWidth="1px" Width="200px" BorderColor="Gray" BackColor="LightGray" Font-Size="13px" Height="18px" ></asp:Label>
                        <asp:Label ID="lblChannelId" runat="server" Visible="false"  ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo" >ชื่อ</td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSearchFirstname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo" >นามสกุล</td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSearchLastname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo" >เบอร์มือถือ</td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSearchTelNo1" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo" >เลขที่สัญญาที่เคยมีกับธนาคาร</td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSearchContractNoRefer" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                     <td class="ColInfo">  
                     </td>
                     <td colspan="3" class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                                    Text="ค้นหา" onclick="btnSearch_Click" />
                     </td>
                </tr>
           </table>
        </ContentTemplate>
    </asp:UpdatePanel> <br />   
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/ProductOffer.gif" ImageAlign="Top" Visible="false" /><br />
            <div style="height:4px;"></div>
            <asp:Panel ID="pnlResult" runat="server" ScrollBars ="Auto" Height="250px" Width="1220px" BorderStyle="Solid" BorderWidth="1px" Visible="false" >
                <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1710px" OnRowDataBound="gvResult_RowDataBound"
                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"  >
                    <Columns>
                      <asp:TemplateField HeaderText="พิจารณา">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imbInsertLead" ImageUrl="~/Images/bAdd.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbInsertLead_Click" />
                            </ItemTemplate>
                            <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="30px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อ">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("Firstname") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="นามสกุล">
                            <ItemTemplate>
                                <asp:Label ID="lblLastname" runat="server" Text='<%# Bind("Lastname") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เบอร์มือถือ">
                            <ItemTemplate>
                                <asp:Label ID="lblTelNo1" runat="server" Text='<%# Bind("TelNo1") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                            <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email">
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                            <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เลขที่สัญญาที่เคยมีกับธนาคาร">
                            <ItemTemplate>
                                <asp:Label ID="lblContractNoRefer" runat="server" Text='<%# Bind("ContractNoRefer") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top"  />
                        </asp:TemplateField>
                          <asp:TemplateField HeaderText="Remark">
                            <ItemTemplate>
                                <asp:Label ID="lblRemark" runat="server" Text='<%# Bind("Remark") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Assignment">
                            <ItemTemplate>
                                <asp:Label ID="lblAssignment" runat="server" Text='<%# Bind("Assignment") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รหัส ผลิตภัณฑ์/<br/>แคมเปญ">
                            <ItemTemplate>
                                <asp:Label ID="lblCompaignId" runat="server" Text='<%# Bind("CampaignId") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อ ผลิตภัณฑ์/แคมเปญ">
                            <ItemTemplate>
                                <asp:Label ID="lblCampaignName" runat="server" Text='<%# Bind("CampaignName") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left"  VerticalAlign="Top"/>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รายละเอียด ผลิตภัณฑ์/แคมเปญ">
                            <ItemTemplate>
                                <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Bind("CampaignDesc") %>' ></asp:Label>
                                <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                            <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                            <ItemTemplate>
                                <%# Eval("ExpireDate") != null && Convert.ToDateTime(Eval("ExpireDate")).Year != 1 ? Convert.ToDateTime(Eval("ExpireDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("ExpireDate")).Year.ToString() : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" HorizontalAlign="Center"  />
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง" >
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="CitizenId">
                            <ItemTemplate>
                                <asp:Label ID="lblCitizenId" runat="server" Text='<%# Bind("CitizenId") %>'></asp:Label>
                            </ItemTemplate>
                            <ControlStyle CssClass="Hidden" />
                            <HeaderStyle CssClass="Hidden" />
                            <ItemStyle CssClass="Hidden" />
                            <FooterStyle CssClass="Hidden" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CampaignFullDesc">
                            <ItemTemplate>
                                <asp:Label ID="lblCampaignFullDesc" runat="server" Text='<%# Bind("CampaignFullDesc") %>'></asp:Label>
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
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel> 
   <br />
   <br /> 
        <asp:UpdatePanel ID="upHistory" runat="server" UpdateMode="Conditional"> 
            <ContentTemplate>
                <asp:Image ID="imgHistory" runat="server" ImageUrl="~/Images/HadOffer2.gif" ImageAlign="Top" Visible="false" />&nbsp;
                <asp:Button ID="btnHistory" runat="server" CssClass="Button" Width="250px" OnClientClick="DisplayLoading()"
                    Text="เรียกดูข้อมูลแคมเปญที่เคยมีการนำเสนอ" Visible="false" onclick="btnHistory_Click" />
                <br />
                <asp:Panel ID="pnlHistory" runat="server"  Height="250px" Width="1220px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px" CssClass="Hidden" >
                    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="False" Width="1800px"
                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                        onrowdatabound="gvHistory_RowDataBound" Visible="false" >
                        <Columns>
                            <asp:TemplateField HeaderText="ชื่อ">
                                <ItemTemplate>
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("Firstname") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"  />
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="นามสกุล">
                                <ItemTemplate>
                                    <asp:Label ID="lblLastname" runat="server" Text='<%# Bind("Lastname") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="เบอร์มือถือ">
                                <ItemTemplate>
                                    <asp:Label ID="lblTelNo1" runat="server" Text='<%# Bind("TelNo1") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="เลขที่สัญญาที่เคยมีกับธนาคาร">
                                <ItemTemplate>
                                    <asp:Label ID="lblContractNoRefer" runat="server" Text='<%# Bind("ContractNoRefer") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                                <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"  />
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Remark">
                                <ItemTemplate>
                                    <asp:Label ID="lblRemark" runat="server" Text='<%# Bind("Remark") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="รหัส<br/>ผลิตภัณฑ์/แคมเปญ">
                                <ItemTemplate>
                                    <asp:Label ID="lblCompaignId" runat="server" Text='<%# Bind("CampaignId") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                                <ItemStyle Width="140px" HorizontalAlign="Center" VerticalAlign="Top"  />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ชื่อ ผลิตภัณฑ์/แคมเปญ">
                                <ItemTemplate>
                                    <asp:Label ID="lblCampaignName" runat="server" Text='<%# Bind("CampaignName") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                                <ItemStyle Width="200px" HorizontalAlign="Left"  VerticalAlign="Top"/>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="รายละเอียด ผลิตภัณฑ์/แคมเปญ">
                                <ItemTemplate>
                                    <asp:Label ID="lblCampaignDesc" runat="server" Text='<%# Bind("CampaignDesc") %>' ></asp:Label>
                                    <asp:LinkButton ID="lbShowCampaignDesc" runat="server" Text="อ่านต่อ" CommandArgument='<%# Eval("CampaignId") %>' Visible="false" ></asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle Width="250px" HorizontalAlign="Center"/>
                                <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:BoundField HtmlEncode ="false" DataField="StaffOfferChannelDesc" HeaderText="ช่องทาง<br />เจ้าหน้าที่ผู้นำเสนอ" >
                                <HeaderStyle Width="130px" HorizontalAlign="Center" />
                                <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StaffOfferBranchName" HeaderText="สาขาเจ้าหน้าที่ผู้นำเสนอ" >
                                <HeaderStyle Width="170px" HorizontalAlign="Center" />
                                <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StaffOfferName" HeaderText="ผู้นำเสนอ" >
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="วันที่นำเสนอ">
                                <ItemTemplate>
                                    <%# Eval("UpdatedDate") != null && Convert.ToDateTime(Eval("UpdatedDate")).Year != 1 ? Convert.ToDateTime(Eval("UpdatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("UpdatedDate")).Year.ToString() : ""%>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center"  />
                                <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="IsInterested" HeaderText="สนใจ/ไม่สนใจ" >
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                            </asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="t_rowhead" />
                        <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                    </asp:GridView>
                    <asp:UpdateProgress runat="server" id="PageUpdateProgress">
                       <ProgressTemplate>
                          <div>
                             <img src="Images/loading14.gif" alt="" />           
                         </div>
                       </ProgressTemplate>
                </asp:UpdateProgress>
                </asp:Panel>
                <br />
            </ContentTemplate>
        </asp:UpdatePanel> 
        <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	                <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupSCR016" ScrollBars="Auto">
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgDetail" runat="server" ImageUrl="~/Images/AddLead.gif" />
		                <table cellpadding="2" cellspacing="0" border="0" >
                            <tr>
                                <td style="width:10px"></td>
                                <td class="style2" valign="top" ></td>
                                <td colspan="4">
                                    <asp:RadioButton ID="rdInterest" runat="server" Text="สนใจ" GroupName="InterestGroup" AutoPostBack="true"  OnCheckedChanged="rdInterest_CheckedChanged" />&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:RadioButton ID="rdNoInterest" runat="server" Text="ไม่สนใจ" GroupName="InterestGroup" AutoPostBack="true"  OnCheckedChanged="rdNoInterest_CheckedChanged" />
                                    <asp:TextBox ID="txtCampaignIdHidden" runat="server" Width="50px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtCampaignFullDescHidden" runat="server" Width="50px" Visible="false"></asp:TextBox>
                                    <asp:TextBox ID="txtCitizenIdHidden" runat="server" Width="50px" Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="height:1px" colspan="6"></td>
                            </tr>
                            <tr>
                                <td colspan="6">
                                    <asp:Panel id="pnlLeadInfo" runat="server" Enabled="false">
                                         <table cellpadding="2" cellspacing="0" border="0">
                                             <tr style="vertical-align:top;">
                                                <td style="width:10px"></td>
                                                <td class="style2">ชื่อ <span class="style4">*</span></td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtFirstname" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                                                    <br /><asp:Label ID="vtxtFirstname" runat="server" CssClass="style4"></asp:Label>
                                                </td>
                                                <td class="style1"></td>
                                                <td class="style5">นามสกุล</td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtLastname" runat="server" CssClass="Textbox" Width="250px" MaxLength="120" ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="vertical-align:top;">
                                                <td style="width:10px"></td>
                                                <td class="style2">หมายเลขโทรศัพท์ 1(มือถือ)<span class="style4">*</span></td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtTelNo1" runat="server" CssClass="Textbox" MaxLength="10" Width="150px" AutoPostBack="true"
                                                     OnTextChanged="txtTelNo1_TextChanged" ></asp:TextBox><br />
                                                    <asp:Label ID="vtxtTelNo1" runat="server" CssClass="style4"></asp:Label>
                                                </td>
                                                <td class="style1"></td>
                                               <td class="style5">เวลาที่สะดวกให้ติดต่อกลับ</td>
                                                <td class="style3" >
                                                    <asp:TextBox ID="txtAvailableTimeHour" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                                                    <asp:Label ID="label4" runat="server" CssClass="LabelC" Text=":" Width="5px" ></asp:Label>
                                                    <asp:TextBox ID="txtAvailableTimeMinute" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                                                    <asp:Label ID="label3" runat="server" CssClass="LabelC" Text=":" Width="5px" ></asp:Label>
                                                    <asp:TextBox ID="txtAvailableTimeSecond" runat="server" CssClass="TextboxC" Width="30px" MaxLength="2" ></asp:TextBox>
                                                    <br />
                                                    <asp:Label ID="vtxtAvailableTime" runat="server" CssClass="style4"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:UpdatePanel ID="upPopupInner" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <table cellpadding="2" cellspacing="0" border="0">
                                                    <tr style="vertical-align:top;">
                                                        <td style="width:10px"></td>
                                                        <td class="style2">Owner Branch</td>
                                                        <td class="style3">
                                                            <asp:DropDownList ID="cmbOwnerBranch" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" 
                                                            onselectedindexchanged="cmbOwnerBranch_SelectedIndexChanged" ></asp:DropDownList><br />
                                                            <asp:Label ID="vcmbOwnerBranch" runat="server" CssClass="style4"></asp:Label>
                                                        </td>
                                                        <td class="style1"></td>
                                                        <td class="style5">Owner lead</td>
                                                        <td class="style3">
                                                            <asp:DropDownList ID="cmbOwner" runat="server" Width="253px" CssClass="Dropdownlist" Enabled="false" AutoPostBack="True" 
                                                                onselectedindexchanged="cmbOwner_SelectedIndexChanged" ></asp:DropDownList>
                                                            <asp:Label ID="vcmbOwner" runat="server" CssClass="style4"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <table cellpadding="2" cellspacing="0" border="0">
                                             <tr>
                                                <td style="width:10px"></td>
                                                <td class="style2" valign="top" >แคมเปญ</td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtCampaignName" runat="server" CssClass="TextboxView" ReadOnly="true" Width="250px"></asp:TextBox>
                                                </td>
                                                <td class="style1"></td>
                                                <td class="style5">
                                                    เลขที่สัญญาที่เคยมีกับธนาคาร
                                                </td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtContractNoRefer" runat="server" CssClass="Textbox" Width="150px" ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width:10px"></td>
                                                <td class="style2" valign="top" >Email</td>
                                                <td class="style3">
                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="Textbox" Width="250px"></asp:TextBox>
                                                    <br />
                                                    <asp:Label ID="vtxtEmail" runat="server" CssClass="style4"></asp:Label>
                                                </td>
                                                <td class="style1"></td>
                                                <td class="style5">
                                                </td>
                                                <td class="style3">
                                                </td>
                                            </tr>
                                             <tr>
                                                <td style="width:10px"></td>
                                                <td class="style2" valign="top" >รายละเอียด</td>
                                                <td colspan="4">
                                                    <asp:TextBox ID="txtDetail" runat="server" CssClass="Textbox" Width="695px" Height="70px" TextMode ="MultiLine"  MaxLength="4000" ></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr style="height:35px;">
                                <td class="ColIndent"></td>
                                <td class="style2" >&nbsp;</td>
                                <td colspan="3" style="vertical-align:top">
                                    <asp:Label ID="vtxtDetail" runat="server" ForeColor="Red" ></asp:Label>
                                </td>
                                <td align="right" style="vertical-align:top" >
                                    <asp:Button ID="btnSaveNoInterest" runat="server" Text="บันทึก" Width="100px" OnClick="btnSaveNoInterest_Click" Visible="false" OnClientClick="if (confirm('ต้องการบันทึกใช่หรือไม่?')) { DisplayProcessing(); return true; } else { return false; }" />&nbsp;
                                    <asp:Button ID="btnCreateLead" runat="server" Text="สร้าง Lead" Width="100px" OnClick="btnCreateLead_Click" Visible="false" OnClientClick="if (true) { DisplayProcessing(); return true; } else { return false; }" />&nbsp;
                                    <asp:Button ID="btnClose" runat="server" Text="ยกเลิก" Width="100px" OnClick="btnClose_Click" />&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
	                </asp:Panel>
	                <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	                </act:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upPopupSaveResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupSaveResult" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopupSaveResult" style="display:none" CssClass="modalPopupCreateLeadSuggestCampaign">
                <br />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="2" style="height:1px;"></td></tr>
                    <tr>
                        <td style="width:40px;"></td>
                        <td class="ColInfo" style="font-size:14px; width:380px;">
                            <b>บันทึกข้อมูลผู้มุ่งหวังสำเร็จ</b>
                        </td>
                    </tr>
                    <tr><td colspan="2" style="height:5px;"></td></tr>
                     <tr>
                        <td style="width:40px;"></td>
                        <td>
                            <b>Ticket Id:</b>&nbsp;<asp:Label ID="lblResultTicketId" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:40px;"></td>
                        <td>
                            <b>แคมเปญ:</b>&nbsp;<asp:Label ID="lblResultCampaign" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:40px;"></td>
                        <td>
                            <b>ช่องทาง:</b>&nbsp;<asp:Label ID="lblResultChannel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:40px;"></td>
                        <td>
                            <b>Owner Lead:</b>&nbsp;<asp:Label ID="lblResultOwnerLead" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="2" style="height:20px;"></td></tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="Button" 
                                Width="100px" OnClick="btnOK_Click" />&nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupSaveResult" runat="server" TargetControlID="btnPopupSaveResult" PopupControlID="pnPopupSaveResult" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
