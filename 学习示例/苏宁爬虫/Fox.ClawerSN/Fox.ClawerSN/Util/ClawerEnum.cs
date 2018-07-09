using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Util
{
    public class ClawerEnum
    {
        public enum TaskState
        {
            [Description("未启动")]
            ToDo = 0,
            [Description("正在爬取")]
            Doing = 1,
            [Description("已完成")]
            Finshed = 2
        }
    }
}
