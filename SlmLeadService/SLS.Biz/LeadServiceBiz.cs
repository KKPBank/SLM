using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Dal;
using SLS.Dal.Models;
using SLS.Resource;
using SLS.Resource.Data;
using System.Transactions;

namespace SLS.Biz
{
    ///<summary>
    /// <br>Class Name : LeadServiceBiz</br>
    /// <br>Purpose    : To manage business of the service.</br>
    /// <br>Author     : TikumpornA.</br>
    ///</summary>
    public class LeadServiceBiz
    {
        /// <summary>
        /// <br>Method Name : AuthenticateHeader</br>
        /// <br>Purpose     : To authenticate header.</br>
        /// </summary>
        /// <param name="username" type="string"></param>
        /// <param name="password" type="string"></param>
        /// <param name="channelId" type="string"></param>
        /// <returns type="bool"></returns>
        public static bool AuthenticateHeader(string username, string password, string channelId, string operationFlag, int timeout)
        {
            SLM_DBEntities slmdb = null;
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, operationFlag);

                SlmMsChannelModel channel = new SlmMsChannelModel(slmdb);
                return channel.AuthenticateHeader(username, password, channelId);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <br>Method Name : InsertLeadData</br>
        /// <br>Purpose     : To insert xml data to database.</br>
        /// </summary>
        /// <param name="mandatory" type="Mandatory"></param>
        /// <param name="cusInfo" type="CustomerInfo"></param>
        /// <param name="cusDetail" type="CustomerDetail"></param>
        /// <param name="prodInfo" type="ProductInfo"></param>
        /// <param name="channInfo" type="ChannelInfo"></param>
        /// <returns type="string"></returns>
        public static string InsertLeadData(Mandatory mandatory, CustomerInfo cusInfo, CustomerDetail cusDetail, ProductInfo prodInfo, ChannelInfo channInfo, DealerInfo dealerInfo, string reference, AppInfo appInfo, string channelId, int timeout)
        {
            string ticketId = "";
            string productId = "";
            SLM_DBEntities slmdb = null;
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, ApplicationResource.INS_OPERATION);

                StoreProcedure store = new StoreProcedure(slmdb);
                ticketId = store.GenerateTicketId();

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    SlmTrLeadModel lead = new SlmTrLeadModel(slmdb);
                    productId = lead.InsertData(ticketId, mandatory, cusInfo, cusDetail, channInfo, channelId, dealerInfo, reference, appInfo);

                    SlmTrCusInfoModel customerInfo = new SlmTrCusInfoModel(slmdb);
                    customerInfo.InsertData(ticketId, cusInfo, cusDetail, channInfo);

                    SlmTrProductInfoModel productInfo = new SlmTrProductInfoModel(slmdb);
                    productInfo.InsertData(ticketId, prodInfo);

                    SlmTrChannelInfoModel channelInfo = new SlmTrChannelInfoModel(slmdb);
                    channelInfo.InsertData(ticketId, channInfo, channelId);

                    SlmTrCampaignFinalModel campaignFinal = new SlmTrCampaignFinalModel(slmdb);
                    campaignFinal.InsertData(ticketId, mandatory, channInfo);

                    //Dynamic by campaign or product
                    SlmMsConfigProductScreenModel screen = new SlmMsConfigProductScreenModel(slmdb);
                    string tableName = screen.GetTableName(mandatory.Campaign, productId, ServiceConstant.ScreenType.Insert, ApplicationResource.INS_OPERATION);
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        switch (tableName)
                        {
                            case "kkslm_tr_renewinsurance":
                                SlmTrRenewInsuranceModel reins = new SlmTrRenewInsuranceModel(slmdb);
                                reins.InsertData(ticketId, prodInfo, channInfo);
                                break;
                            default:
                                break;
                        }
                    }

