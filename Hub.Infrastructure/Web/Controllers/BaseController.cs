using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Infrastructure.Web.Controllers
{
    /// <summary>
    /// Classe base para todos os controlles da aplicação.
    /// Essa classe garante a autenticação mínima necessária de um sistema portal
    /// </summary>
    [Authorize]
    public class BaseController : Controller { }
}
