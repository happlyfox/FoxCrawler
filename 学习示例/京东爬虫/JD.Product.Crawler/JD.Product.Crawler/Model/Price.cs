using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Model
{
    public class Price
    {
        /// <summary>
        /// 
        /// </summary>
        public double op { get; set; }

        /// <summary>
        /// 
        /// </summary>  
        public double m { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 需要的价格值
        /// </summary>
        public double p { get; set; }
    }
}
