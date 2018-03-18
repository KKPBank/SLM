using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Dal;
using SLM.Dal.Models;
using SLM.Resource.Data;
using System.Data;
using System.Globalization;
using System.Transactions;
using System.Data.SqlClient;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr034Biz
    {
        public static List<ControlListData> GetSearchInsComListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            // using (SLM_DBEntities slmdb = new SLM_DBEntities())
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                // List<decimal> companyId = slmdb.kkslm_tr_problem.Select(p => p.slm_ProblemId).Distinct().ToList();
                // if (companyId != null && companyId.Count > 0)
                {
                    result = operdb.kkslm_ms_ins_com
                        //     .Where(kk => companyId.Contains(kk.slm_Ins_Com_Id))
                            .ToList()
                            .Select(kk => new ControlListData() { TextField = kk.slm_InsNameTh, ValueField = kk.slm_Ins_Com_Id.ToString() })
                            .OrderBy(r => r.TextField)
                            .ToList();
                }
                result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }
        public static List<ControlListData> GetImportInsComListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                {
                    result = operdb.kkslm_ms_ins_com                            
                            .ToList()
                            .Select(kk => new ControlListData() { TextField = kk.slm_InsNameTh, ValueField = kk.slm_Ins_Com_Id.ToString() })
                            .OrderBy(r => r.TextField)
                            .ToList();
                }
                result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }
        public static List<ControlListData> GetImportInsComCodeListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                {
                    result = operdb.kkslm_ms_ins_com                            
                            .ToList()
                            .Select(kk => new ControlListData() { TextField = kk.slm_InsNameTh, ValueField = kk.slm_InsCode.ToString() })                            
                            .OrderBy(r => r.TextField)
                            .ToList();
                }
                //result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }
        public static List<ControlListData> GetSearchFileNameListData()
        {
            List<ControlListData> result = new List<ControlListData>();
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {

                result = slmdb.kkslm_tr_problem
                        .Where(p => p.is_Deleted == false)
                        .Select(kk => new ControlListData() { TextField = kk.slm_FileName, ValueField = kk.slm_FileName })
                        .Distinct()
                        .OrderBy(r => r.TextField)
                        .ToList();

                result.Insert(0, new ControlListData() { TextField = "", ValueField = "-1" });
            }
            return result;
        }

        public static List<SlmScr034SearchList> GetSearchResult(string insComId, string fileName)
        {
            DataSet ds = new DataSet();
            List<SlmScr034SearchList> result = null;
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                string commandText = @"Select ROW_NUMBER() OVER(order by insCom.slm_InsNameTh, p.slm_FileName) as RowNum
	                                        , insCom.slm_InsNameTh, p.slm_FileName
	                                        , TotalCase = Count(*)
	                                        , OpenCase = Sum(case when pd.slm_IsAction = 1 then 1 else 0 end)
	                                        , CloseCase = Sum(case when pd.slm_IsAction = 0 then 1 else 0 end)
                                        from " + SLMConstant.OPERDBName + @".[dbo].kkslm_ms_ins_com insCom 
	                                        join [dbo].[kkslm_tr_problem] p on insCom.slm_Ins_Com_Id = p.slm_Ins_Com_Id and p.is_Deleted = 0
	                                        join [dbo].[kkslm_tr_problemdetail] pd on p.slm_ProblemId = pd.slm_ProblemId
                                        where 1=1 {0} {1}
                                        group by insCom.slm_InsNameTh, p.slm_FileName                                         
                                        order by RowNum";
                string whereInsCom = "", whereFileName = "";
                if (insComId != "-1")
                    whereInsCom = string.Format(" and p.slm_Ins_Com_Id = {0}", insComId);
                if (fileName != "-1")
                    whereFileName = string.Format(" and p.slm_FileName = N'{0}'", fileName);
                commandText = string.Format(commandText, whereInsCom, whereFileName);

                result = slmdb.ExecuteStoreQuery<SlmScr034SearchList>(commandText).ToList();
            }
            // return ds;
            return result;
        }
        public static bool HasDuplicate(decimal insComId, string filename, DateTime date)
        {
            DateTime nextDay = date.AddDays(1);
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var existing = slmdb.kkslm_tr_problem.Where(p => p.slm_Ins_Com_Id == insComId
                                && p.slm_FileName == filename && !p.is_Deleted
                    // && p.slm_CreatedDate >= date && p.slm_CreatedDate < nextDay
                                )
                                .ToList();
                if (existing != null && existing.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static void DeleteImportedProblem(decimal insComId, string filename, DateTime date)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                DateTime nextDay = date.AddDays(1);
                var problem = slmdb.kkslm_tr_problem.Where(kk => kk.slm_Ins_Com_Id == insComId && kk.slm_FileName == filename
                                && kk.slm_CreatedDate >= date && kk.slm_CreatedDate < nextDay)
                                .ToList();
                var ids = problem.Select(p => p.slm_ProblemId).ToList<decimal>();
                var problemDetail = slmdb.kkslm_tr_problemdetail
                            .Where(k => ids.Contains((decimal)k.slm_ProblemId)).ToList();
                problemDetail.ForEach(kk =>
                {
                    slmdb.kkslm_tr_problemdetail.DeleteObject(kk);
                });

                problem.ForEach(kk =>
                {
                    slmdb.kkslm_tr_problem.DeleteObject(kk);
                });
                slmdb.SaveChanges();
            }
        }
        public static List<SlmProblemDetailData> ValidateProblemDetails(DataTable dt, string comName)
        {
            List<SlmProblemDetailData> problemD = new List<SlmProblemDetailData>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                if (string.Join("", dr.ItemArray.ToList().Select(a => a.ToString().Trim()).ToList()) == string.Empty)
                    continue;

                SlmProblemDetailData item = new SlmProblemDetailData();
                item.ProblemDetailId = i + 2;
                item.ProblemDateString = dr[0].ToString().Trim();
                item.InsComName = dr[1].ToString().Trim();
                item.InsType = dr[2].ToString().Trim();
                item.Contract_Number = dr[3].ToString().TrimStart('\'').Trim();
                item.Name = dr[4].ToString().Trim();
                item.TelesaleName = dr[5].ToString().Trim();
                item.ProblemDetail = dr[6].ToString().Trim();
                item.CauseDetail = dr[7].ToString().Trim();
                item.HeadStaff = dr[8].ToString().TrimStart('\'').Trim();
                item.ResponseDetail = dr[9].ToString().Trim();
                item.PhoneContact = dr[10].ToString().Trim();
                item.ResponseDateString = dr[11].ToString().Trim();
                item.Remark = dr[12].ToString().Trim();
                problemD.Add(item);
            }

            if (problemD.Count == 0)
                return problemD;

            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                var comlst = operdb.kkslm_ms_ins_com.ToList();
                var stafflst = slmdb.kkslm_ms_staff.ToList();

                problemD.ForEach(d =>
                {
                    DateTime tmpDatetime;
                    if (d.ProblemDateString == "")
                    {
                        d.ErrorMessage += "Column A : กรุณาระบุ วดป<br>";
                    }
                    else
                    {
                        if (DateTime.TryParseExact(d.ProblemDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDatetime))
                        {
                            d.ProblemDate = tmpDatetime;
                        }
                        else
                        {
                            d.ErrorMessage += "Column A : รูปแบบวันที่ผิด (DD/MM/YYYY)<br>";
                        }
                    }

                    if (d.InsComName.Trim() == "")
                        d.ErrorMessage += "Column B: กรุณาระบุชื่อ บจ.ประกันภัย<br>";
                    else if (d.InsComName.Trim() != comName)
                        d.ErrorMessage += "Column B: ชื่อบริษัทไม่ตรงกับที่เลือกจาก DropdownList<br>";
                    else
                    {
                        var insCom = comlst.Where(kk => kk.slm_InsNameTh == d.InsComName /*&& kk.is_Deleted == false*/).FirstOrDefault();
                        if (insCom == null)
                        {
                            d.Ins_Com_Id = null;
                            d.ErrorMessage += "Column B: ชื่อบริษัทไม่ถูกต้อง<br>";
                        }
                        else
                        {
                            d.Ins_Com_Id = insCom.slm_Ins_Com_Id;
                        }
                    }

                    d.ErrorMessage += d.InsType == "" ? "Column C : กรุณาระบุประเภทประกันภัย<br>"
                        : (d.InsType.Trim() == "ประกันภัยรถยนต์" || d.InsType == "พรบ." ? "" : "Column C : มีค่าเป็น \"ประกันภัยรถยนต์\" หรือ \"พรบ.\" ได้เท่านั้น<br>");
                    d.ErrorMessage += d.Contract_Number == "" ? "Column D : กรุณาระบุเลขสัญญา<br>" : "";
                    d.ErrorMessage += d.Name == "" ? "Column E : กรุณาระบุชื่อ-สกุล ลูกค้า<br>" : "";

                    var sale = stafflst.Where(kk => kk.slm_StaffNameTH != null && kk.slm_StaffNameTH.Replace("   ", " ").Replace("  ", " ") == d.TelesaleName.Trim().Replace("   ", " ").Replace("  ", " ") && kk.is_Deleted == 0).FirstOrDefault();
                    if (sale == null || d.TelesaleName.ToString().Trim() == "")
                    {
                        d.ErrorMessage += "Column F : ไม่พบข้อมูล Tele sale ในระบบ<br>";
                        d.TelesaleName = "";
                    }
                    else
                    {
                        d.TelesaleName = sale.slm_UserName;
                    }

                    d.ErrorMessage += d.ProblemDetail == "" ? "Column G : กรุณาระบุปัญหา<br>" : "";
                    d.ErrorMessage += d.CauseDetail == "" ? "Column H : กรุณาระบุสาเหตุ<br>" : "";

                    d.hasError = d.ErrorMessage.Length > 0;

                    string[] statusExclude = new string[] { "08", "09", "10" };
                    var contract = slmdb.kkslm_tr_renewinsurance
                                        .Join(slmdb.kkslm_tr_lead, tr => tr.slm_TicketId, lead => lead.slm_ticketId, (tr, lead) => new { Tr = tr, Lead = lead })
                                        .Where(ticket => ticket.Tr.slm_ContractNo == d.Contract_Number
                                                //&& (ticket.Tr.slm_ActNo == null || ticket.Tr.slm_ActNo == "")
                                                && ticket.Lead.is_Deleted == 0m
                                                && statusExclude.Contains(ticket.Lead.slm_Status) == false
                                                )
                                        .Select(t => new { Tr = t.Tr, Lead = t.Lead })
                                        .ToList();
                    if (contract != null && contract.Count > 0
                        && contract.Select(c => c.Lead.slm_ticketId).Distinct().Count() > 1)
                    {
                        d.hasError = true;
                        throw new Exception("ไม่สามารถนำเข้าข้อมูลติดปัญหาได้ เนื่องจากพบข้อมูล Ticket Id มากกว่า 1 รายการ ที่ Contract No : " + d.Contract_Number);
                    }

                    var ticketid = (from lead in slmdb.kkslm_tr_lead
                                    join ins in slmdb.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
                                    where lead.is_Deleted == 0 && ins.slm_ContractNo == d.Contract_Number
                                    && statusExclude.Contains(lead.slm_Status) == false
                                    select lead.slm_ticketId).FirstOrDefault();
                    if (ticketid == null)
                    {
                        d.hasError = true;
                        d.ErrorMessage += "ไม่พบข้อมูล Ticket Id ในระบบ<br>";
                    }
                    else
                        d.TicketId = ticketid;

                });
            }
            return problemD;
        }



        public static void SaveProblemData(SlmProblemData data)
        {
            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                kkslm_tr_problem problem = new kkslm_tr_problem()
                {
                    is_Deleted = data.Deleted,
                    slm_CreatedBy = data.UpdatedBy,
                    slm_CreatedDate = data.UpdatedDate,
                    slm_FileName = data.FileName,
                    slm_Ins_Com_Id = data.Ins_Com_Id,
                    slm_UpdatedBy = data.UpdatedBy,
                    slm_UpdatedDate = data.UpdatedDate
                };
                slmdb.kkslm_tr_problem.AddObject(problem);
                slmdb.SaveChanges();

                List<kkslm_tr_problemdetail> detailList = new List<kkslm_tr_problemdetail>();

                try
                {
                    data.ProblemId = problem.slm_ProblemId;
                    bool hasProblemPolicy = false;
                    bool hasProblemAct = false;

                    var staff = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == data.UpdatedBy);
                    var users = slmdb.kkslm_ms_staff.Select(p => new { slm_UserName = p.slm_UserName, slm_Position_id = p.slm_Position_id }).ToList();

                    if (data.ProblemDetails != null && data.ProblemDetails.Count > 0)
                    {
                        DateTime curDate = DateTime.Now;
                        List<string> insertedActivityTicket = new List<string>();
                        Dictionary<DateTime, string> monthTicketPolicy = new Dictionary<DateTime, string>();
                        Dictionary<DateTime, string> monthTicketAct = new Dictionary<DateTime, string>();
                        //string ticketIdPhonecallProcessed = "";
                        List<string> ticketIdPhonecallProcessed = new List<string>();
                        List<kkslm_phone_call> phoneCallToInsert = new List<kkslm_phone_call>();

                        foreach (var kk in data.ProblemDetails)
                        {
                            bool isPolicyIssue = false;
                            bool isActIssue = false;
                            DateTime startMonth = kk.ProblemDate.Value.AddDays(-kk.ProblemDate.Value.Day + 1);
                            if (kk.InsType.Contains("ประกัน"))
                            {
                                hasProblemPolicy = true;
                                if (!monthTicketPolicy.ContainsKey(startMonth))
                                    monthTicketPolicy.Add(startMonth, "");
                                if (!string.IsNullOrEmpty(kk.TicketId))
                                    monthTicketPolicy[startMonth] += string.Format("{0},", kk.TicketId);
                                isPolicyIssue = true;
                            }
                            else
                            {
                                hasProblemAct = true;
                                if (!monthTicketAct.ContainsKey(startMonth))
                                    monthTicketAct.Add(startMonth, "");
                                if (!string.IsNullOrEmpty(kk.TicketId))
                                    monthTicketAct[startMonth] += string.Format("{0},", kk.TicketId);
                                isActIssue = true;
                            }

                            #region ProblemDetails
                            kkslm_tr_problemdetail detail = new kkslm_tr_problemdetail
                            {
                                is_Deleted = false,
                                slm_ticketId = kk.TicketId,
                                slm_CauseDetail = kk.CauseDetail,
                                slm_Contract_Number = kk.Contract_Number,
                                slm_CreatedBy = kk.UpdatedBy,
                                slm_CreatedDate = kk.UpdatedDate,
                                slm_HeadStaff = kk.HeadStaff,
                                slm_Ins_Com_Id = kk.Ins_Com_Id,
                                slm_InsType = kk.InsType,
                                slm_IsAction = false,
                                slm_Name = kk.Name,
                                slm_Owner = kk.Owner,
                                slm_PhoneContact = kk.PhoneContact,
                                slm_ProblemDate = kk.ProblemDate,
                                slm_ProblemDetail = kk.ProblemDetail,
                                slm_ProblemId = data.ProblemId,
                                slm_Remark = kk.Remark,
                                slm_ResponseDate = kk.ResponseDate,
                                slm_ResponseDetail = kk.ResponseDetail,
                                slm_TelesaleName = kk.TelesaleName,
                                slm_UpdatedBy = kk.UpdatedBy,
                                slm_UpdatedDate = kk.UpdatedDate,
                                slm_FixTypeFlag = "1"
                            };
                            #endregion ProblemDetails
                            // freeze for activity status

                            #region Activity

                            string sql = $@"SELECT DISTINCT lead.slm_ticketId
                                            FROM kkslm_tr_renewinsurance renew WITH (NOLOCK)
                                            INNER JOIN kkslm_tr_lead lead WITH (NOLOCK) ON renew.slm_TicketId = lead.slm_ticketId
                                            WHERE renew.slm_ContractNo = '{detail.slm_Contract_Number}' 
                                            AND lead.is_Deleted = 0 AND lead.slm_Status NOT IN ('{SLMConstant.StatusCode.Cancel}','{SLMConstant.StatusCode.Reject}','{SLMConstant.StatusCode.Close}') ";

                            var list = slmdb.ExecuteStoreQuery<string>(sql).ToList();
                            if (list.Count > 1)
                            {
                                throw new Exception("ไม่สามารถนำเข้าข้อมูลติดปัญหาได้ เนื่องจากพบข้อมูล Ticket Id มากกว่า 1 รายการ ที่ Contract No : " + detail.slm_Contract_Number);
                            }
                            if (list.Count == 0)
                            {
                                throw new Exception("ไม่สามารถนำเข้าข้อมูลติดปัญหาได้");
                            }

                            string ticketId = list[0];
                            kkslm_tr_lead trLead = slmdb.kkslm_tr_lead.FirstOrDefault(p => p.slm_ticketId == ticketId);
                            if (trLead == null)
                            {
                                throw new Exception($"ไม่สามารถนำเข้าข้อมูลติดปัญหาได้ TicketId {list[0]} not found");
                            }

                            var campaign = slmdb.kkslm_ms_config_product_substatus
                                            .Where(ps => ps.is_Deleted == false && ps.slm_OptionCode == "11"
                                                            && ps.slm_SubStatusCode == "33"
                                                            && (ps.slm_CampaignId == trLead.slm_CampaignId || ps.slm_Product_Id == trLead.slm_Product_Id))
                                            .ToList();

                            kkslm_tr_activity activity = null;
                            if (insertedActivityTicket.FirstOrDefault(ia => ia == trLead.slm_ticketId) == null)
                            {
                                activity = new kkslm_tr_activity()
                                {
                                    slm_TicketId = trLead.slm_ticketId,
                                    slm_OldStatus = trLead.slm_Status,
                                    slm_OldSubStatus = trLead.slm_SubStatus,
                                    slm_NewStatus = "11",
                                    slm_NewSubStatus = "33",
                                    slm_CreatedBy = data.UpdatedBy,
                                    slm_CreatedBy_Position = staff == null ? null : staff.slm_Position_id,
                                    slm_CreatedDate = DateTime.Now,
                                    slm_Type = SLM.Resource.SLMConstant.ActionType.ChangeStatus,
                                    slm_SystemAction = "SLM",
                                    slm_SystemActionBy = "SLM",
                                    slm_ExternalSystem_Old = trLead.slm_ExternalSystem,
                                    slm_ExternalStatus_Old = trLead.slm_ExternalStatus,
                                    slm_ExternalSubStatus_Old = trLead.slm_ExternalSubStatus,
                                    slm_ExternalSubStatusDesc_Old = trLead.slm_ExternalSubStatusDesc
                                };

                                if (campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId) != null)
                                {
                                    activity.slm_ExternalSubStatusDesc_New = campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId).slm_SubStatusName;
                                }
                                else if (campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id) != null)
                                {
                                    activity.slm_ExternalSubStatusDesc_New = campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id).slm_SubStatusName;
                                }
                                else
                                {
                                    activity.slm_ExternalSubStatusDesc_New = null;
                                }

                                slmdb.kkslm_tr_activity.AddObject(activity);
                                insertedActivityTicket.Add(trLead.slm_ticketId);
                            }
                            #endregion Activity

                            #region update tr_lead

                            trLead.slm_ExternalSystem = null;
                            trLead.slm_ExternalStatus = null;
                            trLead.slm_ExternalSubStatus = null;
                            trLead.slm_Status = "11";
                            trLead.slm_SubStatus = "33";
                            trLead.slm_StatusBy = data.UpdatedBy;// "SYSTEM";
                            trLead.slm_StatusDate = DateTime.Now;
                            trLead.slm_Counting = 0;
                            trLead.slm_UpdatedBy = data.UpdatedBy;// "SYSTEM";
                            trLead.slm_UpdatedDate = DateTime.Now;

                            if (campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId) != null)
                            {
                                trLead.slm_ExternalSubStatusDesc = campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId).slm_SubStatusName;
                            }
                            else if (campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id) != null)
                            {
                                trLead.slm_ExternalSubStatusDesc = campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id).slm_SubStatusName;
                            }
                            else
                            {
                                trLead.slm_ExternalSubStatusDesc = null;
                            }

                            #endregion update tr_lead
                            
                            // CalTotalSLA
                            using (RenewInsureBiz calcSlaBiz = new RenewInsureBiz())
                            {
                                calcSlaBiz.CalculateTotalSLA(slmdb, trLead, activity, DateTime.Now);
                            }

                            //var renewIns = contract[0];
                            #region Archive data to sent to CAR system
                            if (isPolicyIssue || isActIssue)
                            {
                                //var prelead = slmdb.kkslm_tr_prelead.Where(p => p.slm_TicketId == trLead.slm_ticketId).FirstOrDefault();
                                var renewIns = slmdb.kkslm_tr_renewinsurance.Where(p => p.slm_TicketId == trLead.slm_ticketId).FirstOrDefault();
                                if (renewIns == null)
                                {
                                    renewIns = new kkslm_tr_renewinsurance();
                                }

                                //kk.preleadId = prelead == null ? (decimal?)null : prelead.slm_Prelead_Id;
                                kk.RenewInsureId = renewIns.slm_RenewInsureId;
                                kk.ReceiveNo = renewIns.slm_ReceiveNo;
                                kk.ActSendDate = renewIns.slm_ActSendDate;
                                kk.IncentiveFlag = renewIns.slm_IncentiveFlag;
                                kk.PolicyGrossPremiumTotal = renewIns.slm_PolicyGrossPremiumTotal;
                                kk.ActIncentiveFlag = renewIns.slm_ActIncentiveFlag;
                                kk.ActGrossPremium = renewIns.slm_ActGrossPremium;
                                kk.PolicyRecAmt = ActivityLeadBiz.GetRecAmount(renewIns.slm_TicketId, "204");
                                kk.ActRecAmt = ActivityLeadBiz.GetRecAmount(renewIns.slm_TicketId, "205");

                                bool InsComp = slmdb.kkslm_tr_renewinsurance_compare.Any(p => p.slm_RenewInsureId == renewIns.slm_RenewInsureId && p.slm_Selected == true);
                                bool ActComp = slmdb.kkslm_tr_renewinsurance_compare_act.Any(p => p.slm_RenewInsureId == renewIns.slm_RenewInsureId && p.slm_ActPurchaseFlag == true);

                                kk.IsActPurchase = ActComp;
                                kk.IsPolicyPurchase = InsComp;
                            }
                            #endregion Archive data to sent to CAR system

                            #region archive phonecall to insert kkslm_phone_call // บันทึกผลการติดต่อ
                            //var owner = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == trLead.slm_Owner);

                            var owner = users.FirstOrDefault(s => s.slm_UserName == trLead.slm_Owner);
                            if (!ticketIdPhonecallProcessed.Contains(trLead.slm_ticketId))
                            {
                                kkslm_phone_call phone = new kkslm_phone_call()
                                {
                                    slm_TicketId = trLead.slm_ticketId,
                                    slm_ContactDetail = ActivityLeadBiz.GetPhonecallContent(trLead.slm_ticketId),
                                    slm_Status = trLead.slm_Status,
                                    slm_SubStatus = trLead.slm_SubStatus,
                                    slm_SubStatusName = trLead.slm_ExternalSubStatusDesc,            
                                    slm_Owner = trLead.slm_Owner,
                                    slm_Owner_Position = owner != null ? owner.slm_Position_id : null,
                                    slm_CreateDate = data.UpdatedDate,
                                    slm_CreateBy = data.UpdatedBy,
                                    slm_CreatedBy_Position = staff.slm_Position_id,
                                    is_Deleted = 0,
                                    slm_CAS_Flag = null
                                };
                                // slmdb.kkslm_phone_call.AddObject(phone);
                                phoneCallToInsert.Add(phone);
                                ticketIdPhonecallProcessed.Add(trLead.slm_ticketId);
                            }
                            #endregion

                            slmdb.kkslm_tr_problemdetail.AddObject(detail);
                            detailList.Add(detail);
                            slmdb.SaveChanges();
                        }

                        List<string> revertInsActInc = new List<string>();

                        // update IncentiveFlag
                        monthTicketPolicy.ToList().ForEach(mt =>
                        {
                            if (hasProblemPolicy)
                            {
                                string updateIncentiveCmd = @"update kkslm_tr_renewinsurance
                        set slm_IncentiveDate = null, slm_IncentiveFlag = null, slm_PolicyComBath = null
                         , slm_PolicyComBathVat = null, slm_PolicyComBathIncentive = null
                         , slm_PolicyOV1Bath = null, slm_PolicyOV1BathIncentive = null, slm_PolicyOV1BathVat = null
                         , slm_PolicyOV2Bath = null, slm_PolicyOV2BathIncentive = null, slm_PolicyOV2BathVat = null
                         , slm_PolicyIncentiveAmount = null, slm_IncentiveCancelDate = GETDATE(), slm_PolicyReferenceNote = null
                            , slm_UpdatedBy = '{3}', slm_UpdatedDate = GETDATE()
                        where slm_TicketId in ({0}) 
                         and slm_IncentiveDate is not null 
                            and slm_IncentiveDate between '{1}' and '{2}' ";

                                updateIncentiveCmd = string.Format(updateIncentiveCmd
                                    , mt.Value.TrimEnd(',')
                                    , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
                                    , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
                                    , data.UpdatedBy);

                                slmdb.ExecuteStoreCommand(updateIncentiveCmd);

                                string selectRevertInct = @"select distinct slm_ticketID 
                                            from kkslm_tr_renewinsurance
                                            where slm_TicketId in ({0}) 
                                                and slm_IncentiveCancelDate is not null and slm_IncentiveCancelDate between dateadd(mi, -1, GETDATE()) and GETDATE() ";
                                selectRevertInct = string.Format(selectRevertInct
                                    , mt.Value.TrimEnd(',')
                                    , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
                                    , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
                                    );

                                var revertInc = slmdb.ExecuteStoreQuery<string>(selectRevertInct);

                                revertInc.ToList().ForEach(t =>
                                {
                                    var revertIncentivePolicy = new kkslm_tr_activity()
                                    {
                                        slm_TicketId = t,
                                        slm_CreatedBy = data.UpdatedBy,
                                        slm_CreatedBy_Position = staff.slm_Position_id,
                                        slm_CreatedDate = curDate,// DateTime.Now,
                                        slm_Type = SLM.Resource.SLMConstant.ActionType.RevertIncentiveIns, // "16", //   (RevertRevert Incentive INS.)
                                        slm_SystemAction = "SLM",
                                        slm_SystemActionBy = "SLM",
                                        slm_UpdatedBy = data.UpdatedBy,
                                        slm_UpdatedDate = curDate,// DateTime.Now
                                    };
                                    slmdb.kkslm_tr_activity.AddObject(revertIncentivePolicy);

                                    revertInsActInc.Add(t.ToString());
                                    data.ProblemDetails.FirstOrDefault(d => d.TicketId == t.ToString()).needCARprocess = true;
                                });
                                slmdb.SaveChanges();
                            }
                        });

                        monthTicketAct.ToList().ForEach(mt =>
                        {
                            if (hasProblemAct)
                            {
                                string updateActIncentiveCmd = @"update kkslm_tr_renewinsurance
                        set slm_ActIncentiveDate = null, slm_ActIncentiveFlag = null, slm_ActComBath = null
                         , slm_ActComBathVat = null, slm_ActComBathIncentive = null
                         , slm_ActOV1Bath = null, slm_ActOV1BathIncentive = null, slm_ActOV1BathVat = null
                         , slm_ActOV2Bath = null, slm_ActOV2BathIncentive = null, slm_ActOV2BathVat = null
                         , slm_ActIncentiveAmount = null, slm_ActIncentiveCancelDate = GETDATE()
                            , slm_ActReferenceNote = null
                            , slm_UpdatedBy = '{3}', slm_UpdatedDate = GETDATE()
                        where slm_TicketId in ({0}) 
                         and slm_ActIncentiveDate is not null 
                            and slm_ActIncentiveDate between '{1}' and '{2}' ";

                                updateActIncentiveCmd = string.Format(updateActIncentiveCmd
                                    , mt.Value.TrimEnd(',')
                                    , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
                                    , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
                                    , data.UpdatedBy);

                                slmdb.ExecuteStoreCommand(updateActIncentiveCmd);

                                string selectRevertActInct = @"select distinct slm_ticketID 
                                            from kkslm_tr_renewinsurance
                                            where slm_TicketId in ({0})                                               
                                                and slm_ActIncentiveCancelDate is not null and  slm_ActIncentiveCancelDate between dateadd(mi, -1, GETDATE()) and GETDATE() ";
                                selectRevertActInct = string.Format(selectRevertActInct
                                    , mt.Value.TrimEnd(',')
                                    , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
                                    , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
                                    , data.UpdatedBy);

                                var revertActInc = slmdb.ExecuteStoreQuery<string>(selectRevertActInct);

                                revertActInc.ToList().ForEach(t =>
                                {
                                    var revertIncentiveAct = new kkslm_tr_activity()
                                    {
                                        slm_TicketId = t,
                                        slm_CreatedBy = data.UpdatedBy,
                                        slm_CreatedBy_Position = staff.slm_Position_id,
                                        slm_CreatedDate = DateTime.Now,
                                        slm_Type = SLM.Resource.SLMConstant.ActionType.RevertIncentiveAct,// "17", //   (RevertRevert Incentive Act)
                                        slm_SystemAction = "SLM",
                                        slm_SystemActionBy = "SLM",
                                        slm_UpdatedBy = data.UpdatedBy,
                                        slm_UpdatedDate = DateTime.Now
                                    };
                                    slmdb.kkslm_tr_activity.AddObject(revertIncentiveAct);
                                    revertInsActInc.Add(t.ToString());
                                    data.ProblemDetails.FirstOrDefault(d => d.TicketId == t.ToString()).needCARprocess = true;
                                });
                                slmdb.SaveChanges();
                            }
                        });

                        revertInsActInc.ForEach(t =>
                        {
                            var pc = phoneCallToInsert.FirstOrDefault(p => p.slm_TicketId == t);
                            if (pc != null)
                            {
                                pc.slm_ContactDetail = ActivityLeadBiz.GetPhonecallContent(t);
                                slmdb.kkslm_phone_call.AddObject(pc);
                            }
                            slmdb.SaveChanges();
                            phoneCallToInsert.RemoveAll(p => p.slm_TicketId == t);
                        });
                        slmdb.SaveChanges();

                    }
                }
                catch (Exception ex)
                {
                    // ลบ detail ทั้งหมดที่เคยเอาเข้า
                    detailList.ForEach(d => slmdb.kkslm_tr_problemdetail.DeleteObject(d));

                    // ถ้าไม่มีการนำเข้า เซท is delete = 0 เพื่อจะได้ไม่เชคซ้ำในการโหลดครั้งต่อไป
                    problem.is_Deleted = true;
                    slmdb.SaveChanges();
                    throw ex;
                }
            }
        }

        //public static void SaveProblemData(SlmProblemData data)
        //{
        //    //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
        //    //{
        //    using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
        //    using (RenewInsureBiz calcSlaBiz = new RenewInsureBiz())
        //    {


        //        kkslm_tr_problem problem = new kkslm_tr_problem()
        //        {
        //            is_Deleted = data.Deleted,
        //            slm_CreatedBy = data.UpdatedBy,
        //            slm_CreatedDate = data.UpdatedDate,
        //            slm_FileName = data.FileName,
        //            slm_Ins_Com_Id = data.Ins_Com_Id,
        //            //    slm_ProblemId = problemId,
        //            slm_UpdatedBy = data.UpdatedBy,
        //            slm_UpdatedDate = data.UpdatedDate
        //        };
        //        slmdb.kkslm_tr_problem.AddObject(problem);
        //        slmdb.SaveChanges();

        //        List<kkslm_tr_problemdetail> detailList = new List<kkslm_tr_problemdetail>();

        //        try
        //        {
        //            var last = slmdb.kkslm_tr_problem.OrderByDescending(kk => kk.slm_ProblemId).FirstOrDefault();
        //            data.ProblemId = last.slm_ProblemId;
        //            bool hasProblemPolicy = false;
        //            bool hasProblemAct = false;

        //            var staff = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == data.UpdatedBy);

        //            if (data.ProblemDetails != null && data.ProblemDetails.Count > 0)
        //            {
        //                DateTime curDate = DateTime.Now;
        //                List<string> insertedActivityTicket = new List<string>();
        //                Dictionary<DateTime, string> monthTicketPolicy = new Dictionary<DateTime, string>();
        //                Dictionary<DateTime, string> monthTicketAct = new Dictionary<DateTime, string>();
        //                string ticketIdPhonecallProcessed = "";
        //                List<kkslm_phone_call> phoneCallToInsert = new List<kkslm_phone_call>();

        //                data.ProblemDetails.ForEach(kk =>
        //                {
        //                    bool isPolicyIssue = false;
        //                    bool isActIssue = false;
        //                    DateTime startMonth = kk.ProblemDate.Value.AddDays(-kk.ProblemDate.Value.Day + 1);
        //                    if (kk.InsType.Contains("ประกัน"))
        //                    {
        //                        hasProblemPolicy = true;
        //                        if (!monthTicketPolicy.ContainsKey(startMonth))
        //                            monthTicketPolicy.Add(startMonth, "");
        //                        if (!string.IsNullOrEmpty(kk.TicketId))
        //                            monthTicketPolicy[startMonth] += string.Format("{0},", kk.TicketId);
        //                        isPolicyIssue = true;
        //                    }
        //                    else
        //                    {
        //                        hasProblemAct = true;
        //                        if (!monthTicketAct.ContainsKey(startMonth))
        //                            monthTicketAct.Add(startMonth, "");
        //                        if (!string.IsNullOrEmpty(kk.TicketId))
        //                            monthTicketAct[startMonth] += string.Format("{0},", kk.TicketId);
        //                        isActIssue = true;
        //                    }
        //                    //var ticketid = (from lead in slmdb.kkslm_tr_lead
        //                    //                join ins in slmdb.kkslm_tr_renewinsurance on lead.slm_ticketId equals ins.slm_TicketId
        //                    //                where lead.is_Deleted == 0 && ins.slm_ContractNo == kk.Contract_Number
        //                    //                select lead.slm_ticketId).FirstOrDefault();

        //                    //if (kk.TicketId)
        //                    //{
        //                    //}

        //                    // sbTicketIds.AppendFormat("{0},", ticketid);
        //                    #region ProblemDetails
        //                    kkslm_tr_problemdetail detail = new kkslm_tr_problemdetail()
        //                    {
        //                        is_Deleted = false,
        //                        slm_ticketId = kk.TicketId,
        //                        slm_CauseDetail = kk.CauseDetail,
        //                        slm_Contract_Number = kk.Contract_Number,
        //                        slm_CreatedBy = kk.UpdatedBy,
        //                        slm_CreatedDate = kk.UpdatedDate,
        //                        slm_HeadStaff = kk.HeadStaff,
        //                        slm_Ins_Com_Id = kk.Ins_Com_Id,
        //                        slm_InsType = kk.InsType,
        //                        slm_IsAction = false,
        //                        slm_Name = kk.Name,
        //                        slm_Owner = kk.Owner,
        //                        slm_PhoneContact = kk.PhoneContact,
        //                        slm_ProblemDate = kk.ProblemDate,
        //                        slm_ProblemDetail = kk.ProblemDetail,
        //                        // slm_ProblemDetailId = kk.ProblemDetailId + lastDetailId,
        //                        slm_ProblemId = data.ProblemId,
        //                        slm_Remark = kk.Remark,
        //                        slm_ResponseDate = kk.ResponseDate,
        //                        slm_ResponseDetail = kk.ResponseDetail,
        //                        slm_TelesaleName = kk.TelesaleName,
        //                        slm_UpdatedBy = kk.UpdatedBy,
        //                        slm_UpdatedDate = kk.UpdatedDate,
        //                        slm_FixTypeFlag = "1"
        //                    };
        //                    #endregion ProblemDetails
        //                    // freeze for activity status

        //                    #region Activity
        //                    var contract = slmdb.kkslm_tr_renewinsurance
        //                                    .Join(slmdb.kkslm_tr_lead
        //                                        , tr => tr.slm_TicketId
        //                                        , lead => lead.slm_ticketId
        //                                        , (tr, lead) => new { Tr = tr, Lead = lead })
        //                                    .Join(slmdb.kkslm_ms_option.Where(op => op.slm_OptionType == "lead status")
        //                                        , tr => tr.Lead.slm_Status
        //                                        , o => o.slm_OptionCode
        //                                        , (tr, opt) => new { Tr = tr.Tr, Lead = tr.Lead, Status = opt })
        //                                    .GroupJoin(slmdb.kkslm_tr_prelead
        //                                        , tr => tr.Tr.slm_TicketId
        //                                        , prelead => prelead.slm_TicketId
        //                                        , (tr, prelead) => new { Tr = tr, Prelead = prelead, Status = tr.Status })
        //                                    .SelectMany(tx => tx.Prelead.DefaultIfEmpty()
        //                                        , (t, x) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Status, Prelead = x })
        //                                    .GroupJoin(slmdb.kkslm_tr_renewinsurance_compare
        //                                        , tr => tr.Tr.slm_RenewInsureId
        //                                        , comp => comp.slm_RenewInsureId
        //                                        , (tr, comp) => new { Tr = tr, InsCompare = comp.Where(c => c.slm_Selected == true) })
        //                                    .SelectMany(tx => tx.InsCompare.DefaultIfEmpty()
        //                                        , (t, x) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Tr.Status, Prelead = t.Tr.Prelead, InsComp = x })
        //                                    .GroupJoin(slmdb.kkslm_tr_renewinsurance_compare_act.Where(a => a.slm_ActPurchaseFlag == true)
        //                                        , tr => tr.Tr.slm_RenewInsureId
        //                                        , a => a.slm_RenewInsureId
        //                                        , (tr, a) => new { Tr = tr, act = a })
        //                                    .SelectMany(tx => tx.act.DefaultIfEmpty()
        //                                        , (t, x) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Tr.Status, Prelead = t.Tr.Prelead, InsComp = t.Tr.InsComp, ActComp = x })
        //                                    //.GroupJoin(slmdb.CMT_MAPPING_PRODUCT
        //                                    //    , tr => tr.Lead.slm_Product_Id
        //                                    //    , p => p.sub_product_id
        //                                    //    , (tr, p) => new { Tr = tr, Product = p })
        //                                    //.SelectMany(tx => tx.Product.DefaultIfEmpty()
        //                                    //    , (t, p) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Tr.Status, Prelead = t.Tr.Prelead, CardType = t.Tr.CardType, Product = p })
        //                                    //.GroupJoin(slmdb.CMT_MS_PRODUCT_GROUP
        //                                    //    , tr => tr.Product.product_id
        //                                    //    , pg => pg.product_id
        //                                    //    , (tr, pg) => new { Tr = tr, ProductGroup = pg })
        //                                    //.SelectMany(tx => tx.ProductGroup.DefaultIfEmpty()
        //                                    //    , (t, p) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Tr.Status, Prelead = t.Tr.Prelead, CardType = t.Tr.CardType, ProductGroup = p })
        //                                    //.GroupJoin(slmdb.kkslm_tr_cusinfo
        //                                    //    , tr => tr.Tr.slm_TicketId
        //                                    //    , cusinfo => cusinfo.slm_TicketId
        //                                    //    , (tr, cusinfo) => new { Tr = tr, CustInfo = cusinfo })
        //                                    //.SelectMany(tx => tx.CustInfo.DefaultIfEmpty()
        //                                    //    , (t, c) => new { Tr = t.Tr.Tr, Lead = t.Tr.Lead, Status = t.Tr.Status, Prelead = t.Tr.Prelead, CardType = t.Tr.CardType, ProductGroup = t.Tr.ProductGroup, CustInfo = c })
        //                                    .Where(ticket => ticket.Tr.slm_ContractNo == detail.slm_Contract_Number
        //                                            // && (ticket.Tr.slm_PolicyNo == null || ticket.Tr.slm_PolicyNo == "")
        //                                            //    && (ticket.Tr.slm_ActNo == null || ticket.Tr.slm_ActNo == "")
        //                                            && ticket.Lead.slm_Status != Resource.SLMConstant.StatusCode.Close
        //                                            && ticket.Lead.slm_Status != Resource.SLMConstant.StatusCode.Reject
        //                                            && ticket.Lead.slm_Status != Resource.SLMConstant.StatusCode.Cancel
        //                                            && ticket.Lead.is_Deleted == 0m
        //                                            // && (ticket.Lead.coc_Status == "08" || ticket.Lead.coc_Status == "09" || ticket.Lead.coc_Status == "10")
        //                                            )
        //                                    //.Select(t => new { Tr = t.Tr, Lead = t.Lead, Prelead = t.Prelead, CardType = t.CardType, ProductGroup = t.ProductGroup, Status = t.Status, CustInfo = t.CustInfo })
        //                                    .Select(t => new { Tr = t.Tr, Lead = t.Lead, Prelead = t.Prelead, Status = t.Status, InsComp = t.InsComp, ActComp = t.ActComp })
        //                                    .ToList();
        //                    if (contract != null && contract.Count > 0
        //                        && contract.Select(c => c.Lead.slm_ticketId).Distinct().Count() > 1)
        //                        throw new Exception("ไม่สามารถนำเข้าข้อมูลติดปัญหาได้ เนื่องจากพบข้อมูล Ticket Id มากกว่า 1 รายการ ที่ Contract No : " + detail.slm_Contract_Number);
        //                    if (contract != null && contract.Count == 0)
        //                        throw new Exception("ไม่สามารถนำเข้าข้อมูลติดปัญหาได้");


        //                    kkslm_tr_lead trLead;
        //                    trLead = (contract == null || contract.Count == 0) ? new kkslm_tr_lead() : contract[0].Lead;
        //                    var campaign = slmdb.kkslm_ms_config_product_substatus
        //                                    .Where(ps => ps.is_Deleted == false && ps.slm_OptionCode == "11"
        //                                                    && ps.slm_SubStatusCode == "33"
        //                                                    && (ps.slm_CampaignId == trLead.slm_CampaignId || ps.slm_Product_Id == trLead.slm_Product_Id))
        //                                    .ToList();
        //                    kkslm_tr_activity activity = null;
        //                    if (insertedActivityTicket.FirstOrDefault(ia => ia == trLead.slm_ticketId) == null)
        //                    {
        //                        activity = new kkslm_tr_activity()
        //                        {
        //                            slm_TicketId = trLead.slm_ticketId,
        //                            slm_OldStatus = trLead.slm_Status,
        //                            slm_OldSubStatus = trLead.slm_SubStatus,
        //                            slm_NewStatus = "11",
        //                            slm_NewSubStatus = "33",
        //                            slm_CreatedBy = data.UpdatedBy,
        //                            slm_CreatedBy_Position = staff == null ? null : staff.slm_Position_id,
        //                            slm_CreatedDate = DateTime.Now,
        //                            slm_Type = SLM.Resource.SLMConstant.ActionType.ChangeStatus,
        //                            slm_SystemAction = "SLM",
        //                            slm_SystemActionBy = "SLM",
        //                            slm_ExternalSystem_Old = trLead.slm_ExternalSystem,
        //                            slm_ExternalStatus_Old = trLead.slm_ExternalStatus,
        //                            slm_ExternalSubStatus_Old = trLead.slm_ExternalSubStatus,
        //                            slm_ExternalSubStatusDesc_Old = trLead.slm_ExternalSubStatusDesc
        //                        };

        //                        if (campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId) != null)
        //                        {
        //                            activity.slm_ExternalSubStatusDesc_New = campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId).slm_SubStatusName;
        //                        }
        //                        else if (campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id) != null)
        //                        {
        //                            activity.slm_ExternalSubStatusDesc_New = campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id).slm_SubStatusName;
        //                        }
        //                        else
        //                            activity.slm_ExternalSubStatusDesc_New = null;

        //                        slmdb.kkslm_tr_activity.AddObject(activity);
        //                        insertedActivityTicket.Add(trLead.slm_ticketId);
        //                    }
        //                    #endregion Activity

        //                    #region update tr_lead
        //                    if (trLead.slm_ticketId != null && trLead.slm_ticketId != "")
        //                    {
        //                        trLead = slmdb.kkslm_tr_lead.FirstOrDefault(t => t.slm_ticketId == trLead.slm_ticketId);
        //                        trLead.slm_ExternalSystem = null;
        //                        trLead.slm_ExternalStatus = null;
        //                        trLead.slm_ExternalSubStatus = null;
        //                        trLead.slm_Status = "11";
        //                        trLead.slm_SubStatus = "33";
        //                        trLead.slm_StatusBy = data.UpdatedBy;// "SYSTEM";
        //                        trLead.slm_StatusDate = DateTime.Now;
        //                        trLead.slm_Counting = 0;
        //                        trLead.slm_UpdatedBy = data.UpdatedBy;// "SYSTEM";
        //                        trLead.slm_UpdatedDate = DateTime.Now;
        //                        if (campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId) != null)
        //                        {
        //                            trLead.slm_ExternalSubStatusDesc = campaign.FirstOrDefault(c => c.slm_CampaignId == trLead.slm_CampaignId).slm_SubStatusName;
        //                        }
        //                        else if (campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id) != null)
        //                        {
        //                            trLead.slm_ExternalSubStatusDesc = campaign.FirstOrDefault(c => c.slm_Product_Id == trLead.slm_Product_Id).slm_SubStatusName;
        //                        }
        //                        else
        //                            trLead.slm_ExternalSubStatusDesc = null;
        //                    }
        //                    #endregion update tr_lead
        //                    // CalTotalSLA
        //                    calcSlaBiz.CalculateTotalSLA(slmdb, trLead, activity, DateTime.Now);

        //                    var renewIns = contract[0];
        //                    #region Archive data to sent to CAR system
        //                    if (isPolicyIssue || isActIssue)
        //                    {

        //                        kk.preleadId = renewIns.Prelead == null ? (decimal?)null : renewIns.Prelead.slm_Prelead_Id;
        //                        kk.RenewInsureId = renewIns.Tr.slm_RenewInsureId;
        //                        kk.ReceiveNo = renewIns.Tr.slm_ReceiveNo;
        //                        kk.ActSendDate = renewIns.Tr.slm_ActSendDate;
        //                        kk.IncentiveFlag = renewIns.Tr.slm_IncentiveFlag;
        //                        kk.PolicyGrossPremiumTotal = renewIns.Tr.slm_PolicyGrossPremiumTotal;
        //                        kk.ActIncentiveFlag = renewIns.Tr.slm_ActIncentiveFlag;
        //                        kk.ActGrossPremium = renewIns.Tr.slm_ActGrossPremium;
        //                        kk.PolicyRecAmt = ActivityLeadBiz.GetRecAmount(renewIns.Tr.slm_TicketId, "204");
        //                        kk.ActRecAmt = ActivityLeadBiz.GetRecAmount(renewIns.Tr.slm_TicketId, "205");

        //                        kk.IsActPurchase = renewIns.ActComp == null ? false : true;
        //                        kk.IsPolicyPurchase = renewIns.InsComp == null ? false : true;
        //                    }
        //                    #endregion Archive data to sent to CAR system

        //                    #region archive phonecall to insert kkslm_phone_call // บันทึกผลการติดต่อ
        //                    var owner = slmdb.kkslm_ms_staff.FirstOrDefault(s => s.slm_UserName == renewIns.Lead.slm_Owner);
        //                    if (!ticketIdPhonecallProcessed.Contains(renewIns.Lead.slm_ticketId))
        //                    {
        //                        kkslm_phone_call phone = new kkslm_phone_call()
        //                        {
        //                            slm_TicketId = renewIns.Lead.slm_ticketId,
        //                            slm_ContactDetail = ActivityLeadBiz.GetPhonecallContent(renewIns.Lead.slm_ticketId),
        //                            slm_Status = renewIns.Lead.slm_Status,
        //                            slm_SubStatus = renewIns.Lead.slm_SubStatus,
        //                            slm_SubStatusName = renewIns.Lead.slm_ExternalSubStatusDesc,            //Added By Pom 23/05/2016
        //                            slm_Owner = renewIns.Lead.slm_Owner,
        //                            slm_Owner_Position = owner.slm_Position_id,
        //                            slm_CreateDate = data.UpdatedDate,
        //                            slm_CreateBy = data.UpdatedBy,
        //                            slm_CreatedBy_Position = staff.slm_Position_id,
        //                            is_Deleted = 0,
        //                            slm_CAS_Flag = null
        //                        };
        //                        // slmdb.kkslm_phone_call.AddObject(phone);
        //                        phoneCallToInsert.Add(phone);
        //                        ticketIdPhonecallProcessed = ticketIdPhonecallProcessed + "," + renewIns.Lead.slm_ticketId;
        //                    }
        //                    #endregion

        //                    slmdb.kkslm_tr_problemdetail.AddObject(detail);
        //                    detailList.Add(detail);
        //                    slmdb.SaveChanges();
        //                });

        //                List<string> revertInsActInc = new List<string>();

        //                // update IncentiveFlag
        //                monthTicketPolicy.ToList().ForEach(mt =>
        //                {
        //                    if (hasProblemPolicy)
        //                    {
        //                        string updateIncentiveCmd = @"update kkslm_tr_renewinsurance
        //                set slm_IncentiveDate = null, slm_IncentiveFlag = null, slm_PolicyComBath = null
        //                 , slm_PolicyComBathVat = null, slm_PolicyComBathIncentive = null
        //                 , slm_PolicyOV1Bath = null, slm_PolicyOV1BathIncentive = null, slm_PolicyOV1BathVat = null
        //                 , slm_PolicyOV2Bath = null, slm_PolicyOV2BathIncentive = null, slm_PolicyOV2BathVat = null
        //                 , slm_PolicyIncentiveAmount = null, slm_IncentiveCancelDate = GETDATE(), slm_PolicyReferenceNote = null
        //                    , slm_UpdatedBy = '{3}', slm_UpdatedDate = GETDATE()
        //                where slm_TicketId in ({0}) 
        //                 and slm_IncentiveDate is not null 
        //                    and slm_IncentiveDate between '{1}' and '{2}' ";

        //                        updateIncentiveCmd = string.Format(updateIncentiveCmd
        //                            , mt.Value.TrimEnd(',')
        //                            , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
        //                            , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
        //                            , data.UpdatedBy);

        //                        slmdb.ExecuteStoreCommand(updateIncentiveCmd);

        //                        string selectRevertInct = @"select distinct slm_ticketID 
        //                                    from kkslm_tr_renewinsurance
        //                                    where slm_TicketId in ({0}) 
        //                                        and slm_IncentiveCancelDate is not null and slm_IncentiveCancelDate between dateadd(mi, -1, GETDATE()) and GETDATE() ";
        //                        selectRevertInct = string.Format(selectRevertInct
        //                            , mt.Value.TrimEnd(',')
        //                            , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
        //                            , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
        //                            );

        //                        var revertInc = slmdb.ExecuteStoreQuery<string>(selectRevertInct);

        //                        revertInc.ToList().ForEach(t =>
        //                        {
        //                            var revertIncentivePolicy = new kkslm_tr_activity()
        //                            {
        //                                slm_TicketId = t,
        //                                slm_CreatedBy = data.UpdatedBy,
        //                                slm_CreatedBy_Position = staff.slm_Position_id,
        //                                slm_CreatedDate = curDate,// DateTime.Now,
        //                                    slm_Type = SLM.Resource.SLMConstant.ActionType.RevertIncentiveIns, // "16", //   (RevertRevert Incentive INS.)
        //                                    slm_SystemAction = "SLM",
        //                                slm_SystemActionBy = "SLM",
        //                                slm_UpdatedBy = data.UpdatedBy,
        //                                slm_UpdatedDate = curDate,// DateTime.Now
        //                                };
        //                            slmdb.kkslm_tr_activity.AddObject(revertIncentivePolicy);

        //                            revertInsActInc.Add(t.ToString());
        //                            data.ProblemDetails.FirstOrDefault(d => d.TicketId == t.ToString()).needCARprocess = true;
        //                        });
        //                        slmdb.SaveChanges();
        //                    }
        //                });

        //                monthTicketAct.ToList().ForEach(mt =>
        //                {
        //                    if (hasProblemAct)
        //                    {
        //                        string updateActIncentiveCmd = @"update kkslm_tr_renewinsurance
        //                set slm_ActIncentiveDate = null, slm_ActIncentiveFlag = null, slm_ActComBath = null
        //                 , slm_ActComBathVat = null, slm_ActComBathIncentive = null
        //                 , slm_ActOV1Bath = null, slm_ActOV1BathIncentive = null, slm_ActOV1BathVat = null
        //                 , slm_ActOV2Bath = null, slm_ActOV2BathIncentive = null, slm_ActOV2BathVat = null
        //                 , slm_ActIncentiveAmount = null, slm_ActIncentiveCancelDate = GETDATE()
        //                    , slm_ActReferenceNote = null
        //                    , slm_UpdatedBy = '{3}', slm_UpdatedDate = GETDATE()
        //                where slm_TicketId in ({0}) 
        //                 and slm_ActIncentiveDate is not null 
        //                    and slm_ActIncentiveDate between '{1}' and '{2}' ";

        //                        updateActIncentiveCmd = string.Format(updateActIncentiveCmd
        //                            , mt.Value.TrimEnd(',')
        //                            , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
        //                            , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
        //                            , data.UpdatedBy);

        //                        slmdb.ExecuteStoreCommand(updateActIncentiveCmd);

        //                        string selectRevertActInct = @"select distinct slm_ticketID 
        //                                    from kkslm_tr_renewinsurance
        //                                    where slm_TicketId in ({0})                                               
        //                                        and slm_ActIncentiveCancelDate is not null and  slm_ActIncentiveCancelDate between dateadd(mi, -1, GETDATE()) and GETDATE() ";
        //                        selectRevertActInct = string.Format(selectRevertActInct
        //                            , mt.Value.TrimEnd(',')
        //                            , mt.Key.ToString("yyyy-MM-dd 00:00", CultureInfo.InvariantCulture)
        //                            , mt.Key.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:00", CultureInfo.InvariantCulture)
        //                            , data.UpdatedBy);

        //                        var revertActInc = slmdb.ExecuteStoreQuery<string>(selectRevertActInct);

        //                        revertActInc.ToList().ForEach(t =>
        //                        {
        //                            var revertIncentiveAct = new kkslm_tr_activity()
        //                            {
        //                                slm_TicketId = t,
        //                                slm_CreatedBy = data.UpdatedBy,
        //                                slm_CreatedBy_Position = staff.slm_Position_id,
        //                                slm_CreatedDate = DateTime.Now,
        //                                slm_Type = SLM.Resource.SLMConstant.ActionType.RevertIncentiveAct,// "17", //   (RevertRevert Incentive Act)
        //                                    slm_SystemAction = "SLM",
        //                                slm_SystemActionBy = "SLM",
        //                                slm_UpdatedBy = data.UpdatedBy,
        //                                slm_UpdatedDate = DateTime.Now
        //                            };
        //                            slmdb.kkslm_tr_activity.AddObject(revertIncentiveAct);
        //                            revertInsActInc.Add(t.ToString());
        //                            data.ProblemDetails.FirstOrDefault(d => d.TicketId == t.ToString()).needCARprocess = true;
        //                        });
        //                        slmdb.SaveChanges();
        //                    }
        //                });

        //                revertInsActInc.ForEach(t =>
        //                {
        //                    var pc = phoneCallToInsert.FirstOrDefault(p => p.slm_TicketId == t);
        //                    if (pc != null)
        //                    {
        //                        pc.slm_ContactDetail = ActivityLeadBiz.GetPhonecallContent(t);
        //                        slmdb.kkslm_phone_call.AddObject(pc);
        //                    }
        //                    slmdb.SaveChanges();
        //                    phoneCallToInsert.RemoveAll(p => p.slm_TicketId == t);
        //                });
        //                slmdb.SaveChanges();

        //            }
        //            //scope.Complete();
        //        }
        //        catch (Exception ex)
        //        {
        //            // ลบ detail ทั้งหมดที่เคยเอาเข้า
        //            detailList.ForEach(d => slmdb.kkslm_tr_problemdetail.DeleteObject(d));

        //            // ถ้าไม่มีการนำเข้า เซท is delete = 0 เพื่อจะได้ไม่เชคซ้ำในการโหลดครั้งต่อไป
        //            problem.is_Deleted = true;
        //            slmdb.SaveChanges();
        //            throw ex;
        //        }
        //    }

        //    //}
        //}
    }

    public class SlmScr034SearchList
    {
        public Int64 RowNum { get; set; }
        public string slm_InsNameTh { get; set; }
        public string slm_FileName { get; set; }
        public int TotalCase { get; set; }
        public int OpenCase { get; set; }
        public int CloseCase { get; set; }
    }
}
