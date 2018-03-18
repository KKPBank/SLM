<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_025.aspx.cs" Inherits="SLM.Application.SLM_SCR_025" %>
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
            width:500px;
        }
    </style>
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
                        ผลิตภัณฑ์/บริการ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สถานะ Lead
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbLeadStatusSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สิทธิ์การบันทึกผลการติดต่อ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbActivityRightSearch" runat="server" CssClass="Dropdownlist" Width="250px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="มีสิทธิ์" Value="1"></asp:ListItem>
                            <asp:ListItem Text="ไม่มีสิทธิ์" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สถานะที่เลือกได้
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbLeadStatusAvailableSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:3px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px" OnClientClick="DisplayProcessing();" onclick="btnSearch_Click" />
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
            <asp:Button ID="btnAddCondition" runat="server" Text="เพิ่มเงื่อนไขการบันทึกผลการติดต่อ" Width="220px" OnClientClick="DisplayProcessing();" 
                CssClass="Button" Height="23px" onclick="btnAddCondition_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="970px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="970px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" ToolTip="แก้ไขเงื่อนไขการบันทึกผลการติดต่อ" OnClientClick="DisplayProcessing();" />
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
                    <asp:BoundField DataField="LeadStatusDesc" HeaderText="สถานะ Lead">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="HaveRightAddDesc" HeaderText="สิทธิ์">
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="LeadAvailableStatusDesc" HeaderText="สถานะที่เลือกได้">
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
                            <asp:Label ID="lblLeadStatusCode" runat="server" Text='<%# Eval("LeadStatusCode") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblHaveRightAdd" runat="server" Text='<%# Eval("HaveRightAdd") != null ? (Convert.ToBoolean(Eval("HaveRightAdd")) == true ? "1" : "0") : "" %>'></asp:Label>
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
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupConditionActivity">
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/ResultContact.png" />
                    <asp:CheckBox ID="cbEdit" runat="server" Text="Edit" Enabled="false" Visible="false" />
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr><td colspan="3" style="height:1px;"></td></tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                ผลิตภัณฑ์/บริการ<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbProductPopup" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>&nbsp;
                                <asp:Label ID="alertProductPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                สถานะ Lead<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbLeadStatusPopup" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>&nbsp;
                                <asp:Label ID="alertLeadStatusPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:UpdatePanel ID="upPopupBranchSection" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr>
                                    <td style="width:20px;">&nbsp;</td>
                                    <td class="ColInfo">
                                        สิทธิ์การบันทึกผลการติดต่อ<span style="color:Red;">*</span>
                                    </td>
                                    <td class="ColInput">
                                        <asp:DropDownList ID="cmbActivityRightPopup" runat="server" AutoPostBack="true"
                                            CssClass="Dropdownlist" Width="250px" onselectedindexchanged="cmbActivityRightPopup_SelectedIndexChanged">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="มีสิทธิ์" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="ไม่มีสิทธิ์" Value="0"></asp:ListItem>
                                        </asp:DropDownList>&nbsp;
                                        <asp:Label ID="alertActivityRightPopup" runat="server" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr><td colspan="3" style="height:3px;"></td></tr>
                            </table>
                            <br />
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td style="width:330px; vertical-align:top; font-weight:bold;">สถานะทั้งหมด&nbsp;
                                        <asp:Label ID="lblLeadStatusAllTotal" runat="server" ForeColor="#33a549"></asp:Label>&nbsp;สถานะ
                                    <td style="width:100px; vertical-align:middle;"></td>
                                    <td style="width:330px; vertical-align:top; font-weight:bold;">สถานะที่เลือก&nbsp;
                                        <asp:Label ID="lblLeadStatusSelectedTotal" runat="server" ForeColor="#33a549"></asp:Label>&nbsp;สถานะ&nbsp;
                                        <asp:Label ID="alertLeadStatusSelected" runat="server" ForeColor="Red" Font-Bold="false"></asp:Label>
                                </tr>
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td style="width:330px; vertical-align:top;">
                                        <asp:ListBox ID="lboxLeadStatusAll" runat="server" Width="330px" Height="300px" SelectionMode="Multiple" >
                                        </asp:ListBox>
                                    </td>
                                    <td align="center" style="width:100px; vertical-align:middle;">
                                        <asp:Button ID="btnSelectAll" runat="server" CssClass="Button" Text=">>" Width="60px" onclick="btnSelectAll_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnSelect" runat="server" CssClass="Button" Text=">" Width="60px" onclick="btnSelect_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnDeselect" runat="server" CssClass="Button" Text="<" Width="60px" onclick="btnDeselect_Click" OnClientClick="DisplayProcessing();" />
                                        <asp:Button ID="btnDeselectAll" runat="server" CssClass="Button" Text="<<" Width="60px" onclick="btnDeselectAll_Click" OnClientClick="DisplayProcessing();" />
                                    </td>
                                    <td style="width:330px; vertical-align:top;">
                                        <asp:ListBox ID="lboxLeadStatusSelected" runat="server" Width="330px" Height="300px" SelectionMode="Multiple" >
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:20px;"></td>
                                    <td colspan="3">
                                        <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" Width="100px" onclick="btnSave_Click" OnClientClick="DisplayProcessing();" />
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
