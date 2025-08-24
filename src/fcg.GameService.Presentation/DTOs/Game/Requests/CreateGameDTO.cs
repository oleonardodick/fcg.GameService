namespace fcg.GameService.Presentation.DTOs.Game.Requests;

public class CreateGameDTO
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double Price { get; set; }

    public DateTime ReleasedDate { get; set; }

    public required List<string> Tags { get; set; } = [];
}
