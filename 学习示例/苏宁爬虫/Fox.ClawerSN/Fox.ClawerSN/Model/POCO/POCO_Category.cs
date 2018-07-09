using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Model
{
    public class POCO_Category
    {
        public string Id { get; set; }
        public string PId { get; set; }
        public int Levels { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int PageCount { get; set; }
    }
}
