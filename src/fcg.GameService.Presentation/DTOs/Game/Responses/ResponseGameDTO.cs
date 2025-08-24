namespace fcg.GameService.Presentation.DTOs.Game.Responses;

public class ResponseGameDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Price { get; set; }

    public DateTime ReleasedDate { get; set; }

    public List<string> Tags { get; set; } = [];
}
