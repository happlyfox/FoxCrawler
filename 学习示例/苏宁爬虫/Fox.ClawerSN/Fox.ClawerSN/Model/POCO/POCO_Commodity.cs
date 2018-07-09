using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Model
{
    public class POCO_Commodity
    {
        public long Id { get; set; }
        public string SUId { get; set; }
        public string CategoryId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ShopName { get; set; }
    }
}
