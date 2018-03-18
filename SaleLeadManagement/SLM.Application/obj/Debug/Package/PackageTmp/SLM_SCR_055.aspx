<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_055.aspx.cs" Inherits="SLM.Application.SLM_SCR_055" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:140px;
        }
        .ColInput
        {
            width:270px;
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
                    <td class="ColInfo" style="padding-left:15px;">
                        ชื่อไฟล์
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtFilename" runat="server" CssClass="Textbox" Width="240px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        สถานะไฟล์
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbFileStatus" runat="server" CssClass="Dropdownlist" Width="150px">
                            <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
                            <asp:ListItem Text="Fail" Value="Fail"></asp:ListItem>
                            <asp:ListItem Text="Submit" Value="Submit"></asp:ListItem>
                            <asp:ListItem Text="Success" Value="Success"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="4" style="height:4px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td colspan="3">
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
            <asp:Button ID="btnUploadNewLead" runat="server" Text="สร้างข้อมูล" Width="100px" CssClass="Button" Height="24px" onclick="btnUploadNewLead_Click"  />
            <div style="height:12px;"></div>
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1080px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1080px"  >
                <Columns>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            &nbsp;<asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Eval("UploadLeadId") %>' OnClick="imbView_Click" OnClientClick="DisplayProcessing()" ToolTip="ดูข้อมูลการอัพโหลด"  />
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" Visible='<%# Eval("FileStatus") != null ? (Eval("FileStatus").ToString().ToLower() == "submit") : false %>' CommandArgument='<%# Eval("UploadLeadId") %>' OnClick="imbEdit_Click" OnClientClick="DisplayProcessing()" ToolTip="แก้ไขข้อมูลการอัพโหลด"  />
                        </ItemTemplate>
                        <ItemStyle Width="60px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อไฟล์">
                        <ItemTemplate>
                            <asp:Label ID="lblFilename" runat="server" Text='<%# Eval("Filename") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="270px" HorizontalAlign="Center"/>
                        <ItemStyle Width="270px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="จำนวน Lead">
                        <ItemTemplate>
                            <asp:Label ID="lblNumOfLead" runat="server" Text='<%# Eval("LeadCount") != null ? Convert.ToInt32(Eval("LeadCount")).ToString("#,##0") : "" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่ Upload ล่าสุด">
                        <ItemTemplate>
                            <%# Eval("LastestUploadDate") != null ? Convert.ToDateTime(Eval("LastestUploadDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("LastestUploadDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("LastestUploadDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="160px" HorizontalAlign="Center"/>
                        <ItemStyle Width="160px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่แจกงาน">
                        <ItemTemplate>
                            <%# Eval("AssignedDate") != null ? Convert.ToDateTime(Eval("AssignedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("AssignedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("AssignedDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="160px" HorizontalAlign="Center"/>
                        <ItemStyle Width="160px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อผู้ Upload">
                        <ItemTemplate>
                            <asp:Label ID="lblUploaderName" runat="server" Text='<%# Eval("UploaderName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="230px" HorizontalAlign="Center"/>
                        <ItemStyle Width="230px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สถานะไฟล์">
                        <ItemTemplate>
                            <asp:Label ID="lblFileStatus" runat="server" Text='<%# Eval("FileStatus") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
