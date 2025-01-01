namespace Hub.Infrastructure.Database.Interfaces
{
    /// <summary>
    /// Interface que implementa um provedor de nome para o atual tenant do sistema
    /// </summary>
    public interface IEntityNameProvider
    {
        /// <summary>
        /// método responsável obter o tenant atual do sistema
        /// </summary>
        string TenantName();
    }
}
