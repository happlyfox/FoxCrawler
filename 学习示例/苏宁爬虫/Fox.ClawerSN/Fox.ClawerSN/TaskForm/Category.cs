using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fox.ClawerSN.LongTimeWork;
using Fox.ClawerSN.Service;
using Fox.ClawerSN.Util;
using Fox.ClawerSN.WebAnalysis;

namespace Fox.ClawerSN.TaskForm
{
    public partial class Category : Form
    {
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly object lock_obj = new object();

        public Category()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            bool first = radioButton1.Checked;
            if (first == true)
            {
                _categoryService.Init();
            }

            var cateList = CategoryAnalysis.GetData();
            CategoryUpdateWork worker = new CategoryUpdateWork();
            worker.ValueChanged += (argObj, argEvent) =>
            {
                void Invoker() => progressBar1.Value = argEvent.Value;
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke((MethodInvoker)Invoker);
                }
                else
                {
                    Invoker();
                }
            };

            Task.Run(() => { worker.CategoryUpdate(cateList); }).ContinueWith(ComplateToDo);
        }

        private void BindGridView()
        {
            dataGridView1.DataSource = _categoryService.GetAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BindGridView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            #region Action1
            var urlList = _categoryService.GetListByLevel(3).Select(u => u.Url).ToList();

            //封装成一个方法
            int pageNum = 100;
            int pageCount = urlList.Count % pageNum == 0 ? urlList.Count / pageNum : urlList.Count / pageNum + 1;
            var pageListCollection = new List<List<string>>();
            for (int i = 0; i < pageCount; i++)
            {
                var pageList = urlList.Skip(i * pageNum).Take(pageNum).ToList();
                pageListCollection.Add(pageList);
            }
            #endregion
            Console.WriteLine(pageCount);
            #region Action2
            //50个为例
            //同步18233毫秒
            //var dics = new Dictionary<string, int>();
            //foreach (var url in urlList)
            //{
            //    dics.Add(url, CategoryPageAnalysis.GetData(url));
            //}

            //异步 6163毫秒 3倍的效率差
            int pageIndex = 1;
            List<Task> taskList = new List<Task>();
            foreach (var pageList in pageListCollection)
            {
                try
                {
                    Task task = Task.Factory.StartNew(() =>
                    {
                        var dics = new Dictionary<string, int>();
                        foreach (var url in pageList)
                        {
                            dics.Add(url, CategoryPageAnalysis.GetData(url));
                        }

                        lock (lock_obj)
                        {
                            _categoryService.BatchUpdatePage(dics);
                        }
                    }).ContinueWith(t =>
                    {
                        int percent = Convert.ToInt32((pageIndex++ * 1.0 / pageCount) * 100);
                        Console.WriteLine($"{pageIndex - 1} {percent}");

                        void Invoker() => progressBar1.Value = percent;
                        if (InvokeRequired)
                        {
                            Invoke((MethodInvoker)Invoker);
                        }
                        else
                            Invoker();
                    });
                    taskList.Add(task);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"button3_Click 异步{ex.Message}");
                }
            }

            Task.WhenAll(taskList.ToArray()).ContinueWith(ComplateToDo);

            #endregion
        }

        public async void ComplateToDo(Task task)
        {
            void Invoker()
            {
                progressBar1.Value = 0;
                FormHelper.Show(true);
                BindGridView();
                button1.Enabled = true;
                button3.Enabled = true;
            }

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)Invoker);
            }
            else
            {
                Invoker();
            }
        }

        private void Category_Load(object sender, EventArgs e)
        {
            BindGridView();
        }
    }
}
