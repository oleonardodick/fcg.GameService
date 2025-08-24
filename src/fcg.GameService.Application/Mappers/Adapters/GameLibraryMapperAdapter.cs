using fcg.GameService.Domain.Entities;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;
using Mapster;

namespace fcg.GameService.Application.Mappers.Adapters;

public static class GameLibraryMapperAdapter
{
    public static ResponseGameLibraryDTO FromEntityToDto(GameLibrary gamelibrary) =>
        gamelibrary.Adapt<ResponseGameLibraryDTO>();

    public static GameLibrary FromDtoToEntity(CreateGameLibraryDTO dto) =>
        dto.Adapt<GameLibrary>();

    public static GameAdquired FromGameAdquiredDtoToEntity(AddGameToLibraryDTO dto) =>
        dto.Adapt<GameAdquired>();
}
