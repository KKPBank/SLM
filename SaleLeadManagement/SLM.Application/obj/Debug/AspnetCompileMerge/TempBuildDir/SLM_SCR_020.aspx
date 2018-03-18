<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_020.aspx.cs" Inherits="SLM.Application.SLM_SCR_020" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:120px;
        }
        .ColInput
        {
            width:650px;
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
                        หัวข้อ
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtSuggestionTopic" runat="server" CssClass="Textbox" Width="500px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ประเภท
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbSuggestionType" runat="server" CssClass="Dropdownlist" Width="260px">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                            <asp:ListItem Value="1" Text="ข้อเสนอแนะ"></asp:ListItem>
                            <asp:ListItem Value="2" Text="แจ้งปัญหาการใช้งาน"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        วันที่บันทึก
                    </td>
                    <td class="ColInput">
                        <uc2:TextDateMask ID="tdmCreateDateFrom" runat="server" />&nbsp;
                        ถึง
                        <uc2:TextDateMask ID="tdmCreateDateTo" runat="server" />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px" />
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
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top"  />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="1140px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="1140px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                            <%# Eval("No") %>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Topic" HeaderText="หัวข้อ"  >
                        <HeaderStyle Width="300px" HorizontalAlign="Center"/>
                        <ItemStyle Width="300px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="Type" HeaderText="ประเภท"  >
                        <HeaderStyle Width="120px" HorizontalAlign="Center"/>
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="Detail" HeaderText="รายละเอียด"  >
                        <HeaderStyle Width="420px" HorizontalAlign="Center"/>
                        <ItemStyle Width="420px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreatedUser" HeaderText="ผู้บันทึก"  >
                        <HeaderStyle Width="200px" HorizontalAlign="Center"/>
                        <ItemStyle Width="200px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="วันที่บันทึก">
                        <ItemTemplate>
                            <%# Eval("CreatedDate") %>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate> 
    </asp:UpdatePanel>
</asp:Content>
