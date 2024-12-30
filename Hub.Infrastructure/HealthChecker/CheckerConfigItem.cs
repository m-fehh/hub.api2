using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.HealthChecker.Enums;
using System.Linq.Expressions;

namespace Hub.Infrastructure.HealthChecker
{
    /// <summary>
    /// Checker para configuração
    /// </summary>
    public class CheckerConfigItem : CheckerItem<string>
    {
        /// <summary>
        /// Nome da configuração
        /// </summary>
        public string ConfigName { get; }

        /// <summary>
        /// Tipo de validação que será aplicada
        /// </summary>
        public EConfigValidationTypes ValidationType { get; }

        public CheckerConfigItem(string configName, EConfigValidationTypes validationType = EConfigValidationTypes.NullOrEmpty)
            : base(() => Engine.AppSettings[configName], GetHealthyExpression(validationType), Engine.Get("HealthConfigException", configName))
        {
            ConfigName = configName;
            ValidationType = validationType;
        }

        private static Expression<Func<string, bool>> GetHealthyExpression(EConfigValidationTypes validationType)
        {
            switch (validationType)
            {
                case EConfigValidationTypes.Zero:
                    return e => e.IsInteger() && e.ToLong() > 0;
                case EConfigValidationTypes.NullOrEmpty:
                default:
                    return e => !string.IsNullOrEmpty(e);
            }
        }
    }
}
