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
    public class CategoryPageAnalysis
    {
        public static int GetData(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var docWeb = web.Load(url);

                var node = docWeb.DocumentNode.SelectSingleNode("//*[@id=\"second-filter\"]/div[2]/div/span");
                int count = 0;
                if (node != null)
                {
                    count = Convert.ToInt32(node.InnerText.Substring(node.InnerText.LastIndexOf('/') + 1));
                }
                return count;
            }
            catch (Exception e)
            {
                Console.WriteLine($"CategoryPageAnalysis {url} {e.Message}");
                return 0;
            }
        }
    }
}
