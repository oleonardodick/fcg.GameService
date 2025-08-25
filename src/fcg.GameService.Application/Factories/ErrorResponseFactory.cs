using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.Application.Factories;

public class ErrorResponseFactory : IErrorResponseFactory
{
    public ProblemDetails CreateInternalServerProblem(string instance, string traceId)
    {
         return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = 500,
                Detail = "Erro interno do servidor.",
                Instance = instance,
                Extensions = {
                    ["errors"] = "Erro interno do servidor.",
                    ["traceId"] = traceId.Trim()
                }
            };
    }

    public ProblemDetails CreateNotFoundProblem(AppNotFoundException exception, string instance, string traceId)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not Found",
            Status = 404,
            Detail = exception.Message,
            Instance = instance,
            Extensions = {
                ["errors"] = exception.Message,
                ["traceId"] = traceId.Trim()
            }
        };
    }

    public ProblemDetails CreateValidationProblem(AppValidationException ex, string instance, string traceId)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Error",
            Status = 400,
            Detail = ex.Message,
            Instance = instance,
            Extensions = {
                ["errors"] = ex.Errors,
                ["traceId"] = traceId.Trim()
            }
        };
    }
}
