<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab018_3.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab018_3" %>
<%@ Register Src="../GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc1" %>

<style type="text/css">
    .style1 {
        width: 30px;
    }
    .style2 {
        width: 170px;
        text-align: left;
        font-weight: bold;
    }
    .style3 {
        width: 380px;
        text-align: left;
    }
</style>
<asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:TextBox ID="txtusername" runat="server" CssClass="Hidden"></asp:TextBox>
        <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1210px" />
        <asp:Panel ID="pnlResult" runat="server" ScrollBars="Auto" Width="1210px" BorderStyle="Solid" BorderWidth="1px">
            <asp:Label runat="server" ID="lblErrorMessage" ForeColor="Red"></asp:Label>
            <asp:GridView ID="gvOwner" runat="server" AutoGenerateColumns="False" DataKeyNames="SrNo"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>" Width="2000">
                <Columns>
                    <asp:TemplateField HeaderText="SR ID">
                        <ItemTemplate>
                            <asp:Label ID="lbTicketId" runat="server" Text='<%# Bind("SrNo") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CustomerFirstName" HeaderText="ชื่อ">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CustomerLastName" HeaderText="นามสกุล">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SrStatusName" HeaderText="สถานะของ SR">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ProductName" HeaderText="Product">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ProductGroupName" HeaderText="Product Group">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CampaignServiceName" HeaderText="แคมเปญ">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AreaName" HeaderText="Area">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SubAreaName" HeaderText="Sub Area">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TypeName" HeaderText="Type">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ChannelName" HeaderText="ช่องทาง">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerUserFullName" HeaderText="Owner SR">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerBranchName" HeaderText="Owner Branch">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelegateUserFullName" HeaderText="Delegate SR">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelegateBranchName" HeaderText="Delegate Branch">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreateDate" HeaderText="วันที่สร้าง SR">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreateDate" HeaderText="วันที่ได้รับมอบหมายล่าสุด">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CustomerSubscriptionTypeCode" HeaderText="ประเภทบุคคล">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CustomerCardNo" HeaderText="เลขที่บัตร">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
