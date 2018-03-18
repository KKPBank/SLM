<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_041.aspx.cs" Inherits="SLM.Application.SLM_SCR_041" %>

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

        .modalPopupImport {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 10px;
            width: 650px;
            position: relative;
            overflow: hidden;
        }

        .style4 {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }

        .telesaleTable .title {
            width: 90px;
            padding-right: 5px;
            text-align: right;
        }

        .telesaleTable .value {
            width: 130px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="4" style="height: 2px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo">ปีเริ่มต้น
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbYearStart" CssClass="Dropdownlist" Width="80px" runat="server">
                        </asp:DropDownList>
                        เดือนเริ่มต้น
                         <asp:DropDownList ID="cmbMonthStart" CssClass="Dropdownlist" Width="80px" runat="server">
                             <asp:ListItem Text="ทั้งหมด"></asp:ListItem>
                             <asp:ListItem Text="มกราคม"></asp:ListItem>
                             <asp:ListItem Text="กุมภาพันธ์"></asp:ListItem>
                             <asp:ListItem Text="มีนาคม"></asp:ListItem>
                             <asp:ListItem Text="เมษายน"></asp:ListItem>
                             <asp:ListItem Text="พฤษภาคม"></asp:ListItem>
                             <asp:ListItem Text="มิถุนายน"></asp:ListItem>
                             <asp:ListItem Text="กรกฎาคม"></asp:ListItem>
                             <asp:ListItem Text="สิงหาคม"></asp:ListItem>
                             <asp:ListItem Text="กันยายน"></asp:ListItem>
                             <asp:ListItem Text="ตุลาคม"></asp:ListItem>
                             <asp:ListItem Text="พฤศจิกายน"></asp:ListItem>
                             <asp:ListItem Text="ธันวาคม"></asp:ListItem>
                         </asp:DropDownList>
                    </td>
                    <td class="ColInfo">ปีสิ้นสุด
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbYearEnd" CssClass="Dropdownlist" Width="80px" runat="server">
                        </asp:DropDownList>
                        เดือนสิ้นสุด
                         <asp:DropDownList ID="cmbMonthEnd" CssClass="Dropdownlist" Width="80px" runat="server">
                             <asp:ListItem Text="ทั้งหมด"></asp:ListItem>
                             <asp:ListItem Text="มกราคม"></asp:ListItem>
                             <asp:ListItem Text="กุมภาพันธ์"></asp:ListItem>
                             <asp:ListItem Text="มีนาคม"></asp:ListItem>
                             <asp:ListItem Text="เมษายน"></asp:ListItem>
                             <asp:ListItem Text="พฤษภาคม"></asp:ListItem>
                             <asp:ListItem Text="มิถุนายน"></asp:ListItem>
                             <asp:ListItem Text="กรกฎาคม"></asp:ListItem>
                             <asp:ListItem Text="สิงหาคม"></asp:ListItem>
                             <asp:ListItem Text="กันยายน"></asp:ListItem>
                             <asp:ListItem Text="ตุลาคม"></asp:ListItem>
                             <asp:ListItem Text="พฤศจิกายน"></asp:ListItem>
                             <asp:ListItem Text="ธันวาคม"></asp:ListItem>
                         </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">Team Code
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbTeamCode" CssClass="Dropdownlist" Width="230px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbTeamCode_SelectedIndexChanged">
                            <asp:ListItem Value="T1" Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem Value="T2" Text="NNPC"></asp:ListItem>
                            <asp:ListItem Value="T3" Text="NNAP"></asp:ListItem>
                            <asp:ListItem Value="T4" Text="NNPS"></asp:ListItem>
                            <asp:ListItem Value="T5" Text="NNLP"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">TTA Name
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbTTAName" CssClass="Dropdownlist" Width="230px" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">Level
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbLevel" CssClass="Dropdownlist" Width="230px" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">ข้อมูลผลงาน Telesales ล่าสุด</td>
                    <td><asp:CheckBox runat="server" ID="chkIsNew" Checked="true" OnCheckedChanged="chkIsNew_CheckedChanged" AutoPostBack="true"/></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
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
            <br />
            <br />
            <div class="Line"></div>
            <br />
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
             <asp:Button ID="btnImport" runat="server" Text="Import Excel" CssClass="Button" Width="150px" OnClick="btnImport_Click" />&nbsp;
            <asp:Button ID="btnExceleExport" runat="server" Text="Export Excel" CssClass="Button" Width="150px" OnClick="btnExceleExport_Click"/>&nbsp;
            <asp:Panel runat="server" ID="pnlResult" Visible="false">
                <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="840px" />
                        <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False"
                            GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                            EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="840px">
                            <Columns>
                                <asp:TemplateField HeaderText="No">
                                    <ItemTemplate>
                                       
                                    </ItemTemplate>
                                    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Team Code">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTeamCode" runat="server" Text='<%# Eval("TeamCode") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="80px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Emp Code">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmpCode" runat="server" Text='<%# Eval("EmpCode") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="TTA Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTTAName" runat="server" Text='<%# Eval("TTAName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Level">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLEVEL" runat="server" Text='<%# Eval("LEVEL") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Year">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLEVEL" runat="server" Text='<%# Eval("slm_Year") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Month">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLEVEL" runat="server" Text='<%# Eval("slm_MonthNameTh") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Performance">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPerformance" runat="server" Text='<%# String.Format("{0:#,##0.00}", Eval("Performance")) %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                        </asp:GridView>
                        <br />

                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>

            <asp:UpdatePanel ID="upnPopupReceiveNo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button runat="server" ID="btnPopupReceiveNo" Width="0px" CssClass="Hidden" />
                    <asp:Panel runat="server" ID="pnPopupReceiveNo" Style="display: none" CssClass="modalPopupImport">
                        <div style="margin: 10px; overflow: hidden;">
                            <br />
                            <table class="telesaleTable">
                                <tr>
                                    <td class="title">ปีนำเข้า <span class="style4">*</span></td>
                                    <td class="value">
                                        <asp:DropDownList runat="server" ID="cmbYear"></asp:DropDownList>
                                        <asp:Label runat="server" ID="lblvyear" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                                    </td>
                                    <td class="title">เดือนนำเข้า <span class="style4">*</span></td>
                                    <td class="value">
                                        <asp:DropDownList runat="server" ID="cmbMonth">
                                            <asp:ListItem Value="0" Text="ระบุเดือน"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="มกราคม"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="กุมภาพันธ์"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="มีนาคม"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="เมษายน"></asp:ListItem>
                                            <asp:ListItem Value="5" Text="พฤษภาคม"></asp:ListItem>
                                            <asp:ListItem Value="6" Text="มิถุนายน"></asp:ListItem>
                                            <asp:ListItem Value="7" Text="กรกฎาคม"></asp:ListItem>
                                            <asp:ListItem Value="8" Text="สิงหาคม"></asp:ListItem>
                                            <asp:ListItem Value="9" Text="กันยายน"></asp:ListItem>
                                            <asp:ListItem Value="10" Text="ตุลาคม"></asp:ListItem>
                                            <asp:ListItem Value="11" Text="พฤศจิกายน"></asp:ListItem>
                                            <asp:ListItem Value="12" Text="ธันวาคม"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label runat="server" ID="lblvmonth" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="title">แนบไฟล์ <span class="style4">*</span> </td>
                                    <td colspan="3">
                                        <act:AsyncFileUpload ID="fuData" runat="server" OnClientUploadError="uploadError" PersistedStoreType="Session" PersistFile="true" CompleteBackColor="#efefef" ErrorBackColor="#dddddd" OnClientUploadStarted="ValidateUpload" />
                                        <asp:Label ID="lblUploadError" runat="server" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:LinkButton runat="server" ID="lbnClear" Text="Clear" OnClick="lbnClear_Click"></asp:LinkButton>
                                        (.xls)
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="title">&nbsp;</td>
                                    <td colspan="4">
                                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="Button" Width="80px" OnClick="btnUpload_Click" OnClientClick="DisplayProcessing()" />
                                        <asp:Button ID="btnCancelUpload" runat="server" Text="Cancel" CssClass="Button" Width="80px" />
                                    </td>
                                </tr>

                            </table>
                            <br />
                            <div class="Line"></div>
                            <br />
                            <table runat="server" id="tbResult" visible="false" border="1" cellpadding="2" cellspacing="0" rules="all">
                                <tr style="background: #ddd;">
                                    <td style="font-weight: bold;" colspan="2">ผลการตรวจสอบข้อมูล</td>
                                </tr>
                                <tr>
                                    <td style="width: 100px; font-weight: bold; text-align: right; padding-right: 10px">Filename</td>
                                    <td>
                                        <asp:Label runat="server" ID="lblFilename"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="width: 100px; font-weight: bold; text-align: right; padding-right: 10px">Total</td>
                                    <td>
                                        <asp:Label runat="server" ID="lblTotal"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="width: 100px; font-weight: bold; text-align: right; padding-right: 10px">Data Valid</td>
                                    <td>
                                        <asp:Label runat="server" ID="lblSucc"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="width: 100px; font-weight: bold; text-align: right; padding-right: 10px">Data Invalid</td>
                                    <td>
                                        <asp:Label runat="server" ID="lblFail"></asp:Label></td>
                                </tr>
                            </table>
                            &nbsp;
                            <asp:Panel runat="server" ID="pnlError" Visible="false">
                                <b>Error Details</b>
                                <div style="height: 200px; overflow: auto;">
                                    <asp:GridView runat="server" ID="gvUploadResult" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField HeaderText="Row No" DataField="ValueField">
                                                <ItemStyle HorizontalAlign="Center" Width="80px" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="Description" DataField="TextField" HtmlEncode="false">
                                                <ItemStyle Width="400px" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle BackColor="#dddddd" />
                                    </asp:GridView>
                                </div>
                                <br />
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                    <act:ModalPopupExtender ID="popImport" runat="server" TargetControlID="btnPopupReceiveNo" PopupControlID="pnPopupReceiveNo" BackgroundCssClass="modalBackground" DropShadow="True" CancelControlID="btnCancelUpload">
                    </act:ModalPopupExtender>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">

        function uploadError(sender, args) {
            var lbl = document.getElementById('<%= lblUploadError.ClientID %>');
            lbl.innerHTML = args.get_errorMessage(); 
        }

        function ValidateUpload(sender, args)
        {
            var fu = document.getElementById('ctl00_ContentPlaceHolder1_fuData_ctl02');
            var lbl = document.getElementById('<%= lblUploadError.ClientID %>');
            var fname = args.get_fileName();
            var ext = fname.substring(fname.lastIndexOf(".") + 1).toLowerCase();
            var fsize = fu.files[0].size;
            var validsize = 30 * 1024 * 1024;

            lbl.innerHTML = "";

            if (ext != "xls")
            {
                var err = new Error();
                err.name = "Upload Error";
                err.message = 'กรุณาระบุไฟล์ให้ถูกต้อง (.xls)';
                throw (err);
                return false;
            }

            if (fsize > validsize)
            {
                var err = new Error();
                err.name = "Upload Error";
                err.message = 'ขนาดไฟล์ต้องไม่เกิน 30MB';
                throw (err);
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
