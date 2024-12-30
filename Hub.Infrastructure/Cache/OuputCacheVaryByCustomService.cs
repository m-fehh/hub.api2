using Hub.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using WebEssentials.AspNetCore.OutputCaching;

namespace Hub.Infrastructure.Cache
{
    public class OuputCacheVaryByCustomService : IOutputCacheVaryByCustomService
    {
        public string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (Engine.AppSettings["useOutputCache"].TryParseBoolean(true) == false)
                return Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(custom))
            {
                return Engine.Resolve<CacheManager>().CachePrefix(custom);
            }

            return null;
        }
    }
}
