using Microsoft.AspNetCore.Http;

namespace Hub.Infrastructure.Web
{
    public static class HttpContextHelper
    {
        public static HttpContext Current => HttpContextAccessor.HttpContext;

        private static readonly HttpContextAccessor HttpContextAccessor = new HttpContextAccessor();
    }
}
