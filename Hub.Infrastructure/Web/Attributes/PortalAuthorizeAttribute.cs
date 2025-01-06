using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Infrastructure.Web.Attributes
{
    public class PortalAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Substitui a propriedade roles com informações do controller se especificado
        /// </summary>
        public string DynamicRoles { get; set; }
        public string Roles { get; set; }
        protected void HandleUnauthorizedRequest(AuthorizationFilterContext context)
        {
            var isAjax = (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest");
            if (isAjax)
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new JsonResult(new
                {
                    Message = Engine.Get("PortalUserAccessDenied"),
                });
            }
            else
            {
                CookieExtensions.CleanCookies();
                context.Result = new RedirectResult("~/Login");
            }
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!string.IsNullOrEmpty(DynamicRoles))
            {
                var controllerDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                Roles = DynamicRoles.Replace("{{ControllerName}}", controllerDescriptor.ControllerName);
            }
            var securityProvider = Engine.Resolve<ISecurityProvider>();

    //---------- TODO: AUTORIZE

            //if (!securityProvider.Authorize(Roles))
            //{
            //    HandleUnauthorizedRequest(context);
            //}
        }
    }
}
