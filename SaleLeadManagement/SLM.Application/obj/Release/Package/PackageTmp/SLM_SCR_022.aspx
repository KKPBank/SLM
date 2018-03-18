<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_022.aspx.cs" Inherits="SLM.Application.SLM_SCR_022" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:2px;"></td></tr>
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
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ช่องทาง
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbChannelSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbStatusSearch" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:3px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing()" />
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
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top"  />&nbsp;
            <asp:Button ID="btnAddSla" runat="server" Text="เพิ่มข้อมูล SLA" Width="150px" 
                CssClass="Button" Height="23px" onclick="btnAddSla_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1110px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1110px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                onrowdatabound="gvResult_RowDataBound"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูล SLA" OnClientClick="DisplayProcessing();"  />&nbsp;
                            <asp:ImageButton ID="imbDelete" runat="server" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("SlaId") %>' ToolTip="ลบข้อมูล SLA" OnClick="imbDelete_Click" 
                                OnClientClick="if (confirm('ต้องการลบข้อมูล SLA ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }"  />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ผลิตภัณฑ์/บริการ">
                        <ItemTemplate>
                            <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="แคมเปญ">
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะ">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="SubStatusDesc" HeaderText="สถานะย่อย">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="SLA(Minute)">
                        <ItemTemplate>
                            <asp:Label ID="lblSlaMin" runat="server" Text='<%# Eval("SlaMin") != null ? Convert.ToInt32(Eval("SlaMin")).ToString("#,##0") : "" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SLA(Times)">
                        <ItemTemplate>
                            <asp:Label ID="lblSlaTime" runat="server" Text='<%# Eval("SlaTime") != null ? Convert.ToInt32(Eval("SlaTime")).ToString("#,##0") : "" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SLA(Days)">
                        <ItemTemplate>
                            <asp:Label ID="lblSlaDay" runat="server" Text='<%# Eval("SlaDay") != null ? Convert.ToInt32(Eval("SlaDay")).ToString("#,##0") : "" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblSlaId" runat="server" Text='<%# Eval("SlaId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblChannelId" runat="server" Text='<%# Eval("ChannelId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblStatusCode" runat="server" Text='<%# Eval("StatusCode") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:Label ID="lblSubStatusCode" runat="server" Text='<%# Eval("SubStatusCode") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="Hidden" />
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <FooterStyle  CssClass="Hidden" />
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
	            <asp:Panel runat="server" ID="pnPopup" style="display:none" CssClass="modalPopupConfigSla">
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/SLA.png" />
                    <asp:TextBox ID="txtSlaId" runat="server" Visible="false"></asp:TextBox>
		            <table cellpadding="2" cellspacing="0" border="0">
                        <tr><td colspan="3" style="height:1px;"></td></tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                <asp:RadioButton ID="rbProductPopup" runat="server" Text="ผลิตภัณฑ์/บริการ" 
                                    GroupName="PopupType" Checked="true" AutoPostBack="true" 
                                    oncheckedchanged="rbProductPopup_CheckedChanged" /><asp:Label ID="lblProductStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbProductPopup" runat="server" CssClass="Dropdownlist" Width="250px" OnSelectedIndexChanged="cmbProductPopup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                <asp:RadioButton ID="rbCampaignPopup" runat="server" Text="แคมเปญ" 
                                    GroupName="PopupType" AutoPostBack="true" 
                                    oncheckedchanged="rbCampaignPopup_CheckedChanged" /><asp:Label ID="lblCampaignStar" runat="server" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbCampaignPopup" runat="server" CssClass="Dropdownlist" Width="250px" Enabled="false" OnSelectedIndexChanged="cmbCampaignPopup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertProductCampaignPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr><td colspan="3" style="height:10px;"></td></tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ช่องทาง<asp:Label ID="lblChannelIdStar" runat="server" ForeColor="Red" Text="*"></asp:Label>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbChannelPopup" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertChannelPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะ<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbStatusPopup" runat="server" CssClass="Dropdownlist" AutoPostBack="true"
                                    Width="250px" onselectedindexchanged="cmbStatusPopup_SelectedIndexChanged"></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertStatusPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;สถานะย่อย
                            </td>
                            <td class="ColInput">
                                <asp:DropDownList ID="cmbSubStatusPopup" runat="server" CssClass="Dropdownlist" Width="250px"></asp:DropDownList>
                                <br />
                                <asp:Label ID="alertSubStatusPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SLA (Minute)<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtSlaMinPopup" runat="server" CssClass="Textbox" Width="80px" MaxLength="9"></asp:TextBox>
                                <asp:Label ID="alertSlaMinPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SLA (Times)<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtSlaTimePopup" runat="server" CssClass="Textbox" Width="80px" MaxLength="9"></asp:TextBox>
                                <asp:Label ID="alertSlaTimePopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr style="vertical-align:top;">
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SLA (Days)<span style="color:Red;">*</span>
                            </td>
                            <td class="ColInput">
                                <asp:TextBox ID="txtSlaDayPopup" runat="server" CssClass="Textbox" Width="80px" MaxLength="9"></asp:TextBox>
                                <asp:Label ID="alertSlaDayPopup" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr><td colspan="3" style="height:8px;"></td></tr>
                        <tr>
                            <td style="width:20px;"></td>
                            <td class="ColInfo">
                            </td>
                            <td>
                                <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" Width="100px" OnClick="btnSave_Click" OnClientClick="DisplayProcessing();" />
                                <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                    Width="100px" onclick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
