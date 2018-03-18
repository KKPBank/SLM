using SLM.Resource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Dal.Models
{
    public class KKSlmTrSuretyModel
    {
        public List<SuretyData> GetSuretyData(int PreleadId)
        {
            string sql = @"SELECT slm_Prelead_Id 
                              , 1 as SEQ
                              ,slm_Guarantor_Code1 AS GCODE
                              ,title.slm_TitleName as TITLENAME 
                              ,slm_Guarantor_First_Name1 AS FIRSTNAME
                              ,slm_Guarantor_Last_Name1 AS LASTNAME
                              ,slm_Guarantor_Card_Id1 AS CARDID
                              ,rl.slm_RelateDesc as RELATION
                              ,slm_Guarantor_Telno1 AS TELNO
                          FROM dbo.kkslm_tr_prelead pre WITH (NOLOCK) LEFT join kkslm_ms_title as title WITH (NOLOCK) on title.slm_TitleId = pre.slm_Guarantor_TitleId1
		                        LEFT JOIN kkslm_ms_relate rl WITH (NOLOCK) on rl.slm_RelateId = pre.slm_Guarantor_RelationId1 
                          WHERE slm_Guarantor_Code1 is not null and  slm_Prelead_Id = " + PreleadId + @"
                          UNION ALL
                          SELECT slm_Prelead_Id
                              , 2 as SEQ
                              ,slm_Guarantor_Code2 AS GCODE
                              ,title.slm_TitleName as TITLENAME 
                              ,slm_Guarantor_First_Name2 AS FIRSTNAME
                              ,slm_Guarantor_Last_Name2 AS LASTNAME
                              ,slm_Guarantor_Card_Id2 AS CARDID
                              ,rl.slm_RelateDesc as RELATION
                              ,slm_Guarantor_Telno2  AS TELNO
                          FROM dbo.kkslm_tr_prelead pre WITH (NOLOCK) left join kkslm_ms_title as title WITH (NOLOCK) on title.slm_TitleId = pre.slm_Guarantor_TitleId2
		                        LEFT JOIN kkslm_ms_relate rl WITH (NOLOCK) on rl.slm_RelateId = pre.slm_Guarantor_RelationId2 
                          WHERE slm_Guarantor_Code2 is not null and  slm_Prelead_Id = " + PreleadId + @" 
                          UNION ALL
                          SELECT slm_Prelead_Id
                              , 3 as SEQ
                              ,slm_Guarantor_Code3 AS GCODE
                              ,title.slm_TitleName as TITLENAME 
                              ,slm_Guarantor_First_Name3 AS FIRSTNAME
                              ,slm_Guarantor_Last_Name3 AS LASTNAME
                              ,slm_Guarantor_Card_Id3  AS CARDID
                              ,rl.slm_RelateDesc as RELATION
                              ,slm_Guarantor_Telno3
                          FROM dbo.kkslm_tr_prelead pre WITH (NOLOCK) left join kkslm_ms_title as title WITH (NOLOCK) on title.slm_TitleId = pre.slm_Guarantor_TitleId3
		                        LEFT JOIN kkslm_ms_relate rl WITH (NOLOCK) on rl.slm_RelateId = pre.slm_Guarantor_RelationId3 
                          WHERE slm_Guarantor_Code3 is not null and slm_Prelead_Id = " + PreleadId + "";

            using(SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
            {
                return slmdb.ExecuteStoreQuery<SuretyData>(sql).ToList();
            }
        }
    }
}
