<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_040.aspx.cs" Inherits="SLM.Application.SLM_SCR_040" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .required { color:red; }
    </style>
    <script language="javascript" type="text/javascript">

        function ConfirmSave() {
            var alertMonth = $("#<%=lblAlertMonth.ClientID%>");
            var alertYear = $("#<%=lblAlertYear.ClientID%>");
            var monthVal = $("#<%=cmbMonth.ClientID%>").val();
            var yearVal = $("#<%=cmbYear.ClientID%>").val();
            var i = 0;

            if (monthVal == "") {
                i += 1;
            }

            if (yearVal == "") {
                i += 1;
            }

            if (i == 0) {
                return true;
            }
            else {
                return false;
            }
        }

<%--        function ChangeImportType() {
            var typeValue = $("#<%=cmbImportType.ClientID%>").val();
            if (typeValue == "1") {
                $("#<%=lblRemark.ClientID%>").text("ระบบจะนำเข้าข้อมูลแจ้งเบี้ยโดย Batch สิ้นวัน");
            }
            else {
                $("#<%=lblRemark.ClientID%>").text("");
            }
        }--%>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
        <ContentTemplate>
            <br />
            <table>
                <tr style="vertical-align:top;">
                    <td style="font-weight:bold; width:70px">
                        ประเภท
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upImportType" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="cmbImportType" runat="server" CssClass="Dropdownlist" Width="150px" AutoPostBack="true" OnSelectedIndexChanged="cmbImportType_SelectedIndexChanged">
                                    <asp:ListItem Text="Batch สิ้นวัน" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Realtime" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="lblRemark" runat="server" ForeColor="Red" Font-Bold="true" Text="ระบบจะนำเข้าข้อมูลแจ้งเบี้ยโดย Batch สิ้นวัน"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            <table>
                <tr style="vertical-align:top;">
                    <td style="font-weight:bold; width:70px">
                        เดือน
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbMonth" runat="server" CssClass="Dropdownlist" Width="150px">
                        </asp:DropDownList>
                        <asp:Label ID="lblAlertMonth" runat="server" ForeColor="Red" Text="*"></asp:Label>
                    </td>
                    <td>
                        <b>ปี</b>&nbsp;&nbsp;
                        <asp:DropDownList ID="cmbYear" runat="server" CssClass="Dropdownlist" Width="60px"></asp:DropDownList>
                        <asp:Label ID="lblAlertYear" runat="server" ForeColor="Red" Text="*"></asp:Label>
                    </td>
                </tr>
                <tr><td colspan="3" style="height:4px;"></td></tr>
                <tr style="vertical-align:top;">
                    <td style="width:70px"><b>แนบไฟล์</b> </td>
                    <td style="height:20px;">
                        <act:AsyncFileUpload ID="fuData" runat="server" OnClientUploadError="uploadError" PersistedStoreType="Session" PersistFile="true" CompleteBackColor="#efefef" ErrorBackColor="#dddddd" OnClientUploadStarted="ValidateUpload" OnClientUploadComplete="ClientUploadComplete" ThrobberID="divLoading" Width="230px" />
                        <div id="divLoading" runat="server">
                            <asp:Image ID="imgWaitingfuData" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;<asp:Label ID="lblLoading" runat="server" Font-Bold="true" Text="Loading..."></asp:Label>
                        </div>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="lnbClear" OnClick="lnbClear_Click">Clear</asp:LinkButton> (.xls)
                        <span class="required">*</span>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <asp:Label ID="lblUploadError" runat="server" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="Button" Width="80px" OnClick="btnUpload_Click" OnClientClick="if (ConfirmSave()) { DisplayProcessing(); return true; } else { return false; } " />
                    </td>
                </tr>
            </table>
            &nbsp;
            <div class="Line"></div>
            &nbsp;
            <table  runat="server" id="tbResult" visible="false" border="1" cellpadding="2" cellspacing="0" rules="all">
                <tr style="background:#ddd;">
                    <td style="font-weight: bold;" colspan="2">ผลการตรวจสอบข้อมูล</td>
                </tr>
                <tr>
                    <td style="width:100px;font-weight: bold; text-align: right; padding-right: 10px">Filename</td>
                    <td><asp:Label runat="server" ID="lblFilename"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width:100px;font-weight: bold; text-align: right; padding-right: 10px">Total</td>
                    <td><asp:Label runat="server" ID="lblTotal"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width:100px;font-weight: bold; text-align: right; padding-right: 10px">Data Valid</td>
                    <td><asp:Label runat="server" ID="lblSucc"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width:100px;font-weight: bold; text-align: right; padding-right: 10px">Data Invalid</td>
                    <td><asp:Label runat="server" ID="lblFail"></asp:Label></td>
                </tr>
            </table>
            &nbsp;
            <asp:Panel runat="server" ID="pnlError" Visible="false">
            <b>Error Details</b>
            <asp:GridView runat="server" ID="gvError" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="ValueField" HeaderText="Row No"   >
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TextField" HeaderText="Description" HtmlEncode="false" >
                        <ItemStyle Width="500px" />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle BackColor="#dddddd" />
            </asp:GridView>
            <br />
            </asp:Panel>
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
            var btnUpload = document.getElementById('<%= btnUpload.ClientID %>');

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

            $("#<%=btnUpload.ClientID%>").prop("disabled", true);
            return true;
        }

        function ClientUploadComplete() {
            $("#<%=btnUpload.ClientID%>").prop("disabled", false);
        }
    </script>
</asp:Content>
