namespace Hub.Infrastructure.Architecture
{
    /// <summary>
    /// Gerenciador das configurações do pool de threads da aplicação
    /// </summary>
    public static class ThreadPoolManager
    {
        /// <summary>
        /// Configura o número máximo de workers threads da aplicação de acordo com a configuração WORKER_THREADS e IOCP_THREADS.
        /// Deve ser executado apenas na inicialização do app.
        /// </summary>
        public static void Configure()
        {
            int currentMinWorker, currentMinIOC;
            int currentMaxWorker, currentMaxIOC;

            // Get the current settings.
            ThreadPool.GetMinThreads(out currentMinWorker, out currentMinIOC);
            ThreadPool.GetMaxThreads(out currentMaxWorker, out currentMaxIOC);

            int workerThreads = string.IsNullOrEmpty(Engine.AppSettings["WORKER_THREADS"])
                ? currentMinWorker
                : Convert.ToInt32(Engine.AppSettings["WORKER_THREADS"]);

            int iocpThreads = string.IsNullOrEmpty(Engine.AppSettings["IOCP_THREADS"])
                ? currentMinIOC
                : Convert.ToInt32(Engine.AppSettings["IOCP_THREADS"]);

            if (workerThreads > currentMaxWorker)
                currentMaxWorker = workerThreads;

            if (iocpThreads > currentMaxIOC)
                currentMaxIOC = iocpThreads;

            ThreadPool.SetMaxThreads(currentMaxWorker, currentMaxIOC);
            ThreadPool.SetMinThreads(workerThreads, iocpThreads);
        }
    }
}
