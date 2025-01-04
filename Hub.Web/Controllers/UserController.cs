using Hub.Application.Services;
using Hub.Domain.Entities.Users;
using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Web;
using Hub.Infrastructure.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
    public class UserController : CommandController<PortalUser, PortalUserVM>
    {
        private readonly UserService _userService;

        public UserController(IOrchestratorService<PortalUser> crudService, IDataMapper<PortalUser, PortalUserVM> modelMapper, UserService userService) : base(crudService, modelMapper) 
        {
            _userService = userService;
        }

        public override ActionResult FormEdit(long id)
        {
            var authProvider = (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), Engine.AppSettings["elos-auth-provider"]);

            ViewBag.AuthProvider = authProvider;

            ViewBag.OrganizationalStructures = organizationalStructureTreeExtension.GenerateTreeList();

            PortalUser entity = _crudService.GetById(id);

            PortalUserVM model = EntityModelMapper.BuildModel(entity);

            model.Phone = model.AreaCode + model.PhoneNumber;

            ViewBag.FromVisualization = false;

            if (ModelFactory != null) model = ModelFactory(model);

            return PartialView(DefaultFormView, model);
        }

        public override ActionResult FormInsert()
        {
            var authProvider = (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), Engine.AppSettings["elos-auth-provider"]);

            ViewBag.AuthProvider = authProvider;

            ViewBag.OrganizationalStructures = organizationalStructureTreeExtension.GenerateTreeList();

            return base.FormInsert();
        }
    }

    public class PortalUserVM : IModelEntity
    {
        public long? Id { get; set; }

        public string SerializedOldValue { get; set; }
    }
}
