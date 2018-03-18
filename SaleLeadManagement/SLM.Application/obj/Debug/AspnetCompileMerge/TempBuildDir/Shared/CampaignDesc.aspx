<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CampaignDesc.aspx.cs" Inherits="SLM.Application.Shared.CampaignDesc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>รายละเอียดแคมเปญ</title>
    <link href="../Styles/kk.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color:#e5edf5;">
    <form id="form1" runat="server">
    <div style="padding:10px;">
        <asp:Label ID="lblCampaignName" runat="server" Font-Bold="true" Font-Size="Medium"></asp:Label><br /><br />
        <asp:Literal ID="ltCampaignDesc" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
