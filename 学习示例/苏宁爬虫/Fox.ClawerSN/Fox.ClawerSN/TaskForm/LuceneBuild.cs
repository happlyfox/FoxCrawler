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
using Fox.ClawerSN.LongTimeWork;
using Fox.ClawerSN.Lucene.Interface;
using Fox.ClawerSN.Lucene.Service;
using Fox.ClawerSN.Processor;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.TaskForm
{
    public partial class LuceneBuild : Form
    {
        public LuceneBuild()
        {
            InitializeComponent();
        }

        private void LuceneBuild_Load(object sender, EventArgs e)
        {
            float size = DBInit.GetDBSize();
            Size_Label.Text = $"{size.ToString()}MB";
            Finish_Label.Text = StopWatchHelper.formatDuring(0);
            Lucenepath_Label.Text = StaticConst.LucenePath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Stopwatch sw=new Stopwatch();
            sw.Start();

            CommodityIndexBuildWorker worker = new CommodityIndexBuildWorker();
            worker.ValueChanged += (argObj, argEvent) =>
            {
                ActInvoker(() =>
                {
                    progressBar1.Value = argEvent.Value;
                    Finish_Label.Text = StopWatchHelper.formatDuring(sw.ElapsedMilliseconds);
                });
            };

            worker.TaskError += (argObj, argEvent) =>
            {
                ActInvoker(() =>
                {
                    FormHelper.Show("索引建立失败:" + argEvent.Value);
                    sw.Stop();
                    button1.Enabled = true;
                });
            };

            worker.TaskComplate += (argObj, argEvent) =>
            {
                ActInvoker(() =>
                {
                    FormHelper.Show("索引建立成功");
                    progressBar1.Value = 0;
                    sw.Stop();
                    button1.Enabled = true;
                });
            };
            worker.Build();
        }

        private void ActInvoker(MethodInvoker invoker)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(invoker);
            }
            else
            {
                invoker();
            }
        }
    }
}