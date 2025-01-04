using AutoMapper;

namespace Hub.Infrastructure.Architecture.Mapper.Interfaces
{
    public interface IAutoMapperStartup
    {
        void RegisterMaps(IMapperConfigurationExpression cfg);
    }
}
