<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/SaleLead.Master" AutoEventWireup="true" CodeBehind="SLM_SCR_003.aspx.cs" Inherits="SLM.Application.SLM_SCR_003" %>
<%@ Register src="Shared/TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
<%@ Register src="Shared/GridviewPageController.ascx" tagname="GridviewPageController" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ColInfo
        {
            font-weight:bold;
            width:180px;
        }
        .ColInput
        {
            width:250px;
        }
        .ColCheckBox
        {
            width:160px;
        }
        
        /* เรียกใช้จาก SlmScript.js */
        .AutoDropdownlist-toggle{
            position: absolute;
            margin-left: -1px;
            padding: 0;
            background-image: url(Images/hDropdownlist.png);
            background-repeat: no-repeat;
            z-index:20000 !important;
        }
        .ui-autocomplete  
        {
            height: 220px; width:400px; overflow-y: scroll; overflow-x: hidden;
            /* add padding to account for vertical scrollbar */
            padding-right: 5px;
            z-index:20000 !important;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function doToggle() {
            var pnAdvanceSearch = document.getElementById('<%=pnAdvanceSearch.ClientID%>');
            var lbAdvanceSearch = document.getElementById('<%=lbAdvanceSearch.ClientID%>');
            var txtAdvanceSearch = document.getElementById('<%=txtAdvanceSearch.ClientID%>');

            if (pnAdvanceSearch.style.display == '' || pnAdvanceSearch.style.display == 'none') {
                lbAdvanceSearch.innerHTML = "[-] <b>Advance Search</b>";
                pnAdvanceSearch.style.display = 'block';
                txtAdvanceSearch.value = "Y";
            }
            else {
                lbAdvanceSearch.innerHTML = "[+] <b>Advance Search</b>";
                pnAdvanceSearch.style.display = 'none';
                txtAdvanceSearch.value = "N";
            }
        }

        function callsaletool(ticketid) {
            var form = document.createElement("form");
            var input_ticketid = document.createElement("input");
            var input_username = document.createElement("input");

            form.action = '<%= System.Configuration.ConfigurationManager.AppSettings["SaleToolUrl"].ToString() %>';
            form.method = "post"
            form.setAttribute("target", "_blank");

            input_ticketid.name = "ticketid";
            input_ticketid.value = ticketid;
            form.appendChild(input_ticketid);

            input_username.name = "username";
            input_username.value = '<%= HttpContext.Current.User.Identity.Name %>';
            form.appendChild(input_username);

            document.body.appendChild(form);
            form.submit();

            document.body.removeChild(form);
        }

        function calladam(ticketid) {
            var form = document.createElement('form');
            var input_ticketid = document.createElement('input');
            var input_username = document.createElement('input');
            var input_product = document.createElement('input');
            var input_campaign = document.createElement('input');
            var input_name = document.createElement('input');
            var input_lastname = document.createElement('input');
            var input_license_plate = document.createElement('input');
            var input_state = document.createElement('input');
            var input_mobile = document.createElement('input');

            form.action = '<%= System.Configuration.ConfigurationManager.AppSettings["AdamlUrl"].ToString() %>';
            form.method = 'post'
            form.setAttribute('target', '_blank');

            input_ticketid.name = 'ticket_id';
            input_ticketid.value = '';
            form.appendChild(input_ticketid);

            input_username.name = 'username';
            input_username.value = '<%= HttpContext.Current.User.Identity.Name %>';
            form.appendChild(input_username);

            input_product.name = 'product';
            input_product.value = "";
            form.appendChild(input_product);

            input_campaign.name = 'campaign';
            input_campaign.value = "";
            form.appendChild(input_campaign);

            input_name.name = 'name';
            input_name.value = '';
            form.appendChild(input_name);

            input_lastname.name = 'lastname';
            input_lastname.value = '';
            form.appendChild(input_lastname);

            input_license_plate.name = 'license_plate';
            input_license_plate.value = '';
            form.appendChild(input_license_plate);

            input_state.name = 'state';
            input_state.value = '';
            form.appendChild(input_state);

            input_mobile.name = 'mobile';
            input_mobile.value = '';
            form.appendChild(input_mobile);

            document.body.appendChild(form);
            form.submit();

            document.body.removeChild(form);
        }

        function TestPost(ticketid) {
            var form = document.createElement("form");
            var input_ticketid = document.createElement("input");

            form.action = 'SLM_SCR_004.aspx?ReturnUrl=' + '<%= Server.UrlEncode(Request.Url.AbsoluteUri) %>';
            form.method = "post"

            input_ticketid.name = "ticketid";
            input_ticketid.value = ticketid;
            form.appendChild(input_ticketid);

            document.body.appendChild(form);
            form.submit();

            document.body.removeChild(form);
        }
    </script>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        //On Page Load
        //Set Autocomplete dropdownlist
        Sys.Application.add_load(function () {

            $("#<%= cmbCampaign.ClientID %>").combobox({
                select: function (event, ui) {
                    DoPostBack(this);
                },
                notfound: function (event) {
                    <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
                },
                cleared: function () {
                    <%=Page.ClientScript.GetPostBackEventReference(cmbCampaign, "-1")%>
                }
            });

            $("#<%= cmbOwnerBranchSearch.ClientID %>").combobox({
                select: function (event, ui) {
                    DoPostBack(this);
                },
                notfound: function (event) {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbOwnerBranchSearch, "-1") %>
                },
                cleared: function () {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbOwnerBranchSearch, "-1") %>
                }
            });

            $("#<%= cmbOwnerLeadSearch.ClientID %>").combobox();

            $("#<%= cmbDelegateBranchSearch.ClientID %>").combobox({
                select: function (event, ui) {
                    DoPostBack(this);
                },
                notfound: function (event) {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbDelegateBranchSearch, "-1") %>
                },
                cleared: function () {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbDelegateBranchSearch, "-1") %>
                }
            });

            $("#<%= cmbDelegateLeadSearch.ClientID %>").combobox();

             $("#<%= cmbCreatebyBranchSearch.ClientID %>").combobox({
                select: function (event, ui) {
                    DoPostBack(this);
                },
                notfound: function (event) {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbCreatebyBranchSearch, "-1") %>
                },
                cleared: function () {
                    <%= Page.ClientScript.GetPostBackEventReference(cmbCreatebyBranchSearch, "-1") %>
                }
            });

            $("#<%= cmbCreatebySearch.ClientID %>").combobox();
        });
    </script>
    <br />
    <asp:Image ID="imgSearch" runat="server" ImageUrl="~/Images/hSearch.gif" />
    <asp:UpdatePanel ID="upSearch" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" border="0">
                <tr><td colspan="4" style="height:2px;"></td></tr>
                <tr>
                    <td class="ColInfo">
                        Ticket ID
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtTicketID" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                        <asp:TextBox ID="txtEmpCode" runat="server" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="txtStaffTypeId" runat="server" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="txtStaffTypeDesc" runat="server" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="txtStaffId" runat="server" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="txtStaffBranchCode" runat="server" Visible="false"></asp:TextBox>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ชื่อ
                    </td>
                    <td class="ColInput">
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                    <td class="ColInfo">
                        นามสกุล
                    </td>
                    <td>
                        <asp:TextBox ID="txtLastname" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ประเภทบุคคล
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbCardType" runat="server" Width="203px" CssClass="Dropdownlist" >
                        </asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                        เลขที่บัตร
                    </td>
                    <td>
                        <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ColInfo">
                        ช่องทาง
                    </td>
                    <td class="ColInput">
                        <asp:DropDownList ID="cmbChannel" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                    <td class="ColInfo">
                        แคมเปญ
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbCampaign" runat="server" Width="203px"
                        CssClass="Dropdownlist" ></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="height:10px; vertical-align:bottom;">
                    </td>
                    <td colspan="3"></td>
                </tr>
            </table>
            <asp:LinkButton ID="lbAdvanceSearch" runat="server" ForeColor="Green" OnClientClick="DisplayProcessing()" 
                Text="[+] <b>Advance Search</b>"  onclick="lbAdvanceSearch_Click"></asp:LinkButton>
            <asp:TextBox ID="txtAdvanceSearch" runat="server" Text="N" Visible="false" ></asp:TextBox>
            <asp:Panel ID="pnAdvanceSearch" runat="server" style="display:none;" >
                <table cellpadding="2" cellspacing="0" border="0">
                    <tr><td colspan="4" style="height:8px;"></td></tr>
                    <tr>
                        <td class="ColInfo">
                            เลขที่สัญญาที่เคยมีกับธนาคาร
                        </td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtContractNoRefer" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo">
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">
                            วันทีสร้าง Lead
                        </td>
                        <td class="ColInput">
                            <uc1:TextDateMask ID="tdmCreateDate" runat="server" Width="182px" />
                        </td>
                        <td class="ColInfo">
                            วันที่ได้รับมอบหมายล่าสุด
                        </td>
                        <td>
                            <uc1:TextDateMask ID="tdmAssignDate" runat="server" Width="182px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">
                            Owner Branch
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbOwnerBranchSearch" runat="server" Width="203px" 
                                CssClass="Dropdownlist"
                                onselectedindexchanged="cmbOwnerBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td class="ColInfo">
                                Owner Lead
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbOwnerLeadSearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">
                            Delegate Branch
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbDelegateBranchSearch" runat="server" Width="203px" 
                                CssClass="Dropdownlist"
                                onselectedindexchanged="cmbDelegateBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td class="ColInfo">
                                Delegate Lead
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbDelegateLeadSearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">
                            สาขาผู้สร้าง Lead
                        </td>
                        <td class="ColInput">
                            <asp:DropDownList ID="cmbCreatebyBranchSearch" runat="server" Width="203px" 
                                CssClass="Dropdownlist" AutoPostBack="true"
                                onselectedindexchanged="cmbCreatebyBranchSearch_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td class="ColInfo">
                                ผู้สร้าง Lead
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbCreatebySearch" runat="server" Width="203px" CssClass="Dropdownlist"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ColInfo">สถานะย่อย</td>
                        <td class="ColInput">
                            <asp:TextBox ID="txtSubStatus" runat="server" CssClass="Textbox" Width="200px" ></asp:TextBox>
                        </td>
                        <td class="ColInfo">วันที่เปลี่ยนแปลงสถานะ</td>
                        <td class="ColInput">
                            <uc1:TextDateMask ID="tdmChangeStatusDate" runat="server" Width="182px" />
                        </td>
                    </tr>
                </table><br />
                <table cellpadding="3" cellspacing="0" border="0">
                    <tr>
                        <td valign="top" class="ColInfo">
                                สถานะของ Lead
                        </td>
                        <td colspan="5">
                            &nbsp;<asp:CheckBox ID="cbOptionAll" runat="server" Text="ทั้งหมด" AutoPostBack="true" oncheckedchanged="cbOptionAll_CheckedChanged" />
                            <asp:CheckBoxList ID="cbOptionList" runat="server" RepeatLayout="Table" AutoPostBack="true" 
                                RepeatDirection="Horizontal" RepeatColumns="5" 
                                onselectedindexchanged="cbOptionList_SelectedIndexChanged"></asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" style="height:15px;">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel> 
    

    <asp:UpdatePanel ID="upButton" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td colspan="6" style="height:3px"></td>
                </tr>
                <tr>
                    <td class="ColInfo">
                    </td>
                    <td colspan="5">
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
            <asp:Button ID="btnAddLead" runat="server" Text="เพิ่ม Lead" Width="120px" 
                CssClass="Button" Height="23px" onclick="btnAddLead_Click"  />
                <br /><br />
                <uc2:GridviewPageController ID="pcTop" runat="server" OnPageChange="PageSearchChange" Width="2755px" />
                <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" DataKeyNames="TicketId"
                    GridLines="Horizontal" BorderWidth="0px" EnableModelValidation="True"   
                    EmptyDataText="<span style='color:Red;'>ไม่พบข้อมูล</span>" 
                    AllowSorting="true" onsorting="gvResult_Sorting" Width="2755px" 
                    onrowdatabound="gvResult_RowDataBound" ondatabound="gvResult_DataBound" >
                    <Columns>
                    <asp:TemplateField HeaderText="SLA">
                        <ItemTemplate>
                            <asp:image ID="imgSla" runat="server" ImageUrl="~/Images/invalid.gif" Visible='<%# Eval("Counting") != null ? (Convert.ToInt32(Eval("Counting")) > 0 ? true : false) : false %>' />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            &nbsp;<asp:ImageButton ID="imbView" runat="server" ImageUrl="~/Images/view.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbView_Click" OnClientClick="DisplayProcessing()" ToolTip="ดูรายละเอียดข้อมูลผู้มุ่งหวัง"  />
                            <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="~/Images/edit.gif" CommandArgument='<%# Container.DisplayIndex %>' OnClick="imbEdit_Click" OnClientClick="DisplayProcessing()" ToolTip="แก้ไขข้อมูลผู้มุ่งหวัง"  />
                        </ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="Notice">
                        <HeaderTemplate>
                            <asp:LinkButton ID="lbNotice" runat="server" Text="Notice" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="Notice"></asp:LinkButton>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:image ID="imgNotify" runat="server" ImageUrl="~/Images/exclamation.jpg" Visible='<%# Eval("NoteFlag") != null ? (Eval("NoteFlag").ToString() == "1" ? true : false) : false %>' />
                        </ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cal">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbCal" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Calculator.png" ToolTip="Calculator" />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Doc">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbDoc" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Document.png" ToolTip="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("TicketId") %>' OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Others">
                        <ItemTemplate>
                            <asp:ImageButton ID="imbOthers" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/Others.png" OnClick="lbAolSummaryReport_Click" CommandArgument='<%# Container.DisplayIndex %>' ToolTip="เรียกดูข้อมูลเพิ่มเติม" OnClientClick="DisplayProcessing()" />
                        </ItemTemplate>
                        <ItemStyle Width="30px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="30px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ticket ID">
                        <ItemTemplate>
                            <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("TicketId") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ประเภทบุคคล">
                        <ItemTemplate>
                            <asp:Label ID="lblCardTypeDesc" runat="server" Text='<%# Eval("CardTypeDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CitizenId" HeaderText="เลขที่บัตร"  >
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="ชื่อ">
                        <ItemTemplate>
                            <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="นามสกุล">
                        <ItemTemplate>
                            <asp:Label ID="lblLastname" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle Width="100px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="StatusDesc">
                        <HeaderTemplate>
                            <asp:LinkButton ID="lbHeadStatusDesc" runat="server" Text="สถานะของ Lead" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="StatusDesc"></asp:LinkButton>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Eval("StatusDesc") %>' ></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ExternalSubStatusDesc" HeaderText="สถานะย่อยของ Lead">
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="วันที่เปลี่ยนแปลงสถานะ">
                        <ItemTemplate>
                            <%# Eval("slmStatusDate") != null ? Convert.ToDateTime(Eval("slmStatusDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("slmStatusDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("slmStatusDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="แจ้งเตือนครั้งที่">
                        <ItemTemplate>
                            <asp:Label ID="lblCounting" runat="server" Text='<%# Eval("Counting") != null ? Convert.ToDecimal(Eval("Counting")).ToString("#,##0") : "0" %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="70px" HorizontalAlign="Center" VerticalAlign="Top" />
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันเวลา NextSLA">
                        <ItemTemplate>
                            <%# Eval("NextSLA") != null ? Convert.ToDateTime(Eval("NextSLA")).ToString("dd/MM/") + Convert.ToDateTime(Eval("NextSLA")).Year.ToString() + " " + Convert.ToDateTime(Eval("NextSLA")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="CampaignName">
                        <HeaderTemplate>
                            <asp:LinkButton ID="lbHeadCampaignName" runat="server" Text="แคมเปญ" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="CampaignName"></asp:LinkButton>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignName" runat="server" Text='<%# Eval("CampaignName") %>' ></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="110px" HorizontalAlign="Center"/>
                        <ItemStyle Width="110px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ChannelDesc" HeaderText="ช่องทาง">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"/>
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OwnerName" HeaderText="Owner Lead">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelegateName" HeaderText="Delegate Lead">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreateName" HeaderText="ผู้สร้าง Lead">
                        <HeaderStyle Width="150px" HorizontalAlign="Center"/>
                        <ItemStyle Width="150px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="วันที่สร้าง Lead">
                        <ItemTemplate>
                            <%# Eval("CreatedDate") != null ? Convert.ToDateTime(Eval("CreatedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("CreatedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("CreatedDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="AssignedDate">
                        <HeaderTemplate>
                            <asp:LinkButton ID="lbAssignedDate" runat="server" Text="วันที่ได้รับมอบหมายล่าสุด" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="AssignedDate"></asp:LinkButton>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("AssignedDate") != null ? Convert.ToDateTime(Eval("AssignedDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("AssignedDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("AssignedDate")).ToString("HH:mm:ss") : ""%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Calculator">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbCalculator" runat="server" Text="Calculator" ></asp:LinkButton>
                            <asp:LinkButton ID="lbSaleTool" runat="server" Text="Calculator" Visible="false" OnClientClick='<%# string.Format("javascript:return callsaletool(\"{0}\")", Eval("TicketId")) %>' ></asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="85px" CssClass="Hidden" />
                        <ItemStyle Width="85px"  CssClass="Hidden"  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Document">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbDocument" runat="server" Text="แนบเอกสาร" OnClick="lbDocument_Click" CommandArgument='<%# Eval("TicketId") %>' OnClientClick="DisplayProcessing()" ></asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="85px" CssClass="Hidden"/>
                        <ItemStyle Width="85px" CssClass="Hidden"  />
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="DelegateDate">
                        <HeaderTemplate>
                            <asp:LinkButton ID="lbDelegateDate" runat="server" Text="วันที่ได้รับ<br/>Delegate" OnClientClick="DisplayProcessing();" CommandName="Sort" CommandArgument="DelegateDate"></asp:LinkButton>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("DelegateDate") != null ? Convert.ToDateTime(Eval("DelegateDate")).ToString("dd/MM/") + Convert.ToDateTime(Eval("DelegateDate")).Year.ToString() + " " + Convert.ToDateTime(Eval("DelegateDate")).ToString("HH:mm:ss") : "" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center"/>
                        <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                     <asp:BoundField DataField="DelegateBranchName" HeaderText="Delegate Branch">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                     <asp:BoundField DataField="BranchCreateBranchName" HeaderText="สาขาผู้สร้าง Lead">
                        <HeaderStyle Width="130px" HorizontalAlign="Center"  />
                        <ItemStyle Width="130px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContractNoRefer" HeaderText="เลขที่สัญญา<br/>ที่เคยมีกับธนาคาร" HtmlEncode="false">
                        <HeaderStyle Width="120px" HorizontalAlign="Center"  />
                        <ItemStyle Width="120px" HorizontalAlign="Left" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="ProductName">
                        <ItemTemplate>
                            <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="HasAdamUrl">
                        <ItemTemplate>
                            <asp:Label ID="lblHasAdamUrl" runat="server" Text='<%# Convert.ToBoolean(Eval("HasAdamUrl")) ? "Y" : "N" %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CampaignId">
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignId" runat="server" Text='<%# Eval("CampaignId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="LicenseNo">
                        <ItemTemplate>
                            <asp:Label ID="lblLicenseNo" runat="server" Text='<%# Eval("LicenseNo") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TelNo1">
                        <ItemTemplate>
                            <asp:Label ID="lblTelNo1" runat="server" Text='<%# Eval("TelNo1") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProvinceRegis">
                        <ItemTemplate>
                            <asp:Label ID="lblProvinceRegis" runat="server" Text='<%# Eval("ProvinceRegis") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CalculatorUrl">
                        <ItemTemplate>
                            <asp:Label ID="lblCalculatorUrl" runat="server" Text='<%# Eval("CalculatorUrl") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProductGroupId">
                        <ItemTemplate>
                            <asp:Label ID="lblProductGroupId" runat="server" Text='<%# Eval("ProductGroupId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ProductId">
                        <ItemTemplate>
                            <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="AppNo">
                        <ItemTemplate>
                            <asp:Label ID="lblAppNo" runat="server" Text='<%# Eval("AppNo") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IS COC">
                        <ItemTemplate>
                            <asp:Label ID="lblIsCOC" runat="server" Text='<%# Eval("ISCOC") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="slmStatusCode">
                        <ItemTemplate>
                            <asp:Label ID="lbslmStatusCode" runat="server" Text='<%# Eval("slmStatusCode") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="COCCurrentTeam">
                        <ItemTemplate>
                            <asp:Label ID="lblCOCCurrentTeam" runat="server" Text='<%# Eval("COCCurrentTeam") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="Hidden" />
                        <ControlStyle CssClass="Hidden" />
                        <HeaderStyle CssClass="Hidden" />
                        <FooterStyle CssClass="Hidden" />
                    </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="t_rowhead" />
                    <RowStyle CssClass="t_row" BorderStyle="Dashed"/>
                </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>  
</asp:Content>
