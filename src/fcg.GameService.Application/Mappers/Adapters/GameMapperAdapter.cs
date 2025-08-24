using fcg.GameService.Domain.Entities;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using Mapster;

namespace fcg.GameService.Application.Mappers.Adapters;

public class GameMapperAdapter
{
    public static ResponseGameDTO FromEntityToDto(Game game) =>
           game.Adapt<ResponseGameDTO>();

    public static IList<ResponseGameDTO> FromListEntityToListDto(IList<Game> games)
    {
        IList<ResponseGameDTO> listDto = [];

        foreach (var game in games)
        {
            var dto = game.Adapt<ResponseGameDTO>();
            listDto.Add(dto);
        }

        return listDto;
    }

    public static Game FromDtoToEntity(CreateGameDTO dto) =>
        dto.Adapt<Game>();

    public static Game FromDtoToUpdateEntity(UpdateGameDTO dto, Game game)
        => dto.Adapt(game);
}
