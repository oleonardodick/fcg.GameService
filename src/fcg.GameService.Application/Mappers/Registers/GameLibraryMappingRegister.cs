using fcg.GameService.Domain.Entities;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;
using Mapster;

namespace fcg.GameService.Application.Mappers.Registers;

public class GameLibraryMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddGameToLibraryDTO, GameAdquired>()
            .ConstructUsing(dto => new GameAdquired(
                dto.Id,
                dto.Name
            ));

        config.NewConfig<CreateGameLibraryDTO, GameLibrary>()
            .ConstructUsing(dto => new GameLibrary(
                string.Empty,
                dto.UserId,
                new List<GameAdquired>()
            ));
            
        config.NewConfig<GameLibrary, ResponseGameLibraryDTO>();
    }
}
