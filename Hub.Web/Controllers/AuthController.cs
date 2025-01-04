using Hub.Application.Models.ViewModels.Auth;
using Hub.Application.Services;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Models;
using Hub.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hub.Web.Controllers
{
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] AuthVM request)
        {
            //try
            //{
            //    request.Provider = EAuthProvider.Api;

            //    var userService = Engine.Resolve<UserService>();

            //    userService.Authenticate(request);

            //    if (request.Token == null)
            //    {
            //        return StatusCode((int)HttpStatusCode.Unauthorized);
            //    }

            //    var userData = userService.AuthenticateToken(request.Token);

            //    return StatusCode((int)HttpStatusCode.OK, new { error = false, request.Token, Data = userData });
            //}
            //catch (BusinessException bex)
            //{
            //    return StatusCode((int)HttpStatusCode.Unauthorized, new { error = true, message = bex.Message });
            //}
            //catch (Exception ex)
            //{
            //    var log = Engine.Resolve<LogService>().LogError(ex, "API-Auth-Login");

            //    var msg = string.Format(Engine.Get("AnErrorWasOcurredReportNumber"), log.ObjectId);

            //    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = true, message = msg });
            //}

            return Ok();
        }

        /// <summary>
        /// Faz autenticação no portal através de token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthVM request)
        {
            //try
            //{
            //    var model = Engine.Resolve<UserService>().AuthenticateToken(request.Token);

            //    if (model == null)
            //    {
            //        return StatusCode((int)HttpStatusCode.Unauthorized);
            //    }


            //    return StatusCode((int)HttpStatusCode.OK, model);
            //}
            //catch (Exception ex)
            //{
            //    var log = Engine.Resolve<LogService>().LogError(ex, "API-Auth-Authenticate");

            //    var msg = string.Format(Engine.Get("AnErrorWasOcurredReportNumber"), log.ObjectId);

            //    return StatusCode((int)HttpStatusCode.Unauthorized, new { error = true, message = msg });
            //}

            return Ok();

        }
    }
}
