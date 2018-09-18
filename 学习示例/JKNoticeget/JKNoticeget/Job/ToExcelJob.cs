using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.Npoi;
using JKNoticeget.Model;
using log4net;
using Quartz;

namespace JKNoticeget
{
    public class ToExcelJob : IJob
    {
        private static string excelPath = ConfigurationManager.AppSettings["ExcelPath"];
        static readonly ILog Log = LogManager.GetLogger(typeof(ToExcelJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (!Directory.Exists(excelPath))
                {
                    Directory.CreateDirectory(excelPath);
                }

                var items = JKNews.GetTodayNews();
                var excel = new Excel();
                excel.CreateSheet("Sheet1");
                int rowIndex = 0;
                excel.WriteTitle(new string[] { "链接", "标题", "部门", "日期" }, 0, 0);
                rowIndex++;
                foreach (var item in items)
                {
                    excel.CreateRow(0, rowIndex);
                    excel.WriteProperty<Notice>(item, 0, rowIndex, 0);
                    rowIndex++;
                }
                excel.SetColumnWidth(0, 0, new[] { 20, 30, 10, 10 });
                string savePath = Path.Combine(excelPath, string.Format("{0}.xlsx", DateTime.Now.ToString("yyyy.MM.dd")));
                excel.WriteFile(savePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }
    }
}
