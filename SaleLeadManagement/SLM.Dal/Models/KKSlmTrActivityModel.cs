using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Resource;

namespace SLM.Dal.Models
{
    public class KKSlmTrActivityModel
    {
        private int? GetPositionId(string username, SLM_DBEntities slmdb)
        {
            KKSlmMsStaffModel staff = new KKSlmMsStaffModel();
            return staff.GetPositionId(username);
        }

        public void InsertData(LeadData leadData, string createByUsername, DateTime createDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = leadData.TicketId;
                    if (!string.IsNullOrEmpty(leadData.OldDelegate))
                    {
                        activity.slm_OldDelegate = leadData.OldDelegate;
                        activity.slm_OldDelegate_Position = GetPositionId(leadData.OldDelegate, slmdb);
                    }
                    activity.slm_NewDelegate = leadData.NewDelegate;
                    activity.slm_NewDelegate_Position = GetPositionId(leadData.NewDelegate, slmdb);
                    activity.slm_OldStatus = leadData.OldStatus;
                    activity.slm_NewStatus = leadData.NewStatus;
                    activity.slm_CreatedBy = createByUsername;
                    activity.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                    activity.slm_CreatedDate = createDate;
                    activity.slm_Type = leadData.Type;
                    activity.slm_SystemAction = SLM.Resource.SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLM.Resource.SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)

                    slmdb.kkslm_tr_activity.AddObject(activity);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertDataChangeOwner(LeadData leadData, string createByUsername, DateTime createDate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    kkslm_tr_activity activity = new kkslm_tr_activity();
                    activity.slm_TicketId = leadData.TicketId;
                    if (!string.IsNullOrEmpty(leadData.OldOwner2))
                    {
                        activity.slm_OldOwner = leadData.OldOwner2;
                        activity.slm_OldOwner_Position = GetPositionId(leadData.OldOwner2, slmdb);
                    }
                    activity.slm_NewOwner = leadData.NewOwner2;
                    activity.slm_NewOwner_Position = GetPositionId(leadData.NewOwner2, slmdb);
                    activity.slm_OldStatus = null;
                    activity.slm_NewStatus = null;
                    activity.slm_CreatedBy = createByUsername;
                    activity.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                    activity.slm_CreatedDate = createDate;
                    activity.slm_Type = leadData.Type2;
                    activity.slm_SystemAction = SLM.Resource.SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                    activity.slm_SystemActionBy = SLM.Resource.SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)

                    slmdb.kkslm_tr_activity.AddObject(activity);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertDataForTransfer(List<string> TicketIdList,string OldOwner,string NewOwner, string createByUsername, string OldDelegate,string NewDelegate)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    foreach (string ticketId in TicketIdList)
                    {
                        kkslm_tr_activity activity = new kkslm_tr_activity();
                        activity.slm_TicketId = ticketId;
                        if (OldOwner != "" && NewOwner != "")
                        {
                            activity.slm_OldOwner = OldOwner;
                            activity.slm_OldOwner_Position = GetPositionId(OldOwner, slmdb);
                            activity.slm_NewOwner = NewOwner;
                            activity.slm_NewOwner_Position = GetPositionId(NewOwner, slmdb);
                        }
                        if (OldDelegate != "" && NewDelegate != "")
                        {
                            activity.slm_OldDelegate = OldDelegate;
                            activity.slm_OldDelegate_Position = GetPositionId(OldDelegate, slmdb);
                            activity.slm_NewDelegate = NewDelegate;
                            activity.slm_NewDelegate_Position = GetPositionId(NewDelegate, slmdb);
                        }
                        if (OldOwner == "" && NewOwner != "")
                        {
                            activity.slm_OldOwner_Position = GetPositionId(OldOwner, slmdb);
                            activity.slm_NewOwner = NewOwner;
                            activity.slm_NewOwner_Position = GetPositionId(NewOwner, slmdb);
                        }
                        activity.slm_Type = SLMConstant.ActionType.Transfer;    //"04";
                        activity.slm_CreatedBy = createByUsername;
                        activity.slm_CreatedBy_Position = GetPositionId(createByUsername, slmdb);
                        activity.slm_CreatedDate = DateTime.Now;
                        activity.slm_SystemAction = SLM.Resource.SLMConstant.SystemName;        //System ที่เข้ามาทำ action (19/03/2015)
                        activity.slm_SystemActionBy = SLM.Resource.SLMConstant.SystemName;      //action เกิดขึ้นที่ระบบอะไร (19/03/2015)

                        slmdb.kkslm_tr_activity.AddObject(activity);
                    }
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
