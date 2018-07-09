using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.Model;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace Fox.ClawerSN.WebAnalysis
{
    /// <summary>
    /// 类别加载 https://list.suning.com/#20089
    /// </summary>
    public class CategoryAnalysis
    {
        public static List<POCO_Category> GetData()
        {
            return CombineA_B_C();
            //打印输出
            //foreach (var node in nodes)
            //{
            //    Console.WriteLine($"id:{node.Id} pid:{node.PId} code:{node.Code} name:{node.Name} levle:{node.Levels} url:{node.Url}");
            //}
        }

        private static List<POCO_Category> CombineA_B_C()
        {
            List<POCO_Category> AList = new List<POCO_Category>();

            int idIndex = 1;
            foreach (HtmlNode xNode in InitA())
            {
                POCO_Category aModel = new POCO_Category()
                {
                    Id = ToLevelCode(idIndex),
                    PId = "000",
                    Levels = 1,
                    Code = ToLevelCode(idIndex),
                    Name = xNode.SelectSingleNode("./h2").InnerText
                };
                AList.Add(aModel);

                var blist = InitB(aModel, xNode);
                AList.AddRange(blist);
                idIndex = idIndex + blist.Count + 1;
            }

            return AList;
        }

        private static List<HtmlNode> InitA()
        {
            var url = "https://list.suning.com/#20089";
            var web = new HtmlWeb();
            var docWeb = web.Load(url);
            //var cssNodes = docWeb.DocumentNode.CssSelect(".search-main.introduce.clearfix > div").ToList();//147毫秒
            List<HtmlNode> xpathNodes = docWeb.DocumentNode.SelectNodes("/html/body/div[5]/div[2]/div").ToList();
            return xpathNodes;
        }

        private static List<POCO_Category> InitB(POCO_Category parentModel, HtmlNode node)
        {
            int idIndex = Convert.ToInt32(parentModel.Id) + 1;
            List<POCO_Category> bList = new List<POCO_Category>();

            var xNodes = node.SelectNodes("./div").ToList();
            foreach (var xNode in xNodes)
            {
                var cateModel = xNode.SelectSingleNode("./div[1]/a");
                POCO_Category bModel = new POCO_Category()
                {
                    Id = ToLevelCode(idIndex),
                    PId = parentModel.Id,
                    Code = $"{parentModel.Code}_{ToLevelCode(idIndex)}",
                    Name = cateModel.InnerText,
                    Url = $"https:{cateModel.GetAttributeValue("href")}",
                    Levels = 2
                };
                bList.Add(bModel);

                var clist = InitC(bModel, xNode.SelectSingleNode("./div[2]"));
                bList.AddRange(clist);
                idIndex = idIndex + clist.Count + 1;
            }

            return bList;
        }

        private static List<POCO_Category> InitC(POCO_Category parentModel, HtmlNode node)
        {
            int idIndex = Convert.ToInt32(parentModel.Id) + 1;
            List<POCO_Category> cList = new List<POCO_Category>();

            HtmlNodeCollection xNodes = node.SelectNodes("./a");

            if (xNodes != null && xNodes.Count > 0)
            {
                foreach (var xNode in xNodes)
                {
                    POCO_Category cModel = new POCO_Category()
                    {
                        Id = ToLevelCode(idIndex),
                        PId = parentModel.Id,
                        Code = $"{parentModel.Code}_{ToLevelCode(idIndex)}",
                        Name = xNode.InnerText,
                        Url = $"https:{xNode.GetAttributeValue("href")}",
                        Levels = 3
                    };
                    cList.Add(cModel);
                    idIndex += 1;
                }
            }

            return cList;
        }

        private static string ToLevelCode(int index)
        {
            return index.ToString("000");
        }
    }
}
