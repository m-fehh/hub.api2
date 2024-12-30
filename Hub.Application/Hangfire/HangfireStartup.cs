using Hangfire;
using Hangfire.Storage;
using Hub.Infrastructure.Architecture;
using System.Linq.Expressions;

namespace Hub.Application.Hangfire
{
    public class HangfireStartup
    {
        private readonly List<string> ActiveRecurringJobsId = new List<string>();
        private readonly IRecurringJobManager recurringJobManager;
        public class HangfireRecurringJob
        {
            public string RecurringJobId { get; set; }
            public Expression<Action> MethodCall { get; set; }
            public string CronExpression { get; set; }
        }

        public HangfireStartup(IRecurringJobManager recurringJobManager)
        {
            this.recurringJobManager = recurringJobManager;
        }

        private List<HangfireRecurringJob> InitializeHangfireRecurringJobs()
        {
            var hangfireExecutionMode = Engine.AppSettings["hangfireExecutionMode"];

            var monthlyIntervalTasks = new List<HangfireRecurringJob>
            {
                //new HangfireRecurringJob { RecurringJobId = "ShortenerURL - Expirar registros antigos", MethodCall = () => new HangfireTasks().DeleteOldShortenerUrlRecords(null), CronExpression = "10 0 1 1/6 *" }
            };

            //var all = neverRunTasks
            //    .Union(minuteIntervalTasks)
            //    .Union(hourIntervalTasks)
            //    .Union(dailyIntervalTasks)
            //    .Union(monthlyIntervalTasks)
            //    .ToList();

            var all = monthlyIntervalTasks.ToList();

            if (hangfireExecutionMode == "MANUAL")
            {
                all.ForEach(h => h.CronExpression = "");
            }

            return all;
        }

        public void ConfigureHangfire()
        {
            var useHangfire = Convert.ToBoolean(Engine.AppSettings["useHangfire"]);

            if (useHangfire)
            {
                ReplaceDefaultRetryFilter();

                List<HangfireRecurringJob> hangfireRecurringJobs = InitializeHangfireRecurringJobs();

                hangfireRecurringJobs = hangfireRecurringJobs.OrderBy(o => o.RecurringJobId).ToList();

                foreach (var hangfireRecurringJob in hangfireRecurringJobs)
                {
                    CreateRecurringJob(hangfireRecurringJob);
                }

                CleanInactiveRecurringJobs();
            }
        }

        private void ReplaceDefaultRetryFilter()
        {
            var automaticRetryFilter = GlobalJobFilters.Filters.FirstOrDefault(x => x.Instance.GetType() == typeof(AutomaticRetryAttribute));
            if (automaticRetryFilter != null)
            {
                GlobalJobFilters.Filters.Remove(automaticRetryFilter.Instance);
                GlobalJobFilters.Filters.Add(new CustomAutomaticRetryAttribute() { Attempts = 5 });
            }
        }

        private void CreateRecurringJob(HangfireRecurringJob hangfireRecurringJob)
        {
            if (hangfireRecurringJob != null)
            {
                if (string.IsNullOrWhiteSpace(hangfireRecurringJob.CronExpression))
                {
                    recurringJobManager.AddOrUpdate(hangfireRecurringJob.RecurringJobId, hangfireRecurringJob.MethodCall, Cron.Never(), TimeZoneInfo.Local);
                }
                else
                {
                    recurringJobManager.AddOrUpdate(hangfireRecurringJob.RecurringJobId, hangfireRecurringJob.MethodCall, hangfireRecurringJob.CronExpression, TimeZoneInfo.Local);
                }

                ActiveRecurringJobsId.Add(hangfireRecurringJob.RecurringJobId);
            }
        }

        private void CleanInactiveRecurringJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();

                foreach (var recurringJob in recurringJobs)
                {
                    if (recurringJob.LoadException != null && recurringJob.LoadException.InnerException != null
                        && recurringJob.LoadException.InnerException.Message.Contains("does not contain a method with signature"))
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }

                foreach (var inactiveRecurringJobId in recurringJobs.Where(w => !ActiveRecurringJobsId.Contains(w.Id)))
                {
                    RecurringJob.RemoveIfExists(inactiveRecurringJobId.Id);
                }
            }
        }
    }
}
