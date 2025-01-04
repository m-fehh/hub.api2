using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Generator;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Application.Models.ViewModels;
using LogEntity = Hub.Domain.Entities.Logs.Log;
using Hub.Infrastructure.Database.Models;
using Hub.Application.CorporateStructure.Interfaces;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture.Security.Interfaces;

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
            // Inicialização de variáveis
            bool hasStructureId = true;
            long structureId = ownerOrgStructId ?? 0;
            string resolvedStructureId = string.Empty;
            OrganizationalStructure currentOrg = null;

            // Resolução da estrutura organizacional, se não fornecida
            if (structureId == 0)
            {
                resolvedStructureId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();

                if (string.IsNullOrEmpty(resolvedStructureId))
                {
                    hasStructureId = false;
                }
                else
                {
                    structureId = long.Parse(resolvedStructureId);
                }
            }

            // Preparação do log
            var log = new LogEntity
            {
                CreateDate = DateTime.Now,
                ObjectName = objectName,
                ObjectId = objectId,
                Action = action,
                LogType = ELogType.Audit,
                Message = message ?? string.Empty,
                LogVersion = logVersion ?? DefaultLogVersion
            };

            if (hasStructureId)
            {
                currentOrg = Engine.Resolve<IRepository<OrganizationalStructure>>().GetById(structureId);

                log.OwnerOrgStruct = currentOrg;
                log.CreateUser = Engine.Resolve<ISecurityProvider>().GetCurrent();

                if (log.CreateUser is PortalUser currentUser)
                {
                    log.IpAddress = currentUser.IpAddress;
                }
            }

            // Persistência do log
            var logRepository = Engine.Resolve<IRepository<LogEntity>>();
            using (var transaction = logRepository.BeginTransaction())
            {
                logRepository.Insert(log);

                if (transaction != null) logRepository.Commit();
            }
        }

        #region PRIVATE METHODS 

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

        /// <summary>
        /// Remove último caracter da string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string StringAjust(string data)
        {
            return string.Join(",", data).Substring(0, string.Join(",", data).Length - 1);
        }

        /// <summary>
        /// Traduz o tipo boolean de 1(true) ou 0(false) para SIM ou NÃO
        /// </summary>
        /// <param name="data"> tipo de dado em string true ou false </param>
        /// <returns></returns>
        private string TranslateFilterType(string data)
        {
            var returnData = string.Empty;

            if (data == "true")
            {
                returnData = Engine.Get("Yes");
            }
            else if (data == "false")
            {
                returnData = Engine.Get("No");
            }
            else
            {
                returnData = data;
            }

            return returnData;
        } 

        #endregion
    }
}
