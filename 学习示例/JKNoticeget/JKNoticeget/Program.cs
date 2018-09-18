using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Topshelf;

namespace JKNoticeget
{
    class Program
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\log4net.config");
            XmlConfigurator.ConfigureAndWatch(fi);
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<ToExcelRunner>(s =>
                    {
                        s.ConstructUsing(name => new ToExcelRunner());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });
                    x.RunAsLocalSystem();

                    x.SetDescription("每天晚上8点讲当日新闻保存为Excel");
                    x.SetDisplayName("新闻保存服务");
                    x.SetServiceName("新闻保存服务");
                });

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }
}
