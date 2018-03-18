<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_010.aspx.cs" Inherits="SLM.Application.SLM_SCR_010x" EnableEventValidation="false" %>
<%@ Register Src="~/Shared/Lead_Share_Common.ascx" TagPrefix="uc1" TagName="Lead_Share_Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .style1 { width: 50px; }
    .style2 { width: 180px; text-align:left; font-weight:bold; }
    .style3 { width: 280px; text-align:left; }
    .style4 { font-family: Tahoma; font-size: 9pt; color: Red; }
    .style5 { width: 955px; }
    .style6 { font-family: Tahoma; font-size: 9pt; color: blue; }
    .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -20px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
        }
    .ui-autocomplete-input { width: 249px !important; margin-right: 0px; }

</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdfCopyTicket" />
            <asp:Panel runat="server" ID="pnlMain" DefaultButton="btnSave">

                <asp:Panel runat="server" ID="pnlSubMain" DefaultButton="btnNext">
                <uc1:Lead_Share_Common runat="server" id="ctlCommon" />
                <asp:PlaceHolder runat="server" ID="plcControl" EnableViewState="true" />
                &nbsp;
                <hr />
                    <table  class="style5" runat="server" id="tbNext" >
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnNext" runat="server" Text="ถัดไป &gt;&gt;" CssClass="Button" 
                                    Width="90px" OnClick="btnNext_Click"  OnClientClick="DisplayProcessing()" /> &nbsp;&nbsp;
                                <asp:Button ID="btnCancelNext" runat="server" Text="ยกเลิก" CssClass="Button" 
                                    Width="90px" OnClientClick="return confirm('ต้องการยกเลิกใช่หรือไม่')" 
                                    OnClick="btnCancelNext_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                    <table  class="style5" runat="server" id="tbSave" visible="false" >
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnSave" runat="server" Text="บันทึก" CssClass="Button" 
                                    Width="90px" onclick="btnSave_Click"  OnClientClick="DisplayProcessing()" /> &nbsp;&nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="ยกเลิก" CssClass="Button" 
                                    Width="90px" OnClientClick="if (confirm('ต้องการยกเลิกใช่หรือไม่')) DisplayProcessing(); else return false;" 
                                    onclick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:UpdatePanel ID="upPopupSaveResult" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Button runat="server" ID="btnPopupSaveResult" Width="0px" CssClass="Hidden"/>
	    <asp:Panel runat="server" ID="pnPopupSaveResult" style="display:none" CssClass="modalPopupCreateLeadResult">
            <br />
		    <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="2" style="height:1px;"></td></tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo" style="font-size:14px; width:380px;">
                        <b>บันทึกข้อมูลผู้มุ่งหวังสำเร็จ</b>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:5px;"></td></tr>
                 <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>Ticket Id:</b>&nbsp;<asp:Label ID="lblResultTicketId" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>แคมเปญ:</b>&nbsp;<asp:Label ID="lblResultCampaign" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>ช่องทาง:</b>&nbsp;<asp:Label ID="lblResultChannel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <b>Owner Lead:</b>&nbsp;<asp:Label ID="lblResultOwnerLead" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:8px;"></td></tr>
                <tr>
                    <td style="width:40px;"></td>
                    <td class="ColInfo">
                        <asp:Label ID="lblResultMessage" runat="server"></asp:Label>
                        <asp:CheckBox ID="cbResultHasAdamsUrl" runat="server" Text="CallAdams" Enabled="false" Visible="false" />
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td align="center" colspan="2" class="ColInfo">
                        <asp:Button ID="btnAttachDocYes" runat="server" Text="ใช่" CssClass="Button" OnClientClick="DisplayProcessing();" 
                            Width="100px" OnClick="btnAttachDocYes_Click" />&nbsp;
                        <asp:Button ID="btnAttachDocNo" runat="server" Text="ไม่ใช่" CssClass="Button" OnClientClick="DisplayProcessing();" 
                            Width="100px" OnClick="btnAttachDocNo_Click"  />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <act:ModalPopupExtender ID="mpePopupSaveResult" runat="server" TargetControlID="btnPopupSaveResult" PopupControlID="pnPopupSaveResult" BackgroundCssClass="modalBackground" DropShadow="True">
	    </act:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
