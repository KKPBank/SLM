<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_021.aspx.cs" Inherits="SLM.Application.SLM_SCR_021" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:140px;
        }
        .ColInput
        {
            width:610px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var topicMaxLength = 2000;

//        function TopicChanged(txtTopic) {
//            var id = txtTopic.id;
//            if (id == '<%= txtTopic.ClientID %>') {
//                if (txtTopic.value.trim().length > topicMaxLength)
//                    document.getElementById('<%= lblTopicAlert.ClientID %>').innerHTML = 'กรุณาระบุหัวข้อไม่เกิน ' + topicMaxLength + ' ตัวอักษร';
//                else
//                    document.getElementById('<%= lblTopicAlert.ClientID %>').innerHTML = '';
//            }
//        }

        function ClearFileUpload(fileupload) {
            var oldInput = null;

            if (fileupload == 'fuImage') {
                document.getElementById('<%= lblFuImageAlert.ClientID %>').innerHTML = '';
                oldInput = document.getElementById('<%= fuImage.ClientID %>');
            }
            if (fileupload == 'fuAttachFile') {
                document.getElementById('<%= lblFuAttachFileAlert.ClientID %>').innerHTML = '';
                oldInput = document.getElementById('<%= fuAttachFile.ClientID %>');
            }
            if (fileupload == 'fuImageEdit') {
                document.getElementById('<%= lblFuImageEditAlert.ClientID %>').innerHTML = '';
                oldInput = document.getElementById('<%= fuImageEdit.ClientID %>');
            }
            if (fileupload == 'fuAttachFileEdit') {
                document.getElementById('<%= lblFuAttachFileEditAlert.ClientID %>').innerHTML = '';
                oldInput = document.getElementById('<%= fuAttachFileEdit.ClientID %>');
            }

            var newInput = document.createElement("input"); 
     
            newInput.type = "file"; 
            newInput.id = oldInput.id; 
            newInput.name = oldInput.name; 
            newInput.className = oldInput.className; 
            newInput.style.cssText = oldInput.style.cssText; 
            newInput.size = 34;
            // copy any other relevant attributes 
            oldInput.parentNode.replaceChild(newInput, oldInput); 
        }

        function ConfirmSave() {
            var i = 0;
            var txtTopic = document.getElementById('<%= txtTopic.ClientID %>');
            var txtTopic_alert = document.getElementById('<%= lblTopicAlert.ClientID %>');
            var fu_image = document.getElementById('<%= fuImage.ClientID %>');
            var fu_image_alert = document.getElementById('<%= lblFuImageAlert.ClientID %>');
            var fu_file = document.getElementById('<%= fuAttachFile.ClientID %>');
            var fu_file_alert = document.getElementById('<%= lblFuAttachFileAlert.ClientID %>');

            if (txtTopic.value.trim() == '') {
                txtTopic_alert.innerHTML = 'กรุณาระบุหัวข้อ';
                i += 1;
            }
            else {
                if (txtTopic.value.trim().length > topicMaxLength) {
                    txtTopic_alert.innerHTML = 'กรุณาระบุหัวข้อไม่เกิน ' + topicMaxLength + ' ตัวอักษร';
                    i += 1;
                }
                else
                    txtTopic_alert.innerHTML = '';
            }

            if (fu_image != null) {
                if (fu_image.files[0] == null) {
                    fu_image_alert.innerHTML = 'กรุณาระบุรูปภาพ';
                    i += 1;
                }
                else {
                    var validImageSize = <%= SLM.Application.Utilities.AppConstant.MaximumImageUploadSize %>;
                    var filePath = fu_image.value;
                    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                    if (ext != 'jpg' && ext != 'jpeg' && ext != 'png') {
                        fu_image_alert.innerHTML = 'กรุณาระบุรูปภาพให้ถูก format (jpg, jpeg, png)';
                        i += 1;
                    }
                    else if (fu_image.files[0].size > validImageSize) {
                        var mb = validImageSize / 1048576;
                        fu_image_alert.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                        i += 1;
                    }
                    else
                        fu_image_alert.innerHTML = '';
                }
            }

            if (fu_file != null) {
                if (fu_file.files[0] != null) {
                    var validFileSize = <%= SLM.Application.Utilities.AppConstant.MaximumFileUploadSize %>;
                    var filePath = fu_file.value;
                    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                    if (ext != 'pdf') {
                        fu_file_alert.innerHTML = 'กรุณาระบุไฟล์แนบให้ถูก format (pdf)';
                        i += 1;
                    }
                    else if (fu_file.files[0].size > validFileSize) {
                        var mb = validFileSize / 1048576;
                        fu_file_alert.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                        i += 1;
                    }
                    else
                        fu_file_alert.innerHTML = '';
                }
            }

            return i > 0 ? false : true;
        }

        function ConfirmSaveEdit() {
            var i = 0;
            var txtTopic = document.getElementById('<%= txtTopicEdit.ClientID %>');
            var txtTopic_alert = document.getElementById('<%= lblTopicEditAlert.ClientID %>');
            var divImageUpload = document.getElementById('<%= divImageUploadEdit.ClientID %>');
            var fu_image = document.getElementById('<%= fuImageEdit.ClientID %>');
            var fu_image_alert = document.getElementById('<%= lblFuImageEditAlert.ClientID %>');
            var divFileUpload = document.getElementById('<%= divFileUploadEdit.ClientID %>');
            var fu_file = document.getElementById('<%= fuAttachFileEdit.ClientID %>');
            var fu_file_alert = document.getElementById('<%= lblFuAttachFileEditAlert.ClientID %>');

            if (txtTopic.value.trim() == '') {
                txtTopic_alert.innerHTML = 'กรุณาระบุหัวข้อ';
                i += 1;
            }
            else {
                if (txtTopic.value.trim().length > topicMaxLength) {
                    txtTopic_alert.innerHTML = 'กรุณาระบุหัวข้อไม่เกิน ' + topicMaxLength + ' ตัวอักษร';
                    i += 1;
                }
                else
                    txtTopic_alert.innerHTML = '';
            }

            if (fu_image != null && divImageUpload.style.display == 'block') {
                if (fu_image.files[0] == null) {
                    fu_image_alert.innerHTML = 'กรุณาระบุรูปภาพ';
                    i += 1;
                }
                else {
                    var validImageSize = <%= SLM.Application.Utilities.AppConstant.MaximumImageUploadSize %>;
                    var filePath = fu_image.value;
                    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                    if (ext != 'jpg' && ext != 'jpeg' && ext != 'png') {
                        fu_image_alert.innerHTML = 'กรุณาระบุรูปภาพให้ถูก format (jpg, jpeg, png)';
                        i += 1;
                    }
                    else if (fu_image.files[0].size > validImageSize) {
                        var mb = validImageSize / 1048576;
                        fu_image_alert.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                        i += 1;
                    }
                    else
                        fu_image_alert.innerHTML = '';
                }
            }

            if (fu_file != null && divFileUpload.style.display == 'block') {
                if (fu_file.files[0] != null) {
                    var validFileSize = <%= SLM.Application.Utilities.AppConstant.MaximumFileUploadSize %>;
                    var filePath = fu_file.value;
                    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();

                    if (ext != 'pdf') {
                        fu_file_alert.innerHTML = 'กรุณาระบุไฟล์แนบให้ถูก format (pdf)';
                        i += 1;
                    }
                    else if (fu_file.files[0].size > validFileSize) {
                        var mb = validFileSize / 1048576;
                        fu_file_alert.innerHTML = 'กรุณาระบุไฟล์แนบขนาดไม่เกิน ' + mb + ' MB';
                        i += 1;
                    }
                    else
                        fu_file_alert.innerHTML = '';
                }
            }

            return i > 0 ? false : true;
        }

        function ToggleDivImage() {
            document.getElementById('<%= divImageInfoEdit.ClientID %>').style.display = 'none';
            document.getElementById('<%= divImageUploadEdit.ClientID %>').style.display = 'block';
        }

        function ToggleDivAttachFile() {
            document.getElementById('<%= divFileInfoEdit.ClientID %>').style.display = 'none';
            document.getElementById('<%= divFileUploadEdit.ClientID %>').style.display = 'block';
            document.getElementById('<%= txtEditAttachFileFlag.ClientID %>').value = 'edit';
        }

        function DisplayAddPopup() {
            var modal = $find('<%= mpePopup.ClientID %>');  // document.getElementById('<%= mpePopup.ClientID %>');
            modal.show();
        }

        function HideAddPopup() {
            document.getElementById('<%= txtTopic.ClientID %>').value = '';
            document.getElementById('<%= rbActive.ClientID %>').checked = true;
            document.getElementById('<%= rbInActive.ClientID %>').checked = false;
            document.getElementById('<%= lblTopicAlert.ClientID %>').innerHTML = '';
            document.getElementById('<%= lblFuImageAlert.ClientID %>').innerHTML = '';
            document.getElementById('<%= lblFuAttachFileAlert.ClientID %>').innerHTML = '';

            ClearFileUpload('fuImage');
            ClearFileUpload('fuAttachFile');

            var modal = $find('<%= mpePopup.ClientID %>');
            modal.hide();
        }

        function HideEditPopup() {
            document.getElementById('<%= txtTopicEdit.ClientID %>').value = '';
            document.getElementById('<%= lbAttachImageEdit.ClientID %>').value = '';
            document.getElementById('<%= lbAttachFileEdit.ClientID %>').value = '';
            document.getElementById('<%= rbActiveEdit.ClientID %>').checked = true;
            document.getElementById('<%= rbInactiveEdit.ClientID %>').checked = false;
            document.getElementById('<%= txtEditAttachFileFlag.ClientID %>').value = '';

            ClearFileUpload('fuImageEdit');
            ClearFileUpload('fuAttachFileEdit');

            var modal = $find('<%= mpePopupEdit.ClientID %>');
            modal.hide();
        }

//        function FileUploadChanged(fileupload) {
//            var id = fileupload.id;
//            if (id == '<%= fuImage.ClientID %>') {
//                document.getElementById('<%= lblFuImageAlert.ClientID %>').innerHTML = '';
//            }
//            else if (id == '<%= fuAttachFile.ClientID %>') {
//                document.getElementById('<%= lblFuAttachFileAlert.ClientID %>').innerHTML = '';
//                if (fileupload.value == '') {
//                    var oldInput = fileupload; 
//                    var newInput = document.createElement("input"); 
//     
//                    newInput.type = "file"; 
//                    newInput.id = oldInput.id; 
//                    newInput.name = oldInput.name; 
//                    newInput.className = oldInput.className; 
//                    newInput.style.cssText = oldInput.style.cssText; 
//                    // copy any other relevant attributes 
//     
//                    oldInput.parentNode.replaceChild(newInput, oldInput); 
//                }
//            }
//        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
<%--    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:2px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        เรื่อง
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTopicSearch" runat="server" CssClass="Textbox" Width="500px" MaxLength="2000"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        วันที่สร้างประกาศ
                    </td>
                    <td class="ColInput">
                        <uc2:TextDateMask ID="tdmCreateDateFrom" runat="server" />&nbsp;
                        ถึง
                        <uc2:TextDateMask ID="tdmCreateDateTo" runat="server" />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:5px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:CheckBox ID="cbActive" runat="server" Text="ใช้งาน"  />&nbsp;
                        <asp:CheckBox ID="cbInActive" runat="server" Text="ไม่ใช้งาน"  />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" OnClientClick="DisplayProcessing();"
                            Width="100px" onclick="btnSearch_Click" />
                    </td>
                </tr>
            </table>
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
    <br />
    <div class="Line"></div>
    <br />
<%--    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top"  />&nbsp;
            <asp:Button ID="btnAddNotice" runat="server" Text="เพิ่มข้อมูลประกาศ" Width="150px" 
                CssClass="Button" Height="23px" OnClientClick="DisplayAddPopup(); return false;" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1020px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1020px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                    onrowdatabound="gvResult_RowDataBound"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" OnClick="imbEdit_Click" OnClientClick="DisplayProcessing();" CommandArgument='<%# Eval("NoticeId") %>' ToolTip="แก้ไขข้อมูลประกาศ"  />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Topic" HeaderText="Topic"  >
                        <HeaderStyle Width="450px" HorizontalAlign="Center"/>
                        <ItemStyle Width="450px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะ"  >
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="วันที่สร้างข้อมูลประกาศ">
                        <ItemTemplate>
                            <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="รูปภาพ">
                        <ItemTemplate>
                            <asp:ImageButton id="imbPreviewImage" runat="server" Width="120px" ToolTip="Preview"  CommandArgument='<%# Eval("ImageVirtualPath") %>' Visible='<%# Eval("ImageVirtualPath") != null && Eval("ImageVirtualPath").ToString() != "" ? true : false %>' />
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ไฟล์แนบ">
                        <ItemTemplate>
                            <asp:ImageButton id="imbDownloadFile" runat="server" Width="24px" ToolTip="Download" ImageUrl="~/Images/icon_pdf.png" CommandArgument='<%# Eval("FileVirtualPath") %>' Visible='<%# Eval("FileVirtualPath") != null && Eval("FileVirtualPath").ToString() != "" ? true : false %>' />
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
<%--
    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupNotice">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/AddNotice.png" />
		        <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            หัวข้อ<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtTopic" runat="server" CssClass="Textbox" Width="500px"  ></asp:TextBox>
                            <br />
                            <asp:Label ID="lblTopicAlert" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            แนบรูป<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:FileUpload ID="fuImage" runat="server" Width="300px" size="34" />
                            <a href="" onclick="ClearFileUpload('fuImage'); return false;">Clear</a>
                            &nbsp;
                            <asp:Label ID="lblImage" runat="server" ForeColor="Red" Text="Size ไม่เกิน 5 MB (jpg, jpeg, png)"></asp:Label>
                            <br />
                            <asp:Label ID="lblFuImageAlert" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            แนบไฟล์
                        </td>
                        <td class="ColInput">
                            <asp:FileUpload ID="fuAttachFile" runat="server" Width="300px" size="34" />
                            <a href="" onclick="ClearFileUpload('fuAttachFile'); return false;">Clear</a>
                            &nbsp;
                            <asp:Label ID="lblAttachFile" runat="server" ForeColor="Red" Text="Size ไม่เกิน 5 MB (pdf)"></asp:Label>
                            <br />
                            <asp:Label ID="lblFuAttachFileAlert" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:4px;"></td></tr>
                    <tr>
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            สถานะ<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:RadioButton ID="rbActive" runat="server" Text="ใช้งาน" GroupName="Status" Checked="true" />&nbsp;
                            <asp:RadioButton ID="rbInActive" runat="server" Text="ไม่ใช้งาน" GroupName="Status" />
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:15px;"></td></tr>
                    <tr>
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                        </td>
                        <td class="ColInput">
                            <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" OnClientClick="return ConfirmSave();"
                                Width="100px" onclick="btnSave_Click" />&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                Width="100px" OnClientClick="HideAddPopup(); return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>

    <%--<asp:UpdatePanel ID="upPopupEdit" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <asp:Button runat="server" ID="btnPopupEdit" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnlPopupEdit" style="display:none" CssClass="modalPopupNotice">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/Images/AddNotice.png" />
		        <table cellpadding="3" cellspacing="0" border="0">
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            หัวข้อ<span style="color:Red;">*</span>
                            <asp:TextBox ID="txtNoticeIdEdit" runat="server" Width="50px" CssClass="Hidden"></asp:TextBox>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtTopicEdit" runat="server" CssClass="Textbox" Width="500px"  ></asp:TextBox>
                            <br />
                            <asp:Label ID="lblTopicEditAlert" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            ไฟล์รูป<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <div id="divImageInfoEdit" runat="server" >
                                <asp:LinkButton ID="lbAttachImageEdit" runat="server" ></asp:LinkButton>
                                <asp:ImageButton ID="imbDeleteAttachImage" runat="server" 
                                    ImageAlign="AbsMiddle" ImageUrl="~/Images/bDelete.gif" ToolTip="ลบ" OnClientClick="ToggleDivImage(); return false;" />
                            </div>
                            <div id="divImageUploadEdit" runat="server">
                                <asp:FileUpload ID="fuImageEdit" runat="server" Width="300px" size="34" />
                                <a href="" onclick="ClearFileUpload('fuImageEdit'); return false;">Clear</a>
                                &nbsp;
                                <asp:Label ID="lblfuImageEdit" runat="server" ForeColor="Red" Text="Size ไม่เกิน 5 MB (jpg, jpeg, png)"></asp:Label>
                                <br />
                                <asp:Label ID="lblFuImageEditAlert" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            ไฟล์แนบ
                        </td>
                        <td class="ColInput">
                            <div id="divFileInfoEdit" runat="server">
                                <asp:LinkButton ID="lbAttachFileEdit" runat="server"></asp:LinkButton>
                                <asp:ImageButton ID="imbDeleteAttachFile" runat="server" ImageAlign="AbsMiddle" 
                                    ImageUrl="~/Images/bDelete.gif" ToolTip="ลบ" OnClientClick="ToggleDivAttachFile(); return false;" />
                            </div>
                            <div id="divFileUploadEdit" runat="server">
                                <asp:FileUpload ID="fuAttachFileEdit" runat="server" Width="300px" size="34" />
                                <a href="" onclick="ClearFileUpload('fuAttachFileEdit'); return false;">Clear</a>
                                &nbsp;
                                <asp:Label ID="lblfuAttachFileEdit" runat="server" ForeColor="Red" Text="Size ไม่เกิน 5 MB (pdf)"></asp:Label>
                                <br />
                                <asp:Label ID="lblFuAttachFileEditAlert" runat="server" ForeColor="Red"></asp:Label>
                                <asp:TextBox ID="txtEditAttachFileFlag" runat="server" Width="50px" CssClass="Hidden" ></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:2px;"></td></tr>
                    <tr>
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                            สถานะ<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:RadioButton ID="rbActiveEdit" runat="server" Text="ใช้งาน" GroupName="StatusEdit" Checked="true" />&nbsp;
                            <asp:RadioButton ID="rbInactiveEdit" runat="server" Text="ไม่ใช้งาน" GroupName="StatusEdit" />
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:15px;"></td></tr>
                    <tr>
                        <td style="width:30px;"></td>
                        <td class="ColInfo">
                        </td>
                        <td class="ColInput">
                            <asp:Button ID="btnSaveEdit" runat="server" Text="บันทึก" CssClass="Button" OnClientClick="return ConfirmSaveEdit();"
                                Width="100px" onclick="btnSaveEdit_Click" />&nbsp;
                            <asp:Button ID="btnCancelEdit" runat="server" Text="ยกเลิก" CssClass="Button" 
                                Width="100px" OnClientClick="HideEditPopup(); return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopupEdit" runat="server" TargetControlID="btnPopupEdit" PopupControlID="pnlPopupEdit" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
       <%-- </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
