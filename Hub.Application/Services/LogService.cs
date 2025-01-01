using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Generator;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Application.Models.ViewModels;
using LogEntity = Hub.Domain.Entities.Logs.Log;

namespace Hub.Application.Services
{
    public class LogService
    {
        /// <summary>
        /// Versão padrão dos logs gravados pelo sistema.
        /// </summary>
        private const int DefaultLogVersion = 2;

        /// <summary>
        /// Registra um erro no log.
        /// </summary>
        /// <param name="exception">Exceção a ser registrada.</param>
        /// <param name="source">Origem do erro.</param>
        /// <param name="additionalData">Dados adicionais (opcional).</param>
        /// <param name="objectId">ID do objeto relacionado (opcional).</param>
        /// <returns>Instância do log criado.</returns>
        public LogEntity LogError(Exception exception, string source, string additionalData = null, long? objectId = null)
        {
            var log = new LogEntity
            {
                Action = ELogAction.Insertion,
                CreateDate = DateTime.Now,
                LogType = ELogType.Error,
                ObjectId = objectId ?? Engine.Resolve<IRandomGeneration>().Generate(),
                ObjectName = source,
                Message = FormatErrorMessage(exception, additionalData)
            };

            var repository = Engine.Resolve<IRepository<LogEntity>>();

            using (var transaction = repository.BeginTransaction())
            {
                repository.StatelessInsert(log);
                if (transaction != null)
                {
                    repository.Commit();
                }
            }

            return log;
        }

        /// <summary>
        /// Registra uma auditoria no log.
        /// </summary>
        /// <param name="model">Modelo contendo os dados da auditoria.</param>
        public void Audit(LogAuditVM model)
        {
            Audit(objectName: model.ObjectName, objectId: model.ObjectId ?? 0, action: model.Action, ownerOrgStructId: model.OwnerOrgStructId, message: model.Message, logVersion: model.LogVersion);
        }

        /// <summary>
        /// Registra uma auditoria no log.
        /// </summary>
        /// <param name="objectName">Nome do objeto relacionado.</param>
        /// <param name="objectId">ID do objeto relacionado.</param>
        /// <param name="action">Ação executada.</param>
        /// <param name="ownerOrgStructId">ID da estrutura organizacional (opcional).</param>
        /// <param name="code">Código da mensagem (opcional).</param>
        /// <param name="status">Status relacionado ao código (opcional).</param>
        /// <param name="message">Mensagem da auditoria (opcional).</param>
        /// <param name="logVersion">Versão do log (opcional).</param>
        public void Audit(string objectName, long objectId, ELogAction action, long? ownerOrgStructId = null, string code = "", string status = "", string message = "", int? logVersion = null)
        {
            var formattedMessage = (string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(code)) ? string.Format(code, Engine.Get(status)) : message ?? string.Empty;

            var log = new LogEntity
            {
                CreateDate = DateTime.Now,
                ObjectName = objectName,
                ObjectId = objectId,
                Action = action,
                LogType = ELogType.Audit,
                Message = formattedMessage,
                LogVersion = logVersion ?? DefaultLogVersion
            };

            var repository = Engine.Resolve<IRepository<LogEntity>>();

            using (var transaction = repository.BeginTransaction())
            {
                repository.Insert(log);
                if (transaction != null)
                {
                    repository.Commit();
                }
            }
        }

        /// <summary>
        /// Formata a mensagem de erro com dados adicionais.
        /// </summary>
        /// <param name="exception">Exceção a ser registrada.</param>
        /// <param name="additionalData">Dados adicionais (opcional).</param>
        /// <returns>Mensagem formatada.</returns>
        private static string FormatErrorMessage(Exception exception, string additionalData)
        {
            var baseMessage = exception.CreateExceptionString();
            return string.IsNullOrEmpty(additionalData) ? baseMessage : $"{baseMessage}\r\nDados Adicionais: {additionalData}";
        }
    }
}
