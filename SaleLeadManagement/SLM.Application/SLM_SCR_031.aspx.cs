using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SLM.Application.Utilities;
using SLM.Biz;
using SLM.Resource.Data;
using log4net;
using SLM.Resource;

namespace SLM.Application
{
    public partial class SLM_SCR_031 : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SLM_SCR_031));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ScreenPrivilegeData priData = RoleBiz.GetScreenPrivilege(HttpContext.Current.User.Identity.Name, "SLM_SCR_031");
                    if (priData == null || priData.IsView != 1)
                    {
                        AppUtil.ClientAlertAndRedirect(Page, "คุณไม่มีสิทธิ์เข้าใช้หน้าจอนี้", "SLM_SCR_003.aspx");
                        return;
                    }

                    ((Label)Page.Master.FindControl("lblTopic")).Text = "จัดการข้อมูลเลขรับแจ้ง";
//                    cmbProductSearch.DataSource = ProductBiz.GetProductList();
                    cmbProductSearch.DataSource = ProductBiz.GetProductListNew();
                    cmbProductSearch.DataBind();
                    cmbProductSearch.Items.Insert(0, new ListItem("", "-1"));
                    cmbInsNameSearch.DataSource = ObtScr002Biz.GetInsCom();
                    cmbInsNameSearch.DataBind();
                    cmbInsNameSearch.Items.Insert(0, new ListItem() { Value = "-1", Text = "ทั้งหมด" });

                    AppUtil.SetNumbericCharacter(textStartReceiveNoNumber);
                    AppUtil.SetNumbericCharacter(textEndReceiveNoNumber);
                    AppUtil.SetNumbericCharacter(textTotalReceiveNoNumber);
                    textStartReceiveNoNumber.Attributes.Add("onblur", "CalcTotalReceiveNo();");
                    textEndReceiveNoNumber.Attributes.Add("onblur", "CalcTotalReceiveNo();");
                    Page.Form.DefaultButton = btnSearch.UniqueID;
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
                decimal? insComId = null;
                decimal idTmp = 0;
                if (decimal.TryParse(cmbInsNameSearch.SelectedValue, out idTmp))
                {
                    if (idTmp > 0)
                        insComId = idTmp;
                }

                List<ReceiveNoData> result = ObtScr002Biz.GetSearchData(cmbProductSearch.SelectedValue, insComId, false);
                // select only first item of result
                result = result.GroupBy(r => new { r.InsComId, r.CodeName })
                    .Select(g => g.OrderByDescending(r => r.Lot).FirstOrDefault())
                    .ToList();

                BindGridview(pcTop, gvResult, result.ToArray(), 0);
                pcTop.Visible = true;
                gvResult.Visible = true;
                upResult.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);                
            }
        }


        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, GridView gv, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gv);
            pageControl.Update(items, pageIndex, 10);
            upResult.Update();
        }
        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                List<ReceiveNoData> result = ObtScr002Biz.GetSearchData(cmbProductSearch.SelectedValue, null, false);
                // select only first item of result
                result = result.GroupBy(r => new { r.InsComId, r.CodeName })
                    .Select(g => g.OrderByDescending(r => r.Lot).FirstOrDefault())
                    .ToList();

                if (result != null && result.Count > 0)
                {
                    BindGridview(pcTop, gvResult, result.ToArray(), pcTop.SelectedPageIndex);
                }
                else
                    BindGridview(pcTop, gvResult, null, 0);

                upResult.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
        protected void pcReceiveNo_PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                string productid = hiddenProductId.Value;
                decimal insComId = 0;
                if (decimal.TryParse(hiddenInsComId.Value, out insComId))
                {
                    var result = ObtScr002Biz.GetSearchData(productid, insComId, true);
                    textInsName.Text = result[0].InsComName;
                    hiddenInsComId.Value = insComId.ToString();
                    textProductName.Text = result[0].ProductName;
                    hiddenProductId.Value = productid;
                    result = result.Where(r => r.ReceiveNoId != -1).OrderBy(r => r.IsDeleted == true ? 1 : 0).ThenByDescending(r => r.UpdatedDate).ToList();

                    BindGridview(pcReceiveNo, gvReceiveNoList, result.ToArray(), pcReceiveNo.SelectedPageIndex);
                    upnPopupEdit.Update();
                    mpePopupEdit.Show();
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            // mpePopupReceiveNo.Show();
        }

        protected void imbEdit_Click(object sender, EventArgs e)
        {
            try
            {
                string[] args = ((ImageButton)sender).CommandArgument.Split(',');
                if (args.Length < 2)
                    return;

                string productid = args[1];
                decimal insComId = 0;
                if (decimal.TryParse(args[0], out insComId))
                {
                    var result = ObtScr002Biz.GetSearchData(productid, insComId, true);
                    textInsName.Text = result[0].InsComName;
                    hiddenInsComId.Value = insComId.ToString();
                    textProductName.Text = result[0].ProductName;
                    hiddenProductId.Value = productid;
                    result = result.Where(r => r.ReceiveNoId != -1).OrderBy(r => r.IsDeleted == true ? 1 : 0).ThenByDescending(r => r.UpdatedDate).ToList();

                    BindGridview(pcReceiveNo, gvReceiveNoList, result.ToArray(), 0);
                    upnPopupEdit.Update();
                    mpePopupEdit.Show();
                }
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
                string message = "";
                ReceiveNoData data = new ReceiveNoData();
                data.CodeName = textCodeName.Text;
                data.InsComId = SLMUtil.SafeDecimal(hiddenInsComId.Value);
                data.IsDeleted = false;
                data.ProductId = hiddenProductId.Value;
                data.ReceiveNoCancel = 0;
                data.ReceiveNoEnd = SLMUtil.SafeDecimal(textEndReceiveNoNumber.Text);
                data.ReceiveNoStart = SLMUtil.SafeDecimal(textStartReceiveNoNumber.Text);
                data.ReceiveNoRemain = data.ReceiveNoEnd - data.ReceiveNoStart;
                data.ReceiveNoTotal = data.ReceiveNoRemain;
                data.ReceiveNoUsed = 0;
                data.UpdatedBy = HttpContext.Current.User.Identity.Name;
                data.UpdatedDate = DateTime.Now;
                data.IsDeleted = false;
                if (ObtScr002Biz.SaveReceiveData(data, out message))
                {
                    // rebind gridview
                    var result = ObtScr002Biz.GetSearchData(data.ProductId, data.InsComId, true);
                    BindGridview(pcReceiveNo, gvReceiveNoList, result.ToArray(), 0);

                    // hide add panel & clear data
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "hidePanel", "CancelAdd();", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "hidePanel", "AddReceiveNo();", true);
                }
                lblError.Text = message;
                mpePopupEdit.Show();
                mpePopupError.Show();
                upError.Update();
                //upnPopupEdit.Update();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void radioActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string alertMessage = "";
                bool inuse = ObtScr002Biz.HasLotInUse(hiddenProductId.Value, SLMUtil.SafeDecimal(hiddenInsComId.Value));
                if (inuse)
                {
                    alertMessage = "มี Lot ที่ใช้งานอยู่แล้ว ไม่สามารถเปลี่ยนสถานะได้";
                    trConfirmOK.Visible = true;
                    trComfirmYesNo.Visible = false;
                }
                else
                {
                    alertMessage = "ต้องการเปลี่ยนสถานะใช่หรือไม่?";
                    trConfirmOK.Visible = false;
                    trComfirmYesNo.Visible = true;
                }

                GridViewRow row = ((RadioButton)sender).Parent.Parent as GridViewRow;
                txtEditRowIndex.Text = row.RowIndex.ToString();
                lblAlertMessage.Text = alertMessage;
                upConfirm.Update();
                mpePopupConfirm.Show();
                mpePopupEdit.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void radioDeactive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = ((RadioButton)sender).Parent.Parent as GridViewRow;
                txtEditRowIndex.Text = row.RowIndex.ToString();
                lblAlertMessage.Text = "ต้องการเปลี่ยนสถานะใช่หรือไม่?";
                trConfirmOK.Visible = false;
                trComfirmYesNo.Visible = true;
                upConfirm.Update();
                mpePopupConfirm.Show();
                mpePopupEdit.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnAlertOK_Click(object sender, EventArgs e)
        {
            try
            {
                RollbackRadioButton(Convert.ToInt32(txtEditRowIndex.Text.Trim()));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnConfirmYes_Click(object sender, EventArgs e)
        {
            try
            {
                int gridviewRowIndex = Convert.ToInt32(txtEditRowIndex.Text.Trim());
                RadioButton radioActive = gvReceiveNoList.Rows[gridviewRowIndex].FindControl("radioActive") as RadioButton;
                RadioButton radioDeactive = gvReceiveNoList.Rows[gridviewRowIndex].FindControl("radioDeactive") as RadioButton;
                HiddenField hiddenReceiveNoId = gvReceiveNoList.Rows[gridviewRowIndex].FindControl("hiddenReceiveNoId") as HiddenField;

                if (string.IsNullOrEmpty(hiddenReceiveNoId.Value))
                {
                    mpePopupConfirm.Hide();
                    RollbackRadioButton(Convert.ToInt32(txtEditRowIndex.Text.Trim()));
                    AppUtil.ClientAlert(Page, "ไม่พบข้อมูลเลขที่รับแจ้งในระบบ");
                }
                else
                {
                    ObtScr002Biz.UpdateDeleleFlag(decimal.Parse(hiddenReceiveNoId.Value), radioDeactive.Checked, HttpContext.Current.User.Identity.Name);
                    mpePopupConfirm.Hide();
                    mpePopupEdit.Show();
                }
            }
            catch (Exception ex)
            {
                mpePopupConfirm.Hide();
                RollbackRadioButton(Convert.ToInt32(txtEditRowIndex.Text.Trim()));

                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnConfirmNo_Click(object sender, EventArgs e)
        {
            try
            {
                RollbackRadioButton(Convert.ToInt32(txtEditRowIndex.Text.Trim()));
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        private void RollbackRadioButton(int gridviewRowIndex)
        {
            try
            {
                RadioButton radioActive = gvReceiveNoList.Rows[gridviewRowIndex].FindControl("radioActive") as RadioButton;
                RadioButton radioDeactive = gvReceiveNoList.Rows[gridviewRowIndex].FindControl("radioDeactive") as RadioButton;

                if (radioActive.Checked)
                {
                    radioActive.Checked = false;
                    radioDeactive.Checked = true;
                }
                else if (radioDeactive.Checked)
                {
                    radioActive.Checked = true;
                    radioDeactive.Checked = false;
                }

                mpePopupConfirm.Hide();

                upnPopupEdit.Update();
                mpePopupEdit.Show();
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopupEdit.Hide();
            btnSearch_Click(null, null);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "hidePanel", "CancelAdd();", true);
        }

        protected void btnCloseError_Click(object sender, EventArgs e)
        {
            mpePopupError.Hide();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            cmbInsNameSearch.SelectedIndex = 0;
            cmbProductSearch.SelectedIndex = 0;
            pcTop.Visible = false;
            gvResult.Visible = false;
            upResult.Update();
        }

        #region Backup 10/02/2017

        //protected void radioActive_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        RadioButton radio = (RadioButton)sender;
        //        decimal receiveNoId;
        //        decimal.TryParse(radio.GroupName.Replace("rb", ""), out receiveNoId);
        //        string error = ObtScr002Biz.UpdateDeleleFlag(receiveNoId, false, HttpContext.Current.User.Identity.Name);
        //        if (error != "")
        //        {
        //            lblError.Text = error;
        //            mpePopupError.Show();
        //            upError.Update();
        //        }
        //        mpePopupEdit.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void radioDeactive_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        RadioButton radio = (RadioButton)sender;
        //        decimal receiveNoId;
        //        decimal.TryParse(radio.GroupName.Replace("rb", ""), out receiveNoId);
        //        string error = ObtScr002Biz.UpdateDeleleFlag(receiveNoId, true, HttpContext.Current.User.Identity.Name);

        //        if (error != "")
        //        {
        //            lblError.Text = error;
        //            mpePopupError.Show();
        //        }
        //        mpePopupEdit.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        _log.Error(message);
        //        AppUtil.ClientAlert(Page, message);
        //    }
        //}

        //protected void radioActive_DataBinding(object sender, EventArgs e)
        //{
        //    ((RadioButton)sender).Attributes.Add("OnClick", " if(!canChangeActiveFlag()) return false; ");
        //}

        #endregion

    }
}