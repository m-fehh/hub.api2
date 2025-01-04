namespace Hub.Application.Models.ViewModels.Auth
{
    /// <summary>
    /// Destinado a armazenar informações diversas no momento do login do ELOS
    /// <see href="https://dev.azure.com/evuptec/EVUP/_workitems/edit/17365">Link do work item</see>
    /// </summary>
    public class FingerPrintVM
    {
        public string OS { get; set; }
        public string BrowserName { get; set; }
        public string BrowserInfo { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"OS: {OS} / BrowserName: {BrowserName} / BrowserInfo: {BrowserInfo} / IP : {IpAddress}";
        }
    }
}
