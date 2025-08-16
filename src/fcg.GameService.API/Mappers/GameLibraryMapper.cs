using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Entities;
using Mapster;

namespace fcg.GameService.API.Mappers;

public static class GameLibraryMapper
{
    public static ResponseGameLibraryDTO FromEntityToDto(GameLibrary gamelibrary) =>
        gamelibrary.Adapt<ResponseGameLibraryDTO>();

    public static GameLibrary FromDtoToEntity(CreateGameLibraryDTO dto) =>
        dto.Adapt<GameLibrary>();
}
