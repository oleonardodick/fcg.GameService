namespace fcg.GameService.Presentation.DTOs.Metrics;

public record PopularGameDTO(string GameId, string Name, int UserCount, ICollection<string> Tags);
