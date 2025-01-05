namespace Hub.Application.Models.ViewModels.Auth
{
    public class AuthResultVM
    {
        /// <summary>
        /// Esse campo representa o QRCodeInfo e não o Id da tabea PortalUser
        /// O QRCodeInfo é o Id externo que pode ser compartilhado com outros sistemas
        /// </summary>
        public string Id { get; set; }
        public string CPF { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; }
        public bool Inactive { get; set; }
    }
}
