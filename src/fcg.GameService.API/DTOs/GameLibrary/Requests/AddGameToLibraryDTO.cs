namespace fcg.GameService.API.DTOs.GameLibrary.Requests;

public class AddGameToLibraryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
}
