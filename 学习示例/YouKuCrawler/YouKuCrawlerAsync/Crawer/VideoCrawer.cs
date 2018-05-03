using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace YouKuCrawlerAsync
{
    /// <summary>
    ///     优酷爬虫
    /// </summary>
    public class VideoCrawer
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
        ///     打印得到的内容
        /// </summary>
        public static void PrintContent()
        {
            var count = 0;
            foreach (var node in GetVideoTypes())
            {
                var resultLi = new List<VideoContent>();
                //得到当前类别总分页数
                var pageCount = GetPageCountByCode(node.Code);
                //遍历分页得到内容
                for (var i = 1; i <= pageCount; i++) resultLi.AddRange(GetContentsByCode(node.Code, i));
                Console.WriteLine($"编码{node.Code} \t 页数{pageCount} \t 总个数{resultLi.Count}");
                count += resultLi.Count;
            }

            Console.WriteLine($"总个数为{count}");
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

        /// <summary>
        ///     得到当前类别的内容
        /// </summary>
        public static List<VideoContent> GetContentsByCode(string code, int pageIndex)
        {
            var web = new HtmlWeb();
            var doc = web.Load($"http://list.youku.com/category/show/{code}_s_1_d_1_p_{pageIndex}.html");

            var returnLi = new List<VideoContent>();
            var contents = doc.DocumentNode.CssSelect(".yk-col4").ToList();

            foreach (var node in contents)
                returnLi.Add(new VideoContent
                {
                    PageIndex = pageIndex.ToString(),
                    Code = code,
                    Title = node.CssSelect(".info-list .title a").FirstOrDefault()?.InnerText,
                    Hits = node.CssSelect(".info-list li").LastOrDefault()?.InnerText,
                    Href = node.CssSelect(".info-list .title a").FirstOrDefault()?.Attributes["href"].Value,
                    ImgHref = node.CssSelect(".p-thumb img").FirstOrDefault()?.Attributes["Src"].Value
                });

            return returnLi;
        }
    }
}