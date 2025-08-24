using fcg.GameService.Application.Mappers;

namespace fcg.GameService.UnitTests.Fixtures;

public class MappingFixture
{
    public MappingFixture()
    {
        MappingConfig.RegisterMappings();
    }
}
