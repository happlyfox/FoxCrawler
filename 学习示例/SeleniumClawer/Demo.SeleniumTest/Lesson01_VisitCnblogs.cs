using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace Demo.SeleniumTest
{
    public class Lesson01_VisitCnblogs
    {
        /// <summary>
        /// 访问博客园
        /// </summary>
        [Fact(DisplayName = "Visit.Cnblogs")]
        public void Visit_Cnblogs()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Url = "http://www.cnblogs.com/NorthAlan";
            var lnkAutomation = driver.FindElement(By.XPath(".//div[@id='sidebar_postcategory']/ul/li/a[text()='架构设计']"));
            lnkAutomation.Click();
        }
    }
}
