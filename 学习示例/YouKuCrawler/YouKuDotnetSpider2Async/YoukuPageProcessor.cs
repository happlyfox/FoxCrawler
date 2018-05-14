using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Selector;
using HtmlAgilityPack;
using YouKuDotnetSpider2Async.Crawer;

namespace YouKuDotnetSpider2Async
{
    public class YoukuPageProcessor : BasePageProcessor
    {
        protected override void Handle(Page page)
        {
            var returnLi = new List<VideoContent>();

            var contents = page.Selectable.SelectList(Selectors.Css(".yk-col4")).Nodes();
            foreach (var node in contents)
            {
                returnLi.Add(new VideoContent
                {
                    Title = node.Css(".info-list .title a").Nodes().FirstOrDefault()?.GetValue(ValueOption.InnerText),
                    Hits = node.Css(".info-list li").Nodes().LastOrDefault()?.GetValue(ValueOption.InnerText),
                    Href = node.Css(".info-list .title a").XPath("@href").GetValue(),
                    ImgHref=node.Css(".p-thumb img").XPath("@src").GetValue()
                });
            }

            //以自定义KEY存入page对象中供Pipeline调用
            page.AddResultItem("VideoResult", returnLi);
        }
    }
}
