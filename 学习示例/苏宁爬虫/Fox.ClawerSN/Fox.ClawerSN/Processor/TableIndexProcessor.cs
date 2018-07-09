using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Service;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.Processor
{
    public class TableIndexProcessor
    {
        private readonly CommodityService _commodityService = new CommodityService();

        /// <summary>
        /// 得到分配线程的任务列表
        /// </summary>
        /// <param name="threadCount">线程数</param>
        /// <returns></returns>
        public List<List<TableIndexModel>> GetEachThreadTask(int threadCount)
        {
            //分页以1000作为基数
            //Dictionary[表名,分页总数]
            Dictionary<int, int> tableCountDictionary = new Dictionary<int, int>();
            for (int i = 0; i < StaticConst.CategorySheetCount; i++)
            {
                int pageNum = PageHelper.GetPageNum(_commodityService.GetTableCount(i), StaticConst.PageGetCount); ;
                tableCountDictionary.Add(i, pageNum);
            }

            //得到[表索引,页码]集合
            List<TableIndexModel> timList = new List<TableIndexModel>();
            foreach (var tcd in tableCountDictionary)
            {
                for (int i = 1; i <= tcd.Value; i++)
                {
                    timList.Add(new TableIndexModel()
                    {
                        TableIndex = tcd.Key,
                        PageIndex = i
                    });
                }
            }

            /*平均分配任务的业务逻辑为：
                每个线程需要处理多少任务=timList.Count 总个数 /线程数
             */
            int workPageNum = PageHelper.GetPageNum(timList.Count, threadCount);

            //得到[平均分配后的任务列表]
            List<List<TableIndexModel>> taskDataList = new List<List<TableIndexModel>>();
            for (int i = 1; i <= workPageNum; i++)
            {
                var list = timList.Skip((i - 1) * threadCount).Take(threadCount).ToList();
                taskDataList.Add(list);
            }

            return taskDataList;
        }
    }
}
