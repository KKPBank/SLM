<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_063.aspx.cs" Inherits="SLM.Application.SLM_SCR_063" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:160px;
        }
        .ColInput
        {
            width:250px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        .style1
        {
            width: 50px;
        }
        .style2
        {
            width: 200px;
            text-align:left;
            font-weight:bold;
        }
        .style3
        {
            width: 380px;
            text-align:left;
        }
        .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
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
                    <td class="ColInfo">
                        ชื่อยี่ห้อรถ
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSBrand" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbSStatus" runat="server" CssClass="Dropdownlist" Width="203px">
                            <asp:ListItem Value="">ทั้งหมด</asp:ListItem>
                            <asp:ListItem Value="0">ใช้งาน</asp:ListItem>
                            <asp:ListItem Value="1">ไม่ใช้งาน</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" style="height:15px;">
                    </td>
                </tr>
                 <tr>
                    <td class="ColInfo">
                    </td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" 
                            OnClientClick="DisplayProcessing()" Text="ค้นหา" onclick="btnSearch_Click"  />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" Text="ล้างข้อมูล" OnClick="btnClear_Click"  />
                    </td>
                </tr>
            </table><br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />

    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
            <asp:Button ID="btnAddTeam" runat="server" Text="เพิ่มข้อมูลรถ Premium" Width="150px" 
                CssClass="Button" Height="23px" onclick="btnAddTeam_Click" OnClientClick="DisplayProcessing()"  /><br /><br />
            <uc2:GridviewPageController ID="pcTop" runat="server" Width="1230px" OnPageChange="PageChange" Visible="false" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1230px"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"  
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" >
                <Columns>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbAction" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("slm_cc_pm_id") %>' ToolTip="แก้ไขข้อมูลทีม Telesales" OnClick="imbAction_Click" OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="slm_BrandName" HeaderText="ชื่อยี่ห้อรถ" HtmlEncode="false"  >
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="สถานะ">
                        <ItemTemplate>
                            <%# Eval("is_Deleted") == null ? "" : Eval("is_Deleted").ToString() == "False" ? "ใช้งาน" : "ไม่ใช้งาน" %>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่สร้าง">
                        <ItemTemplate>
                            <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่แก้ไข">
                        <ItemTemplate>
                            <%# Eval("slm_UpdatedDate") != null ? Convert.ToDateTime(Eval("slm_UpdatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_UpdatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("slm_UpdatedDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>  
<asp:UpdatePanel runat="server" ID="updModalMain">
    <ContentTemplate>

    <asp:LinkButton runat="server" ID="lnbTmpPopup"></asp:LinkButton>
    <act:ModalPopupExtender ID="zPopDetail" runat="server" BackgroundCssClass="modalBackground" TargetControlID="lnbTmpPopup" PopupControlID="pnlDetail" DropShadow="true" CancelControlID="btnCancel"></act:ModalPopupExtender>
    <asp:Panel runat="server" ID="pnlDetail" CssClass="modalPopupPromotion" style="height: 180px; padding: 10px; display:none;">
    <asp:UpdatePanel runat="server" ID="updModal" UpdateMode="Conditional">
        <ContentTemplate>
        <asp:HiddenField runat="server" ID="hdfID" />
        <table style="width:100%;"  >
            <tr>
                <td colspan="2">
                    <div class="box-header">ข้อมูลรถ Premium</div>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="ColInfo">ชื่อยี่ห้อรถ <span class="require">*</span></td>
                <td><asp:DropDownList runat="server" ID="cmbDBrand" CssClass="Textbox" Width="250px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="ColInfo"></td>
                <td></td>
            </tr>
            <tr>
                <td class="ColInfo">สถานะ <span class="require">*</span></td>
                <td>
                    <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rdoDStatus">
                        <asp:ListItem Value="False" Selected="True">ใช้งาน</asp:ListItem>
                        <asp:ListItem Value="True">ไม่ใช้งาน</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button runat="server" ID="btnSave" Text="บันทึก" CssClass="Button" Width="100px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing()" />
                    <asp:Button runat="server" ID="btnCancel" Text="ยกเลิก" CssClass="Button" Width="100px" OnClick="btnCancel_Click"/>
                </td>
            </tr>
        </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
