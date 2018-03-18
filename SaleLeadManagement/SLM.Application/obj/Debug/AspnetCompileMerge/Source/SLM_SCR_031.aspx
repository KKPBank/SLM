<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_031.aspx.cs" Inherits="SLM.Application.SLM_SCR_031" %>
<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo {
            font-weight: bold;
            width: 190px;
        }

        .ColInput {
            width: 400px;
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
            width: 480px;
            /*height: 300px;*/
        }
         
        .modalPopupEdit {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 1200px;
            /*height: 400px;*/
            height: 600px;
            overflow-y: auto;
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

        .error {
            color: red;
        }
        .modalPopupConfirm
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:330px;
            height:130px;
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery-2.2.0.min.js"></script>
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
                <tr>
                    <td class="ColInfo">ผลิตภัณฑ์<span class="style4">*</span>
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" CssClass="Dropdownlist" runat="server" Width="250px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredProductSearch" runat="server" ErrorMessage="กรุณาระบุผลิตภัณฑ์"
                                ControlToValidate="cmbProductSearch" InitialValue="-1" ValidationGroup="search" CssClass="style4"></asp:RequiredFieldValidator>
                    </td>

                </tr>
                <tr>
                    <td class="ColInfo">บริษัทประกัน
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbInsNameSearch" CssClass="Dropdownlist" runat="server" Width="250px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                    </td>

                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td class="ColInfo">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="if(Page_ClientValidate('search')) DisplayProcessing();" ValidationGroup="search"
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
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
            <br />
            <br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1230px" Visible="false" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Visible="false"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1330px">
                <Columns>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# String.Format("{0},{1}", Eval("InsComId"), Eval("ProductId")) %>' ToolTip="แก้ไขเลขรับแจ้ง" OnClick="imbEdit_Click" OnClientClick="DisplayProcessing()" />
                    </ItemTemplate>
                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                </asp:TemplateField>
               
                    <asp:BoundField DataField="ProductName" HeaderText="ผลิตภัณฑ์" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" HeaderStyle-HorizontalAlign="Center" />

                <asp:TemplateField HeaderText="รหัสบริษัทประกันภัย">
                    <ItemTemplate>
                            <asp:Label ID="lblInsComCode" runat="server" Text='<%# Eval("InsComCode") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="140px" HorizontalAlign="Center" />
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="ชื่อบริษัทประกันภัย">
                    <ItemTemplate>
                        <asp:Label ID="lblInsComName" runat="server" Text='<%# Eval("InsComName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                    <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Lot ที่">
                    <ItemTemplate>
                            <asp:Label ID="lblLot" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("Lot").ToString() %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Code Name">
                    <ItemTemplate>
                            <asp:Label ID="lblCodename" runat="server" Text='<%# Eval("CodeName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="เลขรับแจ้งเริ่ม">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoStart" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" :  Eval("ReceiveNoStart") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="เลขรับแจ้งสิ้นสุด">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoEnd" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("ReceiveNoEnd")  %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="จำนวนเลขรับแจ้ง<br />ที่ใช้ได้ทั้งหมด">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoTotal" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("ReceiveNoTotal")   %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="จำนวนเลขรับแจ้ง<br />ที่ใช้แล้ว">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoUsed" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("ReceiveNoUsed")   %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="จำนวนเลขรับแจ้ง<br />ที่ถูกยกเลิก">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoCancel" runat="server" Text='<%# Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("ReceiveNoCancel")  %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="จำนวนเลขรับแจ้ง<br />ที่สามารถใข้ได้">
                    <ItemTemplate>
                            <asp:Label ID="lblReceiveNoUnUsed" runat="server" Text='<%#  Eval("Lot") == null || Eval("Lot").ToString() == "0" ? "" : Eval("ReceiveNoRemain")  %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ผู้บันทึก">
                    <ItemTemplate>
                            <asp:Label ID="lblUpdateBy" runat="server" Text='<%# Eval("UpdaterName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="สถานะ">
                    <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("IsDeleted") == null || (bool)Eval("IsDeleted") == true ? "ไม่ใช้งาน" : "ใช้งาน" %>'></asp:Label>
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

    <asp:UpdatePanel ID="upnPopupEdit" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnPopupDoImportExcel" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnClose" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <asp:Button runat="server" ID="btnEditPopup" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupEdit" Style="display: none" CssClass="modalPopupEdit">

                <asp:Image ID="image1" runat="server" ImageUrl="~/Images/hManageReceive.jpg" />
                <br />
                <br />
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">ผลิตภัณฑ์</td>
                            <td>
                            <asp:TextBox ID="textProductName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" Text="001"></asp:TextBox>
                            <asp:HiddenField ID="hiddenProductId" runat="server" />
                            </td>
                        </tr>
                        <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">ชื่อบริษัทประกัน</td>
                            <td>
                            <asp:TextBox ID="textInsName" runat="server" CssClass="TextboxView" Width="250px" ReadOnly="true" Text="แอลเอ็มจีประกันภัย"></asp:TextBox>
                            <asp:HiddenField ID="hiddenInsComId" runat="server" />
                            <asp:Button ID="btnImportExcel" runat="server" Text="เพิ่มเลขรับแจ้ง" Width="120px"
                                CssClass="Button" Height="23px" OnClick="btnImportExcel_Click" OnClientClick="AddReceiveNo(); return false;" />
                            </td>
                        </tr>
                </table>
                <asp:ValidationSummary ShowMessageBox="true" runat="server" ShowSummary="false" ValidationGroup="ValidateAddReceiveNo" CssClass="error" ID="validationSummary1" />
                <br />
                <table cellpadding="2" cellspacing="0" border="0" id="tblAdd" style="display: none; visibility: visible;">
                        <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">Codename<span class="style4">*</span></td>
                            <td>
                            <asp:TextBox ID="textCodeName" runat="server" CssClass="Textbox" Width="100px" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredCodeName" runat="server" ErrorMessage="กรุณาระบุ Codename" Display="Dynamic" ControlToValidate="textCodeName" ValidationGroup="ValidateAddReceiveNo" CssClass="error"></asp:RequiredFieldValidator>
                            </td>
                        <td colspan="3"></td>
                        </tr>
                          <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">เลขรับแจ้งเริ่ม<span class="style4">*</span></td>
                            <td>
                            <asp:TextBox ID="textStartReceiveNoNumber" runat="server" CssClass="TextboxR" Width="100px" MaxLength="18"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredStartReceiveNoNumber" runat="server" ErrorMessage="กรุณาระบุเลขรับแจ้งเริ่ม" Display="Dynamic" ControlToValidate="textStartReceiveNoNumber" ValidationGroup="ValidateAddReceiveNo" CssClass="error"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="compareStartReceiveNoRange" runat="server" ValidationGroup="ValidateAddReceiveNo" CssClass="error" ControlToValidate="textStartReceiveNoNumber" ErrorMessage="กรณาระบุไม่เกิน 50,000 เลข" ClientValidationFunction="valStart"></asp:CustomValidator>
                            </td>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">เลขรับแจ้งสิ้นสุด<span class="style4">*</span></td>
                            <td>
                            <asp:TextBox ID="textEndReceiveNoNumber" runat="server" CssClass="TextboxR" Width="100px" MaxLength="18" onblur="CalcTotalReceiveNo();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredEndReceiveNoNumber" runat="server" ErrorMessage="กรุณาระบุเลขรับแจ้งสิ้นสุด" ControlToValidate="textEndReceiveNoNumber" ValidationGroup="ValidateAddReceiveNo" CssClass="error" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compareStartEndReceiveNo" runat="server" ValidationGroup="ValidateAddReceiveNo" CssClass="error" ControlToValidate="textEndReceiveNoNumber" ControlToCompare="textStartReceiveNoNumber" Operator="GreaterThanEqual" Type="Integer" ErrorMessage="เลขที่รับแจ้งเริ่มต้องน้อยกว่าเลขที่รับแจ้งสิ้นสุด" Display="Dynamic"></asp:CompareValidator>
                            <asp:CustomValidator ID="compareEndReceiveNoRange" runat="server" ValidationGroup="ValidateAddReceiveNo" CssClass="error" ControlToValidate="textEndReceiveNoNumber" ErrorMessage="กรณาระบุไม่เกิน 50,000 เลข" ClientValidationFunction="valEnd"></asp:CustomValidator>
                                <script type="text/javascript">
                                    function valEnd(oSrc, args)
                                    {
                                        args.IsValid = args.Value - parseInt(document.getElementById('<%= textStartReceiveNoNumber.ClientID %>').value) < 50000;
                                    }
                                    function valStart(oSrc, args)
                                    {
                                        var txt = document.getElementById('<%= textEndReceiveNoNumber.ClientID %>').value;
                                        args.IsValid = (parseInt(txt) - args.Value < 50000) || txt == '';
                                    }
                                </script>
                            </td>
                        </tr>
                         <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 120px;">จำนวนเลขรับแจ้งที่ใช้ได้ทั้งหมด</td>
                            <td>
                            <asp:TextBox ID="textTotalReceiveNoNumber" runat="server" CssClass="TextboxViewR" Width="100px" Text="0" MaxLength="18"></asp:TextBox>
                            </td>
                        <td colspan="3"></td>
                        </tr>
                        <%--<tr>
                        <td colspan="3" style="height: 10px"></td>
                        </tr>--%>
                        <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 100px;"></td>
                        <td colspan="3">
                            <asp:Button ID="btnPopupDoImportExcel" runat="server" Text="บันทึก" Width="100px" ValidationGroup="ValidateAddReceiveNo" OnClick="btnPopupDoImportExcel_Click"  OnClientClick="validate()"/>&nbsp;
                            <asp:Button ID="btnPopupCancel" runat="server" Text="ยกเลิก" Width="100px" OnClientClick="CancelAdd(); return false;" />
                            </td>
                        </tr>
                    </table>
                <br style="height: 10px" />
                    
                <uc2:GridviewPageController ID="pcReceiveNo" runat="server" OnPageChange="pcReceiveNo_PageSearchChange" Width="1180px" />
                <div style="overflow-x: auto; overflow-y:hidden; height:310px">
                    <asp:GridView ID="gvReceiveNoList" runat="server" AutoGenerateColumns="False"
                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1400px">
                        <Columns>
                            <asp:TemplateField HeaderText="สถานะ">
                                <ItemTemplate>
                                    <asp:RadioButton runat="server" Text="ใช้งาน" Checked='<%# !(bool)Eval("IsDeleted") %>' GroupName='<%# String.Format("rb{0}", Eval("ReceiveNoId")) %>' AutoPostBack="true" ID="radioActive"  OnCheckedChanged="radioActive_CheckedChanged"  />
                                    <asp:RadioButton runat="server" Text="ไม่ใช้งาน" Checked='<%# Eval("IsDeleted") %>' GroupName='<%# String.Format("rb{0}", Eval("ReceiveNoId")) %>' AutoPostBack="true" ID="radioDeactive"  OnCheckedChanged="radioDeactive_CheckedChanged"  />
                                    <asp:HiddenField ID="hiddenReceiveNoId" runat="server" Value='<%# Eval("ReceiveNoId") %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="130" />
                                <ItemStyle VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:BoundField HeaderStyle-Width="50" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="Lot ที่" DataField="Lot" />
                            <asp:BoundField HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" HeaderText="Code Name" DataField="CodeName" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="เลขที่รับแจ้งเริ่ม" DataField="ReceiveNoStart" DataFormatString="{0:0}" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="เลขที่รับแจ้งสิ้นสุด" DataField="ReceiveNoEnd" DataFormatString="{0:0}" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="จำนวนเลขที่รับแจ้งที่ใช้ได้ทั้งหมด" DataField="ReceiveNoTotal" DataFormatString="{0:0}" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="จำนวนเลขที่รับแจ้งที่ใช้แล้ว" DataField="ReceiveNoUsed" DataFormatString="{0:0}" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="จำนวนเลขที่รับแจ้งที่ถูกยกเลิก" DataField="ReceiveNoCancel" DataFormatString="{0:0}" />
                            <asp:BoundField HeaderStyle-Width="110" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" HeaderText="จำนวนเลขที่รับแจ้งที่สามารถใช้ได้" DataField="ReceiveNoRemain" DataFormatString="{0:0}" />
                            
                        <asp:TemplateField HeaderText="วันที่สร้าง">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                            <asp:BoundField HeaderText="ผู้สร้าง" DataField="CreatedBy" />
                                                        
                        <asp:TemplateField HeaderText="วันที่แก้ไขข้อมูล">
                            <ItemTemplate>
                                <%# Eval("UpdatedDate") != null ? Convert.ToDateTime(Eval("UpdatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("UpdatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("UpdatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                            <asp:BoundField HeaderText="ผู้แก้ไข" DataField="UpdatedBy" />
                        </Columns>
                        <HeaderStyle CssClass="t_rowhead" />
                        <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                    </asp:GridView>
                </div>
                    <br />
                    <table width="100%">
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 100px;"></td>
                            <td align="right">
                            <asp:Button ID="btnClose" runat="server" Text="ปิดหน้าต่าง" Width="100px" OnClick="btnClose_Click" />
                            </td>
                        </tr>
                    </table>
	            </asp:Panel>
	            <act:ModalPopupExtender ID="mpePopupEdit" runat="server" TargetControlID="btnEditPopup" PopupControlID="pnPopupEdit" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button runat="server" ID="btnPopupConfirm" Width="0px" CssClass="Hidden"/>
	<asp:Panel runat="server" ID="pnPopupConfirm" style="display:none" CssClass="modalPopupConfirm">
        <asp:UpdatePanel ID="upConfirm" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="2" style="height:5px;"></td></tr>
                    <tr>
                        <td style="width:40px;"></td>
                        <td class="ColInfo" style="font-size:12px; width:380px;">
                            <asp:Label ID="lblAlertMessage" runat ="server"></asp:Label>
                            <asp:TextBox ID="txtEditRowIndex" runat="server" Width="40px" Visible="false" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr><td colspan="2" style="height:25px;"></td></tr>
                    <tr id="trConfirmOK" runat="server">
                        <td colspan="2" align="center">
                            <asp:Button ID="btnAlertOK" runat="server" Text="OK" CssClass="Button" OnClick="btnAlertOK_Click" Width="100px" />
                        </td>
                    </tr>
                    <tr id="trComfirmYesNo" runat="server">
                        <td style="width:80px;"></td>
                        <td class="ColInfo">
                            <asp:Button ID="btnConfirmYes" runat="server" Text="ใช่" CssClass="Button" OnClientClick="DisplayProcessing();"
                                Width="100px" OnClick="btnConfirmYes_Click" />&nbsp;
                            <asp:Button ID="btnConfirmNo" runat="server" Text="ไม่ใช่" CssClass="Button" 
                                Width="100px" OnClick="btnConfirmNo_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <act:ModalPopupExtender ID="mpePopupConfirm" runat="server" TargetControlID="btnPopupConfirm" PopupControlID="pnPopupConfirm" BackgroundCssClass="modalBackground modalBackgroundProcessing" DropShadow="True">
	</act:ModalPopupExtender>

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
        function AddReceiveNo() {
            $('#tblAdd').css('display', 'inline').css('visibility', 'visible');
        }

        function CancelAdd() {
            $('#tblAdd').css('display', 'none').css('visibility', 'hidden');
            $("[id$='textCodeName']").val('');            
            $("[id$='textStartReceiveNoNumber']").val('');            
            $("[id$='textEndReceiveNoNumber']").val('');
            $("[id$='textTotalReceiveNoNumber']").val('');
            
        }
        function CalcTotalReceiveNo() {
            var start = Number($('input[id$=textStartReceiveNoNumber]').val());
            var end = Number($('input[id$=textEndReceiveNoNumber]').val());
            if (!isNaN(start) && !isNaN(end) && start <= end) {
                var total = end - start + 1;
                $('input[id$=textTotalReceiveNoNumber]').val(total);
            }
        }

        function canChangeActiveFlag()
        {
            if ($('input[id*=radioActive]:checked').length > 1)
            {
                alert("มี Lot ที่ใช้งานอยู่แล้ว ไม่สามารถเปลี่ยนสถานะได้");
                return false;
            }
            if(confirm('ต้องการเปลี่ยนสถานะใช่หรือไม่?'))
                return true;

            return false;
        }

        function validate()
        {
            if (Page_ClientValidate())
                DisplayProcessing();
        }
    </script>
    
</asp:Content>
