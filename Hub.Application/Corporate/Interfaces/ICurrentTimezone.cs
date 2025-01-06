namespace Hub.Application.Corporate.Interfaces
{
    public interface ICurrentTimezone
    {
        //TimeZoneInfo Get();

        //string GetName();

        string GetServerName();

        /// <summary>
        /// Converte a data passada no timezone do cliente. Exemplo: se o servidor estiver em -03:00 e o cliente em -05:00, a data 06/11/2018 12:00 será convertida para 06/11/2018 10:00
        /// </summary>
        /// <param name="date">Data a ser convertida</param>
        /// <param name="tz"></param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date, TimeZoneInfo tz = null);

        /// <summary>
        /// Converte a data passada no timezone do servidor. Exemplo: se o servidor estiver em -03:00 e o cliente em -05:00, a data 06/11/2018 10:00 será convertida para 06/11/2018 12:00
        /// </summary>
        /// <param name="date">Data a ser convertida</param>
        /// <param name="tz"></param>
        /// <returns></returns>
        DateTime? ConvertServer(DateTime? date, TimeZoneInfo tz = null);
    }
}
