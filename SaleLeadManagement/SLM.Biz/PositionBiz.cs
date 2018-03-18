using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLM.Resource.Data;
using SLM.Dal.Models;

namespace SLM.Biz
{
    public class PositionBiz
    {
        public static void InsertData(string positionNameAbb, string positionNameEN, string positionNameTH, bool isActive, string createdBy)
        {
            KKSlmMsPositionModel position = new KKSlmMsPositionModel();
            position.InsertData(positionNameAbb, positionNameEN, positionNameTH, isActive, createdBy);
        }

        public static void UpdateData(int positionId, string positionNameAbb, string positionNameEN, string positionNameTH, bool isActive, string updatedBy)
        {
            KKSlmMsPositionModel position = new KKSlmMsPositionModel();
            position.UpdateData(positionId, positionNameAbb, positionNameEN, positionNameTH, isActive, updatedBy);
        }

        public static List<PositionData> SearchPosition(string positionNameAbb, string positionNameEN, string positionNameTH, bool statusActive, bool statusInActive)
        {
            KKSlmMsPositionModel position = new KKSlmMsPositionModel();
            return position.SearchPosition(positionNameAbb, positionNameEN, positionNameTH, statusActive, statusInActive);
        }

        public static List<ControlListData> GetPositionList(int flag)
        {
            KKSlmMsPositionModel position = new KKSlmMsPositionModel();
            return position.GetPositionList(flag);
        }
    }
}
