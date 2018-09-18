using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace JKNoticeget
{
    public class ToExcelRunner 
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ToExcelRunner));
        private readonly IScheduler scheduler;

        public ToExcelRunner()
        {
            // 创建一个调度器
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //2、创建一个任务
            IJobDetail job = JobBuilder.Create<ToExcelJob>().WithIdentity("job1", "group1").Build();
            //3、创建一个触发器
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule("0 0 20 ? * *")     //每天晚上8点执行
                .Build();
            //4 将任务与触发器添加到调度器中并执行
            scheduler.ScheduleJob(job, trigger);
        }

        public void Start()
        {
            try
            {
                _log.Info("服务开启");
                scheduler.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        
        }

        public void Stop()
        {
            _log.Info("服务结束");
            scheduler.Shutdown(false);
        }
    }
}
