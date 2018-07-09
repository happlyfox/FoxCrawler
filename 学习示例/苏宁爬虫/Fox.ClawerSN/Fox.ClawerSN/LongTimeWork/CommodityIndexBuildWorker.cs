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
using Fox.ClawerSN.DAL;
using Fox.ClawerSN.Lucene.Interface;
using Fox.ClawerSN.Lucene.Service;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Processor;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.LongTimeWork
{
    public class CommodityIndexBuildWorker
    {
        private static CancellationTokenSource CTS = new CancellationTokenSource();
        private readonly List<string> PathSuffixList = new List<string>();

        // 定义事件使用的委托
        public delegate void ValueChangedEventHandler(object sender, ValueEventArgs e);
        public delegate void TaskComplateEventHandler(object sender, EventArgs e);
        public delegate void TaskErrorEventHandler(object sender, StringValueEventArgs e);

        // 定义一个事件来提示界面工作的进度
        public event ValueChangedEventHandler ValueChanged;

        // 定义一个事件来提示界面完成时的处理
        public event TaskComplateEventHandler TaskComplate;

        // 定义一个事件来提示界面出错时的处理
        public event TaskErrorEventHandler TaskError;

        protected void OnValueChanged(ValueEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
        protected void OnTaskComplate(EventArgs e)
        {
            TaskComplate?.Invoke(this, e);
        }

        protected void OnTaskError(StringValueEventArgs e)
        {
            TaskError?.Invoke(this, e);
        }

        /// <summary>
        /// 构建索引
        /// </summary>
        public void Build()
        {
            TableIndexProcessor processor = new TableIndexProcessor();
            var taskDataList = processor.GetEachThreadTask(40);

            TaskFactory taskFactory = new TaskFactory();
            taskFactory.StartNew(() =>
            {
                List<Task> taskList = new List<Task>();
                var threadNums = Enumerable.Range(1, StaticConst.CategorySheetCount).ToList();
                Random random = new Random();
                int index = 0, alltaskCount = taskDataList.Count;
                foreach (var taskData in taskDataList)
                {
                    index++;
                    var threadCode = CommodityDAL.GetTName(random.Next(1, threadNums.Count));
                    Task task = taskFactory.StartNew(() =>
                    {
                        try
                        {
                            LuceneBulid luceneBuild = new LuceneBulid();
                            if (!PathSuffixList.Any(u => u == threadCode))
                            {
                                PathSuffixList.Add(threadCode);
                            }
                            luceneBuild.BuildIndex(taskData, threadCode, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"BuildIndexError\t{ex.Message}");
                            StringValueEventArgs e = new StringValueEventArgs() { Value = ex.Message };
                            OnTaskError(e);//每完成一个任务触发事件
                            CTS.Cancel();
                        }
                    }, CTS.Token).ContinueWith(t =>
                    {
                        ValueEventArgs e = new ValueEventArgs() { Value = (int)(index * 1.0 / alltaskCount) * 100 };
                        OnValueChanged(e);//每完成一个任务触发事件
                    });
                    taskList.Add(task);
                    if (taskList.Count > 20)
                    {
                        taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
                        Task.WaitAny(taskList.ToArray());
                    }
                }
                taskList.Add(taskFactory.ContinueWhenAll(taskList.ToArray(), MergeAllLuceneIndex));
            });
        }

        private void MergeAllLuceneIndex(Task[] obj)
        {
            try
            {
                ILuceneBulid builder = new LuceneBulid();
                builder.MergeAllLuceneIndex(PathSuffixList.ToArray());
                OnTaskComplate(new EventArgs());//任务完成触发事件
            }
            catch (Exception ex)
            {
                StringValueEventArgs e = new StringValueEventArgs() { Value = ex.Message };
                OnTaskError(e);//每完成一个任务触发事件
                Console.WriteLine($"MergeAllLuceneIndex\t{ex.Message}{ex}");
            }
        }
    }
}
