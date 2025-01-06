using AutoMapper;
using Hub.Application.Models.ViewModels;
using Hub.Domain.Entities.Logs;
using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper;
using Hub.Infrastructure.Generator;
using DEDrake;
using Hub.Infrastructure.Architecture.Logger.Enums;

namespace Hub.Application.Corporate.Mapper
{
    internal class LogMapperConfig : IAutoMapperStartup
    {
        /// <summary>
        /// Criação centralizada dos mapeamentos do automapper
        /// </summary>
        public void RegisterMaps(IMapperConfigurationExpression cfg)
        {
            #region Log

            cfg.CreateMap<Log, LogAuditVM>()
                .ForMember(dst => dst.Id, map => map.MapFrom(src => src.Id != 0 ? $"{src.Id}" : ShortGuid.NewGuid().ToString()))
                .ForMember(dst => dst.CreationUTC, map => map.MapFrom(src => src.CreateDate.ToUniversalTime()))
                //.ForMember(dst => dst.OwnerOrgStructId, map => map.MapFrom(src => src.OwnerOrgStruct == null ? null : (long?)src.OwnerOrgStruct.Id))
                //.ForMember(dst => dst.OwnerOrgStructTree, map => map.MapFrom(src => src.OwnerOrgStruct == null ? null : src.OwnerOrgStruct.Tree))
                //.ForMember(dst => dst.OwnerOrgStructDescription, map => map.MapFrom(src => src.OwnerOrgStruct == null ? null : src.OwnerOrgStruct.Description))
                .ForMember(dst => dst.CreateUserId, map => map.MapFrom(src => src.CreateUser == null ? null : (long?)src.CreateUser.Id))
                .ForMember(dst => dst.CreateUserName, map => map.MapFrom(src => src.CreateUser == null ? null : src.CreateUser.Name));


            cfg.CreateMap<LogAuditVM, LogVM>()
                //Gerar Id pois caso contrário ocorrerão ao exibir no Grid
                .ForMember(dst => dst.Id, map => map.MapFrom(src => src.GetLongId()))
                .ForMember(dst => dst.CreateDate, map => map.MapFrom(src => (src.CreationUTC ?? DateTime.UtcNow).ToLocalTime()))
                .ForMember(dst => dst.Establishment_Name, map => map.MapFrom(src => src.OwnerOrgStructDescription))
                .ForMember(dst => dst.CreateUser_Id, map => map.MapFrom(src => src.CreateUserId))
                .ForMember(dst => dst.CreateUser_Name, map => map.MapFrom(src => src.CreateUserName))
                .ForMember(dst => dst.Children, map => map.MapFrom(src => src.Fields))
                .ForMember(dst => dst.CountChildrens, map => map.MapFrom(src => src.Fields == null ? 0 : src.Fields.Count));

            #endregion

            #region LogField

            cfg.CreateMap<LogField, LogAuditFieldVM>()
                .ForMember(dst => dst.Children, map => map.MapFrom(src => src.Childs));

            cfg.CreateMap<LogAuditFieldVM, LogFieldVM>()
                //Gerar Id aleatório pois caso contrário ocorrerão ao exibir no Grid
                .ForMember(dst => dst.Id, map => map.MapFrom(src => Engine.Resolve<IRandomGeneration>().Generate(int.MaxValue)))
                .ForMember(dst => dst.CountChildrens, map => map.MapFrom(src => src.Children == null ? 0 : src.Children.Count));

            #endregion

            #region Log

            cfg.CreateMap<ILog, LogVM>()
                .Foreign(src => src.CreateUser, dest => dest.CreateUser_Id)
                .Foreign(src => src.Father, dest => dest.LogField_Id)
                .ForMember(dest => dest.ObjectName, opt => opt.MapFrom((src, d) =>
                {
                    if (src.LogType == ELogType.Audit)
                    {
                        return Engine.Get(src.ObjectName);
                    }
                    else
                    {
                        return src.ObjectName;
                    }
                }))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom((src, d) =>
                {
                    return src.CreateUser?.IpAddress;
                }));

            cfg.CreateMap<LogVM, ILog>()
                .Foreign(src => src.CreateUser_Id, dest => dest.CreateUser)
                .Foreign(src => src.LogField_Id, dest => dest.Father);

            #endregion
            #region LogField

            cfg.CreateMap<ILogField, LogFieldVM>()
                .Foreign(src => src.Log, dest => dest.Log_Id);

            cfg.CreateMap<LogFieldVM, ILogField>()
                .Foreign(src => src.Log_Id, dest => dest.Log);

            #endregion    
        }

    }
}

