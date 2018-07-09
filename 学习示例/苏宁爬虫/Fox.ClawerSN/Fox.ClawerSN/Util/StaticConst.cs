using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Util
{
    public class StaticConst
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static readonly string SqlDbCon = ConfigurationManager.AppSettings["SqlDbCon"];

        /// <summary>
        /// 索引存储路径
        /// </summary>
        public static readonly string LucenePath = ConfigurationManager.AppSettings["lucenePath"];

        /// <summary>
        /// 商品分页数
        /// </summary>
        public static readonly int CategorySheetCount = 20;

        /// <summary>
        /// 分页每次取的数量
        /// </summary>
        public static readonly int PageGetCount = 1000;

    }
}
