using Hub.Application.CorporateStructure;
using Hub.Application.Models.ViewModels.Users;
using Hub.Application.Services;
using Hub.Domain.Entities.Users;
using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Web.Controllers;
using Hub.Infrastructure.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Controllers
{
    public class UserController : BaseController<PortalUser, PortalUserVM>
    {
        private readonly UserService _userService;

        public UserController(IOrchestratorService<PortalUser> crudService, IDataMapper<PortalUser, PortalUserVM> modelMapper, UserService userService) : base(crudService, modelMapper) 
        {
            _userService = userService;
        }

        //public override ActionResult FormEdit(long id)
        //{
        //    var authProvider = (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), Engine.AppSettings["auth-provider"]);

        //    ViewBag.AuthProvider = authProvider;

        //    ViewBag.OrganizationalStructures = Engine.Resolve<OrganizationalTreeManager>().GenerateTreeList();

        //    PortalUser entity = _crudService.GetById(id);

        //    PortalUserVM model = EntityModelMapper.BuildModel(entity);

        //    model.Phone = model.AreaCode + model.PhoneNumber;

        //    ViewBag.FromVisualization = false;

        //    if (ModelFactory != null) model = ModelFactory(model);

        //    return PartialView(DefaultFormView, model);
        //}

        //public override ActionResult FormInsert()
        //{
        //    var authProvider = (EPortalAuthProvider)Enum.Parse(typeof(EPortalAuthProvider), Engine.AppSettings["auth-provider"]);

        //    ViewBag.AuthProvider = authProvider;

        //    ViewBag.OrganizationalStructures = Engine.Resolve<OrganizationalTreeManager>().GenerateTreeList();

        //    return base.FormInsert();
        //}
    }
}
