<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_037.aspx.cs" Inherits="SLM.Application.SLM_SCR_037" %>

<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController"
    TagPrefix="uc1" %>
<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc2" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController"
    TagPrefix="uc3" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController"
    TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight: bold;
            width: 120px;
        }
        .ColInput
        {
            width: 200px;
        }
        .ColInfoMKT
        {
            font-weight: bold;
            width: 150px;
        }
        .ColInputMKT
        {
            width: 280px;
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
    <asp:Panel ID="pnlMKT" runat="server">
        <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/MonitoringTitle5.png" ImageAlign="Top" />&nbsp;
        <br />
        <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table cellpadding="3" cellspacing="0">
                    <tr>
                        <td colspan="4" style="height: 10px">
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">
                           กลุ่มผลิตภัณฑ์
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbProductGroup" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td class="ColInfo">
                            ผลิตภัณฑ์
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbProduct" runat="server" CssClass="Dropdownlist" Width="250px" 
                            AutoPostBack ="true" OnSelectedIndexChanged ="cmbProduct_SelectedIndexChanged" >
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoMKT">
                            แคมเปญ
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="250px" >
                            </asp:DropDownList>
                            <asp:TextBox ID="txtStaffTypeIdLogin" runat="server" CssClass ="Hidden" ></asp:TextBox>
                        </td>
                        <td class="ColInfo">
                            ทีม Telesales
                        </td>
                        <td class="ColInputMKT">
                            <asp:DropDownList ID="cmbTeamtelesales" runat="server" CssClass="Dropdownlist" Width="250px" ></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoMKT">
                            สาขา
                        </td>
                        <td class="ColInputMKT">
                            <asp:DropDownList ID="cmbSearchBranch" runat="server" CssClass="Dropdownlist" Width="250px">
                            </asp:DropDownList>
                        </td>
                        <td class="ColInfo">
                            สถานะการทำงาน
                        </td>
                        <td class="ColInputMKT">
                            <asp:DropDownList ID="cmbStatusMKT" runat="server" Width="250px" CssClass="Dropdownlist">
                                <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
                                <asp:ListItem Text="Unavailable" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Available" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoMKT">
                            วันที่จ่ายงาน
                        </td>
                        <td class="ColInputMKT">
                            <uc2:TextDateMask ID="tdMKTAssighDateFrom" runat="server" />
                        </td>
                        <td class="ColInfo">
                            ถึง
                        </td>
                        <td class="ColInputMKT">
                            <uc2:TextDateMask ID="tdMKTAssighDateTo" runat="server" />
                        </td>
                    </tr>
                    
                </table>
                <br />
                <asp:Table runat="server" CellPadding="3" CellSpacing="0" border="0" ID="tblSubOption">
                    <asp:TableRow ID="TableRow1" runat="server">
                        <asp:TableCell ID="TableCell1" runat="server" valign="top" class="ColInfoMKT">
                        สถานะย่อยของ Lead
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell2" runat="server" colspan="5">
                            <asp:CheckBox ID="cbSubOptionAll" runat="server" Text="ทั้งหมด" AutoPostBack="true"
                                OnCheckedChanged="cbSubOptionAll_CheckedChanged" />
                            <asp:CheckBoxList ID="cbSubOptionList" runat="server" RepeatLayout="Table" AutoPostBack="true"
                                RepeatDirection="Horizontal" RepeatColumns="5">
                            </asp:CheckBoxList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow2" runat="server">
                        <asp:TableCell ID="TableCell3" runat="server" colspan="6" Style="height: 15px;">
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
                <table>
                    <tr>
                        <td class="ColInfoMKT">
                        </td>
                        <td style="vertical-align:top;">
                            <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px"
                                OnClientClick="DisplayProcessing()" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnExport" runat="server" CssClass="Button" Text="Export Excel" Width="100px"
                                OnClientClick="DisplayProcessing()" OnClick="btnExport_Click"/>
                            <asp:Button ID="btnImport" runat="server" CssClass="Button" Text="Import Excel" Width="100px"
                                OnClientClick="DisplayProcessing()" OnClick="btnImport_Click"/>

                            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtDateTo" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtIsActive" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtRecursive" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtStatuscode" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtListUsernameFooter" runat="server" CssClass="Hidden" ReadOnly="true"></asp:TextBox>
                            <asp:TextBox ID="txtStaffId" runat="server" CssClass="Hidden" Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtStaffType" runat="server" CssClass="Hidden" Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtPopupFlag" runat="server" CssClass="Hidden" Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtEmpCode" runat="server" CssClass="Hidden" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <uc1:GridviewPageController ID="pcTop10" runat="server" OnPageChange="PageSearchChange"
                    Width="1190px" />
                <asp:GridView ID="gvMKT" runat="server" AutoGenerateColumns="False" Width="1510px"
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"
                    OnDataBound="gvMKT_DataBound" ShowFooter="true" OnRowDataBound="gvMKT_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="">
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkExportAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkExportAll_CheckedChanged" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExport" runat="server" Width="50px" />
                            </ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                            <HeaderStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                            <FooterStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Role">
                            <ItemTemplate>
                                <asp:Label ID="lbRoleName" runat="server" Width="100px" Text='<%# Eval("RoleName") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle Width="100px" HorizontalAlign="Left" />
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <FooterStyle Width="100px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User ID">
                            <ItemTemplate>
                                <asp:Label ID="lbUsername" runat="server" Width="120px" Text='<%# Eval("Username") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle Width="120px" HorizontalAlign="Left" />
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <FooterStyle Width="120px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อ-นามสกุลพนักงาน">
                            <ItemTemplate>
                                <asp:Label ID="lbFullnameTH" runat="server" Width="200px" Text='<%# Eval("FullnameTH") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                Total</FooterTemplate>
                            <ItemStyle Width="200px" HorizontalAlign="Left" />
                            <HeaderStyle Width="200px" HorizontalAlign="Center" />
                            <FooterStyle Font-Bold="true" Width="200px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะการทำงาน">
                            <ItemTemplate>
                                &nbsp;&nbsp;
                                <asp:Image ID="imgAvailable" runat="server" ImageUrl="~/Images/enable.gif" ImageAlign="AbsMiddle"
                                    Visible='<%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? true : false) : false %>' />
                                <asp:Image ID="imgNotAvailable" runat="server" ImageUrl="~/Images/disable.gif" ImageAlign="AbsMiddle"
                                    Visible='<%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? false : true) : false %>' />
                                &nbsp;
                                <%# Eval("Active") != null ? (Eval("Active").ToString().Trim() == "1" ? "Available" : "Unavailable") : "" %>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            <ItemStyle Width="120px" HorizontalAlign="Left" />
                            <FooterStyle Width="120px" HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เสนอขาย">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount1" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_01")).ToString("#,##0") %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount1_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount1" runat="server" OnClick="lbSumAmount1_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ขอปรึกษาที่บ้านก่อน">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount2" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_02")).ToString("#,##0") %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount2_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount2" runat="server" OnClick="lbSumAmount2_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ขอเลือกบริษัทประกันก่อน">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount3" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_03")).ToString("#,##0") %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount3_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount3" runat="server" OnClick="lbSumAmount3_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ฝากให้โทรกลับ">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount4" runat="server" Text='<%#  Convert.ToInt32(Eval("SUM_STATUS_04")).ToString("#,##0") %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount4_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount4" runat="server" OnClick="lbSumAmount4_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="อยู่ระหว่างตัดสินใจ">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount5" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_05")).ToString("#,##0") %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount5_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount5" runat="server" OnClick="lbSumAmount5_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ปิดเครื่อง/ไม่มีสัญญาณ">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount10" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_10")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount10_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount10" runat="server" OnClick="lbSumAmount10_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สายเสีย">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount11" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_11")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount11_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount11" runat="server" OnClick="lbSumAmount11_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สายไม่ว่าง">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount12" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_12")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount12_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount12" runat="server" OnClick="lbSumAmount12_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เบอร์ผิด/เปลี่ยนเบอร์แล้ว">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount13" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_13")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount13_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount13" runat="server" OnClick="lbSumAmount13_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ไม่มีผู้รับสาย">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbAmount14" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_STATUS_14")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbAmount14_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumAmount14" runat="server" OnClick="lbSumAmount14_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รวม Lead ที่อยู่ในมือ">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbTotal" runat="server" Text='<%# Convert.ToInt32(Eval("SUM_TOTAL")).ToString("#,##0")  %>'
                                    CommandArgument='<%# Eval("Username") %>' OnClick="lbTotal_Click"></asp:LinkButton>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbSumTotal" runat="server" OnClick="lbSumTotal_Click" OnClientClick="if (this.innerText != '0') { DisplayProcessing(); }"></asp:LinkButton>
                            </FooterTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                </asp:GridView>
                <asp:UpdatePanel ID="upPopMKT" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlPopMKT" CssClass="modalPopupMKTMonitoring">
                            <asp:Button ID="btnMKT" runat="server" CssClass="Hidden" />
                            <br />
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/hResult.gif" />
                            <br />
                            <br />
                            <br />
                            <uc3:GridviewPageController ID="pcTop11" runat="server" OnPageChange="PageSearchChangePopupMKT"
                                Width="920px" />
                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr>
                                    <td class="style1">
                                        <asp:Panel ID="Panel2" runat="server" CssClass="modalPopupMKTMonitoring2" ScrollBars="Auto">
                                            <asp:GridView ID="gvPopMKT" runat="server" AutoGenerateColumns="False" GridLines="Horizontal"
                                                BorderWidth="0px" EnableModelValidation="True" Width="1610px" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"
                                                OnDataBound="gvPopMKT_DataBound" OnRowDataBound="gvPopMKT_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Eval("PreleadId") %>'
                                                                OnClick="imbView_Click" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง" />
                                                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("PreleadId") %>'
                                                                OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลผู้มุ่งหวัง" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <HeaderStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Doc">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imbDoc" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Document.png"
                                                                ToolTip="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("PreleadId") %>'
                                                                OnClientClick="DisplayProcessing()" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <HeaderStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="เลขที่สัญญา">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPreleadId" runat="server" Text='<%# Eval("ContractNo") %>' Width="120px"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Tranfer Type">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTranferType" runat="server" Text='<%# Eval("TranferType") %>' Width="120px"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead">
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="SubStatusDesc" HeaderText="สถานะย่อยของ Lead">
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="แจ้งเตือนครั้งที่">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCounting" runat="server" Text='<%# Eval("Counting") != null ? Convert.ToDecimal(Eval("Counting")).ToString("#,##0") : "0" %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <HeaderStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CreatedDate" HeaderText="วันที่สร้าง Lead">
                                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="AssignedDate" HeaderText="วันที่ได้รับมอบหมายล่าสุด">
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Firstname" HeaderText="ชื่อ">
                                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Lastname" HeaderText="นามสกุล">
                                                        <HeaderStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="ประเภทบุคคล">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="90px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                        <HeaderStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CITIZENID" HeaderText="เลขที่บัตร">
                                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="CAMPAIGNID">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>' Width="120px"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle CssClass="Hidden" />
                                                        <HeaderStyle CssClass="Hidden" />
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
                                                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                                            </asp:GridView>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr style="height: 35px;">
                                    <td align="right" class="style1">
                                        <asp:Button ID="btnCloseMKT" runat="server" Text="ปิดหน้าจอ" Width="100px" OnClick="btnCloseMKT_Click"
                                            OnClientClick="DisplayProcessing()" />&nbsp;&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <act:ModalPopupExtender ID="mpePopupMKT" runat="server" TargetControlID="btnMKT"
                            PopupControlID="pnlPopMKT" BackgroundCssClass="modalBackground" DropShadow="True">
                        </act:ModalPopupExtender>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
                <uc1:GridviewPageController ID="pcTop12" runat="server" OnPageChange="PageSearchChange"
                    Width="1190px" Visible="false"/>
                <asp:UpdatePanel ID="upExport" runat="server" UpdateMode="Conditional" Visible="false">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlExport" CssClass="modalPopupMKTMonitoring">
                            <asp:Button ID="Button1" runat="server" CssClass="Hidden" />
                            <br />
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hResult.gif" />
                            <br />
                            <br />
                            <br />
                            <asp:GridView ID="gvExport" runat="server" AutoGenerateColumns="False" Width="1830px"
                                Visible="false" GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                                EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"
                                OnDataBound="gvExport_DataBound">
                                <Columns>
                                     <asp:TemplateField HeaderText="ชื่อทีม">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTEAMCODE" runat="server" Text='<%# Eval("TEAMCODE") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="รหัสพนักงาน">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEMPCODE" runat="server" Text='<%# Eval("EMPCODE") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="Hidden" />
                                        <HeaderStyle CssClass="Hidden" />
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="สถานะของ Lead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Eval("StatusDesc") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="สถานะย่อยของ Lead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSubStatusDesc" runat="server" Text='<%# Eval("SubStatusDesc") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="Hidden" />
                                        <HeaderStyle CssClass="Hidden" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="เลขที่สัญญา">
                                        <ItemTemplate>
                                            <asp:Label ID="lblContractNo" runat="server" Text='<%# Eval("ContractNo") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="ชื่อ-สกุล ลูกค้า">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCusFullName" runat="server" Text='<%# Eval("CusFullName") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="ชื่อบริษัทประกันภัย">
                                        <ItemTemplate>
                                            <asp:Label ID="lblINSNAME" runat="server" Text='<%# Eval("INSNAME") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="ประเภทประกันภัย">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCOV_NAME" runat="server" Text='<%# Eval("COV_NAME") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="ค่าเบี้ยประกัน">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGROSS_PREMIUM" runat="server" Text='<%# Eval("GROSS_PREMIUM") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Transfer Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Grade ลูกค้า">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrade" runat="server" Text='<%# Eval("Grade") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prelead ID">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPreleadId" runat="server" Text='<%# Eval("PreleadId") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                  <asp:TemplateField HeaderText="Lot">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLot" runat="server" Text='<%# Eval("Lot") %>' Width="120px"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <HeaderStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead" />
                                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                            </asp:GridView>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
