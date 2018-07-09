using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Model
{
    /// <summary>
    /// 产品类别
    /// </summary>
    public class ProductUnit
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 商品主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int CateId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        ///地址
        /// </summary>
        public string Url { get; set; }
    }
}
