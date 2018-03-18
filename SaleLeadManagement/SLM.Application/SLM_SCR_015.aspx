<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_015.aspx.cs" Inherits="SLM.Application.SLM_SCR_015" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:120px;
        }
        .ColInput
        {
            width:200px;
        }
         .ColInfoMKT
        {
            font-weight:bold;
            width:150px;
        }
         .ColInputMKT
        {
            width:280px;
        }
        .style1
        {
            width: 962px;
        }
          .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }

        .style6
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: blue;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />


    <asp:Panel id="pnlMKT" runat="server" >
    <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/MonitoringTitle5.png" ImageAlign="Top" />&nbsp;
    <br />
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional"> 
        <ContentTemplate>
            <table cellpadding="3" cellspacing="0" >
                <tr><td colspan="4" style="height:10px" ></td></tr>
                <tr>
                    <td class="ColInfoMKT"><asp:Label ID="lbWaitSum" runat="server" Text="จำนวน Lead รอจ่าย"></asp:Label></td>
                    <td colspan="3" class="ColInputMKT">
                        <asp:LinkButton ID="lbUnassignLeadMKT" runat="server" OnClick="lbUnassignLeadMKT_Click" BorderStyle="Solid" BorderWidth="1px" 
                            BorderColor="#7f9db9" BackColor="#e5edf5" Width="100px" CssClass="TextboxViewR" ></asp:LinkButton>
                        <asp:TextBox ID="txtStaffId" runat="server" CssClass="Hidden" Width="100px" ></asp:TextBox>
                        <asp:TextBox ID="txtStaffType" runat="server" CssClass="Hidden" Width="100px" ></asp:TextBox>
                        <asp:TextBox ID="txtPopupFlag" runat="server" CssClass="Hidden" Width="100px" ></asp:TextBox>
                        <asp:TextBox ID="txtEmpCode" runat="server" CssClass="Hidden" Width="100px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfoMKT">ผลิตภัณฑ์/บริการ</td>
                    <td class="ColInputMKT">
                        <asp:DropDownList ID="cmbProduct" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                        <asp:TextBox ID="txtStaffTypeIdLogin" runat="server" CssClass="Hidden" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">แคมเปญ</td>
                    <td class="ColInputMKT"> <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="ColInfoMKT">สถานะย่อยของ Lead</td>
                    <td class="ColInputMKT">
                        <asp:TextBox runat="server" ID="txtSubStatus" CssClass="Textbox" Width="250px"></asp:TextBox>
                    <td class="ColInfoMKT"></td>
                    <td class="ColInputMKT"></td>
                </tr>
                <tr>
                <td class="ColInfoMKT">
                    สาขา
                </td>
                <td class="ColInputMKT">
                    <asp:DropDownList ID="cmbSearchBranch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                </td>
                <td class="ColInfo">สถานะการทำงาน</td>
                <td class="ColInputMKT"> 
                     <asp:DropDownList ID="cmbStatusMKT" runat="server"  Width="250px" CssClass="Dropdownlist">
                        <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
                        <asp:ListItem Text="Unavailable" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Available" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="ColInfoMKT">วันที่จ่ายงาน</td>
                <td class="ColInputMKT">
                    <uc2:TextDateMask ID="tdMKTAssighDateFrom" runat="server" />
                </td>
                    <td  class="ColInfo">
                    ถึง
                </td>
                <td class="ColInputMKT">
                    <uc2:TextDateMask ID="tdMKTAssighDateTo" runat="server" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" >
                    <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" 
                    Width="100px" OnClientClick="DisplayProcessing()" onclick="btnSearch_Click"  />
                    <asp:TextBox ID="txtDateFrom" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtDateTo" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtIsActive" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtRecursive" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtStatuscode" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                    <asp:TextBox ID="txtListUsernameFooter" runat="server" CssClass="Hidden" ReadOnly="true" ></asp:TextBox>
                </td>
            </tr>
            </table><br />
            <br />
            <uc1:GridviewPageController ID="pcTop10" runat="server" OnPageChange="PageSearchChange" Width="1140px" />
            <asp:GridView ID="gvMKT" runat="server" AutoGenerateColumns="False" Width="1140px" 
            GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
            EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" 
                ondatabound="gvMKT_DataBound" ShowFooter="true" OnRowDataBound ="gvMKT_RowDataBound"  >
                <Columns>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:Button ID="btnTransfer" runat="server" CssClass="Button" Width="70px" Text="โอนงาน" CommandArgument ='<%# Eval("Username") %>' OnClick="btnTransfer_Click"  OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <FooterTemplate ></FooterTemplate>
                        <ItemStyle Width="70px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        <FooterStyle Width="70px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate>
                            <asp:Label ID="lbRoleName" runat="server"  Width="100px"  Text='<%# Eval("RoleName") %>' ></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate ></FooterTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Left"  />
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <FooterStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="User ID">
                        <ItemTemplate>
                            <asp:Label ID="lbUsername" runat="server"  Width="120px"  Text='<%# Eval("Username") %>' ></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate ></FooterTemplate>
                        <ItemStyle Width ="120px" HorizontalAlign="Left"  />
                        <HeaderStyle Width ="120px" HorizontalAlign="Center" />
                        <FooterStyle Width ="120px" HorizontalAlign="Center"  />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="ชื่อ-นามสกุลพนักงาน">
                        <ItemTemplate>
                            <asp:Label ID="lbFullnameTH" runat="server"  Width="200px"  Text='<%# Eval("FullnameTH") %>' ></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate >Total</FooterTemplate>
                        <ItemStyle Width="200px" HorizontalAlign="Left"  />
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <FooterStyle Font-Bold="true" Width="200px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สถานะการทำงาน">
                        <ItemTemplate>
                            &nbsp;&nbsp;
                            <asp:Image ID="imgAvailable" runat="server" ImageUrl="~/Images/enable.gif" ImageAlign="AbsMiddle" Visible='<%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? true : false) : false %>' />
                            <asp:Image ID="imgNotAvailable" runat="server" ImageUrl="~/Images/disable.gif" ImageAlign="AbsMiddle" Visible='<%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? false : true) : false %>' />
                            &nbsp;
                            <%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? "Available" : "Unavailable") : "" %>
                        </ItemTemplate>
                         <FooterTemplate ></FooterTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Left"  />
                        <FooterStyle Width="120px" HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สนใจ">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount1" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_00")).ToString("#,##0") %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount1_Click" ></asp:LinkButton>
                        </ItemTemplate>
                         <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount1" runat="server" OnClick="lbSumAmount1_Click" ></asp:LinkButton>
                         </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รอติดต่อลูกค้า">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount15" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_15")).ToString("#,##0") %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount15_Click" ></asp:LinkButton>
                        </ItemTemplate>
                        <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount15" runat="server" OnClick="lbSumAmount15_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                        <asp:TemplateField HeaderText="อยู่ระหว่างดำเนินการ">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount14" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_14")).ToString("#,##0") %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount14_Click"  ></asp:LinkButton>
                        </ItemTemplate>
                        <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount14" runat="server" OnClick="lbSumAmount14_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รอผลการพิจารณา">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount5" runat="server" Text='<%#  Convert.ToInt32(Eval("SUM_STATUS_05")).ToString("#,##0") %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount5_Click" ></asp:LinkButton>
                        </ItemTemplate>
                         <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount5" runat="server" OnClick="lbSumAmount5_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                        <asp:TemplateField HeaderText="อนุมัติ<br>ตามเสนอ">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount6" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_06")).ToString("#,##0") %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount6_Click"  ></asp:LinkButton>
                        </ItemTemplate>
                         <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount6" runat="server" OnClick="lbSumAmount6_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ส่งกลับแก้ไข">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbAmount11" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_11")).ToString("#,##0")  %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbAmount11_Click"  ></asp:LinkButton>
                        </ItemTemplate>
                         <FooterTemplate >
                            <asp:LinkButton ID="lbSumAmount11" runat="server" OnClick="lbSumAmount11_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รวม Lead<br/>ที่อยู่ในมือ">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbTotal" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_TOTAL")).ToString("#,##0")  %>' CommandArgument ='<%# Eval("Username") %>' OnClick="lbTotal_Click"  ></asp:LinkButton>
                        </ItemTemplate>
                         <FooterTemplate >
                            <asp:LinkButton ID="lbSumTotal" runat="server" OnClick="lbSumTotal_Click" ></asp:LinkButton>
                        </FooterTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center"  />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <FooterStyle Width="80px" HorizontalAlign="Center"  />
                    </asp:TemplateField>
                   
                </Columns>
            <HeaderStyle CssClass="t_rowhead" />
            <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
        </asp:GridView>

        <asp:UpdatePanel ID="upPopMKT" runat="server" UpdateMode="Conditional">
            <ContentTemplate >
                <asp:Panel runat="server" ID="pnlPopMKT" CssClass="modalPopupMKTMonitoring" style="display:none;" >
                    <asp:Button ID="btnMKT" runat="server" CssClass="Hidden" />
                     <br />
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/hResult.gif" />
                    <br />  <br />
                    <asp:Panel ID="pnlTransferInfo" runat="server">
                        <table>
                            <tr>
                                <td style="width:50px"></td>
                                <td class="ColInfo">Branch :</td>
                                <td class="ColInput">
                                    <asp:DropDownList ID="cmbBranch" runat="server" Width="200px"  CssClass="Dropdownlist" AutoPostBack="True" 
                                    onselectedindexchanged="cmbBranch_SelectedIndexChanged" >
                                    </asp:DropDownList>
                                </td>
                                <td style="width:50px"></td>
                                <td class="ColInfo">คนที่ถูกโอนงาน :</td>
                                <td style="width:400px">
                                    <asp:DropDownList ID="cmbStaff" runat="server" Width="250px"  CssClass="Dropdownlist" >
                                    </asp:DropDownList>
                                    <asp:Label ID="vcmbOwner" runat="server" CssClass="style4"></asp:Label>
                                </td>
                            </tr>
                            <tr><td colspan="6" style="height:5px"></td></tr>
                            <tr>
                                <td style="width:50px"></td>
                                <td class="ColInfo"></td>
                                <td colspan="4">
                                    <asp:Button ID="btnTransferPopup" runat="server" CssClass="Button" Width="100px" Text="โอนงาน"  OnClientClick="DisplayProcessing()" onclick="btnTransferPopup_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <uc3:GridviewPageController ID="pcTop11" runat="server" OnPageChange="PageSearchChangePopupMKT" Width="920px" />
                    
                    <table cellpadding="2" cellspacing="0" border="0">
                        <tr>        
                            <td class="style1">                                                                                                                                                                                            
		                        <asp:Panel ID="Panel2"  runat="server" CssClass="modalPopupMKTMonitoring2" ScrollBars="Auto" >
                                <asp:GridView ID="gvPopMKT" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" BorderWidth="0px" 
                                    EnableModelValidation="True" Width="1830px" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"
                                     OnDataBound="gvPopMKT_DataBound"  OnRowDataBound ="gvPopMKT_RowDataBound" >
                                    <Columns>
                                     <asp:TemplateField HeaderText="โอนงาน">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkTransfer" runat="server" Width="50px" />
                                        </ItemTemplate>
                                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <HeaderStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Eval("TICKETID") %>' OnClick="imbView_Click" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง"  />
                                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("TICKETID") %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลผู้มุ่งหวัง"  />
                                        </ItemTemplate>
                                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <HeaderStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Doc">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbDoc" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Document.png" ToolTip="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("TicketId") %>' OnClientClick="DisplayProcessing()" />
                                        </ItemTemplate>
                                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <HeaderStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ticket ID">
                                        <ItemTemplate>
                                           <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("TICKETID") %>' Width="120px"></asp:Label> 
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Tranfer Type">
                                        <ItemTemplate>
                                           <asp:Label ID="lblTranferType" runat="server" Text='<%# Eval("TranferType") %>' Width="120px"></asp:Label> 
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead"  >
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DelegateName" HeaderText="Delegate"  >
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead"  >
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="สถานะย่อยของ Lead" DataField="SubStatusDesc">
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="แจ้งเตือนครั้งที่">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCounting" runat="server" Text='<%# Eval("Counting") != null ? Convert.ToDecimal(Eval("Counting")).ToString("#,##0") : "0" %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <HeaderStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ"  >
                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง"  >
                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CreatedDate" HeaderText="วันที่สร้าง Lead"  >
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:BoundField>
                                     <asp:BoundField DataField="AssignedDate" HeaderText="วันที่ได้รับมอบหมายล่าสุด"  >
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Firstname" HeaderText="ชื่อ"  >
                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Lastname" HeaderText="นามสกุล"  >
                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="ประเภทบุคคล">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="90px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top"/>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CITIZENID" HeaderText="เลขที่บัตร"  >
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:BoundField>
                                      <asp:TemplateField HeaderText="CAMPAIGNID">
                                        <ItemTemplate>
                                           <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>' Width="120px"></asp:Label> 
                                        </ItemTemplate>
                                        <ItemStyle CssClass="Hidden" />
                                        <HeaderStyle  CssClass="Hidden"  />
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
                                </Columns> 
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                                </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr style="height:35px;">
                            <td  align="right" class="style1">
                                <asp:Button ID="btnCloseMKT" runat="server" Text="ปิดหน้าจอ" Width="100px" onclick="btnCloseMKT_Click" OnClientClick="DisplayProcessing()"  />&nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>

                </asp:Panel>
                <act:ModalPopupExtender ID="mpePopupMKT" runat="server" TargetControlID="btnMKT" PopupControlID="pnlPopMKT" BackgroundCssClass="modalBackground" DropShadow="True">
	                </act:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Panel>
 </asp:Content>
    