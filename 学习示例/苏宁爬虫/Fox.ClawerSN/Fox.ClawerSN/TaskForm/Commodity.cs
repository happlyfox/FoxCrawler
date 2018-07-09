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
using Fox.ClawerSN.Extension;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Service;
using Fox.ClawerSN.Util;
using Fox.ClawerSN.WebAnalysis;

namespace Fox.ClawerSN.TaskForm
{
    public partial class Commodity : Form
    {
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly CommodityService _commodityService = new CommodityService();
        private List<CategoryDG_Output> categoryList = new List<CategoryDG_Output>();

        public Commodity()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommodityDAL.InitDb();

            long allworkCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            button1.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                List<Task> taskList = new List<Task>();
                foreach (var category in categoryList)
                {
                    Task task = Task.Factory.StartNew(() =>
                    {
                        ActInvoker(() =>
                        {
                            dataGridView1.Rows[category.Index].Cells["State"].Value =
                                (int)ClawerEnum.TaskState.Doing;
                            dataGridView1.Rows[category.Index].Cells["StateText"].Value =
                                ClawerEnum.TaskState.Doing.GetDescription();
                        });

                        for (int i = 1; i <= category.PageCount; i++)
                        {
                            //取值
                            List<POCO_Commodity> cateList = CommodityAnalysis.GetData(category.Url, category.Id, i);
                            //处理
                            List<CommodityGroupInput> groupList = cateList.GroupBy(u => Convert.ToInt64(u.SUId) % StaticConst.CategorySheetCount).Select(u => new CommodityGroupInput
                            {
                                Id = u.Key,
                                Units = u.OrderBy(p => p.SUId).ToList()
                            }).ToList();
                            //入库
                            _commodityService.InsertGroupBulk(groupList);
                        }
                    }).ContinueWith(t =>
                    {
                        allworkCount += category.PageCount;
                        ActInvoker(() =>
                        {
                            this.FinishCount.Text = allworkCount.ToString();
                            this.FinishHour.Text = StopWatchHelper.formatDuring(sw.ElapsedMilliseconds);
                        });

                        ActInvoker(() =>
                        {
                            dataGridView1.Rows[category.Index].Cells["State"].Value =
                                (int)ClawerEnum.TaskState.Finshed;
                            dataGridView1.Rows[category.Index].Cells["StateText"].Value =
                                ClawerEnum.TaskState.Finshed.GetDescription();
                        });
                    });

                    taskList.Add(task);
                    if (taskList.Count > 20)
                    {
                        taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
                        Task.WaitAny(taskList.ToArray());
                    }
                }

                Task.WhenAll(taskList.ToArray()).ContinueWith(t =>
                {
                    ActInvoker(() =>
                    {
                        button1.Enabled = true;
                        FormHelper.Show(true);
                        sw.Reset();
                    });
                });
            });
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

        private void Commodity_Load(object sender, EventArgs e)
        {
            //1.35s 预估单页执行时间
            categoryList = _categoryService.GetListByLevel(3).Where(u => u.PageCount > 0).Select((u, i) => new CategoryDG_Output
            {
                Index = i,
                Id = u.Id,
                Name = u.Name,
                PageCount = u.PageCount,
                State = (int)ClawerEnum.TaskState.ToDo,
                StateText = ClawerEnum.TaskState.ToDo.GetDescription(),
                Url = u.Url
            }).ToList();
            int allPageCount = categoryList.Sum(u => u.PageCount);
            AllCount.Text = allPageCount.ToString();
            AllHour.Text = Math.Round(((allPageCount * 1.35) / 60 / 24), 2).ToString();

            dataGridView1.DataSource = categoryList;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex > -1 && e.ColumnIndex == 4)
            {
                string state = dataGridView1["State", e.RowIndex].Value.ToString();
                Color color = (state == Convert.ToInt32(ClawerEnum.TaskState.Doing).ToString() ? Color.Aqua : Color.LightGray);
                dataGridView1["StateText", e.RowIndex].Style.BackColor = color;
            }
        }

    }
}
