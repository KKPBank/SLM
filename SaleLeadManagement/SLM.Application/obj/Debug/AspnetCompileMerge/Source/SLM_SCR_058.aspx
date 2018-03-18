<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_058.aspx.cs" Inherits="SLM.Application.SLM_SCR_058" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColErrorInfo
        {
            font-weight:bold;
            width:100px;
            border-color:lightgray;
            height:24px;
            padding-left:2px;
        }
        .ColErrorInput
        {
            width:300px;
            border-color:lightgray;
            height:24px;
            padding-left:2px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function ConfirmUpload() {
            var i = 0;
            var alertupload = document.getElementById('<%= lblAlertUpload.ClientID %>');
            var fulead = document.getElementById('<%= fuLead.ClientID %>');
            var filetype = '<%= SLM.Resource.SLMConstant.ReAssignLead.ReAssignFileType %>';
            
            if (fulead != null) {
                if (fulead.files[0] == null) {
                    alertupload.innerHTML = '*';
                    i += 1;
                }
                else {
                    var filePath = fulead.value;
                    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
                    var newfilename = filePath.substring(filePath.lastIndexOf('\\') + 1);
                    var types = filetype.split(',');

                    if (types.indexOf(ext) == -1) {
                        alertupload.innerHTML = 'กรุณาระบุไฟล์ Excel (' + filetype + ') เท่านั้น';
                        i += 1;
                    }
                    else {
                        alertupload.innerHTML = '';
                    }
                }
            }
  
            if (i > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr><td colspan="2" style="height:2px;"></td></tr>
        <tr>
            <td style="width:70px; font-weight:bold;">
                แนบไฟล์
            </td>
            <td colspan="3">
                <asp:FileUpload ID="fuLead" runat="server" Width="300px" BackColor="#e5edf5" BorderStyle="Solid" BorderWidth="1px" BorderColor="#7f9db9" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="Button" Width="100px" OnClientClick="if (ConfirmUpload()) { DisplayProcessing(); return true; } else { return false; }" OnClick="btnUpload_Click"  />&nbsp;
                <asp:Label ID="lblAlertUpload" runat="server"  ForeColor="Red" ></asp:Label>&nbsp;
            </td>
        </tr>
    </table>
    <br />
    <hr />
    <br />
    <asp:Panel ID="pnSummary" runat="server" Visible="false">
        <table cellpadding="2" cellspacing="0" border="1" style="border-collapse:collapse; border-color:lightgray;">
            <tr>
                <td colspan="2" style="height:25px; background-color:aliceblue;">
                    <b>ผลการตรวจสอบข้อมูล</b>
                </td>
            </tr>
            <tr>
                <td class="ColErrorInfo">Filename</td>
                <td class="ColErrorInput">
                    <asp:Label ID="lblFilename" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ColErrorInfo">Total</td>
                <td class="ColErrorInput">
                    <asp:Label ID="lblTotal" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ColErrorInfo">Data Valid</td>
                <td class="ColErrorInput">
                    <asp:Label ID="lblDataValid" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ColErrorInfo">Data Invalid</td>
                <td class="ColErrorInput">
                    <asp:Label ID="lblDataInvalid" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <asp:Repeater ID="rptError" runat="server" ClientIDMode="AutoID">
            <HeaderTemplate>
                <table cellpadding="2" cellspacing="0" border="1" style="border-collapse:collapse; border-color:lightgray;">
                    <tr style="background-color:aliceblue;">
                        <td align="center" style="width:100px; font-weight:bold; height:24px; border-color:lightgray;">Row No</td>
                        <td align="center" style="width:480px; font-weight:bold; height:24px; border-color:lightgray;">Description</td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <td align="center" style="vertical-align:top; height:24px; border-color:lightgray;"><%# Eval("RowNo") %></td>
                        <td align="left" style="vertical-align:top; height:24px; border-color:lightgray;"><%# Eval("ErrorMessage") %></td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    
</asp:Content>
