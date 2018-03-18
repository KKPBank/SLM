using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource.Data
{
    public class SearchPromotionResult
    {

        public decimal? slm_PromotionId { get; set; }

        public string slm_Product_Id { get; set; }

        public string insname { get; set; }

        public string camname { get; set; }

        public DateTime? EffectiveDateFrom { get; set; }

        public DateTime? EffectiveDateTo { get; set; }

        public string brandname { get; set; }

        public string repairname { get; set; }

        //public string modelname { get; set; }

        public string converagetypename { get; set; }

        public decimal? slm_OD { get; set; }

        public decimal? slm_FT { get; set; }

        public decimal? slm_NetGrossPremium { get; set; }

        public decimal? slm_Act { get; set; }

        public int? slm_AgeCarYear { get; set; }
        //public string slm_DeDuctible { get; set; }

        //public string slm_GrossPremium { get; set; }

        //public string slm_Stamp { get; set; }

        //public string slm_Vat { get; set; }

        //public string slm_NetGrossPremium { get; set; }

        //public string slm_Act { get; set; }

        //public string slm_InjuryDeath { get; set; }

        //public string slm_TPPD { get; set; }

        

    }
}
