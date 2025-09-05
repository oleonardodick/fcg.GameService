namespace fcg.GameService.Presentation.DTOs.Game.Requests;

public class UpdateTagsDTO
{
    public ICollection<string> Tags { get; set; } = [];
}
