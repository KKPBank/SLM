using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;
using SLS.Resource.Data;

namespace SLS.Dal.Models
{
    public class SlmTrCusInfoModel
    {
        private SLM_DBEntities slmdb = null;

        public SlmTrCusInfoModel(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public void InsertData(string ticketId, CustomerInfo cusInfo, CustomerDetail cusDetail, ChannelInfo channelInfo)
        {
            try
            {
                MasterModel master = new MasterModel(slmdb);
                kkslm_tr_cusinfo info = new kkslm_tr_cusinfo();
                info.slm_TicketId = ticketId;

                if (cusInfo != null)
                {
                    if (!string.IsNullOrEmpty(cusInfo.Lastname)) info.slm_LastName = cusInfo.Lastname;
                    if (!string.IsNullOrEmpty(cusInfo.Email)) info.slm_Email = cusInfo.Email;
                    if (!string.IsNullOrEmpty(cusInfo.TelNo2)) info.slm_TelNo_2 = cusInfo.TelNo2;
                    if (!string.IsNullOrEmpty(cusInfo.TelNo3)) info.slm_TelNo_3 = cusInfo.TelNo3;
                    if (!string.IsNullOrEmpty(cusInfo.ExtNo2)) info.slm_Ext_2 = cusInfo.ExtNo2;
                    if (!string.IsNullOrEmpty(cusInfo.ExtNo3)) info.slm_Ext_3 = cusInfo.ExtNo3;
                    if (!string.IsNullOrEmpty(cusInfo.BuildingName)) info.slm_BuildingName = cusInfo.BuildingName;
                    if (!string.IsNullOrEmpty(cusInfo.AddrNo)) info.slm_AddressNo = cusInfo.AddrNo;
                    if (!string.IsNullOrEmpty(cusInfo.Floor)) info.slm_Floor = cusInfo.Floor;
                    if (!string.IsNullOrEmpty(cusInfo.Soi)) info.slm_Soi = cusInfo.Soi;
                    if (!string.IsNullOrEmpty(cusInfo.Street)) info.slm_Street = cusInfo.Street;
                    if (!string.IsNullOrEmpty(cusInfo.Province)) info.slm_Province = master.GetProvinceId(cusInfo.Province);    //Foreign Key
                    if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur))                       //Foreign Key
                        info.slm_Amphur = master.GetAmphurId(cusInfo.Province, cusInfo.Amphur);
                    if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur) && !string.IsNullOrEmpty(cusInfo.Tambom))  //Foreign Key
                        info.slm_Tambon = master.GetTambolId(cusInfo.Province, cusInfo.Amphur, cusInfo.Tambom);
                    if (!string.IsNullOrEmpty(cusInfo.PostalCode)) info.slm_PostalCode = cusInfo.PostalCode;
                    if (!string.IsNullOrEmpty(cusInfo.Occupation)) info.slm_Occupation = master.GetOccupationId(cusInfo.Occupation);    //Foreign Key
                    if (!string.IsNullOrEmpty(cusInfo.BaseSalary)) info.slm_BaseSalary = decimal.Parse(cusInfo.BaseSalary);
                    if (!string.IsNullOrEmpty(cusInfo.IsCustomer)) info.slm_IsCustomer = cusInfo.IsCustomer;
                    if (!string.IsNullOrEmpty(cusInfo.CustomerCode)) info.slm_CusCode = cusInfo.CustomerCode;
                    if (cusInfo.DateOfBirth.Year != 1) info.slm_Birthdate = cusInfo.DateOfBirth;
                    if (!string.IsNullOrEmpty(cusInfo.CardType)) info.slm_CardType = int.Parse(cusInfo.CardType);
                    if (!string.IsNullOrEmpty(cusInfo.Cid)) info.slm_CitizenId = cusInfo.Cid;
                    if (!string.IsNullOrEmpty(cusInfo.CountryCode)) info.slm_country_id = master.GetCountryId(cusInfo.CountryCode);
                }

                if (cusDetail != null)
                {
                    if (!string.IsNullOrEmpty(cusDetail.Topic)) info.slm_Topic = cusDetail.Topic;
                    if (!string.IsNullOrEmpty(cusDetail.Detail)) info.slm_Detail = cusDetail.Detail;
                    if (!string.IsNullOrEmpty(cusDetail.PathLink)) info.slm_PathLink = cusDetail.PathLink;
                    if (!string.IsNullOrEmpty(cusDetail.ContactBranch)) info.slm_ContactBranch = cusDetail.ContactBranch;      //Foreign Key
                }

                if (channelInfo != null)
                {
                    if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                    {
                        info.slm_CreatedBy = channelInfo.CreateUser;
                        info.slm_UpdatedBy = channelInfo.CreateUser;
                    }
                    else
                    {
                        info.slm_CreatedBy = "SYSTEM";
                        info.slm_UpdatedBy = "SYSTEM";
                    }
                }
                else
                {
                    info.slm_CreatedBy = "SYSTEM";
                    info.slm_UpdatedBy = "SYSTEM";
                }

