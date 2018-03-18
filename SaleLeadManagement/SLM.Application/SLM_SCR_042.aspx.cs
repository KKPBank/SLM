using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;

namespace SLM.Application
{
    public partial class SLM_SCR_042 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_042));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_042");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "ข้อมูลบริษัทประกันภัย";

                    Page.Form.DefaultButton = btnSearch.UniqueID;
                    InitialControl();
                    // DoSearchBranch(0);

                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var list = ObtScr019Biz.GetSearchInsuredData(cmbProductSearch.SelectedValue, txtCodeSearch.Text, txtNameSearch.Text, cmbTypeSearch.SelectedValue == "-1" ? "" : cmbTypeSearch.SelectedValue);
                BindGridview(pcTop, list.ToArray(), pcTop.SelectedPageIndex);
                txtCodeAdd.Enabled = true;
                // upResult.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void InitialControl()
        {
            AppUtil.SetNotThaiCharacter(txtNameEn);
            AppUtil.SetNotEnglishCharacter(txtNameTh);
            AppUtil.SetNumbericCharacter(txtTaxCode);
            AppUtil.SetNumbericCharacter(txtPostcode);
            cmbProductAdd.DataSource = ProductBiz.GetProductList();
            cmbProductAdd.DataBind();
            cmbProductSearch.DataSource = ProductBiz.GetProductList();
            cmbProductSearch.DataBind();
            cmbProvince.DataSource = ObtScr019Biz.GetProvinceData();
            cmbProvince.DataBind();
            cmbProvince.Items.Insert(0, new ListItem("", "-1"));
            cmbProvince_SelectedIndexChanged(null, null);
            rdActive.Checked = true;
            rdNoActive.Checked = false;
        }

        public void GetData()
        {
            List<Data005> list = new List<Data005>();
            Data005 data1 = new Data005();
            data1.INSCode = "12";
            data1.INSName = "บมจ.ประกันภัยไทยวิวัฒน์";
            data1.INSType = "คู่ค้า";
            data1.INS_SEQ = "1";
            data1.INS_IS_DELETE = "ใช้งาน";
            list.Add(data1);

            Data005 data2 = new Data005();
            data2.INSCode = "09";
            data2.INSName = "บจก.ไทยพาณิชยืสามัคคีประกันภัย";
            data2.INSType = "คู่ค้า";
            data2.INS_SEQ = "2";
            data2.INS_IS_DELETE = "ใช้งาน";
            list.Add(data2);

            Data005 data3 = new Data005();
            data3.INSCode = "B15";
            data3.INSName = "บมจ.วิริยะดำเนิน";
            data3.INSType = "ไม่เป็นคู่ค้า";
            data3.INS_SEQ = "4";
            data3.INS_IS_DELETE = "ใช้งาน";
            list.Add(data3);

            Data005 data4 = new Data005();
            data4.INSCode = "16";
            data4.INSName = "บจก.ลิเบอร์ตี้ประกันภัย";
            data4.INSType = "ไม่เป็นคู่ค้า";
            data4.INS_SEQ = "3";
            data4.INS_IS_DELETE = "ใช้งาน";
            list.Add(data4);

            Data005 data5 = new Data005();
            data5.INSCode = "B11";
            data5.INSName = "ไทยเศรษฐกิจ";
            data5.INSType = "คู่ค้า";
            data5.INS_SEQ = "5";
            data5.INS_IS_DELETE = "ใช้งาน";
            list.Add(data5);

            BindGridview(pcTop, list.ToArray(), 0);
        }

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvResult);
            pageControl.Update(items, pageIndex);

            pcTop.Visible = true;
            gvResult.Visible = true;
            upResult.Update();           
        }
        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                //var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                //DoSearchLeadData(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
                var list = ObtScr019Biz.GetSearchInsuredData(cmbProductSearch.SelectedValue, txtCodeSearch.Text, txtNameSearch.Text, cmbTypeSearch.SelectedValue == "-1" ? "" : cmbTypeSearch.SelectedValue);
                BindGridview(pcTop, list.ToArray(), pcTop.SelectedPageIndex);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbAmphur.DataSource = ObtScr019Biz.GetAmphurData(cmbProvince.SelectedValue);
                cmbAmphur.DataBind();
                cmbAmphur.Items.Insert(0, new ListItem("", "-1"));
                cmbAmphur_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void cmbAmphur_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbTambol.DataSource = ObtScr019Biz.GetTambolData(cmbProvince.SelectedValue, cmbAmphur.SelectedValue, false);
                cmbTambol.DataBind();
                cmbTambol.Items.Insert(0, new ListItem("", "-1"));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPopupDoImportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (ObtScr019Biz.IsDuplicateComCode(_InsuredCompany.CompanyId, _InsuredCompany.InsuredCode, _InsuredCompany.ProductId) && _InsuredCompany.CompanyId < 0)
                {
                    //AppUtil.ClientAlert(Page, "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลรหัสบริษัทประกันซ้ำกับในระบบ");
                    lblError.Text = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลรหัสบริษัทประกันซ้ำกับในระบบ";
                    mpePopupReceiveNo.Show();
                    mpePopupError.Show();
                    upError.Update();
                }
                else if (ObtScr019Biz.IsDuplicateComName(_InsuredCompany.CompanyId, _InsuredCompany.InsuredNameTh, _InsuredCompany.ProductId))
                {
                    // AppUtil.ClientAlert(Page, "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลชื่อบริษัทประกันซ้ำกับในระบบ");
                    lblError.Text = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลชื่อบริษัทประกันซ้ำกับในระบบ";
                    mpePopupReceiveNo.Show();
                    mpePopupError.Show();
                    upError.Update();
                }
                else if (ObtScr019Biz.IsDuplicateAbbName(_InsuredCompany.CompanyId, _InsuredCompany.InsuredAbbreviation))
                {
                    lblError.Text = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากชื่อย่อซ้ำกับในระบบ";
                    mpePopupReceiveNo.Show();
                    mpePopupError.Show();
                    upError.Update();
                }
                else
                {
                    ObtScr019Biz.SaveInsuredData(_InsuredCompany);
                    AppUtil.ClientAlert(Page, "บันทึกข้อมูลเรียบร้อย");
                    // ScriptManager.RegisterClientScriptBlock(Page, GetType(), "clearform", "clearForm();", true);
                    InitialControl();                    
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnPopupCancel_Click(object sender, EventArgs e)
        {
            InitialControl();
            btnSearch_Click(null, null);
        }

        private InsuredData _InsuredCompany
        {
            get
            {
                InsuredData ins = new InsuredData();
                ins.AddressNo = txtAddress.Text;
                ins.AmphurId = (cmbAmphur.SelectedItem == null ? -1 : int.Parse(cmbAmphur.SelectedValue));
                ins.AmphurName = (cmbAmphur.SelectedItem == null ? "" : cmbAmphur.SelectedItem.Text);
                ins.BuildingName = txtBuilding.Text;
                ins.CompanyId = decimal.Parse(hiddenCompId.Value);
                // ins.CreatedBy = 
                // ins.CreatedDate =
                ins.Floor = txtFloor.Text;
                ins.InsuredAbbreviation = txtAbbName.Text;
                ins.InsuredCode = txtCodeAdd.Text;
                ins.InsuredNameEng = txtNameEn.Text;
                ins.InsuredNameTh = txtNameTh.Text;
                ins.InsuredType = cmbTypeAdd.SelectedItem.Value;
                ins.InsusredTax = txtTaxCode.Text;
                ins.is_Deleted = rdActive.Checked ? false : true;
                ins.Moo = txtMoo.Text;
                ins.PostCode = txtPostcode.Text;
                ins.ProductId = cmbProductAdd.SelectedValue;
                ins.ProvinceId = int.Parse(cmbProvince.SelectedValue);
                ins.ProvinceName = cmbProvince.SelectedItem.Text;
                ins.Road = txtRoad.Text;
                ins.Soi = txtSoi.Text;
                ins.TambolId = int.Parse(cmbTambol.SelectedValue);
                ins.TambonName = cmbTambol.SelectedItem.Text;
                ins.Tel = txtPhone.Text;
                ins.TelContact = txtSms.Text;
                ins.UpdatedBy = HttpContext.Current.User.Identity.Name;
                ins.UpdatedDate = DateTime.Now;
                return ins;
            }
            set
            {
                txtAddress.Text = value.AddressNo;
                txtBuilding.Text = value.BuildingName;
                hiddenCompId.Value = value.CompanyId.ToString();
                // ins.CreatedBy = 
                // ins.CreatedDate =
                txtFloor.Text = value.Floor;
                txtAbbName.Text = value.InsuredAbbreviation;
                txtCodeAdd.Text = value.InsuredCode;
                txtNameEn.Text = value.InsuredNameEng;
                txtNameTh.Text = value.InsuredNameTh;
                cmbTypeAdd.SelectedValue = value.InsuredType;
                txtTaxCode.Text = value.InsusredTax;
                rdActive.Checked = !value.is_Deleted;
                rdNoActive.Checked = value.is_Deleted;
                txtMoo.Text = value.Moo;
                txtPostcode.Text = value.PostCode;
                cmbProductAdd.SelectedValue = value.ProductId;

                cmbProvince.SelectedValue = value.ProvinceId == null ? cmbProvince.Items[0].Value : value.ProvinceId.ToString();
                cmbProvince_SelectedIndexChanged(null, null);

                cmbAmphur.SelectedValue = value.AmphurId == null ? cmbAmphur.Items[0].Value : value.AmphurId.ToString();
                cmbAmphur_SelectedIndexChanged(null, null);

                cmbTambol.SelectedValue = value.TambolId == null ? cmbTambol.Items[0].Value : value.TambolId.ToString();
                txtRoad.Text = value.Road;
                txtSoi.Text = value.Soi;
                txtPhone.Text = value.Tel;
                txtSms.Text = value.TelContact;
            }
        }

        public class Data005
        {
            public string INSCode { get; set; }
            public string INSName { get; set; }
            public string INSType { get; set; }
            public string INS_IS_DELETE { get; set; }
            public string INS_SEQ { get; set; }
        }

        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            mpePopupReceiveNo.Show();
        }

        protected void imbEdit_Click(object sender, ImageClickEventArgs e)
        {
            decimal id;
            if (decimal.TryParse(((ImageButton)sender).CommandArgument, out id))
            {
                _InsuredCompany = ObtScr019Biz.GetEditInsuredData(id);
                txtCodeAdd.Enabled = false;
            }
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton ibImageButton = (ImageButton)e.Row.FindControl("imbEdit");
                ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(ibImageButton);
            }
        }

        protected void gvResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            mpePopupReceiveNo.Show();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            //gvResult.DataSource = null;
            //gvResult.DataBind();
            pcTop.Visible = false;
            gvResult.Visible = false;
            upResult.Update();
            //txtCodeSearch.Text = "";
            //txtNameSearch.Text = "";
            //cmbProductSearch.SelectedIndex = 0;
            //cmbTypeSearch.SelectedIndex = 0;
        }

        protected void btnCloseError_Click(object sender, EventArgs e)
        {
            mpePopupError.Hide(); 
        }

    }
}