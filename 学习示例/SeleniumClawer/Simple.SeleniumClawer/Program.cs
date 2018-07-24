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
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.PhantomJS;
using Simple.SeleniumClawer.Model;

namespace Simple.SeleniumClawer
{
    class Program
    {
        static void Main(string[] args)
        {
            //一次演示一个
            #region 百度一键签到
            Console.WriteLine("请输入用户名");
            string userName = Console.ReadLine();
            Console.WriteLine("密码");
            string pwd = Console.ReadLine();
            Baidu.Login(userName, pwd);
            #endregion

            //#region 滑块认证demo
            //SlidingBlock.StartClawer();
            //#endregion

            //#region 新闻列表获取+模拟鼠标 移动
            //JKNew.StartNewsClawer();
            //#endregion
        }
    }
}
