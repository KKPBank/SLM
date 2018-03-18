<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabNote.ascx.cs" Inherits="SLM.Application.Shared.Obt.TabNote" %>
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
    
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="left" style="width:1146px;">
                        <%--<asp:CheckBox ID="cbNoteFlag" runat="server" Text="แสดงเตือน Note" 
                            onchange="DisplayProcessing()" AutoPostBack="true" 
                            oncheckedchanged="cbNoteFlag_CheckedChanged" />&nbsp;&nbsp;--%>
                        <asp:Button ID="btnAddNote" runat="server" CssClass="Button" 
                            Text="เพิ่ม Note" Width="120px" onclick="btnAddNote_Click"  />
                    </td>
                </tr>
            </table>
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1149px" />
            <asp:GridView ID="gvNoteHistory" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" >
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
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="EmailSubject" HeaderText="Subject">
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:BoundField>
                    <asp:BoundField DataField="NoteDetail" HeaderText="บันทึก Note">
                        <HeaderStyle Width="424" HorizontalAlign="Center"/>
                        <ItemStyle Width="424" HorizontalAlign="Left" VerticalAlign="Top"   />
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
                                Ticket ID
                            </td>
                            <td class="Tab009ColInput">
                                <asp:TextBox ID="txtTicketID" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" OnKeyPress="return ChkInt(event)" ></asp:TextBox>
                            </td>
                            <td>
                                เลขที่สัญญา
                            </td>
                            <td>
                                <asp:TextBox ID="txtContractNo" runat="server" CssClass="TextboxView" ReadOnly="true" Width="200px" ></asp:TextBox>
                                <asp:TextBox ID="txtPreleadId" runat="server" Width="40px" Visible="false"></asp:TextBox>
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
