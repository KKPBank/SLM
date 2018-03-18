using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_26_Biz : ServiceExcelBase<OBT_PRO_26_DataAccess>
    {
        public OBT_PRO_26_Biz() : base(
            Util.eExportModule.PaymentPending,
            "PaymentPending_Template.xlsx",
            "OBT_PRO_26_EmailTemplate.html",
            AppConstant.PaymentPending.Email,
            AppConstant.PaymentPending.Export
        )
        {
        }

        public bool ExportExcelPaymentPending(string batchCode)
        {
            return ExportExcel(batchCode);
        }
    }
}
