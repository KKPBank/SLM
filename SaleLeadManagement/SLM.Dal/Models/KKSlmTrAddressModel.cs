using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class KKSlmTrAddressModel
    {
        public List<AddressData> ListProvince()
        {
            string sql = @"select slm_ProvinceNameTH as ProvinceNameTh,slm_ProvinceID as ProvinceId from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_province order by slm_ProvinceNameTH";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<AddressData>(sql).ToList();
            }
        }

        public List<AddressData> ListDistinct(string provinceValue)
        {
            string sql = @"select slm_AmphurNameTH as AmphurName,slm_AmphurID as AmphurId 
                           from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_amphur a
                            inner join " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_province p on a.slm_ProvinceCode = p.slm_ProvinceCode
                           where p.slm_ProvinceId = '" + provinceValue + "'order by slm_AmphurNameTH";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<AddressData>(sql).ToList();
            }
        }

        public List<AddressData> ListTambol(string distinctValue,string provinceValue)
        {
            string sql = @"select slm_TambolNameTH as TambolNameTh,slm_TambolId as TambolId
                           from " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_province p 
                           inner join  " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_amphur a on a.slm_ProvinceCode = p.slm_ProvinceCode
                           inner join  " + SLM.Resource.SLMConstant.SLMDBName + @".dbo.kkslm_ms_tambol t on t.slm_ProvinceCode = p.slm_ProvinceCode and t.slm_AmphurCode = a.slm_AmphurCode
                           where slm_ProvinceId='" + provinceValue+"' and slm_AmphurId= '"+distinctValue+"'order by slm_TambolNameTH";

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<AddressData>(sql).ToList();
            }
        }

        public void saveAddressData(AddressData address)
        {

            kkslm_tr_prelead_address renewAddress = new kkslm_tr_prelead_address();

            //renewAddress.slm_RenewInsureId = addressData.slm_Ins_Com_Id;//ได้มาจาก preleadId
            renewAddress.slm_CmtLot = 1;
            renewAddress.slm_Customer_Key = "1001216762";
            renewAddress.slm_Address_Type = "D";
            renewAddress.slm_House_No = address.slm_House_No;
            renewAddress.slm_Moo = address.slm_Moo;
            renewAddress.slm_Building = address.slm_Building;
            renewAddress.slm_Village = address.slm_Village;
            renewAddress.slm_Soi = address.slm_Soi;
            renewAddress.slm_Street = address.slm_Street;
            renewAddress.slm_TambolId = address.slm_TambolId;
            renewAddress.slm_Amphur_Id = address.slm_Amphur_Id;
            renewAddress.slm_Province_Id = address.slm_Province_Id;
            renewAddress.slm_Zipcode = address.slm_Zipcode;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.kkslm_tr_prelead_address.AddObject(renewAddress);
                slmdb.SaveChanges();
            }
        }

        public void deletePreleadAddressData(int PreleadId)
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var sortConList = slmdb.kkslm_tr_prelead_address.Where(p => p.slm_Prelead_Id == PreleadId).ToList();
                    foreach (kkslm_tr_prelead_address obj in sortConList)
                    {
                        slmdb.kkslm_tr_prelead_address.DeleteObject(obj);
                    }

                    slmdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }

        }
        public void updateAddressData(AddressData address)
        {

            kkslm_tr_prelead_address renewAddress = new kkslm_tr_prelead_address();

            //renewAddress.slm_RenewInsureId = addressData.slm_Ins_Com_Id;//ได้มาจาก preleadId
            renewAddress.slm_CmtLot = 1;
            renewAddress.slm_Customer_Key = "1001216762";
            renewAddress.slm_Address_Type = "D";
            renewAddress.slm_House_No = address.slm_House_No;
            renewAddress.slm_Moo = address.slm_Moo;
            renewAddress.slm_Building = address.slm_Building;
            renewAddress.slm_Village = address.slm_Village;
            renewAddress.slm_Soi = address.slm_Soi;
            renewAddress.slm_Street = address.slm_Street;
            renewAddress.slm_TambolId = address.slm_TambolId;
            renewAddress.slm_Amphur_Id = address.slm_Amphur_Id;
            renewAddress.slm_Province_Id = address.slm_Province_Id;
            renewAddress.slm_Zipcode = address.slm_Zipcode;

            using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                slmdb.SaveChanges();
            }
        }
    }
}