                    ts.Complete();
                }

                return ticketId;
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <br>Method Name : UpdateLeadData</br>
        /// <br>Purpose     : To update xml data to database.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="mandatory" type="Mandatory"></param>
        /// <param name="cusInfo" type="CustomerInfo"></param>
        /// <param name="cusDetail" type="CustomerDetail"></param>
        /// <param name="prodInfo" type="ProductInfo"></param>
        /// <param name="channInfo" type="ChannelInfo"></param>
        /// <returns type="void"></returns>
        public static string UpdateLeadData(string ticketId, Mandatory mandatory, CustomerInfo cusInfo, CustomerDetail cusDetail, ProductInfo prodInfo, ChannelInfo channInfo, string channelId, string sendEmailFlag, List<string> sendEmailPerson, int timeout, string systemName, out string calculateTotalSlaError)
        {
            SLM_DBEntities slmdb = null;
            string noteId = "";
            try
            {
                calculateTotalSlaError = "";
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, ApplicationResource.UPD_OPERATION);

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    SlmTrLeadModel lead = new SlmTrLeadModel(slmdb);
                    lead.UpdateData(ticketId, cusInfo, channInfo, systemName, out calculateTotalSlaError);

                    SlmTrCusInfoModel customerInfo = new SlmTrCusInfoModel(slmdb);
                    customerInfo.UpdateData(ticketId, cusInfo, cusDetail, channInfo);

                    //ISlmTrProductInfoModel productInfo = new SlmTrProductInfoModel(slmdb);
                    //productInfo.UpdateData(ticketId, prodInfo);

                    //ISlmTrChannelInfoModel channelInfo = new SlmTrChannelInfoModel(slmdb);
                    //channelInfo.UpdateData(ticketId, channInfo, channelId);

                    SlmNoteModel note = new SlmNoteModel(slmdb);
                    noteId = note.InsertData(ticketId, cusDetail, channInfo, sendEmailFlag, sendEmailPerson);

                    ts.Complete();
                }

                return noteId;
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <br>Method Name : UpdateLeadData</br>
        /// <br>Purpose     : To update xml data to database.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="mandatory" type="Mandatory"></param>
        /// <param name="cusInfo" type="CustomerInfo"></param>
        /// <param name="cusDetail" type="CustomerDetail"></param>
        /// <param name="prodInfo" type="ProductInfo"></param>
        /// <param name="channInfo" type="ChannelInfo"></param>
        /// <returns type="void"></returns>
        public static string UpdateLeadByHpaofl(string ticketId, Mandatory mandatory, CustomerInfo cusInfo, CustomerDetail cusDetail, ProductInfo prodInfo, ChannelInfo channInfo, string channelId, string sendEmailFlag, List<string> sendEmailPerson, AppInfo appInfo, DealerInfo dealerInfo, int timeout)
        {
            SLM_DBEntities slmdb = null;
            string productId = "";
            string noteId = "";
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, ApplicationResource.UPD_OPERATION);

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    SlmTrLeadModel lead = new SlmTrLeadModel(slmdb);
                    productId = lead.UpdateDataByHpaolf(ticketId, mandatory, cusInfo, cusDetail, channInfo, channelId, appInfo, dealerInfo);

                    SlmTrCusInfoModel customerInfo = new SlmTrCusInfoModel(slmdb);
                    customerInfo.UpdateDataByHpaofl(ticketId, cusInfo, cusDetail, channInfo);

                    SlmTrProductInfoModel productInfo = new SlmTrProductInfoModel(slmdb);
                    productInfo.UpdateDataByHpaofl(ticketId, prodInfo);

                    SlmTrChannelInfoModel channelInfo = new SlmTrChannelInfoModel(slmdb);
                    channelInfo.UpdateDataByHpaofl(ticketId, channInfo, channelId);

                    SlmNoteModel note = new SlmNoteModel(slmdb);
                    noteId = note.InsertData(ticketId, cusDetail, channInfo, sendEmailFlag, sendEmailPerson);

                    //Dynamic by campaign or product
                    SlmMsConfigProductScreenModel screen = new SlmMsConfigProductScreenModel(slmdb);
                    string tableName = screen.GetTableName(mandatory.Campaign, productId, ServiceConstant.ScreenType.Edit, ApplicationResource.UPD_OPERATION);
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        switch (tableName)
                        {
                            case "kkslm_tr_renewinsurance":
                                SlmTrRenewInsuranceModel reins = new SlmTrRenewInsuranceModel(slmdb);
                                reins.UpdateDataByHpaofl(ticketId, prodInfo, channInfo);
                                break;
                            default:
                                break;
                        }
                    }

                    ts.Complete();
                }

                return noteId;
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        //public static void UpdateLeadData(string ticketId, CustomerDetail cusDetail, ChannelInfo channInfo, int timeout)
        //{
        //    SLM_DBEntities slmdb = null;
        //    try
        //    {
        //        slmdb = new SLM_DBEntities();
        //        slmdb.Connection.TryOpen(timeout * 1000, ApplicationResource.UPD_OPERATION);

        //        ISlmNoteModel note = new SlmNoteModel(slmdb);
        //        note.InsertData(ticketId, cusDetail, channInfo);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (slmdb.Connection.State == System.Data.ConnectionState.Open)
        //        {
        //            slmdb.Connection.Close();
        //            slmdb.Connection.Dispose();
        //        }
        //    }
        //}

        /// <summary>
        /// <br>Method Name : UpdateChannelInfoResponse</br>
        /// <br>Purpose     : To update response data to table channelInfo.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="responseCode" type="string"></param>
        /// <param name="responseMsg" type="string"></param>
        /// <param name="responseDate" type="DateTime"></param>
        /// <returns type="void"></returns>
        public static void UpdateChannelInfoResponse(string ticketId, string responseCode, string responseMsg, DateTime responseDate, string operationFlag, int timeout)
        {
            SLM_DBEntities slmdb = null;
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, operationFlag);

                SlmTrChannelInfoModel channelInfo = new SlmTrChannelInfoModel(slmdb);
                channelInfo.UpdateResponseData(ticketId, responseCode, responseMsg, responseDate);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <br>Method Name : SearchLeadData</br>
        /// <br>Purpose     : To search xml data from database.</br>
        /// </summary>
        /// <param name="ticketId" type="string"></param>
        /// <param name="timeout" type="int"></param>
        /// <returns type="SearchLeadData"></returns>
        public static SearchLeadData SearchLeadData(string ticketId, int timeout)
        {
            SLM_DBEntities slmdb = null;
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, ApplicationResource.SEARCH_OPERATION);

                SearchLeadModel search = new SearchLeadModel(slmdb);
                return search.SearchLeadData(ticketId);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }
        
        public static bool CheckAvailableTime(string time, string operationFlag)
        {
            try
            {
                int availableTime = Convert.ToInt32(time);
                string hour_db = "";
                string min_db = "";
                int startTime = 0;
                int endTime = 0;

                SLM_DBEntities slmdb = new SLM_DBEntities();
                var var1 = slmdb.kkslm_ms_option.Where(p => p.slm_OptionType.ToUpper() == "WORKTIME" && p.slm_OptionCode.ToUpper() == "STARTTIME").Select(p => p.slm_OptionDesc).FirstOrDefault();
                var var2 = slmdb.kkslm_ms_option.Where(p => p.slm_OptionType.ToUpper() == "WORKTIME" && p.slm_OptionCode.ToUpper() == "ENDTIME").Select(p => p.slm_OptionDesc).FirstOrDefault();

                if (var1 == null || var2 == null)
                    return false;

                string[] tmp = var1.Split(':');
                hour_db = Convert.ToInt32(tmp[0].Trim()).ToString("00");
                min_db = Convert.ToInt32(tmp[1].Trim()).ToString("00");
                startTime = Convert.ToInt32(hour_db + min_db + "00");

                tmp = var2.Split(':');
                hour_db = Convert.ToInt32(tmp[0].Trim()).ToString("00");
                min_db = Convert.ToInt32(tmp[1].Trim()).ToString("00");
                endTime = Convert.ToInt32(hour_db + min_db + "00");

                if (availableTime < startTime || availableTime > endTime)
                    return false;
                else
                    return true;
            }
            catch(Exception ex)
            {
                if (operationFlag == ApplicationResource.INS_OPERATION)
                    throw new ServiceException(ApplicationResource.INS_INVALID_PARAMETERS_CODE, ApplicationResource.INS_INVALID_PARAMETERS_DESC, ex.Message, null);
                else
                    throw new ServiceException(ApplicationResource.UPD_INVALID_PARAMETERS_CODE, ApplicationResource.UPD_INVALID_PARAMETERS_DESC, ex.Message, null);
            }
        }

        public static bool CheckUseOfAolService(string channelId, string campaignId, int timeout, string operationFlag)
        {
            SLM_DBEntities slmdb = null;
            try
            {
                slmdb = new SLM_DBEntities();
                slmdb.Connection.TryOpen(timeout * 1000, operationFlag);

                SlmWsConfigModel config = new SlmWsConfigModel(slmdb);
                return config.CheckUseOfAolService(channelId, campaignId);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slmdb.Connection.State == System.Data.ConnectionState.Open)
                {
                    slmdb.Connection.Close();
                    slmdb.Connection.Dispose();
                }
            }
        }

        public static CampaignData GetGampaign(string campaignId)
        {
            try
            {
                SLM_DBEntities slmdb = new SLM_DBEntities();
                var campaign = slmdb.kkslm_ms_campaign.Where(p => p.slm_CampaignId == campaignId && p.is_Deleted == 0).FirstOrDefault();
                if (campaign != null)
                {
                    CampaignData data = new CampaignData();
                    data.CampaignId = campaign.slm_CampaignId;
                    data.CampaignName = campaign.slm_CampaignName != null ? campaign.slm_CampaignName : "";
                    data.CampaignType = campaign.slm_CampaignType != null ? campaign.slm_CampaignType : "";
                    data.Status = campaign.slm_Status != null ? campaign.slm_Status : "";

                    if (!string.IsNullOrEmpty(campaign.slm_Offer) && !string.IsNullOrEmpty(campaign.slm_criteria))
                        data.CampaignDesc = campaign.slm_Offer + " : " + campaign.slm_criteria;
                    else if (!string.IsNullOrEmpty(campaign.slm_Offer))
                        data.CampaignDesc = campaign.slm_Offer;
                    else if (!string.IsNullOrEmpty(campaign.slm_criteria))
                        data.CampaignDesc = campaign.slm_criteria;
                    else
                        data.CampaignDesc = "";

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUsername(string empCode)
        {
            try
            {
                SlmMsStaffModel staff = new SlmMsStaffModel(new SLM_DBEntities());
                return staff.GetUsername(empCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckUsernameExist(string username)
        {
            try
            {
                SlmMsStaffModel staff = new SlmMsStaffModel(new SLM_DBEntities());
                return staff.CheckUsernameExist(username);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckEmpCodeExist(string empCode)
        {
            try
            {
                SlmMsStaffModel staff = new SlmMsStaffModel(new SLM_DBEntities());
                return staff.CheckEmpCodeExist(empCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckCountryCodeExist(string countryCode)
        {
            try {
                SlmMsCountryModel country = new SlmMsCountryModel(new SLM_DBEntities());
                return country.CheckCountryCodeExist(countryCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertLogData(string ticketId, string operation, DateTime operationDate, string channelId, string username, string inputXml, string outputXml
            , string responseCode, string responseDesc, DateTime responseDate, string responseTime, string causeError)
        {
            try
            {
                SlmWsLogModel log = new SlmWsLogModel(new SLM_DBEntities());
                log.InsertLogData(ticketId, operation, operationDate, channelId, username, inputXml, outputXml, responseCode, responseDesc, responseDate, responseTime, causeError);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ValidateSystem(string systemName)
        {
            CocMsSystemModel system = new CocMsSystemModel();
            return system.ValidateSystem(systemName);
        }

        public static LeadDataForCARLogService GetDataForCARLogService(string ticketId)
        {
            try
            {
                SlmTrLeadModel model = new SlmTrLeadModel(new SLM_DBEntities());
                return model.GetDataForCARLogService(ticketId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetStaffName(string username)
        {
            return new SlmMsStaffModel(new SLM_DBEntities()).GetStaffName(username);
        }

        public static void UpdateCasFlag(string ticketId, string noteId, string flag)
        {
            new SlmNoteModel(new SLM_DBEntities()).UpdateCasFlag(ticketId, noteId, flag);
        }
    }
}
