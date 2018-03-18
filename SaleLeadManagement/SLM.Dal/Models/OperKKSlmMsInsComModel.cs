using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;

namespace SLM.Dal.Models
{
    public class OperKKSlmMsInsComModel
    {
        public List<InsuredData> GetSearchList(string product, string code, string name, string type)
        {
            try
            {
                using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                {
                    var queries = operdb.kkslm_ms_ins_com.Where(kk => kk.slm_InsType.Contains(type));
                    if (code.Trim() != string.Empty)
                        queries = queries.Where(kk => kk.slm_InsCode.Contains(code));
                    if (name.Trim() != string.Empty)
                        queries = queries.Where(kk => kk.slm_InsNameEng.Contains(name) || kk.slm_InsNameTh.Contains(name));

                    List<InsuredData> result = new List<InsuredData>();
                    queries.ToList().ForEach(kk => result.Add(convertObject(kk)));
                    return result;
                }
            }
            catch
            {
                return new List<InsuredData>();
            }
        }

        public List<InsuredData> GetSearchByComCodeList(string product, string abbName, string code, string type)
        {
            try
            {
                //Comment By Pom 25/03/2016 - field slm_Product_Id ไม่มีใน table แล้ว

                //var queries = operdb.kkslm_ms_ins_com.Where(kk => kk.slm_Product_Id.Contains(product)
                //    && kk.slm_InsABB.Contains(abbName)
                //    && (kk.slm_InsNameTh.Contains(name) || kk.slm_InsNameEng.Contains(name))
                //    && kk.slm_InsType.Contains(type))
                //    .ToList();

                using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                {
                    var queries = operdb.kkslm_ms_ins_com.Where(kk => kk.slm_InsABB.Contains(abbName)
                        && (kk.slm_InsCode.Contains(code)) // || kk.slm_InsNameEng.Contains(name))
                        && kk.slm_InsType.Contains(type)
                        && kk.is_Deleted == false)
                        .ToList();

                    if (queries == null)
                        return new List<InsuredData>();

                    List<InsuredData> result = new List<InsuredData>();
                    queries.ForEach(kk => result.Add(convertObject(kk)));
                    return result;
                }
            }
            catch
            {
                return new List<InsuredData>();
            }
        }
        public List<InsuredData> GetSearchByComNameList(string product, string abbName, string name, string type)
        {
            try
            {
                using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                {
                    var queries = operdb.kkslm_ms_ins_com.Where(kk => kk.slm_InsABB.Contains(abbName)
                        && (kk.slm_InsNameTh.Contains(name)) // || kk.slm_InsNameEng.Contains(name))
                        && kk.slm_InsType.Contains(type)
                        && kk.is_Deleted == false)
                        .ToList();

                    if (queries == null)
                        return new List<InsuredData>();

                    List<InsuredData> result = new List<InsuredData>();
                    queries.ForEach(kk => result.Add(convertObject(kk)));
                    return result;
                }
            }
            catch
            {
                return new List<InsuredData>();
            }
        }
        public InsuredData GetObjectById(decimal id)
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                var result = operdb.kkslm_ms_ins_com.Where(kk => kk.slm_Ins_Com_Id == id).FirstOrDefault();
                if (result == null)
                    return new InsuredData();
                else
                    return convertObject(result);
            }
        }
        private InsuredData convertObject(kkslm_ms_ins_com data)
        {
            InsuredData target = new InsuredData();
            target.CompanyId = data.slm_Ins_Com_Id;            
            target.is_Deleted = data.is_Deleted;
            target.AddressNo = data.slm_AddressNo;
            target.AmphurId = data.slm_AmphurId;
            target.BuildingName = data.slm_BuildingName;
            target.Floor = data.slm_Floor;
            target.InsuredAbbreviation = data.slm_InsABB;
            target.InsuredCode = data.slm_InsCode;
            target.InsuredNameEng = data.slm_InsNameEng;
            target.InsuredNameTh = data.slm_InsNameTh;
            target.InsusredTax = data.slm_InsTax;
            target.InsuredType = data.slm_InsType;
            target.Moo = data.slm_Moo;
            target.PostCode = data.slm_PostCode;
            //target.ProductId = data.slm_Product_Id;       //Comment By Pom 25/03/2016 - field slm_Product_Id ไม่มีใน table แล้ว
            target.ProvinceId = data.slm_ProvinceId;
            target.Road = data.slm_Road;
            target.Soi = data.slm_Soi;
            target.TambolId = data.slm_TambolId;
            target.Tel = data.slm_Tel;
            target.TelContact = data.slm_TelContact;
            target.UpdatedBy = data.slm_UpdatedBy;
            target.UpdatedDate = data.slm_UpdatedDate;
            if(data.slm_CreatedDate != null)
                target.CreatedDate = (DateTime)data.slm_CreatedDate;

            target.CreatedBy = data.slm_CreatedBy;

            return target;
        }

        public void InsertOrUpdate(InsuredData data)
        {
            try
            {
                using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
                {
                    var target = operdb.kkslm_ms_ins_com.FirstOrDefault(kk => kk.slm_Ins_Com_Id == data.CompanyId);
                    if (target == null)
                    {
                        target = new kkslm_ms_ins_com();
                        // get id 
                        var lastItem = operdb.kkslm_ms_ins_com.OrderByDescending(kk => kk.slm_Ins_Com_Id).FirstOrDefault();
                        target.slm_Ins_Com_Id = lastItem == null ? 1 : lastItem.slm_Ins_Com_Id + 1;

                        target.slm_CreatedBy = data.UpdatedBy;
                        target.slm_CreatedDate = data.UpdatedDate;

                        operdb.kkslm_ms_ins_com.AddObject(target);
                    }
                    else if (data.is_Deleted == false && target.is_Deleted == true)
                    {
                        // check dup code
                        if (operdb.kkslm_ms_ins_com.Where(c => c.slm_InsCode == data.InsuredCode && c.is_Deleted == false).Count() > 0)
                            throw new Exception("ไม่สามารถบันทึกข้อมูลได้ เนื่องจากข้อมูลรหัสบริษัทประกันซ้ำกับในระบบ");
                    }

                    target.is_Deleted = data.is_Deleted;
                    target.slm_AddressNo = data.AddressNo;
                    target.slm_AmphurId = data.AmphurId == -1 ? null : data.AmphurId;
                    target.slm_BuildingName = data.BuildingName;
                    target.slm_Floor = data.Floor;
                    target.slm_InsABB = data.InsuredAbbreviation;
                    target.slm_InsCode = data.InsuredCode;
                    target.slm_InsNameEng = data.InsuredNameEng;
                    target.slm_InsNameTh = data.InsuredNameTh;
                    target.slm_InsTax = data.InsusredTax;
                    target.slm_InsType = data.InsuredType;
                    target.slm_Moo = data.Moo;
                    target.slm_PostCode = data.PostCode;
                    //target.slm_Product_Id = data.ProductId;           //Comment By Pom 25/03/2016 - field slm_Product_Id ไม่มีใน table แล้ว
                    target.slm_ProvinceId = data.ProvinceId == -1 ? null : data.ProvinceId;
                    target.slm_Road = data.Road;
                    target.slm_Soi = data.Soi;
                    target.slm_TambolId = data.TambolId == -1 ? null : data.TambolId;
                    target.slm_Tel = data.Tel;
                    target.slm_TelContact = data.TelContact;
                    target.slm_UpdatedBy = data.UpdatedBy;
                    target.slm_UpdatedDate = data.UpdatedDate;

                    operdb.SaveChanges();
                }
            }
            catch
            {
                throw;
            }

        }

        public List<ControlListData> GetInsComList()
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                var list = operdb.kkslm_ms_ins_com.Where(p => p.is_Deleted == false).OrderBy(p => p.slm_InsNameTh).AsEnumerable().Select(p => new ControlListData { TextField = p.slm_InsNameTh, ValueField = p.slm_Ins_Com_Id.ToString() }).ToList();
                return list;
            }
        }

        public string GetInsComName(decimal insComId)
        {
            using (OPERDBEntities operdb = DBUtil.GetOperDbEntities())
            {
                var name = operdb.kkslm_ms_ins_com.Where(p => p.slm_Ins_Com_Id == insComId).Select(p => p.slm_InsNameTh).FirstOrDefault();
                return name != null ? name : "";
            }
        }
    }
}
