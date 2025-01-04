using Hub.Application.Services;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
    public class HomeController : BaseController
    {
        void SetBaseURL()
        {
            ViewBag.CurrentURL = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
        }

        [HttpPost]
        public JsonResult GetVersion()
        {
            return Json(Engine.AppSettings["system_version"]);
        }

        [OutputCache(Duration = 600, VaryByCustom = CacheManager.UserLevel)]
        public ActionResult Index()
        {
            Engine.Resolve<UserService>().RegisterAccess();

            SetBaseURL();

            return View();
        }
    }
}
