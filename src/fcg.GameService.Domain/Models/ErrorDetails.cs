namespace fcg.GameService.Domain.Models;

public class ErrorDetails
{
    public string Property { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
