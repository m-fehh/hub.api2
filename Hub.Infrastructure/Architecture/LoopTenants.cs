using Hub.Infrastructure.Database;

namespace Hub.Infrastructure.Architecture
{
    public class LoopTenantManager
    {
        /// <summary>
        /// Itera todos os tenants da aplicação. Abre o contexo do autofac e depois executa a ação do parâmetro.
        /// </summary>
        /// <param name="actionId">ID único da ação, serve para fazer a gestão se a tarefa está habilitada no ambiente</param>
        /// <param name="action">Ação a ser executada para cada tenant da aplicação</param>
        /// <param name="logAction">Ação de log recebe os parâmetros tipo de log e mensagem.</param>
        /// <param name="applyTenantSettings">Altera a cultura da thread para a cultura gravada na tabela de gestão dos tenants (adm.Tenants)</param>
        /// <param name="throwOnError">Define se irá estourar o erro ou apenas disparar o log</param>
        public void LoopTenants(string actionId, Action action, Action<string, string> logAction = null, bool applyTenantSettings = false, bool throwOnError = false)
        {
            var mapIds = new[] { "cliente", "default" };

            var mapeamentoBase = Singleton<ConfigurationTenant>.Instance.Mapeamentos.FirstOrDefault(m => mapIds.Contains(m.MapeamentoId));

            foreach (var tenant in mapeamentoBase.ConfigurationTenants.Where(t => !mapIds.Contains(t.Subdomain)))
            {
                using (Engine.BeginLifetimeScope(tenant.Subdomain, applyTenantSettings))
                {
                    try
                    {
                        //if (string.IsNullOrWhiteSpace(actionId) || (!string.IsNullOrWhiteSpace(actionId) && Engine.Resolve<BackgroundJobManager>().IsActive(actionId)))
                        if (string.IsNullOrWhiteSpace(actionId) || !string.IsNullOrWhiteSpace(actionId))
                        {
                            if (logAction != null)
                            {
                                logAction("info", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt")} - Tarefa Iniciada  [{tenant.TenantId} - {tenant.TenantName}] | [{actionId}]");
                            }

                            action();

                            if (logAction != null)
                            {
                                logAction("info", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt")} - Tarefa Finalizada  [{tenant.TenantId} - {tenant.TenantName}] | [{actionId}]");
                            }
                        }
                        else
                        {
                            logAction("error", $"{DateTime.Now} - WARN [{tenant.TenantId} - {tenant.TenantName}]: A tarefa está desligada no ambiente {Engine.AppSettings["environment"]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (logAction != null)
                        {
                            logAction("error", $"{DateTime.Now} - ERRO [{tenant.TenantId} - {tenant.TenantName}]: {ex.Message}\r\n{ex.StackTrace}");
                        }

                        if (throwOnError) throw;
                    }
                }
            }
        }

        /// <summary>
        /// Itera todos os tenants da aplicação. Abre o contexo do autofac e depois executa a ação do parâmetro.
        /// </summary>
        /// <param name="actionId">ID único da ação, serve para fazer a gestão se a tarefa está habilitada no ambiente</param>
        /// <param name="action">Ação a ser executada para cada tenant da aplicação</param>
        /// <param name="logAction">Ação de log recebe os parâmetros tipo de log e mensagem.</param>
        /// <param name="applyTenantSettings">Altera a cultura da thread para a cultura gravada na tabela de gestão dos tenants (adm.Client)</param>
        /// <param name="throwOnError">Define se irá estourar o erro ou apenas disparar o log</param>
        public async Task LoopTenants(string actionId, Func<Task> action, Action<string, string> logAction = null, bool applyTenantSettings = false, bool throwOnError = false)
        {
            var mapIds = new[] { "cliente", "default" };

            var mapeamentoBase = Singleton<ConfigurationTenant>.Instance.Mapeamentos.FirstOrDefault(m => mapIds.Contains(m.MapeamentoId));

            foreach (var tenant in mapeamentoBase.ConfigurationTenants.Where(t => !mapIds.Contains(t.Subdomain)))
            {
                using (Engine.BeginLifetimeScope(tenant.Subdomain, applyTenantSettings))
                {
                    try
                    {
                        //if (string.IsNullOrWhiteSpace(actionId) || (!string.IsNullOrWhiteSpace(actionId) && Engine.Resolve<BackgroundJobManager>().IsActive(actionId)))
                        if (string.IsNullOrWhiteSpace(actionId) || !string.IsNullOrWhiteSpace(actionId))
                        {
                            if (logAction != null)
                            {
                                logAction("info", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt")} - Tarefa Iniciada [{tenant.TenantId} - {tenant.TenantName}] | [{actionId}]");
                            }

                            await action();


                            if (logAction != null)
                            {
                                logAction("info", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt")} - Tarefa Finalizada [{tenant.TenantId} - {tenant.TenantName}]  | [{actionId}]");
                            }
                        }
                        else
                        {
                            logAction("error", $"{DateTime.Now} - WARN [{tenant.TenantId} - {tenant.TenantName}]: A tarefa está desligada no ambiente {Engine.AppSettings["environment"]}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (logAction != null)
                        {
                            logAction("error", $"{DateTime.Now} - ERRO [{tenant.TenantId} - {tenant.TenantName}]: {ex.Message}\r\n{ex.StackTrace}");
                        }

                        if (throwOnError) throw;
                    }
                }
            }
        }
    }
}
