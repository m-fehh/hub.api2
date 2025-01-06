namespace Hub.Application.Models.Helpers.Incorporation
{
    /// <summary>
    /// Classe de filtro com dados para incorporação
    /// </summary>
    public class IncorporationEstablishmentFilter
    {
        /// <summary>
        /// Id do estabelecimento
        /// </summary>
        public long EstablishmentId { get; set; }

        /// <summary>
        /// CNPJ do estabelecimento
        /// </summary>
        public string CNPJ { get; set; }
    }
}
