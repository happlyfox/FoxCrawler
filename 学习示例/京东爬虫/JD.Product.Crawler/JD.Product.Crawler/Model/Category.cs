using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Model
{
    public class Category
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; } 
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 父编码
        /// </summary>
        public string PCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Levels { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
    }
}
