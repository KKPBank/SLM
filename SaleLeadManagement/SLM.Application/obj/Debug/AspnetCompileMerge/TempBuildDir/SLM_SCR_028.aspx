<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_028.aspx.cs" Inherits="SLM.Application.SLM_SCR_028" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:130px;
        }
        .input2 { width:125px }
        .info2 { font-weight: bold; width:50px; }
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
                        รหัสสาขา
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtBranchCodeSearch" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อสาขา
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtBranchNameSearch" runat="server" CssClass="Textbox" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                   <tr>
                    <td class="ColInfo">
                        ข่องทาง
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbChannelSearch" runat="server" Width="204px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:5px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        สถานะ
                    </td>
                    <td class="ColInput">
                        <asp:CheckBox ID="cbActiveSearch" runat="server" Text="ใช้งาน"  />&nbsp;
                        <asp:CheckBox ID="cbInActiveSearch" runat="server" Text="ปิดสาขา"  />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        
                    </td>
                    <td class="ColInput">
                        <asp:Button ID="btnSearch" runat="server" CssClass="Button" Text="ค้นหา" Width="100px" OnClick="btnSearch_Click" OnClientClick="DisplayProcessing();" />
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
            <asp:Image ID="imgResult" runat="server" ImageUrl="~/Images/hResult.gif" ImageAlign="Top"  />&nbsp;
            <asp:Button ID="btnAddBranch" runat="server" Text="เพิ่มสาขา" Width="150px" OnClientClick="DisplayProcessing()" 
                CssClass="Button" Height="23px" onclick="btnAddBranch_Click" />
            <br /><br />
            <uc1:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="980px" />
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" Width="980px" 
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>"  >
                <Columns>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Eval("IdBranchCode") %>' OnClick="imbEdit_Click" ToolTip="แก้ไขข้อมูลสาขา" OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                        <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ID">
                        <ItemTemplate>
                            <%# Eval("IdBranchCode")%>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="BranchCode" HeaderText="รหัสสาขา"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="BranchName" HeaderText="ชื่อสาขา"  >
                        <HeaderStyle Width="300px" HorizontalAlign="Center"/>
                        <ItemStyle Width="300px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="เวลาทำการเริ่มต้น">
                        <ItemTemplate>
                            <%# Eval("StartTimeHour") != null && Eval("StartTimeMinute") != null ? Eval("StartTimeHour").ToString() + ":" + Eval("StartTimeMinute").ToString() : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="เวลาทำการสิ้นสุด">
                        <ItemTemplate>
                            <%# Eval("EndTimeHour") != null && Eval("EndTimeMinute") != null ? Eval("EndTimeHour").ToString() + ":" + Eval("EndTimeMinute").ToString() : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top"  />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง"  >
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top"   />
                    </asp:BoundField>
                    <asp:BoundField DataField="StatusDesc" HeaderText="สถานะ"  >
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top"   />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle CssClass="t_rowhead" />
                <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upPopup" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnPopup" Width="0px" CssClass="Hidden"/>
	        <asp:Panel runat="server" ID="pnPopup" style="display:none; height: 450px;" CssClass="modalPopupBranch">
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Image ID="imgSaveNote" runat="server" ImageUrl="~/Images/Branch.png" />
                <asp:CheckBox ID="cbEdit" runat="server" Text="Edit" Visible="false" />
				<asp:TextBox ID="txtBranchCodeOldPopup" Visible="False" runat="server" />
		        <table cellpadding="2" cellspacing="0" border="0" >
                    <tr><td colspan="3" style="height:1px;"></td></tr>
                    <tr id="trIdInfo" runat="server">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            ID<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtBranchCodeIdPopup" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            รหัสสาขา<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtBranchCodePopup" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                            <br />
                            <asp:Label ID="alertBranchCodePopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            ชื่อสาขา<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtBranchNamePopup" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                            <br />
                            <asp:Label ID="alertBranchNamePopup" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
						<td style="width: 20px;"></td>
                        <td class="ColInfo">
                            Upper Branch
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbUpperBranchPopup" runat="server" width="204px" CssClass="Dropdownlist"></asp:DropDownList>
                        </td>
                    </tr>
					<tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            
		 ช่องทาง<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbChannelPopup" runat="server" Width="204px" CssClass="Dropdownlist"></asp:DropDownList>
							<br/>
                            <asp:Label ID="alertChannel" runat="server" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            เวลาทำการเริ่มต้น<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtWorkStartHourPopup" runat="server" CssClass="Textbox" Width="30px" MaxLength="2" ></asp:TextBox>
                            <asp:Label Id="lbWorkstart" runat="server" Text=":" ></asp:Label>
                             <asp:TextBox ID="txtWorkStartMinPopup" runat="server" CssClass="Textbox" Width="30px" MaxLength="2" ></asp:TextBox>&nbsp;
                             <asp:Label ID="alertWorkStartTime" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                     <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            เวลาทำการสิ้นสุด<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtWorkEndHourPopup" runat="server" CssClass="Textbox" Width="30px" MaxLength="2" ></asp:TextBox>
                            <asp:Label Id="lbWorkEnd" runat="server" Text=":" ></asp:Label>
                             <asp:TextBox ID="txtWorkEndMinPopup" runat="server" CssClass="Textbox" Width="30px" MaxLength="2" ></asp:TextBox>&nbsp;
                             <asp:Label ID="alertWorkEndTime" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo"> เลขที่</td>
                        <td>
                            <table>
                                <tr>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtAddressNo" Width="80px" ></asp:TextBox></td>
                                    <td class="info2">หมู่</td>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtMoo"  Width="80px" ></asp:TextBox></td>
                                    <td class="info2">อาคาร</td>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtBuilding" Width="120px"  ></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo"> ชั้น</td>
                        <td>
                            <table>
                                <tr>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtFloor" Width="80px"></asp:TextBox></td>
                                    <td class="info2">ซอย</td>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtLane" Width="120px" ></asp:TextBox></td>
                                    <td class="info2">ถนน</td>
                                    <td class="input2"><asp:TextBox runat="server" ID="txtStreet" Width="120px" ></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo"> จังหวัด</td>
                        <td>
                            <table>
                                <tr>
                                    <td class="input2"><asp:DropDownList runat="server" Width="120px" ID="cmbProvince" OnSelectedIndexChanged="cmbProvince_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList></td>
                                    <td class="info2">อำเภอ</td>
                                    <td class="input2"><asp:DropDownList runat="server" Width="120px" ID="cmbAmphur" OnSelectedIndexChanged="cmbAmphur_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                                    <td class="info2">ตำบล</td>
                                    <td class="input2"><asp:DropDownList runat="server" Width="120px" ID="cmbTambol"></asp:DropDownList></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo"> รหัสไปรษณีย์</td>
                        <td>
                            <table>
                                <tr>
                                    <td class="input2"><asp:TextBox runat="server" Width="80px" ID="txtZipCode"></asp:TextBox></td>
                                    <td class="info2"> </td>
                                    <td class="input2"> </td>
                                    <td class="info2"> </td>
                                    <td class="input2"> </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:4px;"></td></tr>
                    <tr style="vertical-align:top;">
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                            สถานะ<span style="color:Red;">*</span>
                        </td>
                        <td class="ColInput">
                            <asp:RadioButton ID="rbActive" runat="server" Text="ใช้งาน" GroupName="StatusPopup" Checked="true" />&nbsp;
                            <asp:RadioButton ID="rbInActive" runat="server" Text="ปิดสาขา" GroupName="StatusPopup" />
                            <br />
                            <asp:Label ID="alertStatus" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3" style="height:8px;"></td></tr>
                    <tr>
                        <td style="width:20px;"></td>
                        <td class="ColInfo">
                        </td>
                        <td class="ColInput">
                            <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" OnClientClick="DisplayProcessing();" 
                                Width="100px" onclick="btnSave_Click" />&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                Width="100px" onclick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <act:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnPopup" PopupControlID="pnPopup" BackgroundCssClass="modalBackground" DropShadow="True">
	        </act:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
