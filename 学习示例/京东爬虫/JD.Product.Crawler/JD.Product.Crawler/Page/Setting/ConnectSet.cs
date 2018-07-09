using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using JD.Product.Crawler.Other;

namespace JD.Product.Crawler.Page.Setting
{
    public partial class ConnectSet : Form
    {
        public ConnectSet()
        {
            InitializeComponent();
        }

        private void ConnectSet_Load(object sender, EventArgs e)
        {
            string conStr = PublicConst.ConnectiongString;

            //Data Source=.; Initial Catalog=JDCrawler; User Id=sa; Password=123456;
            //正则表达式拆分
            Regex userReg = new Regex("User Id=(.+?);");
            Regex pwdReg = new Regex("Password=(.+?);");

            var match1 = userReg.Match(conStr).Groups[1].Value;
            var match2 = pwdReg.Match(conStr).Groups[1].Value;

            userName.Text = match1;
            pwd.Text = match2;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["connectiongString"].Value = string.Format($"Data Source=.; Initial Catalog=JDCrawler; User Id={userName.Text}; Password={pwd.Text};");
            cfa.Save();

            this.Close();
        }
    }
}
