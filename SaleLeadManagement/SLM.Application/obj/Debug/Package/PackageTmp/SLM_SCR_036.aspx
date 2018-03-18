<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_036.aspx.cs" Inherits="SLM.Application.SLM_SCR_036" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:150px;
        }
        .ColInput
        {
            width:250px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        .modalPopupAddScript
        {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width:710px;
            height:520px;
        }
        .style4
        {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="4" style="height:2px;"></td></tr>
                
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbProductSearch" runat="server" Text="ผลิตภัณฑ์/บริการ" 
                            GroupName="Type" Checked="true" AutoPostBack="true" 
                            oncheckedchanged="rbProductSearch_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbProductSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        <asp:RadioButton ID="rbCampaignSearch" runat="server" Text="แคมเปญ" 
                            GroupName="Type" AutoPostBack="true" 
                            oncheckedchanged="rbCampaignSearch_CheckedChanged" />
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCampaignSearch" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ประเภทข้อมูล
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbDataTypeSearch" runat="server" CssClass="Dropdownlist" Width="150px">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                            <asp:ListItem Value="001" Text="Script การขาย"></asp:ListItem>
                            <asp:ListItem Value="002" Text="Q&A"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Subject
                    </td>
                    <td>
                         <asp:TextBox ID="txtSubjectSearch" runat="server" CssClass="Textbox" Width="500px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ข้อความ
                    </td>
                    <td>
                         <asp:TextBox ID="txtDetailSearch" runat="server" CssClass="Textbox" Width="500px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ
                    </td>
                    <td>
                        <asp:CheckBox ID="cbActive" runat="server" Text="ใช้งาน"  />&nbsp;
                        <asp:CheckBox ID="cbInActive" runat="server" Text="ไม่ใช้งาน"  />
                    </td>
                </tr>
            </table>   
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td >
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" onclick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" 
                            Text="ล้างข้อมูล" onclick="btnClear_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" />&nbsp;
            <asp:Button ID="btnAddScript" runat="server" Text="เพิ่มข้อมูล Script หรือ Q&A" Width="250px" 
                CssClass="Button" Height="23px" onclick="btnAddScript_Click"  />
            <br /><br />
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1530px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" 
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  Width="1530px" >
                <Columns>
                <asp:TemplateField HeaderText="Action" >
                    <ItemTemplate>
                        <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("ConfigScriptId") %>'  ToolTip="แก้ไขข้อมูล" OnClick="imbEdit_Click" OnClientClick="DisplayProcessing();"  />
                        <asp:ImageButton ID="imbDelete" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("ConfigScriptId") %>' ToolTip="ลบข้อมูล" OnClick="imbDelete_Click" 
                                OnClientClick="if (confirm('ต้องการลบข้อมูล ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                    </ItemTemplate>
                    <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"  />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                    <ItemTemplate>
                        <asp:Label ID="lblProduct" runat="server" Text='<%# Eval("ProductName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="170px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="แคมเปญ">
                    <ItemTemplate>
                        <asp:Label ID="lblCampaign" runat="server" Text='<%# Eval("CampaignName") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="170px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="170px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ประเภทข้อมูล">
                    <ItemTemplate>
                        <asp:Label ID="lblDataType" runat="server" Text='<%# Eval("DataType") != null ? (Eval("DataType").ToString() == "001" ? "Script การขาย" : "Q&A") : "" %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Subject">
                    <ItemTemplate>
                        <asp:Label ID="lblSubject" runat="server" Text='<%# Eval("Subject") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                    <HeaderStyle Width="140px" HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ข้อความ">
                    <ItemTemplate>
                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Detail") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="550px" HorizontalAlign="Center" />
                    <ItemStyle Width="550px" HorizontalAlign="Left" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="seq" >
                    <ItemTemplate>
                        <asp:Label ID="lblseq" runat="server" Text='<%# Eval("seq") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="สถานะ">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
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
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupAddScript">
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr>
                            <td colspan="3" style="height:5px;"></td>
                        </tr>
                        <tr>
                            <td colspan="3" style="padding-left:28px; height:30px; vertical-align:top;">
                                <asp:Image ID="imgConfigScriptQA" runat="server" ImageUrl="~/Images/ConfigScriptQA.png" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                <asp:RadioButton ID="rbProductPopup" runat="server" Text="ผลิตภัณฑ์/บริการ" 
                                    GroupName="PopupType" Checked="true" AutoPostBack="true" 
                                    oncheckedchanged="rbProductPopup_CheckedChanged" /><asp:Label ID="lblProductStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbProductPopup" runat="server" CssClass="Dropdownlist" Width="250px" ></asp:DropDownList>
                                <asp:TextBox ID="txtConfigScriptId" runat="server" Width="50px" Visible="false" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                <asp:RadioButton ID="rbCampaignPopup" runat="server" Text="แคมเปญ" 
                                    GroupName="PopupType" AutoPostBack="true" 
                                    oncheckedchanged="rbCampaignPopup_CheckedChanged" /><asp:Label ID="lblCampaignStar" runat="server" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbCampaignPopup" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false" ></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertProductCampaignPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr><td colspan="3" style="height:5px;"></td></tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ประเภทข้อมูล<span class="style4">*</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="cmbDataTypePopup" runat="server" Width="200px" CssClass="Dropdownlist">
                                    <asp:ListItem Value="001" Text="Script การขาย"></asp:ListItem>
                                    <asp:ListItem Value="002" Text="Q&A"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                             <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Subject<span class="style4">*</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSubjectPopup" runat="server" CssClass="Textbox" Width="500px"></asp:TextBox>
                                <br />
                                <asp:Label ID="alertSubjectPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                         <tr>
                             <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ข้อความ<span class="style4">*</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtDetailPopup" runat="server" CssClass="Textbox" TextMode="MultiLine" Width="500px" Height="200px"></asp:TextBox>
                                <br />
                                <asp:Label ID="alertDetailPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                         <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ<span class="style4">*</span>
                            </td>
                            <td>
                                <asp:RadioButton ID="rdActivePopup" runat="server" GroupName ="Status" Text="ใช้งาน" Checked="true" />
                                <asp:RadioButton ID="rdNoActivePopup" runat="server" GroupName ="Status" Text="ไม่ใช้งาน" />
                            </td>
                        </tr>
                        <tr>
                         <tr >
                            <td style="width:20px;"></td>
                            <td class="ColInfo" valign="top">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Seq
                            </td>
                            <td>
                                <asp:TextBox ID="txtSeqPopup" runat="server" CssClass="TextboxR" Width="50px" ></asp:TextBox>
                                <asp:Label ID="alertSeqPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                       
                            <td colspan="3" style="height:10px"></td>
                        </tr>
                        <tr >
                            <td style="width:20px;"></td>
                             <td class="ColInfo"></td>
                            <td >
                                <asp:Button ID="btnSave" runat="server" Text="บันทึก" Width="100px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing();" />&nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" Width="100px" onclick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>
	            </asp:Panel>
	            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
