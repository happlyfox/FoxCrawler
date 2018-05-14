using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Downloader;
using DotnetSpider.Core.Scheduler;

namespace YouKuDotnetSpider2
{
    class Program
    {
        static void Main(string[] args)
        {
            //爬虫框架参考 http://www.cnblogs.com/modestmt/p/5480773.html
            //使用爬虫框架 DotnetSpider2 进行爬取优酷界面
            //基于框架要求 配置解析器&管道

            CustmizeProcessorAndPipeline();
            Console.Read();
        }

        /// <summary>
        /// 定义处理器和管道
        /// </summary>
        public static void CustmizeProcessorAndPipeline()
        {
            // 定义采集的 Site 对象, 设置 Header、Cookie、代理等
            var site = new Site();
            //  添加初始采集链接
            site.AddStartUrl($"http://list.youku.com/category/show/c_96_s_1_d_1_p_1.html");

            Spider spider = Spider.Create(site,
                    // 使用内存调度
                    new QueueDuplicateRemovedScheduler(),
                    // 为优酷自定义的 Processor
                    new YoukuPageProcessor())
                // 为优酷自定义的 Pipeline
                .AddPipeline(new YoukuPipeline());

            spider.Downloader = new HttpClientDownloader();
            spider.ThreadNum = 1;
            spider.EmptySleepTime = 3000;

            // 启动爬虫
            spider.Run();
        }
    }
}
