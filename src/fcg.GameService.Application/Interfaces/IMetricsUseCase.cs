using fcg.GameService.Presentation.DTOs.Metrics;

namespace fcg.GameService.Application.Interfaces;

public interface IMetricsUseCase
{
    Task<IEnumerable<PopularGameDTO>> GetPopularGamesAsync(int limit = 10);
}
