using fcg.GameService.Domain.Entities;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using Mapster;

namespace fcg.GameService.Application.Mappers.Registers;

public class GameMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateGameDTO, Game>()
            .ConstructUsing(dto => new Game(
                string.Empty,
                dto.Name,
                dto.Price,
                dto.ReleasedDate,
                dto.Tags,
                dto.Description
            ));

        config.NewConfig<Game, ResponseGameDTO>();
    }
}
