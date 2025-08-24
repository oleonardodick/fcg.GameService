using System.Reflection;
using Mapster;

namespace fcg.GameService.Application.Mappers;

public static class MappingConfig
{
        public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetExecutingAssembly());
    }
}
