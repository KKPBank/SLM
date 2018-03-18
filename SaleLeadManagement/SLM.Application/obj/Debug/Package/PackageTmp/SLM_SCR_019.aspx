<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_019.aspx.cs" Inherits="SLM.Application.SLM_SCR_019" %>
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
            width: 900px;
            text-align: left;
        }
        .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
        .style5
        {
            width: 500px;
            text-align: left;
        }
        .phone-label {
            margin-left: 40px;
            margin-right: 10px;
            font-weight: bold;
        }
        .roleservice-label {
            margin-left: 20px;
            margin-right: 10px;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:UpdatePanel ID="upInfo" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0" >
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">Windows Username <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="Textbox" Width="260px" MaxLength="100" ></asp:TextBox>
                        <asp:Button ID="btnCheckUsername" runat="server" Text="ตรวจสอบ" Visible="false"
                            CssClass="Button" Width="80px" onclick="btnCheckUsername_Click" />
                        <asp:Label ID="vtxtUserName" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ประเภท User <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:RadioButton ID="rbnUserTypeIndividual" runat="server" GroupName="UserType" Text="Individual" Checked="True"/>
                        <asp:RadioButton ID="rbnUserTypeGroup" runat="server" GroupName="UserType" Text="Group (Dummy)" />
                        <asp:Label ID="vrbnUserType" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">รหัสพนักงานธนาคาร <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtEmpCode" runat="server" CssClass="Textbox" Width="150px" MaxLength="6" ></asp:TextBox>
                        <asp:Label ID="vtxtEmpCode" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">รหัสเจ้าหน้าที่การตลาด </span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtMarketingCode" runat="server" CssClass="Textbox" Width="150px" MaxLength="6" ></asp:TextBox>
                        <asp:Label ID="vtxtMarketingCode" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2"> </span></td>
                    <td class="style3">
                        <asp:CheckBox ID="chkMarketing" runat="server" Text="Marketing"/>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ชื่อ-นามสกุลพนักงาน <span class="style4">*</span></td>
                    <td class="style5">
                        <asp:TextBox ID="txtStaffNameTH" runat="server" CssClass="Textbox" Width="260px" MaxLength="100" ></asp:TextBox>
                        <asp:Label ID="vtxtStaffNameTH" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">เบอร์โทรศัพท์ #1</td>
                    <td class="style3">
                        <asp:TextBox ID="txtTellNo" runat="server" CssClass="Textbox" Width="100px" MaxLength="10" ></asp:TextBox>
                        <span class="phone-label">เบอร์โทรศัพท์ #2</span>
                        <asp:TextBox ID="txtTellNo2" runat="server" CssClass="Textbox" Width="100px" MaxLength="10" ></asp:TextBox>
                        <span class="phone-label">เบอร์โทรศัพท์ #3</span>
                        <asp:TextBox ID="txtTellNo3" runat="server" CssClass="Textbox" Width="100px" MaxLength="10" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">E-mail <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:TextBox ID="txtStaffEmail" runat="server" CssClass="Textbox" Width="260px" 
                        MaxLength="100" AutoPostBack="True" ontextchanged="txtEmail_TextChanged" ></asp:TextBox>
                        <asp:Label ID="vtxtStaffEmail" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ตำแหน่ง <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbPosition" runat="server" CssClass="Dropdownlist" Width="263px"></asp:DropDownList>      
                        <asp:Label ID="vtxtPositionName" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">Role Sale <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbStaffType" runat="server" CssClass="Dropdownlist" Width="263px"></asp:DropDownList>
                        <asp:Label ID="vcmbStaffType" runat="server" CssClass="style4"></asp:Label>
                        <span class="roleservice-label">Role Service <span class="style4">*</span></span>
                        <asp:DropDownList ID="cmbRoleService" runat="server" CssClass="Dropdownlist" Width="263px"></asp:DropDownList>
                        <asp:Label ID="vcmbRoleService" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">ทีมการตลาด </td>
                    <td class="style3">
                        <asp:TextBox ID="txtTeam" runat="server" CssClass="Textbox" Width="260px" MaxLength="100" ></asp:TextBox>
                        <asp:Label ID="vtxtTeam" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr> 
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">สาขาพนักงาน <span class="style4">*</span></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbBranchCode" runat="server" CssClass="Dropdownlist" 
                            Width="263px" ></asp:DropDownList>
                        <asp:Label ID="vcmbBranchCode" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">สาขาหัวหน้างาน</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbHeadBranchCode" runat="server" CssClass="Dropdownlist" 
                            Width="263px" AutoPostBack="True" 
                            onselectedindexchanged="cmbHeadBranchCode_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px"></td>
                    <td class="style2">หัวหน้างาน <asp:Label ID="lblHeadStaffId" runat="server" CssClass="style4"></asp:Label></td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbHeadStaffId" runat="server" CssClass="Dropdownlist" Width="263px" Enabled="false"></asp:DropDownList>
                        <asp:Label ID="vcmbHeadStaffId" runat="server" CssClass="style4"></asp:Label>
                    </td>
                </tr>
                 <tr>
                    <td style="width:10px"></td>
                    <td class="style2">สาย </td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbDepartment" runat="server" CssClass="Dropdownlist" Width="263px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px">&nbsp;</td>
                    <td class="style2">ยศ</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbLevel" runat="server" CssClass="Dropdownlist" Width="263px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px">&nbsp;</td>
                    <td class="style2">ประเภทพนักงาน</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbCategory" runat="server" CssClass="Dropdownlist" Width="263px" OnSelectedIndexChanged="cmbCategory_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="trCompany" visible="false">
                    <td style="width:10px">&nbsp;</td>
                    <td class="style2">ชื่อบริษัท</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbHost" runat="server" CssClass="Dropdownlist" Width="263px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:10px">&nbsp;</td>
                    <td class="style2">ทีม Telesales</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbTeamTelesale" runat="server" CssClass="Dropdownlist" Width="263px">
                        </asp:DropDownList>
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
                        <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="100px" onclick="btnSave_Click"  OnClientClick="if (confirm('ต้องการบันทึกข้อมูลใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />&nbsp;
                        <asp:Button ID="btnClose" runat="server" Text="ยกเลิก" Width="100px" onclick="btnClose_Click" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่')" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
