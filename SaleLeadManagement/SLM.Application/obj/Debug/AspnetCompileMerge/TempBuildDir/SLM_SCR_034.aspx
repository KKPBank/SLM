<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_034.aspx.cs" Inherits="SLM.Application.SLM_SCR_034" %>

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
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div>
                <asp:Image runat="server" ImageUrl="images/hSearch.gif" />
                <table style="margin-left: 30px; margin-bottom: 20px;">
                    <tr>
                        <td style="width: 120px;"><b>ชื่อบริษัทประกัน</b></td>
                        <td style="width: 300px;">
                            <asp:DropDownList runat="server" ID="cmbInsComSearch" Width="250px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                        </td>
                        <td style="width: 150px;"><b>ชื่อไฟล์งานติดปัญหา</b></td>
                        <td>
                            <asp:DropDownList runat="server" ID="cmbFileNameSearch" Width="250px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td colspan="3">
                            <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" Width="100px" OnClick="btnSearch_Click" />
                            &nbsp;
                <asp:Button runat="server" ID="btnClear" Text="ล้างข้อมูล" Width="100px" OnClick="btnClear_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="Line"></div>

            <div style="margin-top: 20px;">
                <asp:Image runat="server" ImageUrl="images/hResult.gif" ImageAlign="Top" />
                <asp:Button ID="btnImportExcel" runat="server" Text="Import Excel" Width="150px"
                    CssClass="Button" Height="23px" OnClick="btnImportExcel_Click" />
            </div>
            <br />
            <br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1080px" Visible="false" />

            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Visible="false"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1000px">
                <Columns>
                    <asp:TemplateField HeaderText="ลำดับที่">
                        <ItemTemplate>
                            <asp:Label ID="lblRowNum" runat="server" Text='<%# Eval("RowNum") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อบริษัทประกัน">
                        <ItemTemplate>
                            <asp:Label ID="lblInsComName" runat="server" Text='<%# Eval("slm_InsNameTh") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อไฟล์">
                        <ItemTemplate>
                            <asp:Label ID="lblFileName" runat="server" Text='<%# Eval("slm_FileName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="จำนวนเคสทั้งหมด">
                        <ItemTemplate>
                            <asp:Label ID="lblTotalCase" runat="server" Text='<%# Eval("TotalCase") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="จำนวนเคสที่ปิดแล้ว">
                        <ItemTemplate>
                            <asp:Label ID="lblOpenCase" runat="server" Text='<%# Eval("OpenCase") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="จำนวนเคสที่ยังไม่ปิด">
                        <ItemTemplate>
                            <asp:Label ID="lblCloseCase" runat="server" Text='<%# Eval("CloseCase") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>

                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upnPopupImport" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupImportExcel" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnPopupImport" Style="display: none" CssClass="modalPopupAddReceiveNo">
                <br />
                <br />
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;">ชื่อบริษัทประกัน<span class="style4">*</span></td>
                        <td colspan="2">
                            <asp:DropDownList ID="cmbInsComImport" runat="server" CssClass="Dropdownlist" Width="200px" DataTextField="TextField" DataValueField="ValueField"></asp:DropDownList>
                            <asp:Label runat="server" ID="lblCompError" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;"></td>
                        <td style="width: 180px;" valign="top">แนบไฟล์<span class="style4">*</span></td>
                        <td>
                            <act:AsyncFileUpload ID="fuData" runat="server" OnClientUploadError="uploadError" PersistedStoreType="Session" PersistFile="true" CompleteBackColor="#efefef" ErrorBackColor="#dddddd" OnClientUploadStarted="ValidateUpload" OnClientUploadComplete="UploadCompleted" />
                        </td>
                        <td>
                            <span class="style4">(xls)</span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                        <td colspan="2">
                            <asp:Label runat="server" ID="lblUploadError" CssClass="style4" ViewStateMode="Disabled"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;"></td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                        <td colspan="2">
                            <asp:Button ID="btnUpload" runat="server" Text="Import" CssClass="Button" Width="80px" OnClick="btnUpload_Click" OnClientClick="DisplayProcessing()" ValidationGroup="ImportProblem" />&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="Button" Width="80px" OnClick="btnCancel_Click" OnClientClick="DisplayProcessing()" />
                        </td>
                    </tr>
                </table>
                <hr />
                <table runat="server" id="tableUploadResult" visible="false" border="1" cellpadding="2" cellspacing="0" rules="all" style="margin-left: 20px">
                    <tr style="background: #ddd;">
                        <td style="font-weight: bold;" colspan="2">ผลการตรวจสอบข้อมูล</td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 100px; padding-right: 5px"><b>Filename</b></td>
                        <td style="width: 200px">
                            <asp:Label ID="lblFilename" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-right: 5px"><b>Total</b></td>
                        <td>
                            <asp:Label ID="lblTotal" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-right: 5px"><b>Data Valid</b></td>
                        <td>
                            <asp:Label ID="lblSuccess" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-right: 5px"><b>Data Invalid</b></td>
                        <td>
                            <asp:Label ID="lblFail" runat="server"></asp:Label></td>
                    </tr>
                </table>
                &nbsp;
                <div style="max-height: 250px; overflow-y: auto; padding: 20px">
                    <b runat="server" id="gvUploadErrorHeader" visible="false">Error Details</b>
                    <asp:GridView ID="gvUploadError" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                        EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="500px">
                        <Columns>
                            <asp:TemplateField HeaderText="Row No">
                                <ItemTemplate>
                                    <asp:Label ID="lblRowNo" runat="server" Text='<%# Eval("ProblemDetailId") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="20px" HorizontalAlign="Center" />
                                <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemTemplate>
                                    <asp:Label ID="lblError" runat="server" Text='<%# Eval("ErrorMessage") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#dddddd" />
                    </asp:GridView>
                </div>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupImport" runat="server" TargetControlID="btnPopupImportExcel" PopupControlID="pnPopupImport" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upnPopupConfirm" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnConfirmImport" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelImport" />
        </Triggers>
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopupConfirm" Width="0px" CssClass="Hidden" />
            <asp:Panel runat="server" ID="pnConfirm" Style="display: none; width: 250px; padding: 30px; text-align: center;" CssClass="modalPopupAddReceiveNo">
                มีข้อมูลนำเข้างานติดปัญหาซ้ำ ต้องการนำเข้าใหม่ใช่หรือไม่
                <br />
                <br />
                <br />
                <br />
                <asp:Button ID="btnConfirmImport" runat="server" Text="Yes" CssClass="Button" Width="80px" OnClick="btnConfirmImport_Click" OnClientClick="DisplayProcessing()" />&nbsp;
                <asp:Button ID="btnCancelImport" runat="server" Text="No" CssClass="Button" Width="80px" OnClick="btnCancelImport_Click" OnClientClick="DisplayProcessing()" />
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupConfirm" runat="server" TargetControlID="btnPopupConfirm" PopupControlID="pnConfirm" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">

        function uploadError(sender, args) {
            var lbl = document.getElementById('<%= lblUploadError.ClientID %>');
            lbl.innerHTML = args.get_errorMessage();
        }

        function ValidateUpload(sender, args) {
            var btnU = document.getElementById('<%= btnUpload.ClientID %>');
            var btnC = document.getElementById('<%= btnCancel.ClientID %>');

            btnU.value = 'loading..';
            btnU.disabled = true;
            btnC.disabled = true;

            var fu = document.getElementById('ctl00_ContentPlaceHolder1_fuData_ctl02');
            var lbl = document.getElementById('<%= lblUploadError.ClientID %>');
            var fname = args.get_fileName();
            var ext = fname.substring(fname.lastIndexOf(".") + 1).toLowerCase();
            var fsize = fu.files[0].size;
            var validsize = 30 * 1024 * 1024;

            lbl.innerHTML = "";

            if (ext != "xls") {
                var err = new Error();
                err.name = "Upload Error";
                err.message = 'กรุณาระบุไฟล์ให้ถูกต้อง (.xls)';
                throw (err);
                return false;
            }

            if (fsize > validsize) {
                var err = new Error();
                err.name = "Upload Error";
                err.message = 'ขนาดไฟล์ต้องไม่เกิน 30MB';
                throw (err);
                return false;
            }

            return true;
        }

        function UploadCompleted(sender, args)
        {
            var btnU = document.getElementById('<%= btnUpload.ClientID %>');
            var btnC = document.getElementById('<%= btnCancel.ClientID %>');

            btnU.value = 'Import';
            btnU.disabled = false;
            btnC.disabled = false;
        }
    </script>
</asp:Content>
