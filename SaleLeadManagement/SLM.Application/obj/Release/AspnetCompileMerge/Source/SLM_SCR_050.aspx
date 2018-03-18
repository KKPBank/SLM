<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_050.aspx.cs" Inherits="SLM.Application.SLM_SCR_050" %>
<%@ Register Src="~/Shared/TextDateMask.ascx" TagPrefix="uc1" TagName="TextDateMask" %>
<%@ Register Src="~/Shared/GridviewPageController.ascx" TagPrefix="uc1" TagName="GridviewPageController" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #menuwrapper ul li a { width: inherit; }
         .style1 { width: 220px; font-weight:bold; }
         .style2 { width: 180px; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExcel1" />
            <asp:PostBackTrigger ControlID="btnExcel2" />
            <asp:PostBackTrigger ControlID="btnExcel3" />
        </Triggers>        
        <ContentTemplate>
            <asp:Label runat="server" ID="lblError" ForeColor="Red" ViewStateMode="Disabled"></asp:Label>
            <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="5" style="height:20px;"></td></tr>
                    <tr>
                        <td style="width:50px"></td>
                        <td class="style1">วันที่กดเบิก Incentive เริ่มต้น <span class="require">*</span></td>
                        <td class="style2"><uc1:TextDateMask ID="tdmIncFrom" runat="server" /></td>
                        <td class="style1">วันที่กดเบิก Incentive สิ้นสุด <span class="require">*</span></td>
                        <td><uc1:TextDateMask ID="tdmIncTo" runat="server" /></td>
                    </tr>
                     <tr>
                        <td style="width:50px"></td>
                        <td class="style1">วันที่เริ่มต้นของวันคุ้มครองกธ</td>
                        <td class="style2"><uc1:TextDateMask ID="tdmFrom" runat="server" /></td>
                        <td class="style1">วันที่สิ้นสุดของวันคุ้มครองกธ</td>
                        <td><uc1:TextDateMask ID="tdmTo" runat="server" /></td>
                    </tr>
                    <tr>
                        <td style="width:50px"></td>
                        <td class="style1">Team Telesales</td>
                        <td class="style2">
                        <asp:DropDownList ID="cmbTeamTelesale" runat="server" Width="152px" CssClass="Dropdownlist" OnSelectedIndexChanged="cmbTeamTelesale_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="ทั้งหมด"></asp:ListItem>
                            <asp:ListItem Text="NNPS"></asp:ListItem>
                            <asp:ListItem Text="NNAP"></asp:ListItem>
                            <asp:ListItem Text="NNLP"></asp:ListItem>
                            <asp:ListItem Text="NNNP(EXP.)"></asp:ListItem>
                        </asp:DropDownList></td>
                        <td class="style1">ชื่อ Telesales</td>
                        <td><asp:DropDownList ID="cmbTelsameName" runat="server" Width="152px" CssClass="Dropdownlist">
                            <asp:ListItem>ทั้งหมด</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>

                    <tr>
                        <td style="width:50px"></td>
                        <td class="style1">สังกัดของหน่วยงาน</td>
                        <td class="style2">
                            <asp:DropDownList ID="cmbStaffCategory" runat="server" Width="152px" CssClass="Dropdownlist">
                                <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
                                <asp:ListItem Text="KK" Value="1"></asp:ListItem>
                                 <asp:ListItem Text="Outsource" Value="2"></asp:ListItem>
                            </asp:DropDownList></td>
                        <td class="style1">เกรด</td>
                        <td><asp:DropDownList ID="cmbIncentiveFlat" runat="server" Width="152px" CssClass="Dropdownlist">
                                <asp:ListItem Text="ทั้งหมด" Value="0"></asp:ListItem>
                                <asp:ListItem Text="เบิกแล้ว" Value="1"></asp:ListItem>
                                <asp:ListItem Text="ยังไม่เบิก" Value="2"></asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr><td colspan="5" style="height:15px;"></td></tr>
                    <tr>
                        <td style="width:50px"></td>
                        <td class="style1"></td>
                        <td valign="bottom" colspan="3">
                            <asp:Button ID="btnSearch" runat="server" Text="ค้นหา" CssClass="Button" Width="100px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing()" />&nbsp;
                            <asp:Button ID="btnExcel1" runat="server" Text="Export CSV ข้อมูลรายละเอียด ประกัน" CssClass="Button" Width="230px" OnClick="btnExcel1_Click" OnClientClick="DisplayProcessing()" />
                            <asp:Button ID="btnExcel2" runat="server" Text="Export CSV ข้อมูลรายละเอียด พรบ" CssClass="Button" Width="230px" OnClick="btnExcel2_Click" OnClientClick="DisplayProcessing()" />
                            <asp:Button ID="btnExcel3" runat="server" Text="Export CSV ใบปะหน้า" CssClass="Button" Width="150px" OnClick="btnExcel3_Click" OnClientClick="DisplayProcessing()" />
                        </td>
                    </tr>
                </table>
    <br />
    <div class="Line"></div>
    <br />
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top" /><br />&nbsp;            
            <uc1:GridviewPageController runat="server" ID="pgTop" Visible="false" OnPageChange="pg_PageChange"  />    
            <asp:GridView runat="server" ID="gvMain" AutoGenerateColumns="false" Width="100%" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" GridLines="Horizontal" BorderWidth="0px">
                <Columns>
                    <asp:BoundField HeaderText="Telesales" DataField="StaffName" />
                    <asp:BoundField HeaderText="เลขที่สัญญาเช่าซื้อ" DataField="ContractNo" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="คำนำหน้าชื่อ" DataField="TitleName" />
                    <asp:BoundField HeaderText="ชื่อผู้เอาประกัน" DataField="Name" />
                    <asp:BoundField HeaderText="นามสกุลผู้เอาประกัน" DataField="LastName" />
                    <asp:BoundField HeaderText="เลขทะเบียน" DataField="LicenseNo" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="รายชื่อบริษัทประกันภัย" DataField="InsName" />
                    <asp:BoundField HeaderText="ประเภทความคุ้มครอง" DataField="CoverageTypeName" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="เบี้ยประกันภัย (เบี้ยเต็ม)" DataField="PolicyGrossTotal" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}" />
                    <asp:BoundField HeaderText="ค่าเบี้ยหลังหักส่วนลด" DataField="PolicyGross" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:#,##0.00}"/>
                </Columns>
                        <HeaderStyle CssClass="t_rowhead" />
                        <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
            <uc1:GridviewPageController runat="server" ID="pgBot"  Visible="false" OnPageChange="pg_PageChange"/>    
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
