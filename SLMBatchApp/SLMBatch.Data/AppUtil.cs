using SLMBatch.Common;
using SLMBatch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMBatch.Data
{
    public class AppUtil
    {
        private static string errMsg = string.Empty;

        public static SLMDBEntities GetSlmDbEntities()
        {
            SLMDBEntities slmdb = new SLMDBEntities();
            slmdb.CommandTimeout = AppConstant.CommandTimeout;
            return slmdb;
        }

        public static string SLMDBConnectionString
        {
            get
            {
                SLMDBEntities slmdb = new SLMDBEntities();
                return ((System.Data.EntityClient.EntityConnection)slmdb.Connection).StoreConnection.ConnectionString;
            }
        }

        /// <summary>
        /// <br>Method Name : ConvertToDateTime</br>
        /// <br>Purpose     : To convert datetime string to DateTime object.</br>
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertToDateTime(string fieldName, string date, string dateFormat)
        {
            try
            {
                if (dateFormat == AppConstant.DateTimeFormat.Format1)
                {
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                }
                else if (dateFormat == AppConstant.DateTimeFormat.Format2)
                {
                    string[] tmp = date.Trim().Split('/');
                    return new DateTime(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]));
                }
                else
                    return new DateTime();
            }
            catch (Exception ex)
            {
                string msg = "String date(" + date + ") is not in a correct format in field " + fieldName + ": " + ex.Message;
                throw new Exception(msg);
            }
        }

        public static List<ProvinceData> GetProvinceDataList(SLMDBEntities slmdb)
        {
            try
            {
                string sql = @"select pro.slm_ProvinceId AS ProvinceId, pro.slm_ProvinceCode AS ProvinceCode, pro.slm_ProvinceNameTH AS ProvinceName
                                , am.slm_AmphurId AS AmphurId, am.slm_AmphurCode AS AmphurCode, am.slm_AmphurNameTH AS AmphurName
                                , tam.slm_TambolId AS TambolId, tam.slm_TambolCode AS TambolCode, tam.slm_TambolNameTH AS TambolName
                                from " + AppConstant.SLMDBName + @".dbo.kkslm_ms_province pro
                                left join " + AppConstant.SLMDBName + @".dbo.kkslm_ms_amphur am on am.slm_ProvinceCode = pro.slm_ProvinceCode
                                left join " + AppConstant.SLMDBName + @".dbo.kkslm_ms_tambol tam on tam.slm_ProvinceCode = am.slm_ProvinceCode and tam.slm_AmphurCode = am.slm_AmphurCode
                                order by pro.slm_ProvinceNameTH, am.slm_AmphurNameTH, tam.slm_TambolNameTH ";

                return slmdb.ExecuteStoreQuery<ProvinceData>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<BrandData> GetRedbookBrandList(SLMDBEntities slmdb)
        {
            try
            {
                return slmdb.kkslm_ms_redbook_brand.Select(p => new BrandData { BrandCode = p.slm_BrandCode, BrandName = p.slm_BrandName }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static List<ModelData> GetRedbookModelList(SLMDBEntities slmdb)
        //{
        //    try
        //    {
        //        return slmdb.kkslm_ms_redbook_model.Select(p => new ModelData { BrandCode = p.slm_BrandCode, ModelCode = p.slm_ModelCode, ModelName = p.slm_ModelName }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static List<SubModelData> GetRedbookSubModelList(SLMDBEntities slmdb)
        {
            try
            {
                return slmdb.kkslm_ms_redbook_submodel.Select(p => new SubModelData { KKKey = p.slm_KKKey, ModelCode = p.slm_ModelCode }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slmdb"></param>
        /// <param name="lead"></param>
        /// <param name="act"></param>
        /// <param name="newStatusDate">newStatusDate คือ วันเวลาที่เปลี่ยนสถานะใหม่</param>
        public static void CalculateTotalSLA(SLMDBEntities slmdb, kkslm_tr_lead lead, kkslm_tr_activity act, DateTime newStatusDate, string logfilename, string batchCode)
        {
            try
            {
                //Initial variables
                DateTime currentSla = new DateTime();
                DateTime currentDate = DeleteSeconds(newStatusDate);
                int thisWork = lead.slm_ThisWork != null ? Convert.ToInt32(lead.slm_ThisWork) : 0;
                int slaCounting = lead.slm_Counting != null ? Convert.ToInt32(lead.slm_Counting.Value) : 0;

                int workingMinPerDay = 0;
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = slmdb.kkslm_ms_calendar_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch && p.is_Deleted == false).ToList();
                var branch = slmdb.kkslm_ms_branch.Where(p => p.slm_BranchCode == lead.slm_Owner_Branch).FirstOrDefault();

                if (branch == null) { throw new Exception("ไม่พบข้อมูลสาขา BranchCode:" + lead.slm_Owner_Branch); }

                if (string.IsNullOrEmpty(branch.slm_StartTime_Hour) || string.IsNullOrEmpty(branch.slm_StartTime_Minute)
                    || string.IsNullOrEmpty(branch.slm_EndTime_Hour) || string.IsNullOrEmpty(branch.slm_EndTime_Minute))
                {
                    string start = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "startTime").Select(p => p.slm_OptionDesc).FirstOrDefault();
                    string end = slmdb.kkslm_ms_option.Where(p => p.slm_OptionCode == "endTime").Select(p => p.slm_OptionDesc).FirstOrDefault();

                    if (start != null)
                    {
                        string[] str = start.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            startTimeHour = Convert.ToInt32(str[0]);
                            startTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาเปิดสาขา: " + branch.slm_BranchName);

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                    }
                    else
                        throw new Exception("ไม่พบเวลาปิดสาขา: " + branch.slm_BranchName);
                }
                else
                {
                    startTimeHour = int.Parse(branch.slm_StartTime_Hour);
                    endTimeMin = int.Parse(branch.slm_StartTime_Minute);
                    endTimeHour = int.Parse(branch.slm_EndTime_Hour);
                    endTimeMin = int.Parse(branch.slm_EndTime_Minute);

                    DateTime tmpStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTimeHour, startTimeMin, 0);
                    DateTime tmpEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTimeHour, endTimeMin, 0);

                    TimeSpan tmpTs = tmpEnd.Subtract(tmpStart);
                    workingMinPerDay = Convert.ToInt32(tmpTs.TotalMinutes);     //ได้เวลาที่ต้องทำงานในแต่ละวันของสาขา หน่วยเป็นนาที
                }

                if (slaCounting == 0)    //กรณีทำงานเสร็จก่อน sla เตือน
                {
                    //เผื่อในกรณี slm_StatusDate เป็น null, ซึ่งน่าจะไม่มีโอกาสเกิดเคสนี้ ยกเว้น คีย์ข้อมูลลงเบสตรง
                    currentSla = lead.slm_StatusDate != null ? DeleteSeconds(lead.slm_StatusDate.Value) : currentDate;
                }
                else
                {
                    currentSla = lead.slm_CurrentSLA != null ? DeleteSeconds(lead.slm_CurrentSLA.Value) : currentDate;
                }

                //ปรับเวลาในกรณี currentSla อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                int checkStartTime = Convert.ToInt32(startTimeHour.ToString("00") + startTimeMin.ToString("00"));
                int checkEndTime = Convert.ToInt32(endTimeHour.ToString("00") + endTimeMin.ToString("00"));
                int timeToCheck = Convert.ToInt32(currentSla.Hour.ToString("00") + currentSla.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentSla ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentSla = currentSla.AddDays(1);
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentSla.Date).Count() > 0)
                {
                    currentSla = currentSla.AddDays(1);
                }

                //ปรับเวลาในกรณี currentDate อยู่นอกเวลาทำงาน, หรือถ้าปรับแล้วยังไปตกวันหยุดก็ต้องให้เลื่อนไปจนถึงวันทำงาน
                timeToCheck = Convert.ToInt32(currentDate.Hour.ToString("00") + currentDate.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)     //ถ้าเวลาของ currentDate ไม่ได้อยู่ในช่วงเวลาทำงาน
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentDate = currentDate.AddDays(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.slm_HolidayDate.Date == currentDate.Date).Count() > 0)
                {
                    currentDate = currentDate.AddDays(1);
                }

                //คำนวณ TotalAlert, TotalWork
                if (currentSla.Date == currentDate.Date)
                {
                    TimeSpan ts = currentDate.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }
                else
                {
                    //หาวันที่อยู่ตรงกลางระหว่าง currentSla กับ currentDate
                    //เช่น currentSla = 12/04/2016, currentDate = 16/04/2016 ให้หาวันที่ 13, 14, 15 ออกมาเพื่อเช็กว่าเป็นวันทำงานหรือวันหยุด ถ้าเป็นวันทำงานให้เก็บจำนวนนาทีที่ต้องทำงานต่อวัน
                    DateTime startDate = currentSla.Date;
                    DateTime endDate = currentDate.Date;

                    DateTime tmpDate = startDate.AddDays(1);
                    while (tmpDate < endDate)
                    {
                        if (calendarTab.Where(p => p.slm_HolidayDate == tmpDate).Count() == 0)
                        {
                            thisWork += workingMinPerDay;
                        }
                        tmpDate = tmpDate.AddDays(1);
                    }

                    //ให้คำนวณเวลาที่เหลือในส่วนของวัน startDate และ endDate
                    DateTime endWorkTime_for_currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, endTimeHour, endTimeMin, 0);
                    DateTime startWorkTime_for_currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);

                    TimeSpan ts = endWorkTime_for_currentSla.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);

                    ts = currentDate.Subtract(startWorkTime_for_currentDate);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }

                lead.slm_TotalAlert = (lead.slm_TotalAlert != null ? Convert.ToInt32(lead.slm_TotalAlert) : 0) + slaCounting;
                lead.slm_TotalWork = (lead.slm_TotalWork != null ? Convert.ToInt32(lead.slm_TotalWork) : 0) + thisWork;
                lead.slm_Counting = 0;
                lead.slm_ThisWork = 0;
                lead.slm_CurrentSLA = null;

                //OwnerLoggin
                act.slm_ThisAlert = slaCounting;
                act.slm_ThisWork = thisWork;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Util.WriteLogFile(logfilename, batchCode, message);
            }
        }

        private static DateTime DeleteSeconds(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        public static LeadDataForCARLogService GetDataForCARLogService(string ticketId, SLMDBEntities slmdb)
        {
            try
            {
                string slmdbName = AppConstant.SLMDBName;
                string sql = @"SELECT lead.slm_TicketId AS TicketId, prelead.slm_Prelead_Id AS PreleadId, lead.slm_CampaignId AS CampaignId, cam.slm_CampaignName AS CampaignName, lead.slm_ChannelId AS ChannelId, lead.slm_Product_Group_Id AS ProductGroupId, lead.slm_Product_Id AS ProductId
                                , lead.slm_Status AS StatusCode, opt.slm_OptionDesc AS StatusName, lead.slm_SubStatus AS SubStatusCode, lead.slm_ExternalSubStatusDesc AS SubStatusName
                                , cus.slm_CitizenId AS CitizenId, cus.slm_CardType AS CardTypeId, cardtype.slm_CardTypeName AS CardTypeName, pg.product_name AS ProductGroupName, mp.sub_product_name AS ProductName
                                , renew.slm_LicenseNo AS LicenseNo, renew.slm_ContractNo AS ContractNo, ISNULL(title.slm_TitleName, '') + ISNULL(lead.slm_Name, '') + ' ' + ISNULL(lead.slm_LastName, '') AS CustomerName
                                , LEAD.slm_Owner AS Owner, LEAD.slm_Delegate AS Delegate
                                , insurcom.slm_InsNameTh AS InsuranceCompany, renew.slm_PolicyStartCoverDate AS PolicyStartCoverDate, renew.slm_PolicyEndCoverDate AS PolicyEndCoverDate
                                , renew.slm_PolicyNo AS PolicyNo, cardtype.slm_CIFSubscriptTypeId AS SubScriptionTypeId, renew.slm_ActNo AS ActNo 
                                , renew.slm_IncentiveFlag AS IncentiveFlag, renew.slm_ActIncentiveFlag AS IncentiveFlagAct, renew.slm_ReceiveNo AS ReceiveNo, renew.slm_ActSendDate AS ActSendDate
                                , renew.slm_ActStartCoverDate AS ActStartCoverDate, renew.slm_ActEndCoverDate AS ActEndCoverDate
                                FROM " + slmdbName + @".dbo.kkslm_tr_lead LEAD
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_cusinfo cus ON cus.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_renewinsurance renew ON renew.slm_TicketId = lead.slm_ticketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_option opt ON lead.slm_Status = opt.slm_OptionCode AND opt.slm_OptionType = 'lead status' AND opt.is_Deleted = '0'
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_campaign cam ON cam.slm_CampaignId = lead.slm_CampaignId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_cardtype cardtype ON cardtype.slm_CardTypeId = cus.slm_CardType
                                LEFT JOIN " + slmdbName + @".dbo.CMT_MS_PRODUCT_GROUP pg ON lead.slm_Product_Group_Id = pg.product_id
                                LEFT JOIN " + slmdbName + @".dbo.CMT_MAPPING_PRODUCT mp ON mp.sub_product_id = lead.slm_Product_Id 
                                LEFT JOIN " + AppConstant.OPERDBName + @".dbo.kkslm_ms_ins_com insurcom ON insurcom.slm_Ins_Com_Id = renew.slm_InsuranceComId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_tr_prelead prelead ON prelead.slm_TicketId = lead.slm_TicketId
                                LEFT JOIN " + slmdbName + @".dbo.kkslm_ms_title title ON title.slm_TitleId = lead.slm_TitleId AND title.is_Deleted = 0
                                WHERE LEAD.slm_ticketId = '" + ticketId + "' ";

                return slmdb.ExecuteStoreQuery<LeadDataForCARLogService>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int SafeInt(string str)
        {
            if (str == null) return 0;
            int r; int.TryParse(str.Replace(",", ""), out r);
            return r;
        }

        public static void UpdatePhonecallCASFlag(SLMDBEntities slmdb, int phonecall_id, string casFlag)
        {
            try
            {
                var phonecall = slmdb.kkslm_phone_call.Where(p => p.slm_PhoneCallId == phonecall_id).FirstOrDefault();
                if (phonecall != null)
                {
                    phonecall.slm_CAS_Flag = casFlag;
                    phonecall.slm_CAS_Date = DateTime.Now;
                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
        }

        #region Backup

        //public static string GetProductId(SLMDBEntities slmdb, string campaignId)
        //{
        //    try
        //    {
        //        string sql = "SELECT PR_ProductId FROM SLMDB.dbo.CMT_CAMPAIGN_PRODUCT WHERE PR_CampaignId = '" + campaignId + "'";
        //        return slmdb.ExecuteStoreQuery<string>(sql).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static int? GetTitleId(SLMDBEntities slmdb, string titleName)
        //{
        //    try
        //    {
        //        int? id = slmdb.kkslm_ms_title.Where(p => p.slm_TitleName == titleName && p.is_Deleted == false).Select(p => p.slm_TitleId).FirstOrDefault();
        //        return id != 0 ? id : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static int? GetOccupationId(SLMDBEntities slmdb, string occupationCode)
        //{
        //    try
        //    {
        //        int? id = slmdb.kkslm_ms_occupation.Where(p => p.slm_OccupationCode == occupationCode).Select(p => p.slm_OccupationId).FirstOrDefault();
        //        return id != 0 ? id : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static int? GetInsuranceCarTypeId(SLMDBEntities slmdb, string insuranceCarTypeName)
        //{
        //    try
        //    {
        //        int? id = slmdb.kkslm_ms_insurancecartype.Where(p => p.slm_InsurancecarTypeName == insuranceCarTypeName && p.is_Deleted == false).Select(p => p.slm_InsurancecarTypeId).FirstOrDefault();
        //        return id != 0 ? id : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static int? GetRelationId(SLMDBEntities slmdb, string relationDesc)
        //{
        //    try
        //    {
        //        int? id = slmdb.kkslm_ms_relate.Where(p => p.slm_RelateDesc == relationDesc && p.is_Deleted == false).Select(p => p.slm_RelateId).FirstOrDefault();
        //        return id != 0 ? id : null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static string GetEmpCode(SLMDBEntities slmdb, string mktCode)
        //{
        //    try
        //    {
        //        return slmdb.kkslm_ms_staff.Where(p => p.slm_MarketingCode == mktCode).Select(p => p.slm_EmpCode).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static bool CheckBrandCodeExist(SLMDBEntities slmdb,string brandCode)
        //{
        //    try
        //    {
        //        int count = slmdb.kkslm_ms_redbook_brand.Where(p => p.slm_BrandCode == brandCode).Count();
        //        return count > 0 ? true : false;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static bool CheckModelCodeExist(SLMDBEntities slmdb, string brandCode, string modelCode)
        //{
        //    try
        //    {
        //        int count = slmdb.kkslm_ms_redbook_model.Where(p => p.slm_BrandCode == brandCode && p.slm_ModelCode == modelCode).Count();
        //        return count > 0 ? true : false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static kkslm_ms_province GetProvince(SLMDBEntities slmdb, string provinceName)
        //{
        //    try
        //    {
        //        return slmdb.kkslm_ms_province.Where(p => p.slm_ProvinceNameTH == provinceName).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static kkslm_ms_amphur GetAmphur(SLMDBEntities slmdb, string provinceCode, string amphurName)
        //{
        //    try
        //    {
        //        return slmdb.kkslm_ms_amphur.Where(p => p.slm_ProvinceCode == provinceCode && p.slm_AmphurNameTH == amphurName).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static kkslm_ms_tambol GetTambol(SLMDBEntities slmdb, string provinceCode, string amphurCode, string tambolName)
        //{
        //    try
        //    {
        //        return slmdb.kkslm_ms_tambol.Where(p => p.slm_ProvinceCode == provinceCode && p.slm_AmphurCode == amphurCode && p.slm_TambolNameTH == tambolName).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion
    }
}
