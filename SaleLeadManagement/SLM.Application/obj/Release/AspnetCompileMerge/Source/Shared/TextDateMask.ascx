<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextDateMask.ascx.cs" Inherits="SLM.Application.Shared.TextDateMask" %>

<style type="text/css"> .ajax__calendar { z-index:500000 !important; position:relative; }</style>

<asp:TextBox ID="txtDate" runat="server" Width="100px" CssClass="Textbox"></asp:TextBox>
<asp:ImageButton runat="Server" ID="imgCalendar" ImageUrl="~/Images/calendar.gif" Width="16px" ToolTip="Click to show calendar" ImageAlign="Absmiddle" />
<act:CalendarExtender ID="txtDate_CalendarExtender" runat="server"
    TargetControlID="txtDate" PopupButtonID="imgCalendar" Format="dd/MM/yyyy" >
</act:CalendarExtender>
<act:MaskedEditExtender ID="txtDate_MaskedEditExtender" runat="server"
    CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
    CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
    Mask="99/99/9999" MaskType="Date" TargetControlID="txtDate" 
    UserDateFormat="DayMonthYear" CultureName="th-TH" ClearTextOnInvalid="True" 
    ErrorTooltipEnabled="True"
    MessageValidatorTip="False" Century="1900">
</act:MaskedEditExtender>