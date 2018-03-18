<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_035.aspx.cs" Inherits="SLM.Application.SLM_SCR_035" %>

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
            width: 550px;
            /*height:420px;*/
        }

        .style4 {
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
                    <td colspan="4" style="height: 2px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo">ผลิตภัณฑ์
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" runat="server" Width="200px" CssClass="Dropdownlist" DataTextField="TextField" DataValueField="ValueField" AutoPostBack="true" OnSelectedIndexChanged="cmbProductSearch_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td class="ColInfo">ชื่อบริษัทประกัน
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbInsComName" runat="server" Width="200px" CssClass="Dropdownlist" AutoPostBack="true" DataTextField="TextField" DataValueField="ValueField" OnSelectedIndexChanged="cmbInsComName_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td class="ColInfo" style="width: 280px;">แคมเปญ
                    </td>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upCampaignSearch" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="cmbCampaign" runat="server" Width="200px" CssClass="Dropdownlist" DataTextField="TextField" DataValueField="ValueField">
                                    <asp:ListItem Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">ผลตอบแทนธนาคาร</td>
                    <td class="ColInput">
                        <asp:CheckBox runat="server" ID="chkCoverageTypeSearch" Text="ประกันภัยรถยนต์" AutoPostBack="true" OnCheckedChanged="chkCoverageTypeSearch_CheckedChanged" /></td>
                    <td class="ColInfo">ประเภทความคุ้มครอง (ประกันภัยรถยนต์)</td>
                    <td>
                        <asp:DropDownList ID="ddlCoverageTypeSearch" runat="server" Width="200px" CssClass="Dropdownlist" Enabled="false"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="ColInput">
                        <asp:CheckBox runat="server" ID="chkInsuranceTypeSearch" Text="พรบ." AutoPostBack="true" OnCheckedChanged="chkInsuranceTypeSearch_CheckedChanged" /></td>
                    <td class="ColInfo">ประเภทความคุ้มครอง (พรบ.)</td>
                    <td>
                        <asp:DropDownList ID="ddlInsuranceTypeSearch" runat="server" Width="200px" CssClass="Dropdownlist" Enabled="false"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="ColInfo">สถานะ</td>
                    <td class="ColInput">
                        <asp:CheckBox runat="server" ID="chkActive" Text="ใช้งาน" />
                        <asp:CheckBox runat="server" ID="chkInActive" Text="ไม่ใช้งาน" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height: 3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" OnClick="btnClear_Click"
                            Text="ล้างข้อมูล" />
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
            <asp:Button ID="btnAdd" runat="server" Text="เพิ่มข้อมูลผลประโยชน์" Width="150px"
                CssClass="Button" Height="23px" OnClick="btnAdd_Click" />
            <br />
            <br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1800px" Visible="false" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Visible="false"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1800px">
                <Columns>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("BenefitId") %>' ToolTip="แก้ไขเลขรับแจ้ง" OnClick="imbEdit_Click" />
                        </ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อผลิตภัณฑ์">
                        <ItemTemplate>
                            <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อบริษัทประกัน">
                        <ItemTemplate>
                            <asp:Label ID="lblInsComName" runat="server" Text='<%# Eval("InsComName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อแคมเปญ">
                        <ItemTemplate>
                            <asp:Label ID="lbCampaignName" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ผลตอบแทนธนาคาร">
                        <ItemTemplate>
                            <asp:Label ID="lblBenefitTypeDesc" runat="server" Text='<%# Eval("BenefitTypeDesc") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ประเภทความคุ้มครอง">
                        <ItemTemplate>
                            <asp:Label ID="lblInsuranceTypeDesc" runat="server" Text='<%# Eval("InsuranceTypeDesc") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ค่าคอมมิชชั่น(%)">
                        <ItemTemplate>
                            <asp:Label ID="lblCommissionPct" runat="server" Text='<%# Eval("ComissionPercentValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ค่าคอมมิชชั่น(บาท)">
                        <ItemTemplate>
                            <asp:Label ID="lblCommissionThb" runat="server" Text='<%# Eval("ComissionBathValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="OV1(%)">
                        <ItemTemplate>
                            <asp:Label ID="lblOv1Pct" runat="server" Text='<%# Eval("OV1PercentValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="OV1(บาท)">
                        <ItemTemplate>
                            <asp:Label ID="lblOv1Thb" runat="server" Text='<%# Eval("OV1BathValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="OV2(%)">
                        <ItemTemplate>
                            <asp:Label ID="lblOv2Pct" runat="server" Text='<%# Eval("OV2PercentValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="OV2(บาท)">
                        <ItemTemplate>
                            <asp:Label ID="lblOv2Thb" runat="server" Text='<%# Eval("OV2BathValueDisplay") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle Width="120px" HorizontalAlign="right" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vat">
                        <ItemTemplate>
                            <asp:Label ID="lblVat" runat="server" Text='<%# Eval("VatFlag").ToString() == "I" ? "Include Vat" : "Exclude Vat" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ผู้บันทึก">
                        <ItemTemplate>
                            <asp:Label ID="lblUpdateBy" runat="server" Text='<%# Eval("UpdatedBy") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วัน-เวลาข้อมูลล่าสุด">
                        <ItemTemplate>
                            <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Eval("UpdatedDate") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สถานะ">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("is_Deleted").ToString() == "True" ? "ไม่ใช้งาน" : "ใช้งาน" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button runat="server" ID="btnPopupReceiveNo" Width="0px" CssClass="Hidden" />
    <asp:Panel runat="server" ID="pnPopupReceiveNo" Style="display: none; padding: 15px;" CssClass="modalPopupAddReceiveNo">
        <asp:UpdatePanel runat="server" ID="updDetail" UpdateMode="Conditional">
            <ContentTemplate>

                <br />
                <br />
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="width: 20px;">
                            <asp:HiddenField runat="server" ID="hiddenBenefitId" />
                        </td>
                        <td style="width: 180px;">ผลิตภัณฑ์<span class="style4">*</span></td>
                        <td>
                            <asp:DropDownList ID="cmbProductEdit" runat="server" CssClass="Dropdownlist" Width="200px" DataTextField="TextField" DataValueField="ValueField" AutoPostBack="true" OnSelectedIndexChanged="cmbProductEdit_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="requireProduct" runat="server" ErrorMessage="กรุณาระบุชื่อผลิตภัณฑ์" ControlToValidate="cmbProductEdit" CssClass="style4" Display="Dynamic" InitialValue="-1" ValidationGroup="EditBenefit"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;">ชื่อบริษัทประกัน<span class="style4">*</span></td>
                        <td>
                            <asp:DropDownList ID="cmbInsComNameEdit" runat="server" CssClass="Dropdownlist" Width="200px" DataTextField="TextField" DataValueField="ValueField" AutoPostBack="true" OnSelectedIndexChanged="cmbInsComNameEdit_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="requireInsComName" runat="server" ErrorMessage="กรุณาระบุชื่อบริษัทประกัน" ControlToValidate="cmbInsComNameEdit" CssClass="style4" Display="Dynamic" InitialValue="-1" ValidationGroup="EditBenefit"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;">ชื่อแคมเปญ</td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="upCampaignEdit" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cmbProductEdit" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cmbInsComNameEdit" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:DropDownList ID="cmbCampaignEdit" runat="server" CssClass="Dropdownlist" Width="200px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="requireCampaignEdit" runat="server" ErrorMessage="กรุณาระบุชื่อแคมเปญ" ControlToValidate="cmbCampaignEdit" CssClass="style4" Display="Dynamic" InitialValue="-1" ValidationGroup="EditBenefit" Enabled="false"></asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>ผลตอบแทนธนาคาร<span class="style4">*</span></td>
                        <td>
                            <asp:RadioButton ID="rdoCoverageType" runat="server" GroupName="Benefit" Text="ประกันภัยรถยนต์" Checked="true" AutoPostBack="true" OnCheckedChanged="rdoCoverageType_CheckedChanged" onchange="CheckChangeValidator();"/>&nbsp;&nbsp;&nbsp;
                            <asp:RadioButton ID="rdoInsuranceType" runat="server" GroupName="Benefit" Text="พรบ." AutoPostBack="true" OnCheckedChanged="rdoInsuranceType_CheckedChanged" onchange="CheckChangeValidator();"/>
                             <asp:CustomValidator ID="cvalBenefitEdit" runat="server" ErrorMessage="กรุณาระบุผลตอบแทนธนาคาร" 
                                 ValidationGroup="EditBenefit" CssClass="style4" ClientValidationFunction="ValidateBenifitType"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>ประเภทความคุ้มครอง</td>
                        <td>
                            <%--<asp:DropDownList ID="ddlInsuranceType" runat="server" CssClass="Dropdownlist" Width="200px"></asp:DropDownList>--%>
                            <asp:UpdatePanel runat="server" ID="uplBenefitEdit" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="rdoCoverageType" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="rdoInsuranceType" EventName="CheckedChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddlInsuranceType" runat="server" CssClass="Dropdownlist" Width="200px"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>

                <fieldset>
                    <table>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioCommissionPct" runat="server" Text="ค่าคอมมิชชั่น(%)" GroupName="com_cost" Checked="true" AutoPostBack="true" OnCheckedChanged="radioCommission_CheckedChanged" />
                            </td>
                            <td>
                                <asp:TextBox ID="textCommissionPct" runat="server" CssClass="Textbox" Width="70px" MaxLength="6"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireCommissionPct" runat="server" ErrorMessage="กรุณาระบุค่าคอมมิชชั่น" ControlToValidate="textCommissionPct" ValidationGroup="EditBenefit" CssClass="style4"></asp:RequiredFieldValidator>

                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioCommissionThb" runat="server" Text="ค่าคอมมิชชั่น(บาท)" Checked="false" GroupName="com_cost" AutoPostBack="true" OnCheckedChanged="radioCommission_CheckedChanged" /></td>
                            <td>
                                <asp:TextBox ID="textCommissionThb" runat="server" CssClass="Textbox" Width="70px" Enabled="false"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireCommissionThb" runat="server" ErrorMessage="กรุณาระบุค่าคอมมิชชั่น" ControlToValidate="textCommissionThb" ValidationGroup="EditBenefit" CssClass="style4" Enabled="false"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset>
                    <table>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV1Pct" runat="server" Text="OV1(%)" GroupName="OV1" OnCheckedChanged="radioOV1_CheckedChanged" AutoPostBack="true" /></td>
                            <td>
                                <asp:TextBox ID="textOV1Pct" runat="server" CssClass="Textbox" Width="70px" MaxLength="6"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireOV1Pct" runat="server" ErrorMessage="กรุณาระบุ OV1(%)" ControlToValidate="textOV1Pct" ValidationGroup="EditBenefit" CssClass="style4" Enabled="false"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV1Thb" runat="server" Text="OV1(บาท)" GroupName="OV1" OnCheckedChanged="radioOV1_CheckedChanged" AutoPostBack="true" /></td>
                            <td>
                                <asp:TextBox ID="textOV1Thb" runat="server" CssClass="Textbox" Width="70px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireOV1Thb" runat="server" ErrorMessage="กรุณาระบุ OV1(บาท)" ControlToValidate="textOV1Thb" ValidationGroup="EditBenefit" CssClass="style4" Enabled="false"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV1Undefine" runat="server" Text="ไม่ระบุ" GroupName="OV1" OnCheckedChanged="radioOV1_CheckedChanged" AutoPostBack="true" /></td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset>

                    <table>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV2Pct" runat="server" Text="OV2(%)" GroupName="OV2" AutoPostBack="true" OnCheckedChanged="radioOV2_CheckedChanged" /></td>
                            <td>
                                <asp:TextBox ID="textOV2Pct" runat="server" CssClass="Textbox" Width="70px" MaxLength="6"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireOV2Pct" runat="server" ErrorMessage="กรุณาระบุ OV2(%)" ControlToValidate="textOV2Pct" ValidationGroup="EditBenefit" CssClass="style4" Enabled="false"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV2Thb" runat="server" Text="OV2(บาท)" GroupName="OV2" AutoPostBack="true" OnCheckedChanged="radioOV2_CheckedChanged" /></td>
                            <td>
                                <asp:TextBox ID="textOV2Thb" runat="server" CssClass="Textbox" Width="70px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requireOV2Thb" runat="server" ErrorMessage="กรุณาระบุ OV2(บาท)" ControlToValidate="textOV2Thb" ValidationGroup="EditBenefit" CssClass="style4" Enabled="false"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px;"></td>
                            <td style="width: 180px;">
                                <asp:RadioButton ID="radioOV2Undefine" runat="server" Text="ไม่ระบุ" GroupName="OV2" AutoPostBack="true" OnCheckedChanged="radioOV2_CheckedChanged" /></td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
                <table>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;">Vat<span class="style4">*</span></td>
                        <td>
                            <asp:RadioButton ID="radioInVat" runat="server" GroupName="VAT" Text="Include Vat" Checked="true" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="radioExVat" runat="server" GroupName="VAT" Text="Exclude Vat" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;">สถานะ<span class="style4">*</span></td>
                        <td>
                            <asp:RadioButton ID="radioActive" runat="server" GroupName="Status" Text="ใช้งาน" Checked="true" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="radioNoActive" runat="server" GroupName="Status" Text="ไม่ใช้งาน" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height: 10px"></td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 100px;"></td>
                        <td>
                            <asp:Button ID="btnPopupSave" runat="server" Text="บันทึก" Width="100px" ValidationGroup="EditBenefit" OnClick="btnPopupSave_Click" OnClientClick="DisplayProcessing" />&nbsp;
                                <asp:Button ID="btnPopupCancel" runat="server" Text="ยกเลิก" Width="100px" OnClick="btnPopupCancel_Click" OnClientClick="DisplayProcessing" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <act:ModalPopupExtender ID="mpePopupReceiveNo" runat="server" TargetControlID="btnPopupReceiveNo" PopupControlID="pnPopupReceiveNo" BackgroundCssClass="modalBackground" DropShadow="True">
    </act:ModalPopupExtender>

    <script type="text/javascript">
        function validateDecimal(e, sender) {
            var charCode = (e.which) ? e.which : event.keyCode;
            if (charCode == 8)
                return true;

            if (isNaN(Number($(sender).val() + String.fromCharCode(charCode))))
                return false;
            else
                return true;
        }

        function ValidateBenifitType(source, arguments) {

            var rdoCoverageType = $get('<%=rdoCoverageType.ClientID %>');
            var rdoInsuranceType = $get('<%=rdoInsuranceType.ClientID %>');
            
            if (!rdoCoverageType.checked && !rdoInsuranceType.checked) {
                arguments.IsValid = false;
            }
            else {
                arguments.IsValid = true;
            }                      
        }

        function CheckChangeValidator()
        {
            var rdoCoverageType = $get('<%=rdoCoverageType.ClientID %>');
            var rdoInsuranceType = $get('<%=rdoInsuranceType.ClientID %>');
            var cvalBenefitEdit = $get('<%=cvalBenefitEdit.ClientID %>');
            if (rdoCoverageType.checked || rdoInsuranceType.checked) {
                cvalBenefitEdit.setAttribute("style", "visibility:hidden");
            }
        }
    </script>
</asp:Content>

