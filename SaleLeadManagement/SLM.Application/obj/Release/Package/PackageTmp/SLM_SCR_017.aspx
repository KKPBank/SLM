<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_017.aspx.cs" Inherits="SLM.Application.SLM_SCR_017" %>
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
                        Windows Username
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtUsernameSearch" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        สาขา
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbBranchSearch" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        รหัสพนักงานธนาคาร
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtEmpCodeSearch" runat="server" CssClass="Textbox" Width="200px" MaxLength="6" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        รหัสเจ้าหน้าที่การตลาด
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtMarketingCodeSearch" runat="server" CssClass="Textbox" Width="200px" MaxLength="10" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อ-นามสกุลพนักงาน
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtStaffNameTHSearch" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        ตำแหน่ง
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbPositionSearch" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Role
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbStaffTypeSearch" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                        ทีมการตลาด
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTeamSearch" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        สาย
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbDepartmentSearch" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                    </td>
                    <td class="ColInfo"></td>
                    <td></td>
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
            <asp:Button ID="btnAddUser" runat="server" Text="เพิ่มพนักงาน" Width="130px" 
                CssClass="Button" Height="23px" onclick="btnAddUser_Click"  /><br /><br />
            <uc2:GridviewPageController ID="pcTop" runat="server" Width="1230px" OnPageChange="PageSearchChange" Visible="false" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1230px"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"  
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" >
                <Columns>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbAction" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("StaffId") %>' ToolTip="แก้ไขข้อมูลพนักงาน" OnClick="imbAction_Click" />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmpCode" HeaderText="รหัสพนักงาน<br/>ธนาคาร" HtmlEncode="false"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="MarketingCode" HeaderText="รหัสเจ้าหน้าที่<br/>การตลาด" HtmlEncode="false"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Username" HeaderText="Windows Username" >
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="ชื่อ-นามสกุลพนักงาน">
                        <ItemTemplate>
                            <%# Eval("StaffNameTH") != null ? Eval("StaffNameTH").ToString().Replace(" ", "&nbsp;") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                        <ItemStyle Width="180px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="PositionName" HeaderText="ตำแหน่ง" >
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="StaffTypeDesc" HeaderText="Role">
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Team" HeaderText="ทีมการตลาด">
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BranchName" HeaderText="สาขา">
                        <HeaderStyle Width="180px" HorizontalAlign="Center"/>
                        <ItemStyle Width="180px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DepartmentName" HeaderText="สาย">
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="สถานะ">
                        <ItemTemplate>
                            <%# Eval("Is_Deleted") != null ? (Eval("Is_Deleted").ToString() == "1" ? "ลาออก" : "ปกติ") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่แก้ไขสถานะล่าสุด">
                        <ItemTemplate>
                            <%# Eval("UpdateStatusDate") != null ? Convert.ToDateTime(Eval("UpdateStatusDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("UpdateStatusDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("UpdateStatusDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="90px" HorizontalAlign="Center"/>
                        <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>  
<%--    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupSCR017Add">
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgDetail" runat="server" ImageUrl="~/Images/AddUser.gif" />
            <br /><br />
		    <table cellpadding="2" cellspacing="0" border="0" >
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">Windows Username <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtUserId" runat="server" CssClass="Textbox" Width="150px" MaxLength="100" ></asp:TextBox>
                        <asp:Label ID="vtxtUserId" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">รหัสพนักงานธนาคาร <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtBankCode" runat="server" CssClass="Textbox" Width="150px" MaxLength="100" ></asp:TextBox>
                        <asp:Label ID="vtxtBankCode" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">รหัสเจ้าหน้าที่การตลาด <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtMarketCode" runat="server" CssClass="Textbox" Width="150px" MaxLength="100" ></asp:TextBox>
                        <asp:Label ID="vtxtMarketCode" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ชื่อ-นามสกุลพนักงาน <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtFullNameTh" runat="server" CssClass="Textbox" Width="260px" MaxLength="300" ></asp:TextBox><br />
                        <asp:Label ID="vtxtFullNameTh" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">เบอร์โทรศัพท์ </td>
                    <td class="style3">
                        <asp:TextBox ID="txtTelephone" runat="server" CssClass="Textbox" Width="100px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">E-mail <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="Textbox" Width="260px" 
                        MaxLength="100" AutoPostBack="True" ontextchanged="txtEmail_TextChanged" ></asp:TextBox><br />
                        <asp:Label ID="vtxtEmail" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                 <tr>
                    <td style="width:10px"></td>
                    <td class="style2">Role <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbRole" runat="server" CssClass="Dropdownlist" Width="262px"></asp:DropDownList><br />
                        <asp:Label ID="vcmbRole" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ทีมการตลาด <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtTeamMarket" runat="server" CssClass="Textbox" Width="260px" 
                        MaxLength="100" AutoPostBack="True" ontextchanged="txtEmail_TextChanged" ></asp:TextBox><br />
                        <asp:Label ID="vtxtTeamMarket" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr> 
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">สาขา <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbBranch" runat="server" CssClass="Dropdownlist" Width="262px"></asp:DropDownList><br />
                        <asp:Label ID="vcmbBranch" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">หัวหน้างาน</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbHeader" runat="server" CssClass="Dropdownlist" Width="262px"></asp:DropDownList>
                    </td>
                </tr>
                 <tr>
                    <td style="width:10px"></td>
                    <td class="style2">สถานะ <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:RadioButton ID="rdNormal" runat="server" GroupName="EmpStatus" Text="ปกติ" />
                        <asp:RadioButton ID="rdRetire" runat="server" GroupName="EmpStatus" Text="ลาออก" />&nbsp;&nbsp;&nbsp;
                         <asp:Label ID="vEmpStatus" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height:15px;">
                    </td>
                </tr>
                <tr style="height:35px;">
                    <td style="width:10px"></td>
                    <td class="style2"></td>
                    <td  class="style3" >
                        <asp:Button ID="btnSavePopup" runat="server" Text="บันทึก" Width="100px" onclick="btnSavePopup_Click"  OnClientClick="DisplayProcessing()" />&nbsp;
                        <asp:Button ID="btnClose" runat="server" Text="ยกเลิก" Width="100px" />
                    </td>
                </tr>
            </table>
	    </asp:Panel>
	        <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
