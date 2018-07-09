using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Utils;
using Newtonsoft.Json;

namespace JD.Product.Crawler.Search
{
    /// <summary>
    /// 产品爬取
    /// </summary>
    public class ProductSearch
    {

        /// <summary>
        /// 得到分页总数
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>分页数</returns>
        public static int GetPageNum(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var node = doc.DocumentNode.CssSelect(".fp-text i");
            return Convert.ToInt32(node.FirstOrDefault()?.InnerText);
        }

        /// <summary>
        /// 得到当前页数据
        /// </summary>
        /// <param name="url"></param>
        public static  List<ProductUnit> GetPageDataAsync(string url, int pageIndex, int cateId)
        {
            List<ProductUnit> productList = new List<ProductUnit>();
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(string.Format($"{url}&page={pageIndex}"));

                //使用ScrapySharp进行Css定位
                var productHtmlNodes = doc.DocumentNode.CssSelect(".gl-item");

                //遍历商品
                foreach (var productHtmlNode in productHtmlNodes)
                {
                    var block = productHtmlNode.CssSelect(".p-name a"); //名称区块

                    string href = block.FirstOrDefault()?.Attributes["href"]?.Value;
                    string title = block.CssSelect("em").FirstOrDefault()?.InnerText.Trim();
                    Match match = Regex.Match(href, @"(\d+).html");
                    string id = "0";
                    if (match.Success)
                    {
                        id = match.Groups[1].Value;
                    }

                    productList.Add(new ProductUnit()
                    {
                        CateId = cateId,
                        ProductId = Convert.ToInt64(id),
                        Title = title,
                        Url = $"https:{href}"
                    });
                }

                //价格通过模拟jsonp请求得到
                //!!钱取不到值的问题，研究!!
                string idStrs = string.Join(",", productList.Select(u => "J_" + u.ProductId).ToArray());
                string priceJsonpUrl =
                    $"https://p.3.cn/prices/mgets?callback=jQuery5702621&ext=11000000&pin=&type=1&area=1_72_4137_0&skuIds={idStrs}";

                var priceDoc =  web.LoadFromWebAsync(priceJsonpUrl);
                Match jsonMatch = Regex.Match(priceDoc.Result.DocumentNode.InnerText, @"\[.*\]");
                List<Price> priceList = new List<Price>();
                if (jsonMatch.Success)
                {
                    priceList = JsonConvert.DeserializeObject<List<Price>>(jsonMatch.Value);
                }
                else
                {
                    Console.WriteLine(priceJsonpUrl);
                }

                priceList.ForEach(u =>
                {
                    productList.FirstOrDefault(p => p.ProductId == Convert.ToInt64(u.id.Substring(2))).Price = u.p;
                });


            }
            catch (Exception e)
            {
                LogHelper.WriteLog(e.Message, e);
            }
            return productList;
        }
    }
}
