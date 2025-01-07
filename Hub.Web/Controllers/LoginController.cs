using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shyjus.BrowserDetection;
using Hub.Infrastructure.Extensions;
using Hub.Application.Services.Users;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Exceptions;
using static QRCoder.PayloadGenerator.WiFi;
using Hub.Application.Models.ViewModels;

namespace Hub.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IBrowserDetector browserDetector;
        private readonly IWebHostEnvironment webHostEnvironment;

        public LoginController(IBrowserDetector browserDetector, IWebHostEnvironment webHostEnvironment)
        {
            GetAuthProvider();

            this.browserDetector = browserDetector;
            this.webHostEnvironment = webHostEnvironment;
        }

        #region PRIVATE METHODS

        private EPortalAuthProvider GetAuthProvider()
        {
            try
            {
                if (HttpContext?.Request?.Query?.ContainsKey("username") ?? false)
                {
                    ViewBag.ElosAuthProvider = "Native";
                }
                else
                {
                    var elosAuthProvider = Engine.AppSettings["elos-auth-provider"];

                    if (elosAuthProvider != null)
                    {
                        ViewBag.ElosAuthProvider = elosAuthProvider;
                    }
                    else
                    {
                        ViewBag.ElosAuthProvider = "Native";
                    }
                }

                return (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), ViewBag.ElosAuthProvider);
            }
            catch (Exception) { }

            return EPortalAuthProvider.Native;
        }

        private FingerPrintVM GetFingerPrint(string model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model)) return Newtonsoft.Json.JsonConvert.DeserializeObject<FingerPrintVM>(model);


                return new FingerPrintVM()
                {
                    BrowserName = browserDetector.Browser.Name,
                    BrowserInfo = browserDetector.Browser.Version,
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString()
                };
            }
            catch
            {
                return null;
            }
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout(string message)
        {
            //RevokeAuthenticationExtensions.RevokeEtrustAuthentication();
            Infrastructure.Extensions.CookieExtensions.CleanCookies();

            TempData["LogoutMessage"] = message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                GetAuthProvider();
                SetBaseURL();

                Infrastructure.Extensions.CookieExtensions.CleanCookies();
                ViewBag.LogoutMessage = TempData["LogoutMessage"]?.ToString();

                TempData.Remove("LogoutMessage");
            }
            catch (Exception) { }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecoverPass()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePass(string login)
        {
            var authProvider = GetAuthProvider();

            ViewBag.AuthProvider = authProvider;

            return PartialView(new ChangePassVM() { Login = login });
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult SaveChangePass(ChangePassVM loginVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (loginVM.ConfirmPassword != loginVM.NewPassword)
        //            {
        //                ModelState.AddModelError("", Engine.Get("PasswordDoesNotMatchConfirmation"));

        //                return Json(new
        //                {
        //                    Callback = "$('.modal.active .modal-content').html('" + this.RenderPartialToString("ChangePass", loginVM).MinifierHtml() + @"');"
        //                });
        //            }

        //            var securityProvider = (UserService)Engine.Resolve<ISecurityProvider>();

        //            securityProvider.ChangePass(loginVM.Login, loginVM.OldPassword, loginVM.NewPassword);

        //            var authenticationVM = new AuthenticationVM(loginVM.Login, loginVM.NewPassword, false);
        //            securityProvider.Authenticate(authenticationVM);

        //            return Json(new
        //            {
        //                Message = Engine.Get("PasswordChangeSuccessful"),
        //                Callback = "window.location = $App.resolveUrl('~/');"
        //            });
        //        }
        //        catch (BusinessException ex)
        //        {
        //            ModelState.AddModelError("", ex.Message);

        //            return Json(new
        //            {
        //                Callback = "$('.modal.active .modal-content').html('" + this.RenderPartialToString("ChangePass", loginVM).MinifierHtml() + @"');"
        //            });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            Callback = "$('.modal.active .modal-content').html('" + this.RenderPartialToString("ChangePass", loginVM).MinifierHtml() + @"');"
        //        });
        //    }
        //}

        void SetBaseURL()
        {
            ViewBag.CurrentURL = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
        }
    }
}
