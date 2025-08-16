namespace fcg.GameService.API.DTOs.Requests;

public class CreateGameDTO
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public double Price { get; set; }

    public DateTime ReleasedDate { get; set; }

    public required List<string> Tags { get; set; }
}
