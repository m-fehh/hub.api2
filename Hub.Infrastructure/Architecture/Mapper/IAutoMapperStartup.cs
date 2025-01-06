using AutoMapper;

namespace Hub.Infrastructure.Architecture.Mapper
{
    public interface IAutoMapperStartup
    {
        void RegisterMaps(IMapperConfigurationExpression cfg);
    }
}
