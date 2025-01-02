using Hub.Application;
using Hub.Infrastructure.Architecture;

namespace Hub.Web.Middlewares
{
    public class RequestTenantMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var baseUrl = string.Format("{0}://{1}", context.Request.Scheme, context.Request.Host.Value);

            Engine.Resolve<HubTenantNameProvider>().SetCurrentTenant(baseUrl);

            await _next(context);
        }
    }

    public static class RequestTenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTenant(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTenantMiddleware>();
        }
    }
}
