using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JD.Product.Crawler.Model;
using ScrapySharp.Extensions;

namespace JD.Product.Crawler.Search
{
    /// <summary>
    /// 京东类别
    /// </summary>
    public class CategorySearch
    {
        public static List<Category> GetData()
        {
            // 从网站中加载
            var url = "https://www.jd.com/allSort.aspx";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            //使用ScrapySharp进行Css定位
            var cssNodes = doc.DocumentNode.CssSelect(".category-item.m");
            //去掉图书类别
            cssNodes = cssNodes.Where((u, i) => i != 0 && i != 18);

            //A B C 区块
            List<Category> categoryList = new List<Category>();
            foreach (var cateNode in cssNodes)
            {
                //A区块
                var A_Div = cateNode.CssSelect(".mt");
                string ACode = string.Empty;
                //B区块
                var B_Div = cateNode.CssSelect(".mc .items dl");

                //遍历B
                foreach (var dNode in B_Div)
                {
                    //B-A区块
                    var B_A_Div = dNode.CssSelect("dt");
                    //B-B区块
                    var B_B_Div = dNode.CssSelect("dd a");

                    string B_A_Href = B_A_Div.CssSelect("a").FirstOrDefault()?.Attributes["href"].Value;
                    string B_A_Name = B_A_Div.CssSelect("a").FirstOrDefault()?.InnerText;

                    if (B_A_Name == "宠物生活")
                    {
                        continue;
                    }

                    string BCode = string.Empty;
                    if (string.IsNullOrEmpty(ACode))
                    {
                        Match match = Regex.Match(B_A_Href, @"(\d+)-(\d+)");

                        if (match.Groups.Count == 3)
                        {
                            ACode = match.Groups[1].Value;//A编码
                            BCode = string.Format($"{match.Groups[1].Value},{ match.Groups[2].Value}");//B编码
                        }
                    }

                    List<Category> categoryRangeList = new List<Category>();
                    //遍历B-B
                    foreach (var bNode in B_B_Div)
                    {
                        string B_B_Href = bNode.Attributes["href"].Value;
                        string B_B_Name = bNode.InnerText;
                        Match match = Regex.Match(B_B_Href, @"(\d+),(\d+),(\d+)");
                        string CCode = match.Groups[0].Value;

                        if (string.IsNullOrEmpty(BCode))
                        {
                            ACode = string.Format($"{match.Groups[1].Value}");
                            BCode = string.Format($"{match.Groups[1].Value},{match.Groups[2].Value}");
                        }

                        //增加C类别
                        categoryRangeList.Add(new Category()
                        {
                            Code = CCode,//C编码
                            PCode = BCode,
                            Name = B_B_Name,
                            Url = string.Format($"https:{B_B_Href}")
                        });
                    }

                    categoryList.AddRange(categoryRangeList);

                    //增加B类别
                    categoryList.Add(new Category()
                    {
                        Code = BCode,
                        PCode = ACode,
                        Name = B_A_Name,
                        Url = string.Format($"https:{B_A_Href}")
                    });
                }

                //增加A类别
                categoryList.Add(new Category()
                {
                    Code = ACode,
                    PCode = "0",
                    Name = A_Div.CssSelect("span").FirstOrDefault()?.InnerText
                });
            }

            foreach (var cg in categoryList)
            {
                cg.Levels = cg.Code.Split(',').Length;
            }

            return categoryList;
        }
    }
}
