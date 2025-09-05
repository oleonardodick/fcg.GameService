using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class SuggestionController(ISuggestionUseCase suggestionUseCase) : ControllerBase
{
    private readonly ISuggestionUseCase _suggestionUseCase = suggestionUseCase;

    [HttpGet]
    public async Task<IActionResult> GetSuggestionByUserId([FromQuery] SuggestGameDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            List<ErrorDetails> error = [
                new ErrorDetails {
                        Property = nameof(request.UserId),
                        Errors = ["O ID deve ser informado"]
                    }
            ];

            throw new AppValidationException(
                error
            );
        }

        var games = await _suggestionUseCase.GetSuggestionByUserIdAsync(request);

        return Ok(games);
    }
}
