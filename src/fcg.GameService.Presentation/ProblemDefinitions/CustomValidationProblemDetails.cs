using System.Text.Json.Serialization;
using fcg.GameService.Presentation.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.Presentation.ProblemDefinitions;

public class CustomValidationProblemDetails : ProblemDetails
{
    public CustomValidationProblemDetails()
    {
        Title = "Um ou mais erros de validação aconteceram.";
        Type = "https://httpstatuses.com/400";
        Status = StatusCodes.Status400BadRequest;
        Detail = "A requisição contém dados inválidos.";
    }

    public CustomValidationProblemDetails(List<ErrorResponseDTO> errors) : this()
    {
        Errors = errors;
    }

    public CustomValidationProblemDetails(ErrorResponseDTO error) : this()
    {
        Errors = [error];
    }

    [JsonPropertyName("errors")]
    public IList<ErrorResponseDTO> Errors { get; set; } = [];
}
