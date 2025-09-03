using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.Interfaces;

public interface ISuggestionUseCase
{
    Task<IList<ResponseGameDTO>> GetSuggestionByUserIdAsync(SuggestGameDto request);
}
