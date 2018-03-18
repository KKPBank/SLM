<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_042.aspx.cs" Inherits="SLM.Application.SLM_SCR_042" %>

<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc2" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo {
            font-weight: bold;
            width: 190px;
        }

        .ColInput {
            width: 250px;
        }

        .ColCheckBox {
            width: 160px;
        }

        .modalPopupAddReceiveNo {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 750px;
            height: 460px;
        }
        .modalPopupError {
            text-align:center;
            vertical-align: middle;
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 300px;
            height: 130px;
            padding: 40px 40px 0px 40px;
        }

        .style4 {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery-2.2.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/jscript" src="Scripts/SlmScript.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="4" style="height: 2px;"></td>
                </tr>
                <tr class="Hidden">
                    <td class="ColInfo">ผลิตภัณฑ์
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" CssClass="Dropdownlist" Width="200px" runat="server" DataTextField="TextField" DataValueField="ValueField">
                            <asp:ListItem Value="T1" Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem Value="T2" Text="ประกันภัยรถยนต์ปีต่อ"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo"></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="ColInfo">รหัสบริษัทประกัน
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtCodeSearch" runat="server" CssClass="Textbox" Width="100px"></asp:TextBox>
                    </td>
                    <td class="ColInfo"></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="ColInfo">ชื่อบริษัทประกัน
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtNameSearch" runat="server" CssClass="Textbox" Width="300px"></asp:TextBox>
                </tr>
                <tr>
                    <td class="ColInfo">ประเภท                    
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="cmbTypeSearch" CssClass="Dropdownlist" Width="200px" runat="server">
                            <asp:ListItem Value="-1" Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem Value="01" Text="เป็นคู่ค้า"></asp:ListItem>
                            <asp:ListItem Value="02" Text="ไม่เป็นคู่ค้า"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height: 3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing();"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing();clearSearchForm();"
                            Text="ล้างข้อมูล" OnClick="btnClear_Click" />
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
            <asp:Button ID="btnImportExcel" runat="server" Text="เพิ่มข้อมูลบริษัทประกัน" Width="150px"
                CssClass="Button" Height="23px" OnClick="btnImportExcel_Click" OnClientClick="clearForm();" />
            <br />
            <br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="800px" Visible="false"/>
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Visible="false"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="800px" OnRowCommand="gvResult_RowCommand" OnRowDataBound="gvResult_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("CompanyId") %>' ToolTip="แก้ไขบริษัทประกัน" OnClick="imbEdit_Click" />
                        </ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ผลิตภัณฑ์">
                        <ItemTemplate>
                            <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" CssClass="Hidden" />
                        <HeaderStyle Width="120px" HorizontalAlign="Center" CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รหัสบริษัทประกัน">
                        <ItemTemplate>
                            <asp:Label ID="lblInsuredCode" runat="server" Text='<%# Eval("InsuredCode") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อบริษัทประกัน">
                        <ItemTemplate>
                            <asp:Label ID="lblDataType" runat="server" Text='<%# Eval("InsuredNameTh") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="300px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="300px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ประเภท">
                        <ItemTemplate>
                            <asp:Label ID="lblSubject" runat="server" Text='<%# Eval("InsuredType").ToString() == "01" ? "เป็นคู่ค้า" : "ไม่เป็นคู่ค้า" %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="สถานะ">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server" Text='<%# (bool)Eval("is_Deleted") == false ? "ใช้งาน" : "ไม่ใช้งาน" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upnPopupReceiveNo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnPopupDoImportExcel" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnPopupCancel" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="gvResult" EventName="RowCommand" />
        </Triggers>
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupReceiveNo" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupReceiveNo" Style="display: none" CssClass="modalPopupAddReceiveNo">
                <br />
                <br />
                <asp:HiddenField runat="server" ID="hiddenCompId" Value="-1" />
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr class="Hidden">
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ผลิตภัณฑ์<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:DropDownList ID="cmbProductAdd" runat="server" DataTextField="TextField" DataValueField="ValueField">
                            </asp:DropDownList>
                            <%--<asp:CompareValidator ID="CompareValidatorProduct" runat="server" ErrorMessage="กรุณาระบุผลิตภัณฑ์" ValueToCompare="-1"
                                ControlToValidate="cmbProductAdd" ValidationGroup="InsuredAdd" CssClass="style4"></asp:CompareValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">รหัสบริษัทประกัน<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtCodeAdd" runat="server" CssClass="Textbox" Width="100px" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredCodeAdd" runat="server" ErrorMessage="กรุณาระบุรหัสบริษัทประกัน"
                                ControlToValidate="txtCodeAdd" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ชื่อย่อบริษัทประกัน<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtAbbName" runat="server" CssClass="Textbox" Width="300px" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredAbbName" runat="server" ErrorMessage="กรุณาระบุชื่อย่อบริษัทประกัน"
                                ControlToValidate="txtAbbName" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ชื่อบริษัทประกัน (EN)</td>
                        <td colspan="7">
                            <asp:TextBox ID="txtNameEn" runat="server" CssClass="Textbox" Width="300px" MaxLength="500"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredNameEn" runat="server" ErrorMessage="กรุณาระบุชื่อบริษัทประกัน (EN)" Enabled="false"
                                ControlToValidate="txtNameEn" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ชื่อบริษัทประกัน (TH)<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtNameTh" runat="server" CssClass="Textbox" Width="300px" MaxLength="500"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredNameTh" runat="server" ErrorMessage="กรุณาระบุชื่อบริษัทประกัน (TH)"
                                ControlToValidate="txtNameTh" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ประเภท<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:DropDownList ID="cmbTypeAdd" runat="server">
                                <asp:ListItem Value=""></asp:ListItem>
                                <asp:ListItem Text="คู่ค้า" Value="01"></asp:ListItem>
                                <asp:ListItem Text="ไม่เป็นคู่ค้า" Value="02"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredType" runat="server" ErrorMessage="กรุณาระบุประเภท"
                                ControlToValidate="cmbTypeAdd" InitialValue="" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">เลขที่ผู้เสียภาษี<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtTaxCode" runat="server" CssClass="Textbox" Width="150px" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredTaxCode" runat="server" ErrorMessage="กรุณาระบุเลขที่ผู้เสียภาษี"
                                ControlToValidate="txtTaxCode" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">เบอร์โทรศัพท์<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="Textbox" Width="150px" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredPhone" runat="server" ErrorMessage="กรุณาระบุเบอร์โทรศํพท์"
                                ControlToValidate="txtPhone" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">เบอร์โทรศัพท์สำหรับส่ง SMS ลูกค้า<span class="style4">*</span></td>
                        <td colspan="7">
                            <asp:TextBox ID="txtSms" runat="server" CssClass="Textbox" Width="150px" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredSms" runat="server" ErrorMessage="กรุณาระบุเบอร์โทรศํพท์สำหรับส่ง SMS ลูกค้า"
                                ControlToValidate="txtSms" ValidationGroup="InsuredAdd" CssClass="style4"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">เลขที่</td>
                        <td>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="Textbox" Width="70px" MaxLength="50"></asp:TextBox>
                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 50px;">หมู่</td>
                        <td>
                            <asp:TextBox ID="txtMoo" runat="server" CssClass="Textbox" Width="70px" MaxLength="50"></asp:TextBox>
                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 70px;">อาคาร</td>
                        <td>
                            <asp:TextBox ID="txtBuilding" runat="server" CssClass="Textbox" Width="130px" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">ชั้น</td>
                        <td>
                            <asp:TextBox ID="txtFloor" runat="server" CssClass="Textbox" Width="70px" MaxLength="50"></asp:TextBox>
                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 50px;">ซอย</td>
                        <td>
                            <asp:TextBox ID="txtSoi" runat="server" CssClass="Textbox" Width="130px" MaxLength="500"></asp:TextBox>
                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 70px;">ถนน</td>
                        <td>
                            <asp:TextBox ID="txtRoad" runat="server" CssClass="Textbox" Width="130px" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">จังหวัด</td>
                        <td>
                            <asp:DropDownList ID="cmbProvince" runat="server" CssClass="Dropdownlist" Width="100px" AutoPostBack="true" DataTextField="TextField" DataValueField="ValueField" OnSelectedIndexChanged="cmbProvince_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 50px;">อำเภอ</td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="updateAmphur" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cmbProvince" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:DropDownList ID="cmbAmphur" runat="server" CssClass="Dropdownlist" Width="100px" AutoPostBack="true" DataTextField="TextField" DataValueField="ValueField" OnSelectedIndexChanged="cmbAmphur_SelectedIndexChanged"></asp:DropDownList>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                        <td style="width: 10px;"></td>
                        <td style="width: 70px;">ตำบล</td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cmbProvince" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cmbAmphur" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:DropDownList ID="cmbTambol" runat="server" CssClass="Dropdownlist" Width="100px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">รหัสไปรษณีย์</td>
                        <td colspan="7">
                            <asp:TextBox ID="txtPostcode" runat="server" CssClass="Textbox" Width="70px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 160px;">สถานะ</td>
                        <td colspan="7">
                            <asp:RadioButton ID="rdActive" runat="server" GroupName="Status" Text="ใช้งาน" Checked="true" />
                            <asp:RadioButton ID="rdNoActive" runat="server" GroupName="Status" Text="ไม่ใช้งาน" />
                        </td>
                    </tr>

                    <tr>
                        <td colspan="10" style="height: 10px"></td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 100px;"></td>
                        <td colspan="7">
                            <asp:Button ID="btnPopupDoImportExcel" runat="server" Text="บันทึก" Width="100px" ValidationGroup="InsuredAdd" OnClick="btnPopupDoImportExcel_Click" OnClientClick="if(Page_ClientValidate('InsuredAdd')) DisplayProcessing();" />&nbsp;
                                <asp:Button ID="btnPopupCancel" runat="server" Text="ยกเลิก" Width="100px" OnClick="btnPopupCancel_Click"
                                    OnClientClick="DisplayProcessing();clearForm();" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupReceiveNo" runat="server" TargetControlID="btnPopupReceiveNo" PopupControlID="pnPopupReceiveNo" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>

            
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel runat="server" ID="upError" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupError" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupError" Style="display: none" CssClass="modalPopupError">
                <asp:Label runat="server" ID="lblError"></asp:Label>
                <br /><br /><br />
                <asp:Button runat="server" ID="btnCloseError" OnClick="btnCloseError_Click" Text="OK"/>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupError" runat="server" TargetControlID="btnPopupError" PopupControlID="pnPopupError" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function clearForm() {
            $('#ContentPlaceHolder1_upnPopupReceiveNo').find('input[type="text"]').val("");
            $('select[id*="cmbProductAdd"] option:first').attr("selected", true);
            $('select[id*="cmbTypeAdd"] option:first').attr("selected", true)
            $('input[id*="hiddenCompId"]').val("-1")
        }
        function clearSearchForm() {
            $('div[id$="upSearch"]').find('input[type="text"]').val("");
            $('div[id$="upSearch"]').find('select')
                .each(function () {
                    this[0].selected = true;
                });
        }
    </script>

</asp:Content>

