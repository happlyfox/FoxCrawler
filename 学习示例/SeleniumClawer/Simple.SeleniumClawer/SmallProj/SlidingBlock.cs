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
using Simple.SeleniumClawer.Model;

namespace Simple.SeleniumClawer
{
    /// <summary>
    /// 模拟解锁
    /// </summary>
    public class SlidingBlock
    {
        public static void StartClawer()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2); //图片
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.helloweba.net/demo/2017/unlock/");
            var block = driver.FindElement(By.ClassName("slide-to-unlock-handle"));
            var action = new Actions(driver);

            //鼠标点击并按下
            action.ClickAndHold(block).Perform();
            try
            {
                for (int i = 1; i <= 50; i++)
                {
                    //每次移动2个像素
                    action.MoveByOffset(5, 0).Perform();
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("解锁成功");
            }

            var alertText = driver.SwitchTo().Alert().Text;
            Console.WriteLine(alertText);
        }
    }
}
