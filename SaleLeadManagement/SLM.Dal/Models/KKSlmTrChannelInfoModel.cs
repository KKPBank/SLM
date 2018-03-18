using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmTrChannelInfoModel
    {
        public void InsertData(LeadData leadData, string createByUsername, DateTime createDate)
        {
            try
            {
                kkslm_tr_channelinfo channel = new kkslm_tr_channelinfo();
                channel.slm_TicketId = leadData.TicketId;
                channel.slm_ChannelId = leadData.ChannelId;
                channel.slm_IPAddress = leadData.IPAddress;
                channel.slm_Company = leadData.Company;
                channel.slm_Branch = leadData.Branch;
                channel.slm_BranchNo = leadData.BranchNo;
                channel.slm_MachineNo = leadData.MachineNo;
                channel.slm_ClientServiceType = leadData.ClientServiceType;
                channel.slm_DocumentNo = leadData.DocumentNo;
                channel.slm_CommPaidCode = leadData.CommPaidCode;
                channel.slm_Zone = leadData.Zone;
                channel.slm_RequestBy = createByUsername;
                channel.slm_RequestDate = createDate;

                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    slmdb.kkslm_tr_channelinfo.AddObject(channel);
                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateData(LeadData leadData, string UserId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var channel = slmdb.kkslm_tr_channelinfo.Where(p => p.slm_TicketId.Equals(leadData.TicketId)).FirstOrDefault();
                    if (channel != null)
                    {
                        //channel.slm_ChannelId = leadData.ChannelId;
                        //channel.slm_IPAddress = leadData.IPAddress;
                        channel.slm_Company = leadData.Company;
                        channel.slm_Branch = leadData.Branch;
                        //channel.slm_BranchNo = leadData.BranchNo;
                        //channel.slm_MachineNo = leadData.MachineNo;
                        //channel.slm_ClientServiceType = leadData.ClientServiceType;
                        //channel.slm_DocumentNo = leadData.DocumentNo;
                        //channel.slm_CommPaidCode = leadData.CommPaidCode;
                        //channel.slm_Zone = leadData.Zone;
                        channel.slm_RequestBy = UserId;

                        slmdb.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertData(SLM_DBEntities slmdb, string ticketId, string new_ticketId, string username, DateTime createdDate, string channelId)
        {
            try
            {
                var channel = slmdb.kkslm_tr_channelinfo.Where(p => p.slm_TicketId == ticketId).FirstOrDefault();
                if (channel != null)
                {
                    kkslm_tr_channelinfo new_channel = new kkslm_tr_channelinfo()
                    {
                        slm_TicketId = new_ticketId,
                        slm_ChannelId = channelId,
                        slm_IPAddress = channel.slm_IPAddress,
                        slm_Company = channel.slm_Company,
                        slm_Branch = channel.slm_Branch,
                        slm_BranchNo = channel.slm_BranchNo,
                        slm_MachineNo = channel.slm_MachineNo,
                        slm_ClientServiceType = channel.slm_ClientServiceType,
                        slm_DocumentNo = channel.slm_DocumentNo,
                        slm_CommPaidCode = channel.slm_CommPaidCode,
                        slm_Zone = channel.slm_Zone,
                        slm_RequestBy = username,
                        slm_RequestDate = createdDate
                    };

                    slmdb.kkslm_tr_channelinfo.AddObject(new_channel);
                }
                else
                    throw new Exception("ไม่พบ Ticket Id " + ticketId + " ใน Table kkslm_tr_channelinfo");
            }
            catch
            {
                throw;
            }
        }
    }
}
