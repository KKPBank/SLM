<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab007.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab007" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<div style="font-family:Tahoma; font-size:13px;">
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height:10px;">
                <asp:UpdateProgress ID="upgTab007" runat="server" AssociatedUpdatePanelID="upTabMain" >
                    <ProgressTemplate>
                        <div style="vertical-align:middle; text-align:center;">
                            <asp:Image ID="imgLoadingTab007" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <div style="float:right; height:18px;">
                <asp:LinkButton ID="lbReloadLogging" runat="server" Text="Refresh" Font-Underline="true" OnClick="lbReloadLogging_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;
            </div>
            <br />
            <asp:Panel ID="pnTab007" runat="server" Visible="false">
                <asp:TextBox ID="txtTicketId" runat="server" Visible="false" ></asp:TextBox>
                <asp:TextBox ID="txtPreleadId" runat="server" Visible="false" ></asp:TextBox>
                <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1170px" />
                <asp:GridView ID="gvOwnerLogging" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" 
                    EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"  >
                    <Columns>
                        <asp:TemplateField HeaderText="วันที่บันทึกข้อมูล">
                            <ItemTemplate>
                                <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="TicketId" HeaderText="Ticket ID"  >
                            <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CitizenId" HeaderText="รหัสบัตรประชาชน"  >
                            <HeaderStyle Width="120px" HorizontalAlign="Center" CssClass="Hidden" />
                            <ItemStyle Width="120px" HorizontalAlign="Center"  CssClass="Hidden" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Firstname" HeaderText="ชื่อ" >
                            <HeaderStyle Width="110px" HorizontalAlign="Center" CssClass="Hidden" />
                            <ItemStyle Width="110px" HorizontalAlign="Left"  CssClass="Hidden" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Lastname" HeaderText="นามสกุล" >
                            <HeaderStyle Width="110px" HorizontalAlign="Center" CssClass="Hidden" />
                            <ItemStyle Width="110px" HorizontalAlign="Left"  CssClass="Hidden" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SystemAction" HeaderText="System Action" >
                            <HeaderStyle Width="80px" HorizontalAlign="Center"/>
                            <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                         <asp:BoundField DataField="Action" HeaderText="Action" >
                            <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                            <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CreateBy" HeaderText="Action By" >
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OldStatusDesc" HeaderText="สถานะเดิม<br/>(หลัก)" HtmlEncode="false">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OldSubStatusDesc" HeaderText="สถานะเดิม<br/>(ย่อย)" HtmlEncode="false">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NewStatusDesc" HeaderText="สถานะใหม่<br/>(หลัก)" HtmlEncode="false">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NewSubStatusDesc" HeaderText="สถานะใหม่<br/>(ย่อย)" HtmlEncode="false">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OldOwnerName" HeaderText="Old Owner">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NewOwnerName" HeaderText="New Owner">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OldDelegateName" HeaderText="Old Delegate">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NewDelegateName" HeaderText="New Delegate">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView>
            </asp:Panel>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</div>



