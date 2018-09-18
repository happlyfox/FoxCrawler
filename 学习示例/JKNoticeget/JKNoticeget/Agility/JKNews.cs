using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JKNoticeget.Model;

namespace JKNoticeget
{
    public class JKNews
    {
        /// <summary>
        /// 得到今日的新闻
        /// </summary>
        /// <returns></returns>
        public static List<Notice> GetTodayNews()
        {
            string[] url =
            {
                "http://www.jit.edu.cn/myNews_list_out.aspx?infotype=1",
                "http://www.jit.edu.cn/myNews_list_out.aspx?infotype=2"
            };

            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.GetEncoding("gb2312");
            List<Notice> noticeItems = new List<Notice>();
            for (int i = 0; i < url.Length; i++)
            {
                var docWeb = web.Load(url[i]);
                var listItems = docWeb.DocumentNode.SelectNodes("//*[@id=\"table_list\"]/li").ToList();
                foreach (var item in listItems)
                {
                    string href = item.SelectSingleNode("./a").Attributes["href"].Value;
                    string title = item.InnerText;
                    string remark = item.SelectSingleNode("./span[@class=\'puber\']").InnerText;

                    var splitArr = remark.Split(' ');
                    string dep = splitArr[0].TrimStart('[');
                    string time = splitArr[1].TrimEnd(']');
                    noticeItems.Add(new Notice()
                    {
                        Href = href,
                        Title = title,
                        Dep = dep,
                        Time = time
                    });
                }
            }

            return noticeItems.Where(u => Convert.ToDateTime(u.Time).Date.Equals(DateTime.Now)).ToList();
        }
    }
}
