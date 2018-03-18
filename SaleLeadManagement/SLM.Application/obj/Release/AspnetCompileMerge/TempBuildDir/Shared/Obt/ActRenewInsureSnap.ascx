<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActRenewInsureSnap.ascx.cs"
    Inherits="SLM.Application.Shared.Obt.ActRenewInsureSnap" %>
<%@ Register Src="../GridviewPageController.ascx" TagName="GridviewPageController"
    TagPrefix="uc3" %>
<%--   popup history main--%>
<style type="text/css">
       .td_number
       {
           text-align: right;           
       } 
       .td_lb_sum
       {
           text-align: left;
           font-size:14px;
           font-weight:bold;
       }
       .td_number_sum
       {
           text-align: right;  
           font-size:16px;
           font-weight:bold;
           background-color:#9CEDD5;            
       }
       .btn_close
       {
           float:right;
           padding-top:20px;
           padding-right:50px;
       }
       .t_rowhead
        {            
            text-align:center;
        }
</style>


<script language="javascript" type="text/javascript">
    //debug Snap History
//    function pageLoad() {
//        $(document).ready(function () {
//            //=====for Snap History====  
//            
//        
//        });

//    }

</script>

<asp:UpdatePanel ID="UpHistoryMain" runat="server" UpdateMode="Conditional">
<ContentTemplate>
        <asp:HiddenField ID="hidTicketId" runat="server" />
        <asp:Button runat="server" ID="btnPopupHistoryMain" Width="0px" CssClass="Hidden" />
        <asp:Panel runat="server" ID="pnPopupHistoryMain" Style="display: block" CssClass="modalPopupHistoryMain">
            <div style="height: 450px; overflow: auto">
                <br/>
                <uc3:GridviewPageController ID="pcTopHistory" runat="server" OnPageChange="HistoryPageSearchChange"
                    Width="500px" />
                <asp:GridView ID="gvHistoryMain" runat="server" AutoGenerateColumns="False" Width="500px"
                    GridLines="Horizontal" BorderWidth="0px" OnRowCommand="lblHistoryMain_Click"
                    EnableModelValidation="True" EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>">
                    <Columns>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:ImageButton ImageUrl="~/Images/iSearch.gif" ID="imgHistoryMain" runat="server" OnClientClick="DisplayProcessing();"
                                    CommandName="popup" CommandArgument='<%# Eval("slm_RenewInsureId") + "|" + Eval("slm_Version") %>'>
                                </asp:ImageButton>
                            </ItemTemplate>
                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                            <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="slm_CreatedDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"  HeaderText="วันที่เก็บประวัติ">
                            <HeaderStyle Width="160px" HorizontalAlign="Center" />
                            <ItemStyle Width="160px" HorizontalAlign="center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="slm_Version" HeaderText="เวอร์ชั่น">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle Width="80px" HorizontalAlign="center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="slm_CreatedBy" HeaderText="ผู้บันทึก">
                            <HeaderStyle Width="220px" HorizontalAlign="Center" />
                            <ItemStyle Width="220px" HorizontalAlign="Left" VerticalAlign="Top" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed" />
                </asp:GridView>
                <br/>
                <asp:Button ID="btnCancelPopupHistoryMain" runat="server" Text="ปิดหน้าต่าง" Width="90px"
                    CssClass="Button" OnClick="btnCancelPopupHistoryMain_Click" Style="float: right;" />
            </div>
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupHistoryMain" runat="server" TargetControlID="btnPopupHistoryMain"
            PopupControlID="pnPopupHistoryMain" BackgroundCssClass="modalBackground" DropShadow="True">
        </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
