namespace fcg.GameService.Presentation.DTOs.GameLibrary.Requests;

public class CreateGameLibraryDTO
{
    public string UserId { get; set; } = string.Empty;
    public ICollection<AddGameToLibraryDTO> Games { get; set; } = [];
}
