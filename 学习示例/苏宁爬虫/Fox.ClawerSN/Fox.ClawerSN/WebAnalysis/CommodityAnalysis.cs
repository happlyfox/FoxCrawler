using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Util;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Extensions;

namespace Fox.ClawerSN.WebAnalysis
{
    public class CommodityAnalysis
    {

        //https://list.suning.com/emall/showProductList.do?ci=20006&pg=03&cp=4
        //https://list.suning.com/emall/showProductList.do?ci=20006&pg=03&cp=4&paging=1&sub=1
        public static List<POCO_Commodity> GetData(string url, string catId, int pageIndex)
        {
            pageIndex = pageIndex - 1;
            var match = Regex.Match(url, @"-(\d+)-");
            if (!match.Success)
            {
                Console.WriteLine($"CommodityAnalysis.GetData url:{url}");
                return null;
            }

            string shopId = match.Value.Trim('-');
            var web = new HtmlWeb();
            string[] shopUrlStrs = new string[]
            {
                $"https://list.suning.com/emall/showProductList.do?ci={shopId}&pg=03&cp={pageIndex}",
                $"https://list.suning.com/emall/showProductList.do?ci={shopId}&pg=03&cp={pageIndex}&paging=1&sub=1"
            };

            List<POCO_Commodity> commodityList = new List<POCO_Commodity>();
            foreach (var shopUrl in shopUrlStrs)
            {
                commodityList.AddRange(GetShopList(web.Load(shopUrl).DocumentNode, shopUrl.EndsWith("sub=1"), catId));
            }

            return commodityList;
        }

        private static List<POCO_Commodity> GetShopList(HtmlNode htmlNode, bool isSecond, string catId)
        {
            List<POCO_Commodity> commodityList = new List<POCO_Commodity>();
            StringBuilder priceJsonpSb = new StringBuilder();
            List<string> strList = new List<string>();
            int priceInitCount = 1;

            string xPath = "//*[@id=\"filter-results\"]/ul/li";
            if (isSecond == true)
            {
                xPath = "./li";
            }
            var shopNode = htmlNode.SelectNodes(xPath);
            if (shopNode != null)
            {
                foreach (var shop in shopNode)
                {
                    var contentNode = shop.SelectSingleNode("./div/div/div/div[2]/p[2]/a");
                    if (contentNode != null)
                    {
                        string title = contentNode.InnerText;
                        string description = contentNode.SelectSingleNode("./em")?.InnerText;//简单介绍
                        string aUrl = $"https:{contentNode.Attributes["href"].Value}";
                        string shopName = shop.SelectSingleNode("./div/div/div/div[2]/p[4]")?.InnerText;

                        //For Example. 0000000000 - 10518836389 | 0000000000 - 104553759
                        var codeMatch = Regex.Match(shop.Attributes["class"].Value, "(\\d+)-(\\d+)");
                        if (codeMatch.Success)
                        {
                            long suid = Convert.ToInt64(codeMatch.Groups[2].Value);
                            commodityList.Add(new POCO_Commodity()
                            {
                                SUId = suid.ToString(),
                                Title = title,
                                Description = description,
                                Url = aUrl,
                                CategoryId = catId,
                                ShopName = shopName
                            });

                            priceJsonpSb.Append($"{codeMatch.Groups[2].Value}_");
                            if (codeMatch.Groups[1].Value != "0000000000")
                            {
                                priceJsonpSb.Append($"_2_{codeMatch.Groups[1].Value}");
                            }
                            priceJsonpSb.Append(",");

                            if (priceInitCount == 15)
                            {
                                strList.Add(priceJsonpSb.ToString());
                                priceJsonpSb.Clear();
                            }

                            priceInitCount++;
                        }
                    }
                }
                strList.Add(priceJsonpSb.ToString());
                /*
                * 获取价格接口
                 *https://ds.suning.cn/ds/generalForTile/000000010009763888_,000000010248177027_,000000000945032708_,000000000945031409_,000000000617087553_-025-2-0000000000-1--ds0000000005492.jsonp?callback=ds0000000005492
                 *https://ds.suning.cn/ds/generalForTile/000000000171958803__2_0000000000,000000000776997041_,000000000176455058__2_0070129296,000000010559146415_,000000010372764620__2_0000000000-025-2-0070079092-1--ds0000000004363.jsonp?callback=ds0000000004363
                 */


                /*
                 * 000000010035285781_-025-2-0000000000-1--ds000000000449.jsonp?callback=ds000000000449
                 * 000000000740963441_-025-2-0000000000-1--ds0000000004394.jsonp?callback=ds0000000004394
                 * 000000010175632408_-025-2-0000000000-1--ds0000000001594.jsonp?callback=ds0000000001594
                 */

                List<Priceobject> priceList = new List<Priceobject>();
                foreach (var str in strList)
                {
                    string numrandom = new Random().Next(100, 9999).ToString();
                    string priceStr = $"https://ds.suning.cn/ds/generalForTile/{str}000000000{new Random().Next(100, 999999999)}_-025-2-0000000000-1--ds000000000{numrandom}.jsonp?callback=ds000000000{numrandom}";
                    var priceMidList = GetPriceList(priceStr);
                    if (priceMidList != null)
                    {
                        priceList.AddRange(priceMidList);
                    }
                }

                //000000010375342473 
                //000000010395026433
                //000000000690128134

                if (priceList.Count < commodityList.Count)
                {
                    Console.WriteLine($"商品列表中存在无法匹配价格数据");
                }

                foreach (var commodity in commodityList)
                {
                    var priceModel = priceList.FirstOrDefault(u => u.cmmdtyCode == commodity.SUId.ToString().PadLeft(18, '0'));
                    if (priceModel != null)
                    {
                        commodity.Price = priceModel.price;
                    }
                }

            }
            return commodityList;
        }

        public static List<Priceobject> GetPriceList(string url)
        {
            var web = new HtmlWeb();
            var result = web.LoadFromWebAsync(url);
            var match = Regex.Match(result.Result.ParsedText, @"\""rs\"":(.*])");

            if (!match.Success)
            {
                Console.WriteLine($"无法获取价格 CommodityAnalysis.GetPriceList {url}");
            }
            return JsonConvert.DeserializeObject<List<Priceobject>>(match.Groups[1].Value);
        }
    }
}
