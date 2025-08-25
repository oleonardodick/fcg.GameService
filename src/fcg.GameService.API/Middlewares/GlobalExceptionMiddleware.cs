using System.Diagnostics;
using System.Text.Json;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppLogger<GlobalExceptionMiddleware> _logger;
    private readonly IErrorResponseFactory _errorResponseFactory;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        IAppLogger<GlobalExceptionMiddleware> logger,
        IErrorResponseFactory errorResponseFactory
    )
    {
        _next = next;
        _logger = logger;
        _errorResponseFactory = errorResponseFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var instance = context.Request.Path.Value ?? string.Empty;
        //var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var traceId = context.TraceIdentifier;

        var problemDetails = exception switch
        {
            AppValidationException validationEx => _errorResponseFactory.CreateValidationProblem(validationEx, instance, traceId),
            AppNotFoundException notFoundEx => _errorResponseFactory.CreateNotFoundProblem(notFoundEx, instance, traceId),
            Exception internalEx => HandleInternalException(internalEx, instance, traceId)
        };

        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private ProblemDetails HandleInternalException(Exception ex, string instance, string traceId)
    {
        _logger.LogError(ex, ex.Message);
        return _errorResponseFactory.CreateInternalServerProblem(instance, traceId);
    }
}
