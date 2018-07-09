using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;
using JD.Product.Crawler.Page.Setting;
using JD.Product.Crawler.Processor;
using JD.Product.Crawler.Search;
using JD.Product.Crawler.Service;
using JD.Product.Crawler.Utils;

namespace JD.Product.Crawler
{
    public partial class Index : Form
    {
        public Index()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //判断爬虫文件夹是否存在
            if (string.IsNullOrWhiteSpace(PublicConst.DataDirPath))
            {
                MessageBox.Show("请配置数据日志保存文件路径！", PublicConst.ToopTip, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                new DataDirSet().ShowDialog();
            }
            else
            {
                if (!Directory.Exists(PublicConst.DataDirPath))
                {
                    Directory.CreateDirectory(PublicConst.DataDirPath);
                }
            }

            var result = MessageBox.Show("您是否需要确认数据库链接配置?", PublicConst.ToopTip, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                new ConnectSet().ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("您是否需要重置类别表?", PublicConst.ToopTip, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                LogHelper.WriteLog("*************开始类别爬取*************");
                //1.0 爬取类别数据
                var list = CategorySearch.GetData();
                LogHelper.WriteLog("*************结束类别爬取*************");

                #region 打印输出
                //foreach (var item in list)
                //{
                //    Console.WriteLine($"Code:{item.Code} \t PCode:{item.PCode}\t Name:{item.Name}\t Url:{item.Url}");
                //} 
                #endregion

                //2.0 保存入库
                //2.0.1 重置表
                CategoryService catesService = new CategoryService();
                catesService.ResetTable();

                LogHelper.WriteLog("*************开始更新数据库*************");

                //2.0.2 批量插入
                catesService.InsertBulk(list);
                //2.0.3 删除无效数据
                catesService.DeleteRubbish();

                LogHelper.WriteLog("*************结束更新数据库*************");

                MessageBox.Show("更新类别成功", PublicConst.ToopTip);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProductService.Init();//初始化表

            //得到商品类别
            CategoryService catesService = new CategoryService();
            var list = catesService.SelectAll().Where(u => u.Levels == 3);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Task> taskList = new List<Task>();
            foreach (Category cg in list)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    Stopwatch swShop = new Stopwatch();
                    swShop.Start();
                    LogHelper.WriteLog($"*************商品名称:{cg.Name}开始爬取*************", ConsoleColor.Blue);
                    int pageNum = ProductSearch.GetPageNum(cg.Url);
                    for (int index = 1; index <= pageNum; index++)
                    {
                        LogHelper.WriteLog($"{cg.Name} 页码{index} 总页码{pageNum}");
                        //判断当前是否存在优化可能
                        var productList = ProductSearch.GetPageDataAsync(cg.Url, index, cg.Id);

                        //分组，主要为了分表插入做准备
                        List<ProductGroupInput> groupList = productList.GroupBy(u => u.ProductId % PublicConst.SheetNum).Select(u => new ProductGroupInput
                        {
                            Id = u.Key,
                            ProductUnits = u.OrderBy(p => p.ProductId).ToList()
                        }).ToList();

                        ProductService.InsertGroupBulk(groupList);
                    }
                    swShop.Stop();
                    return swShop.ElapsedMilliseconds;

                }).ContinueWith(t =>
              {
                  LogHelper.WriteLog($"*************商品名称:{cg.Name} 结束爬取 耗时:{t.Result / 1000 / 60}分钟*************", ConsoleColor.Blue);
              });

                taskList.Add(task);
                if (taskList.Count > 20)
                {
                    taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
                    Task.WaitAny(taskList.ToArray());
                }
            }

            Task.WaitAll(taskList.ToArray());
            sw.Stop();
            LogHelper.WriteLog($"*************商品爬取结束 耗时:{sw.ElapsedMilliseconds / 1000 / 60}分钟 {sw.ElapsedMilliseconds / 1000 / 60 / 24}小时*************");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                LogHelper.WriteLog("开始构建索引");
                IndexBuilder.Build();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("构建索引出错,出错原因:" + ex.Message, ex);
            }
            finally
            {
                LogHelper.WriteLog("结束构建索引");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new LuceneIndexDemo().Show();
        }
    }
}
