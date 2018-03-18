using SLMBatch.Common;
using SLMBatch.Data;
using SpreadsheetLight;

namespace SLMBatch.Business
{
    public class OBT_PRO_31_Biz : ServiceExcelBase<OBT_PRO_31_DataAccess>
    {
        public OBT_PRO_31_Biz() : base(
            Util.eExportModule.LeadsForTKS,
            "LeadsForTKS_Template.xlsx",
            "OBT_PRO_31_EmailTemplate.html",
            AppConstant.LeadsForTKS.Email,
            AppConstant.LeadsForTKS.Export
        )
        {
        }

        public bool ExportExcelLeadsForTKS(string batchCode)
        {
            return ExportExcel(batchCode);
        }

        protected override void StyleExcel(SLDocument report)
        {
            var dummy = new SLDocument();

            SLStyle numberStyle = dummy.CreateStyle();
            numberStyle.FormatCode = "#,##0";
            //report.SetColumnStyle(33, numberStyle);     // Car_Insurance_Cost_Damage
            //report.SetColumnStyle(34, numberStyle);     // Car_Insurance_Cost_Lost

            SLStyle moneyStyle = dummy.CreateStyle();
            moneyStyle.FormatCode = "#,##0.00";
            //report.SetColumnStyle(35, moneyStyle);      // Discount_Good_Records
            //report.SetColumnStyle(36, moneyStyle);      // Gross_Premium
            //report.SetColumnStyle(37, moneyStyle);      // Discount_KK
            //report.SetColumnStyle(39, moneyStyle);      // Compulsory_Gross_Premium

            SLStyle dateStyle = dummy.CreateStyle();
            dateStyle.FormatCode = "dd/mm/yyyy";
            report.SetColumnStyle(26, dateStyle);       // Previous_Insurance_Expire_Date
            report.SetColumnStyle(48, dateStyle);       // System_Date
        }
    }
}
