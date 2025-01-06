namespace Hub.Infrastructure.Architecture.Localization.Interfaces
{
    public interface IResourceWrapper
    {
        /// <summary>
        /// Classes que implementarem devem armazenar os resources locais afim de fornecer as traduções quando o serviço LocalizationProvider solicitar
        /// </summary>
        string GetString(string key);

        string GetString(string key, string language);

        string GetByValue(string value);
    }
}
