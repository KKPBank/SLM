<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_030.aspx.cs" Inherits="SLM.Application.SLM_SCR_030" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
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
            width:230px;
        }
        .ColInfoPopup
        {
            font-weight:bold;
            width:120px;
            padding-left:30px;
        }
        .ColInputPopup
        {
            width:210px;
        }
        .modalPopupAddConfigCustomer
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:480px;
            height:180px;
        }
        .modalPopupEditConfigCustomer
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:770px;
            height:570px;
        }
        .modalPopupAddConfigStaff
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:480px;
            height:200px;
        }
        .modalPopupEditConfigStaff
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:770px;
            height:570px;
        }
        .t_rowhead2
        {
            font-weight: bold;
            color: #4b134a;
            background-color: #fbcbfa;
            height: 24px;
            border: 1px double #f995f8;
        }
        .t_row2
        {
            background-color: #ffffff;
            height: 25px;
            border-top-style:none;
            border-top-width:0px;
            border-left-style:none;
            border-left-width:0px;
            border-right-style:none;
            border-right-width:0px;
            border-bottom-style:Dashed;
            border-bottom-width:1px;
            border-bottom-color:#fbcbfa;
            border-collapse:collapse;
            font-family:Tahoma;
            font-size:13px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="height:5px;"></td>
        </tr>
        <tr>
            <td style="background-color:#c3e2f9; border:1px solid #1578c0; width:1200px; font-weight:bold; font-size:14px; height:22px; color:#0c2f48;">
                &nbsp;เงื่อนไขลูกค้า
            </td>
        </tr>
        <tr>
            <td style="height:5px;"></td>
        </tr>
    </table>
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
                        <asp:DropDownList ID="cmbProductSearch" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Grade ลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbGradeSearch" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ประเภทการจ่ายงาน
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbAssignTypeSearch" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="height:10px; vertical-align:bottom;">
                    </td>
                    <td colspan="3"></td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height:3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" 
                            Text="ล้างข้อมูล" OnClick="btnClear_Click"  />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResultCustomer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
            <asp:Button ID="btnAddConfigCustomer" runat="server" Text="เพิ่มเงื่อนไขการจ่ายงาน" CssClass="Button" Width="150px" OnClick="btnAddConfigCustomer_Click" />
            <br /><br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange_AddConfigCustomer" Width="660px" />
            <asp:GridView ID="gvAddConfigCustomer" runat="server" AutoGenerateColumns="False" Width="660px" 
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" HeaderStyle-BorderColor="#f995f8"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"   >
                <Columns>
                 <asp:TemplateField HeaderText="Action" >
                    <ItemTemplate>
                        <asp:ImageButton ID="imbEditConfigCustomer" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEditConfigCustomer_Click"  ToolTip="แก้ไขเงื่อนไขการจ่ายงาน" OnClientClick="DisplayProcessing();"  />
                        <asp:ImageButton ID="imbDeleteMainConfigCustomer" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("AssignConditionCusId") %>' ToolTip="ลบเงื่อนไขการจ่ายงาน" OnClick="imbDeleteMainConfigCustomer_Click" OnClientClick="if (confirm('ต้องการลบข้อมูล ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                    </ItemTemplate>
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate>
                    </ItemTemplate>
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                    <ItemTemplate>
                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Grade ลูกค้า">
                    <ItemTemplate>
                        <asp:Label ID="lblGradeName" runat="server" Text='<%# Eval("GradeName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภทจ่ายงาน">
                    <ItemTemplate>
                        <asp:Label ID="lblAssignTypeName" runat="server" Text='<%# Eval("AssignTypeName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate>
                        <asp:Label ID="lblAssignConditionCusId" runat="server" Text='<%# Eval("AssignConditionCusId") %>' ></asp:Label>
                    </ItemTemplate>
                    <ControlStyle CssClass="Hidden" />
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Begin Customer Popup Section -->
    <asp:UpdatePanel ID="upPopupAddConfigCustomer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupAddConfigCustomer" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopupAddConfigCustomer" style="display:none" CssClass="modalPopupAddConfigCustomer">
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:20px;"></td></tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            ผลิตภัณฑ์/บริการ<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddProduct_Cus" runat="server" CssClass="Dropdownlist" 
                                Width="203px" AutoPostBack="true" 
                                onselectedindexchanged="cmbAddProduct_Cus_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddProduct_Cus" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            Grade ลูกค้า<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddCustomerGrade_Cus" runat="server" CssClass="Dropdownlist" Width="203px" AutoPostBack="true" onselectedindexchanged="cmbAddCustomerGrade_Cus_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddCustomerGrade_Cus" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            ประเภทการจ่ายงาน<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddAssignType_Cus" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddAssignType_Cus" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:10px;"></td></tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="btnSavePopupAddConfigCustomer" runat="server" Text="บันทึก" Width="90px" CssClass="Button" OnClick="btnSavePopupAddConfigCustomer_Click" OnClientClick="DisplayProcessing();" />
                            <asp:Button ID="btnCancelPopupAddConfigCustomer" runat="server" Text="ยกเลิก" Width="90px" CssClass="Button" OnClick="btnCancelPopupAddConfigCustomer_Click" />
                        </td>
                    </tr>
                </table> 
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupAddConfigCustomer" runat="server" TargetControlID="btnPopupAddConfigCustomer" PopupControlID="pnPopupAddConfigCustomer" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPopupEditConfigCustomer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupEditConfigCustomer" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopupEditConfigCustomer" style="display:none" CssClass="modalPopupEditConfigCustomer">
                <table cellpadding="2" cellspacing="0" border="0" >
                    <tr><td colspan="4" style="height:7px;"></td></tr>
                    <tr>
                        <td colspan="4" style="padding-left:24px;">
                            <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/normal_detail.png" ImageAlign="Top" />&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoPopup">
                            ผลิตภัณฑ์/บริการ
                            <asp:TextBox ID="txtEditAssignConditionCusId" runat="server" Width="40px" Visible="false"></asp:TextBox>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtEditProductName" runat="server" Width="566px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoPopup">
                            Grade ลูกค้า
                        </td>
                        <td class="ColInputPopup">
                            <asp:TextBox ID="txtEditCustomerGrade" runat="server" ReadOnly="true" Width="200px" CssClass="TextboxView"></asp:TextBox>
                        </td>
                        <td class="ColInfoPopup">
                            ประเภทการจ่ายงาน
                        </td>
                        <td class="ColInputPopup">
                            <asp:TextBox ID="txtEditAssignType" runat="server" ReadOnly="true" Width="200px" CssClass="TextboxView"></asp:TextBox>
                        </td>
                    </tr>
                </table>

                <asp:UpdatePanel ID="upEditConfigCusButton_Inner" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" border="0">
                            <tr>
                                <td colspan="4" style="height:4px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="padding-left:24px;">
                                    <asp:Image ID="Image7" runat="server" ImageUrl="~/Images/add_con_field.png" ImageAlign="Top" />&nbsp;
                                </td>
                            </tr>
                            <tr style="vertical-align:top;">
                                <td class="ColInfoPopup">
                                    ฟิลด์เงื่อนไข<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:DropDownList ID="cmbEditConditionField" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList><br />
                                    <asp:Label ID="lblAlertEditConditionField" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                                <td class="ColInfoPopup">
                                    เรียงลำดับ<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:DropDownList ID="cmbEditOrder" runat="server" CssClass="Dropdownlist" Width="203px">
                                        <asp:ListItem Text="น้อยไปมาก" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="มากไปน้อย" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align:top;">
                                <td class="ColInfoPopup">
                                    Seq<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:TextBox ID="txtEditSeq" runat="server" CssClass="Textbox" Width="50px" MaxLength="3"></asp:TextBox>
                                    <asp:Label ID="lblAlertEditSeq" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                                <td class="ColInfoPopup">
                                </td>
                                <td class="ColInputPopup">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height:1px"></td>
                            </tr>
                            <tr>
                                <td class="ColInfoPopup">
                                </td>
                                <td colspan="3" class="ColInputPopup">
                                    <asp:Button ID="btnEditConfigCusSave" runat="server" Text="บันทึก" Width="100px" CssClass="Button" OnClick="btnEditConfigCusSave_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                                    <asp:Button ID="btnEditConfigCusCancel" runat="server" Text="ปิด" Width="100px" CssClass="Button" OnClick="btnEditConfigCusCancel_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height:3px"></td>
                            </tr>
                        </table>
                        <br />
                        <div style="padding-left: 30px;">
                            <uc2:GridviewPageController ID="pcTopEditConfigCustomer" runat="server" OnPageChange="PageSearchChange_EditConfigCustomer" Width="590px" />
                            <asp:GridView ID="gvEditConfigCustomer" runat="server" AutoGenerateColumns="False" Width="590px" 
                                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" HeaderStyle-BorderColor="#f995f8"
                                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"   >
                                <Columns>
                                 <asp:TemplateField HeaderText="Action" >
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imbDeleteConfigCustomer" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("SortConCusId") %>' ToolTip="ลบเงื่อนไขการจ่ายงาน" OnClick="imbDeleteConfigCustomer_Click" OnClientClick="if (confirm('ต้องการลบข้อมูล ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ฟีลด์เงื่อนไข">
                                    <ItemTemplate>
                                        <asp:Label ID="lblConditionFieldName" runat="server" Text='<%# Eval("ConditionFieldName") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="220px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Seq">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSeq" runat="server" Text='<%# Eval("Seq") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="จัดเรียง">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderByName" runat="server" Text='<%# Eval("SortBy") != null ? (Eval("SortBy").ToString() == "1" ? "น้อยไปมาก" : "มากไปน้อย") : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead" />
                                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupEditConfigCustomer" runat="server" TargetControlID="btnPopupEditConfigCustomer" PopupControlID="pnPopupEditConfigCustomer" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- End Customer Popup Section -->

    <br /><br /><br />

    <!---------------------------------------------------- เงื่อนไขเจ้าหน้าที่ ---------------------------------------------------------------->
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="height:5px;"></td>
        </tr>
        <tr>
            <td style="background-color:#fbcbfa; border:1px solid #f995f8; width:1200px; font-weight:bold; font-size:14px; height:22px; color:#4b134a;">
                &nbsp;เงื่อนไขเจ้าหน้าที่
            </td>
        </tr>
        <tr>
            <td style="height:5px;"></td>
        </tr>
    </table>
    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearchOfficer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:2px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        ผลิตภัณฑ์/บริการ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearchStaff" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Grade ลูกค้า
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbGradeSearchStaff" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ประเภทการจ่ายงาน
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbAssignTypeSearchStaff" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        Team Telesales
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbTeamTeleSearchStaff" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="height:10px; vertical-align:bottom;">
                    </td>
                    <td colspan="3"></td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height:3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td colspan="5">
                        <asp:Button ID="btnSearchStaff" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearchStaff_Click" />&nbsp;
                        <asp:Button ID="btnClearOff" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" 
                            Text="ล้างข้อมูล" OnClick="btnClearStaff_Click"  />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResultStaff" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
            <asp:Button ID="btnAddConfigStaff" runat="server" Text="เพิ่มเงื่อนไขการจ่ายงาน" CssClass="Button" Width="150px" OnClick="btnAddConfigStaff_Click" />
            <br /><br />
            <uc2:GridviewPageController ID="pcTopAddConfigStaff" runat="server" OnPageChange="PageSearchChange_AddConfigStaff" Width="910px" />
            <asp:GridView ID="gvAddConfigStaff" runat="server" AutoGenerateColumns="False" Width="910px" 
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" HeaderStyle-BorderColor="#f995f8"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"   >
                <Columns>
                 <asp:TemplateField HeaderText="Action" >
                    <ItemTemplate>
                        <asp:ImageButton ID="imbEditConfigStaff" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEditConfigStaff_Click"  ToolTip="แก้ไขเงื่อนไขการจ่ายงาน" OnClientClick="DisplayProcessing();"  />
                        <asp:ImageButton ID="imbDeleteMainConfigStaff" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("AssignConditionStaffId") %>' ToolTip="ลบเงื่อนไขการจ่ายงาน" OnClick="imbDeleteMainConfigStaff_Click" OnClientClick="if (confirm('ต้องการลบข้อมูล ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                    </ItemTemplate>
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate>
                    </ItemTemplate>
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                    <ItemTemplate>
                        <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Grade ลูกค้า">
                    <ItemTemplate>
                        <asp:Label ID="lblGradeName" runat="server" Text='<%# Eval("GradeName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภทจ่ายงาน">
                    <ItemTemplate>
                        <asp:Label ID="lblAssignTypeName" runat="server" Text='<%# Eval("AssignTypeName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Team Telesales">
                    <ItemTemplate>
                        <asp:Label ID="lblTeamTelesalesName" runat="server" Text='<%# Eval("TeamTelesalesName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate>
                        <asp:Label ID="lblAssignConditionStaffId" runat="server" Text='<%# Eval("AssignConditionStaffId") %>' ></asp:Label>
                    </ItemTemplate>
                    <ControlStyle CssClass="Hidden" />
                    <ItemStyle CssClass="Hidden" />
                    <HeaderStyle CssClass="Hidden" />
                    <FooterStyle CssClass="Hidden" />
                </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead2" />
                <RowStyle CssClass="t_row2" BorderStyle="Dashed"/>
            </asp:GridView>
        
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Begin Staff Popup Section -->
    <asp:UpdatePanel ID="upPopupAddConfigStaff" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupAddConfigStaff" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopupAddConfigStaff" style="display:none" CssClass="modalPopupAddConfigStaff">
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:20px;"></td></tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            ผลิตภัณฑ์/บริการ<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddProduct_Staff" runat="server" CssClass="Dropdownlist" Width="203px" AutoPostBack="true" OnSelectedIndexChanged="cmbAddProduct_Staff_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddProduct_Staff" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            Grade ลูกค้า<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddGrade_Staff" runat="server" CssClass="Dropdownlist" Width="203px" AutoPostBack="true" OnSelectedIndexChanged="cmbAddGrade_Staff_SelectedIndexChanged"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddGrade_Staff" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            ประเภทการจ่ายงาน<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddAssignType_Staff" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddAssignType_Staff" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="font-weight:bold; width:140px;">
                            Team Telesales<span style="color:Red;">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbAddTeamTeles_Staff" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList>
                            <asp:Label ID="lblAlertAddTeamTeles_Staff" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:10px;"></td></tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="btnSavePopupAddConfigStaff" runat="server" Text="บันทึก" Width="90px" CssClass="Button" OnClick="btnSavePopupAddConfigStaff_Click" OnClientClick="DisplayProcessing();" />
                            <asp:Button ID="btnCancelPopupAddConfigStaff" runat="server" Text="ยกเลิก" Width="90px" CssClass="Button" OnClick="btnCancelPopupAddConfigStaff_Click" />
                        </td>
                    </tr>
                </table> 
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupAddConfigStaff" runat="server" TargetControlID="btnPopupAddConfigStaff" PopupControlID="pnPopupAddConfigStaff" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPopupEditConfigStaff" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupEditConfigStaff" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopupEditConfigStaff" style="display:none" CssClass="modalPopupEditConfigStaff">
                <table cellpadding="2" cellspacing="0" border="0" >
                    <tr><td colspan="4" style="height:7px;"></td></tr>
                    <tr>
                        <td colspan="4" style="padding-left:24px;">
                            <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/normal_detail.png" ImageAlign="Top" />&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoPopup">
                            ผลิตภัณฑ์/บริการ
                            <asp:TextBox ID="txtEditAssignConditionStaffId" runat="server" Width="40px" Visible="false" ></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEditProductName_Staff" runat="server" Width="200px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="ColInfoPopup">
                            Team Telesales
                        </td>
                        <td>
                            <asp:TextBox ID="txtEditTeamTeles_Staff" runat="server" Width="200px" CssClass="TextboxView" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfoPopup">
                            Grade ลูกค้า
                        </td>
                        <td class="ColInputPopup">
                            <asp:TextBox ID="txtEditGradeName_Staff" runat="server" ReadOnly="true" Width="200px" CssClass="TextboxView"></asp:TextBox>
                        </td>
                        <td class="ColInfoPopup">
                            ประเภทการจ่ายงาน
                        </td>
                        <td class="ColInputPopup">
                            <asp:TextBox ID="txtEditAssignTypeName_Staff" runat="server" ReadOnly="true" Width="200px" CssClass="TextboxView"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                
                <asp:UpdatePanel ID="upEditConfigStaffButton_Inner" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" border="0">
                            <tr>
                                <td colspan="4" style="height:4px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="padding-left:24px;">
                                    <asp:Image ID="Image5" runat="server" ImageUrl="~/Images/add_con_field.png" ImageAlign="Top" />&nbsp;
                                </td>
                            </tr>
                            <tr style="vertical-align:top;">
                                <td class="ColInfoPopup">
                                    ฟิลด์เงื่อนไข<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:DropDownList ID="cmbEditConditionField_Staff" runat="server" CssClass="Dropdownlist" Width="203px"></asp:DropDownList><br />
                                </td>
                                <td class="ColInfoPopup">
                                    เรียงลำดับ<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:DropDownList ID="cmbEditSortBy_Staff" runat="server" CssClass="Dropdownlist" Width="203px">
                                        <asp:ListItem Text="น้อยไปมาก" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="มากไปน้อย" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align:top;">
                                <td class="ColInfoPopup">
                                    Seq<span style="color:Red;">*</span>
                                </td>
                                <td class="ColInputPopup">
                                    <asp:TextBox ID="txtEditSeq_Staff" runat="server" CssClass="Textbox" Width="50px" MaxLength="3"></asp:TextBox>
                                </td>
                                <td class="ColInfoPopup">
                                </td>
                                <td class="ColInputPopup">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height:1px"></td>
                            </tr>
                            <tr>
                                <td class="ColInfoPopup">
                                </td>
                                <td colspan="3" class="ColInputPopup">
                                    <asp:Button ID="btnEditConfigStaffSave" runat="server" Text="บันทึก" Width="100px" CssClass="Button" OnClick="btnEditConfigStaffSave_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                                    <asp:Button ID="btnEditConfigStaffCancel" runat="server" Text="ปิด" Width="100px" CssClass="Button" OnClick="btnEditConfigStaffCancel_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height:3px"></td>
                            </tr>
                        </table>
                        <br />
                        <div style="padding-left: 30px;">
                            <uc2:GridviewPageController ID="pcTopEditConfigStaff" runat="server" OnPageChange="PageSearchChange_EditConfigStaff" Width="590px" />
                            <asp:GridView ID="gvEditConfigStaff" runat="server" AutoGenerateColumns="False" Width="590px" 
                                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" HeaderStyle-BorderColor="#f995f8"
                                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"   >
                                <Columns>
                                 <asp:TemplateField HeaderText="Action" >
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imbDeleteConfigStaff" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("SortConStaffId") %>' ToolTip="ลบเงื่อนไขการจ่ายงาน" OnClick="imbDeleteConfigStaff_Click" OnClientClick="if (confirm('ต้องการลบข้อมูล ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ฟีลด์เงื่อนไข">
                                    <ItemTemplate>
                                        <asp:Label ID="lblConditionFieldName" runat="server" Text='<%# Eval("ConditionFieldName") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="220px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Seq">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSeq" runat="server" Text='<%# Eval("Seq") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="จัดเรียง">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderByName" runat="server" Text='<%# Eval("SortBy") != null ? (Eval("SortBy").ToString() == "1" ? "น้อยไปมาก" : "มากไปน้อย") : "" %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="t_rowhead2" />
                                <RowStyle CssClass="t_row2" BorderStyle="Dashed"/>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupEditConfigStaff" runat="server" TargetControlID="btnPopupEditConfigStaff" PopupControlID="pnPopupEditConfigStaff" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- End Staff Popup Section -->
</asp:Content>
