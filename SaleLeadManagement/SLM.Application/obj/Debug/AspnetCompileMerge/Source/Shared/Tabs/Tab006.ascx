<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tab006.ascx.cs" Inherits="SLM.Application.Shared.Tabs.Tab006" %>
<%@ Register src="../GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<div style="font-family:Tahoma; font-size:13px;">
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height:10px;">
                <asp:UpdateProgress ID="upgTab006" runat="server" AssociatedUpdatePanelID="upTabMain" >
                    <ProgressTemplate>
                        <div style="vertical-align:middle; text-align:center;">
                            <asp:Image ID="imgLoadingTab006" runat="server" ImageUrl="~/Images/waiting.gif" ImageAlign="AbsMiddle" />&nbsp;Loading...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <div style="float:left; height:18px;">
                <asp:Label ID="lblNoticeText" runat="server" ForeColor="Red" Text="" >
                    
                </asp:Label>
            </div>
            <div style="float:right; height:18px;">
                <asp:LinkButton ID="lbReloadExistingProduct" runat="server" Text="Refresh" Font-Underline="true" OnClick="lbReloadExistingProduct_Click" OnClientClick="DisplayProcessing();"></asp:LinkButton>&nbsp;&nbsp;
            </div>
            <br />
            <asp:Panel ID="pnTab006" runat="server" Visible="false">
                <asp:TextBox ID="txtCitizenId" runat="server" Visible="false" ></asp:TextBox>
                <asp:TextBox ID="txtCardTypeId" runat="server" Visible="false" ></asp:TextBox>
                <asp:TextBox ID="txtCountryId" runat="server" Visible="false" ></asp:TextBox>
                <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1170px" />
                <asp:Panel id="pnlExistProduct" runat="server" CssClass="PanelExistingProduct" ScrollBars="Auto">
                    <asp:GridView ID="gvExistProduct" runat="server" AutoGenerateColumns="False" Width="1155"
                        GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" AllowSorting="true" onsorting="gvExistProduct_Sorting"
                        EmptyDataText="<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>"  >
                        <Columns>
                        <asp:BoundField DataField="No" HeaderText="No"  >
                            <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                            <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ProductGroup" HeaderText="Group Product"  SortExpression="ProductGroup" >
                            <HeaderStyle Width="140px" HorizontalAlign="Center" />
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ProductName" HeaderText="Product" SortExpression="ProductName" >
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <%--<asp:BoundField DataField="Grade" HeaderText="Grade">
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>--%>
                        <asp:BoundField DataField="ContactNo" HeaderText="Contract No" SortExpression="ContactNo" >
                            <HeaderStyle Width="140px" HorizontalAlign="Center"/>
                            <ItemStyle Width="140px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Start Date"  SortExpression="StartDate">
                            <ItemTemplate>
                                <%# Eval("StartDate") != null ? Convert.ToDateTime(Eval("StartDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("StartDate")).Year.ToString() : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date"  SortExpression="EndDate" >
                            <ItemTemplate>
                                <%# Eval("EndDate") != null ? Convert.ToDateTime(Eval("EndDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("EndDate")).Year.ToString() : ""%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                            <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="PaymentTerm" HeaderText="ผ่อนชำระมาแล้วกี่งวด">
                            <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                            <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>--%>
                        <asp:BoundField DataField="Status" HeaderText="Grade/Status"  SortExpression="Status" >
                            <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                            <ItemStyle Width="120px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                            <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                            <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
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