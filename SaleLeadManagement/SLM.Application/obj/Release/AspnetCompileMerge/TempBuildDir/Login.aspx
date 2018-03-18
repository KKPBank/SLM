<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Login.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SLM.Application.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .DomainCombobox
        {
            font-family:Cordia New;
            font-size:22px;
            height:27px;
            width:255px;
            border:1px solid #dcdadc;
            vertical-align:middle;
        }
        .UsernameTextBox
        {
            font-family:Cordia New;
            font-size:22px;
            color:Black;
            height:24px;
            width:225px;
            border:1px solid #dcdadc;
            vertical-align:middle;
            padding-left:3px;
            padding-right:25px;
            background-image:url(Images/icon_username.png);
            background-position:right;
            background-repeat:no-repeat;
        }
        .PasswordTextBox
        {
            font-family:Cordia New;
            font-size:16px;
            color:Black;
            height:24px;
            width:225px;
            border:1px solid #dcdadc;
            vertical-align:middle;
            padding-left:3px;
            padding-right:25px;
            background-image:url(Images/icon_password.png);
            background-position:right;
            background-repeat:no-repeat;
        }
        .LoginButton
        {
            color:White;
            height:30px;
            width:100px;
            border:1px solid #1793c9;
            background-color:#1793c9;
        }
        .WatermarkUsername
        {
            font-family:Cordia New;
            font-size:20px;
            color:Gray;
            height:24px;
            width:225px;
            border:1px solid #dcdadc;
            vertical-align:middle;
            padding-left:3px;
            padding-right:25px;
            background-image:url(Images/icon_username.png);
            background-position:right;
            background-repeat:no-repeat;
        }
        .WatermarkPassword
        {
            font-family:Cordia New;
            font-size:20px;
            color:Gray;
            height:24px;
            width:225px;
            border:1px solid #dcdadc;
            vertical-align:middle;
            padding-left:3px;
            padding-right:25px;
            background-image:url(Images/icon_password.png);
            background-position:right;
            background-repeat:no-repeat;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br /><br />
    <script language="javascript" type="text/javascript">
        function LoginProcessing() {
            var username = document.getElementById('ContentPlaceHolder1_Login1_UserName').value.trim();
            var password = document.getElementById('ContentPlaceHolder1_Login1_Password').value.trim();
            if (username != '' && username != 'Enter Username Here' && password != '' && password != 'Enter Password Here') {
                DisplayProcessing();
            }
        }
        function SetWaterMark(txtName, event) {

            if (txtName.value.length == 0 & event.type == 'blur') {
                txtName.style.fontSize = '20px';
                txtName.style.color = 'Gray';
                txtName.setAttribute('type', 'input');
            }
        }
        function PasswordMode(txtName) {
            txtName.style.fontSize = '16px';
            txtName.style.color = 'black';
            
            txtName.setAttribute('type', 'Password');
        }

    </script>
    <asp:UpdatePanel ID="upLogin" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
			<asp:HiddenField ID="hddReturnUrl" runat="server"/>
            <asp:HiddenField ID="hddCsmUsername" runat="server"/>
            <table  cellpadding="3" width="100%" border="0">
                <tr>
                    <td style="width:80px;"></td>
                    <td valign="top" style="width:770px; font-weight:bold; font-size:16px; color:#214f8d; ">
                        <asp:Label ID="lblNoticeTopic" runat="server" Width="650px"></asp:Label>
                    </td>
                    <td align="left" valign="top" style="width:400px;" ></td>
                    <td></td>
                </tr>
                <tr>
                    <td style="width:80px;"></td>
                    <td style="width:770px;">
                        <table border="0" cellpadding="2" cellspacing="0" >
                            <tr>
                                <td valign="top" style="height:412px;">
                                    <asp:Image ID="imgNotice" runat="server" Width="650px" />
                                </td>
                            </tr>
                            <tr>
                                <td style="height:20px;"></td>
                            </tr>
                            <tr id="trNoticeDownload" runat="server" >
                                <td valign="top">
                                    <asp:Image ID="imgBullet1" runat="server"  ImageUrl="~/Images/iGoDeposit.gif" Width="16px" ImageAlign="AbsMiddle" />
                                    <span style="color:#214f8d; font-weight:bold;">Download เอกสารเพิ่มเติม :</span>
                                    <%--<asp:LinkButton ID="lbNoticeDownload" runat="server" ForeColor="#214f8d" ></asp:LinkButton>&nbsp;&nbsp;--%>
                                    <asp:ImageButton ID="imbNoticeDownload" runat="server" ImageUrl="~/Images/icon_pdf.png" Width="24px" ImageAlign="Bottom" ToolTip="Download" />
                                </td>
                            </tr>
                            <tr id="trNoticeDownloadAll" runat="server">
                                <td>
                                    <asp:UpdatePanel ID="upNoticeShowAll" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Image ID="imgBullet2" runat="server"  ImageUrl="~/Images/iGoDeposit.gif" Width="16px" ImageAlign="AbsMiddle" />
                                            <asp:LinkButton ID="lbNoticeShowAll" runat="server" ForeColor="#214f8d" Text="แสดงทั้งหมด" Font-Bold="true" onclick="lbNoticeShowAll_Click" ></asp:LinkButton>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td style="height:20px;"></td>
                            </tr>
                        </table>
                    </td>
                    <td align="left" valign="top" style="width:400px;" >
                        <asp:Login ID="Login1" runat="server" onauthenticate="Login1_Authenticate">
                            <LayoutTemplate>
                                <table cellpadding="3" cellspacing="0" style="border-collapse:collapse;">
                                    <tr>
                                        <td style="font-size:18px; vertical-align:bottom; color:#1a83b4; ">
                                            <asp:Image ID="imgLogoLogin" runat="server"  ImageUrl="~/Images/logo_slm.bmp" Width="54px" ImageAlign="AbsMiddle"/>
                                            &nbsp;เข้าสู่ระบบ
                                        </td>
                                    </tr>
                                    <tr><td style="height:10px;"></td></tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="cmbDomain" runat="server" CssClass="DomainCombobox" ToolTip="Domain" >
                                                <asp:ListItem Text="kiatnakin.co.th"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="UserName" runat="server" CssClass="UsernameTextBox" ToolTip="Username" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ForeColor="Red"  
                                                ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                                ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                            <act:TextBoxWatermarkExtender ID="txtUsernameWatermark" runat="server" TargetControlID="UserName" WatermarkText="Enter Username Here" WatermarkCssClass="WatermarkUsername"></act:TextBoxWatermarkExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="Password" runat="server" CssClass="PasswordTextBox" ToolTip="Password" onkeypress="PasswordMode(this);" onfocus="PasswordMode(this);" onblur="SetWaterMark(this, event);" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ForeColor="Red" 
                                                ControlToValidate="Password" ErrorMessage="Password is required." 
                                                ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                            <act:TextBoxWatermarkExtender ID="txtPasswordWatermark" runat="server" TargetControlID="Password" WatermarkText="Enter Password Here" WatermarkCssClass="WatermarkPassword"></act:TextBoxWatermarkExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height:10px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td >
                                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="เข้าสู่ระบบ" ValidationGroup="Login1" CssClass="LoginButton" OnClientClick="LoginProcessing()" />
                                            <asp:CheckBox ID="cbRememberMe" runat="server" Text="Remember me next time." Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="color:Red;">
                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False" Visible="false"></asp:Literal>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color:Red;">&#9658; คุณสามารถเข้าระบบได้ด้วย Account Windows ของคุณ<br />&nbsp;&nbsp;&nbsp;&nbsp;และห้ามกรอกรหัสผ่านผิดเกิน 3 ครั้ง</span>
                                        </td>
                                    </tr>
                                    <tr><td style="height:5px;"></td></tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblBrowserWarning" runat="server" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </LayoutTemplate>
                        </asp:Login>
                        
                    </td>
                    <td></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upNoticeAll" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlNoticeAll" CssClass="modalPopupNoticeAll" ScrollBars="Auto" style="display:none">
                <asp:Button ID="btnPopupNoticeAll" runat="server" CssClass="Hidden" />
                <br />
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width:14px;">
                        </td>
                        <td style="width:200px;">
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/NoticeAll.png" />
                        </td>
                        <td align="right" style="width:599px;">
                            <asp:Label ID="lblTotalNotice" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width:15px;"></td>
                        <td style="height:460px; vertical-align:top">
                            <asp:Panel runat="server" ID="Panel1" CssClass="modalPopupNoticeAllInner" ScrollBars="Auto" BorderColor="LightGray" >
                                <asp:GridView ID="gvNotice" runat="server" AutoGenerateColumns="False"
                                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                                        EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" 
                                        onrowdatabound="gvNotice_RowDataBound" Width="777px" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Preview">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imbNoticeImage" runat="server" Width="120px" ToolTip="Preview" CommandArgument='<%# Eval("ImageVirtualPath") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Topic" HeaderText="เรื่อง" >
                                                <HeaderStyle Width="452px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="452px" HorizontalAlign="Left" VerticalAlign="Top"   />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="วันที่สร้าง">
                                                <ItemTemplate>
                                                    <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                                                </ItemTemplate>
                                                <HeaderStyle Width="145px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="145px" HorizontalAlign="Center" VerticalAlign="Top"  />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ไฟล์แนบ">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imbPopupNoticeDownload" runat="server" ImageUrl="~/Images/icon_pdf.png" Width="24px" ToolTip="Download" CommandArgument='<%# Eval("FileVirtualPath") %>' Visible='<%# Eval("FileVirtualPath") != null && Eval("FileVirtualPath").ToString() != "" ? true : false %>'  />
                                                </ItemTemplate>
                                                <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                                                <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"   />
                                            </asp:TemplateField>
                                        </Columns>
                                    <HeaderStyle CssClass="t_rowhead" />
                                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                                    </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr><td colspan="2" style="height:10px"></td></tr>
                    <tr>
                        <td style="width:15px;"></td>
                        <td>
                            <asp:Button ID="btnPopupNoticeAllClose" runat="server" Text="ปิด" Width="100px" 
                                onclick="btnPopupNoticeAllClose_Click" />
                        </td>
                    </tr>
                </table>
                <br />
            </asp:Panel>
             <act:ModalPopupExtender ID="mpeNoticeAll" runat="server" TargetControlID="btnPopupNoticeAll" PopupControlID="pnlNoticeAll" BackgroundCssClass="modalBackground" DropShadow="True">
	         </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
