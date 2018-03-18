<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_043.aspx.cs" Inherits="SLM.Application.SLM_SCR_043" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:170px;
        }
        .ColInput
        {
            width:250px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        .modalPopupAddPremium
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:650px;
            height:316px;
        }
        .modalPopupEditPremium
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:950px;
            height:600px;
        }
        .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
        .modalPopupConfirm
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:300px;
            height:130px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height:2px;">
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbProductSearch" runat="server" Text="ผลิตภัณฑ์/บริการ" 
                            GroupName="Type" Checked="true" AutoPostBack="true" 
                            oncheckedchanged="rbProductSearch_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbCampaignSearch" runat="server" Text="แคมเปญ" 
                            GroupName="Type" AutoPostBack="true" 
                            oncheckedchanged="rbCampaignSearch_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCampaignSearch" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ประเภทของ Premium
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbPremiumTypeSearch" runat="server" CssClass="Dropdownlist" Width="250px">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                            <asp:ListItem Value="001" Text="สินค้า"></asp:ListItem>
                            <asp:ListItem Value="002" Text="Service"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ชื่อ Premium
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtPremiumSearch" runat="server" Width="400px" CssClass="Textbox"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:CheckBox ID="cbActiveSearch" runat="server" Text="ใช้งาน" />&nbsp;
                        <asp:CheckBox ID="cbInActiveSearch" runat="server" Text="ไม่ใช้งาน" />
                    </td>
                </tr>
            </table>   
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" style="height:5px"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td >
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
            <asp:Button ID="btnAddPremium" runat="server" Text="เพิ่มข้อมูล Premium" Width="150px" CssClass="Button" Height="23px" onclick="btnAddPremium_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1200px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" 
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  Width="1200px" >
                <Columns>
                <asp:TemplateField HeaderText="Action" >
                    <ItemTemplate>
                        <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>'  ToolTip="แก้ไขข้อมูล Premium" OnClick="imbEdit_Click" />
                    </ItemTemplate>
                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                 <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                    <ItemTemplate>
                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="แคมเปญ">
                    <ItemTemplate>
                        <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ชื่อ Premium">
                    <ItemTemplate>
                        <asp:Label ID="lblPremiumName" runat="server" Text='<%# Eval("PremiumName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Lot No">
                    <ItemTemplate>
                        <asp:Label ID="lblLotNo" runat="server" Text='<%# Eval("LotNo") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภท<br/>ของPremium">
                    <ItemTemplate>
                        <asp:Label ID="lblPremiumTypeName" runat="server" Text='<%# Eval("PremiumType").ToString() == "001" ? "สินค้า" : (Eval("PremiumType").ToString() == "002" ? "Service" : "") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                    <ItemTemplate>
                        <asp:Label ID="lblStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ( Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() ) : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                    <ItemTemplate>
                        <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ( Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() ) : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="จำนวน">
                    <ItemTemplate>
                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("TotalAll") != null ? ( Convert.ToInt32(Eval("TotalAll")).ToString("#,##0") ) : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="จำนวน<br/>ที่ใช้ไป">
                    <ItemTemplate>
                        <asp:Label ID="lblUsed" runat="server" Text='<%# Eval("TotalUsed") != null ? ( Convert.ToInt32(Eval("TotalUsed")).ToString("#,##0") ) : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="คงเหลือ">
                    <ItemTemplate>
                        <asp:Label ID="lblRemain" runat="server" Text='<%# Eval("TotalRemain") != null ? ( Convert.ToInt32(Eval("TotalRemain")).ToString("#,##0") ) : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="สถานะ">
                    <ItemTemplate>
                        <asp:Label ID="lblIsActive" runat="server" Text='<%# Eval("Status") != null ? (Convert.ToBoolean(Eval("Status")) ? "ใช้งาน" : "ไม่ใช้งาน") : "" %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID" >
                    <ItemTemplate>
                        <asp:Label ID="lblPremiumId" runat="server" Text='<%# Eval("PremiumId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden"  />
                    <FooterStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ProductId" >
                    <ItemTemplate>
                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden"  />
                    <FooterStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CampaignId" >
                    <ItemTemplate>
                        <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden"  />
                    <FooterStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PremiumType" >
                    <ItemTemplate>
                        <asp:Label ID="lblPremiumType" runat="server" Text='<%# Eval("PremiumType") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden"  />
                    <FooterStyle CssClass="Hidden" />
                    <ControlStyle CssClass="Hidden" />
                </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button runat="server" ID="btnPopupPremium" Width="0px" CssClass="Hidden"/>
	<asp:Panel runat="server" ID="pnPopupPremium" style="display:none" CssClass="modalPopupAddPremium">
        <br /><br />
        <asp:UpdatePanel ID="upnPopupPremium" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            <asp:RadioButton ID="rbProductAdd" runat="server" Text="ผลิตภัณฑ์/บริการ" GroupName="AddType" Checked="true" AutoPostBack="true" 
                                oncheckedchanged="rbProductAdd_CheckedChanged" />
                            <asp:Label ID="lblProductStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbProductAdd" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            <asp:RadioButton ID="rbCampaignAdd" runat="server" Text="แคมเปญ" 
                                GroupName="AddType" AutoPostBack="true" 
                                oncheckedchanged="rbCampaignAdd_CheckedChanged" /><asp:Label ID="lblCampaignStar" runat="server" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbCampaignAdd" runat="server" CssClass="Dropdownlist" Width="203px" Enabled="false"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height:10px"></td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ชื่อ Premium<span class="style4">*</span></td>
                        <td>
                            <asp:TextBox ID="txtPremiumNameAdd" runat="server" CssClass="Textbox" Width="400px" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ประเภทของที่แจก<span class="style4">*</span></td>
                        <td >
                            <asp:DropDownList ID="cmbPremiumTypeAdd" runat="server" CssClass="Dropdownlist" Width="203px">
                                <asp:ListItem Value ="001" Text="สินค้า"></asp:ListItem>
                                <asp:ListItem Value ="002" Text="Service"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;วันที่เริ่มต้น<span class="style4">*</span></td>
                        <td>
                            <uc2:TextDateMask ID="tdmStartDateAdd" runat="server" Width="100px" />
                            <asp:Label ID="alrtStartDateAdd" runat="server" ForeColor="Red" ></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;วันที่สิ้นสุด<span class="style4">*</span></td>
                        <td>
                            <uc2:TextDateMask ID="tdmEndDateAdd" runat="server" Width="100px" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;จำนวน<span class="style4">*</span></td>
                        <td >
                                <asp:TextBox ID="txtTotalAdd" runat="server" CssClass="TextboxR" Width="100px" ></asp:TextBox><br />
                                <asp:Label ID="alertTotalAdd" runat="server" ForeColor="Red" ></asp:Label>
                        </td>
                    </tr>
                        <tr>
                        <td style="width:20px;"></td>
                        <td style="width:160px; font-weight:bold;">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ<span class="style4">*</span></td>
                        <td >
                            <asp:RadioButton ID="rbActiveAdd" runat="server" GroupName ="Status" Text="ใช้งาน" Checked="true" />
                            <asp:RadioButton ID="rbNoActiveAdd" runat="server" GroupName ="Status" Text="ไม่ใช้งาน" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height:10px"></td>
                    </tr>
                    <tr >
                        <td style="width:20px;"></td>
                            <td style="width:160px;"></td>
                        <td>
                            <asp:Button ID="btnPopupSave" runat="server" Text="บันทึก" Width="100px" OnClick="btnPopupSave_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                            <asp:Button ID="btnPopupCancel" runat="server" Text="ยกเลิก" Width="100px" OnClick="btnPopupCancel_Click"  />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
	</asp:Panel>
	<act:ModalPopupExtender ID="mpePopupPremium" runat="server" TargetControlID="btnPopupPremium" PopupControlID="pnPopupPremium" BackgroundCssClass="modalBackground" DropShadow="True">
	</act:ModalPopupExtender>
    
    <asp:Button runat="server" ID="btnPopupEditPremium" Width="0px" CssClass="Hidden"/>
	<asp:Panel runat="server" ID="pnPopupEditPremium" style="display:none" CssClass="modalPopupEditPremium">
        <asp:UpdatePanel ID="upnPopupEditPremium" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/label_premium.png"  />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td colspan="3" style="height:2px;"></td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:130px; font-weight:bold;">
                            <asp:Label ID="lblEditTitle" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEditTitle" runat="server" CssClass="TextboxView" ReadOnly="true" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td style="width:130px; font-weight:bold;">ชื่อ Premium</td>
                        <td>
                            <asp:TextBox ID="txtEditPremiumName" runat="server" CssClass="TextboxView" ReadOnly="true" Width="400px"></asp:TextBox>
                            <asp:TextBox ID="txtEditProductId" runat="server" Width="40px" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="txtEditCampaignId" runat="server" Width="40px" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="txtEditPremiumTypeCode" runat="server" Width="40px" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                </table>
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="height:12px;"></td>
                    </tr>
                    <tr>
                        <td style="padding-left:15px;">
                            <uc1:GridviewPageController ID="pcTopForEdit" runat="server" OnPageChange="PageSearchChangeForEdit" Width="910px" />
                            <asp:GridView ID="gvEditPremium" runat="server" AutoGenerateColumns="False" 
                                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  Width="910px" >
                                <Columns>
                                <asp:TemplateField HeaderText="สถานะ" >
                                    <ItemTemplate>
                                        <asp:RadioButton ID="rdActive" runat="server" GroupName="Status" Text="ใช้งาน" ValidationGroup='<%# Eval("PremiumId") %>'  Checked='<%# Eval("Status") != null ? ( Convert.ToBoolean(Eval("Status")) ? true : false ) : false  %>' OnCheckedChanged="rdActive_CheckedChanged" AutoPostBack="true"  />
                                        <asp:RadioButton ID="rdNoActive" runat="server" GroupName="Status" Text="ไม่ใช้งาน" ValidationGroup='<%# Eval("PremiumId") %>' Checked='<%# Eval("Status") != null ? ( Convert.ToBoolean(Eval("Status")) ? false : true ) : true  %>' OnCheckedChanged="rdNoActive_CheckedChanged" AutoPostBack="true"  />
                                    </ItemTemplate>
                                    <ItemStyle Width="140px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="140px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่สร้าง">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate") != null ? ( Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Lot No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLotNo" runat="server" Text='<%# Eval("LotNo") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ประเภท<br/>ของPremium">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPremiumType" runat="server" Text='<%# Eval("PremiumType").ToString() == "001" ? "สินค้า" : (Eval("PremiumType").ToString() == "002" ? "Service" : "") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่เริ่มต้น">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ( Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่สิ้นสุด">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ( Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="จำนวน">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("TotalAll") != null ? ( Convert.ToInt32(Eval("TotalAll")).ToString("#,##0") ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="จำนวน<br/>ที่ใช้ไป">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUsed" runat="server" Text='<%# Eval("TotalUsed") != null ? ( Convert.ToInt32(Eval("TotalUsed")).ToString("#,##0") ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="คงเหลือ">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRemain" runat="server" Text='<%# Eval("TotalRemain") != null ? ( Convert.ToInt32(Eval("TotalRemain")).ToString("#,##0") ) : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Right" VerticalAlign="Top" />
                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ID" >
                                    <ItemTemplate>
                                        <asp:Label ID="lblPremiumId" runat="server" Text='<%# Eval("PremiumId") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="Hidden" />
                                    <HeaderStyle CssClass="Hidden"  />
                                    <FooterStyle CssClass="Hidden" />
                                    <ControlStyle CssClass="Hidden" />
                                </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead" />
                                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <table cellpadding="2" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td align="right" style="padding-right:26px;">
                            <asp:Button ID="btnEditPopupClose" runat="server" CssClass="Button" Width="90px" Text="ปิดหน้าต่าง" OnClick="btnEditPopupClose_Click" OnClientClick="DisplayProcessing();" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
	</asp:Panel>
	<act:ModalPopupExtender ID="mpePopupEditPremium" runat="server" TargetControlID="btnPopupEditPremium" PopupControlID="pnPopupEditPremium" BackgroundCssClass="modalBackground" DropShadow="True">
	</act:ModalPopupExtender>

    <asp:Button runat="server" ID="btnPopupConfirm" Width="0px" CssClass="Hidden"/>
	<asp:Panel runat="server" ID="pnPopupConfirm" style="display:none" CssClass="modalPopupConfirm">
        <asp:UpdatePanel ID="upConfirm" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="2" style="height:5px;"></td></tr>
                    <tr>
                        <td style="width:80px;"></td>
                        <td class="ColInfo" style="font-size:12px; width:380px;">
                            ต้องการเปลี่ยนสถานะใช่หรือไม่
                            <asp:TextBox ID="txtEditRowIndex" runat="server" Width="40px" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr><td colspan="2" style="height:25px;"></td></tr>
                    <tr>
                        <td style="width:80px;"></td>
                        <td class="ColInfo">
                            <asp:Button ID="btnConfirmYes" runat="server" Text="ใช่" CssClass="Button" OnClientClick="DisplayProcessing();"
                                Width="100px" OnClick="btnConfirmYes_Click" />&nbsp;
                            <asp:Button ID="btnConfirmNo" runat="server" Text="ไม่ใช่" CssClass="Button" 
                                Width="100px" OnClick="btnConfirmNo_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <act:ModalPopupExtender ID="mpePopupConfirm" runat="server" TargetControlID="btnPopupConfirm" PopupControlID="pnPopupConfirm" BackgroundCssClass="modalBackground modalBackgroundProcessing" DropShadow="True">
	</act:ModalPopupExtender>
</asp:Content>
