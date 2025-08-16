namespace fcg.GameService.API.DTOs.Requests;

public class UpdateGameDTO
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public DateTime? ReleasedDate { get; set; }

    public List<string>? Tags { get; set; }
}
