using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class SlmTrChannelInfoModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrChannelInfoModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertData(string ticketId, ChannelInfo channelInfo, string channelId)
        {
            try
            {
                kkslm_tr_channelinfo channel = new kkslm_tr_channelinfo();
                channel.slm_TicketId = ticketId;
                channel.slm_ChannelId = channelId;

                if (channelInfo != null)
                {
                    if (!string.IsNullOrEmpty(channelInfo.Ipaddress)) channel.slm_IPAddress = channelInfo.Ipaddress;
                    if (!string.IsNullOrEmpty(channelInfo.Company)) channel.slm_Company = channelInfo.Company;
                    if (!string.IsNullOrEmpty(channelInfo.Branch)) channel.slm_Branch = channelInfo.Branch;        //Foreign Key
                    if (!string.IsNullOrEmpty(channelInfo.BranchNo)) channel.slm_BranchNo = channelInfo.BranchNo;
                    if (!string.IsNullOrEmpty(channelInfo.MachineNo)) channel.slm_MachineNo = channelInfo.MachineNo;
                    if (!string.IsNullOrEmpty(channelInfo.ClientServiceType)) channel.slm_ClientServiceType = channelInfo.ClientServiceType;
                    if (!string.IsNullOrEmpty(channelInfo.DocumentNo)) channel.slm_DocumentNo = channelInfo.DocumentNo;
                    if (!string.IsNullOrEmpty(channelInfo.CommPaidCode)) channel.slm_CommPaidCode = channelInfo.CommPaidCode;
                    if (!string.IsNullOrEmpty(channelInfo.Zone)) channel.slm_Zone = channelInfo.Zone;
                    if (!string.IsNullOrEmpty(channelInfo.CreateUser)) channel.slm_RequestBy = channelInfo.CreateUser;
                    if (!string.IsNullOrEmpty(channelInfo.TransId)) channel.slm_TransId = channelInfo.TransId;
                    if (channelInfo.Date.Year != 1) channel.slm_RequestDate = channelInfo.Date;
                }

                slmdb.kkslm_tr_channelinfo.AddObject(channel);
                slmdb.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        public void UpdateDataByHpaofl(string ticketId, ChannelInfo channelInfo, string channelId)
        {
            try
            {
                var channel = slmdb.kkslm_tr_channelinfo.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();
                if (channel != null)
                {
                    try
                    {
                        channel.slm_ChannelId = channelId;

                        if (channelInfo != null)
                        {
                            if (channelInfo.Ipaddress != null) channel.slm_IPAddress = channelInfo.Ipaddress;
                            if (channelInfo.Company != null) channel.slm_Company = channelInfo.Company;
                            if (!string.IsNullOrEmpty(channelInfo.Branch)) channel.slm_Branch = channelInfo.Branch;            //Foreign Key
                            if (channelInfo.BranchNo != null) channel.slm_BranchNo = channelInfo.BranchNo;
                            if (channelInfo.MachineNo != null) channel.slm_MachineNo = channelInfo.MachineNo;
                            if (channelInfo.ClientServiceType != null) channel.slm_ClientServiceType = channelInfo.ClientServiceType;
                            if (channelInfo.DocumentNo != null) channel.slm_DocumentNo = channelInfo.DocumentNo;
                            if (channelInfo.CommPaidCode != null) channel.slm_CommPaidCode = channelInfo.CommPaidCode;
                            if (channelInfo.Zone != null) channel.slm_Zone = channelInfo.Zone;
                            if (channelInfo.CreateUser != null) channel.slm_RequestBy = channelInfo.CreateUser;
                            if (channelInfo.TransId != null) channel.slm_TransId = channelInfo.TransId;
                            if (channelInfo.Date.Year != 1) channel.slm_RequestDate = channelInfo.Date;
                        }

                        slmdb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
                    }
                }
                else
                    throw new ServiceException(ApplicationResource.UPD_NO_RECORD_FOUND_CODE, ApplicationResource.UPD_NO_RECORD_FOUND_DESC);
            }
            catch (ServiceException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        public void UpdateResponseData(string ticketId, string responseCode, string responseMsg, DateTime responseDate)
        {
            try
            {
                var channel = slmdb.kkslm_tr_channelinfo.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();
                if (channel != null)
                {
                    channel.slm_ResponseCode = responseCode;
                    channel.slm_ResponseDate = responseDate;
                    channel.slm_ResponseMessage = responseMsg;

                    slmdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
