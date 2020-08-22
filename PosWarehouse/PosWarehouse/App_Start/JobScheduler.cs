using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PosWarehouse;
using Quartz;
using Quartz.Impl;


namespace PosWarehouse
{
    public class JobScheduler
    {
        public static async Task Start()
        {
            //Task<IScheduler> scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //scheduler.Start();

            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<JobClass>()
                .WithIdentity("job1", "group1")
                .Build();

            //IJobDetail job = JobBuilder.Create<JobClass>().Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(120)
                    //.WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}