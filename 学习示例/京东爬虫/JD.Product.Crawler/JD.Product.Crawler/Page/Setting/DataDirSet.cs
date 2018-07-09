using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JD.Product.Crawler.Other;

namespace JD.Product.Crawler.Page.Setting
{
    public partial class DataDirSet : Form
    {
        public DataDirSet()
        {
            InitializeComponent();
        }

        private void DataDirSet_Load(object sender, EventArgs e)
        {
            path.Text = PublicConst.DataDirPath;

            if (string.IsNullOrEmpty(path.Text))
            {
                path.Text = "单击文字进行配置";
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path.Text = dialog.SelectedPath;
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["dataDirPath"].Value = dialog.SelectedPath;
                cfa.Save();
                this.Close();
            }
        }
    }
}
