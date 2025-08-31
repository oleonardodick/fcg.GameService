namespace fcg.GameService.Presentation.DTOs.GameLibrary.Responses;

public class ResponseGameLibraryDTO
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ICollection<ResponseGameAdquiredDTO> Games { get; set; } = [];
}

public class ResponseGameAdquiredDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<string> Tags { get; set; } = [];
}
