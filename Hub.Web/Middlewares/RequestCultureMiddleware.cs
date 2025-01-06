using Hub.Infrastructure.Architecture;
using System.Globalization;

namespace Hub.Web.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string culture = null;

            if (context.Request.Cookies["Culture"] != null && !string.IsNullOrEmpty(context.Request.Cookies["Culture"]))
            {
                culture = context.Request.Cookies["Culture"];
            }
            else
            {
                culture = Engine.AppSettings["DefaultCulture"];
            }

            if (string.IsNullOrEmpty(culture))
            {
                culture = "pt-BR";
            }

            CultureInfo ci = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;

            await _next(context);
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }
    }
}
