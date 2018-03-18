using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SLM.Dal;
using SLM.Resource;

namespace SLM.Biz
{
    public class SlmScr064Biz
    {
        public string ErrorMessage { get; private set; }

        readonly string _sql = string.Format(@"
SELECT
    RECEIPT_POLICY.slm_TransDate,
    PRE.slm_BranchCode,
    BRANCH.slm_BranchName,
    REN.slm_ContractNo,
    ISNULL(TIL.slm_TitleName, '') + ISNULL(LEAD.slm_Name, '') + ' ' + ISNULL(LEAD.slm_LastName, '') AS CustomerName,
    INS.slm_InsNameTh,
    REN.slm_PolicyGrossPremium                                                                      AS GrossPremium,
    REN.slm_PolicyStartCoverDate                                                                    AS StartCoverDate,
    REN.slm_PolicyEndCoverDate                                                                      AS EndCoverDate,
    'ค่าเบี้ยประกันภัยปีต่ออายุ'                                                                    AS DocType,
    RECEIPT_POLICY.slm_RecBy,
    RECEIPT_POLICY.slm_RecAmount,
    OP.slm_OptionDesc,
    LEAD.slm_ExternalSubStatusDesc,
    REN.slm_ReceiveNo,
    REN.slm_ActSendDate,
    CASE WHEN REN.slm_ActIssuePlace = '1'
        THEN 'ธนาคาร'
    WHEN REN.slm_ActIssuePlace = '2'
        THEN 'บริษัทประกัน'
    ELSE '' END                                                                                        ActIssuePlace,
    ST.slm_EmpCode,
    ST.slm_StaffNameTH,
    TEAM.HeadStaffNameTH,
    TEAM.slm_TeamTelesales_Code
FROM {0}KKSLM_TR_LEAD LEAD INNER JOIN {0}kkslm_tr_renewinsurance REN
        ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
    INNER JOIN {0}kkslm_ms_option OP
        ON OP.slm_OptionCode = LEAD.slm_Status AND OP.slm_OptionType = 'lead status'
    INNER JOIN {0}kkslm_ms_staff ST
        ON st.slm_UserName = lead.slm_Owner
    INNER JOIN
    (
        SELECT DISTINCT
            RR.slm_ticketId,
            RRD.slm_TransDate,
            RRD.slm_RecBy,
            RRD.slm_RecAmount
        FROM {0}kkslm_tr_renewinsurance_receipt RR INNER JOIN {0}kkslm_tr_renewinsurance_receipt_detail RRD
                ON RRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
        WHERE RRD.slm_PaymentCode = '204'
    ) AS RECEIPT_POLICY
        ON RECEIPT_POLICY.slm_ticketId = LEAD.slm_ticketId
    LEFT JOIN {1}kkslm_ms_ins_com INS
        ON INS.slm_Ins_Com_Id = REN.slm_InsuranceComId
    LEFT JOIN {0}kkslm_tr_prelead PRE
        ON PRE.SLM_TICKETID = LEAD.slm_ticketId
    LEFT JOIN {0}kkslm_ms_branch BRANCH
        ON BRANCH.slm_BranchCode = PRE.slm_BranchCode
    LEFT JOIN {0}kkslm_ms_title TIL
        ON TIL.slm_TitleId = LEAD.slm_TitleId
    LEFT JOIN
    (
        SELECT
            TT.slm_TeamTelesales_Id,
            ST.slm_StaffNameTH AS HeadStaffNameTH,
            TT.slm_TeamTelesales_Code
        FROM {0}kkslm_ms_teamtelesales TT INNER JOIN {0}kkslm_ms_staff ST
                ON ST.slm_StaffId = TT.slm_HeadStaff
    ) AS TEAM
        ON TEAM.slm_TeamTelesales_Id = ST.slm_TeamTelesales_Id
WHERE LEAD.is_Deleted = 0 AND REN.slm_ReceiveNo IS NULL
UNION ALL
SELECT
    RECEIPT_ACT.slm_TransDate,
    PRE.slm_BranchCode,
    BRANCH.slm_BranchName,
    REN.slm_ContractNo,
    ISNULL(TIL.slm_TitleName, '') + ISNULL(LEAD.slm_Name, '') + ' ' + ISNULL(LEAD.slm_LastName, '') AS CustomerName,
    INS.slm_InsNameTh,
    REN.slm_ActGrossPremium                                                                         AS GrossPremium,
    REN.slm_ActStartCoverDate                                                                       AS StartCoverDate,
    REN.slm_ActEndCoverDate                                                                         AS EndCoverDate,
    'ค่าเบี้ยพรบ.ปีต่ออายุ'                                                                         AS DocType,
    RECEIPT_ACT.slm_RecBy,
    RECEIPT_ACT.slm_RecAmount,
    OP.slm_OptionDesc,
    LEAD.slm_ExternalSubStatusDesc,
    REN.slm_ReceiveNo,
    REN.slm_ActSendDate,
    CASE WHEN REN.slm_ActIssuePlace = '1'
        THEN 'ธนาคาร'
    WHEN REN.slm_ActIssuePlace = '2'
        THEN 'บริษัทประกัน'
    ELSE '' END                                                                                        ActIssuePlace,
    ST.slm_EmpCode,
    ST.slm_StaffNameTH,
    TEAM.HeadStaffNameTH,
    TEAM.slm_TeamTelesales_Code
FROM {0}KKSLM_TR_LEAD LEAD INNER JOIN {0}kkslm_tr_renewinsurance REN
        ON REN.SLM_TICKETID = LEAD.SLM_TICKETID
    INNER JOIN {0}kkslm_ms_option OP
        ON OP.slm_OptionCode = LEAD.slm_Status AND OP.slm_OptionType = 'lead status'
    INNER JOIN {0}kkslm_ms_staff ST
        ON st.slm_UserName = lead.slm_Owner
    INNER JOIN
    (
        SELECT DISTINCT
            RR.slm_ticketId,
            RRD.slm_TransDate,
            RRD.slm_RecBy,
            RRD.slm_RecAmount
        FROM {0}kkslm_tr_renewinsurance_receipt RR INNER JOIN {0}kkslm_tr_renewinsurance_receipt_detail RRD
                ON RRD.slm_RenewInsuranceReceiptId = RR.slm_RenewInsuranceReceiptId
        WHERE RRD.slm_PaymentCode = '205'
    ) AS RECEIPT_ACT
        ON RECEIPT_ACT.slm_ticketId = LEAD.slm_ticketId
    LEFT JOIN {1}kkslm_ms_ins_com INS
        ON INS.slm_Ins_Com_Id = REN.slm_ActComId
    LEFT JOIN {0}kkslm_tr_prelead PRE
        ON PRE.SLM_TICKETID = LEAD.slm_ticketId
    LEFT JOIN {0}kkslm_ms_branch BRANCH
        ON BRANCH.slm_BranchCode = PRE.slm_BranchCode
    LEFT JOIN {0}kkslm_ms_title TIL
        ON TIL.slm_TitleId = LEAD.slm_TitleId
    LEFT JOIN
    (
        SELECT
            TT.slm_TeamTelesales_Id,
            ST.slm_StaffNameTH AS HeadStaffNameTH,
            TT.slm_TeamTelesales_Code
        FROM {0}kkslm_ms_teamtelesales TT INNER JOIN {0}kkslm_ms_staff ST
                ON ST.slm_StaffId = TT.slm_HeadStaff
    ) AS TEAM
        ON TEAM.slm_TeamTelesales_Id = ST.slm_TeamTelesales_Id
WHERE LEAD.is_Deleted = 0 AND REN.slm_ActSendDate IS NULL
",
            SLMConstant.SLMDBName + ".dbo.",
            SLMConstant.OPERDBName + ".dbo."
        );

