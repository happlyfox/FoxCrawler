using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;

namespace YouKuDotnetSpider2
{
    /// <summary>
    /// 优酷爬虫管道(保存文件)
    /// </summary>
    public class YoukuPipeline : BasePipeline
    {
        public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
        {
            ResultItems result = resultItems.First();
            StringBuilder builder = new StringBuilder();

            YouKu entry = result.Results["VideoResult"];
            builder.Append($"输出 [YouKu {entry.id}] {entry.videoNum}");
            Console.WriteLine(builder.ToString());

            // Other actions like save data to DB. 可以自由实现插入数据库或保存到文件
        }
    }
}
