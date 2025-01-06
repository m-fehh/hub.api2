using Hub.Infrastructure.Web;

namespace Hub.Infrastructure.Extensions
{
    public static class CookieExtensions
    {
        public static void CleanCookies()
        {
            try
            {
                HttpContextHelper.Current.Response.Cookies.Delete("Authentication");
                HttpContextHelper.Current.Response.Cookies.Delete("ASP.NET_SessionId");
            }
            catch (InvalidOperationException) { }
        }
    }
}
