using System.Diagnostics;
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
public class SuggestionController(
    ISuggestionUseCase suggestionUseCase,
    IAppLogger<SuggestionController> logger
) : ControllerBase
{
    private readonly ISuggestionUseCase _suggestionUseCase = suggestionUseCase;
    private readonly IAppLogger<SuggestionController> _logger = logger;

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

        _logger.LogInformation("Iniciando a busca das sugestões para o usuário usuário {userId}", request.UserId);
        var stopwatch = Stopwatch.StartNew();
        var games = await _suggestionUseCase.GetSuggestionByUserIdAsync(request);
        stopwatch.Stop();
        _logger.LogInformation("Finalizou a busca das sugestões para o usuário usuário {userId} após {duration}ms",
            request.UserId,
            stopwatch.ElapsedMilliseconds
        );

        return Ok(games);
    }
}
