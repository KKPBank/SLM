using SLMBatch.Common;
using SLMBatch.Data;
using SpreadsheetLight;

namespace SLMBatch.Business
{
    public class OBT_PRO_30_Biz : ServiceExcelBase<OBT_PRO_30_DataAccess>
    {
        public OBT_PRO_30_Biz() : base(
            Util.eExportModule.LeadsForTransfer,
            "LeadsForTransfer_Template.xlsx",
            "OBT_PRO_30_EmailTemplate.html",
            AppConstant.LeadsForTransfer.Email,
            AppConstant.LeadsForTransfer.Export
        )
        {
        }

        public bool ExportExcelLeadsForTransfer(string batchCode)
        {
            return ExportExcel(batchCode);
        }

        protected override void StyleExcel(SLDocument report)
        {
            var dummy = new SLDocument();

            SLStyle moneyStyle = dummy.CreateStyle();
            moneyStyle.FormatCode = "#,##0.00";
            report.SetColumnStyle(10, moneyStyle);

            SLStyle dateStyle = dummy.CreateStyle();
            dateStyle.FormatCode = "dd/mm/yyyy";
            report.SetColumnStyle(14, dateStyle);
            report.SetColumnStyle(17, dateStyle);
            report.SetColumnStyle(18, dateStyle);
        }
    }
}
