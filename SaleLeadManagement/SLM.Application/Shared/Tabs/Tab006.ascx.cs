using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLM.Resource;
using SLM.Resource.Data;
using SLM.Biz;
using SLM.Application.Utilities;
using log4net;
using SLM.Application.CBSCustomerService;

namespace SLM.Application.Shared.Tabs
{
    public partial class Tab006 : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Tab006));

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DisplayContent(bool value)
        {
            upResult.Visible = value;
        }

        public void SetDefaultValue(string citizenId, string countryId, string cardTypeId)
        {
            txtCitizenId.Text = citizenId;
            txtCountryId.Text = countryId;
            txtCardTypeId.Text = cardTypeId;
        }

        #region "Call CBS Webservice"
        public void GetCBSExistingProductList(string citizenId, string cardTypeId, string countryId)
        {
            try
            {
                txtCitizenId.Text = citizenId;
                txtCardTypeId.Text = cardTypeId;
                txtCountryId.Text = countryId;
                DoSearchCBSExistingProductList(0, "", SortDirection.Ascending);
            }
            catch
            {
                throw;
            }
        }

        private void DoSearchCBSExistingProductList(int pageIndex, string sortExpression, SortDirection sortDirection)
        {
            try
            {
                IEnumerable<ExistingProductData> result = new List<ExistingProductData>(); //SlmScr006Biz.SearchExistingProduct(txtCitizenId.Text.Trim());
                string CIFCardType = !string.IsNullOrEmpty(txtCardTypeId.Text)?CardTypeBiz.GetCardTypeCIF(Convert.ToInt32(txtCardTypeId.Text)):"";
                string CountryCode = !string.IsNullOrEmpty(txtCountryId.Text)?CountryBiz.GetCountryCodeById(Convert.ToInt32(txtCountryId.Text)).CountryCode:"";

                if (!string.IsNullOrEmpty(txtCitizenId.Text) && !string.IsNullOrEmpty(CIFCardType) && !string.IsNullOrEmpty(CountryCode))
                {
                    result = InquiryCustomerSingleView(txtCitizenId.Text, CIFCardType, CountryCode);
                }
                else
                {
                    gvExistProduct.EmptyDataText = "<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>";
                }

                #region "Manage Sort Expression"

                if (sortExpression == "ProductGroup")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.ProductGroup).ToList();
                    else
                        result = result.OrderByDescending(p => p.ProductGroup).ToList();
                }
                else if (sortExpression == "ContactNo")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.ContactNo).ToList();
                    else
                        result = result.OrderByDescending(p => p.ContactNo).ToList();
                }
                else if (sortExpression == "StartDate")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.StartDate).ToList();
                    else
                        result = result.OrderByDescending(p => p.StartDate).ToList();
                }
                else if (sortExpression == "EndDate")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.EndDate).ToList();
                    else
                        result = result.OrderByDescending(p => p.EndDate).ToList();
                }
                else if (sortExpression == "Status")
                {
                    if (sortDirection == SortDirection.Ascending)
                        result = result.OrderBy(p => p.Status).ToList();
                    else
                        result = result.OrderByDescending(p => p.Status).ToList();
                }
                #endregion

                BindGridview((SLM.Application.Shared.GridviewPageController)pcTop, result.ToArray(), pageIndex);
                upResult.Update();

                pnTab006.Visible = true;
            }
            catch
            {
                throw;
            }
        }

        IEnumerable<ExistingProductData> InquiryCustomerSingleView(string IDNumber, string IDTypeCode, string IDIssueCountryCode)
        {
            CBSCustomerService.CBSCustomerService ws = new CBSCustomerService.CBSCustomerService();
            ws.Timeout = AppConstant.CBSTimeout;
            IEnumerable<ExistingProductData> result = new List<ExistingProductData>();
            try {
                InquiryCustomerSingleView req = new InquiryCustomerSingleView();
                req.Header = new InquiryCustomerSingleViewHeader
                {
                    ReferenceNo = DateTime.Now.ToString("yyyyMMddhhmmss", new System.Globalization.CultureInfo("en-US")),
                    TransactionDateTime = DateTime.Now,
                    ServiceName = AppConstant.CBSInquiryCustomerSingleView,
                    SystemCode = AppConstant.CBSSystemCode,
                    ChannelID = AppConstant.CBSChannelID
                };
                _log.Info("InquiryCustomerSingleView => ReferenceNo:" + req.Header.ReferenceNo + "  IDNumber:" + IDNumber + "  IDTypeCode : "+ IDTypeCode + "    IDIssueCountryCode : " + IDIssueCountryCode);

                req.InquiryCustomerSingleViewRequest = new InquiryCustomerSingleViewInquiryCustomerSingleViewRequest();
                req.InquiryCustomerSingleViewRequest.IDNumber = IDNumber;
                req.InquiryCustomerSingleViewRequest.IDTypeCode = IDTypeCode;
                req.InquiryCustomerSingleViewRequest.IDIssueCountryCode = IDIssueCountryCode;
                _log.DebugFormat("XML Request " + Environment.NewLine + "{0}", req.SerializeObject());

                InquiryCustomerSingleViewResponse res = new InquiryCustomerSingleViewResponse();
                res = ws.InquiryCustomerSingleView(req);
                
                if (res != null)
                {
                    if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-I-1000")
                    {
                        IEnumerable<ExistingProductData> awb = new List<ExistingProductData>();
                        IEnumerable<ExistingProductData> swb = new List<ExistingProductData>();
                        IEnumerable<ExistingProductData> iwb = new List<ExistingProductData>();

                        if (res.Header.ProductList != null)
                        {
                            var tmpAwb = (from c in res.Header.ProductList
                                   select new ExistingProductData
                                   {
                                       CitizenId = txtCitizenId.Text,
                                       ProductGroup = (!string.IsNullOrEmpty(c.AccountTypeCode) ? c.AccountTypeCode : ""),
                                       ProductName = (!string.IsNullOrEmpty(c.ProductCode) ? c.ProductCode : ""),
                                       //Grade = ,
                                       ContactNo = (!string.IsNullOrEmpty(c.AccountNumber) ? c.AccountNumber : ""),
                                       //StartDate = c.EffectiveDate,
                                       //EndDate = c.ValidTill,
                                       //PaymentTerm = ,
                                       Status = (string.IsNullOrEmpty(c.AccountStatus) == false ? AccountStatusBiz.GetAccountStatusByCode(c.AccountStatus).AccountStatusName : "")
                                   });
                            awb = (tmpAwb != null ? tmpAwb : new List<ExistingProductData>());
                        }

                        if (res.Header.ServiceList != null)
                        {
                            var tmpSwb = (from c in res.Header.ServiceList
                                   select new ExistingProductData
                                   {
                                       CitizenId = txtCitizenId.Text,
                                       ProductGroup = (!string.IsNullOrEmpty(c.AccountTypeCode) ? c.AccountTypeCode : ""),
                                       ProductName = (!string.IsNullOrEmpty(c.ProductCode) ? c.ProductCode : ""),
                                       //Grade = ,
                                       ContactNo = (!string.IsNullOrEmpty(c.AccountNumber) ? c.AccountNumber : ""),
                                       StartDate = SLMUtil.ConvertTimeZoneFromUtc(c.EffectiveDate),
                                       EndDate = SLMUtil.ConvertTimeZoneFromUtc(c.ValidTill),
                                       //PaymentTerm = ,
                                       Status = (string.IsNullOrEmpty(c.AccountStatus) == false ? AccountStatusBiz.GetAccountStatusByCode(c.AccountStatus).AccountStatusName : "")
                                   });
                            swb = (tmpSwb != null ? tmpSwb : new List<ExistingProductData>());
                        }

                        if (res.Header.InstructionList != null)
                        {
                            var tmpIwb = (from c in res.Header.InstructionList
                                   select new ExistingProductData
                                   {
                                       CitizenId = txtCitizenId.Text,
                                       ProductGroup = (!string.IsNullOrEmpty(c.AccountTypeCode) ? c.AccountTypeCode : ""),
                                       ProductName = (!string.IsNullOrEmpty(c.InstructionDescription) ? c.InstructionDescription : ""),
                                       //Grade = ,
                                       ContactNo = BankBiz.GetBankByNo(c.BankNumber.ToString()) + c.AccountNumber,
                                       StartDate = SLMUtil.ConvertTimeZoneFromUtc(c.EffectiveDate),
                                       //EndDate = c.ValidTill,
                                       //PaymentTerm = ,
                                       Status = (string.IsNullOrEmpty(c.SubscriptionStatus) == false ? AccountStatusBiz.GetAccountStatusByCode(c.SubscriptionStatus).AccountStatusName : "")
                                   });
                            iwb = (tmpIwb != null ? tmpIwb : new List<ExistingProductData>());
                        }

                        result = awb.Union(swb).Union(iwb);
                    }
                    else if (res.Header.ResponseStatusInfo.ResponseStatus.ResponseCode == "CBS-M-2001") {
                        _log.Info(res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage);
                        gvExistProduct.EmptyDataText = "<center><span style='color:Red;'>ไม่พบข้อมูล</span></center>";
                    }
                    else
                    {
                        _log.Error(res.Header.ResponseStatusInfo.ResponseStatus.ResponseMessage);
                        gvExistProduct.EmptyDataText = "<center><span style='color:Red;'>เกิดข้อผิดพลาดในการเชื่อมต่อ Webservice InquiryCustomerSingleView</span></center>";
                    }
                }
                _log.DebugFormat("XML Response " + Environment.NewLine + "{0}", res.SerializeObject());
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                gvExistProduct.EmptyDataText = "<center><span style='color:Red;'>ไม่สามารถเชื่อมต่อ Webservice InquiryCustomerSingleView</span></center>";
            }

            return result;
        }
        #endregion

        #region Page Control

        private void BindGridview(SLM.Application.Shared.GridviewPageController pageControl, object[] items, int pageIndex)
        {
            pageControl.SetGridview(gvExistProduct);
            pageControl.Update(items, pageIndex);
            pageControl.GenerateRecordNumber(0, pageIndex);
            upResult.Update();
        }

        protected void PageSearchChange(object sender, EventArgs e)
        {
            try
            {
                //List<ExistingProductData> result = SlmScr006Biz.SearchExistingProduct(txtCitizenId.Text.Trim());
                //var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                //BindGridview(pageControl, result.ToArray(), pageControl.SelectedPageIndex);

                var pageControl = (SLM.Application.Shared.GridviewPageController)sender;
                //DoSearchExistingProductList(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
                DoSearchCBSExistingProductList(pageControl.SelectedPageIndex, SortExpressionProperty, SortDirectionProperty);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        #endregion

        #region Sorting

        protected void gvExistProduct_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                if (SortExpressionProperty != e.SortExpression)         //เมื่อเปลี่ยนคอลัมน์ในการ sort
                    SortDirectionProperty = SortDirection.Ascending;
                else
                {
                    if (SortDirectionProperty == SortDirection.Ascending)
                        SortDirectionProperty = SortDirection.Descending;
                    else
                        SortDirectionProperty = SortDirection.Ascending;
                }

                SortExpressionProperty = e.SortExpression;
                //DoSearchExistingProductList(0, SortExpressionProperty, SortDirectionProperty);
                DoSearchCBSExistingProductList(0, SortExpressionProperty, SortDirectionProperty);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }

        public SortDirection SortDirectionProperty
        {
            get
            {
                if (ViewState["SortingState"] == null)
                {
                    ViewState["SortingState"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["SortingState"];
            }
            set
            {
                ViewState["SortingState"] = value;
            }
        }

        public string SortExpressionProperty
        {
            get
            {
                if (ViewState["ExpressionState"] == null)
                {
                    ViewState["ExpressionState"] = string.Empty;
                }
                return ViewState["ExpressionState"].ToString();
            }
            set
            {
                ViewState["ExpressionState"] = value;
            }
        }

        #endregion

        protected void lbReloadExistingProduct_Click(object sender, EventArgs e)
        {
            try
            {
                //GetExistingProductList(txtCitizenId.Text.Trim());
                GetCBSExistingProductList(txtCitizenId.Text.Trim(), txtCardTypeId.Text, txtCountryId.Text);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _log.Error(message);
                AppUtil.ClientAlert(Page, message);
            }
        }
    }
}