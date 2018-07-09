using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox.ClawerSN.Util
{
    public class StopWatchHelper
    {
        public static String formatDuring(long mss)
        {
            long hours = (mss % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60);
            long minutes = (mss % (1000 * 60 * 60)) / (1000 * 60);
            long seconds = (mss % (1000 * 60)) / 1000;
            return +hours + " 小时 " + minutes + " 分钟 "
                   + seconds + " 秒 ";
        }
    }
}
