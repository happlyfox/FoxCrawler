using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fox.ClawerSN.DAL;
using Fox.ClawerSN.TaskForm;
using Fox.ClawerSN.Util;
using Fox.ClawerSN.WebAnalysis;

namespace Fox.ClawerSN
{
    public partial class Default : Form
    {
        public Default()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Category().Show();
        }

        private void Default_Load(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var digResult = MessageBox.Show("您是否确认初始化?", "提示", MessageBoxButtons.YesNo);
            if (digResult == DialogResult.OK)
            {
                FormHelper.Show(DBInit.InitDb());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Commodity().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new LuceneBuild().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new LuceneSearch().Show();
        }
    }
}
