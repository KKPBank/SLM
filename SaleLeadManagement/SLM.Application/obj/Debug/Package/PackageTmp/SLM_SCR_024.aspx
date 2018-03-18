<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_024.aspx.cs" Inherits="SLM.Application.SLM_SCR_024" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:180px;
        }
        .ColInput
        {
            width:250px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function ConfirmEditSave() {
            var cb = document.getElementById('<%= cbEdit.ClientID %>');
            var listbox_selected = document.getElementById('<%= lboxBranchSelected.ClientID %>');
            if (cb != null && cb.checked && listbox_selected != null) {
                var count = listbox_selected.options.length;
                if (count == 0) {
                    return confirm('ต้องการลบสาขาทั้งหมดของสิทธิ์การเข้าถึงข้อมูลนี้ ใช่หรือไม่');
                }
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
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:2px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbProduct" runat="server" Text="ผลิตภัณฑ์/บริการ" AutoPostBack="true" 
                            GroupName="SearchType" Checked="true" 
                            oncheckedchanged="rbProduct_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbCampaign" runat="server" Text="แคมเปญ" AutoPostBack="true" 
                            GroupName="SearchType" oncheckedchanged="rbCampaign_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCampaignSearch" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สาขา
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbBranchSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Role
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbStaffTypeSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:3px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" OnClientClick="DisplayProcessing();"
                            Width="100px" onclick="btnSearch_Click" />
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
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top"  />&nbsp;
            <asp:Button ID="btnAddConfig" runat="server" Text="เพิ่มสิทธิ์การเข้าถึงข้อมูล" Width="200px" OnClientClick="DisplayProcessing();" 
                CssClass="Button" Height="23px" onclick="btnAddConfig_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1000px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1000px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif"  OnClick="imbEdit_Click" ToolTip="แก้ไขสิทธิ์การเข้าถึงข้อมูล" CommandArgument='<%# Container.DisplayIndex %>' OnClientClick="DisplayProcessing();" />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ProductName" HeaderText="ผลิตภัณฑ์/บริการ"  >
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                        <HeaderStyle Width="300px" HorizontalAlign="Center"/>
                        <ItemStyle Width="300px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="BranchName" HeaderText="สาขา">
                        <HeaderStyle Width="300px" HorizontalAlign="Center"/>
                        <ItemStyle Width="300px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="StaffTypeDesc" HeaderText="Role">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblStaffTypeId" runat="server" Text='<%# Eval("StaffTypeId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupConfigAccessData">
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/Privilege.png" />
                    <asp:CheckBox ID="cbEdit" runat="server" Text="Edit" Enabled="false" CssClass="Hidden" />
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr><td colspan="6" style="height:1px;"></td></tr>
                        <tr style="vertical-align:top;">
                            <td style="width:10px;"></td>
                            <td class="ColInfo">
                                <asp:RadioButton ID="rbProductPopup" runat="server" Text="ผลิตภัณฑ์/บริการ" GroupName="PopupType" Checked="true" 
                                    AutoPostBack="true" OnCheckedChanged="rbProductPopup_CheckedChanged" /><asp:Label ID="lblProductStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbProductPopup" runat="server" CssClass="Dropdownlist" Width="250px" ></asp:DropDownList>
                            </td>
                            <td colspan="3"></td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:10px;"></td>
                            <td class="ColInfo">
                                <asp:RadioButton ID="rbCampaignPopup" runat="server" Text="แคมเปญ" GroupName="PopupType" 
                                    AutoPostBack="true" OnCheckedChanged="rbCampaignPopup_CheckedChanged"  /><asp:Label ID="lblCampaignStar" runat="server" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbCampaignPopup" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false" ></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertProductCampaign" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                            <td style="width:60px;">&nbsp;</td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;Role<asp:Label ID="lblStaffTypeStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                            </td>
                            <td class="ColInput">
                                 <asp:UpdatePanel ID="upStaffTypeSection" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="cmbStaffTypePopup" runat="server" CssClass="Dropdownlist" AutoPostBack="true" 
                                            Width="207px" onselectedindexchanged="cmbStaffTypePopup_SelectedIndexChanged"></asp:DropDownList>
                                        <br />
                                        <asp:Label ID="alertStaffTypePopup" runat="server" ForeColor="Red"></asp:Label>
                                    </ContentTemplate>
                                 </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr><td colspan="6" style="height:3px;"></td></tr>
                    </table>
                    <br />
                    <asp:UpdatePanel ID="upPopupBranchSection" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td style="width:330px; vertical-align:top; font-weight:bold;">สาขาทั้งหมด&nbsp;
                                        <asp:Label ID="lblBranchAllTotal" runat="server" ForeColor="#33a549"></asp:Label>&nbsp;สาขา
                                    </td>
                                    <td style="width:100px; vertical-align:middle;"></td>
                                    <td style="width:330px; vertical-align:top; font-weight:bold;">สาขาที่เลือก&nbsp;
                                        <asp:Label ID="lblBranchSelectedTotal" runat="server" ForeColor="#33a549"></asp:Label>&nbsp;สาขา&nbsp;
                                        <asp:Label ID="alertBranchSelected" runat="server" ForeColor="Red" Font-Bold="false"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td style="width:330px; vertical-align:top;">
                                        <asp:ListBox ID="lboxBranchAll" runat="server" Width="330px" Height="330px" SelectionMode="Multiple" >
                                        </asp:ListBox>
                                    </td>
                                    <td align="center" style="width:100px; vertical-align:middle;">
                                        <asp:Button ID="btnSelectAll" runat="server" CssClass="Button" Text=">>" Width="60px" onclick="btnSelectAll_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnSelect" runat="server" CssClass="Button" Text=">" Width="60px" onclick="btnSelect_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnDeselect" runat="server" CssClass="Button" Text="<" Width="60px" onclick="btnDeselect_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnDeselectAll" runat="server" CssClass="Button" Text="<<" Width="60px" onclick="btnDeselectAll_Click" OnClientClick="DisplayProcessing();" />
                                    </td>
                                    <td style="width:330px; vertical-align:top;">
                                        <asp:ListBox ID="lboxBranchSelected" runat="server" Width="330px" Height="330px" SelectionMode="Multiple" >
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td colspan="3">
                                        <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" OnClientClick="if (ConfirmEditSave()) { DisplayProcessing(); return true; } else { return false; }" 
                                            Width="100px" onclick="btnSave_Click" />
                                        <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                            Width="100px" onclick="btnCancel_Click" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
