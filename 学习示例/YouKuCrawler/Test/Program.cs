using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Processor;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var site = new Site { CycleRetryTimes = 3, SleepTime = 300 };
            var spider = Spider.Create(site, new GithubProfileProcessor()).AddStartUrl("https://github.com/zlzforever");
            spider.ThreadNum = 5;
            spider.Run();
            Console.Read();
        }
    }

    class GithubProfileProcessor : BasePageProcessor
    {
        protected override void Handle(Page page)
        {
            page.AddResultItem("author", page.Selectable.XPath("//div[@class='p-nickname vcard-username d-block']").GetValue());
            var name = page.Selectable.XPath("//span[@class='p-name vcard-fullname d-block']").GetValue();
            page.Skip = string.IsNullOrWhiteSpace(name);
            page.AddResultItem("name", name);
            page.AddResultItem("bio", page.Selectable.XPath("//div[@class='p-note user-profile-bio']/div").GetValue());
        }
    }
}
