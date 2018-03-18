<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_026.aspx.cs" Inherits="SLM.Application.SLM_SCR_026" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:120px;
        }
        .ColInput
        {
            width:250px;
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
                        ชื่อย่อ
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtPositionNameAbbSearch" runat="server" CssClass="Textbox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อเต็ม
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtPositionNameENSearch" runat="server" CssClass="Textbox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อตำแหน่ง
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtPositionNameTHSearch" runat="server" CssClass="Textbox" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:5px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:CheckBox ID="cbActiveSearch" runat="server" Text="ใช้งาน"  />&nbsp;
                        <asp:CheckBox ID="cbInActiveSearch" runat="server" Text="ไม่ใช้งาน"  />
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
            <asp:Button ID="btnAddPosition" runat="server" Text="เพิ่มตำแหน่ง" Width="150px" OnClientClick="DisplayProcessing();"
                CssClass="Button" Height="23px" onclick="btnAddPosition_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="900px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="900px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลตำแหน่ง" OnClientClick="DisplayProcessing();" />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อย่อ">
                        <ItemTemplate>
                            <asp:Label ID="lblPositionNameAbb" runat="server" Text='<%# Eval("PositionNameAbb") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อเต็ม">
                        <ItemTemplate>
                            <asp:Label ID="lblPositionNameEN" runat="server" Text='<%# Eval("PositionNameEN") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="270px" HorizontalAlign="Center"/>
                        <ItemStyle Width="270px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อตำแหน่ง">
                        <ItemTemplate>
                            <asp:Label ID="lblPositionNameTH" runat="server" Text='<%# Eval("PositionNameTH") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="270px" HorizontalAlign="Center"/>
                        <ItemStyle Width="270px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:TemplateField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะ"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblPositionId" runat="server" Text='<%# Eval("PositionId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
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
	        <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupPosition">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/Position.png" />
                <asp:TextBox ID="txtPositionId" runat="server" Width="50px" Visible="false" ></asp:TextBox>
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            ชื่อย่อ<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtPositionNameAbb" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
                            <br />
                            <asp:Label ID="alertPositionNameAbb" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            ชื่อเต็ม
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtPositionNameEN" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
                            <br />
                            <asp:Label ID="alertPositionNameEN" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            ชื่อตำแหน่ง<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtPositionNameTH" runat="server" CssClass="Textbox" Width="250px"></asp:TextBox>
                            <br />
                            <asp:Label ID="alertPositionNameTH" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:4px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            สถานะ
                        </td>
                        <td class="ColInput">
                            <asp:RadioButton ID="rbActive" runat="server" Text="ใช้งาน" GroupName="StatusPopup" Checked="true" />&nbsp;
                            <asp:RadioButton ID="rbInActive" runat="server" Text="ไม่ใช้งาน" GroupName="StatusPopup" />
                            <br />
                            <asp:Label ID="alertStatus" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:8px;"></td></tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                        </td>
                        <td class="ColInput">
                            <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" OnClientClick="DisplayProcessing();"
                                Width="100px" onclick="btnSave_Click" />&nbsp;
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
