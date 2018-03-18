using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SLM.Dal;
using SLM.Resource.Data;
using SLM.Biz;
using SLM.Dal.Models;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr033Biz
    {
        string _error = "";
        public string ErrorMessage { get { return _error; } }
        public List<ControlListData> GetProductList()
        {
            return new CmtMappingProductModel().GetProductList();
        }

        public bool SaveData(string pdID, DataTable dt, out int total, out int succ, out int fail, out List<ControlListData> errLst, string userID)
        {
            errLst = new List<ControlListData>();
            succ = 0; fail = 0; total = 0;
            bool ret = true;
            try
            {
                using (SLM_DBEntities sdc = DBUtil.GetSlmDbEntities())
                {
                    using (OPERDBEntities odc = DBUtil.GetOperDbEntities())
                    {
                        var coml = odc.kkslm_ms_ins_com.Where(c => c.is_Deleted == false).ToList();
                        var cmpl = odc.kkslm_ms_campaigninsurance.Where(c => c.is_Deleted == false).ToList();
                        var covl = sdc.kkslm_ms_coveragetype.Where(c => c.is_Deleted == false).ToList();
                        var rptl = sdc.kkslm_ms_repairtype.Where(c => c.is_Deleted == false).ToList();
                        var brl = sdc.kkslm_ms_redbook_brand.ToList();
                        var mdl = sdc.kkslm_ms_redbook_model.ToList();


                        List<kkslm_ms_promotioninsurance> objlst = new List<kkslm_ms_promotioninsurance>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            bool insertcampaign = false;
                            DataRow row = dt.Rows[i];
                            try
                            {
                                var com = coml.Where(c => c.slm_InsNameTh == row[0].ToString().Trim()).FirstOrDefault();
                                var cmp = cmpl.Where(c => c.slm_CampaignName == row[1].ToString().Trim()).FirstOrDefault();
                                var rpt = rptl.Where(r => r.slm_RepairTypeName.Contains(row[10].ToString().Trim())).FirstOrDefault();
                                var br = brl.Where(b => b.slm_BrandName.ToUpper() == row[5].ToString().ToUpper().Trim()).FirstOrDefault();
                                var md = mdl.Where(m => m.slm_ModelName.ToUpper() == row[6].ToString().ToUpper().Trim()).FirstOrDefault();
                               //Edit By Nang 2016-07-17
                               // var cv = covl.Where(c => c.slm_CoverageTypeId == SLMUtil.SafeInt(row[8].ToString().Trim())).FirstOrDefault();
                                var cv = covl.Where(c => c.slm_ConverageTypeName.Replace(" ","") == row[8].ToString().Trim().Replace(" ","")).FirstOrDefault();

                                total++;
                                string err = "";
                                // verify 
                                if (row[0].ToString().Trim() == "") err = "Column A: กรุณาระบุชื่อ บจ.ประกันภัย";
                                else if (com == null) err += (err == "" ? "" : ",<br/>") + "Column A: ชื่อบริษัทไม่ถูกต้อง";
                                //2016-11-28 -> be able to import empty campaign name;
                                //if (row[1].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column B: กรุณาระบุ Campaign Name";
                                else if (cmp == null && !string.IsNullOrWhiteSpace(row[1].ToString().Trim()))
                                {
                                    if (com != null)
                                        insertcampaign = true;
                                    //else
                                    //    err += (err == "" ? "" : ",<br/>") + "Column B: ชื่อ Campaign ไม่ถูกต้อง";
                                }
                                if (row[2].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column C: กรุณาระบุ ระยะเวลาคุ้มครอง";
                                if (row[3].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column D: กรุณาระบุ Effective Date From";
                                if (!SLMUtil.CheckDate(row[3].ToString())) err += (err == "" ? "" : ",<br/>") + "Column D: รูปแบบวันที่ไม่ถูกต้อง (DD/MM/YYYY)";
                                if (row[4].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column E: กรุณาระบุ Effective Date To";
                                if (!SLMUtil.CheckDate(row[4].ToString())) err += (err == "" ? "" : ",<br/>") + "Column E: รูปแบบวันที่ไม่ถูกต้อง (DD/MM/YYYY)";
                                if (row[5].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column F: กรุณาระบุ ยี่ห้อ";
                                if (br == null) err += (err == "" ? "" : ",<br />") + "Column F: ไม่พบยี่ห้อในระบบ";
                                if (row[6].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column G: กรุณาระบุ รุ่น";
                                if (md == null) err += (err == "" ? "" : ",<br />") + "Column G: ไม่พบรุ่นในระบบ - " + row[6].ToString();
                                if (row[7].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column H: กรุณาระบุ ประเภทรถยนต์/ลักษณะ";
                                if (row[8].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column I: กรุณาระบุ ประเภทความคุ้มครอง";
                                if (cv == null) err += (err == "" ? "" : ",<br />") + "Column I: ไม่พบประเภทความคุ้มครองในระบบ";
                                if (row[9].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column J: กรุณาระบุ อายุผู้ขับขี่";
                                if (row[10].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column K: กรุณาระบุ ประเภทการซ่อม";
                                if (rpt == null) err += (err == "" ? "" : ",<br/>") + "Column K: ประเภทการซ่อมไม่ถูกต้อง";
                                if (row[11].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column L: กรุณาระบุ อายุรถ (ปี)";
                                if (row[12].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column M: กรุณาระบุ ขนาดเครื่องยนต์  / น้ำหนักบรรทุก";
                                if (row[13].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column N: กรุณาระบุ ความเสียหายต่อตัวรถยนต์ (OD)";
                                if (row[14].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column O: กรุณาระบุ กรณีรถยนต์สูญหาย/ไฟไหม้ (F&T)";
                                if (row[15].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column P: กรุณาระบุ ความรับผิดชอบส่วนแรก";
                                if (row[16].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column Q: กรุณาระบุ เบี้ยประกันภัยสุทธิ";
                                if (row[17].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column R: กรุณาระบุ อากร";
                                if (row[18].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column S: กรุณาระบุ ภาษี";
                                if (row[19].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column T: กรุณาระบุ เบี้ยรวมภาษีและอากร";
                                if (row[20].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column U: กรุณาระบุ พรบ";
                                if (row[21].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column V: กรุณาระบุ ความรับผิดชอบต่อความบาดเจ็บ/เสียชีวิต ";
                                if (row[22].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column W: กรุณาระบุ ความรับผิดชอบต่อทรัพย์สินคู่กรณี (TPPD)";
                                if (row[23].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column X: กรุณาระบุ ประกันภัยอุบัติเหตุส่วนบุคคล (ร.ย. 01) ผู้ขับขี่และผู้โดยสาร";
                                if (row[24].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column Y: กรุณาระบุ ผู้ขับขี่";
                                if (row[25].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column Z: กรุณาระบุ ผู้โดยสาร";
                                if (row[26].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column AA: กรุณาระบุ ค่ารักษาพยาบาล (ร.ย 02) ";
                                if (row[27].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column AB: กรุณาระบุ ผู้ขับขี่";
                                if (row[28].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column AC: กรุณาระบุ ผู้โดยสาร";
                                if (row[29].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column AD: กรุณาระบุ การประกันตัวผู้ขับขี่ (ร.ย.03) ";
                                //if (row[30].ToString().Trim() == "") err += (err == "" ? "" : ",<br/>") + "Column AE: กรุณาระบุ เงื่อนไขพิเศษ อื่นๆ";

                                // insert
                                if (err != "") throw new Exception(err);

                                if (insertcampaign)
                                {
                                    cmp = new kkslm_ms_campaigninsurance();
                                    cmp.slm_CampaignName = row[1].ToString().Trim();
                                    cmp.slm_CreatedBy = userID;
                                    cmp.slm_CreatedDate = DateTime.Now;
                                    odc.kkslm_ms_campaigninsurance.AddObject(cmp);
                                    odc.SaveChanges();
                                    cmpl.Add(cmp);

                                    var cpi = new kkslm_ms_mapping_campaign_insurance();
                                    cpi.slm_Product_Id = pdID;
                                    cpi.slm_Ins_Com_Id = com.slm_Ins_Com_Id;
                                    cpi.slm_CampaignInsuranceId = cmp.slm_CampaignInsuranceId;
                                    cpi.slm_CreatedBy = userID;
                                    cpi.slm_CreatedDate = DateTime.Now;
                                    cpi.is_Deleted = false;
                                    sdc.kkslm_ms_mapping_campaign_insurance.AddObject(cpi);
                                    sdc.SaveChanges();
                                }

                                kkslm_ms_promotioninsurance pr = new kkslm_ms_promotioninsurance();
                                pr.slm_Product_Id = pdID;
                                pr.slm_Ins_Com_Id = com.slm_Ins_Com_Id;
                                pr.slm_CampaignInsuranceId = cmp != null ? cmp.slm_CampaignInsuranceId : (decimal?)null;
                                pr.slm_DurationYear = SLMUtil.SafeInt(row[2].ToString().ToUpper().Replace("Y", ""));
                                pr.slm_EffectiveDateFrom = SLMUtil.GetDateFromStr(row[3].ToString());
                                pr.slm_EffectiveDateTo = SLMUtil.GetDateFromStr(row[4].ToString());
                                pr.slm_Brand_Code = br != null ? br.slm_BrandCode : row[5].ToString().ToUpper().Trim();
                                pr.slm_Model_Code = md != null ? md.slm_ModelCode : row[6].ToString().ToUpper().Trim();
                                pr.slm_UseCarType = row[7].ToString();
                                //Edit By Nang 2016-07-17
                                //pr.slm_CoverageTypeId = SLMUtil.SafeInt(row[8].ToString());
                                pr.slm_CoverageTypeId = cv != null ? cv.slm_CoverageTypeId : 0;
                                pr.slm_AgeDrivenFlag = row[9].ToString();
                                if (rpt != null) pr.slm_RepairTypeId = rpt.slm_RepairTypeId;
                                pr.slm_AgeCarYear = SLMUtil.SafeInt(row[11].ToString());
                                //pr.slm_EngineSize = SLMUtil.SafeDecimal(row[12].ToString());
                                pr.slm_EngineSize = row[12].ToString();
                                pr.slm_OD = SLMUtil.SafeDecimal(row[13].ToString());
                                pr.slm_FT = SLMUtil.SafeDecimal(row[14].ToString());
                                pr.slm_DeDuctible = row[15].ToString();
                                pr.slm_GrossPremium = SLMUtil.SafeDecimal(row[16].ToString());
                                pr.slm_Stamp = SLMUtil.SafeDecimal(row[17].ToString());
                                pr.slm_Vat = SLMUtil.SafeDecimal(row[18].ToString());
                                pr.slm_NetGrossPremium = SLMUtil.SafeDecimal(row[19].ToString());
                                pr.slm_Act = SLMUtil.SafeDecimal(row[20].ToString());
                                pr.slm_InjuryDeath = SLMUtil.SafeDecimal(row[21].ToString());
                                pr.slm_TPPD = SLMUtil.SafeDecimal(row[22].ToString());
                                pr.slm_PersonalAccident = SLMUtil.SafeDecimal(row[23].ToString());
                                pr.slm_PersonalAccidentDriver = row[24].ToString();
                                pr.slm_PersonalAccidentPassenger = row[25].ToString();
                                pr.slm_MedicalFee = SLMUtil.SafeDecimal(row[26].ToString());
                                pr.slm_MedicalFeeDriver = row[27].ToString();
                                pr.slm_MedicalFeePassenger = row[28].ToString();
                                pr.slm_InsuranceDriver = SLMUtil.SafeDecimal(row[29].ToString());
                                pr.slm_Remark = row[30].ToString();
                                pr.slm_CreatedBy = userID;
                                pr.slm_CreatedDate = DateTime.Now;
                                pr.slm_UpdatedBy = userID;
                                pr.slm_UpdatedDate = DateTime.Now;
                                pr.is_Deleted = false;

                               // objlst.Add(pr);
                                odc.kkslm_ms_promotioninsurance.AddObject(pr);
                                succ++;
                            }
                            catch (Exception ex)
                            {
                                errLst.Add(new ControlListData()
                                {
                                    ValueField = (i + 2).ToString(),
                                    TextField = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                                });
                                fail++;
                            }

                        }

                        if (errLst.Count == 0)
                        {
                            // get code
                            //var allbrand = objlst.Select(o => o.slm_Brand_Code).Distinct().ToArray();
                            //var allmodel = objlst.Select(o => o.slm_Model_Code).Distinct().ToArray();

                            //var brands = sdc.kkslm_ms_redbook_brand.Where(b => allbrand.Contains(b.slm_BrandName.ToUpper())).ToList();
                            //var models = sdc.kkslm_ms_redbook_model.Where(m => allmodel.Contains(m.slm_ModelName.ToUpper())).ToList();
                            //foreach (var obj in objlst)
                            //{
                                //var br = brands.Where(b => b.slm_BrandName.ToUpper() == obj.slm_Brand_Code).FirstOrDefault();
                                //if (br != null) obj.slm_Brand_Code = br.slm_BrandCode;

                                //var md = models.Where(m => m.slm_ModelName.ToUpper() == obj.slm_Model_Code).FirstOrDefault();
                                //if (md != null) obj.slm_Model_Code = md.slm_ModelCode;

                            //    odc.kkslm_ms_promotioninsurance.AddObject(obj);
                            //}

                            odc.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                if (_error.Contains("The duplicate key value is"))
                    _error = "ไม่สามารถนำเข้าข้อมูลได้เนื่องจากมีข้อมูลซ้ำกัน\\n\\n" +  _error.Substring(_error.IndexOf("The duplicate key value is")).Replace("\r","").Replace("\n","").Replace("The statement has been terminated.","");
                ret = false;
            }
            return ret;
        }
    }
}
