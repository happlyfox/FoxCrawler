using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouKuDotnetSpider2
{
    /// <summary>
    /// 单页面实体
    /// </summary>
    public class YouKu
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 总个数
        /// </summary>
        public int videoNum { get; set; }

        /// <summary>
        /// 电影国籍集合
        /// </summary>
        public List<string> videoCountry { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public List<string> videoType { get; set; }
    }
}
