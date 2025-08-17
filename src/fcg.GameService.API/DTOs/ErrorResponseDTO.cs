namespace fcg.GameService.API.DTOs;

public class ErrorResponseDTO
{
    public string Property { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
