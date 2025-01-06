namespace Hub.Infrastructure.Architecture
{
    public static class SignalRStartup
    {
        public static void Init()
        {
            //if (Singleton<MessagingPusher>.Instance == null)
            //{
            //    Singleton<MessagingPusher>.Instance = new MessagingPusher();
            //}

            //if (Singleton<AttendanceMonitoringDashboardPusher>.Instance == null)
            //{
            //    Singleton<AttendanceMonitoringDashboardPusher>.Instance = new AttendanceMonitoringDashboardPusher();
            //}

            //if (Singleton<ClientProposalCardPusher>.Instance == null)
            //{
            //    Singleton<ClientProposalCardPusher>.Instance = new ClientProposalCardPusher();
            //}

            //if (Singleton<SubscriptionAppMessagingPusher>.Instance == null)
            //{
            //    Singleton<SubscriptionAppMessagingPusher>.Instance = Engine.Resolve<SubscriptionAppMessagingPusher>();
            //}

            //var tenantName = Singleton<INhNameProvider>.Instance.TenantName();

            //var tokenSource = new CancellationTokenSource();
            //CancellationToken ct = tokenSource.Token;

            //Singleton<MessagingPusher>.Instance.InfinityValidationCancelationTokens.Add(tokenSource);

            //Task.Run(async () =>
            //{

            //    try
            //    {
            //        while (true)
            //        {
            //            using (Engine.BeginLifetimeScope(tenantName))
            //            {
            //                if (ct.IsCancellationRequested)
            //                {
            //                    ct.ThrowIfCancellationRequested();
            //                }

            //                try
            //                {
            //                    await Singleton<MessagingPusher>.Instance.SendMessages();
            //                }
            //                catch (Exception) { }

            //                Thread.Sleep(5000);
            //            }
            //        }
            //    }
            //    catch (OperationCanceledException) { }

            //}, ct);
        }
    }
}
