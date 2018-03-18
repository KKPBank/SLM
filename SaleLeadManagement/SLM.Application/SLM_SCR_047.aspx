<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_047.aspx.cs" Inherits="SLM.Application.SLM_SCR_047" %>

<%@ Register Src="Shared/TextDateMask.ascx" TagName="TextDateMask" TagPrefix="uc1" %>
<%@ Register Src="Shared/GridviewPageController.ascx" TagName="GridviewPageController" TagPrefix="uc2" %>
<%@ Register Src="Shared/Calendar.ascx" TagName="Calendar" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo {
            font-weight: bold;
            width: 190px;
        }

        .ColInput {
            width: 250px;
        }

        .ColCheckBox {
            width: 160px;
        }

        .modalPopupAddReceiveNo {
            background-color: #ffffff;
            border-width: 1px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            width: 550px;
            /*height:420px;*/
        }

        .style4 {
            font-family: Tahoma;
            font-size: 9pt;
            color: Red;
        }
    </style>
    <!-- Comment By Pom 23/05/2016 -->
    <%--<script type="text/jscript" src="Scripts/SlmScript.js"></script>
    <script type="text/javascript" src="Scripts/jquery-2.2.0.min.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td colspan="4" style="height: 2px;"></td>
                </tr>
                <tr>
                    <td class="ColInfo">Transfer Date เริ่มต้น <span class="style4">*</span>
                    </td>
                    <td class="ColInput">
                        <uc3:Calendar ID="tdmTransferDateStart" runat="server" HideClearButton="true" ClientIDMode="AutoID" />
                        &nbsp;
                    </td>

                    <td class="ColInfo">Transfer Date สิ้นสุด<span class="style4">*</span>
                    </td>
                    <td class="ColInput">
                        <uc3:Calendar ID="tdmTransferDateEnd" runat="server" HideClearButton="true" ClientIDMode="AutoID" />
                        &nbsp;
                    </td>
                </tr>
            </table>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height: 3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo"></td>
                    <td colspan="5">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()"
                            Text="ค้นหา" OnClick="btnSearch_Click" />&nbsp;
                        <asp:Button ID="btnClear" runat="server" CssClass="Button" Width="100px" OnClientClick="DisplayProcessing()" OnClick="btnClear_Click"
                            Text="ล้างข้อมูล" />
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
            <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1080px"  />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False"
                GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1080px">
                <Columns>
                    <%--<asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("BenefitId") %>' ToolTip="แก้ไขเลขรับแจ้ง" OnClick="imbEdit_Click" />
                        </ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    </asp:TemplateField>--%>
                    <%-- Comment By Pom 23/05/2016 --%>
                    <%--<asp:BoundField HeaderText="No" DataField="slm_PreleadMonitorId" DataFormatString="{0:0}">
                        <HeaderStyle Width="20px" HorizontalAlign="Center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>--%>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="20px" HorizontalAlign="Center" />
                        <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Transfer Date">
                        <ItemTemplate>
                            <%# Eval("slm_CreatedDate") != null ? Convert.ToDateTime(Eval("slm_CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slm_CreatedDate")).Year.ToString() : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Lot ที่" DataField="slm_CMTLot">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Total" DataField="slm_TotalPort" DataFormatString="{0:#,###}">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Success" DataField="slm_Success" DataFormatString="{0:#,###}">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Dedup" DataField="slm_DeDup" DataFormatString="{0:#,###}">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Blacklist" DataField="slm_Blaklist" DataFormatString="{0:#,###}">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Exceptional" DataField="slm_Exceptional" DataFormatString="{0:#,###}">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="สถานะ" DataField="slm_PortStatus">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>

                            <%--<asp:LinkButton CssClass="Button" id="btnLink" runat="server" Text="ตรวจสอบ" NavigateUrl='<%# String.Format("~/SLM_SCR_032.aspx?lot={0}", Eval("slm_CMTLot")) %>' 
                                Visible='<%# Eval("slm_PortStatus").ToString().ToLower() == "waiting" ? true : false %>'></asp:LinkButton>--%>
                            <asp:Button CssClass="Button" ID="btnLink" runat="server" Text="ตรวจสอบ" CommandArgument='<%# Eval("slm_CMTLot") %>' OnClick="btnLink_Click"
                                Visible='<%# Eval("slm_PortStatus").ToString().ToLower() == "allocated" ? true : false %>' OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed" />
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>

   <%-- <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden" />
            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
            </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>--%>

</asp:Content>

