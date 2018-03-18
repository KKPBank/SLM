using SLMBatch.Common;
using SLMBatch.Data;

namespace SLMBatch.Business
{
    public class OBT_PRO_29_Biz : ServiceExcelBase<OBT_PRO_29_DataAccess>
    {
        public OBT_PRO_29_Biz() : base(
            Util.eExportModule.PolicyNoActNoPending,
            "PolicyNoActNoPending_Template.xlsx",
            "OBT_PRO_29_EmailTemplate.html",
            AppConstant.PolicyNoActNoPending.Email,
            AppConstant.PolicyNoActNoPending.Export
        )
        {
        }

        public bool ExportExcelPolicyNoActNoPending(string batchCode)
        {
            return ExportExcel(batchCode);
        }
    }
}
