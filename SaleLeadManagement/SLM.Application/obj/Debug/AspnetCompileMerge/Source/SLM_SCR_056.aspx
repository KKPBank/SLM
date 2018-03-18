<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_056.aspx.cs" Inherits="SLM.Application.SLM_SCR_056" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:110px;
        }
        .ColInput
        {
            width:250px;
        }
        .modalPopupError
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:590px;
            height:470px;
        }

        /* เรียกใช้จาก SlmScript.js */
        .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -1px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
            z-index:20000 !important;
        }
        .ui-autocomplete  
        {
            height: 220px; width:400px; overflow-y: scroll; overflow-x: hidden;
            /* add padding to account for vertical scrollbar */
            padding-right: 5px;
            z-index:20000 !important;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function ConfirmUpload() {
            var i = 0;
            var ddlchannel = document.getElementById('<%= cmbChannel.ClientID %>');
            var ddlcampaign = document.getElementById('<%= cmbCampaign.ClientID %>');
            var alertchannel = document.getElementById('<%= lblAlertChannel.ClientID %>');
            var alertcampaign = document.getElementById('<%= lblAlertCampaign.ClientID %>');
            var alertupload = document.getElementById('<%= lblAlertUpload.ClientID %>');
            var fulead = document.getElementById('<%= fuLead.ClientID %>');
            var hdfexistingfilename = document.getElementById('<%= hdfExistingFilename.ClientID %>');
            var filetype = '<%= SLM.Application.Utilities.AppConstant.UploadFileType %>';
            
            if (ddlchannel.options[ddlchannel.selectedIndex].value == '') {
                alertchannel.innerHTML = '*';
                i += 1;
            }
            else {
                alertchannel.innerHTML = '';
            }

            if (ddlcampaign.options[ddlcampaign.selectedIndex].value == '') {
                alertcampaign.innerHTML = '*';
                i += 1;
            }
            else {
                alertcampaign.innerHTML = '';
            }

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

                    //if (ext != 'xls' && ext != 'xlsx') {
                    //    alertupload.innerHTML = 'กรุณาระบุไฟล์ Excel เท่านั้น';
                    //    i += 1;
                    //}

                    if (types.indexOf(ext) == -1) {
                        alertupload.innerHTML = 'กรุณาระบุไฟล์ Excel (' + filetype + ') เท่านั้น';
                        i += 1;
                    }
                    else if (hdfexistingfilename.value != '' && hdfexistingfilename.value.toLowerCase() != newfilename.toLowerCase()) {
                        //alertupload.innerHTML = 'ชื่อไฟล์ไม่ตรงกับของเดิม (' + hdfexistingfilename.value + ')';
                        alertupload.innerHTML = 'ชื่อไฟล์ไม่ตรงกับของเดิม';
                        i += 1;
                    }
                    else
                        alertupload.innerHTML = '';
                }
            }
  
            if (i > 0) {
                return false;
            }
            else {
                return true;
            }
        }

        function ConfirmSave() {
            var i = 0;
            var ddlchannel = document.getElementById('<%= cmbChannel.ClientID %>');
            var ddlcampaign = document.getElementById('<%= cmbCampaign.ClientID %>');
            var alertchannel = document.getElementById('<%= lblAlertChannel.ClientID %>');
            var alertcampaign = document.getElementById('<%= lblAlertCampaign.ClientID %>');
            var table = document.getElementById('<%= gvResult.ClientID %>');
            var rows = table.getElementsByTagName("tr");

            if (ddlchannel.options[ddlchannel.selectedIndex].value == '') {
                alertchannel.innerHTML = '*';
                i += 1;
            }
            else {
                alertchannel.innerHTML = '';
            }

            if (ddlcampaign.options[ddlcampaign.selectedIndex].value == '') {
                alertcampaign.innerHTML = '*';
                i += 1;
            }
            else {
                alertcampaign.innerHTML = '';
            }

            //rows.length == 1, only header
            if (rows.length <= 1) {
                i += 1;
            }

            if (i > 0) {
                alert('กรุณาระบุข้อมูลให้ครบถ้วน');
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript">
        Sys.Application.add_load(function () {

            $("#<%= cmbCampaign.ClientID %>").combobox({
                select: function (event, ui) {
                    //DoPostBack(this);
                },
                notfound: function (event) {
                    <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
                },
                cleared: function () {
                    <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
                }
            });
        });
    </script>

    <br />
    <asp:HiddenField ID="hdfFilename" runat="server"  />
    <asp:HiddenField ID="hdfUploadLeadId" runat="server"  />
    <asp:HiddenField ID="hdfExistingFilename" runat="server"  />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr><td colspan="4" style="height:2px;"></td></tr>
        <tr>
            <td class="ColInfo" style="padding-left:15px;">
                ช่องทาง
            </td>
            <td class="ColInput">
                <asp:DropDownList ID="cmbChannel" runat="server" CssClass="Dropdownlist" Width="220px">
                </asp:DropDownList>&nbsp;
                <asp:Label ID="lblAlertChannel" runat="server"  ForeColor="Red" ></asp:Label>
            </td>
            <td class="ColInfo" >
                แคมเปญ
            </td>
            <td >
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td style="width:208px;">
                            <asp:UpdatePanel ID="upCampaign" runat="server" >
                                <ContentTemplate>
                                    <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="300px">
                                    </asp:DropDownList>&nbsp;
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td style="width:30px;">
                            <asp:Label ID="lblAlertCampaign" runat="server"  ForeColor="Red"  ></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td colspan="4" style="height:1px;"></td></tr>
        <tr>
            <td class="ColInfo" style="padding-left:15px;">
                ชื่อไฟล์
            </td>
            <td colspan="3">
                <asp:TextBox ID="txtFileName" runat="server" ReadOnly="true" CssClass="TextboxView" Width="219px"></asp:TextBox>
            </td>
        </tr>
        <tr><td colspan="4" style="height:2px;"></td></tr>
        <tr>
            <td class="ColInfo">
            </td>
            <td colspan="3">
                <asp:FileUpload ID="fuLead" runat="server" Width="362px" BackColor="#e5edf5" BorderStyle="Solid" BorderWidth="1px" BorderColor="#7f9db9" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="Button" Width="100px" OnClientClick="if (ConfirmUpload()) { DisplayProcessing(); return true; } else { return false; }" OnClick="btnUpload_Click"  />&nbsp;
                <asp:Label ID="lblAlertUpload" runat="server"  ForeColor="Red" ></asp:Label>&nbsp;
            </td>
        </tr>
        <tr><td colspan="4" style="height:10px;"></td></tr>
    </table>
    <div class="Line"></div>
    <br />
    
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlResult" runat="server" Visible="false">
                <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
                <div style="height:12px;"></div>
                <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1290px" />
                <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                    EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1290px"  >
                    <Columns>
                        <asp:TemplateField HeaderText="ลำดับที่">
                            <ItemTemplate>
                            </ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อลูกค้า">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="นามสกุลลูกค้า">
                            <ItemTemplate>
                                <asp:Label ID="lblLastname" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ประเภทลูกค้า">
                            <ItemTemplate>
                                <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="บัตรประชาชน/<br/>นิติบุคคล">
                            <ItemTemplate>
                                <asp:Label ID="lblCitizenId" runat="server" Text='<%# Eval("CitizenId") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Owner<br />Lead ID">
                            <ItemTemplate>
                                <asp:Label ID="lblOwnerEmpID" runat="server" Text='<%# Eval("OwnerEmpCode") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delegate<br />Lead ID">
                            <ItemTemplate>
                                <asp:Label ID="lblDelegateLead" runat="server" Text='<%# Eval("DelegateEmpCode") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เบอร์โทร 1">
                            <ItemTemplate>
                                <asp:Label ID="lblTelNo1" runat="server" Text='<%# Eval("TelNo1") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เบอร์โทร 2">
                            <ItemTemplate>
                                <asp:Label ID="lblTelNo2" runat="server" Text='<%# Eval("TelNo2") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รายละเอียด Lead">
                            <ItemTemplate>
                                <asp:Label ID="lblDetail" runat="server" Text='<%# Eval("Detail") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="310px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="310px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะรายการ">
                            <ItemTemplate>
                                <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Eval("StatusDesc") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="หมายเหตุ">
                            <ItemTemplate>
                                <asp:Label ID="lblRemark" runat="server" Text='<%# Eval("Remark") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>    

    <br /><br />
    <div style="border:solid 0px; float:right; padding-right:110px;">
        <asp:UpdatePanel ID="upControlButton" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <asp:Button ID="btnDownload" runat="server" Text="Download" Width="100px" CssClass="Button" OnClick="btnDownload_Click" OnClientClick="if (confirm('ต้องการดาวโหลดไฟล์ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
                <asp:Button ID="btnDelete" runat="server" Text="ลบ" Width="100px" CssClass="Button" OnClick="btnDelete_Click" OnClientClick="if (confirm('ต้องการลบใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
                <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="100px" CssClass="Button" OnClick="btnSave_Click " OnClientClick="if (confirm('ต้องการบันทึกใช่หรือไม่')) { if (ConfirmSave()) { DisplayProcessing(); return true; } else { return false; }  } else { return false; }" />
                <asp:Button ID="btnBack" runat="server" Text="ยกเลิก" Width="100px" CssClass="Button" OnClick="btnBack_Click" OnClientClick="if (confirm('ต้องการกลับหน้าค้นหาใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <br /><br />

    <asp:Button runat="server" ID="btnPopupError" Width="0px" CssClass="Hidden"/>
    <asp:Panel ID="pnlPopupEdit" runat="server" style="display:none" CssClass="modalPopupError" >
        <asp:UpdatePanel ID="upPopupError" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <table cellpadding="2" cellspacing="0" border="0" style="padding-left:12px; padding-top:12px;">
                    <tr>
                        <td style="width:150px;"><b>Error Details</b></td>
                        <td align="right" style="width:400px;">
                            <asp:Label ID="lblTotalError" runat="server" Font-Bold="true" ></asp:Label>&nbsp;
                        </td>
                    </tr>
                </table>
                <div style="padding-left:10px; ">
                    <asp:Panel ID="pnlHoldGvError" runat="server" Height="390" ScrollBars="Auto" Width="570" >
                        <asp:Repeater ID="rptError" runat="server" ClientIDMode="AutoID">
                            <HeaderTemplate>
                                <table cellpadding="2" cellspacing="0" border="1" style="border-collapse:collapse;">
                                    <tr style="background-color:aliceblue;">
                                        <td align="center" style="width:100px; font-weight:bold">Row No</td>
                                        <td align="center" style="width:450px; font-weight:bold"">Description</td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <tr>
                                        <td align="center" style="vertical-align:top;"><%# Eval("Row") %></td>
                                        <td align="left" style="vertical-align:top;"><%# Eval("ErrorDetail") %></td>
                                    </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>

                        <%--<asp:GridView ID="gvError" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                            EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="550px"  >
                            <Columns>
                                <asp:TemplateField HeaderText="Row No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRow" runat="server" Text='<%# Eval("Row") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblErrorDetail" runat="server" Text='<%# Eval("ErrorDetail") %>' ></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="450px" HorizontalAlign="Left" VerticalAlign="Top" />
                                    <HeaderStyle Width="450px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="t_rowhead" />
                            <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                        </asp:GridView>--%>
                    </asp:Panel>
                </div><br />
                <div style="width:560px;">
                    <div style="float:right;">
                        <asp:Button ID="btnClosePopupError" runat="server" Text="ปิด" Width="100px" CssClass="Button" OnClick="btnClosePopupError_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <act:ModalPopupExtender ID="mpePopupError" runat="server" TargetControlID="btnPopupError" PopupControlID="pnlPopupEdit" BackgroundCssClass="modalBackground" DropShadow="True">
	</act:ModalPopupExtender>
</asp:Content>
