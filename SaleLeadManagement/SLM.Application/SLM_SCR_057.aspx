<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_057.aspx.cs" Inherits="SLM.Application.SLM_SCR_057" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:110px;
        }
        .ColInput
        {
            width:250px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:HiddenField ID="hdfUploadLeadId" runat="server"  />
    <table cellpadding="2" cellspacing="0" border="0">
        <tr><td colspan="4" style="height:2px;"></td></tr>
        <tr>
            <td class="ColInfo" style="padding-left:15px;">
                ช่องทาง
            </td>
            <td class="ColInput">
                <asp:DropDownList ID="cmbChannel" runat="server" CssClass="Dropdownlist" Width="200px" Enabled="false">
                </asp:DropDownList>
            </td>
            <td class="ColInfo">
                แคมเปญ
            </td>
            <td >
                <asp:DropDownList ID="cmbCampaign" runat="server" CssClass="Dropdownlist" Width="300px" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr><td colspan="4" style="height:1px;"></td></tr>
        <tr>
            <td class="ColInfo" style="padding-left:15px;">
                ชื่อไฟล์
            </td>
            <td colspan="3">
                <asp:TextBox ID="txtFileName" runat="server" ReadOnly="true" CssClass="TextboxView" Width="362px"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <div class="Line"></div>
    <br />
    <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1290px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" Width="1290px"  >
                <Columns>
                    <asp:TemplateField HeaderText="ลำดับที่">
                        <ItemTemplate>
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชื่อลูกค้า">
                        <ItemTemplate>
                            <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="นามสกุลลูกค้า">
                        <ItemTemplate>
                            <asp:Label ID="lblLastname" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ประเภทลูกค้า">
                        <ItemTemplate>
                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="บัตรประชาชน/นิติบุคคล">
                        <ItemTemplate>
                            <asp:Label ID="lblCitizenId" runat="server" Text='<%# Eval("CitizenId") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="110px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Owner<br />Lead ID">
                        <ItemTemplate>
                            <asp:Label ID="lblOwnerEmpID" runat="server" Text='<%# Eval("OwnerEmpCode") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Delegate<br />Lead ID">
                        <ItemTemplate>
                            <asp:Label ID="lblDelegateLead" runat="server" Text='<%# Eval("DelegateEmpCode") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="เบอร์โทร 1">
                            <ItemTemplate>
                                <asp:Label ID="lblTelNo1" runat="server" Text='<%# Eval("TelNo1") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="เบอร์โทร 2">
                            <ItemTemplate>
                                <asp:Label ID="lblTelNo2" runat="server" Text='<%# Eval("TelNo2") %>' ></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="90px" HorizontalAlign="Center" VerticalAlign="Top" />
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="รายละเอียด Lead">
                        <ItemTemplate>
                            <asp:Label ID="lblDetail" runat="server" Text='<%# Eval("Detail") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="310px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="310px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สถานะรายการ">
                        <ItemTemplate>
                            <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Eval("StatusDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="หมายเหตุ">
                        <ItemTemplate>
                            <asp:Label ID="lblRemark" runat="server" Text='<%# Eval("Remark") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br /><br />
    <div style="border:solid 0px; float:right; padding-right:130px;">
        <asp:UpdatePanel ID="upControlButton" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <asp:Button ID="btnDownload" runat="server" Text="Download" Width="100px" CssClass="Button" OnClick="btnDownload_Click" OnClientClick="if (confirm('ต้องการดาวโหลดไฟล์ใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
                <asp:Button ID="btnBack" runat="server" Text="ยกเลิก" Width="100px" CssClass="Button" OnClick="btnBack_Click" OnClientClick="if (confirm('ต้องการกลับหน้าค้นหาใช่หรือไม่')) { DisplayProcessing(); return true; } else { return false; }" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <br /><br />
</asp:Content>
