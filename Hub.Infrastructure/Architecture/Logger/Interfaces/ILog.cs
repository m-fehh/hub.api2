using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Database.Entity.Interfaces;

namespace Hub.Infrastructure.Architecture.Logger.Interfaces
{
    /// <summary>
    /// A classe que extender essa interface deve ser destinada a gravação de log do sistema em banco de dados
    /// </summary>
    public interface ILog : IBaseEntity
    {
        DateTime CreateDate { get; set; }

        IUserAccount CreateUser { get; set; }

        long ObjectId { get; set; }

        string ObjectName { get; set; }

        ELogAction Action { get; set; }

        ELogType LogType { get; set; }

        string Message { get; set; }

        ISet<ILogField> Fields { get; set; }

        /// <summary>
        /// sinaliza que esse log está atrelado a alguma mudança em um campo de outro registro de log
        /// </summary>
        ILogField Father { get; set; }

        string IpAddress { get; set; }
    }
}
