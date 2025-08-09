using fcg.GameService.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Handlers;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public CustomExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            DataValidationException d => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = d.Error,
                Detail = d.Message,
                Type = "Bad Request"
            },
            NotFoundException n => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = n.Error,
                Detail = n.Message,
                Type = "NotFound"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro inesperado",
                Detail = "Um erro inesperado foi disparado. Entre em contato com o administrador do sistema.",
                Type = "Internal Server Error"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            }
        );
    }
}
