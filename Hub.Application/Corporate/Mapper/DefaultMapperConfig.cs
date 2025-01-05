using AutoMapper;
using Hub.Application.Models.ViewModels;
using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture.Mapper;

namespace Hub.Application.Corporate.Mapper
{
    /// <summary>
    /// Centraliza todas as configurações de mapemaneto do automapper
    /// </summary>
    public class DefaultMapperConfig : IAutoMapperStartup
    {
        /// <summary>
        /// Criação centralizada dos mapeamentos do automapper
        /// </summary>
        public void RegisterMaps(IMapperConfigurationExpression cfg)
        {

            #region DocumentType

            cfg.CreateMap<DocumentType, DocumentTypeVM>();
            cfg.CreateMap<DocumentTypeVM, DocumentType>();

            #endregion
        }
    }
}
