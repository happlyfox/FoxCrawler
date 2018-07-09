using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fox.ClawerSN.Util
{
    public class FormHelper
    {
        public static void Show(bool f)
        {
            if (f)
            {
                MessageBox.Show("更新成功");
            }
            else
            {
                MessageBox.Show("更新失败");
            }
        }

        public static void Show(int iret)
        {
            Show(iret > 0);
        }

        public static void Show(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
