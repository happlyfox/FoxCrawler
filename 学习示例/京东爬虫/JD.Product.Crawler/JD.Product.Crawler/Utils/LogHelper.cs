using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Utils
{
    /// <summary>  
    /// LogHelper的摘要说明。   
    /// </summary>   
    public class LogHelper
    {
        /// <summary>
        /// 静态只读实体对象info信息
        /// </summary>
        public static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("loginfo");
        /// <summary>
        ///  静态只读实体对象error信息
        /// </summary>
        public static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");

        public static void Init()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                       ConfigurationManager.AppSettings["log4net"];
            var file = new System.IO.FileInfo(path);
            log4net.Config.XmlConfigurator.Configure(file);
        }

        /// <summary>
        ///  添加info信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        public static void WriteLog(string info, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            if (Loginfo.IsInfoEnabled)
            {
                Console.WriteLine(info);
                Loginfo.Info(info);
            }
        }

        /// <summary>
        /// 添加异常信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        /// <param name="ex">异常信息</param>
        public static void WriteLog(string info, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (Loginfo.IsErrorEnabled)
            {
                Console.WriteLine(info);
                Logerror.Error(info, ex);
            }
        }
    }
}
