using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;

namespace YouKuDotnetSpider2Async
{
    public class YoukuPipeline : BasePipeline
    {
        public override void Process(IEnumerable<ResultItems> resultItems, ISpider spider)
        {
            var resultItem= resultItems.FirstOrDefault();

            foreach (VideoContent entry in resultItem.GetResultItem("VideoResult"))
            {
                Console.WriteLine($"{entry.Title}\t\t{entry.Href}");
            }
        }
    }
}
