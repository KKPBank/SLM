<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab005.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab005" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<div style="font-family:Tahoma; font-size:13px;">
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height:10px;">
                <asp:UpdateProgress ID="upgTab005" runat="server" AssociatedUpdatePanelID="upTabMain" >
                    <ProgressTemplate>
                        <div style="vertical-align:middle; text-align:center;">
                            <asp:Image ID="imgLoadingTab005" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <div style="float:right; height:18px;">
                <asp:LinkButton ID="lbReloadExistingLead" runat="server" Text="Refresh" Font-Underline="true" OnClick="lbReloadExistingLead_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;
            </div>
            <br />
            <asp:Panel ID="pnTab005" runat="server" Visible="false">
                <asp:TextBox ID="txtTelNo1" runat="server" Visible="false" ></asp:TextBox>
                <asp:TextBox ID="txtCitizenId" runat="server" Visible="false" ></asp:TextBox>
                <asp:TextBox ID="txtTicketId" runat="server"  Width="40px" Visible="false"></asp:TextBox>
                <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1166px" />
                <asp:Panel id="pnlExistingLead" runat="server" CssClass="PanelExistingLead" ScrollBars="Auto">
                    <asp:GridView ID="gvExistingLead" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" Width="1320px"
                    EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"  >
                    <Columns>
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Eval("TicketId") %>' OnClick="imbView_Click" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง"  />
                            </ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="TicketId" HeaderText="Ticket ID"  >
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Firstname" HeaderText="ชื่อ" >
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Lastname" HeaderText="นามสกุล" >
                            <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StatusDesc" HeaderText="สถานะของ Lead">
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CampaignName" HeaderText="แคมเปญ">
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง">
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="วันที่สร้าง Lead">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                            </ItemTemplate>
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ประเภทบุคคล">
                            <ItemTemplate>
                                <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                            <HeaderStyle Width="110px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView><br />
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>



