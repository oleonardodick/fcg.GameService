namespace fcg.GameService.Presentation.DTOs.Game.Requests;

public class UpdateGameDTO
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public DateTime? ReleasedDate { get; set; }

    public ICollection<string>? Tags { get; set; }
}
