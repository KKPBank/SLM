<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_033.aspx.cs" Inherits="SLM.Application.SLM_SCR_033" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .required { color: red; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
        <ContentTemplate>
           <br />
            <table>
                <tr>
                    <td style="width:100px"><b>ผลิตภัณฑ์</b> <span class="required">*</span></td>
                    <td colspan="2"><asp:DropDownList runat="server" ID="cmbProduct" Width="380px"></asp:DropDownList>
                        <asp:Label runat="server" ID="lblPdError" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td><b>แนบไฟล์</b> <span class="required">*</span></td>
                    <td>
                        <act:AsyncFileUpload ID="fuData" runat="server" OnClientUploadError="uploadError" PersistedStoreType="Session" PersistFile="true" CompleteBackColor="#efefef" ErrorBackColor="#dddddd" OnClientUploadStarted="ValidateUpload" Width="230px" />
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="lnbClear" Text="Clear" OnClick="lnbClear_Click"></asp:LinkButton> (.xls)
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
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="Button" Width="80px" OnClick="btnUpload_Click" OnClientClick="DisplayProcessing()" />
                    </td>
                </tr>
            </table> 
            <div class="Line"></div>
            &nbsp;
            <table  runat="server" id="tbResult" visible="false" border="1" cellpadding="2" cellspacing="0" rules="all"  >
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
            <asp:GridView runat="server" ID="gvError" AutoGenerateColumns="false" AllowPaging="true" PageSize="100" OnPageIndexChanging="gvError_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="ValueField" HeaderText="Row No"   >
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TextField" HeaderText="Description" HtmlEncode="false" >
                        <ItemStyle Width="400px" />
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
