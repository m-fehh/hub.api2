using Hub.Application.Services.Users;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        void SetBaseURL()
        {
            ViewBag.CurrentURL = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
        }

        [OutputCache(Duration = 600, VaryByCustom = CacheManager.UserLevel)]
        public ActionResult Index()
        {
            Engine.Resolve<UserService>().RegisterAccess();

            SetBaseURL();

            return View();
        }

        [HttpPost]
        public JsonResult GetVersion()
        {
            return Json(Engine.AppSettings["system_version"]);
        }
    }
}
