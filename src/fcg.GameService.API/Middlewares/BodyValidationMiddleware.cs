using System.Text.Json;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;

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
                await ThrowProblem("O corpo da requisição não pode ser vazio");
                return;
            }

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body.Substring(1,body.Length -2)))
            {
                await ThrowProblem("O corpo da requisição não pode ser vazio");
                return;
            }

            try
            {
                JsonDocument.Parse(body);
            }
            catch (JsonException)
            {
                await ThrowProblem("O corpo da requisição não é um JSON válido.");
                return;
            }

            context.Request.Body.Position = 0;
        }

        await _next(context);
    }

    private static Task ThrowProblem(string errorMessage)
    {
        List<ErrorDetails> error = [
            new ErrorDetails {
                Property = "Body",
                Errors = [errorMessage]
            }
        ];
        
        throw new AppValidationException(
            error
        );
    }
}
