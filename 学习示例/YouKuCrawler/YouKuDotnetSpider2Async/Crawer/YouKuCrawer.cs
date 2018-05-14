using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace YouKuDotnetSpider2Async.Crawer
{
   public class YouKuCrawer
    {
        private static readonly string _url = "http://list.youku.com/category/video/c_0.html";

        /// <summary>
        ///     得到所有的类别
        /// </summary>
        public static List<VideoType> GetVideoTypes()
        {
            //加载web内容
            var web = new HtmlWeb();
            var doc = web.Load(_url);

            //内容解析-获得所有的类别
            var allTypes = doc.DocumentNode.SelectNodes("//*[@id='filterPanel']/div/ul/li/a").ToList();

            //类别列表中去掉【全部】这个选项
            var typeResults = allTypes.Where((u, i) => { return i > 0; }).ToList();

            var reList = new List<VideoType>();
            foreach (var node in typeResults)
            {
                var href = node.Attributes["href"].Value;
                reList.Add(new VideoType
                {
                    Code = href.Substring(href.LastIndexOf("/") + 1, href.LastIndexOf(".") - href.LastIndexOf("/") - 1),
                    Name = node.InnerText
                });
            }
            return reList;
        }

        /// <summary>
        ///     得到当前类别的总页数
        /// </summary>
        public static int GetPageCountByCode(string code)
        {
            var web = new HtmlWeb();
            var doc = web.Load($"http://list.youku.com/category/show/{code}.html");

            //分页列表
            var pageList = doc.DocumentNode.CssSelect(".yk-pages li").ToList();
            //得到倒数第二项
            var lastsecond = pageList[pageList.Count - 2];
            return Convert.ToInt32(lastsecond.InnerText);
        }
    }
}
