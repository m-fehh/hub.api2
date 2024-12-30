using Hub.Infrastructure;
using Hub.Infrastructure.Security.Interfaces;

namespace Hub.Web.Middlewares
{
    public class RequestAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private string GetTokenFromRequest(HttpRequest request)
        {
            var userAuthenticationToken = string.Empty;

            #region Verifica se Token veio via Header
            if (request.Headers.ContainsKey("jwt"))
            {
                userAuthenticationToken = request.Headers["jwt"];
            }
            #endregion

            #region Verifica se Token veio via QueryString

            if (request.Query.ContainsKey("jwt"))
            {
                var queryStringUserAuthentication = request.Query["jwt"];

                if (string.IsNullOrEmpty(queryStringUserAuthentication) == false)
                {
                    userAuthenticationToken = queryStringUserAuthentication;
                }
            }

            #endregion

            return userAuthenticationToken;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userAuthentication = GetTokenFromRequest(context.Request);

            if (string.IsNullOrEmpty(userAuthentication) == false)
            {
                var securityProvider = Engine.Resolve<ISecurityProvider>();
                securityProvider.Authenticate(userAuthentication);
            }
            else
            {
                var authToken = context.Request.Cookies["Authentication"];

                if (!string.IsNullOrEmpty(authToken))
                {
                    try
                    {
                        //Engine.Resolve<UserService>().SetCurrentUser(authToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }



            await _next(context);
        }
    }

    public static class RequestAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestAuthMiddleware>();
        }
    }
}
