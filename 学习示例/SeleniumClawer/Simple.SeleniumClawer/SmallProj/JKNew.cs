using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using Simple.SeleniumClawer.Model;

namespace Simple.SeleniumClawer
{
    /// <summary>
    /// 金陵科技学院新闻爬取
    /// </summary>
    public class JKNew
    {
        /// <summary>
        /// 鼠标移动到对应位置，依次点开链接。在最后一个窗口打开后，关闭除了第一个的其他所有页面
        /// </summary>
        public static void SwitchCollege()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2); //图片禁用
            var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://www.jit.edu.cn/");

            string oldwin = driver.CurrentWindowHandle;//当前窗口句柄
            IWebElement linkElement = driver.FindElementByXPath("//*[@id=\"Form1\"]/div[3]/div[2]/div[4]/ul/li[1]");
            Actions action = new Actions(driver);
            action.MoveToElement(linkElement).Perform();

            var links = linkElement.FindElements(By.XPath("./ul/li/a"));
            foreach (var link in links.Take(2).ToList())
            {
                action.MoveToElement(link).Click().Perform();
                Thread.Sleep(500);
            }

            driver.SwitchTo().Window(oldwin);//移动到当前窗口
            //获取所有的WindowHandle，关闭所有子窗口
            ReadOnlyCollection<string> windows = driver.WindowHandles;
            foreach (var win in windows)
            {
                if (win != oldwin)
                {
                    driver.SwitchTo().Window(win).Close();
                }
                Thread.Sleep(500);
            }

            Console.Read();
        }

        /// <summary>
        /// 爬取学校新闻类别的所有数据
        /// </summary>
        public static void StartNewsClawer()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var docHtml = new HtmlDocument();
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://www.jit.edu.cn/myNews_list_out.aspx?infotype=1");

            bool nextpage = true;
            do
            {
                ReadOnlyCollection<IWebElement> newsNodes =
                    driver.FindElements(By.XPath("//*[@id=\"table_list\"]/li/a")); //获取li内容
                GetNewList(newsNodes);
                docHtml.LoadHtml(driver.PageSource);
                //找到下一页按钮
                HtmlNode node = docHtml.GetElementbyId("nextpage");
                IWebElement element = null;
                if (node != null)
                {
                    element = driver.FindElementById("nextpage");
                }
                else
                {
                    nextpage = false;
                }

                //如果存在下一页按钮，模拟点击
                if (nextpage)
                {
                    element.Click();
                }
            } while (nextpage);

            sw.Stop();
            Console.WriteLine($"总时长 {sw.ElapsedMilliseconds / 1000 / 60}分钟");
            Console.Read();
        }

        /// <summary>
        /// 获得新闻内容
        /// </summary>
        /// <param name="newsNodes"></param>
        /// <returns></returns>
        private static List<NewInfo> GetNewList(ReadOnlyCollection<IWebElement> newsNodes)
        {
            List<NewInfo> newInfoList = new List<NewInfo>();
            foreach (var news in newsNodes)
            {
                newInfoList.Add(new NewInfo()
                {
                    Url = news.GetAttribute("href"),
                    Title = news.Text
                });
                Console.WriteLine($"{news.Text} {news.GetAttribute("href")}");
            }
            return newInfoList;
        }

    }
}
