using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Model
{
    public class CommodityGroupInput
    {
        public long Id { get; set; }
        public List<POCO_Commodity> Units { get; set; }
    }
}