<%--  popup history detail--%>
<asp:UpdatePanel ID="UpHistoryDetail" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupHistoryDetail" Width="0px" CssClass="Hidden" />
        <asp:Panel runat="server" ID="pnPopupHistoryDetail" Style="display: block" CssClass="modalPopupHistoryDetail">
            <div style="height: 450px; overflow: auto">
                <asp:Panel ID="pnHistoryPolicy" runat="server">
                    <table id="tbHistory" cellpadding="2" cellspacing="0" border="1" style="border-collapse:collapse;">
                        <tr>
                            <td id="hist0" class="t_rowhead" align="center">
                                  ข้อมูลประกัน
                            </td>
                            <td id="hist1" class="t_rowhead" align="center">
                                ปีเดิม(<asp:Label ID="lblYear_his1" runat="server" Text=""></asp:Label>)
                                <asp:HiddenField ID="hidHistId1" runat="server" />
                            </td>
                            <td id="hist2" class="t_rowhead" align="center">
                                ปีนี้(<asp:Label ID="lblYear_his2" runat="server" Text=""></asp:Label>)
                                <asp:HiddenField ID="hidHistId2" runat="server" />
                            </td>
                            <td id="hist3" class="t_rowhead" align="center">
                                <asp:Label ID="lblYear_his3" runat="server" Text=""></asp:Label>
                                <asp:HiddenField ID="hidHistId3" runat="server" />
                            </td>
                            <td id="hist4" class="t_rowhead" align="center">
                                <asp:Label ID="lblYear_his4" runat="server" Text=""></asp:Label>
                                <asp:HiddenField ID="hidHistId4" runat="server" />
                            </td>
                            <td id="hist5" class="t_rowhead" align="center">
                                <asp:Label ID="lblYear_his5" runat="server" Text=""></asp:Label>
                                <asp:HiddenField ID="hidHistId5" runat="server" />
                            </td>
                            <td id="hist6" class="t_rowhead" align="center">
                                <asp:Label ID="lblYear_his6" runat="server" Text=""></asp:Label>
                                <asp:HiddenField ID="hidHistId6" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ประกัน
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his1" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his2" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his3" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his4" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his5" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbInsNameTh_his6" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ประเภทประกัน
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his1" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his2" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his3" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his4" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his5" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType_his6" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                บริษัทประกันภัย
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his1" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his2" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his3" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his4" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his5" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInsNameTh_his6" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                เลขที่กรมธรรม์
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his1" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his2" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his3" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his4" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his5" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Number_his6" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ชื่อผู้ขับขี่
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rbDriver_Flag_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                คำนำหน้าผู้ขับขี่คนที่ 1
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName1_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ชื่อผู้ขับขี่คนที่ 1
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name1_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                นามสกุลผู้ขับขี่คนที่ 1
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name1_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                วันเกิดผู้ขับขี่คนที่ 1
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate1_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                คำนำหน้าผู้ขับขี่คนที่ 2
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="cmbTitleName2_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ชื่อผู้ขับขี่คนที่ 2
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_First_Name2_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                นามสกุลผู้ขับขี่คนที่ 2
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="txtDriver_Last_Name2_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                วันเกิดผู้ขับขี่คนที่ 2
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="tdmDriver_Birthdate2_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                เลขที่รับแจ้ง
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblInformed_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                วันเริ่มคุ้มครอง
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Eff_Date_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                วันหมดอายุประกัน
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVoluntary_Policy_Exp_Date_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr style="display:none">
                            <td>
                                ประเภทประกัน
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCoverageType2_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ประเภทการซ่อม
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="hidMaintanance_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ค่าเสียหายส่วนแรก
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCost_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ทุนประกันความเสียหายต่อตัวรถยนต์ (OD)
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Cov_Amt_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ทุนประกันกรณีรถยนต์สูญหาย/ไฟไหม้ (F&amp;T)
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblCostFT_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                เบี้ยสุทธิ
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblNetpremium_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                อากร
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblDuty_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ภาษี
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVat_amount_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                เบี้ยประกันรวมภาษีอากร
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblVoluntary_Gross_Premium_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ประเภท
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his1" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his2" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his3" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his4" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his5" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCardTypeId_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chkCardType_his" runat="server" AutoPostBack="true"  onclick="javascript: return false;"/>
                                หัก 1% (กรณีนิติบุคคล)
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="lblPersonType_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ส่วนลด(%)
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountPercent_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ส่วนลด(บาท)
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his5" runat="server"></asp:Label>
                            </td>
                            <td class="td_number">
                                <asp:Label ID="txtDiscountBath_his6" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_lb_sum">
                                เบี้ยประกันที่ต้องชำระ
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his1"  runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his2"  runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his5"  runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtTotal_Voluntary_Gross_Premium_his6"  runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td align="center" class="td_lb_sum">
                                ลูกค้าประหยัดเงิน
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtSafe_his1" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtSafe_his2" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtSafe_his3" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtSafe_his4" runat="server"></asp:Label>
                            </td>
                            <td class="td_number_sum">
                                <asp:Label ID="txtSafe_his5" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    ข้อมูลผู้รับผลประโยชน์&nbsp;
                    <asp:TextBox ID="txtBeneficiaryName" runat="server" CssClass="TextboxView" ReadOnly="true" Width="441px"></asp:TextBox>
                    <br /><br /><br />
                </asp:Panel>              
                <table id="tbDtlAct" cellpadding="2" cellspacing="0" border="1" style="border-collapse:collapse;" >
                    <tr>
                        <td class="t_rowhead" style="width: 155px">
                            ข้อมูล พรบ.
                        </td>
                        <td class="t_rowhead">
                            ปีเดิม (<asp:Label ID="lblPromoAct_name_his1" runat="server" Text=""></asp:Label>)
                            <asp:HiddenField ID="hdActHist1" runat="server" />
                        </td>
                        <td class="t_rowhead">
                            <asp:Label ID="lblPromoAct_name_his2" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hdActHist2" runat="server" />
                        </td>
                        <td class="t_rowhead">
                            <asp:Label ID="lblPromoAct_name_his3" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hdActHist3" runat="server" />
                        </td>
                        <td class="t_rowhead">
                            <asp:Label ID="lblPromoAct_name_his4" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hdActHist4" runat="server" />
                        </td>
                        <td class="t_rowhead">
                            <asp:Label ID="lblPromoAct_name_his5" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hdActHist5" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            พรบ.
                        </td>
                        <td>
                            <asp:Label ID="rbAct_his1" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbAct_his2" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbAct_his3" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbAct_his4" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbAct_his5" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ออกที่
                        </td>
                        <td>
                            <asp:Label ID="lblActIssuePlace_his1" runat="server"></asp:Label>
                            &nbsp;
                            <asp:Label ID="lblActIssueBranch_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblActIssuePlace_his2" Style="text-align: left"  runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="lblActIssueBranch_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblActIssuePlace_his3" Style="text-align: left"  runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="lblActIssueBranch_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblActIssuePlace_his4" Style="text-align: left"  runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="lblActIssueBranch_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblActIssuePlace_his5" Style="text-align: left"  runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="lblActIssueBranch_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            เอกสารลงทะเบียน
                        </td>
                        <td>
                            <asp:Label ID="rbRegisterAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbRegisterAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbRegisterAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbRegisterAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="rbRegisterAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            บริษัท พรบ.
                        </td>
                        <td>
                            <asp:Label ID="lblCompanyInsuranceAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCompanyInsuranceAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCompanyInsuranceAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCompanyInsuranceAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCompanyInsuranceAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            เลขเครื่องหมายตาม พรบ.
                        </td>
                        <td>
                            <asp:Label ID="txtSignNoAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtSignNoAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtSignNoAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtSignNoAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtSignNoAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            วันเริ่มต้น พรบ.
                        </td>
                        <td>
                            <asp:Label ID="txtActStartCoverDateAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActStartCoverDateAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActStartCoverDateAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActStartCoverDateAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActStartCoverDateAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            วันสิ้นสุด พรบ.
                        </td>
                        <td>
                            <asp:Label ID="txtActEndCoverDateAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActEndCoverDateAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActEndCoverDateAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActEndCoverDateAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtActEndCoverDateAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            วันสิ้นสุดอายุภาษีรถยนต์
                        </td>
                        <td>
                            <asp:Label ID="txtCarTaxExpiredDateAct_his1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtCarTaxExpiredDateAct_his2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtCarTaxExpiredDateAct_his3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtCarTaxExpiredDateAct_his4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="txtCarTaxExpiredDateAct_his5" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            เบี้ยสุทธิ (เต็มปี)
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumFullAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumFullAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumFullAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumFullAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumFullAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            เบี้ยสุทธิ
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossPremiumAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number" >
                            <asp:Label ID="lblActGrossPremiumAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossPremiumAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossPremiumAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossPremiumAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            อากร
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossStampAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossStampAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossStampAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossStampAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossStampAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ภาษี
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossVatAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossVatAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossVatAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossVatAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActGrossVatAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            เบี้ย พรบ.รวมภาษีอากร
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActNetGrossPremiumAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkCardType_his2" runat="server" onclick="javascript: return false;"/>
                            หัก 1% (กรณีนิติบุคคล)
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActPersonType_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActPersonType_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActPersonType_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActPersonType_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblActPersonType_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ส่วนลด (%)
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblVat1PercentBathAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblVat1PercentBathAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblVat1PercentBathAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblVat1PercentBathAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblVat1PercentBathAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ส่วนลด (บาท)
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblDiscountPercentAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblDiscountPercentAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblDiscountPercentAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblDiscountPercentAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number">
                            <asp:Label ID="lblDiscountPercentAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_lb_sum">
                            เบี้ย พรบ. ที่ต้องชำระ
                        </td>
                        <td class="td_number_sum">
                            <asp:Label ID="txtDiscountBathAct_his1" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number_sum">
                            <asp:Label ID="txtDiscountBathAct_his2" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number_sum">
                            <asp:Label ID="txtDiscountBathAct_his3" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number_sum">
                            <asp:Label ID="txtDiscountBathAct_his4" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                        <td class="td_number_sum">
                            <asp:Label ID="txtDiscountBathAct_his5" class="td_number" 
                                runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                
            </div>
            <div class="btn_close">
                <asp:Label ID="lbErrorMsg" runat="server" Text="Show error message" Visible="false"></asp:Label>
                <asp:Button ID="btnClosePopupHistoryMain" runat="server" Text="ปิดหน้าต่าง" Width="90px"
                    CssClass="Button" OnClick="btnCancelPopupHistoryMain_Click"  />
            </div>
        </asp:Panel>
        <act:ModalPopupExtender ID="ModalPopupHistoryDetail" runat="server" TargetControlID="btnPopupHistoryDetail"
            PopupControlID="pnPopupHistoryDetail" BackgroundCssClass="modalBackground" DropShadow="True">
        </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
