using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Logger.Enums;
using Hub.Infrastructure.Logger.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Hub.Infrastructure.Logger
{
    public class LogManager : ILogManager
    {
        protected virtual ILog InterceptLog(ILog log)
        {
            return log;
        }

        public ILog Audit(IBaseEntity obj, ELogAction action, bool verifyLogableEntity = true, bool deeper = true)
        {
            
            return null;
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

        public void Error(Exception ex)
        {
           
        }
    }
}

