using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Downloader;
using DotnetSpider.Core.Scheduler;
using YouKuDotnetSpider2Async.Crawer;

namespace YouKuDotnetSpider2Async
{
    /// <summary>
    /// 分页框架爬虫
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            CustmizeProcessorAndPipeline();
            Console.Read();
        }

        public static void CustmizeProcessorAndPipeline()
        {
            //  定义采集的 Site 对象, 设置 Header、Cookie、代理等
            var site = new Site { EncodingName = "UTF-8" };

            // 添加初始采集链接
            foreach (var node in YouKuCrawer.GetVideoTypes())
            {
                //得到当前类别总分页数
                var pageCount = YouKuCrawer.GetPageCountByCode(node.Code);
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    site.AddStartUrl($"http://list.youku.com/category/show/{node.Code}_s_1_d_1_p_{pageIndex}.html");
                }
            }

            Spider spider = Spider.Create(site,
                    // 使用内存调度
                    new QueueDuplicateRemovedScheduler(),
                    // 为优酷自定义的 Processor
                    new YoukuPageProcessor())
                //为优酷自定义的 Pipeline
                .AddPipeline(new YoukuPipeline());

            spider.Downloader = new HttpClientDownloader();
            spider.ThreadNum = 10;
            spider.EmptySleepTime = 3000;

            // 启动爬虫
            spider.Run();

        }
    }
}
