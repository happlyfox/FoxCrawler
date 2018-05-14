using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Selector;

namespace YouKuDotnetSpider2
{
    /// <summary>
    /// 优酷单页面解析类
    /// </summary>
    public class YoukuPageProcessor : BasePageProcessor
    {
        protected override void Handle(Page page)
        {
            // 利用 Selectable 查询并构造自己想要的数据对象
            var totalVideoElement = page.Selectable.Select(Selectors.XPath("//*[@id='total_videonum']"));
            var id = totalVideoElement.XPath("@id").GetValue();
            string videoNum = totalVideoElement.GetValue();

            //得到电影类别
            var linqNodes = page.Selectable.Select(Selectors.XPath("//*[@id='filterPanel']/div[2]/ul")).XPath("li").Nodes();
            var videoCountry = new List<string>();
            foreach (var node in linqNodes)
            {
                string text = node.GetValue(ValueOption.InnerText);
                videoCountry.Add(text);
                //Console.WriteLine($"{text}");
            }

            //整合实体
            var results = new YouKu()
            {
                id = id,
                videoNum = Convert.ToInt32(videoNum),
                videoCountry = videoCountry
            };

            // 以自定义KEY存入page对象中供Pipeline调用
            page.AddResultItem("VideoResult", results);

        }
    }
}
