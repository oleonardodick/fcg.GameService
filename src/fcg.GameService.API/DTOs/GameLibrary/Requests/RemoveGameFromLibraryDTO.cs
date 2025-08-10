namespace fcg.GameService.API.DTOs.GameLibrary.Requests;

public class RemoveGameFromLibraryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
}
