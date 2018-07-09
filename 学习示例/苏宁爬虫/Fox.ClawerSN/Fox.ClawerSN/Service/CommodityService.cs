using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.DAL;
using Fox.ClawerSN.Model;

namespace Fox.ClawerSN.Service
{
    public class CommodityService
    {
        private readonly CommodityDAL _commodityDAL = new CommodityDAL();

        public void InsertGroupBulk(List<CommodityGroupInput> groupList)
        {
            _commodityDAL.InsertGroupBulk(groupList);
        }

        public int GetTableCount(int sheetIdnex)
        {
            return _commodityDAL.GetTableCount(sheetIdnex);
        }

        public List<POCO_Commodity> GetPageList(int sheetIndex, int pageIndex, int pageSize)
        {
            return _commodityDAL.GetPageList(sheetIndex, pageIndex, pageSize);
        }

    }
}
