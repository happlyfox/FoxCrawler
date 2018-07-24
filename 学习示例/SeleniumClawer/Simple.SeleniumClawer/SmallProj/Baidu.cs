using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Simple.SeleniumClawer
{
    /// <summary>
    /// 百度模拟登陆
    /// </summary>
    public class Baidu
    {
        private static string oldwin;

        //贴吧一键签到
        public static void OnTimeSign(ChromeDriver driver)
        {
            driver.FindElement(By.XPath("//*[@id=\"u_sp\"]/a[5]")).Click();
            driver.SwitchTo().Window(driver.WindowHandles[1]);//操作权限为第二个页签
            var tiebaList = driver.FindElements(By.XPath("//*[@id=\"likeforumwraper\"]/a"));
            foreach (var tieba in tiebaList)
            {
                tieba.Click();
                driver.SwitchTo().Window(driver.WindowHandles[2]);//操作权限为第三个页签
                driver.FindElement(By.XPath("//*[@id=\"signstar_wrapper\"]/a")).Click();
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[1]);//操作权限为第二个页签
            }
            driver.Navigate().Refresh();
            driver.Close();
            driver.SwitchTo().Window(oldwin);
        }


        /// <summary>
        /// 百度账号登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        public static void Login(string userName, string pwd)
        {
            var driver = new ChromeDriver();
            driver.Manage().Window.Maximize();//浏览器最大化
            driver.Navigate().GoToUrl("https://www.baidu.com");
            oldwin = driver.CurrentWindowHandle;//首页签句柄
            driver.FindElement(By.XPath("//*[@id=\"u1\"]/a[7]")).Click();//点击登陆

            /*隐式等待设置的内容在driver的整个生命周期都有效，所以实际使用过程当中有弊端。*/
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 0, 5);

            driver.FindElement(By.Id("TANGRAM__PSP_10__footerULoginBtn")).Click();//点击用户名登陆
            driver.FindElement(By.Name("userName")).SendKeys(userName);//用户名
            driver.FindElement(By.Name("password")).SendKeys(pwd);//密码
            driver.FindElement(By.Id("TANGRAM__PSP_10__submit")).Click();   //点击登陆
            Thread.Sleep(1000);

            try
            {
                //判断是否存在手机验证码
                driver.FindElement(By.Id("TANGRAM__36__button_send_mobile")).Click();//发送手机验证码
                string vcode = Console.ReadLine();
                driver.FindElement(By.Id("TANGRAM__36__input_vcode")).SendKeys(vcode);//输入6为数字验证码
                driver.FindElement(By.Id("TANGRAM__36__button_submit")).Click();//确认
            }
            catch (Exception e)
            {
            }
        }
    }
}
