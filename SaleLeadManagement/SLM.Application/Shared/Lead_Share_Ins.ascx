<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Lead_Share_Ins.ascx.cs" Inherits="SLM.Application.Shared.Lead_Share_Ins" %>
<%@ Register src="TextDateMask.ascx" tagname="TextDateMask" tagprefix="uc1" %>
            <asp:HiddenField runat="server" ID="hdfID" />

<asp:UpdatePanel runat="server" ID="updMain" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="2" cellspacing="0" border="0">

            <tr>
                <td colspan="5">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/hGeneral.gif" />
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">Contract No<span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtContractNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" ></asp:TextBox>
                    <asp:Label ID="vtxtContract" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3">
                    <asp:CheckBox runat="server" ID="chkOldCustomer" Text="เคยเป็นลูกค้าของธนาคาร"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td class="style2">ประเภทลูกค้า</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCardType" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbCardType_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="กรุณาระบุ"></asp:ListItem>
                        <asp:ListItem Value="1" Text="บุคคลธรรมดา"></asp:ListItem>
                        <asp:ListItem Value="2" Text="นิติบุคคล"></asp:ListItem>
                        <asp:ListItem Value="3" Text="บุคคลต่างชาติ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="vtxtCardType" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">เลขที่บัตร
                    <asp:Label ID="lblCitizenId" runat="server" ForeColor="Red"></asp:Label></td>
                <td class="style3">
                    <asp:TextBox ID="txtCitizenId" runat="server" CssClass="Textbox" Width="250px" MaxLength="100" Enabled="false"></asp:TextBox>
                    <asp:Label ID="vtxtCitizenId" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">ประเทศ<asp:Label ID="lblCountryId" runat="server" ForeColor="Red"></asp:Label></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCountry" runat="server" Width="253px" CssClass="Dropdownlist" >
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="vcmbCountry" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
            </tr>
            <tr>
                <td class="style2">วัน/เดือน/ปี เกิด</td>
                <td class="style3">
                    <uc1:TextDateMask ID="tdBirthdate" runat="server" />
                </td>
                <td class="style1"></td>
                <td class="style2">สถานภาพ</td>
                <td class="style3">
                    <asp:RadioButtonList runat="server" ID="rdoMarital" RepeatDirection="Horizontal">
                        <asp:ListItem Value="1">โสด</asp:ListItem>
                        <asp:ListItem Value="2">สมรส</asp:ListItem>
                        <asp:ListItem Value="3">หย่าร้าง</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">อาชีพ</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbOccupation" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">ประเภทรถ<span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbCarType" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbCarType_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:Label ID="vtxtCartype" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">ยี่ห้อรถ<span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbBrand" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbBrand_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="กรุณาระบุ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="vtxtBrand" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">รุ่น<span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbModel" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbModel_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="กรุณาระบุ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="vtxtModel" runat="server" CssClass="style4"></asp:Label>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">ปีรถ<span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbYearGroup" runat="server" CssClass="Dropdownlist" Width="253px" AutoPostBack="True" OnSelectedIndexChanged="cmbYearGroup_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="กรุณาระบุ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="vtxtYearGroup" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">รุ่นย่อยรถ<span class="style4"></span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbSubModel" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbSubModel_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="กรุณาระบุ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="vtxtSubModel" runat="server" CssClass="style4"></asp:Label>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">ซีซีรถ<span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtCarCC" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"  ></asp:TextBox>
                    <asp:Label ID="vtxtCC" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">ทะเบียนรถ<span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtCarLicense" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
                    <asp:Label ID="vtxtLicenseNo" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">จังหวัดที่จดทะเบียน<span class="style4">*</span></td>
                <td class="style3">
                    <asp:DropDownList ID="cmbProvinceRegis" runat="server" Width="253px" CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbProvinceRegis_SelectedIndexChanged" >
                    </asp:DropDownList>
                    <asp:Label ID="vcmbProvinceRegis" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2"></td>
                <td class="style3">
                </td>
            </tr>
            <tr style="vertical-align:top;">
                <td class="style2">เลขตัวถัง<span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtCarChassie" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
                    <asp:Label ID="vtxtChassis" runat="server" CssClass="style4"></asp:Label>
                </td>
                <td class="style1"></td>
                <td class="style2">เลขเครื่อง<span class="style4">*</span></td>
                <td class="style3">
                    <asp:TextBox ID="txtCarEngine" runat="server" CssClass="Textbox" Width="250px" ></asp:TextBox>
                    <asp:Label ID="vtxtEngineNo" runat="server" CssClass="style4"></asp:Label>
                </td>
            </tr>
            <tr runat="server" id="trSelectAddress">
                <td class="style2">รูปแบบที่อยู่</td>
                <td class="style3">
                    <asp:DropDownList ID="cmbAddrType" runat="server" CssClass="Dropdownlist" Width="250px"
                        OnSelectedIndexChanged="cmbAddType_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="a1" Text="ข้อมูลที่อยู่ที่สามารถติดต่อได้"></asp:ListItem>
                        <asp:ListItem Value="a2" Text="ข้อมูลที่อยู่ตามทะเบียนบ้าน"></asp:ListItem>
                        <asp:ListItem Value="a3" Text="ข้อมูลที่อยู่ในการจัดส่งเอกสาร" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="style1"></td>
                <td class="style2">&nbsp;</td>
                <td class="style3">&nbsp;</td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
