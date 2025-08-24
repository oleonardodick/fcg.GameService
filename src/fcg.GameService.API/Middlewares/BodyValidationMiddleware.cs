using System.Text.Json;
using fcg.GameService.Presentation.DTOs;
using fcg.GameService.Presentation.ProblemDefinitions;

namespace fcg.GameService.API.Middlewares;

public class BodyValidationMiddleware
{
    private readonly RequestDelegate _next;

    public BodyValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;

        if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsPatch(method))
        {
            context.Request.EnableBuffering();

            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
            {
                await WriteProblemDetails(context, "O corpo da requisição não pode ser vazio");
                return;
            }

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body.Substring(1,body.Length -2)))
            {
                await WriteProblemDetails(context, "O corpo da requisição não pode ser vazio");
                return;
            }

            try
            {
                JsonDocument.Parse(body);
            }
            catch (JsonException)
            {
                await WriteProblemDetails(context, "O corpo da requisição não é um JSON válido.");
                return;
            }

            context.Request.Body.Position = 0;
        }

        await _next(context);
    }

    private static async Task WriteProblemDetails(HttpContext context, string errorMessage)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var problem = new CustomValidationProblemDetails(new ErrorResponseDTO
        {
            Property = "Body",
            Errors = [errorMessage]
        });

        await context.Response.WriteAsJsonAsync(problem);
    }
}
