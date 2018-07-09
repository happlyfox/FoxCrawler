using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.LongTimeWork
{
    // 定义事件的参数类
    public class ValueEventArgs
        : EventArgs
    {
        public int Value { set; get; }
    }

    public class LongValueEventArgs
        : EventArgs
    {
        public long Value { set; get; }
    }

    public class StringValueEventArgs
        : EventArgs
    {
        public string Value { set; get; }
    }
}
