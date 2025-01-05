using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.HealthChecker.Enums;
using Hub.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Hub.Infrastructure.Architecture.HealthChecker
{
    /// <summary>
    /// Checker para configuração com opções
    /// </summary>
    public class CheckerConfigOptions : CheckerItem<IEnumerable<string>>
    {
        /// <summary>
        /// Nome da configuração
        /// </summary>
        public string ConfigName { get; }

        /// <summary>
        /// Tipo de validação que será aplicada
        /// </summary>
        public EConfigValidationTypes ValidationType { get; }

        public CheckerConfigOptions(EConfigValidationTypes validationType = EConfigValidationTypes.NullOrEmpty, params string[] configNames)
            : base(() => configNames.Select(c => Engine.AppSettings[c]).ToList(), GetHealthyExpression(validationType), Engine.Get("HealthConfigException", string.Join(", ", configNames)))
        {
            ConfigName = string.Join(", ", configNames);
            ValidationType = validationType;
        }

        private static Expression<Func<IEnumerable<string>, bool>> GetHealthyExpression(EConfigValidationTypes validationType)
        {
            switch (validationType)
            {
                case EConfigValidationTypes.Zero:
                    return e => e.Any(i => i.IsInteger() && i.ToLong() > 0);
                case EConfigValidationTypes.NullOrEmpty:
                default:
                    return e => e.Any(i => !string.IsNullOrEmpty(i));
            }
        }
    }
}
