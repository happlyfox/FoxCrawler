using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Other
{
    public class PublicConst
    {
        /// <summary>
        /// 索引存储路径
        /// </summary>
        public static readonly string LucenePath = ConfigurationManager.AppSettings["lucenePath"];

        /// <summary>
        /// 数据日志
        /// </summary>
        public static readonly string DataDirPath = ConfigurationManager.AppSettings["dataDirPath"];

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static readonly string ConnectiongString = ConfigurationManager.AppSettings["connectiongString"];

        /// <summary>
        /// 提示
        /// </summary>
        public static readonly string ToopTip = "提示";

        /// <summary>
        /// 分表数
        /// </summary>
        public static readonly int SheetNum = 40;

        /// <summary>
        /// 分页每次取的数量
        /// </summary>
        public static readonly int PageGetCount = 1000;
    }
}
