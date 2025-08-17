using System.Text.Json.Serialization;
using fcg.GameService.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.ProblemsDefinitions;

public class BusinesRuleProblemDetails : ProblemDetails
{
    public BusinesRuleProblemDetails()
    {
        Title = "Um ou mais erros de validação aconteceram.";
        Type = "https://httpstatuses.com/400";
        Status = StatusCodes.Status400BadRequest;
        Detail = "A requisição contém dados inválidos.";
    }

    public BusinesRuleProblemDetails(List<ErrorResponseDTO> errors) : this()
    {
        Errors = errors;
    }

    public BusinesRuleProblemDetails(ErrorResponseDTO error) : this()
    {
        Errors = [error];
    }

    [JsonPropertyName("errors")]
    public IList<ErrorResponseDTO> Errors { get; set; } = [];
}
