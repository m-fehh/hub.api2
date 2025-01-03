using Hub.Infrastructure.Architecture.Logger.Enums;
using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Nominator.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Configuration;
using System.Net;
using System.Reflection;

namespace Hub.Infrastructure.Architecture.Logger
{
    public class LogManager : ILogManager
    {
        protected virtual ILog InterceptLog(ILog log)
        {
            return log;
        }

        public ILog Audit(IBaseEntity obj, ELogAction action, bool verifyLogableEntity = true, bool deeper = true)
        {
            bool logsActived = true;

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            if (bool.TryParse(ConfigurationManager.AppSettings["LogsActived"], out logsActived))
            {
                if (!logsActived) return null;
            }

            if (obj == null) return null;

            if (verifyLogableEntity)
            {
                if (!typeof(ILogableEntity).IsAssignableFrom(obj.GetType())) return null;
            }

            using (Engine.BeginLifetimeScope())
            {
                ILog log;

                if (Engine.TryResolve<ILog>(out log))
                {
                    log.Action = action;
                    log.CreateDate = DateTime.Now;
                    log.LogType = ELogType.Audit;

                    log.IpAddress = GetIp();

                    if (deeper)
                    {
                        log.Fields = GetFieldList(obj, log);

                        if (log.Fields.Count == 0) return null;
                    }

                    log.ObjectId = obj.Id;

                    if (typeof(ILogableEntityCustomName).IsAssignableFrom(obj.GetType()))
                    {
                        if (!string.IsNullOrEmpty((obj as ILogableEntityCustomName).CustomLogName))
                        {
                            log.ObjectName = (obj as ILogableEntityCustomName).CustomLogName;
                        }
                        else
                        {
                            log.ObjectName = Engine.Get(obj.GetType().Name.Replace("Proxy", ""));
                        }
                    }
                    else
                    {
                        log.ObjectName = Engine.Get(obj.GetType().Name.Replace("Proxy", ""));
                    }
                    log.CreateUser = Engine.Resolve<ISecurityProvider>().GetCurrent();
                    log.Message = Engine.Resolve<INominatorManager>().GetName(obj);

                    //tenta dar um refreh caso a mensagem esteja em branco, isso pode ocorrer quando há a tentativa de gravar um log de um objeto que tenha somente o id carregado
                    if (string.IsNullOrEmpty(log.Message))
                    {
                        using (Engine.BeginLifetimeScope())
                        {

                            var repository = Engine.Resolve(typeof(IRepository<>), obj.GetType());

                            var loadMethod = repository.GetType().GetMethod("StatelessGetById", new Type[] { typeof(long) });

                            var loadedObj = loadMethod.Invoke(repository, new object[] { obj.Id });

                            log.Message = Engine.Resolve<INominatorManager>().GetName(loadedObj);
                        }
                    }

                    log = InterceptLog(log);

                    return log;
                }

                return null;
            }
        }

        public void Error(Exception ex)
        {
            bool logsActived = true;

            if (bool.TryParse(ConfigurationManager.AppSettings["LogsActived"], out logsActived))
            {
                if (!logsActived) return;
            }

            ILog log;

            if (Engine.TryResolve(out log))
            {
                var repo = Engine.Resolve<IRepository<ILog>>();

                log.Action = ELogAction.Insertion;
                log.CreateDate = DateTime.Now;
                log.CreateUser = Engine.Resolve<ISecurityProvider>().GetCurrent();
                log.LogType = ELogType.Error;
                log.ObjectId = ex.HResult;
                log.ObjectName = ex.Message;
                log.Message = ex.StackTrace;

                log = InterceptLog(log);

                using (var transaction = repo.BeginTransaction())
                {
                    repo.Insert(log);

                    if (transaction != null) repo.Commit();
                }
            }
        }

        public string GetIp()
        {
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            string ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (ip == null || ip == "::1")
            {
                ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
            }
            return ip;
        }

