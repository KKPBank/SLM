<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab009.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab009" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<style type="text/css">
    .Tab009ColIndent
    {
        width:35px;
    }
    .Tab009ColInfo1
    {
        font-weight:bold;
        width:170px;
    }
    .Tab009ColInfo2
    {
        font-weight:bold;
        width:200px;
    }
    .Tab009ColInput
    {
        width:250px;
    }
</style>

<div style="font-family:Tahoma; font-size:13px;">
    <script language="javascript" type="text/javascript">
        function ConfirmSave() {
            var cb = document.getElementById('<%= cbSendEmail.ClientID %>');
            var detail = document.getElementById('<%= txtNoteDetail.ClientID %>').value;
            var errorMsg = document.getElementById('<%= lblError.ClientID %>');

            if (cb.disabled == false) {
                var subject = document.getElementById('<%= txtEmailSubject.ClientID %>');

                if (cb.checked && subject != null && subject.value.trim() == '') {
                    errorMsg.innerHTML = 'กรุณากรอกข้อมูล Email Subject ก่อนบันทึก';
                    return false;
                }

                var emails = document.getElementById('<%= txtEmailTo.ClientID %>');
                if (cb.checked && emails != null && emails.value.trim() != '')
                {
                    var arr_email = emails.value.trim().split(',');
                    for (i = 0; i < arr_email.length; i++) {
                        if (arr_email[i] != null && arr_email[i].trim() != '') {
                            var filter = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
                            if (filter.test(arr_email[i].trim()) == false) {
                                errorMsg.innerHTML = 'กรุณาระบุ E-mail ให้ถูกต้อง';
                                return false;
                            }
                        }
                    }
                }
            }

            if (detail.trim() == '') {
                errorMsg.innerHTML = 'กรุณากรอกข้อมูลบันทึก Note ก่อนบันทึก';
                return false;
            }
            else {
                if (detail.trim().length > '<%= SLM.Application.Utilities.AppConstant.TextMaxLength %>')
                {
                    errorMsg.innerHTML = 'ไม่สามารถบันทึกรายละเอียด Note เกิน ' + '<%= SLM.Application.Utilities.AppConstant.TextMaxLength %>' + ' ตัวอักษรได้';
                    return false;
                }
            }

            errorMsg.innerHTML = '';
            return confirm('ต้องการบันทึกใช่หรือไม่');
        }
    </script>
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height:10px;">
                <asp:UpdateProgress ID="upgTab009" runat="server" AssociatedUpdatePanelID="upTabMain" >
                    <ProgressTemplate>
                        <div style="vertical-align:middle; text-align:center;">
                            <asp:Image ID="imgLoadingTab009" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <div style="float:right; height:18px;">
                <asp:LinkButton ID="lbReloadNote" runat="server" Text="Refresh" Font-Underline="true" OnClick="lbReloadNote_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;
            </div>
            <asp:Panel ID="pnTab009" runat="server" Visible="false">
                <table cellpadding="0" cellspacing="0" border="0" >
                    <tr>
                        <td align="left" style="width:1146px;">
                            <asp:CheckBox ID="cbNoteFlag" runat="server" Text="แสดงเตือน Note" 
                                onchange="DisplayProcessing()" AutoPostBack="true" 
                                oncheckedchanged="cbNoteFlag_CheckedChanged" />&nbsp;&nbsp;
                            <asp:Button ID="btnAddNote" runat="server" CssClass="Button" 
                                Text="เพิ่ม Note" Width="120px" onclick="btnAddNote_Click"  />
                        </td>
                    </tr>
                </table>
                <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1168px" />
                <asp:GridView ID="gvNoteHistory" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                    GridLines="Horizontal" BorderWidth="0px" Width="1168px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" >
                    <Columns>
                        <asp:TemplateField HeaderText="วันที่สร้าง Note">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                        </asp:TemplateField>
                        <asp:BoundField DataField="ContractNo" HeaderText="เลขที่สัญญา"  >
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                        </asp:BoundField>
                        <asp:BoundField DataField="TicketId" HeaderText="Ticket ID"  >
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreateBy" HeaderText="ผู้สร้าง Note">
                            <HeaderStyle Width="160px" HorizontalAlign="Center"/>
                            <ItemStyle Width="160px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:BoundField>
                        <asp:BoundField DataField="EmailSubject" HeaderText="Subject">
                            <HeaderStyle Width="218px" HorizontalAlign="Center"/>
                            <ItemStyle Width="218px" HorizontalAlign="Left" VerticalAlign="Top"  />
                        </asp:BoundField>
                        <asp:BoundField DataField="NoteDetail" HeaderText="บันทึก Note">
                            <HeaderStyle Width="430" HorizontalAlign="Center"/>
                            <ItemStyle Width="430" HorizontalAlign="Left" VerticalAlign="Top"   />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="ส่งอีเมล">
                            <ItemTemplate>
                                <%# Eval("SendEmailFlag") != null ? (Convert.ToBoolean(Eval("SendEmailFlag")) == true ? "ส่ง" : "ไม่ส่ง") : "ไม่ส่ง" %>
                            </ItemTemplate>
                            <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                            <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"   />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView>
             </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupTab009">
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/SaveNote.gif" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr><td colspan="5" style="height:1px;"></td></tr>
                        <tr>
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                Contract No
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtContractNo" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                <asp:TextBox ID="txtPreleadId" runat="server" Width="40px" Visible="false" ></asp:TextBox>
                            </td>
                            <td class="Tab009ColInfo2">
                                Ticket ID
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtTicketID" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" OnKeyPress="return ChkInt(event)" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                ชื่อ
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtFirstname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                            </td>
                            <td class="Tab009ColInfo2">
                                นามสกุล
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtLastname" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                Owner Lead
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtOwnerLead" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                            </td>
                            <td class="Tab009ColInfo2">
                                แคมเปญ
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtCampaign" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                หมายเลขโทรศัพท์
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtTelNo1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                <%--&nbsp;-&nbsp;--%>
                                <asp:TextBox ID="txtExt1" runat="server" CssClass="TextboxView" ReadOnly="true" Width="46px" Visible="false" ></asp:TextBox>
                            </td>
                            <td class="Tab009ColInfo2">
                                ส่งอีเมล
                            </td>
                            <td class="Tab009ColInput">
                                <asp:CheckBox ID="cbSendEmail" runat="server" AutoPostBack="true" oncheckedchanged="cbSendEmail_CheckedChanged" />
                            </td>
                        </tr>
                        <tr id="trEmailTo" runat="server">
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                To
                            </td>
                            <td colspan="3" valign="top">
                                <asp:TextBox ID="txtEmailTo" runat="server" CssClass="Textbox" Width="658px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trEmailSample" runat="server">
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                            </td>
                            <td colspan="3" valign="top">
                                <asp:Label ID="lblEmailSample" runat="server" ForeColor="DarkCyan" Text="(example1@kiatnakin.co.th, example2@kiatnakin.co.th)"></asp:Label>&nbsp;
                                <asp:Label ID="alertEmailTo" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr id="trEmail" runat="server" style="vertical-align:top;">
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1">
                                Subject<span style="color:Red;">*</span>
                            </td>
                            <td colspan="3" valign="top">
                                <asp:TextBox ID="txtEmailSubject" runat="server" CssClass="Textbox" Width="658px" Enabled="false"></asp:TextBox>
                                <br />
                                <asp:Label ID="alertEmailSubject" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1" valign="top">
                                บันทึก Note<span style="color:Red;">*</span>
                            </td>
                            <td colspan="3" valign="top">
                                <asp:TextBox ID="txtNoteDetail" runat="server" CssClass="Textbox" TextMode="MultiLine" Rows="5" Width="658px" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="height:35px;">
                            <td class="Tab009ColIndent"></td>
                            <td class="Tab009ColInfo1" valign="top">
                            </td>
                            <td colspan="2" style="vertical-align:top;">
                                <%--<asp:RequiredFieldValidator ID="NoteDetailRequired" runat="server" ForeColor="Red" Font-Size="12px"  
                                    ControlToValidate="txtNoteDetail" ErrorMessage="กรุณากรอกข้อมูลบันทึก Note ก่อนทำการบันทึก" 
                                    ToolTip="กรุณากรอกข้อมูลบันทึก Note ก่อนทำการบันทึก" ValidationGroup="Save"></asp:RequiredFieldValidator>--%>
                                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                <div id="lblMsg2" style="color:Red;"></div>
                            </td>
                            <td class="Tab009ColInput">
                                <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="98px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing()" />&nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" Width="98px" OnClick="btnCancel_Click" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่?')" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>
	            </asp:Panel>
	            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
