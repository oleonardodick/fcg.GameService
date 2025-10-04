using fcg.GameService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers;

[Route("api/metrics")]
[ApiController]
[Produces("application/json")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsUseCase _metricsUseCase;

    public MetricsController(IMetricsUseCase metricsUseCase)
    {
        _metricsUseCase = metricsUseCase;
    }

    [HttpGet("popular-games")]
    public async Task<IActionResult> GetPopularGames([FromQuery] int limit = 10)
    {
        if (limit <= 0 || limit > 100)
        {
            return BadRequest("O limite deve estar entre 1 e 100");
        }

        var popularGames = await _metricsUseCase.GetPopularGamesAsync(limit);
        return Ok(popularGames);
    }
}
