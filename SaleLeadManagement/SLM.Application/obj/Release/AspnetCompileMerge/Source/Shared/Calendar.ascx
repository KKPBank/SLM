<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="SLM.Application.Shared.Calendar" %>

<style type="text/css"> .ajax__calendar { z-index:500000 !important; position:relative; }</style>

<script language="javascript" type="text/javascript">
    function ClearDate() {
        var textbox = '<%= txtDate.ClientID %>';        
        $find(textbox).set_selectedDate(null);
        return false;
    }
</script>
<asp:TextBox runat="server" ID="txtDate"  Width="100px" Enabled="false" CssClass="Textbox" />
<asp:ImageButton runat="Server" ID="imgCalendar" ImageUrl="~/Images/calendar.gif" ToolTip="Click to show calendar" ImageAlign="Absmiddle"/>
<act:CalendarExtender ID="calendar" runat="server" TargetControlID="txtDate"  Format="dd/MM/yyyy" PopupButtonID="imgCalendar" ClearTime="False" />
<asp:ImageButton runat="Server" ID="imbClear" ImageUrl="~/Images/bDelete.gif" ToolTip="Clear calendar" ImageAlign="Absmiddle" OnClientClick="return ClearDate()" />