        public bool CreateExcelV2(string filename)
        {
            bool ret = true;
            try
            {
                using (SLM_DBEntities slmdb = new SLM_DBEntities())
                {
                    var lst = slmdb.ExecuteStoreQuery<SlmScr064SearchResult>(_sql).ToList();
                    if (lst.Count == 0) throw new Exception("ไม่พบข้อมูล");

                    List<object[]> data = new List<object[]>();
                    foreach (var itm in lst)
                    {
                        data.Add(new object[]
                        {
                            //itm.slm_TransDate.GetValueOrDefault().ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            itm.slm_TransDate != null ? itm.slm_TransDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture) : "",
                            itm.slm_BranchCode,
                            itm.slm_BranchName,
                            itm.slm_ContractNo,
                            itm.CustomerName,
                            itm.slm_InsNameTh,
                            itm.GrossPremium,
                            //itm.StartCoverDate.GetValueOrDefault().ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            //itm.EndCoverDate.GetValueOrDefault().ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            itm.StartCoverDate != null ? itm.StartCoverDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture) : "",
                            itm.EndCoverDate != null ? itm.EndCoverDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture) : "",
                            itm.DocType,
                            itm.slm_RecBy,
                            itm.slm_RecAmount,
                            itm.slm_OptionDesc,
                            itm.slm_ExternalSubStatusDesc,
                            itm.slm_ReceiveNo,
                            //itm.slm_ActSendDate.GetValueOrDefault().ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                            itm.slm_ActSendDate != null ? itm.slm_ActSendDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture) : "",
                            itm.ActIssuePlace,
                            itm.slm_EmpCode,
                            itm.slm_StaffNameTH,
                            itm.HeadStaffNameTH,
                            itm.slm_TeamTelesales_Code,
                        });
                    }

                    ExcelExportBiz ebz = new ExcelExportBiz();
                    if (!ebz.CreateExcel(filename, new List<ExcelExportBiz.SheetItem>()
                    {
                        new ExcelExportBiz.SheetItem()
                        {
                            SheetName = "REPORT_ชำระ ประกันและ พรบ#",
                            RowPerSheet = SLMConstant.ExcelRowPerSheet,
                            Data = data,
                            Columns = new List<ExcelExportBiz.ColumnItem>()
                            {
                                new ExcelExportBiz.ColumnItem() {ColumnName = "วันที่รับชำระ ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "รหัสสาขาที่สร้างสัญญา (HP) ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "สาขาที่สร้างสัญญา (HP) ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "เลขที่สัญญา ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "ชื่อลูกค้า ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "ชื่อบริษัทประกันภัย ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "เบี้ยประกันที่ต้องชำระ ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "วันที่เริ่มคุ้มครอง ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "วันสิ้นสุดคุ้มครอง ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "ประเภท (ประกันภัย , พรบ.) ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "PAY_TYPE ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "จำนวนเงิน ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "สถานะหลักของ lead ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "สถานะย่อยของ Lead ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "เลขที่รับแจ้ง ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "วันที่ส่งแจ้ง พรบ. ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "พรบ. ออกที่ ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "Emp Code ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "Owner Lead ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "Head Team ", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                                new ExcelExportBiz.ColumnItem() {ColumnName = "Team Code", ColumnDataType = ExcelExportBiz.ColumnType.Text},
                            }
                        }
                    }))
                        throw new Exception(ebz.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                ErrorMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
            }

            return ret;
        }
    }

    public class SlmScr064SearchResult
    {
        public DateTime? slm_TransDate { get; set; }
        public string slm_BranchCode { get; set; }
        public string slm_BranchName { get; set; }
        public string slm_ContractNo { get; set; }
        public string CustomerName { get; set; }
        public string slm_InsNameTh { get; set; }
        public decimal? GrossPremium { get; set; }
        public DateTime? StartCoverDate { get; set; }
        public DateTime? EndCoverDate { get; set; }
        public string DocType { get; set; }
        public string slm_RecBy { get; set; }
        public decimal? slm_RecAmount { get; set; }
        public string slm_OptionDesc { get; set; }
        public string slm_ExternalSubStatusDesc { get; set; }
        public string slm_ReceiveNo { get; set; }
        public DateTime? slm_ActSendDate { get; set; }
        public string ActIssuePlace { get; set; }
        public string slm_EmpCode { get; set; }
        public string slm_StaffNameTH { get; set; }
        public string HeadStaffNameTH { get; set; }
        public string slm_TeamTelesales_Code { get; set; }
    }
}
