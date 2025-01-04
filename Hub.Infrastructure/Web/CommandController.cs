using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Web.Attributes;
using Hub.Infrastructure.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Infrastructure.Web
{
    public abstract class CommandController<TEntity, TModel> : CommandController<TEntity, TModel, TModel> where TEntity : IBaseEntity where TModel : IModelEntity
    {
        public CommandController(IOrchestratorService<TEntity> crudService, IDataMapper<TEntity, TModel> modelMapper) : base(crudService, modelMapper, modelMapper) { }
    }

    public abstract class CommandController<TEntity, TModel, TQueryModel> : CollectionController<TEntity, TQueryModel> where TEntity : IBaseEntity where TModel : IModelEntity where TQueryModel : IModelEntity
    {
        #region Defaults

        protected string DefaultViewName = "";
        protected string DefaultFormView = "Form";
        protected TModel InitialModelForNewPage = default(TModel);
        protected Func<TModel, TModel> ModelFactory = null;
        protected IDataMapper<TEntity, TModel> EntityModelMapper;
        protected IOrchestratorService<TEntity, TModel> ModelOrchestrationService = null;

        #endregion

        public CommandController(IOrchestratorService<TEntity> crudService, IDataMapper<TEntity, TModel> modelMapper, IDataMapper<TEntity, TQueryModel> queryModelMapper) : base(crudService, queryModelMapper)
        {
            EntityModelMapper = modelMapper;
            Engine.TryResolve(out ModelOrchestrationService);
        }

        [PortalAuthorize(DynamicRoles = "CB_{{ControllerName}}_Ins")]
        public virtual ActionResult FormInsert()
        {
            TModel model;

            TEntity entity;

            if (typeof(TEntity).IsInterface)
            {
                entity = (TEntity)Engine.Resolve(typeof(TEntity));
            }
            else if (typeof(TEntity).IsAbstract)
            {
                return PartialView(DefaultFormView, Activator.CreateInstance<TModel>());
            }
            else
            {
                entity = Activator.CreateInstance<TEntity>();
            }

            model = EntityModelMapper.BuildModel(entity);

            ViewBag.FromVisualization = false;

            if (ModelFactory != null) model = ModelFactory(model);

            return PartialView(DefaultFormView, model);
        }

        [PortalAuthorize(DynamicRoles = "CB_{{ControllerName}}_Upd")]
        public virtual ActionResult FormEdit(long id)
        {
            TEntity entity = _crudService.GetById(id);

            TModel model = EntityModelMapper.BuildModel(entity);

            ViewBag.FromVisualization = false;

            if (ModelFactory != null) model = ModelFactory(model);

            return PartialView(DefaultFormView, model);
        }

        [PortalAuthorize(DynamicRoles = "CB_{{ControllerName}}_Vis")]
        public virtual ActionResult FormVis(long id)
        {
            TEntity entity = _crudService.GetById(id);

            ViewBag.FromVisualization = true;

            TModel model = EntityModelMapper.BuildModel(entity);

            if (ModelFactory != null) model = ModelFactory(model);

            return PartialView(DefaultFormView, model);
        }
    }
}
