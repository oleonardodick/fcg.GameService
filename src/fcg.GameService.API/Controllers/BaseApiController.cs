using fcg.GameService.Presentation.DTOs;
using fcg.GameService.Presentation.ProblemDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<IList<T>> Success<T>(IList<T> data) => Ok(data);
    protected ActionResult<T> Success<T>(T data) => Ok(data);

    protected ActionResult NotFound(string description)
    {
        var problemDetails = new NotFoundProblemDetails(description);
        EnrichProblemDetails(problemDetails);
        return NotFound(problemDetails);
    }

    protected ActionResult BadRequest(List<ErrorResponseDTO> errors)
    {
        var problemDetails = new CustomValidationProblemDetails(errors);
        EnrichProblemDetails(problemDetails);
        return BadRequest(problemDetails);
    }

    protected ActionResult BadRequest(ErrorResponseDTO error)
    {
        var problemDetails = new CustomValidationProblemDetails(error);
        EnrichProblemDetails(problemDetails);

        return BadRequest(problemDetails);
    }

    private void EnrichProblemDetails(ProblemDetails problemDetails)
    {
        problemDetails.Instance = $"{HttpContext.Request.Method} {HttpContext.Request.Path}";
        
        if (!problemDetails.Extensions.ContainsKey("traceId"))
            problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;
            
        if (!problemDetails.Extensions.ContainsKey("timestamp"))
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
    }
}
