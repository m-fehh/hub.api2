﻿using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Generator;

namespace Hub.Application.Models.ViewModels
{
    public class LogAuditVM : LogBaseVM
    {
        public long? CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public long? ObjectId { get; set; }
        public string ObjectName { get; set; }
        public ELogAction Action { get; set; }
        public ELogType LogType { get; set; }
        public string Message { get; set; }
        public string IpAddress { get; set; }
        public long? OwnerOrgStructId { get; set; }
        public string OwnerOrgStructTree { get; set; }
        public string OwnerOrgStructDescription { get; set; }
        public int? LogVersion { get; set; }
        public ISet<LogAuditFieldVM> Fields { get; set; }
    }

    public class LogAuditFieldVM
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string PropertyName { get; set; }
        public ISet<LogAuditVM> Children { get; set; }
    }

    public abstract class LogBaseVM
    {
        public string Id { get; set; }
        public DateTime? CreationUTC { get; set; }

        /// <summary>
        /// Permite definir o provedor de logs.
        /// Os valores aceitos são: elastic, sqlserver, azureblob
        /// </summary>
        public string LogProvider { get; set; }

        /// <summary>
        /// Gera um Id numérico para exibição em grid relacional
        /// </summary>
        /// <returns>Id convertido, HashCode ou aleatório</returns>
        public long GetLongId()
        {
            var id = 0L;

            if (long.TryParse(Id, out id))
                return id;
            else
            {
                if (string.IsNullOrEmpty(Id))
                    return Engine.Resolve<IRandomGeneration>().Generate();
                else
                    return Id.GetHashCode();
            }
        }
    }
}
