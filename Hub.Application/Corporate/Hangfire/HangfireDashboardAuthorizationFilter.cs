﻿using Hangfire.Dashboard;

namespace Hub.Application.Corporate.Hangfire
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //return Engine.Resolve<ISecurityProvider>().Authorize("HF");
            return true;
        }
    }
}