        private ISet<ILogField> GetFieldList<T>(T obj, ILog logFather) where T : class, IBaseEntity
        {
            // Recuperando o DbContext
            var dbContext = Engine.Resolve<DbContext>();

            T oldObj = null;

            if (logFather.Action == ELogAction.Update)
            {
                oldObj = dbContext.Set<T>().AsNoTracking().FirstOrDefault(o => o.Id == obj.Id);
            }

            var ret = new HashSet<ILogField>();

            var propertyList = obj.GetType().GetProperties().Where(p => p.Name != "Id" && !p.GetCustomAttributes(true).Any(a => a is IgnoreLog)).ToList();

            foreach (var prop in propertyList)
            {
                var newValue = prop.GetValue(obj);

                // Se a propriedade for primitiva ou se estiver inicializada (no caso de lazy loading), processa
                if (PrimitiveTypes.Test(prop.PropertyType))
                {
                    string newComparator = null, oldComparator = null;

                    bool isCollection = IsCollection(prop.PropertyType);

                    if (!isCollection)
                    {
                        if (typeof(IBaseEntity).IsAssignableFrom(prop.PropertyType))
                        {
                            newComparator = ((IBaseEntity)prop.GetValue(obj))?.Id.ToString();

                            if (oldObj != null)
                            {
                                var oldPropValue = prop.GetValue(oldObj);
                                if (oldPropValue != null)
                                {
                                    oldComparator = ((IBaseEntity)oldPropValue)?.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            var propValue = prop.GetValue(obj);
                            if (propValue != null)
                            {
                                newComparator = propValue.ToString();
                            }

                            if (oldObj != null)
                            {
                                var oldPropValue = prop.GetValue(oldObj);
                                if (oldPropValue != null)
                                {
                                    oldComparator = oldPropValue.ToString();
                                }
                            }
                        }
                    }

                    if (isCollection ||
                        newComparator != oldComparator ||
                        logFather.Action == ELogAction.Insertion)
                    {
                        var generateField = false;

                        // Obtendo as representações string para comparação
                        string newValueStr = Engine.Resolve<INominatorManager>().GetPropertyDescritor(prop, obj);
                        string oldValueStr = Engine.Resolve<INominatorManager>().GetPropertyDescritor(prop, oldObj, false);

                        var field = Engine.Resolve<ILogField>();

                        field.OldValue = oldValueStr;
                        field.NewValue = newValueStr;
                        field.PropertyName = Engine.Get(prop.Name);
                        field.Log = logFather;
                        field.Childs = new HashSet<ILog>();

                        if (isCollection)
                        {
                            if (prop.GetCustomAttributes(true).Any(a => a is DeeperLog))
                            {
                                var oldList = oldObj != null ? (IEnumerable<IBaseEntity>)prop.GetValue(oldObj) : new HashSet<IBaseEntity>();
                                var newList = (IEnumerable<IBaseEntity>)newValue;

                                foreach (var item in newList.Except(oldList))
                                {
                                    ILog log = Audit(item, ELogAction.Insertion, false, false);
                                    if (log != null) field.Childs.Add(log);
                                }

                                foreach (var item in oldList.Except(newList))
                                {
                                    ILog log = Audit(item, ELogAction.Deletion, false, false);
                                    if (log != null) field.Childs.Add(log);
                                }

                                if (field.Childs.Count > 0) generateField = true;
                            }
                        }
                        else if (typeof(IBaseEntity).IsAssignableFrom(prop.PropertyType))
                        {
                            if (prop.GetCustomAttributes(true).Any(a => a is DeeperLog))
                            {
                                if (field.Childs == null) field.Childs = new HashSet<ILog>();

                                var log = Audit(prop.GetValue(obj) as IBaseEntity, ELogAction.Update, false, true);
                                field.Childs.Add(log);
                            }

                            generateField = true;
                        }
                        else
                        {
                            generateField = true;
                        }

                        if (generateField)
                        {
                            ret.Add(field);
                        }
                    }
                }
            }

            return ret;
        }

        // Função para verificar se uma propriedade é uma coleção
        private bool IsCollection(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}

