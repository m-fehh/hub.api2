using Hub.Infrastructure.Autofac.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Microsoft.AspNetCore.Http;

namespace Hub.Infrastructure.Autofac
{
    public class DefaultNameProvider : INhNameProvider { 
    //{
    //    private static readonly HttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

    //    public string TenantName()
    //    {
    //        var fixedDomain = Engine.Resolve<TenantLifeTimeScope>().CurrentTenantName;

    //        if (!string.IsNullOrEmpty(fixedDomain)) return fixedDomain;

    //        if (_httpContextAccessor.HttpContext == null ||
    //            _httpContextAccessor.HttpContext.Request == null) return "default";

    //        var baseUrl = string.Format("{0}://{1}",
    //            _httpContextAccessor.HttpContext.Request.Scheme,
    //            _httpContextAccessor.HttpContext.Request.Host.Value);

    //        return TenantByUrl(baseUrl);
    //    }

    //    public string TenantByUrl(string url)
    //    {
    //        return "default";
    //    }
    }
}
