using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JD.Product.Crawler.Lucene.Interface;
using JD.Product.Crawler.Lucene.Service;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Utils;

namespace JD.Product.Crawler.Processor
{
    public class IndexBuilder
    {
        private static CancellationTokenSource CTS = new CancellationTokenSource();
        private static List<string> PathSuffixList = new List<string>();

        /// <summary>
        /// 构建索引
        /// </summary>
        public static void Build()
        {
            //分页数必须大于20 后期考虑
            //得到任务集合，分30页
            var taskDataList = TableIndexProcessor.GetEachThreadTask(20);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            TaskFactory taskFactory = new TaskFactory();
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < taskDataList.Count; i++)
            {
                var i1 = i;
                Task task = taskFactory.StartNew(() =>
                   {
                       try
                       {
                           LogHelper.WriteLog($"***Build{i1.ToString()} 索引开始创建****",ConsoleColor.Blue);
                           LuceneBulid luceneBuild = new LuceneBulid();
                           PathSuffixList.Add((i1 + 1).ToString("000"));
                           luceneBuild.BuildIndex(taskDataList.ElementAt(i1), (i1 + 1).ToString("000"), true);
                       }
                       catch (Exception e)
                       {
                           LogHelper.WriteLog($"Build{i1.ToString()}\t{e.Message}", e);
                           CTS.Cancel();
                       }
                   }, CTS.Token).ContinueWith((t) =>
                {
                    LogHelper.WriteLog($"***Build{i1.ToString()} 索引创建完成****", ConsoleColor.Blue);
                });

                taskList.Add(task);
            }

            taskList.Add(taskFactory.ContinueWhenAll(taskList.ToArray(), MergeAllLuceneIndex));
            Task.WaitAll(taskList.ToArray());
            sw.Stop();
            LogHelper.WriteLog($"Build{(CTS.IsCancellationRequested ? "失败" : "成功")} 耗时{sw.ElapsedMilliseconds / 1000 / 60}分钟");
        }

        private static void MergeAllLuceneIndex(Task[] obj)
        {
            try
            {
                if (CTS.IsCancellationRequested) return;
                ILuceneBulid builder = new LuceneBulid();
                builder.MergeAllLuceneIndex(PathSuffixList.ToArray());
            }
            catch (Exception ex)
            {
                CTS.Cancel();
                LogHelper.WriteLog($"MergeAllLuceneIndex\t{ex.Message}{ex}");
            }
        }
    }
}
