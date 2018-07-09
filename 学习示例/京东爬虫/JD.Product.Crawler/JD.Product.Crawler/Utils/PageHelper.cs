using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Utils
{
    public class PageHelper
    {
        public static int GetPageNum(int allCount, int pageSize)
        {
            int PageNum = 0;//任务分页个数
            if (allCount % pageSize == 0)
            {
                PageNum = allCount / pageSize;
            }
            else
            {
                PageNum = allCount / pageSize + 1;
            }

            return PageNum;
        }
    }
}
