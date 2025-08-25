using fcg.GameService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.Application.Interfaces;

public interface IErrorResponseFactory
{
    ProblemDetails CreateValidationProblem(AppValidationException exception, string instance, string traceId);
    ProblemDetails CreateNotFoundProblem(AppNotFoundException exception, string instance, string traceId);
    ProblemDetails CreateInternalServerProblem(string instance, string traceId);
}
