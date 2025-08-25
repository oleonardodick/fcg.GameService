using System.Text.Json.Serialization;
using fcg.GameService.Domain.Models;

namespace fcg.GameService.Presentation.DTOs;

public class ErrorResponseDTO
{
    [JsonPropertyName("errors")]
    public List<ErrorDetails> Errors { get; set; } = [];
}
