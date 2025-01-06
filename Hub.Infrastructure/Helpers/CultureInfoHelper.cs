using System.Globalization;
using System.Text.RegularExpressions;
using Hub.Infrastructure.Architecture;

namespace Hub.Infrastructure.Helpers
{
    public static class CultureInfoHelper
    {
        //pt-BR
        private const string _LANG_PATTERN = @"^[a-z]{2}-[A-Z]{2}$";

        public static string SetCultureInfo(string lang)
        {
            var currentLang = lang;

            if (string.IsNullOrWhiteSpace(currentLang) || !Regex.IsMatch(currentLang, _LANG_PATTERN))
            {
                currentLang = CultureInfo.CurrentCulture.Name;
            }

            return currentLang;
        }

        /// <summary>
        ///     Define a linguagem do sistema de acordo com a configuração de linguagem padrão. 
        /// </summary>
        /// <param name="createCookie" cref="bool">Variável que define se um cookie de linguagem deve ser criado</param>
        /// <returns cref="void">Sem retorno</returns>
        public static void SetDefaultCultureInfo()
        {
            var culture = Engine.AppSettings["defaultCulture"];

            if (culture == null)
            {
                culture = "pt-BR";
            }
            else
            {
                culture = culture.Contains("es") ? "es-ES" : "pt-BR";
            }

            CultureInfo ci = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }
    }
}
