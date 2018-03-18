<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_027.aspx.cs" Inherits="SLM.Application.SLM_SCR_027" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>
<%@ Register src="Shared/Calendar.ascx" tagname="Calendar" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:130px;
        }
        .ColInput
        {
            width:600px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function ConfirmEditSave() {
            var cb = document.getElementById('<%= cbEdit.ClientID %>');
            var listbox_selected = document.getElementById('<%= lboxBranchSelected.ClientID %>');
            if (cb != null && cb.checked && listbox_selected != null) {
                var count = listbox_selected.options.length;
                if (count == 0) {
                    return confirm('ต้องการลบสาขาทั้งหมดของวันหยุดนี้ ใช่หรือไม่');
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
                        วันหยุด
                    </td>
                    <td class="ColInput">
                        <uc2:TextDateMask ID="tdmHolidayDateSearch" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        รายละเอียดวันหยุด
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtHolidayDescSearch" runat="server" CssClass="Textbox" Width="246px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สาขา
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbBranchSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing();" />
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
            <asp:Button ID="btnAddBranchHoliday" runat="server" Text="เพิ่มวันหยุดสาขา" Width="150px" OnClientClick="DisplayProcessing();" 
                CssClass="Button" Height="23px" onclick="btnAddBranchHoliday_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="920px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="920px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลวันหยุดสาขา" OnClientClick="DisplayProcessing();" />
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
                    <asp:TemplateField HeaderText="วันหยุด">
                        <ItemTemplate>
                            <asp:Label ID="lblHolidayDate" runat="server" Text='<%# Eval("HolidayDate") != null ? Convert.ToDateTime(Eval("HolidayDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("HolidayDate")).Year.ToString() : ""%>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รายละเอียดวันหยุด">
                        <ItemTemplate>
                            <asp:Label ID="lblHolidayDesc" runat="server" Text='<%# Eval("HolidayDesc") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="350px" HorizontalAlign="Center"/>
                        <ItemStyle Width="350px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="BranchName" HeaderText="สาขา"  >
                        <HeaderStyle Width="350px" HorizontalAlign="Center"/>
                        <ItemStyle Width="350px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupBranchHoliday">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/BranchHoliday.png" />
                <asp:CheckBox ID="cbEdit" runat="server" Text="Edit" Enabled="false" CssClass="Hidden" />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            วันหยุด<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <uc3:Calendar ID="tdmHolidayDatePopup" runat="server" HideClearButton="true" />&nbsp;
                            <asp:Label ID="alertHolidayDatePopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            รายละเอียดวันหยุด<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtHolidayDescPopup" runat="server" CssClass="Textbox" Width="193px"></asp:TextBox>&nbsp;
                            <asp:Label ID="alertHolidayDescPopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:3px;"></td></tr>
                </table>
                <br />
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
                            <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" Width="100px" onclick="btnSave_Click" OnClientClick="if (ConfirmEditSave()) { DisplayProcessing(); return true; } else { return false; }" />
                            <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                Width="100px" onclick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
