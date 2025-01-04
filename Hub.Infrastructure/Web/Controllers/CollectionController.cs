using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Web.Attributes;
using Hub.Infrastructure.Web.Interfaces;
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

    public abstract class CollectionController<TEntity, TModel> : BaseController where TEntity : IBaseEntity where TModel : IModelEntity
    {
        protected readonly IOrchestratorService<TEntity> _crudService;
        protected readonly IDataMapper<TEntity, TModel> _queryModelMapper;
        protected IQueryable<TEntity> _customQuery;

        #region Defaults

        protected string DefaultViewName = "Index";

        #endregion

        public CollectionController(IOrchestratorService<TEntity> crudService, IDataMapper<TEntity, TModel> modelMapper)
        {
            _crudService = crudService;
            _queryModelMapper = modelMapper;
        }


        [PortalAuthorize(DynamicRoles = "CB_{{ControllerName}}_Vis")]
        public virtual ActionResult Index()
        {
            return PartialView(DefaultViewName);
        }

        [HttpPost]
        [PortalAuthorize(DynamicRoles = "CB_{{ControllerName}}_Exp")]
        public virtual ActionResult Export(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    }
}
