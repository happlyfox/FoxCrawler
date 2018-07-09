using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Model
{
   public class ProductGroupInput
    {
        public long Id { get; set; }
        public List<ProductUnit> ProductUnits { get; set; }
    }
}
