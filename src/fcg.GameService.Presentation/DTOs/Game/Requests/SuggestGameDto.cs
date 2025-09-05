namespace fcg.GameService.Presentation.DTOs.Game.Requests;

public class SuggestGameDto
{
    public string UserId { get; set; } = string.Empty;
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 10;

}
