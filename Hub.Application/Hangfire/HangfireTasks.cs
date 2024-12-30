using Hangfire.Server;
using Hangfire;

namespace Hub.Application.Hangfire
{
    public class HangfireTasks
    {
        const long DayInSeconds = 60 * 60 * 24;

        [CustomAutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(3600)]
        public void UpdateChangedClientView360(PerformContext context)
        {
            //Singleton<LoopTenantManager>.Instance.LoopTenants(context?.Job?.ToString(), () =>
            //{
            //    Engine.Resolve<ClientView360Service>().ProcessChangedClientView360();
            //}, (type, message) => HangfireAction.Log(context, type, message));
        }
    }
}
