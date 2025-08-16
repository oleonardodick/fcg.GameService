using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;
using Mapster;

namespace fcg.GameService.API.Mappers;

public static class GameMapper
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

    public static Game FromDtoToUpdateEntity(UpdateGameDTO dto, string id)
    {
        var entity = dto.Adapt<Game>();

        entity.Id = id;

        return entity;
    }
}
