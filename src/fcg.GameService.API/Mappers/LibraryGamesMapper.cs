using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.Entities;
using Mapster;

namespace fcg.GameService.API.Mappers;

public class LibraryGamesMapper
{
    public static GameAdquired FromDtoToEntity(AddGameToLibraryDTO dto) =>
        dto.Adapt<GameAdquired>();
}