&nbsp;
<asp:UpdatePanel runat="server" ID="updAddress" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlAddressContract" runat="server" Visible="false">
            <asp:HiddenField runat="server" ID="hdfAddrID" />
            <table>
                <tr>
                    <td colspan="5">
                        <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/hAddressContract.gif" />
                    </td>
                </tr>
                <tr>
                    <td class="style2">เลขที่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtAddressNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">หมู่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtMoo" runat="server" CssClass="Textbox" Width="250px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ชื่ออาคาร/หมู่บ้าน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtBuildingName" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ชั้น</td>
                    <td class="style3">
                        <asp:TextBox ID="txtFloor" runat="server" CssClass="Textbox" Width="250px" MaxLength="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ซอย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtSoi" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ถนน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtStreet" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">จังหวัด</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbProvince" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbProvince_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เขต/อำเภอ</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbAmphur" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbAmphur_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="style2">แขวง/ตำบล</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbTambol" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">รหัสไปรษณีย์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtPostalCode" runat="server" CssClass="Textbox" Width="250px" MaxLength="5"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlAddressFact" runat="server" Visible="false">
            <asp:HiddenField runat="server" ID="hdfOAddrID" />
            <table>
                <tr>
                    <td colspan="5">
                        <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/hAddressOrg.jpg" />
                        &nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="chkCopyAddressContract1" runat="server" Text="คัดลอกที่อยู่ที่สามารถติดต่อได้" AutoPostBack="True" OnCheckedChanged="chkCopyAddressContract1_CheckedChanged" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="height: 10px"></td>
                </tr>

                <tr>
                    <td class="style2">เลขที่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOAddressNo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">หมู่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOMoo" runat="server" CssClass="Textbox" Width="250px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ชื่ออาคาร/หมู่บ้าน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOBuildingname" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ชั้น</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOFloor" runat="server" CssClass="Textbox" Width="250px" MaxLength="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ซอย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOSoi" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ถนน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOStreet" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">จังหวัด</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbOProvince" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbOProvince_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เขต/อำเภอ</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbOAmphur" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbOAmphur_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="style2">แขวง/ตำบล</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbOTambol" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">รหัสไปรษณีย์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtOPostCode" runat="server" CssClass="Textbox" Width="250px" MaxLength="5"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlAddressSendDoc" runat="server" Visible="true">
            <asp:HiddenField runat="server" ID="hdfDAddrID" />
            <table>
                <tr>
                    <td colspan="5">
                        <asp:Image ID="Image7" runat="server" ImageUrl="~/Images/hAddressSendDoc.gif" />
                        &nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="chkCopyAddressContact2" runat="server" Text="คัดลอกที่อยู่ที่สามารถติดต่อได้" AutoPostBack="true" OnCheckedChanged="chkCopyAddressContact2_CheckedChanged" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="height: 10px"></td>
                </tr>
                <tr>
                    <td class="style2">
                        <asp:CheckBox ID="chkSendToBranch" runat="server" Text="ส่งเอกสารไปที่สาขา" OnCheckedChanged="chkSendToBranch_CheckedChanged" AutoPostBack="true" /></td>
                    <td colspan="3">
                        <asp:DropDownList ID="cmbDocBranch" runat="server" CssClass="Dropdownlist" Width="200px" Visible="False" OnSelectedIndexChanged="cmbDocBranch_SelectedIndexChanged" AutoPostBack="true"><asp:ListItem Value=""></asp:ListItem></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="style2">เลขที่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDAddressno" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">หมู่</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDMoo" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ชื่ออาคาร/หมู่บ้าน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDBuilding" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ชั้น</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDFloor" runat="server" CssClass="Textbox" Width="250px" MaxLength="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">ซอย</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDSoi" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">ถนน</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDStreet" runat="server" CssClass="Textbox" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style2">จังหวัด</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbDProvince" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbDProvince_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">เขต/อำเภอ</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbDAmphur" runat="server" Width="253px"
                            CssClass="Dropdownlist" AutoPostBack="True" OnSelectedIndexChanged="cmbDAmphur_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="style2">แขวง/ตำบล</td>
                    <td class="style3">
                        <asp:DropDownList ID="cmbDTambol" runat="server" Width="253px" CssClass="Dropdownlist"></asp:DropDownList>
                    </td>
                    <td class="style1"></td>
                    <td class="style2">รหัสไปรษณีย์</td>
                    <td class="style3">
                        <asp:TextBox ID="txtDPostCode" runat="server" CssClass="Textbox" Width="250px" MaxLength="5"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" style="height: 15px"></td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:HiddenField runat="server" ID="hdfPolicyDiscountAmt" />
<asp:HiddenField runat="server" ID="hdfPolicyGrossVat" />
<asp:HiddenField runat="server" ID="hdfPolicyGrossStamp" />
<asp:HiddenField runat="server" ID="hdfPolicyGrossPremium" />
<asp:HiddenField runat="server" ID="hdfPolicyGrossPremiumTotal" />
<asp:HiddenField runat="server" ID="hdfActGrossPremium" />
<asp:HiddenField runat="server" ID="hdfActNetPremium" />
<asp:HiddenField runat="server" ID="hdfActStamp" />
<asp:HiddenField runat="server" ID="hdfActVat" />

