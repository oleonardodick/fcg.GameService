using System.Text.Json.Serialization;
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

    public BusinesRuleProblemDetails(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors;
    }

    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
