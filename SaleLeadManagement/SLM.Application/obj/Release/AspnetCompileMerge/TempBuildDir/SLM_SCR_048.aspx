<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_048.aspx.cs" Inherits="SLM.Application.SLM_SCR_048" %>
<%@ Register Src="~/Shared/TextDateMask.ascx" TagPrefix="uc1" TagName="TextDateMask" %>
<%@ Register Src="~/Shared/GridviewPageController.ascx" TagPrefix="uc1" TagName="GridviewPageController" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #menuwrapper ul li a { width: inherit; }
        .style1 { width: 180px; font-weight:bold; }
        .style2 { width: 180px; }
        div.contentAreaControlSheets{ width: inherit; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="2" cellspacing="0" border="0">
            <tr><td colspan="5" style="height:20px;"></td></tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1">วันที่กดเบิก Incentive เริ่มต้น</td>
                <td class="style2"><uc1:TextDateMask ID="tdmIncFrom" runat="server" /></td>
                <td class="style1">วันที่กดเบิก Incentive สิ้นสุด</td>
                <td><uc1:TextDateMask ID="tdmIncTo" runat="server" /></td>
            </tr>
            <tr style="display:none;">
                <td style="width:50px"></td>
                <td class="style1">วันที่เริ่มต้นของวันคุ้มครองกธ</td>
                <td class="style2"><uc1:TextDateMask ID="tdmCovFrom" runat="server" /></td>
                <td class="style1">วันที่สิ้นสุดของวันคุ้มครองกธ</td>
                <td><uc1:TextDateMask ID="tdmCovTo" runat="server" /></td>
            </tr>
            <tr style="display:none;">
                <td style="width:50px"></td>
                <td class="style1">วันที่เริ่มต้นของวันที่แจก Lead</td>
                <td class="style2"><uc1:TextDateMask ID="tdmLeadFrom" runat="server" /></td>
                <td class="style1">วันที่สิ้นสุดของวันที่แจก Lead</td>
                <td><uc1:TextDateMask ID="tdmLeadTo" runat="server" /></td>
            </tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1">Team Telesales</td>
                <td class="style2">
                    <asp:DropDownList ID="cmbTelesale" runat="server" Width="152px" CssClass="Dropdownlist" AutoPostBack="true" OnSelectedIndexChanged="cmbTeamTelesale_SelectedIndexChanged">
                         <asp:ListItem Text="ทั้งหมด"></asp:ListItem>
                         <asp:ListItem Text="NNPS"></asp:ListItem>
                         <asp:ListItem Text="NNAP"></asp:ListItem>
                         <asp:ListItem Text="NNLP"></asp:ListItem>
                         <asp:ListItem Text="NNNP(EXP.)"></asp:ListItem>
                    </asp:DropDownList></td>
                <td class="style1">ชื่อ Telesales</td>
                <td><asp:DropDownList ID="cmbTelesaleName" runat="server" Width="152px" CssClass="Dropdownlist">
                    <asp:ListItem Value="">ทั้งหมด</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1">Level</td>
                <td class="style2"><asp:DropDownList ID="cmbType" runat="server" Width="152px" CssClass="Dropdownlist">
                            <asp:ListItem Text="TAA3"></asp:ListItem>
                            <asp:ListItem Text="TAA4"></asp:ListItem>
                            <asp:ListItem Text="NC"></asp:ListItem>
                            <asp:ListItem Text="TAA2"></asp:ListItem>
                </asp:DropDownList></td>
                <td class="style1"></td>
                <td></td>
            </tr>
            <tr><td colspan="5" style="height:15px;"></td></tr>
            <tr>
                <td style="width:50px"></td>
                <td class="style1"></td>
                <td valign="bottom" colspan="3">
                    <asp:Button ID="btnSearch" runat="server" Text="ค้นหา" CssClass="Button" Width="100px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing()" /> 
                    <asp:Button ID="btnExcel" runat="server" Text="Export Excel ข้อมูล" CssClass="Button" Width="140px" OnClick="btnExcel_Click" />
                </td>
            </tr>
        </table>
    <br />
    <div class="Line"></div>
    <br />
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" /><br />&nbsp;            
            <uc1:GridviewPageController runat="server" ID="pgTop" Visible="false" OnPageChange="pg_PageChange"  />    
            <asp:GridView runat="server" ID="gvMain" AutoGenerateColumns="false" Width="1290px" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" GridLines="Horizontal" BorderWidth="0px">
                <Columns>
<%--                    <asp:TemplateField HeaderText="ลำดับ">
                        <ItemStyle HorizontalAlign="Center" Width="40px" />
                    </asp:TemplateField>--%>
<%--                    <asp:BoundField HeaderText="วันที่กดเบิก" DataField="slm_IncentiveDate" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="วันที่เริ่มต้น คุ้มครอง กธ." DataField="slm_PolicyStartCoverDate" ItemStyle-HorizontalAlign="Center" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="วันที่สิ้นสุด คุ้มครอบ กธ." DataField="slm_PolicyEndCoverDate" ItemStyle-HorizontalAlign="Center"  HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="วันที่แจก Lead" DataField="slm_AssignDate" ItemStyle-HorizontalAlign="Center"  HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}"  />--%>
<%--                    <asp:BoundField HeaderText="Team" DataField="Team" />--%>
                    <asp:BoundField HeaderText="Telesales" DataField="StaffName" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="270px" />
                    <asp:BoundField HeaderText="Level" DataField="Level" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="120px" />
                    <asp:TemplateField HeaderText="จำนวนกรมธรรม์" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px"  >
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# String.Format("{0:#,##0}", SLM.Resource.SLMUtil.SafeDecimal(Eval("NumberOfPolicyNotThisMonth").ToString()) + SLM.Resource.SLMUtil.SafeDecimal(Eval("TotalLead").ToString()))  %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="ค่าเบี้ยหลังหักส่วนลด" DataField="TotalPremium" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="120px" />
                    <asp:BoundField HeaderText="ค่าเบี้ย พรบ" DataField="TotalAct" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="120px" />
                    <asp:BoundField HeaderText="ประกัน+พรบ" DataField="PremiumAndAct" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="120px"  />
                    <asp:BoundField HeaderText="Avg ค่าเบี้ยประกัน" DataField="AvgPremium" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="80px" />
<%--                    <asp:BoundField HeaderText="Avg %ส่วนลด" DataField="AvgDiscount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}" />--%>
                    <asp:BoundField HeaderText="RunRate" DataField="RunRate" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="120px" />
                    <asp:BoundField HeaderText="%Success (ของเดือนที่ทำงาน)" DataField="PercentSuccess" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"  ItemStyle-Width="120px"  />
                    <asp:BoundField HeaderText="Call (พบ)" DataField="Pob" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:#,##0}"  ItemStyle-Width="100px" />
<%--                    <asp:BoundField HeaderText="% Success of Partners" DataField="PercentSuccess" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}" />--%>
                </Columns>
                        <HeaderStyle CssClass="t_rowhead" />
                        <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
            <uc1:GridviewPageController runat="server" ID="pgBot"  Visible="false" OnPageChange="pg_PageChange"/>    



    <asp:Button runat="server" ID="btnTextExcel" Text="TestExcel" OnClick="TestExcel_Click" Visible="false" />
    <asp:Label runat="server" ID="lblResult"></asp:Label>
    <asp:Button runat="server" ID="btnGenXML" Text="GenXML" OnClick="XMLTest_Click" Visible="false" />
    <asp:TextBox runat="server" ID="txtXML" TextMode="MultiLine" Height="400px" Width="600px" Visible="false" ></asp:TextBox>
</asp:Content>
