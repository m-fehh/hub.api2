using System.Globalization;

namespace Hub.Infrastructure.Architecture.Localization.Interfaces
{
    public interface ILocalizationProvider
    {
        string Get(string key);
        string Get(string key, CultureInfo culture);
        string GetByValue(string value);
    }
}
