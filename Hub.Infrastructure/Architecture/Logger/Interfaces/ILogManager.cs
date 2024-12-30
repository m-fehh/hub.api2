using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Database.Entity.Interfaces;

namespace Hub.Infrastructure.Architecture.Logger.Interfaces
{
    public interface ILogManager
    {
        /// <summary>
        /// As classes que implementarem esse método se responsabilizam por gravar log das alterações encontradas no objeto passado
        /// </summary>
        /// <param name="obj">Objeto a ser logado</param>
        /// <param name="action">Ação da auditoria</param>
        /// <param name="verifyLogableEntity">Quando marcado, o método deve verificar se o objeto a ser logado possui algum marcador que indique que o mesmo deve passar pela auditoria ou nao</param>
        /// <param name="deeper"></param>
        /// <returns></returns>
        ILog Audit(IBaseEntity obj, ELogAction action, bool verifyLogableEntity = true, bool deeper = true);

        /// <summary>
        /// As classes que implementarem esse método se responsabilizam por gravar log do erro passado por parametro
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
    }
}