                DateTime createDate = DateTime.Now;
                info.slm_CreatedDate = createDate;
                info.slm_UpdatedDate = createDate;

                slmdb.kkslm_tr_cusinfo.AddObject(info);
                slmdb.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_INSERT_FAIL_CODE, ApplicationResource.INS_INSERT_FAIL_DESC, ex.Message, ex.InnerException);
            }
        }

        public void UpdateData(string ticketId, CustomerInfo cusInfo, CustomerDetail cusDetail, ChannelInfo channelInfo)
        {
            MasterModel master = new MasterModel(slmdb);
            var info = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();

            if (info != null)
            {
                try
                {
                    //if (cusInfo != null)
                    //{
                        //if (!string.IsNullOrEmpty(cusInfo.Lastname)) info.slm_LastName = cusInfo.Lastname;
                        //if (!string.IsNullOrEmpty(cusInfo.Email)) info.slm_Email = cusInfo.Email;
                        //if (!string.IsNullOrEmpty(cusInfo.TelNo2)) info.slm_TelNo_2 = cusInfo.TelNo2;
                        //if (!string.IsNullOrEmpty(cusInfo.TelNo3)) info.slm_TelNo_3 = cusInfo.TelNo3;
                        //if (!string.IsNullOrEmpty(cusInfo.ExtNo2)) info.slm_Ext_2 = cusInfo.ExtNo2;
                        //if (!string.IsNullOrEmpty(cusInfo.ExtNo3)) info.slm_Ext_3 = cusInfo.ExtNo3;
                        //if (!string.IsNullOrEmpty(cusInfo.BuildingName)) info.slm_BuildingName = cusInfo.BuildingName;
                        //if (!string.IsNullOrEmpty(cusInfo.AddrNo)) info.slm_AddressNo = cusInfo.AddrNo;
                        //if (!string.IsNullOrEmpty(cusInfo.Floor)) info.slm_Floor = cusInfo.Floor;
                        //if (!string.IsNullOrEmpty(cusInfo.Soi)) info.slm_Soi = cusInfo.Soi;
                        //if (!string.IsNullOrEmpty(cusInfo.Street)) info.slm_Street = cusInfo.Street;
                        //if (!string.IsNullOrEmpty(cusInfo.Province)) info.slm_Province = master.GetProvinceId(cusInfo.Province);        //Foreign Key
                        //if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur))
                        //    info.slm_Amphur = master.GetAmphurId(cusInfo.Province, cusInfo.Amphur);                                     //Foreign Key
                        //if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur) && !string.IsNullOrEmpty(cusInfo.Tambom))
                        //    info.slm_Tambon = master.GetTambolId(cusInfo.Province, cusInfo.Amphur, cusInfo.Tambom);                     //Foreign Key
                        //if (!string.IsNullOrEmpty(cusInfo.PostalCode)) info.slm_PostalCode = cusInfo.PostalCode;
                        //if (!string.IsNullOrEmpty(cusInfo.Occupation)) info.slm_Occupation = master.GetOccupationId(cusInfo.Occupation);    //Foreign Key
                        //if (!string.IsNullOrEmpty(cusInfo.BaseSalary)) info.slm_BaseSalary = decimal.Parse(cusInfo.BaseSalary);
                        //if (!string.IsNullOrEmpty(cusInfo.IsCustomer)) info.slm_IsCustomer = cusInfo.IsCustomer;
                        //if (!string.IsNullOrEmpty(cusInfo.CustomerCode)) info.slm_CusCode = cusInfo.CustomerCode;
                        //if (cusInfo.DateOfBirth.Year != 1) info.slm_Birthdate = cusInfo.DateOfBirth;
                        //if (!string.IsNullOrEmpty(cusInfo.Cid)) info.slm_CitizenId = cusInfo.Cid;
                    //}

                    if (cusDetail != null)
                    {
                        //if (!string.IsNullOrEmpty(cusDetail.Topic)) info.slm_Topic = cusDetail.Topic;
                        //if (!string.IsNullOrEmpty(cusDetail.Detail)) info.slm_Detail = cusDetail.Detail;
                        if (!string.IsNullOrEmpty(cusDetail.PathLink)) info.slm_PathLink = cusDetail.PathLink;
                        //if (!string.IsNullOrEmpty(cusDetail.ContactBranch)) info.slm_ContactBranch = cusDetail.ContactBranch;       //Foreign Key
                    }

                    if (channelInfo != null)
                    {
                        if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                            info.slm_UpdatedBy = channelInfo.CreateUser;
                        else
                            info.slm_UpdatedBy = "SYSTEM";
                    }
                    else
                        info.slm_UpdatedBy = "SYSTEM";

                    info.slm_UpdatedDate = DateTime.Now;
                    slmdb.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw new ServiceException(ApplicationResource.UPD_UPDATE_FAIL_CODE, ApplicationResource.UPD_UPDATE_FAIL_DESC, ex.Message, ex.InnerException);
                }
            }
            else
                throw new ServiceException(ApplicationResource.UPD_NO_RECORD_FOUND_CODE, ApplicationResource.UPD_NO_RECORD_FOUND_DESC);
        }

        public void UpdateDataByHpaofl(string ticketId, CustomerInfo cusInfo, CustomerDetail cusDetail, ChannelInfo channelInfo)
        {
            try
            {
                MasterModel master = new MasterModel(slmdb);
                var info = slmdb.kkslm_tr_cusinfo.Where(p => p.slm_TicketId.Equals(ticketId)).FirstOrDefault();

                if (info != null)
                {
                    try
                    {
                        if (cusInfo != null)
                        {
                            if (cusInfo.Lastname != null) info.slm_LastName = cusInfo.Lastname;
                            if (cusInfo.Email != null) info.slm_Email = cusInfo.Email;
                            if (cusInfo.TelNo2 != null) info.slm_TelNo_2 = cusInfo.TelNo2;
                            if (cusInfo.TelNo3 != null) info.slm_TelNo_3 = cusInfo.TelNo3;
                            if (cusInfo.ExtNo2 != null) info.slm_Ext_2 = cusInfo.ExtNo2;
                            if (cusInfo.ExtNo3 != null) info.slm_Ext_3 = cusInfo.ExtNo3;
                            if (cusInfo.BuildingName != null) info.slm_BuildingName = cusInfo.BuildingName;
                            if (cusInfo.AddrNo != null) info.slm_AddressNo = cusInfo.AddrNo;
                            if (cusInfo.Floor != null) info.slm_Floor = cusInfo.Floor;
                            if (cusInfo.Soi != null) info.slm_Soi = cusInfo.Soi;
                            if (cusInfo.Street != null) info.slm_Street = cusInfo.Street;
                            if (!string.IsNullOrEmpty(cusInfo.Province)) info.slm_Province = master.GetProvinceId(cusInfo.Province);        //Foreign Key
                            if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur))
                                info.slm_Amphur = master.GetAmphurId(cusInfo.Province, cusInfo.Amphur);                                     //Foreign Key
                            if (!string.IsNullOrEmpty(cusInfo.Province) && !string.IsNullOrEmpty(cusInfo.Amphur) && !string.IsNullOrEmpty(cusInfo.Tambom))
                                info.slm_Tambon = master.GetTambolId(cusInfo.Province, cusInfo.Amphur, cusInfo.Tambom);                     //Foreign Key
                            if (cusInfo.PostalCode != null) info.slm_PostalCode = cusInfo.PostalCode;
                            if (!string.IsNullOrEmpty(cusInfo.Occupation)) info.slm_Occupation = master.GetOccupationId(cusInfo.Occupation);    //Foreign Key
                            if (!string.IsNullOrEmpty(cusInfo.BaseSalary)) info.slm_BaseSalary = decimal.Parse(cusInfo.BaseSalary);
                            if (cusInfo.IsCustomer != null) info.slm_IsCustomer = cusInfo.IsCustomer;
                            if (cusInfo.CustomerCode != null) info.slm_CusCode = cusInfo.CustomerCode;
                            if (cusInfo.DateOfBirth.Year != 1) info.slm_Birthdate = cusInfo.DateOfBirth;
                            if (!string.IsNullOrEmpty(cusInfo.CardType))
                                info.slm_CardType = int.Parse(cusInfo.CardType);
                            else
                                info.slm_CardType = null;

                            if (cusInfo.Cid != null) info.slm_CitizenId = cusInfo.Cid;
                        }

                        if (cusDetail != null)
                        {
                            if (cusDetail.Topic != null) info.slm_Topic = cusDetail.Topic;
                            if (cusDetail.Detail != null) info.slm_Detail = cusDetail.Detail;
                            if (cusDetail.PathLink != null) info.slm_PathLink = cusDetail.PathLink;
                            if (!string.IsNullOrEmpty(cusDetail.ContactBranch)) info.slm_ContactBranch = cusDetail.ContactBranch;       //Foreign Key
                        }

                        if (channelInfo != null)
                        {
                            if (!string.IsNullOrEmpty(channelInfo.CreateUser))
                                info.slm_UpdatedBy = channelInfo.CreateUser;
                            else
                                info.slm_UpdatedBy = "SYSTEM";
                        }
                        else
                            info.slm_UpdatedBy = "SYSTEM";

                        info.slm_UpdatedDate = DateTime.Now;
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
    }
}
