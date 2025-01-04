using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.HealthChecker.Builders;
using Hub.Infrastructure.HealthChecker.Enums;
using Hub.Infrastructure.HealthChecker.Interfaces;

namespace Hub.Infrastructure.HealthChecker
{
    /// <summary>
    /// Centralizador para execução básica de testes já implementados para HealthChecker
    /// </summary>
    public class BasicHealthChecking : IHealthChecker
    {
        private static readonly IHealthChecker instance = new BasicHealthChecking();

        BasicHealthChecking() { }

        public static IHealthChecker Instance => instance;

        /// <summary>
        /// Simular falha
        /// </summary>
        public static bool SimulateFail { get; set; }

        public CheckerContainer CheckerContainer
        {
            get
            {
                if (SimulateFail)
                {
                    SimulateFail = false;
                    return new CheckerContainerBuilder(this)
                        .AddItem(new CheckerConfigItem("thisconfignotexists", EConfigValidationTypes.NullOrEmpty))
                        .Build();
                }

                return new CheckerContainerBuilder(this)
                    // Configuration
                    .AddItem(new CheckerConfigItem("environment", EConfigValidationTypes.NullOrEmpty))
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("default"), e => !string.IsNullOrEmpty(e), msgError: Engine.Get("DefaultConnectionMsg")))
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("adm"), e => !string.IsNullOrEmpty(e), msgError: Engine.Get("AdmConnectionMsg")))
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("redis"), e => !string.IsNullOrEmpty(e), msgError: Engine.Get("RedisConnectionMsg")))
                    .AddItem(new CheckerItem<string>(() => Engine.Resolve<ConnectionStringInitializer>().Get().ConnectionStringBaseSchema, e => !string.IsNullOrEmpty(e), msgError: Engine.Get("BaseSchemaConnectionMsg")))
                    // Database
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("default"), e => Checkers.DbConnectionChecker.CheckSqlServer(e), msgError: Engine.Get("DbConnectionCheckerMsg")))
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("adm"), e => Checkers.DbConnectionChecker.CheckSqlServer(e), msgError: Engine.Get("DbConnectionCheckerMsg")))
                    // Redis
                    .AddItem(new CheckerItem<string>(() => Engine.ConnectionString("redis"), e => Checkers.RedisChecker.CheckConnection(e), msgError: Engine.Get("RedisCheckerMsg")))
                    .Build();
            }
        }
    }
}
